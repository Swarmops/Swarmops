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

public partial class Pages_Public_SE_Rss_PiratpartietRss : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime now = DateTime.Now;

        MediaEntries entries = (MediaEntries) Cache.Get(cacheKey);

        if (entries == null)
        {
            entries = MediaEntries.FromBlogKeyword("Piratpartiet", now.AddDays(-7));
            Cache.Insert(cacheKey, entries, null, DateTime.UtcNow.AddMinutes (5) , System.Web.Caching.Cache.NoSlidingExpiration); // five minute cache is plenty to prevent db F5 attacks
        }
      
        Response.ContentType = "text/xml";

        XmlWriter xml = new XmlTextWriter(Response.Output);
        RssWriter rss = new RssWriter(xml);

        rss.WriteHeader("Bloggat om Piratpartiet", "http://www.piratpartiet.se", "Bloggposter som nämner Piratpartiet", null);

        foreach (MediaEntry entry in entries)
        {
            string title = entry.Title;
            if (title.Length > 30)
            {
                title = title.Substring(0, 27) + "...";
            }

            rss.WriteItem(title, string.Empty, new Uri (entry.Url), entry.DateTime);
        }

        rss.Close();
        xml.Close();
    }

    private string cacheKey = "Rss-Blogs-Keyword-Piratpartiet";
}
