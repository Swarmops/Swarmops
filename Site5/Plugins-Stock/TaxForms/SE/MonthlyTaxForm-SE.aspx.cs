using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;

namespace Swarmops.Plugins.Stock.TaxForms
{
    public partial class MonthlyTaxFormSE : DataV5Base
    {
        private Font _fontHandwriting;
        private Brush _brushHandwriting;

        protected void Page_Load (object sender, EventArgs e)
        {
            string monthString = Request.QueryString["YearMonth"];
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Organization organization = authData.CurrentOrganization;

            int year = Int32.Parse (monthString.Substring (0, 4));
            int month = Int32.Parse (monthString.Substring (4)); // never mind input checking

            int[] yearBreakpoints = { 1938, year-65, year-26 };
            double[] taxRates = {10.21, 15.49, 31.42}; // will need to adapt this when some of the rates change

            SalaryTaxData data = GetSalaryData (year, month, organization, yearBreakpoints);

            Response.ContentType = "image/png";

            int formVersion = 2015;
            if (year < 2015)
            {
                formVersion = 2010;
            }

            Dictionary<int, Dictionary<GraphicsElement, int>> coord =
                new Dictionary<int, Dictionary<GraphicsElement, int>>();

            // Coordinates for various elements on the form versions

            coord[2010] = new Dictionary<GraphicsElement, int>();
            coord[2015] = new Dictionary<GraphicsElement, int>();


            Image form = Image.FromFile (MapPath (".") + "/MonthlyTaxForm-SE-" + formVersion + ".png");  // the "." says "in same folder as this file"

            CultureInfo swedishCulture = CultureInfo.CreateSpecificCulture ("sv-SE");
            monthString = new DateTime(year, month, 1).ToString("MMMM yyyy", swedishCulture);

            string orgNumber = "xxxxxx-xxxx";
            string sansFontName = "Liberation Sans";
            string courierFontName = "Liberation Mono";
            int smallSize = 18;
            int regularSize = 24;
            int handWriteSize = 40;

            if (Debugger.IsAttached)
            {
                sansFontName = "Arial"; // yeahyeah...
                courierFontName = "Courier New";
                orgNumber = "DEBUGGER ATTACHED";
                smallSize = 24;
                regularSize = 30;
                handWriteSize = 52;
            }

            using (Graphics graphics = Graphics.FromImage (form))
            {
                StringFormat rightAlign = new StringFormat();
                rightAlign.Alignment = StringAlignment.Far;

                _fontHandwriting = new Font (sansFontName, handWriteSize, FontStyle.Regular);
                Font fontPreprinted = new Font (courierFontName, regularSize, FontStyle.Bold);
                Font fontPreprintedSmall = new Font (courierFontName, smallSize, FontStyle.Bold);
                Font fontPreprintedSans = new Font (sansFontName, regularSize, FontStyle.Bold);

                _brushHandwriting = Brushes.Blue;
                Brush brushPreprinted = Brushes.Red;
                Brush brushPreprintedDiscreet = Brushes.DarkRed;

                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw the header and monthname - things to pay attention to

                graphics.DrawString (organization.Name, fontPreprinted, brushPreprinted, 150, 170);

                if (year <= 2014)
                {
                    graphics.DrawString (monthString, fontPreprinted, brushPreprinted, 680, 292);
                    graphics.DrawString (monthString, fontPreprinted, brushPreprinted, 620, 1460);
                    graphics.DrawString (String.Format ("{0,4}-{1:D2}-{2:D2}", year, month + 1, 12), fontPreprinted,
                        brushPreprinted, 820, 160);
                    graphics.DrawString (orgNumber, fontPreprinted, brushPreprinted, 1110, 160);
                }
                else // the latest revision of the tax form
                {
                    graphics.DrawString (monthString, fontPreprintedSans, brushPreprinted, 670, 287);
                    graphics.DrawString (monthString, fontPreprintedSans, brushPreprinted, 610, 1455);
                    graphics.DrawString (String.Format ("{0,4}-{1:D2}-{2:D2}", year, month + 1, 12), fontPreprinted,
                        brushPreprinted, 820, 160);
                    graphics.DrawString (orgNumber, fontPreprinted, brushPreprinted, 1110, 160);
                }

                // Draw the years and tax rates and other on-form constants more discreetly

                graphics.DrawString (taxRates[2].ToString ("F2", swedishCulture), fontPreprintedSmall,
                    brushPreprintedDiscreet, 840, 729);
                graphics.DrawString (taxRates[1].ToString ("F2", swedishCulture), fontPreprintedSmall,
                    brushPreprintedDiscreet, 840, 796);
                graphics.DrawString (taxRates[0].ToString ("F2", swedishCulture), fontPreprintedSmall,
                    brushPreprintedDiscreet, 840, 863);

                graphics.DrawString (string.Format ("{0} - {1}", yearBreakpoints[1], yearBreakpoints[2] - 1),
                    fontPreprintedSmall, brushPreprintedDiscreet, 190, 729);
                graphics.DrawString (string.Format ("{0} -", yearBreakpoints[2] - 1),
                    fontPreprintedSmall, brushPreprintedDiscreet, 190, 796);
                graphics.DrawString (string.Format ("{0}", yearBreakpoints[1] - 1),
                    fontPreprintedSmall, brushPreprintedDiscreet, 250, 859);

                // Draw the actual numbers

                graphics.DrawString (data.SalaryTotal.ToString ("F0"), _fontHandwriting, _brushHandwriting, 792, 358,
                    rightAlign); // Salary total sub
                graphics.DrawString (data.SalaryTotal.ToString ("F0"), _fontHandwriting, _brushHandwriting, 792, 558,
                    rightAlign); // Salary total total

                for (int ageBracket = 0; ageBracket <= 2; ageBracket++)
                {
                    if (data.Salary[ageBracket] > 0.0)
                    {
                        graphics.DrawString (data.Salary[ageBracket].ToString ("F0"), _fontHandwriting, _brushHandwriting,
                            792, 692 + ageBracket*67, rightAlign); // Salary
                        graphics.DrawString (data.TaxAdditive[ageBracket].ToString ("F0"), _fontHandwriting,
                            _brushHandwriting, 1510, 692 + ageBracket*67, rightAlign); // Employer's fee
                    }
                }

                graphics.DrawString(data.SalaryTotal.ToString("F0"), _fontHandwriting, _brushHandwriting, 792, 1524,
                    rightAlign); // Salary total again
                graphics.DrawString (data.SalaryTotal.ToString ("F0"), _fontHandwriting, _brushHandwriting, 792, 1725,
                    rightAlign); // Salary total again
                graphics.DrawString(data.TaxAdditiveTotal.ToString("F0"), _fontHandwriting, _brushHandwriting, 1510,
                    1326, rightAlign); // Emp fee total
                graphics.DrawString (data.TaxSubtractiveTotal.ToString ("F0"), _fontHandwriting, _brushHandwriting, 1510, 1525,
                    rightAlign); // Deducted main
                graphics.DrawString (data.TaxSubtractiveTotal.ToString ("F0"), _fontHandwriting, _brushHandwriting, 1510, 1726,
                    rightAlign); // Deducted total

                DrawWrittenNumber (data.TaxTotal, 1485, 1793, graphics);
            }

            using (Stream responseStream = Response.OutputStream)
            {
                form.Save (responseStream, ImageFormat.Png);
            }

        }


        private void DrawWrittenNumber (double number, int x, int y, Graphics graphics)
        {
            // This inserts spaces between every other character
            string numberString = number.ToString ("F0");
            char space = ' ';
            string drawString = string.Empty + numberString[0];

            for (int loop = 1; loop < numberString.Length; loop++)
            {
                drawString += space;
                drawString += numberString[loop];
            }

            StringFormat rightAlign = new StringFormat();
            rightAlign.Alignment = StringAlignment.Far;

            graphics.DrawString (drawString, _fontHandwriting, _brushHandwriting, x, y, rightAlign);

        }


        private class SalaryTaxData
        {
            public SalaryTaxData()
            {
                Salary = new double[3];
                TaxAdditive = new double[3];
                TaxTotal = 123456789; // Prime with something for font metrics debugging
            }

            public double[] Salary;
            public double SalaryTotal;
            public double TaxSubtractiveTotal;
            public double[] TaxAdditive;
            public double TaxAdditiveTotal;
            public double TaxTotal;
        }


        private SalaryTaxData GetSalaryData (int year, int month, Organization organization, int[] yearBreakpoints)
        {
            SalaryTaxData result = new SalaryTaxData();

            Salaries salaries = Salaries.ForOrganization (organization, true);

            foreach (Salary salary in salaries)
            {
                if (salary.PayoutDate.Year == year && salary.PayoutDate.Month == month)
                {
                    int employeeBirthYear = salary.PayrollItem.Person.Birthdate.Year;
                    int ageBracket = 0; // main
                    if (employeeBirthYear >= yearBreakpoints[2])
                    {
                        ageBracket = 1; // youth rebate
                    }
                    else if (employeeBirthYear < yearBreakpoints[1])
                    {
                        ageBracket = 2; // pensioners
                    }

                    result.Salary[ageBracket] += salary.GrossSalaryCents/100.0;
                    result.SalaryTotal += salary.GrossSalaryCents/100.0;
                    result.TaxSubtractiveTotal += salary.SubtractiveTaxCents/100.0;
                    result.TaxAdditive[ageBracket] += salary.AdditiveTaxCents/100.0;
                    result.TaxAdditiveTotal += salary.AdditiveTaxCents/100.0;
                    result.TaxTotal += salary.TaxTotalCents/100.0;
                }
            }

            return result;
        }

        private enum GraphicsElement
        {
            Unknown = 0,
            LeftColumnX,
            RightColumnX,
            AgeBracketMainY,
            AgeBracketDistY,
            AdditiveTaxTotalY,
            SummaryTopLineY,
            SummaryBottomLineY,
            SummaryTotalY
        }
    }
}