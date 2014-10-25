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
            this.PageTitle = "Inspect Ledgers"; // TODO localize
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

                this.DropYears.SelectedIndex = 0;
                this.DropMonths.SelectedValue = today.Month.ToString(CultureInfo.InvariantCulture);
            }

            Localize();
        }

        private void Localize()
        {
            
        }
    }
}