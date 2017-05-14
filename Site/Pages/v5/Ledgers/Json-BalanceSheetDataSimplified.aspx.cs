using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class BalanceSheetDataSimplified : DataV5Base
    {
        private AuthenticationData _authenticationData;


        private int _year = 2012;

        protected void Page_Load (object sender, EventArgs e)
        {
            // Get auth data

            this._authenticationData = GetAuthenticationDataAndCulture();

            // Get current year

            this._year = DateTime.Today.Year;

            string yearParameter = Request.QueryString["Year"];

            if (!string.IsNullOrEmpty (yearParameter))
            {
                this._year = Int32.Parse (yearParameter); // will throw if non-numeric - don't matter for app
            }

            AnnualReport report = AnnualReport.Create (this._authenticationData.CurrentOrganization, this._year,
                FinancialAccountType.Balance);
            LocalizeRoot (report.ReportLines);

            _totals = new BalanceSimplifiedTotals();

            Response.ContentType = "application/json";

            Response.Output.WriteLine("{\"rows\": " + RecurseReport(report.ReportLines) + ", \"footer\": [" +
                                       WriteFooter() + "]}");


            Response.End();
        }

        private class BalanceSimplifiedTotals
        {
            public Int64 AssetsCents { get; set; }
            public Int64 LiabilitiesCents { get; set; }
        }

        private BalanceSimplifiedTotals _totals;

        private string WriteFooter()
        {
            string line1 = string.Format("\"name\":\"{0}\"", Resources.Global.Global_Total);

            line1 += string.Format(CultureInfo.CurrentCulture, ",\"assets\":\"{0:N0}\"",
                _totals.AssetsCents / 100.0);

            line1 += string.Format(CultureInfo.CurrentCulture, ",\"liabilities\":\"{0:N0}\"", _totals.LiabilitiesCents/-100.0);

            if (_totals.AssetsCents == _totals.LiabilitiesCents)
            {
                return "{" + line1 + "}";
            }
            else if (_totals.AssetsCents > -_totals.LiabilitiesCents) // Predicted profit
            {
                string line2 = string.Format("\"name\":\"{0}\"", string.Format(Resources.Pages.Ledgers.BalanceSheet_ProfitToDate, _year));

                line2 += string.Format(CultureInfo.CurrentCulture, ",\"liabilities\":\"{0:N0}\"",
                    (_totals.AssetsCents + _totals.LiabilitiesCents) / 100.0);

                return "{" + line1 + "},{" + line2 + "}";
            }
            else
            {
                string line2 = string.Format("\"name\":\"{0}\"", string.Format(Resources.Pages.Ledgers.BalanceSheet_LossToDate, _year));

                line2 += string.Format(CultureInfo.CurrentCulture, ",\"assets\":\"{0:N0}\"",
                    (_totals.LiabilitiesCents + _totals.AssetsCents) / -100.0);

                return "{" + line1 + "},{" + line2 + "}";
            }
        }


        private void LocalizeRoot (List<AnnualReportLine> lines)
        {
            Dictionary<string, string> localizeMap = new Dictionary<string, string>();

            localizeMap["%ASSET_ACCOUNTGROUP%"] = Resources.Global.Financial_Asset;
            localizeMap["%DEBT_ACCOUNTGROUP%"] = Resources.Global.Financial_Debt;
            localizeMap["%INCOME_ACCOUNTGROUP%"] = Resources.Global.Financial_Income;
            localizeMap["%COST_ACCOUNTGROUP%"] = Resources.Global.Financial_Cost;

            foreach (AnnualReportLine line in lines)
            {
                if (localizeMap.ContainsKey (line.AccountName))
                {
                    line.AccountName = localizeMap[line.AccountName];
                    line.DefaultExpand = true;
                }
            }
        }


        private string RecurseReport (List<AnnualReportLine> reportLines)
        {
            List<string> elements = new List<string>();

            foreach (AnnualReportLine line in reportLines)
            {
                string columnCardinal = "assets";
                string columnDelta = "assetdelta";
                int signReverser = 1;

                if (line.AccountType == FinancialAccountType.Debt)
                {
                    columnCardinal = "liabilities";
                    columnDelta = "liabilitydelta";
                    signReverser = -1;
                    this._totals.LiabilitiesCents += line.AccountValues.ThisYear;
                }
                else
                {
                    this._totals.AssetsCents += line.AccountValues.ThisYear;
                }

                string element = string.Format ("\"id\":\"{0}\",\"name\":\"{1}\"", line.AccountId,
                    JsonSanitize (line.AccountName));

                if (line.Children.Count > 0)
                {
                    element += ",\"" + columnCardinal + "\":" +
                               JsonDualString (line.AccountId, line.AccountTreeValues.ThisYear * signReverser,
                                   line.AccountValues.ThisYear * signReverser, line.DefaultExpand);

                    element += ",\"" + columnDelta + "\":" +
                               JsonDualString (line.AccountId, (line.AccountTreeValues.ThisYear-line.AccountTreeValues.PreviousYear) * signReverser,
                                   (line.AccountValues.ThisYear-line.AccountValues.PreviousYear) * signReverser, line.DefaultExpand, string.Empty, "(+#,#.);(-#,#.);---");


                    element += ",\"state\":\"" + (line.DefaultExpand? "open":"closed") + "\",\"children\":" + RecurseReport (line.Children);
                }
                else
                {
                    element += string.Format (CultureInfo.CurrentCulture, ",\"" + columnCardinal + "\":\"{0:N0}\"",
                        (double) line.AccountValues.ThisYear/100.0*signReverser);

                    element += string.Format (CultureInfo.CurrentCulture, ",\"" + columnDelta + "\":\"{0:(+#,#.);(-#,#.);---}\"",
                        (double) (line.AccountValues.ThisYear-line.AccountValues.PreviousYear)/100.0*signReverser)
                        .Replace("---","<span style='color:#CCC'>&mdash;</span>")
                        .Replace("-", "&minus;");
                }

                elements.Add ("{" + element + "}");
            }

            return "[" + String.Join (",", elements.ToArray()) + "]";
        }


        private string JsonDualString (int accountId, Int64 treeValue, Int64 singleValue, bool expanded, string sigma = "<strong>&Sigma;</strong>", string format = "N0")
        {
            if (expanded || treeValue != 0 && singleValue == 0)
            {
                return string.Format (CultureInfo.CurrentCulture,
                    "\"<span class='annualreportdata-collapsed-{0}' " + (expanded? "style='display:none'": "") + ">" + sigma + " " + FormatSingleString(treeValue, format) + "</span>" + 
                    "<span class='annualreportdata-expanded-{0}' " + (!expanded ? "style='display:none'" : "") + ">&nbsp;</span>\"",
                    accountId);
            }
            return string.Format(CultureInfo.CurrentCulture,
                "\"<span class='annualreportdata-collapsed-{0}'>" + sigma + " " + FormatSingleString(treeValue, format) + " " + "</span>" +
                "<span class='annualreportdata-expanded-{0}' style='display:none'>" + FormatSingleString(singleValue, format) + "</span>\"",
                accountId);

        }

        private string FormatSingleString(Int64 centsValue, string format = "N0")
        {
            return String.Format("{0:" + format + "}", centsValue / 100.0)
                        .Replace("---", "<span style='color:#CCC'>&mdash;</span>")
                        .Replace("-", "&minus;");
        }
    }
}