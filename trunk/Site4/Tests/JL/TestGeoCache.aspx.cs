using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Structure;
using Telerik.Web.UI;
using Activizr.Logic.Pirates;

public partial class Tests_JL_TestGeoCache : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        People ppl = People.FromIdentities(Roles.GetAllUpwardRoles(38, 95));
        foreach (Person p in ppl)
        {
            Response.Write("Person:" + p.Name + ";" + p.PartyEmail + ";" + p.Country.Identity + ";<br>");
        }
    }

}
