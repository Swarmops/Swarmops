using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;
using Activizr.Logic.Structure;
using System.Text;

public partial class HtmlIncludes_DisplayLatestActivism : Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        int pWidth = Int32.Parse("0" + Request.QueryString["Width"]);
        int pNoOverlay = Int32.Parse("0" + Request.QueryString["NoOverlay"]);

        Organization org = Organization.FromIdentity(Int32.Parse(Request.QueryString["OrganizationId"]));
        int count = Int32.Parse(Request.QueryString["Count"]);
        string content = "";

        ExternalActivities activities = ExternalActivities.ForOrganization(org);
        activities.Sort(ExternalActivities.SortOrder.CreationDateDescending);

        StringBuilder sb = new StringBuilder();
        sb.Append("<div class='pirateActivityFrame'>");

        sb.Append("<div class='pirateActivityItems'>");
        foreach (ExternalActivity activism in activities)
        {
            count--;
            if (count < 0)
                break;
            string title = "Aktivism i " + activism.Geography.Name;
            sb.Append("<div class='pirateActivityItem'>");
            sb.Append("<h4 class='pirateActivityTitle'>" + title + "</h4>");
            sb.Append("<div class='pirateActivityItemBody'>");
            sb.Append("<img src=\"http://data.piratpartiet.se/Handlers/DisplayActivism.aspx?Width=" + pWidth + "&NoOverlay=" + pNoOverlay + "&Id=" + activism.Identity.ToString() + "\" />");
            if (pNoOverlay == 1)
            {
                sb.Append("<div class='pirateActivityItemDescr'>");
                sb.Append("" + activism.Description);
                sb.Append("</div>");
            }
            sb.Append("</div>");
            sb.Append("</div>");
        }
        sb.Append("</div>");
        sb.Append("<div class='pirateActivityRss'>");
        sb.Append("<a href=\"http://data.piratpartiet.se/Rss/Activism.aspx?OrganizationId=" + org.Identity.ToString() + "\" />RSS</a>");
        sb.Append("</div>");

        sb.Append("</div>");
        content = sb.ToString();


        Response.Write(content);


    }

}
