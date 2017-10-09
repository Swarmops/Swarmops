using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class VatReport: BasicVatReport
    {
        #region Creation and Construction

        private VatReport(BasicVatReport basic): base (basic)
        {
            // private ctor prevents random instantiation
        }

        public static VatReport FromBasic(BasicVatReport basic)
        {
            // Interface to private ctor
            return new VatReport(basic);
        }

        public static VatReport FromIdentity(int vatReportId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetVatReport(vatReportId));
        }

        protected static VatReport FromIdentityAggressive(int vatReportId)
        {
            // "Open For Writing" is intentional - it bypasses the lag of replication-to-readonly
            // instances of large-deployment databases and reads at the write source

            return FromBasic(SwarmDb.GetDatabaseForWriting().GetVatReport(vatReportId));
        }

        private static VatReport CreateDbRecord (Organization organization, int year, int startMonth, int monthCount)
        {
            System.Guid guid = System.Guid.NewGuid();

            int vatReportId = SwarmDb.GetDatabaseForWriting()
                .CreateVatReport(organization.Identity, guid.ToString(), year*100 + startMonth, monthCount);
            return FromIdentityAggressive(vatReportId);
        }

        internal static VatReport Create(Organization organization, int year, int startMonth, int monthCount)
        {
            VatReport newReport = CreateDbRecord(organization, year, startMonth, monthCount);
            VatReportItems testItems = newReport.Items;
            DateTime startDate = new DateTime(year, startMonth, 1).AddYears(-1); // start a year before report starts
            DateTime endDate = new DateTime(year, startMonth, 1).AddMonths(monthCount);

            FinancialAccount vatInbound = organization.FinancialAccounts.AssetsVatInboundUnreported;
            FinancialAccount vatOutbound = organization.FinancialAccounts.DebtsVatOutboundUnreported;
            FinancialAccount sales = organization.FinancialAccounts.IncomeSales;

            FinancialAccountRows inboundRows = RowsNotInVatReport(vatInbound, endDate);
            FinancialAccountRows outboundRows = RowsNotInVatReport(vatOutbound, endDate);
            FinancialAccountRows turnoverRows = RowsNotInVatReport(sales, endDate);

            Dictionary<int, bool> transactionsIncludedLookup = new Dictionary<int, bool>();

            newReport.AddVatReportItemsFromAccountRows(inboundRows, transactionsIncludedLookup);
            newReport.AddVatReportItemsFromAccountRows(outboundRows, transactionsIncludedLookup);
            newReport.AddVatReportItemsFromAccountRows(turnoverRows, transactionsIncludedLookup);

            newReport.Release();

            // Create financial TX that moves this VAT from unreported to reported

            Int64 differenceCents = newReport.VatInboundCents - newReport.VatOutboundCents;

            if (differenceCents != 0 && newReport.VatInboundCents > 0)
            {
                // if there's anything to report

                FinancialTransaction vatReportTransaction = FinancialTransaction.Create(organization, endDate.AddDays(4).AddHours(9),
                    newReport.Description);

                if (newReport.VatInboundCents > 0)
                {
                    vatReportTransaction.AddRow(organization.FinancialAccounts.AssetsVatInboundUnreported,
                        -newReport.VatInboundCents, null);
                }
                if (newReport.VatOutboundCents > 0)
                {
                    vatReportTransaction.AddRow(organization.FinancialAccounts.DebtsVatOutboundUnreported,
                        newReport.VatOutboundCents, null);
                        // not negative, because our number is sign-different from the bookkeeping's
                }

                if (differenceCents < 0) // outbound > inbound
                {
                    vatReportTransaction.AddRow(organization.FinancialAccounts.DebtsVatOutboundReported,
                        differenceCents, null); // debt, so negative as in our variable
                }
                else // inbound > outbound
                {
                    vatReportTransaction.AddRow(organization.FinancialAccounts.AssetsVatInboundReported,
                        differenceCents, null); // asset, so positive as in our variable
                }

                vatReportTransaction.Dependency = newReport;
            }

            return newReport;
        }

        private void AddVatReportItemsFromAccountRows(FinancialAccountRows rows,
            Dictionary<int, bool> transactionsIncludedLookup)
        {
            if (rows.Count == 0)
            {
                return;
            }

            Organization organization = rows[0].Transaction.Organization; // there is always a rows[0] because check above
            int vatInboundAccountId = organization.FinancialAccounts.AssetsVatInboundUnreported.Identity;
            int vatOutboundAccountId = organization.FinancialAccounts.DebtsVatOutboundUnreported.Identity;

            Dictionary<int, bool> turnoverAccountLookup = new Dictionary<int, bool>();

            FinancialAccounts turnoverAccounts = FinancialAccounts.ForOrganization(organization,
                FinancialAccountType.Income);

            foreach (FinancialAccount turnoverAccount in turnoverAccounts)
            {
                turnoverAccountLookup[turnoverAccount.Identity] = true;
            }

            foreach (FinancialAccountRow accountRow in rows)
            {
                FinancialTransaction tx = accountRow.Transaction;

                if (!transactionsIncludedLookup.ContainsKey(tx.Identity))
                {
                    Int64 vatInbound = 0;
                    Int64 vatOutbound = 0;
                    Int64 turnOver = 0;

                    transactionsIncludedLookup[accountRow.FinancialTransactionId] = true;

                    FinancialTransactionRows txRows = accountRow.Transaction.Rows;

                    foreach (FinancialTransactionRow txRow in txRows)
                    {
                        if (txRow.FinancialAccountId == vatInboundAccountId)
                        {
                            vatInbound += txRow.AmountCents;
                        }
                        else if (txRow.FinancialAccountId == vatOutboundAccountId)
                        {
                            vatOutbound += -txRow.AmountCents;  // this is a negative, so converting to positive
                        }
                        else if (turnoverAccountLookup.ContainsKey(txRow.FinancialAccountId))
                        {
                            turnOver -= txRow.AmountCents;  // turnover accounts are sign reversed, so convert to positive
                        }
                    }

                    // Add new row to the VAT report

                    AddItem(tx, turnOver, vatInbound, vatOutbound);
                }
            }
        }

        #endregion

        public void AddItem(FinancialTransaction transaction, Int64 turnoverCents,
            Int64 vatInboundCents, Int64 vatOutboundCents)
        {
            if (!UnderConstruction)
            {
                throw new InvalidOperationException("Cannot add items once the report is released");
            }

            VatReportItem newItem = VatReportItem.Create(this, transaction, turnoverCents, vatInboundCents,
                vatOutboundCents);
        }

        public void Release()
        {
            // if we're not under construction, throw exception

            if (!this.UnderConstruction)
            {
                throw new InvalidOperationException("VAT report is already released");
            }

            // Releasing calculates sums of turnover, VAT inbound, outbound across items into report record

            SwarmDb.GetDatabaseForWriting().SetVatReportReleased(this.Identity);
            
            base.UnderConstruction = false;
            base.TurnoverCents = Items.TurnoverCents;
            base.VatInboundCents = Items.VatInboundCents;
            base.VatOutboundCents = Items.VatOutboundCents;
        }

        public VatReportItems Items
        {
            get { return VatReportItems.ForReport(this); }
        }

        private static FinancialAccountRows RowsNotInVatReport(FinancialAccount account, DateTime endDateTime)
        {
            Organization organization = account.Organization;

            FinancialAccountRows rows =
                FinancialAccountRows.FromArray(
                    SwarmDb.GetDatabaseForReading().GetAccountRowsNotInVatReport(account.Identity, endDateTime));

            return rows;
        }

        public string Description
        {
            get
            {
                switch (MonthCount)
                {
                    case 1:
                        return String.Format(Resources.Logic_Financial_VatReport.Description_SingleMonth,
                            new DateTime(YearMonthStart/100, YearMonthStart%100, 2));
                    case 12:
                        return String.Format(Resources.Logic_Financial_VatReport.Description_FullYear,
                            YearMonthStart/100);
                    default:
                        DateTime start = new DateTime(YearMonthStart/100, YearMonthStart%100, 2);

                        return String.Format(Resources.Logic_Financial_VatReport.Description_Months,
                            start, start.AddMonths(MonthCount - 1));
                }
            }
        }
    }

}
