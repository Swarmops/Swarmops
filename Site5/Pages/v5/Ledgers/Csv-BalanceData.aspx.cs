using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

public partial class Pages_v5_Ledgers_Csv_BalanceData : DataV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current year

        _year = DateTime.Today.Year;

        string yearParameter = Request.QueryString["Year"];

        if (!string.IsNullOrEmpty(yearParameter))
        {
            _year = Int32.Parse(yearParameter); // will throw if non-numeric - don't matter for app
        }

        YearlyReport report = YearlyReport.Create(this.CurrentOrganization, _year, FinancialAccountType.Balance);

        Response.ClearContent();
        Response.ClearHeaders();
        Response.ContentType = "text/plain";
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + Resources.Pages.Ledgers.BalanceSheet_DownloadFileName + _year.ToString(CultureInfo.InvariantCulture) + "-" + DateTime.Today.ToString("yyyyMMdd") + ".csv");

        Response.Output.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"", Resources.Pages.Ledgers.ProfitLossStatement_AccountName, Resources.Pages.Ledgers.BalanceSheet_StartYear.Replace("XXXX", _year.ToString(CultureInfo.InvariantCulture)),
            Resources.Pages.Ledgers.BalanceSheet_Q1, Resources.Pages.Ledgers.BalanceSheet_Q2, Resources.Pages.Ledgers.BalanceSheet_Q3, Resources.Pages.Ledgers.BalanceSheet_Q4,
            (_year == DateTime.Today.Year? Resources.Pages.Ledgers.BalanceSheet_Current : Resources.Pages.Ledgers.BalanceSheet_EndYear.Replace("XXXX", _year.ToString(CultureInfo.InvariantCulture))));

        LocalizeRoot(report.ReportLines);

        RecurseCsvReport(report.ReportLines, string.Empty);

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




    private int _year = 2012;

    private void RecurseCsvReport(List<YearlyReportLine> reportLines, string accountPrefix)
    {
        foreach (YearlyReportLine line in reportLines)
        {
            Response.Output.WriteLine("\"{0}{1}\",{2},{3},{4},{5},{6},{7}",
                                      accountPrefix, line.AccountName,
                                      line.AccountValues.PreviousYear / 100.0,
                                      line.AccountValues.Quarters[0] / 100.0,
                                      line.AccountValues.Quarters[1] / 100.0,
                                      line.AccountValues.Quarters[2] / 100.0,
                                      line.AccountValues.Quarters[3] / 100.0,
                                      line.AccountValues.ThisYear / 100.0);


            if (line.Children.Count > 0)
            {
                RecurseCsvReport(line.Children, "-" + accountPrefix);
            }
        }

    }
}