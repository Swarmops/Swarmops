using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Image=System.Drawing.Image;

// This form returns an image.

public partial class Forms_DisplayOutboundInvoice : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string referenceString = Request.QueryString["Reference"];
        string cultureString = Request.QueryString["Culture"];

        cultureString = cultureString.Substring(0, 2).ToLowerInvariant() + "-" +
                        cultureString.Substring(2).ToUpperInvariant();
        CultureInfo culture = new CultureInfo(cultureString);

        OutboundInvoice invoice = OutboundInvoice.FromReference(referenceString);
        Organization organization = invoice.Organization;

        Response.ContentType = "image/jpeg";
        Image form = Image.FromFile(MapPath("~/Data/ImageTemplates/PPSE-invoice-" + culture + ".png"));

        using (Graphics graphics = Graphics.FromImage(form))
        {
            StringFormat rightAlign = new StringFormat();
            rightAlign.Alignment = StringAlignment.Far;

            Font fontLarge = new Font("Liberation Mono", 12, FontStyle.Bold);
            Font fontMedium = new Font("Liberation Mono", 10, FontStyle.Bold);
            Font fontNormal = new Font("Liberation Mono", 10, FontStyle.Regular);

            Brush brushPrint = Brushes.Blue;

            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            graphics.DrawString(invoice.Amount.ToString("N2", culture), fontLarge, brushPrint, 650, 378);
            graphics.DrawString(invoice.DueDate.ToShortDateString(), fontLarge, brushPrint, 650, 445);
            graphics.DrawString(invoice.Reference, fontLarge, brushPrint, 650, 512);

            graphics.DrawString(invoice.TheirReference, fontMedium, brushPrint, 650, 657);

            graphics.DrawString(invoice.CustomerName, fontMedium, brushPrint, 1500, 378);
            graphics.DrawString(invoice.InvoiceAddressPaper, fontNormal, brushPrint, 1500, 430);

            int yPos = 960;

            foreach (OutboundInvoiceItem item in invoice.Items)
            {
                graphics.DrawString(item.Description, fontNormal, brushPrint, 104, yPos);
                graphics.DrawString(item.Amount.ToString("N2", culture), fontNormal, brushPrint, 2333, yPos, rightAlign);
                yPos += 60;
            }

            graphics.DrawString(invoice.Amount.ToString("N2", culture), fontMedium, brushPrint, 2333, 2940, rightAlign);
        }

        using (Stream responseStream = Response.OutputStream)
        {
            form.Save(responseStream, ImageFormat.Jpeg);
        }


    }


}
