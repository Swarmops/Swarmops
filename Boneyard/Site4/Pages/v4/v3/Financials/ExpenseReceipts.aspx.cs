using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Enums;
using Activizr.Interface.Localization;
using Activizr.Logic.Structure;

public partial class Pages_Financials_ExpenseReceipts : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        ExpenseList.OrganizationId = Organization.PPSEid;
    }
}
