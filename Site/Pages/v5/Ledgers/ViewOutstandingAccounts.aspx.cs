using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class ViewOutstandingAccounts : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageIcon = "iconshock-cabinet";
            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Financials, AccessType.Read);

            if (!Page.IsPostBack)
            {
                Localize();

                if (!Page.IsPostBack)
                {
                    int year = DateTime.Today.Year - 1;
                    int firstFiscalYear = CurrentOrganization.FirstFiscalYear;

                    this.DropYears.Items.Add (new ListItem (Global.Global_AsOfNow, "Now"));

                    while (year >= firstFiscalYear)
                    {
                        this.DropYears.Items.Add (new ListItem (new DateTime (year, 12, 31).ToShortDateString(),
                            year.ToString (CultureInfo.InvariantCulture)));
                        year--;
                    }

                    this.DropAccounts.Items.Add (new ListItem (Global.Financial_ExpenseClaimsLong,
                        "ExpenseClaims"));
                    this.DropAccounts.Items.Add (new ListItem (Global.Financial_CashAdvancesLong, "CashAdvances"));

                    Localize();
                }
            }
            RegisterControl (EasyUIControl.DataGrid);
        }

        private void Localize()
        {
            PageTitle = Resources.Pages.Ledgers.ViewOutstandingAccounts_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.ViewOutstandingAccounts_Info;

            this.LabelViewOutstandingAccountsHeader.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_Header;
            this.LabelGridHeaderAction.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderAction;
            this.LabelGridHeaderAmount.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderAmount;
            this.LabelGridHeaderCreatedDate.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderOpenedDate;
            this.LabelGridHeaderDescription.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderDescription;
            this.LabelGridHeaderExpectedCloseDate.Text =
                Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderExpectedCloseDate;
            this.LabelGridHeaderId.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderId;
            this.LabelGridHeaderRecipient.Text = Resources.Pages.Ledgers.ViewOutstandingAccounts_GridHeaderRecipient;
        }
    }
}