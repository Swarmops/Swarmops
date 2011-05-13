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

public partial class Handlers_DisplayPortrait : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "image/jpeg";

        System.Drawing.Image unscaledPortrait = _currentUser.Portrait;

        if (unscaledPortrait == null)
        {
            unscaledPortrait = System.Drawing.Image.FromFile(MapPath("~/Images/Public/v4/Portrait-Default.png"));
        }

        // Here, use an extreme size just for demo purposes

        System.Drawing.Image newImage = Activizr.Logic.Support.Imagery.Resize(unscaledPortrait, 225, 300);

        using (Stream responseStream = Response.OutputStream)
        {
            newImage.Save(responseStream, ImageFormat.Jpeg);
        }
    }


}
