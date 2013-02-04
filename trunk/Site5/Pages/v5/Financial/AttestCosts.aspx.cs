using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_v5_Finance_AttestCosts : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.CurrentOrganization.IsEconomyEnabled)
        {
            Response.Redirect("/Pages/v5/Finance/EconomyNotEnabled.aspx", true);
            return;
        }

        this.PageIcon = "iconshock-stamped-paper";

        if (!Page.IsPostBack)
        {
            Localize();
        }
    }


    private void Localize()
    {
        this.PageTitle = Resources.Pages.Finance.AttestCosts_PageTitle;
        this.InfoBoxLiteral = Resources.Pages.Finance.AttestCosts_Info;
    }
}