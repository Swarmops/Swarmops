using System;
using System.Collections.Generic;
using System.Globalization;
using Resources.Pages;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;

public partial class Pages_v5_Ledgers_Csv_ProfitLossData : DataV5Base
{
    private int _year = 2012;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get current year

        this._year = DateTime.Today.Year;

        string yearParameter = Request.QueryString["Year"];

        if (!string.IsNullOrEmpty (yearParameter))
        {
            this._year = Int32.Parse (yearParameter); // will throw if non-numeric - don't matter for app
        }

        YearlyReport report = YearlyReport.Create (CurrentOrganization, this._year, FinancialAccountType.Result);

        Response.ClearContent();
        Response.ClearHeaders();
        Response.ContentType = "text/plain";
        Response.AppendHeader ("Content-Disposition",
            "attachment;filename=" + Ledgers.ProfitLossStatement_DownloadFileName +
            this._year.ToString (CultureInfo.InvariantCulture) + "-" + DateTime.Today.ToString ("yyyyMMdd") + ".csv");

        if (this._year == DateTime.Today.Year)
        {
            Response.Output.WriteLine ("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                Ledgers.ProfitLossStatement_AccountName,
                Ledgers.ProfitLossStatement_LastYear,
                Ledgers.ProfitLossStatement_Q1, Ledgers.ProfitLossStatement_Q2,
                Ledgers.ProfitLossStatement_Q3, Ledgers.ProfitLossStatement_Q4,
                Ledgers.ProfitLossStatement_Ytd);
        }
        else
        {
            Response.Output.WriteLine ("\"{0}\",\"{1}\",\"{6}-{2}\",\"{6}-{3}\",\"{6}-{4}\",\"{6}-{5}\",\"{6}\"",
                Ledgers.ProfitLossStatement_AccountName, this._year - 1,
                Ledgers.ProfitLossStatement_Q1, Ledgers.ProfitLossStatement_Q2,
                Ledgers.ProfitLossStatement_Q3, Ledgers.ProfitLossStatement_Q4,
                this._year);
        }

        LocalizeRoot (report.ReportLines);

        RecurseCsvReport (report.ReportLines, string.Empty);

        Response.End();
    }


    private void LocalizeRoot (List<YearlyReportLine> lines)
    {
        Dictionary<string, string> localizeMap = new Dictionary<string, string>();

        localizeMap["%ASSET_ACCOUNTGROUP%"] = Ledgers.BalanceSheet_Assets;
        localizeMap["%DEBT_ACCOUNTGROUP%"] = Ledgers.BalanceSheet_Debt;
        localizeMap["%INCOME_ACCOUNTGROUP%"] = Ledgers.ProfitLossStatement_Income;
        localizeMap["%COST_ACCOUNTGROUP%"] = Ledgers.ProfitLossStatement_Costs;

        foreach (YearlyReportLine line in lines)
        {
            if (localizeMap.ContainsKey (line.AccountName))
            {
                line.AccountName = localizeMap[line.AccountName];
            }
        }
    }


    private void RecurseCsvReport (List<YearlyReportLine> reportLines, string accountPrefix)
    {
        foreach (YearlyReportLine line in reportLines)
        {
            Response.Output.WriteLine ("\"{0}{1}\",{2},{3},{4},{5},{6},{7}",
                accountPrefix, line.AccountName,
                line.AccountValues.PreviousYear/-100.0,
                line.AccountValues.Quarters[0]/-100.0,
                line.AccountValues.Quarters[1]/-100.0,
                line.AccountValues.Quarters[2]/-100.0,
                line.AccountValues.Quarters[3]/-100.0,
                line.AccountValues.ThisYear/-100.0);


            if (line.Children.Count > 0)
            {
                RecurseCsvReport (line.Children, "-" + accountPrefix);
            }
        }
    }
}