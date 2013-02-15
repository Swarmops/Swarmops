using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class ViewOutstandingAccounts : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-cabinet";

            if (!Page.IsPostBack)
            {
                Localize();

                if (!Page.IsPostBack)
                {
                    int year = DateTime.Today.Year;
                    int firstFiscalYear = CurrentOrganization.FirstFiscalYear;

                    this.DropYears.Items.Add(new ListItem("XYZ Today", "Now"));

                    while (year >= firstFiscalYear)
                    {
                        this.DropYears.Items.Add(new ListItem(new DateTime(year, 12, 31).ToShortDateString(), year.ToString(CultureInfo.InvariantCulture)));
                        year--;
                    }

                    this.DropAccounts.Items.Add(new ListItem(Resources.Global.Financial_ExpenseClaimsLong, "Expenses"));

                    Localize();
                }

            }
        }

        private void Localize()
        {
            this.PageTitle = Resources.Pages.Ledgers.ViewOutstandingAccounts_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Ledgers.ViewOutstandingAccounts_Info;
        }
    }
}