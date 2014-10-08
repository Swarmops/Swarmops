using System;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;

public partial class Tests_TestBookkeepingIntegrity : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = "Test Bookkeeping Integrity";
        this.PageIcon = "iconshock-tester";

        if (!Page.IsPostBack)
        {
            this.DropYear.Items.Add(new ListItem("--Select--", "0"));
            this.DropYear.Items.Add(new ListItem("2008", "2008"));
            this.DropYear.Items.Add(new ListItem("2009", "2009"));
            this.DropYear.Items.Add(new ListItem("2010", "2010"));
            this.DropYear.Items.Add(new ListItem("2011", "2011"));
        }

        this.LabelThisOrganization.Text = this.CurrentOrganization.Name;
    }

    protected void DropYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        int year = Int32.Parse(this.DropYear.SelectedValue);

        if (year < 2000) return;

        FinancialAccounts balanceAccounts = FinancialAccounts.ForOrganization(this.CurrentOrganization,
                                                                              FinancialAccountType.Balance);

        FinancialAccounts resultAccounts = FinancialAccounts.ForOrganization(this.CurrentOrganization,
                                                                             FinancialAccountType.Result);

        FinancialAccount ownCapital = this.CurrentOrganization.FinancialAccounts.DebtsEquity;
        FinancialAccount resultAsNoted = this.CurrentOrganization.FinancialAccounts.CostsYearlyResult;

        FinancialAccounts balancesWithoutCapital =
            FinancialAccounts.ForOrganization(this.CurrentOrganization, FinancialAccountType.Balance);
        balancesWithoutCapital.Remove(ownCapital);

        FinancialAccounts resultAccountsWithoutNotedResult = FinancialAccounts.ForOrganization(this.CurrentOrganization,
                                                                             FinancialAccountType.Result);
        resultAccountsWithoutNotedResult.Remove(this.CurrentOrganization.FinancialAccounts.CostsYearlyResult);

        Currency currency = this.CurrentOrganization.DefaultCountry.Currency;

        this.LabelResultsAll.Text = String.Format("{0} {1:N2}", currency.Code,
                                                  resultAccounts.GetDeltaCents(new DateTime(year, 1, 1),
                                                                               new DateTime(year + 1, 1, 1)) / 100.0);

        this.LabelResultsNoted.Text = String.Format("{0} {1:N2}", currency.Code,
                                                    resultAsNoted.GetDeltaCents(new DateTime(year, 1, 1),
                                                                                new DateTime(year + 1, 1, 1))/100.0);

        this.LabelEoyBalance.Text = String.Format("{0} {1:N2}", currency.Code,
                                                    balanceAccounts.GetDeltaCents(new DateTime(1900, 1, 1),
                                                                                new DateTime(year + 1, 1, 1)) / 100.0);

        Int64 endOfLastYearCapital = ownCapital.GetDeltaCents(new DateTime(1900, 1, 1),
                                                              new DateTime(year, 1, 1));

        Int64 endOfSelectedYearCapital = ownCapital.GetDeltaCents(new DateTime(1900, 1, 1),
                                                              new DateTime(year + 1, 1, 1));

        this.LabelEolyOwnCapital.Text = String.Format("{0} {1:N2}", currency.Code, endOfLastYearCapital / 100.0);

        this.LabelEocyOwnCapital.Text = String.Format("{0} {1:N2}", currency.Code,
                                            endOfSelectedYearCapital / 100.0);

        this.LabelOwnCapitalDiff.Text = String.Format("{0} {1:N2}", currency.Code,
                                            (endOfSelectedYearCapital - endOfLastYearCapital) / 100.0);

        this.LabelOwnCapitalDelta.Text = String.Format("{0} {1:N2}", currency.Code,
                                            ownCapital.GetDeltaCents(new DateTime(year, 1, 1),
                                                                        new DateTime(year + 1, 1, 1)) / 100.0);

    }
}