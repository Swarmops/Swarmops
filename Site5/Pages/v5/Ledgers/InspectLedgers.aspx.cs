using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class InspectLedgers : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read);
            DbVersionRequired = 0; // base schema is fine
            PageTitle = Resources.Pages.Ledgers.InspectLedgers_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.InspectLedgers_Info;
            PageIcon = "iconshock-ledger-inspect";

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;
                int year = today.Year;
                int firstYear = CurrentOrganization.FirstFiscalYear;

                while (year >= firstYear)
                {
                    this.DropYears.Items.Add (year.ToString (CultureInfo.InvariantCulture));
                    year--;
                }

                for (int monthNumber = 1; monthNumber <= 12; monthNumber++)
                {
                    this.DropMonths.Items.Add (new ListItem (new DateTime (2014, monthNumber, 1).ToString ("MMM"),
                        monthNumber.ToString (CultureInfo.InvariantCulture))); // will autolocalize
                }

                this.DropMonths.Items.Add (new ListItem (Global.Global_Q1, "21"));
                // quarters and all-year are coded as fake month numbers
                this.DropMonths.Items.Add (new ListItem (Global.Global_Q2, "22"));
                this.DropMonths.Items.Add (new ListItem (Global.Global_Q3, "23"));
                this.DropMonths.Items.Add (new ListItem (Global.Global_Q4, "24"));
                this.DropMonths.Items.Add (new ListItem (Global.Global_AllYear, "31"));

                this.DropYears.SelectedIndex = 0;
                this.DropMonths.SelectedValue = today.Month.ToString (CultureInfo.InvariantCulture);
            }

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);

            Localize();
        }

        private void Localize()
        {
            this.LabelHeaderInspect.Text = Resources.Pages.Ledgers.InspectLedgers_Header_Inspect;
            this.LabelHeaderInspectFor.Text = Resources.Pages.Ledgers.InspectLedgers_Header_For;
            this.LabelGridHeaderAction.Text = Global.Global_Action;
            this.LabelGridHeaderBalance.Text = Global.Ledgers_Balance;
            this.LabelGridHeaderDateTime.Text = Global.Global_Timestamp;
            this.LabelGridHeaderDeltaNegative.Text = Global.Ledgers_Credit;
            this.LabelGridHeaderDeltaPositive.Text = Global.Ledgers_Debit;
            this.LabelGridHeaderDescription.Text = Global.Global_Description;
            this.LabelGridHeaderId.Text = Resources.Pages.Ledgers.InspectLedgers_TransactionId;

            this.LabelFlagNotAvailable.Text = Resources.Pages.Ledgers.InspectLedgers_FlaggingNotAvailable;
            this.LabelInspectNotAvailable.Text = Resources.Pages.Ledgers.InspectLedgers_InspectNotAvailable;
        }
    }
}