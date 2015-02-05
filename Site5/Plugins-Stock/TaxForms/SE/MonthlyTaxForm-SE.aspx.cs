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

            int[] yearBreakpoints = {1938, year - 65, year - 26};
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

            coord[2010][GraphicsElement.LeftColumnX] = 775;
            coord[2010][GraphicsElement.RightColumnX] = 1492;

            coord[2015][GraphicsElement.LeftColumnX] = 800;
            coord[2015][GraphicsElement.RightColumnX] = 1512;

            coord[2010][GraphicsElement.AgeBracketMainY] = 694;
            coord[2010][GraphicsElement.AgeBracketDistY] = 67;

            coord[2015][GraphicsElement.AgeBracketMainY] = 770;
            coord[2015][GraphicsElement.AgeBracketDistY] = 65;

            coord[2010][GraphicsElement.SalaryY] = 355;
            coord[2010][GraphicsElement.SalaryTotalY] = 555;
            coord[2010][GraphicsElement.AdditiveTaxTotalY] = 1329;
            coord[2010][GraphicsElement.DeductedTaxY] = 1529;
            coord[2010][GraphicsElement.DeductedTaxTotalY] = 1727;
            coord[2010][GraphicsElement.SummaryTotalY] = 1795;

            coord[2015][GraphicsElement.SalaryY] = 440;
            coord[2015][GraphicsElement.SalaryTotalY] = 640;
            coord[2015][GraphicsElement.AdditiveTaxTotalY] = 1395;
            coord[2015][GraphicsElement.DeductedTaxY] = 1560;
            coord[2015][GraphicsElement.DeductedTaxTotalY] = 1759;
            coord[2015][GraphicsElement.SummaryTotalY] = 1825;



            Image form = Image.FromFile (MapPath (".") + "/MonthlyTaxForm-SE-" + formVersion + ".png");  // the "." says "in same folder as this file"

            CultureInfo swedishCulture = CultureInfo.CreateSpecificCulture ("sv-SE");
            monthString = new DateTime(year, month, 1).ToString("MMMM yyyy", swedishCulture);

            string orgNumber = "xxxxxx-xxxx";
            string sansFontName = "Liberation Sans";
            string courierFontName = "Liberation Mono";
            int smallSize = 18;
            int regularSize = 24;
            int handWriteSize = 36;

            if (Debugger.IsAttached)
            {
                sansFontName = "Arial"; // yeahyeah...
                courierFontName = "Courier New";
                orgNumber = "DEBUGGER ATTACHED";
                smallSize = 24;
                regularSize = 30;
                handWriteSize = 48;
            }

            using (Graphics graphics = Graphics.FromImage (form))
            {
                StringFormat rightAlign = new StringFormat();
                rightAlign.Alignment = StringAlignment.Far;

                _fontHandwriting = new Font (sansFontName, handWriteSize, FontStyle.Bold);
                Font fontPreprinted = new Font (courierFontName, regularSize, FontStyle.Bold);
                Font fontPreprintedSmall = new Font (courierFontName, smallSize, FontStyle.Bold);
                Font fontPreprintedSans = new Font (sansFontName, regularSize, FontStyle.Bold);

                _brushHandwriting = Brushes.Blue;
                Brush brushPreprinted = Brushes.Red;
                Brush brushPreprintedDiscreet = Brushes.DarkRed;

                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw the header and monthname - things to pay attention to when looking at cheat sheet

                if (year <= 2014)
                {
                    graphics.DrawString(organization.Name, fontPreprinted, brushPreprinted, 150, 170);

                    graphics.DrawString(monthString, fontPreprinted, brushPreprinted, 676, 288);
                    graphics.DrawString (monthString, fontPreprinted, brushPreprinted, 610, 1455);
                    graphics.DrawString (String.Format ("{0,4}-{1:D2}-{2:D2}", year, month + 1, 12), fontPreprinted,
                        brushPreprinted, 820, 160);
                    graphics.DrawString (orgNumber, fontPreprinted, brushPreprinted, 1110, 160);

                    // Draw the years and tax rates and other on-form constants more discreetly

                    graphics.DrawString(taxRates[2].ToString("F2", swedishCulture), fontPreprintedSmall,
                        brushPreprintedDiscreet, 840, 729);
                    graphics.DrawString(taxRates[1].ToString("F2", swedishCulture), fontPreprintedSmall,
                        brushPreprintedDiscreet, 840, 796);
                    graphics.DrawString(taxRates[0].ToString("F2", swedishCulture), fontPreprintedSmall,
                        brushPreprintedDiscreet, 840, 863);

                    graphics.DrawString(string.Format("{0} - {1}", yearBreakpoints[1], yearBreakpoints[2] - 1),
                        fontPreprintedSmall, brushPreprintedDiscreet, 190, 729);
                    graphics.DrawString(string.Format("{0} -", yearBreakpoints[2] - 1),
                        fontPreprintedSmall, brushPreprintedDiscreet, 190, 796);
                    graphics.DrawString(string.Format("{0}", yearBreakpoints[1] - 1),
                        fontPreprintedSmall, brushPreprintedDiscreet, 250, 859);
                }
                else // the latest revision of the tax form
                {
                    graphics.DrawString(organization.Name.ToUpperInvariant(), fontPreprinted, brushPreprinted, 840, 255);

                    graphics.DrawString(monthString, fontPreprintedSans, brushPreprinted, 702, 363);
                    graphics.DrawString (monthString, fontPreprintedSans, brushPreprinted, 639, 1485);
                    graphics.DrawString (String.Format ("{0,4}-{1:D2}-{2:D2}", year, month + 1, 12), fontPreprinted,
                        brushPreprinted, 840, 180);
                    graphics.DrawString (orgNumber, fontPreprinted, brushPreprinted, 1125, 180);

                    // Draw the years and tax rates and other on-form constants more discreetly

                    graphics.DrawString(taxRates[2].ToString("F2", swedishCulture), fontPreprintedSmall,
                        brushPreprintedDiscreet, 870, 798);
                    graphics.DrawString(taxRates[1].ToString("F2", swedishCulture), fontPreprintedSmall,
                        brushPreprintedDiscreet, 870, 865);
                    graphics.DrawString(taxRates[0].ToString("F2", swedishCulture), fontPreprintedSmall,
                        brushPreprintedDiscreet, 870, 932);

                    graphics.DrawString(string.Format("{0} - {1}", yearBreakpoints[1], yearBreakpoints[2] - 1),
                        fontPreprintedSmall, brushPreprintedDiscreet, 220, 800);
                    graphics.DrawString(string.Format("{0} -", yearBreakpoints[2] - 1),
                        fontPreprintedSmall, brushPreprintedDiscreet, 220, 863);
                    graphics.DrawString(string.Format("{0}", yearBreakpoints[1] - 1),
                        fontPreprintedSmall, brushPreprintedDiscreet, 270, 924);
                }

                // Draw the actual numbers: First, salary totals

                DrawWrittenNumber(data.SalaryTotal, coord[formVersion][GraphicsElement.LeftColumnX], coord[formVersion][GraphicsElement.SalaryY], graphics);
                DrawWrittenNumber(data.SalaryTotal, coord[formVersion][GraphicsElement.LeftColumnX], coord[formVersion][GraphicsElement.SalaryTotalY], graphics);

                // Additive tax

                for (int ageBracket = 0; ageBracket <= 2; ageBracket++)
                {
                    if (data.Salary[ageBracket] > 0.0)
                    {
                        DrawWrittenNumber(data.Salary[ageBracket], coord[formVersion][GraphicsElement.LeftColumnX],
                            coord[formVersion][GraphicsElement.AgeBracketMainY] +
                            coord[formVersion][GraphicsElement.AgeBracketDistY] * ageBracket,
                            graphics);

                        DrawWrittenNumber(data.TaxAdditive[ageBracket], coord[formVersion][GraphicsElement.RightColumnX],
                            coord[formVersion][GraphicsElement.AgeBracketMainY] +
                            coord[formVersion][GraphicsElement.AgeBracketDistY] * ageBracket,
                            graphics);

                    }
                }

                DrawWrittenNumber (data.TaxAdditiveTotal, coord[formVersion][GraphicsElement.RightColumnX], coord[formVersion][GraphicsElement.AdditiveTaxTotalY], graphics);

                // Deducted taxes

                DrawWrittenNumber(data.SalaryTotal, coord[formVersion][GraphicsElement.LeftColumnX], coord[formVersion][GraphicsElement.DeductedTaxY], graphics);
                DrawWrittenNumber(data.SalaryTotal, coord[formVersion][GraphicsElement.LeftColumnX], coord[formVersion][GraphicsElement.DeductedTaxTotalY], graphics);

                DrawWrittenNumber(data.TaxSubtractiveTotal, coord[formVersion][GraphicsElement.RightColumnX], coord[formVersion][GraphicsElement.DeductedTaxY], graphics);
                DrawWrittenNumber(data.TaxSubtractiveTotal, coord[formVersion][GraphicsElement.RightColumnX], coord[formVersion][GraphicsElement.DeductedTaxTotalY], graphics);

                // Grand total

                DrawWrittenNumber (data.TaxTotal, coord[formVersion][GraphicsElement.RightColumnX], coord[formVersion][GraphicsElement.SummaryTotalY], graphics);
            }

            // Write graphics buffer as PNG stream to Response object

            using (Stream responseStream = Response.OutputStream)
            {
                form.Save (responseStream, ImageFormat.Png);
            }

        }


        private void DrawWrittenNumber (double number, int x, int y, Graphics graphics)
        {
            if (Debugger.IsAttached)
            {
                x += 10; // Mono has different metrics because of bloody course it has
            }

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
            DeductedTaxY,
            DeductedTaxTotalY,
            SalaryY,
            SalaryTotalY,
            SummaryTopLineY,
            SummaryTotalY
        }
    }
}