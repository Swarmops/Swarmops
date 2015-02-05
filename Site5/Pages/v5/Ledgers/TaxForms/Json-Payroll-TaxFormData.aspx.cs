using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.v5.Ledgers.TaxForms
{
    public partial class Json_PayrollData : DataV5Base
    {
        private AuthenticationData _authenticationData;
        private Country _country;
        private int _minKey = 999999;
        private int _currentYear = DateTime.UtcNow.Year;

        protected void Page_Load (object sender, EventArgs e)
        {
            // Get auth data

            this._authenticationData = GetAuthenticationDataAndCulture();

            // Get current year

            int yearLoop = _currentYear;

            // Country asked for

            this._country = Country.FromCode (Request["Country"]);

            Response.ContentType = "application/json";

            Dictionary<int, SalaryData> data = GetSalaryData();

            List<string> yearElements = new List<string>();
            while (_minKey < yearLoop * 1000)  // at least one more year with data
            {
                List<string> monthElements = new List<string>();
                SalaryData yearData = new SalaryData();

                for (int month = 12; month >= 1; month--)
                {
                    int key = yearLoop*100 + month;
                    if (data.ContainsKey (key))
                    {
                        monthElements.Add (WriteElement (key, new DateTime (yearLoop, month, 1).ToString("yyyy MMMM"), data [key]));
                        yearData.GrossSalaryCents += data[key].GrossSalaryCents;
                        yearData.AdditiveTaxCents += data[key].DeductedTaxCents;
                        yearData.DeductedTaxCents += data[key].AdditiveTaxCents;
                    }
                }

                if (monthElements.Count > 0)
                {
                    yearElements.Add (WriteElement (yearLoop, String.Format (Resources.Global.Global_YearX, yearLoop), yearData, monthElements));
                }

                yearLoop--;
            }

            Response.Output.WriteLine ("[" + String.Join (",", yearElements) + "]");

            Response.End();
        }


        private Dictionary<int, SalaryData> GetSalaryData()
        {
            Dictionary<int, SalaryData> result = new Dictionary<int, SalaryData>();
            Salaries salaries = Salaries.ForOrganization(_authenticationData.CurrentOrganization, true);

            foreach (Salary salary in salaries)
            {
                int monthKey = salary.PayoutDate.Year*100 + salary.PayoutDate.Month;

                if (!result.ContainsKey (monthKey))
                {
                    result[monthKey] = new SalaryData();
                }

                result[monthKey].GrossSalaryCents += salary.GrossSalaryCents;
                result[monthKey].AdditiveTaxCents += salary.AdditiveTaxCents;
                result[monthKey].DeductedTaxCents += salary.SubtractiveTaxCents;

                if (monthKey < _minKey)
                {
                    _minKey = monthKey;
                }
            }

            return result;
        }



        private string WriteElement (int key, string title, SalaryData data, List<string> children = null)
        {
            string element = string.Format("\"id\":\"{0}\",\"yearMonth\":\"{1}\"", key,
                    JsonSanitize(title));

            string imageClass = "IconFormMonth";

            if (children != null)
            {
                element += String.Format (",\"state\":\"{0}\",\"children\":{1}",
                    key == this._currentYear ? "open" : "closed",
                    "[" + String.Join (",", children) + "]");

                imageClass = "IconFormYear";
            }

            string image = String.Format("<img src=\"{0}\" class=\"{1}\" yearmonth=\"{2}\" height=\"24px\" width=\"24px\" />",
                "/Images/Flags/" + _country.Code.ToLower() + "-24px.png", imageClass, key);

            element += String.Format(",\"grossPay\":\"{0:N0}\",\"additiveTax\":\"{1:N0}\",\"deductedTax\":\"{2:N0}\",\"costTotal\":\"{3:N0}\",\"taxTotal\":\"{4:N0}\",\"forms\":\"{5}\"",
                data.GrossSalaryCents / 100.0, data.AdditiveTaxCents / 100.0, data.DeductedTaxCents / 100.0, (data.GrossSalaryCents + data.AdditiveTaxCents) / 100.0,
                (data.AdditiveTaxCents + data.DeductedTaxCents) / 100.0, image.Replace ("\"", "\\\""));

            return "{" + element + "}";
        }



        private void LocalizeRoot (List<YearlyReportLine> lines)
        {
            Dictionary<string, string> localizeMap = new Dictionary<string, string>();

            localizeMap["%ASSET_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.BalanceSheet_Assets;
            localizeMap["%DEBT_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.BalanceSheet_Debt;
            localizeMap["%INCOME_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.ProfitLossStatement_Income;
            localizeMap["%COST_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.ProfitLossStatement_Costs;

            foreach (YearlyReportLine line in lines)
            {
                if (localizeMap.ContainsKey (line.AccountName))
                {
                    line.AccountName = localizeMap[line.AccountName];
                }
            }
        }


        private string RecurseReport (List<YearlyReportLine> reportLines)
        {
            List<string> elements = new List<string>();

            foreach (YearlyReportLine line in reportLines)
            {
                string element = string.Format ("\"id\":\"{0}\",\"monthYear\":\"{1}\"", line.AccountId,
                    JsonSanitize (line.AccountName));

                if (line.Children.Count > 0)
                {
                    element += ",\"lastYear\":" +
                               JsonDualString (line.AccountId, line.AccountTreeValues.PreviousYear,
                                   line.AccountValues.PreviousYear);

                    for (int quarter = 1; quarter <= 4; quarter++)
                    {
                        element += string.Format (",\"q{0}\":", quarter) +
                                   JsonDualString (line.AccountId, line.AccountTreeValues.Quarters[quarter - 1],
                                       line.AccountValues.Quarters[quarter - 1]);
                    }

                    element += ",\"ytd\":" +
                               JsonDualString (line.AccountId, line.AccountTreeValues.ThisYear,
                                   line.AccountValues.ThisYear);


                    element += ",\"state\":\"closed\",\"children\":" + RecurseReport (line.Children);
                }
                else
                {
                    element += string.Format (CultureInfo.CurrentCulture, ",\"lastYear\":\"{0:N0}\"",
                        (double) line.AccountValues.PreviousYear/100.0);

                    for (int quarter = 1; quarter <= 4; quarter++)
                    {
                        element += string.Format (CultureInfo.CurrentCulture, ",\"q{0}\":\"{1:N0}\"", quarter,
                            line.AccountValues.Quarters[quarter - 1]/100.0);
                    }

                    element += string.Format (CultureInfo.CurrentCulture, ",\"ytd\":\"{0:N0}\"",
                        (double) line.AccountValues.ThisYear/100.0);
                }

                elements.Add ("{" + element + "}");
            }

            return "[" + String.Join (",", elements.ToArray()) + "]";
        }


        private string JsonDualString (int accountId, Int64 treeValue, Int64 singleValue)
        {
            if (treeValue != 0 && singleValue == 0)
            {
                return string.Format (CultureInfo.CurrentCulture,
                    "\"<span class=\\\"profitlossdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"profitlossdata-expanded-{0}\\\" style=\\\"display:none\\\">&nbsp;</span>\"",
                    accountId, treeValue/100.00);
            }
            return string.Format (CultureInfo.CurrentCulture,
                "\"<span class=\\\"profitlossdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"profitlossdata-expanded-{0}\\\" style=\\\"display:none\\\">{2:N0}</span>\"",
                accountId, treeValue/100.0, singleValue/100.0);
        }


        private class SalaryData
        {
            public Int64 GrossSalaryCents { get; set; }
            public Int64 AdditiveTaxCents { get; set; }
            public Int64 DeductedTaxCents { get; set; }
            // all others can be inferred from these three
        }
    }
}