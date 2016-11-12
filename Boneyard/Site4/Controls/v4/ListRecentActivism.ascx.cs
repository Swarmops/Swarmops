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
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

public partial class Controls_v4_ListRecentActivism : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string literal = "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"line-height:120%\" width=\"100%\">";

        ExternalActivities activities = ExternalActivities.ForOrganization(Organization.PPSE);
        activities.Sort(ExternalActivities.SortOrder.DateDescending);

        int count = 0;

        foreach (ExternalActivity activity in activities)
        {
            string description = activity.Description;

            if (description.Length > 25)
            {
                description = description.Substring(0, 22) + "...";
            }
        
            literal += "<tr><td>" + activity.DateTime.ToString("yyyy-MM-dd") + "&nbsp;</td><td>" +
                       Server.HtmlEncode(activity.Geography.Name) + "&nbsp;</td><td>" + activity.Type.ToString() +
                       "&nbsp;</td><td><a href=\"http://data.piratpartiet.se/Handlers/DisplayActivism.aspx?Id=" + activity.Identity.ToString() + "\" target=\"_blank\">" + Server.HtmlEncode(description) + "</a></td></tr>";

            if (++count > 12)
            {
                break;
            }
        }

        literal += "</table>";

        this.LiteralActivismTable.Text = literal;
    }
}
