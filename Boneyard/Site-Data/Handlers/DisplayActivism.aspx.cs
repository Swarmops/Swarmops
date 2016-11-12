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

public partial class Handlers_DisplayActivism : Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        int activismId = 0;

        string idString = Request.QueryString["Id"];

        if (idString == "latest")
        {
            ExternalActivities activities = ExternalActivities.ForOrganization(Organization.PPSE, ExternalActivities.SortOrder.CreationDateDescending, 1);
            activismId = activities[0].Identity;
        }
        else
        {
            activismId = Int32.Parse(idString);
        }

        int ySize = 450;
        int pWidth = Int32.Parse("0" + Request.QueryString["Width"]);
        int pNoOverlay = Int32.Parse("0" + Request.QueryString["NoOverlay"]);

        int cacheLength = (activismId % 10) * 100 + 500;
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(cacheLength));

        string photoPath = @"C:\Data\Uploads\PirateWeb\ActivismImgCache\";
        string fileName = "a" + activismId + "w" + pWidth + "o" + pNoOverlay + ".jpg";

        Response.ContentType = "image/jpeg";
        try
        {
            FileInfo cachedFile=new FileInfo(photoPath + fileName);
            ExternalActivity act = ExternalActivity.FromIdentity(activismId);
            if (cachedFile.LastWriteTime < act.CreatedDateTime)
            {
                cachedFile.Delete();
                throw new FileNotFoundException("Cached File Invalid");
            }
            Response.WriteFile(photoPath + fileName, false);
        }
        catch (FileNotFoundException)
        {
            // Call garbage collector to prevent Out Of Memory
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (System.Drawing.Image photo = GetScaledPhoto(activismId, ySize, pWidth, pNoOverlay))
            {
                if (photo != null)
                {
                    try
                    {
                        photo.Save(photoPath + fileName, ImageFormat.Jpeg);
                    }
                    catch (Exception) { }
                    using (Stream outputstream = Response.OutputStream)
                    {
                        photo.Save(outputstream, ImageFormat.Jpeg);
                    }
                }
            }
        }

    }

    private System.Drawing.Image GetScaledPhoto (int activismId, int ySize, int pWidth, int pNoOverlay)
    {
        ExternalActivity activity = ExternalActivity.FromIdentity(activismId);

        Documents docs = Documents.ForObject(activity);

        if (docs.Count == 0)
        {
            return null;
        }

        using (System.Drawing.Image unscaledImage = System.Drawing.Image.FromFile(@"C:\Data\Uploads\PirateWeb\" + docs[0].ServerFileName))
        {
            // Generate image
            float factor = 1;
            int xSize = 600;
            if (pWidth > 0 && pWidth < 1000)
            {
                factor = ((float)pWidth) / ((float)xSize);
                xSize = pWidth;
                ySize = (int)(xSize * ((float)unscaledImage.Height) / ((float)unscaledImage.Width));
            }
            string label = activity.DateTime.ToString("yyyy-MM-dd") + ": " + activity.Description;

            System.Drawing.Image newImage = GetResizedImage(xSize, label, unscaledImage, pNoOverlay);

            return newImage;

        }
    }

    private static System.Drawing.Image resizeImage (System.Drawing.Image imgToResize, int destWidth, int destHeight)
    {
        Bitmap b = new Bitmap(destWidth, destHeight);
        Graphics g = Graphics.FromImage((System.Drawing.Image)b);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        g.Dispose();

        return (System.Drawing.Image)b;
    }

    private System.Drawing.Image GetResizedImage (float targetWidth, string text, System.Drawing.Image imgOrg, int pNoOverlay)
    {

        System.Drawing.Image resized = resizeImage(imgOrg, (int)targetWidth, (int)(imgOrg.Height * (targetWidth / imgOrg.Width)));
        if (pNoOverlay == 0)
        {
            using (Graphics graphic = Graphics.FromImage(resized))
            {
                graphic.SmoothingMode = SmoothingMode.AntiAlias;
                StringFormat strFormatter = new StringFormat();

                float fontsize = (int)resized.Height / 15f;
                if (fontsize > 30) fontsize = 30;

                Font font = new Font("Liberation Sans", fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF measure = graphic.MeasureString(text, font, (int)(resized.Width - 20), strFormatter);
                float hPos = resized.Height - measure.Height - 5;

                while (hPos < resized.Height * 0.75)
                {
                    fontsize *= 0.9f;
                    font = new Font("Liberation Sans", fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
                    measure = graphic.MeasureString(text, font, (int)(resized.Width - 20), strFormatter);
                    hPos = resized.Height - measure.Height - 5;
                }

                DrawOutlinedText(graphic, strFormatter, text, font, measure, hPos);
                return resized;
            }
        }
        else
        {
            return resized;
        }
    }

    private static void DrawOutlinedText (Graphics graphic, StringFormat strFormatter, string text, Font font, SizeF measure, float hPos)
    {
        RectangleF rect = new RectangleF(new PointF(10f, hPos), measure);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        rect.Offset(-2, 0);
        graphic.DrawString(text, font, blackBrush, rect, strFormatter);
        rect.Offset(0, -2);
        graphic.DrawString(text, font, blackBrush, rect, strFormatter);
        rect.Offset(2, 0);
        graphic.DrawString(text, font, blackBrush, rect, strFormatter);
        rect.Offset(1, 0);
        graphic.DrawString(text, font, blackBrush, rect, strFormatter);
        rect.Offset(0, 2);
        graphic.DrawString(text, font, blackBrush, rect, strFormatter);
        rect.Offset(0, 1);
        graphic.DrawString(text, font, blackBrush, rect, strFormatter);
        rect.Offset(-2, -2);
        graphic.DrawString(text, font, whiteBrush, rect, strFormatter);
    }
}
