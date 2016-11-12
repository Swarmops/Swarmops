using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_v4_FixIncompleteTransactions : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        //RadAjaxPanel1.Width = Unit.Percentage(100); //Has a fixed width in markup to help edit it.

        Person currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));

        if (!Page.IsPostBack)
        {
            this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString()));
            if (currentUser.Identity == 1)
            {
                // Ok, so this is an ugly-as-all-hell hack. But it keeps me sane.

                this.DropOrganizations.Items.Add(new ListItem("Rick's Sandbox", "40"));
            }

            PopulateGrid();
            PopulateAccounts(Int32.Parse(this.DropOrganizations.SelectedValue));
            PopulatePayouts(Int32.Parse(this.DropOrganizations.SelectedValue));
        }
    }

    protected void PopulateGrid ()
    {
        this.GridTransactions.DataSource = FinancialTransactions.GetIncomplete(Int32.Parse(this.DropOrganizations.SelectedValue));
    }

    protected void PopulatePayouts(int organizationId)
    {
        this.GridPayouts.DataSource = Payouts.ForOrganization(Organization.FromIdentity(organizationId));
    }

    protected void GridPayouts_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        // This is FUCKING REDUNDANT. We already HAVE this data. We shouldn't need to get it AGAIN.

        PopulatePayouts(Int32.Parse(this.DropOrganizations.SelectedValue));
    }

    protected void PopulatePaymentGroups(int organizationId)
    {
        this.GridPaymentGroups.DataSource = PaymentGroups.ForOrganization(Organization.FromIdentity(organizationId));
    }

    protected void GridPaymentGroups_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        // This is FUCKING REDUNDANT. We already HAVE this data. We shouldn't need to get it AGAIN.

        PopulatePaymentGroups(Int32.Parse(this.DropOrganizations.SelectedValue));
    }

    protected void PopulateOutboundInvoices(int organizationId)
    {
        this.GridOutboundInvoices.DataSource = OutboundInvoices.ForOrganization(Organization.FromIdentity(organizationId));
    }

    protected void GridOutboundInvoices_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        // This is FUCKING REDUNDANT. We already HAVE this data. We shouldn't need to get it AGAIN.

        PopulateOutboundInvoices(Int32.Parse(this.DropOrganizations.SelectedValue));
    }

    private void PopulateAccounts(int organizationId)
    {
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.FromIdentity(organizationId));

        this.DropAutoBalanceAccount.Items.Clear();
        this.DropAutoBalanceAccount.Items.Add(new ListItem("-- Select account --", "0"));
        foreach (FinancialAccount account in accounts)
        {
            this.DropAutoBalanceAccount.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
        }
    }


    private Int64 unbalancedCentsTotal = 0;
    private int unbalancedCount = 0;
    private int undocumentedCount = 0;


    protected void GridTransactions_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            // This is actually quite ineffective as Rows does plenty of lookups. A cache would
            // speed this up, but otoh, it's a very rare call.
            /*
            int transactionIdentity = (int) e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["Identity"];
            FinancialTransaction transaction = FinancialTransaction.FromIdentity (transactionIdentity);
            FinancialTransactionRows rows = transaction.Rows;*/

            FinancialTransaction transaction = (FinancialTransaction)e.Item.DataItem;

            if (transaction == null)
            {
                return;
            }

            FinancialTransactionRows rows = transaction.Rows;

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowTransactionForm('{0}','{1}');",
                                                           transaction.Identity, e.Item.ItemIndex);

            Label labelAccount = (Label)e.Item.FindControl("LabelAccount");
            if (rows.Count > 1)
            {
                labelAccount.Text = "[Several]";
            }
            else
            {
                labelAccount.Text = rows[0].Account.Name;
            }

            Int64 amountCentsTotal = rows.AmountCentsTotal;

            Label labelBalance = (Label)e.Item.FindControl("LabelBalance");
            labelBalance.Text = (amountCentsTotal / 100.0).ToString("N2", new System.Globalization.CultureInfo("sv-SE"));

            Label labelProblems = (Label)e.Item.FindControl("LabelProblems");

            List<string> problems = new List<string>();

            if (amountCentsTotal != 0.0)
            {
                problems.Add("Unbalanced");
                unbalancedCentsTotal += amountCentsTotal;
                unbalancedCount++;
            }

            if (rows.BalanceCentsDelta < 0 && transaction.Documents.Count == 0)
            {
                problems.Add("Undocumented");
                undocumentedCount++;
            }

            labelProblems.Text = String.Join(", ", problems.ToArray());
        }

        if (e.Item is GridFooterItem)
        {
            Label labelBalance = (Label)e.Item.FindControl("LabelBalanceAccumulated");
            labelBalance.Text = "Total Diff: " + (unbalancedCentsTotal / 100.0).ToString("N2", new System.Globalization.CultureInfo("sv-SE"));

            Label labelProblemTotals = (Label)e.Item.FindControl("LabelProblemTotals");
            labelProblemTotals.Text = String.Format("Unbal {0}, Undoc {1}", unbalancedCount, undocumentedCount);
        }
    }

    protected void GridPayouts_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            Payout payout = (Payout)e.Item.DataItem;

            if (payout == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowPayoutForm('{0}','{1}');",
                                                           payout.Identity, e.Item.ItemIndex);
        }

    }


    protected void GridPaymentGroups_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            PaymentGroup group = (PaymentGroup)e.Item.DataItem;

            if (group == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowPaymentGroupForm('{0}','{1}');",
                                                           group.Identity, e.Item.ItemIndex);
        }

    }


    protected void GridOutboundInvoices_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            OutboundInvoice invoice = (OutboundInvoice)e.Item.DataItem;

            if (invoice == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowOutboundInvoiceForm('{0}','{1}');",
                                                           invoice.Identity, e.Item.ItemIndex);
        }

    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind" || e.Argument== "RebindAndNavigate")
        {
            //this.GridTransactions.MasterTableView.SortExpressions.Clear();
            //this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
            this.GridTransactions.Rebind();
            PopulatePayouts(Int32.Parse(this.DropOrganizations.SelectedValue));
            this.GridPayouts.Rebind();
            PopulatePaymentGroups(Int32.Parse(this.DropOrganizations.SelectedValue));
            this.GridPaymentGroups.Rebind();
            PopulateOutboundInvoices(Int32.Parse(this.DropOrganizations.SelectedValue));
            this.GridOutboundInvoices.Rebind();

        }
    }


    protected void ButtonAutoBalance_Click (object sender, EventArgs e)
    {
        Person currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = currentUser.GetAuthority();

        if (!authority.HasPermission(Permission.CanDoEconomyTransactions, Int32.Parse(this.DropOrganizations.SelectedValue), -1, Authorization.Flag.ExactOrganization))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "validationfailed",
                                                "alert ('You do not have access to changing financial records.');",
                                                true);
            return;
        }


        if (this.DropAutoBalanceAccount.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "validationfailed",
                                                "alert ('Please select an account to auto-balance against.');",
                                                true);
            return;
        }

        FinancialAccount autoBalanceAccount = FinancialAccount.FromIdentity(Int32.Parse(this.DropAutoBalanceAccount.SelectedValue));

        if (this.GridTransactions.SelectedIndexes.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "validationfailed",
                                                "alert ('Please select one or more transactions to auto-balance.');",
                                                true);

            return;
        }

        foreach (string indexString in this.GridTransactions.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int transactionId = (int)this.GridTransactions.MasterTableView.DataKeyValues[index]["Identity"];

            FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);

            transaction.AddRow(autoBalanceAccount, -transaction.Rows.AmountCentsTotal, currentUser);
        }

        PopulateGrid();
        this.GridTransactions.Rebind();
        this.DropAutoBalanceAccount.SelectedValue = "0";
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        PopulateAccounts(Int32.Parse(this.DropOrganizations.SelectedValue));
        PopulateGrid();
        PopulatePayouts(Int32.Parse(this.DropOrganizations.SelectedValue));
        this.GridPayouts.Rebind();
        this.GridTransactions.Rebind();
    }


    protected void ButtonAutoRemap_Click (object sender, EventArgs e)
    {
        Payouts.AutomatchAgainstUnbalancedTransactions(Organization.FromIdentity(Int32.Parse(this.DropOrganizations.SelectedValue)));
        PopulateGrid();
        PopulatePayouts(Int32.Parse(this.DropOrganizations.SelectedValue));
        this.GridPayouts.Rebind();
        this.GridTransactions.Rebind();
    }
}