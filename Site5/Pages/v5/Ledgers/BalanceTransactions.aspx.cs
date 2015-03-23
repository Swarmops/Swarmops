using System;
using System.Collections.Generic;
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


        public class TransactionMatchabilityData
        {
            public string DifferingAmount { get; set; }
            public string OpenPayoutDropDownJsonData { get; set; }
        }

        [WebMethod]
        public static TransactionMatchabilityData GetTransactionMatchability (int transactionId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization,
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

            result.DifferingAmount = String.Format ("{0} {1:+#,#.00;−#,#.00;0}",
                // this is a UNICODE MINUS (U+2212), not the hyphen on the keyboard
                authData.CurrentOrganization.Currency.DisplayCode, transaction.Rows.AmountCentsTotal/100.0);

            return result;
        }
    }
}