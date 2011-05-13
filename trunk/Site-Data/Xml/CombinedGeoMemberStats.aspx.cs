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

using Activizr.Logic.Structure;

public partial class Xml_CombinedGeoMemberStats : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";

        GeographyStatistics stats = (GeographyStatistics)Cache.Get("GeographyStatistics");

        if (stats == null)
        {
            stats = GeographyStatistics.GeneratePresent(new int[] { 1, 2 });
            Cache.Insert("GeographyStatistics", stats, null, DateTime.Today.ToUniversalTime().AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        Response.Write(stats.ToXml());
        Response.Write("\r\n");
    }
}
