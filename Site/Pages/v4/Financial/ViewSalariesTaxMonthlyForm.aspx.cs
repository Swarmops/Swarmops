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

public partial class Pages_v4_Financial_ViewSalariesTaxMonthlyForm : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string monthString = Request.QueryString["Month"];

        if (String.IsNullOrEmpty(monthString))
        {
            monthString = "201004";
        }

        int year = Int32.Parse(monthString.Substring(0, 4));
        int month = Int32.Parse(monthString.Substring(4));  // never mind input checking
        Organization organization = Organization.PPSE;

        monthString = new DateTime(year, month, 1).ToString("MMMM yyyy", new CultureInfo("sv-SE"));

        SalaryTaxData data = GetSalaryData(year, month, organization);

        Response.ContentType = "image/jpeg";

        Image form = Image.FromFile(MapPath("/Data/ImageTemplates/salarytaxes-se.png"));

        using (Graphics graphics = Graphics.FromImage(form))
        {
            StringFormat rightAlign = new StringFormat();
            rightAlign.Alignment = StringAlignment.Far;

            Font fontHandwriting = new Font("Courier New", 64, FontStyle.Bold);
            Font fontPreprinted = new Font("Courier New", 30, FontStyle.Bold);

            Brush brushHandwriting = Brushes.Blue;
            Brush brushPreprinted = Brushes.Red;

            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            graphics.DrawString(organization.Name, fontPreprinted, brushPreprinted, 150, 170);
            graphics.DrawString(monthString, fontPreprinted, brushPreprinted, 680, 292);
            graphics.DrawString(monthString, fontPreprinted, brushPreprinted, 620, 1460);
            graphics.DrawString(String.Format("{0,4}-{1:D2}-{2:D2}", year, month+1, 12), fontPreprinted, brushPreprinted, 820, 160);
            graphics.DrawString("802430-4514", fontPreprinted, brushPreprinted, 1110, 160);

            graphics.DrawString(data.SalaryTotal.ToString("F0"), fontHandwriting, brushHandwriting, 792, 358, rightAlign); // Salary total sub
            graphics.DrawString(data.SalaryTotal.ToString("F0"), fontHandwriting, brushHandwriting, 792, 558, rightAlign); // Salary total total
            graphics.DrawString(data.SalaryMain.ToString("F0"), fontHandwriting, brushHandwriting, 792, 692, rightAlign); // Salary age main
            graphics.DrawString(data.SalaryTotal.ToString("F0"), fontHandwriting, brushHandwriting, 792, 1524, rightAlign); // Salary total again
            graphics.DrawString(data.SalaryTotal.ToString("F0"), fontHandwriting, brushHandwriting, 792, 1725, rightAlign); // Salary total again
            graphics.DrawString(data.TaxAdditiveMain.ToString("F0"), fontHandwriting, brushHandwriting, 1510, 692, rightAlign); // Emp fee main
            graphics.DrawString(data.TaxAdditiveTotal.ToString("F0"), fontHandwriting, brushHandwriting, 1510, 1326, rightAlign); // Emp fee total
            graphics.DrawString(data.TaxSubtractive.ToString("F0"), fontHandwriting, brushHandwriting, 1510, 1525, rightAlign); // Deducted main
            graphics.DrawString(data.TaxSubtractive.ToString("F0"), fontHandwriting, brushHandwriting, 1510, 1726, rightAlign); // Deducted total
            graphics.DrawString(data.TaxTotal.ToString("F0"), fontHandwriting, brushHandwriting, 1510, 1793, rightAlign); // Tax cost total
        }

        using (Stream responseStream = Response.OutputStream)
        {
            form.Save(responseStream, ImageFormat.Jpeg);
        }


    }

    private class SalaryTaxData
    {
        public double SalaryTotal;
        public double SalaryMain;
        public double TaxSubtractive;
        public double TaxAdditiveMain;
        public double TaxAdditiveTotal;
        public double TaxTotal;
    }

    private SalaryTaxData GetSalaryData (int year, int month, Organization organization)
    {
        SalaryTaxData result = new SalaryTaxData();

        Salaries salaries = Salaries.ForOrganization(organization, true);

        foreach (Salary salary in salaries)
        {
            if (salary.PayoutDate.Year == year && salary.PayoutDate.Month == month)
            {
                result.SalaryTotal += salary.GrossSalaryCents / 100.0;
                result.SalaryMain += salary.GrossSalaryCents / 100.0;
                result.TaxSubtractive += salary.SubtractiveTaxCents / 100.0;
                result.TaxAdditiveMain += salary.AdditiveTaxCents / 100.0;
                result.TaxAdditiveTotal += salary.AdditiveTaxCents / 100.0;
                result.TaxTotal += salary.TaxTotalCents / 100.0;
            }
        }

        return result;
    }

}
