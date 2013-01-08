﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using System.Globalization;

public partial class Pages_v5_Ledgers_Json_ProfitLossData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Current authentication

        string identity = HttpContext.Current.User.Identity.Name;
        string[] identityTokens = identity.Split(',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        int currentUserId = Convert.ToInt32(userIdentityString);
        int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

        Person currentUser = Person.FromIdentity(currentUserId);
        Authority authority = currentUser.GetAuthority();
        Organization currentOrganization = Organization.FromIdentity(currentOrganizationId);

        // Get culture

        string cultureString = "en-US";
        HttpCookie cookie = Request.Cookies["PreferredCulture"];

        if (cookie != null)
        {
            cultureString = cookie.Value;
        }

        _renderCulture = new CultureInfo(cultureString);

        // Get current year

        _year = DateTime.Today.Year;

        string yearParameter = Request.QueryString["Year"];

        if (!string.IsNullOrEmpty(yearParameter))
        {
            _year = Int32.Parse(yearParameter); // will throw if non-numeric - don't matter for app
        }

        YearlyReport report = YearlyReport.Create(currentOrganization, _year, FinancialAccountType.Result);
        LocalizeRoot(report.ReportLines);

        Response.ContentType = "application/json";

        Response.Output.WriteLine("{\"rows\": " + RecurseReport(report.ReportLines) + ", \"footer\": [" + WriteFooter(report.Totals) + "]}");

        Response.End();
    }


    private void LocalizeRoot(List<YearlyReportLine> lines)
    {
        Dictionary<string, string> localizeMap = new Dictionary<string, string>();

        localizeMap["%ASSET_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.BalanceSheet_Assets;
        localizeMap["%DEBT_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.BalanceSheet_Debt;
        localizeMap["%INCOME_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.ProfitLossStatement_Income;
        localizeMap["%COST_ACCOUNTGROUP%"] = Resources.Pages.Ledgers.ProfitLossStatement_Costs;

        foreach (YearlyReportLine line in lines)
        {
            if (localizeMap.ContainsKey(line.AccountName))
            {
                line.AccountName = localizeMap[line.AccountName];
            }
        }
    }


    private string WriteFooter (YearlyReportNode totals)
    {
        string result = string.Format("\"name\":\"{0}\"", Resources.Pages.Ledgers.ProfitLossStatement_Results);

        result += string.Format(_renderCulture, ",\"lastYear\":\"{0:N0}\"", (double)totals.PreviousYear / -100.0);

        for (int quarter = 1; quarter <= 4; quarter++)
        {
            result += string.Format(_renderCulture, ",\"q{0}\":\"{1:N0}\"", quarter, totals.Quarters[quarter-1] / -100.0);
        }

        result += string.Format(_renderCulture, ",\"ytd\":\"{0:N0}\"", (double)totals.ThisYear / -100.0);

        return "{" + result + "}";
    }


    private int _year = 2012;
    private CultureInfo _renderCulture;

    private string RecurseReport (List<YearlyReportLine> reportLines)
    {
        List<string> elements = new List<string>();

        foreach (YearlyReportLine line in reportLines)
        {
            string element = string.Format("\"id\":{0},\"name\":\"{1}\"", line.AccountId,
                                            line.AccountName.Replace("\"", "'"));

            if (line.Children.Count > 0)
            {

                element += ",\"lastYear\":" +
                           JsonDualString(line.AccountId, line.AccountTreeValues.PreviousYear, line.AccountValues.PreviousYear);

                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    element += string.Format(",\"q{0}\":", quarter) +
                               JsonDualString(line.AccountId, line.AccountTreeValues.Quarters[quarter-1], line.AccountValues.Quarters[quarter-1]);
                }

                element += ",\"ytd\":" +
                   JsonDualString(line.AccountId, line.AccountTreeValues.ThisYear, line.AccountValues.ThisYear);


                element += ",\"state\":\"closed\",\"children\":" + RecurseReport(line.Children);
            }
            else
            {
                element += string.Format(_renderCulture, ",\"lastYear\":\"{0:N0}\"", (double)line.AccountValues.PreviousYear / -100.0);

                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    element += string.Format(_renderCulture, ",\"q{0}\":\"{1:N0}\"", quarter, line.AccountValues.Quarters[quarter-1] / -100.0);
                }

                element += string.Format(_renderCulture, ",\"ytd\":\"{0:N0}\"", (double)line.AccountValues.ThisYear / -100.0);
            }

            elements.Add("{" + element + "}");
        }

        return "[" + String.Join(",", elements.ToArray()) + "]";
    }


    private string JsonDualString (int accountId, Int64 treeValue, Int64 singleValue)
    {
        if (treeValue != 0 && singleValue == 0)
        {
            return string.Format(_renderCulture, "\"<span class=\\\"profitlossdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"profitlossdata-expanded-{0}\\\" style=\\\"display:none\\\">&nbsp;</span>\"", accountId, treeValue / -100.00);
        }
        return string.Format(_renderCulture, "\"<span class=\\\"profitlossdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"profitlossdata-expanded-{0}\\\" style=\\\"display:none\\\">{2:N0}</span>\"", accountId, treeValue / -100.0, singleValue / -100.0);
    }



}