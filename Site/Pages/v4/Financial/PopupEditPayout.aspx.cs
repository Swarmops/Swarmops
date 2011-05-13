using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_Financial_PopupEditPayout : PageV4Base
{
    private Payout thisPayout = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get account

        int thisPayoutId = Int32.Parse(Request.QueryString["PayoutId"]);
        thisPayout = Payout.FromIdentity(thisPayoutId);
        int year = DateTime.Today.Year;

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, thisPayout.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        if (!Page.IsPostBack)
        {
            // Populate all data

            this.LabelHeader.Text = String.Format("Editing Payout #{0} ({1:N2})", thisPayout.Identity, thisPayout.Amount);
            PopulateDropTransactions();
            PopulateDropPayouts();
        }
    }

    private void PopulateDropTransactions()
    {
        int orgId = thisPayout.OrganizationId;
        FinancialTransactions unbalancedTransactions =
            FinancialTransactions.GetUnbalanced(Organization.FromIdentity(orgId));

        this.DropTransactions.Items.Clear();
        this.DropTransactions.Items.Add(new ListItem("-- Select a transaction --", "0"));

        int matchCount = 0;
        foreach (FinancialTransaction transaction in unbalancedTransactions)
        {
            if (transaction.Rows.AmountCentsTotal == -thisPayout.AmountCents)
            {
                string transactionDescription = transaction.Description;

                if (transactionDescription.Length > 15)
                {
                    transactionDescription = transactionDescription.Substring(0, 12) + "...";
                }

                matchCount++;
                this.DropTransactions.Items.Add(
                    new ListItem(
                        String.Format(new CultureInfo("sv-SE"), "#{0}, {1:yyyy-MM-dd}, {2}", transaction.Identity,
                                      transaction.DateTime, transaction.Description),
                        transaction.Identity.ToString()));
            }
        }

        if (matchCount == 0)
        {
            this.DropTransactions.Items.Add(new ListItem("No unbalanced transactions at this amount.", "0"));
            this.RadioManualMap.Enabled = false;
            if (this.RadioManualMap.Checked)
            {
                this.RadioManualMap.Checked = false;
            }
        }
    }


    private void PopulateDropPayouts()
    {
        int orgId = thisPayout.OrganizationId;
        Payouts openPayouts = Payouts.ForOrganization(Organization.FromIdentity(orgId));

        this.DropPayouts.Items.Clear();
        this.DropPayouts.Items.Add(new ListItem("-- Select a payout --", "0"));

        foreach (Payout payout in openPayouts)
        {
            if (payout.Identity != thisPayout.Identity)
            {
                this.DropPayouts.Items.Add(
                    new ListItem(
                        String.Format(new CultureInfo("sv-SE"), "#{0}, {1}, {2:N2}", payout.Identity,
                                      payout.Recipient, payout.Amount), 
                        payout.Identity.ToString()));
            }
        }

        if (openPayouts.Count < 2)
        {
            this.DropPayouts.Items.Add(new ListItem("No other open payout.", "0"));
            this.RadioManualMerge.Enabled = false;
        }
    }


/*
    protected void ButtonSetTag_Click(object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanDoEconomyTransactions, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            _account.BankTransactionTag = this.TextTransactionTag.Text;
        }

        // ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);

    }

    protected void ButtonSetGeography_Click(object sender, EventArgs e)
    {
        if (this.DropGeographies.SelectedGeography != null)
        {
            if (_authority.HasPermission(Permission.CanDoEconomyTransactions, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
            {
                _account.AssignedGeography = this.DropGeographies.SelectedGeography;
            }
        }

        // 

    }*/

    protected void DropTransactions_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    private void UpdateExecute()
    {
        bool valid = false;
        string message = string.Empty;

        Dictionary<decimal, List<int>> transactionDeltas = new Dictionary<decimal, List<int>>();
        FinancialTransactions transactions = FinancialTransactions.GetUnbalanced(thisPayout.Organization);
        this.ButtonExecute.Enabled = false;

        foreach (FinancialTransaction transaction in transactions)
        {
            decimal diff = transaction.Rows.AmountTotal;

            if (!transactionDeltas.ContainsKey (-diff))
            {
                transactionDeltas[-diff] = new List<int>();
            }

            transactionDeltas[-diff].Add(transaction.Identity);
        }

        if (this.RadioManualMap.Checked)
        {

            int transactionId = Int32.Parse(this.DropTransactions.SelectedValue);

            if (transactionId > 0)
            {
                FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);

                this.LabelActionDescription.Text =
                    String.Format(
                        "Payout #{0}, supposed date {1:yyyy-MM-dd}, will be mapped to Transaction #{2} on date {3:yyyy-MM-dd}. Transaction #{2} will be balanced and documented and tied to {4}. Payout #{0} will be closed.",
                        thisPayout.Identity, thisPayout.ExpectedTransactionDate, transaction.Identity, transaction.DateTime, GetDependencyString(thisPayout));
                valid = true;
                this.ButtonExecute.Text = "Execute MAP";
                this.ButtonExecute.Enabled = true;
            }
        }

        if (this.RadioManualMerge.Checked)
        {
            int payoutId = Int32.Parse(this.DropPayouts.SelectedValue);

            if (payoutId > 0)
            {
                Payout payout = Payout.FromIdentity(payoutId);
                decimal newAmount = (decimal) (payout.Amount + thisPayout.Amount);
                this.LabelActionDescription.Text =
                    String.Format(
                        "Payout #{0} will be zeroed and closed. Payout #{1} will take over the dependencies of Payout #{0}. It will cover {2} and have an amount of {3:N2}.",
                        payout.Identity, thisPayout.Identity, GetDependencyString(thisPayout, payout), newAmount);

                if (transactionDeltas.ContainsKey(newAmount))
                {
                    this.LabelActionDescription.Text += " This matches Transaction " +
                                                        Formatting.GenerateRangeString(transactionDeltas[newAmount]) +
                                                        ".";
                }
                valid = true;
                this.ButtonExecute.Text = "Execute MERGE";
                this.ButtonExecute.Enabled = true;
            }
        }

        if (this.RadioUndoPayout.Checked)
        {
            this.LabelActionDescription.Text =
                String.Format("Payout #{0} will be zeroed and closed. {1} will be re-opened and ready for payout.",
                              thisPayout.Identity, GetDependencyString(thisPayout));
            valid = true;
            this.ButtonExecute.Text = "Execute UNDO";
            this.ButtonExecute.Enabled = true;
        }

        if (!valid)
        {
            this.ButtonExecute.Enabled = false;
            this.ButtonExecute.Text = "Execute Nothing";
            this.LabelActionDescription.Text = "Nothing.";
        }
    }

    private static string GetDependencyString(params object[] payoutParams)
    {
        // This should be moved to some sort of interface layer for localization.

        List<int> expenseClaimIdentities = new List<int>();
        List<int> invoiceIdentities = new List<int>();
        List<int> salaryNetIdentities = new List<int>();
        List<int> salaryTaxIdentities = new List<int>();

        Payouts allPayouts = new Payouts();

        foreach (object parameter in payoutParams)
        {
            if (parameter is Payout)
            {
                allPayouts.Add((Payout) parameter);
            }

            if (parameter is Payouts)
            {
                foreach (Payout payout in (Payouts)parameter)
                {
                    allPayouts.Add(payout);
                }
            }
        }

        foreach (Payout payout in allPayouts)
        {
            expenseClaimIdentities.AddRange(payout.DependentExpenseClaims.Identities);
            invoiceIdentities.AddRange(payout.DependentInvoices.Identities);
            salaryNetIdentities.AddRange(payout.DependentSalariesNet.Identities);
            salaryTaxIdentities.AddRange(payout.DependentSalariesTax.Identities);
        }

        List<string> parts = new List<string>();

        if (expenseClaimIdentities.Count > 0)
        {
            parts.Add(GenerateDependencyPart("Expense Claim", "Expense Claims", expenseClaimIdentities));
        }

        if (invoiceIdentities.Count > 0)
        {
            parts.Add(GenerateDependencyPart("Invoice", "Invoices", invoiceIdentities));
        }

        if (salaryNetIdentities.Count > 0)
        {
            parts.Add(GenerateDependencyPart("Salary Net", "Salaries Net", salaryNetIdentities));
        }

        if (salaryTaxIdentities.Count > 0)
        {
            parts.Add(GenerateDependencyPart("Salary Tax", "Salaries Tax", salaryTaxIdentities));
        }


        if (parts.Count == 0)
        {
            return "Nothing";
        }

        string result = string.Empty;

        for (int index = 0; index < parts.Count - 1; index++)
        {
            result += ", " + parts[index];
        }

        if (parts.Count > 2)
        {
            result += ",";
        }

        if (parts.Count > 1)
        {
            result = result.Substring(2) + " and ";
        }

        result += parts[parts.Count - 1];

        return result;
    }


    private static string GenerateDependencyPart (string singular, string plural, List<int> identities)
    {
        if (identities.Count > 0)
        {
            if (identities.Count > 1)
            {
                return plural + " " +
                          Activizr.Logic.Support.Formatting.GenerateRangeString(identities);
            }
            else
            {
                return singular + " #" + identities[0].ToString();
            }
        }

        return string.Empty;
    }


    protected void RadioManualMap_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    protected void RadioManualMerge_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    protected void RadioUndoPayout_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    protected void DropPayouts_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }



    protected void ButtonExecute_Click(object sender, EventArgs e)
    {
        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, thisPayout.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        if (this.RadioManualMap.Checked)
        {
            int transactionId = Int32.Parse(this.DropTransactions.SelectedValue);
            FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);

            thisPayout.BindToTransactionAndClose(transaction, _currentUser);
        }
        else if (this.RadioManualMerge.Checked)
        {
            int otherPayoutId = Int32.Parse(this.DropPayouts.SelectedValue);
            Payout otherPayout = Payout.FromIdentity(otherPayoutId);

            otherPayout.MigrateDependenciesTo(thisPayout);
            otherPayout.Open = false;
            thisPayout.Reference = GetDependencyString(thisPayout);
        }
        else if (this.RadioUndoPayout.Checked)
        {
            thisPayout.UndoPayout();
        }

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }
}