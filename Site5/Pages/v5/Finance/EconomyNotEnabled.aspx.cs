using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_v5_Finance_EconomyNotEnabled : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = "Economy Not Enabled";
        this.PageIcon = "iconshock-coins-cross";
        this.InfoBoxLiteral = "Please contact an organization administrator for assistance.";
    }
}