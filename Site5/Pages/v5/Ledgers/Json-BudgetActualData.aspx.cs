using System;
using System.Collections.Generic;
using System.Globalization;
using Resources.Pages;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;

public partial class Pages_v5_Ledgers_Json_BudgetActualData : DataV5Base
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

        AnnualReport report = AnnualReport.Create (CurrentOrganization, this._year, FinancialAccountType.Result);
        LocalizeRoot (report.ReportLines);

        Response.ContentType = "application/json";

        Response.Output.WriteLine ("{\"rows\": " + RecurseReport (report.ReportLines) + ", \"footer\": [" +
                                   WriteFooter (report.Totals) + "]}");

        Response.End();
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
            }
        }
    }


    private string WriteFooter (AnnualReportNode totals)
    {
        string result = string.Format ("\"name\":\"{0}\"", Ledgers.ProfitLossStatement_Results);

        result += string.Format(CultureInfo.CurrentCulture, ",\"lastYearActual\":\"{0:N0}\"",
            (double)totals.PreviousYear / -100.0);

        result += string.Format(CultureInfo.CurrentCulture, ",\"yearBudget\":\"{0:N0}\"", (double)totals.ThisYearBudget / 100.0);

        result += string.Format(CultureInfo.CurrentCulture, ",\"yearActual\":\"{0:N0}\"", (double)totals.ThisYear / -100.0);

        return "{" + result + "}";
    }


    private string RecurseReport (List<AnnualReportLine> reportLines)
    {
        List<string> elements = new List<string>();

        foreach (AnnualReportLine line in reportLines)
        {
            string element = string.Format ("\"id\":\"{0}\",\"name\":\"{1}\"", line.AccountId,
                JsonSanitize (line.AccountName));

            if (line.Children.Count > 0)
            {
                element += ",\"lastYearActual\":" +
                           JsonDualString (line.AccountId, line.AccountTreeValues.PreviousYear,
                               line.AccountValues.PreviousYear);

                element += ",\"yearBudget\":" +
                               JsonDualString(line.AccountId, -line.AccountTreeValues.ThisYearBudget, -line.AccountValues.ThisYearBudget);

                element += ",\"yearActual\":" +
                           JsonDualString (line.AccountId, line.AccountTreeValues.ThisYear, line.AccountValues.ThisYear);


                element += ",\"state\":\"closed\",\"children\":" + RecurseReport (line.Children);
            }
            else
            {
                element += string.Format (CultureInfo.CurrentCulture, ",\"lastYearActual\":\"{0:N0}\"",
                    (double) line.AccountValues.PreviousYear/-100.0);

                element += string.Format(CultureInfo.CurrentCulture, ",\"yearBudget\":\"{0:N0}\"",
                    (double)line.AccountValues.ThisYearBudget / 100.0);

                element += string.Format (CultureInfo.CurrentCulture, ",\"yearActual\":\"{0:N0}\"",
                    (double) line.AccountValues.ThisYear/-100.0);
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
                "\"<span class=\\\"budgetactualdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"budgetactualdata-expanded-{0}\\\" style=\\\"display:none\\\">&nbsp;</span>\"",
                accountId, treeValue/-100.00);
        }
        return string.Format (CultureInfo.CurrentCulture,
            "\"<span class=\\\"budgetactualdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"budgetactualdata-expanded-{0}\\\" style=\\\"display:none\\\">{2:N0}</span>\"",
            accountId, treeValue/-100.0, singleValue/-100.0);
    }
}