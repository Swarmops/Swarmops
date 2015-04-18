using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class BalanceTransactions : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read);

            RegisterControl (EasyUIControl.DataGrid);
            Localize();

        }

        private void Localize()
        {
            this.PageTitle = Resources.Pages.Ledgers.BalanceTransactions_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Ledgers.BalanceTransactions_Info;
            this.LabelHeaderUnbalancedTransactions.Text =
                Resources.Pages.Ledgers.BalanceTransactions_HeaderUnbalancedTransactions;

            this.LabelGridHeaderAccountName.Text = Resources.Pages.Ledgers.BalanceTransactions_GridHeader_AccountName;
            this.LabelGridHeaderAction.Text = Resources.Global.Global_Action;
            this.LabelGridHeaderDateTime.Text = Resources.Global.Global_Timestamp;
            this.LabelGridHeaderDelta.Text = Resources.Pages.Ledgers.BalanceTransactions_GridHeader_Amount;
            this.LabelGridHeaderId.Text = Resources.Pages.Ledgers.BalanceTransactions_GridHeader_TransactionId;
            this.LabelGridHeaderDescription.Text = Resources.Pages.Ledgers.BalanceTransactions_GridHeader_Description;

            this.LiteralModalHeader.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalHeader;
            this.LabelDoYouWishTo.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_DoYouWishTo;

            this.LabelDescribeBalance.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_DescribeBalance;
            this.LabelRadioBalance.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_RadioBalance;
            this.LiteralButtonBalance.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_ButtonBalance;

            this.LabelDescribePayout.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_DescribePayout;
            this.LabelRadioPayout.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_RadioPayout;
            this.LiteralButtonPayout.Text = Resources.Pages.Ledgers.BalanceTransactions_ModalDialog_ButtonPayout;
        }


        [Serializable]
        public class TransactionMatchabilityData
        {
            public string TransactionDate { get; set; }
            public string DifferingAmount { get; set; }
            public DropdownOption[] OpenPayoutData { get; set; }
        }

        [Serializable]
        public class DropdownOption
        {
            // Fields are lowercase for immediate usability in JavaScript without conversion.
            // ReSharper disable InconsistentNaming
            public string id;
            public string text;
            public string group;
            // ReSharper restore InconsistentNaming
        }


        [WebMethod]
        public static TransactionMatchabilityData GetTransactionMatchability (int transactionId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                    AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity (transactionId);
            if (transaction.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            TransactionMatchabilityData result = new TransactionMatchabilityData();

            result.TransactionDate = transaction.DateTime.ToString ("yyyy-MMM-dd HH:mm");
            result.DifferingAmount = String.Format ("{0} {1:+#,#.00;−#,#.00;0}",
                // this is a UNICODE MINUS (U+2212), not the hyphen on the keyboard
                authData.CurrentOrganization.Currency.DisplayCode, transaction.Rows.AmountCentsTotal/100.0);

            result.OpenPayoutData = GetOpenPayoutData (transaction);

            return result;
        }

        private static DropdownOption[] GetOpenPayoutData(FinancialTransaction transaction)
        {
            DateTime transactionDateTime = transaction.DateTime;
            Int64 matchAmount = transaction.Rows.AmountCentsTotal;

            List<DropdownOption> result = new List<DropdownOption>();

            Payouts openPayouts = Payouts.ForOrganization (transaction.Organization);

            foreach (Payout payout in openPayouts)
            {
                if (payout.AmountCents == -matchAmount)
                {
                    string description = String.Format (Resources.Pages.Ledgers.BalanceTransactions_PayoutMatch, payout.Identity,
                        payout.ExpectedTransactionDate, payout.Recipient, payout.Organization.Currency.DisplayCode, payout.AmountCents / 100.0,
                        payout.Specification);

                    result.Add(new DropdownOption
                    {
                        id = payout.Identity.ToString(CultureInfo.InvariantCulture),
                        @group = Resources.Pages.Ledgers.BalanceTransactions_ExactMatches,
                        text = description
                    });
                }
            }

            return result.ToArray();
        }

        [WebMethod]
        public static void BalanceTransactionManually(int transactionId, int accountId)
        {
            if (transactionId == 0 || accountId == 0)
            {
                return;
            }

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess(new Access(authData.CurrentOrganization,
                    AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);
            FinancialAccount account = FinancialAccount.FromIdentity(accountId);
            if (transaction.OrganizationId != authData.CurrentOrganization.Identity || account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            transaction.AddRow (account, -transaction.Rows.AmountCentsTotal, authData.CurrentUser);
        }



        [WebMethod]
        public static void MatchTransactionOpenPayout (int transactionId, int payoutId)
        {
            if (transactionId == 0 || payoutId == 0)
            {
                return;
            }

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess(new Access(authData.CurrentOrganization,
                    AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);
            Payout payout = Payout.FromIdentity (payoutId);
            if (transaction.OrganizationId != authData.CurrentOrganization.Identity || payout.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            if (transaction.Rows.AmountCentsTotal != -payout.AmountCents)
            {
                throw new InvalidOperationException();
            }

            payout.BindToTransactionAndClose (transaction, authData.CurrentUser);

        }
    }
}