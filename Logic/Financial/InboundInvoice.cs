using System;
using System.Collections.Generic;
using NBitcoin;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class InboundInvoice : BasicInboundInvoice, IApprovable, IPayable
    {
        private InboundInvoice (BasicInboundInvoice basicInstance) :
            base (basicInstance)
        {
            // constructor from basic type
        }

        public Documents Documents
        {
            get { return Documents.ForObject (this); }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (base.Open != value)
                {
                    SwarmDb.GetDatabaseForWriting().SetInboundInvoiceOpen (Identity, value);
                    base.Open = value;
                }
            }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity (OrganizationId); }
        }


        public new int OrganizationSequenceId
        {
            get
            {
                if (base.OrganizationSequenceId == 0)
                {
                    // This case is for legacy installations before DbVersion 41, when
                    // OrganizationSequenceId was added for each new invoice

                    SwarmDb db = SwarmDb.GetDatabaseForWriting();
                    base.OrganizationSequenceId = db.SetInboundInvoiceSequence(this.Identity);
                    return base.OrganizationSequenceId;
                }

                return base.OrganizationSequenceId;
            }
        }


        public decimal Amount
        {
            get { return base.AmountCents/100.0m; }
        }


        public new DateTime DueDate
        {
            get { return base.DueDate; }
            set
            {
                if (value != base.DueDate)
                {
                    SwarmDb.GetDatabaseForWriting().SetInboundInvoiceDueDate (Identity, value);
                    base.DueDate = value;
                }
            }
        }

        public FinancialTransaction FinancialTransaction
        {
            get
            {
                FinancialTransactions transactions = FinancialTransactions.ForDependentObject (this);

                if (transactions.Count == 0)
                {
                    return null; // not possible from July 26 onwards, but some grandfather work
                }

                if (transactions.Count == 1)
                {
                    return transactions[0];
                }

                throw new InvalidOperationException ("It appears inbound invoice #" + Identity +
                                                     " has multiple dependent financial transactions. This is an invalid state.");
            }
        }

        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject (this); }
        }


        public string Description
        {
            get
            {
                return
                    ObjectOptionalData.ForObject (this).GetOptionalDataString (
                        ObjectOptionalDataType.InboundInvoiceDescription);
            }

            set
            {
                ObjectOptionalData.ForObject (this)
                    .SetOptionalDataString (ObjectOptionalDataType.InboundInvoiceDescription, value);
            }
        }

        public Int64 ExactBitcoinAmountSatoshis
        {
            get
            {
                return
                    ObjectOptionalData.ForObject(this).GetOptionalDataInt64(
                        ObjectOptionalDataType.ExactBitcoinAmountSatoshis);
            }
            set
            {
                ObjectOptionalData.ForObject(this)
                    .SetOptionalDataInt64(ObjectOptionalDataType.ExactBitcoinAmountSatoshis, value);
            }

        }

        #region IApprovable Members

        public void Approve (Person approvingPerson)
        {
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceAttested (Identity, true);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Approval,
                FinancialDependencyType.InboundInvoice, Identity,
                DateTime.UtcNow, approvingPerson.Identity, BudgetAmountCents);
            base.Attested = true;

            UpdateTransaction(approvingPerson); // will re-enable the tx if denied and reopened earlier
        }

        public void RetractApproval (Person retractingPerson)
        {
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceAttested (Identity, false);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.UndoApproval,
                FinancialDependencyType.InboundInvoice, Identity,
                DateTime.UtcNow, retractingPerson.Identity, BudgetAmountCents);
            base.Attested = false;
        }

        public void DenyApproval (Person denyingPerson, string reason)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Kill,
                FinancialDependencyType.InboundInvoice, Identity,
                DateTime.UtcNow, denyingPerson.Identity, (double)Amount);
            Attested = false;
            Open = false;

            UpdateTransaction(denyingPerson); // will zero out the tx
        }

        #endregion

        public FinancialAccount Budget
        {
            get
            {
                if (base.BudgetId == 0)
                {
                    return null;
                }

                return FinancialAccount.FromIdentity (base.BudgetId);
            }
        }

        public static InboundInvoice Create (Organization organization, DateTime dueDate, Int64 amountCents,
            Int64 vatCents, FinancialAccount budget, string supplier, string description, string payToAccount, string ocr,
            string invoiceReference, Person creatingPerson)
        {
            InboundInvoice newInvoice = FromIdentity (SwarmDb.GetDatabaseForWriting().
                CreateInboundInvoice (organization.Identity, dueDate, budget.Identity,
                    supplier, payToAccount, ocr,
                    invoiceReference, amountCents, creatingPerson.Identity));

            newInvoice.Description = description; // Not in original schema; not cause for schema update

            if (vatCents > 0)
            {
                newInvoice.VatCents = vatCents;
            }

            // Create a corresponding financial transaction with rows

            FinancialTransaction transaction =
                FinancialTransaction.Create (organization.Identity, DateTime.UtcNow,
                    "Invoice #" + newInvoice.OrganizationSequenceId + " from " + supplier);

            transaction.AddRow (organization.FinancialAccounts.DebtsInboundInvoices, -amountCents, creatingPerson);
            if (vatCents > 0)
            {
                transaction.AddRow(organization.FinancialAccounts.AssetsVatInboundUnreported, vatCents,
                    creatingPerson);
                transaction.AddRow(budget, amountCents - vatCents, creatingPerson);
            }
            else
            {
                transaction.AddRow(budget, amountCents, creatingPerson);
            }


            // Make the transaction dependent on the inbound invoice

            transaction.Dependency = newInvoice;

            // Create notification (slightly misplaced logic, but this is failsafest place)

            OutboundComm.CreateNotificationApprovalNeeded (budget, creatingPerson, supplier, (amountCents-vatCents)/100.0,
                description, NotificationResource.InboundInvoice_Created);
            // Slightly misplaced logic, but failsafer here
            SwarmopsLogEntry.Create (creatingPerson,
                new InboundInvoiceCreatedLogEntry (creatingPerson, supplier, description, amountCents/100.0, budget),
                newInvoice);

            // Clear a cache
            FinancialAccount.ClearApprovalAdjustmentsCache(organization);

            return newInvoice;
        }

        /// <summary>
        /// The amount that charges the budget (invoice total minus inbound VAT)
        /// </summary>
        public Int64 BudgetAmountCents
        {
            get { return this.AmountCents - this.VatCents; }
        }


        public static InboundInvoice FromBasic (BasicInboundInvoice basic)
        {
            return new InboundInvoice (basic);
        }

        public static InboundInvoice FromIdentity (int inboundInvoiceId)
        {
            return new InboundInvoice (SwarmDb.GetDatabaseForReading().GetInboundInvoice (inboundInvoiceId));
        }


        public void SetBudget (FinancialAccount budget, Person settingPerson)
        {
            base.BudgetId = budget.Identity;
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceBudget (Identity, budget.Identity);
            UpdateTransaction (settingPerson);
        }


        public void SetAmountCents (Int64 amountCents, Person settingPerson)
        {
            base.AmountCents = amountCents;
            SwarmDb.GetDatabaseForWriting().SetInboundInvoiceAmount (Identity, amountCents);
            UpdateTransaction (settingPerson);
        }


        public new Int64 VatCents
        {
            get { return base.VatCents; }
            internal set
            {
                base.VatCents = value;
                SwarmDb.GetDatabaseForWriting().SetInboundInvoiceVatCents(this.Identity, value);
            }
        }


        private void UpdateTransaction (Person updatingPerson)
        {
            Dictionary<int, Int64> nominalTransaction = new Dictionary<int, Int64>();

            // Create an image of what the transaction SHOULD look like with changes.

            if (Attested || Open)
            {
                // ...only holds values if not closed as invalid...

                nominalTransaction[Organization.FinancialAccounts.DebtsInboundInvoices.Identity] = -AmountCents;

                if (this.Organization.VatEnabled)
                {
                    nominalTransaction[BudgetId] = AmountCents - VatCents;
                    nominalTransaction[Organization.FinancialAccounts.AssetsVatInboundUnreported.Identity] = VatCents;
                }
                else
                {
                    nominalTransaction[BudgetId] = AmountCents;
                }
            }

            FinancialTransaction.RecalculateTransaction (nominalTransaction, updatingPerson);
        }

        public bool PaidOut   // IPayable implementation
        {
            get { return (!Open && Attested); }
            set
            {
                if (Attested)
                {
                    Open = false;
                    return;
                }

                throw new InvalidOperationException("Can't set PaidOut if Attested is false");
            }
        }

        public string DisplayNativeAmount
        {
            get
            {
                if (HasNativeCurrency)
                {
                    Money money = NativeCurrencyAmount;
                    return money.Currency.DisplayCode + " " + (money.Cents/100.0).ToString("N2");
                }
                else
                {
                    return (AmountCents/100.0).ToString("N2");
                }
            }
        }

        public bool HasNativeCurrency
        {
            get
            {
                return
                    ObjectOptionalData.ForObject(this)
                        .GetOptionalDataString(ObjectOptionalDataType.NativeCurrencyCode)
                        .Length > 0;
            }
        }

        public Money NativeCurrencyAmount
        {
            get
            {
                ObjectOptionalData optionalData = ObjectOptionalData.ForObject(this);

                return new Money(optionalData.GetOptionalDataInt64(ObjectOptionalDataType.NativeCurrencyAmountCents), Currency.FromCode(optionalData.GetOptionalDataString(ObjectOptionalDataType.NativeCurrencyCode)));
            }
            set
            {
                ObjectOptionalData optionalData = ObjectOptionalData.ForObject(this);

                optionalData.SetOptionalDataInt64(ObjectOptionalDataType.NativeCurrencyAmountCents, value.Cents);
                optionalData.SetOptionalDataString(ObjectOptionalDataType.NativeCurrencyCode, value.Currency.Code);
            }
        }
    }
}