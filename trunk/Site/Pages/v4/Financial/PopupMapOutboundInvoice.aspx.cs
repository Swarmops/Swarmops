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
using Membership = System.Web.Security.Membership;

public partial class Pages_v4_Financial_PopupMapOutboundInvoice : PageV4Base
{
    private OutboundInvoice thisInvoice = null;

    protected void Page_Load (object sender, EventArgs e)
    {

        int thisInvoiceId = Int32.Parse(Request.QueryString["OutboundInvoiceId"]);
        thisInvoice = OutboundInvoice.FromIdentity(thisInvoiceId);
        int year = DateTime.Today.Year;

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, thisInvoice.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        if (!Page.IsPostBack)
        {
            // Populate all data

            this.LabelHeader.Text = String.Format("Mapping Outbound Invoice #{0} ({1:N2})", thisInvoice.Identity, thisInvoice.Amount);
            PopulateDropTransactions();
        }
    }

    private void PopulateDropTransactions()
    {
        int orgId = thisInvoice.OrganizationId;
        FinancialTransactions unbalancedTransactions =
            FinancialTransactions.GetUnbalanced(Organization.FromIdentity(orgId));

        this.DropTransactions.Items.Clear();
        this.DropTransactions.Items.Add(new ListItem("-- Select a transaction --", "0"));

        int matchCount = 0;
        foreach (FinancialTransaction transaction in unbalancedTransactions)
        {
            if (transaction.Rows.AmountCentsTotal == thisInvoice.AmountCents)
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
                                      transaction.DateTime, transactionDescription),
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

    /*
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
    }*/


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

        Dictionary<double, List<int>> transactionDeltas = new Dictionary<double, List<int>>();
        this.ButtonExecute.Enabled = false;

        if (this.RadioManualMap.Checked)
        {

            int transactionId = Int32.Parse(this.DropTransactions.SelectedValue);

            if (transactionId > 0)
            {
                FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);

                this.LabelActionDescription.Text =
                    String.Format(
                        "Outbound Invoice #{0} to {1}, supposed date {2:yyyy-MM-dd}, will be mapped to Transaction #{3} on date {4:yyyy-MM-dd}. Transaction #{3} will be balanced, documented, and tied to a new payment group with a single payment referring to Outbound Invoice #{0}. Outbound Invoice #{0} will be closed.",
                        thisInvoice.Identity, thisInvoice.CustomerName, thisInvoice.DueDate, transaction.Identity, transaction.DateTime);
                valid = true;
                this.ButtonExecute.Text = "Execute MAP";
                this.ButtonExecute.Enabled = true;
            }
        }

        if (this.RadioCreditInvoice.Checked)
        {
            this.LabelActionDescription.Text =
                String.Format("A credit invoice for the same amount as Outbound Invoice #{0}, {1} {2:N2}, will be issued to {3} and closed immediately together with Outbound Invoice #{0}.",
                              thisInvoice.Identity, thisInvoice.Currency.Code, thisInvoice.Amount, thisInvoice.CustomerName);
            valid = true;
            this.ButtonExecute.Text = "Execute CREDIT";
            this.ButtonExecute.Enabled = true;
        }

        if (!valid)
        {
            this.ButtonExecute.Enabled = false;
            this.ButtonExecute.Text = "Execute Nothing";
            this.LabelActionDescription.Text = "Nothing.";
        }
    }


    protected void RadioManualMap_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    protected void RadioManualMerge_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    protected void RadioCreditInvoice_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }

    protected void DropPayouts_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateExecute();
    }



    protected void ButtonExecute_Click(object sender, EventArgs e)
    {
        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, thisInvoice.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        if (this.RadioManualMap.Checked)
        {
            int transactionId = Int32.Parse(this.DropTransactions.SelectedValue);
            FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);

            Payment payment = Payment.CreateSingle(thisInvoice.Organization, transaction.DateTime, thisInvoice.Currency, thisInvoice.AmountCents, thisInvoice, _currentUser);
            payment.AddInformation(PaymentInformationType.Freeform, "Mapped manually in PirateWeb");

            transaction.AddRow(thisInvoice.Organization.FinancialAccounts.AssetsOutboundInvoices, -thisInvoice.AmountCents, _currentUser);
            transaction.Dependency = payment.Group;
        }
        else if (this.RadioCreditInvoice.Checked)
        {
            thisInvoice.Credit(_currentUser);
        }

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }
}