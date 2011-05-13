using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Pirates;

public partial class Handlers_DisplayPortrait: Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int personId = Int32.Parse(Request.QueryString["PersonId"]);
        int ySize = 0;

        string ySizeString = Request.QueryString["YSize"];

        if (!String.IsNullOrEmpty(ySizeString))
        {
            ySize = Int32.Parse(ySizeString);
        }
        int cacheLength = (personId % 10) * 100 + 500;
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(cacheLength));

        Response.ContentType = "image/jpeg";

        string cacheDataKey = "PersonPhoto-" + personId.ToString() + "-" + ySize.ToString();

        System.Drawing.Image photo = (System.Drawing.Image)Cache.Get(cacheDataKey);

        if (photo == null)
        {
            photo = GetScaledPortrait (personId, ySize);

            // Cache for a different time for different people, so they don't expire
            // all at the same time

            Cache.Insert(cacheDataKey, photo, null, DateTime.Today.AddSeconds(personId+3600).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        using (Stream responseStream = Response.OutputStream)
        {
            photo.Save(responseStream, ImageFormat.Jpeg);
        }
    }

    private System.Drawing.Image GetScaledPortrait (int personId, int ySize)
    {
        // Call garbage collector to prevent Out Of Memory

        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Generate image

        int xSize = ySize*3/4;

        System.Drawing.Image unscaledPortrait = Person.FromIdentity(personId).Portrait;

        if (unscaledPortrait == null)
        {
            unscaledPortrait = System.Drawing.Image.FromFile(MapPath("~/Images/Public/Portrait-Default.png"));
        }

        if (ySize == 0)
        {
            return unscaledPortrait;
        }

        System.Drawing.Image newImage = Activizr.Logic.Support.Imagery.Resize(unscaledPortrait, xSize, ySize);

        return newImage;
    }
}
