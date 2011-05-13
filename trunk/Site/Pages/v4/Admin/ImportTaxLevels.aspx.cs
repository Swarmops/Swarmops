using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_Admin_ImportTaxLevels : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
            Master.CurrentPageProhibited = true;


        this.TextTaxData.Style[HtmlTextWriterStyle.Width] = "100%";
    }

    protected void ButtonProcessData_Click(object sender, EventArgs e)
    {
        TaxLevels.ImportTaxLevels(Country.FromCode(this.DropCountries.SelectedValue), this.TextTaxData.Text);

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alldone",
                                    "alert ('The income tax levels have been imported.');",
                                    true);

        this.TextTaxData.Text = string.Empty;
    }
}
