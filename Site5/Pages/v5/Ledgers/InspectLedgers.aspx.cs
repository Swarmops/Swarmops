using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class InspectLedgers : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read);
            this.DbVersionRequired = 0; // base schema is fine
            this.PageTitle = Resources.Pages_Ledgers.InspectLedgers_PageTitle;
            this.InfoBoxLiteral = Resources.Pages_Ledgers.InspectLedgers_Info;
            this.PageIcon = "iconshock-ledger-inspect";

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;
                int year = today.Year;
                int firstYear = CurrentOrganization.FirstFiscalYear;

                while (year >= firstYear)
                {
                    this.DropYears.Items.Add(year.ToString(CultureInfo.InvariantCulture));
                    year--;
                }

                for (int monthNumber = 1; monthNumber <= 12; monthNumber++)
                {
                    this.DropMonths.Items.Add(new ListItem(new DateTime(2014, monthNumber, 1).ToString("MMM"), monthNumber.ToString(CultureInfo.InvariantCulture))); // will autolocalize
                }

                this.DropMonths.Items.Add(new ListItem(Resources.Global.Global_Q1, "21"));  // quarters and all-year are coded as fake month numbers
                this.DropMonths.Items.Add(new ListItem(Resources.Global.Global_Q2, "22"));
                this.DropMonths.Items.Add(new ListItem(Resources.Global.Global_Q3, "23"));
                this.DropMonths.Items.Add(new ListItem(Resources.Global.Global_Q4, "24"));
                this.DropMonths.Items.Add(new ListItem(Resources.Global.Global_AllYear, "31"));

                this.DropYears.SelectedIndex = 0;
                this.DropMonths.SelectedValue = today.Month.ToString(CultureInfo.InvariantCulture);
            }

            Localize();
        }

        private void Localize()
        {
            this.LabelHeaderInspect.Text = Resources.Pages_Ledgers.InspectLedgers_Header_Inspect;
            this.LabelHeaderInspectFor.Text = Resources.Pages_Ledgers.InspectLedgers_Header_For;
            this.LabelGridHeaderAction.Text = Resources.Global.Global_Action;
            this.LabelGridHeaderBalance.Text = Resources.Global.Ledgers_Balance;
            this.LabelGridHeaderDateTime.Text = Resources.Global.Global_Timestamp;
            this.LabelGridHeaderDeltaNegative.Text = Resources.Global.Ledgers_Credit;
            this.LabelGridHeaderDeltaPositive.Text = Resources.Global.Ledgers_Debit;
            this.LabelGridHeaderDescription.Text = Resources.Global.Global_Description;
            this.LabelGridHeaderId.Text = Resources.Pages_Ledgers.InspectLedgers_TransactionId;

            this.LabelFlagNotAvailable.Text = Resources.Pages_Ledgers.InspectLedgers_FlaggingNotAvailable;
            this.LabelInspectNotAvailable.Text = Resources.Pages_Ledgers.InspectLedgers_InspectNotAvailable;
        }
    }
}