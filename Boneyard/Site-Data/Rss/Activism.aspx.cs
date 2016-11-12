using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Interface;
using Activizr.Logic.Media;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Rss_Activism : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime now = DateTime.Now;
        Organization org = Organization.FromIdentity(Int32.Parse(Request.QueryString["OrganizationId"]));

        ExternalActivities activities = ExternalActivities.ForOrganization(org);
        activities.Sort(ExternalActivities.SortOrder.DateDescending);

        Response.ContentType = "text/xml";

        XmlWriter xml = new XmlTextWriter(Response.Output);
        RssWriter rss = new RssWriter(xml);

        rss.WriteHeader("Aktivism - " + org.Name, "http://www.piratpartiet.se", "Aktivism för Piratpartiet", null);

        foreach (ExternalActivity activism in activities)
        {
            string title = "Aktivism i " + activism.Geography.Name;
            if (title.Length > 60)
            {
                title = title.Substring(0, 57) + "...";
            }

            rss.WriteItem(title, "<img src=\"http://data.piratpartiet.se/Handlers/DisplayActivism.aspx?Id=" + activism.Identity.ToString() + "\" />", new Uri("http://data.piratpartiet.se/Handlers/DisplayActivism.aspx?Id=" + activism.Identity), activism.CreatedDateTime);
        }

        rss.Close();
        xml.Close();
    }
}
