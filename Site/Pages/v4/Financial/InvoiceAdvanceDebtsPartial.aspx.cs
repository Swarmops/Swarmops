using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Logic.Security;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_InvoiceAdvanceDebts : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateGrid();
        }
    }


    private void PopulateGrid()
    {
        string personIdString = Request.QueryString["PersonId"];
        string organizationIdString = Request.QueryString["OrganizationId"];

        Person person = Person.FromIdentity(Int32.Parse(personIdString));
        Organization organization = Organization.FromIdentity(Int32.Parse(organizationIdString));

        this.LabelOrganization.Text = organization.Name;
        this.LabelDebtor.Text = person.Canonical;

        ExpenseClaims allClaims = ExpenseClaims.FromClaimingPersonAndOrganization(person, organization);  // again, this needs a "get open only"
        ExpenseClaims gridClaims = new ExpenseClaims();

        foreach (ExpenseClaim claim in allClaims)
        {
            // If ready for payout, add to list.

            if (claim.Open)
            {
                if (claim.Attested && claim.Validated && !claim.Repaid)
                {
                    gridClaims.Add(claim);
                }
            }
        }

        // Now, we have grouped all ready but unsettled expenses per person. Let's add only those with a positive debt to the final list.

        gridClaims.Sort(SortGridItems);

        this.GridDebts.DataSource = gridClaims;
    }


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        if (claim1.CreatedDateTime == claim2.CreatedDateTime)
        {
            return 0;
        }

        return (claim2.CreatedDateTime > claim1.CreatedDateTime? 1: -1);
    }


    protected void GridPayouts_ItemDataBound(object sender, GridItemEventArgs e)
    {
        // Don't care -- use ItemCreated
    }


    protected void GridPayouts_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            ExpenseClaim claim = (ExpenseClaim) e.Item.DataItem;

            if (claim == null)
            {
                return;
            }



            /*
            Label labelDueDate = (Label)e.Item.FindControl("LabelDueDate");
            if (payout.ExpectedTransactionDate < DateTime.Now)
            {
                labelDueDate.Text = "ASAP";
            }
            else
            {
                labelDueDate.Text = payout.ExpectedTransactionDate.ToString("yyyy-MM-dd");
            }*/


            /*
            Controls_v4_DocumentList docList = (Controls_v4_DocumentList) e.Item.FindControl("DocumentListClaim");

            if (docList != null)
            {
                docList.Documents = Documents.ForObject(claim);
            }*/
            
            Label labelDebt = (Label)e.Item.FindControl("LabelDebt");
            labelDebt.Text = (-claim.Amount).ToString("N2");
            /*editLink.Attributes["href"] = "InvoiceAdvanceDebtsPartial.aspx?PersonId=" + debt.Person.Identity;*/
            /*editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
                                                           claim.Identity, e.Item.ItemIndex);*/

        }
    }

    protected void GridPayouts_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        // This is FUCKING REDUNDANT. We already HAVE this data. We shouldn't need to get it AGAIN.

        PopulateGrid();
    }


    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            //this.GridTransactions.MasterTableView.SortExpressions.Clear();
            //this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
            this.GridDebts.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }


    protected void ButtonInvoice_Click(object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanPayOutMoney, Organization.PPSEid, -1, Authorization.Flag.ExactOrganization))
        {

            OutboundInvoice invoice = null;
            string personCanonical = string.Empty;

            foreach (string indexString in this.GridDebts.SelectedIndexes)
            {
                // Creating the invoice closes the expense claims.

                int index = Int32.Parse(indexString);
                int claimId = (int) (this.GridDebts.MasterTableView.DataKeyValues[index]["Identity"]);
                ExpenseClaim claim = ExpenseClaim.FromIdentity(claimId);

                if (invoice == null)
                {
                    CultureInfo oldCulture = CultureInfo.CurrentUICulture;
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Organization.PPSE.DefaultCountry.Culture);

                    invoice = OutboundInvoice.Create(Organization.PPSE, _currentUser,
                                                     DateTime.Today.AddDays(30),
                                                     Organization.PPSE.FinancialAccounts.
                                                         DebtsExpenseClaims,
                                                     claim.Claimer.Name,
                                                     claim.Claimer.Email,
                                                     claim.Claimer.Street + "\r\n" +
                                                     claim.Claimer.PostalCodeAndCity,
                                                     Organization.PPSE.DefaultCountry.Currency,
                                                     true, GetLocalResourceObject("InvoiceLiterals.ReclaimedCashAdvance").ToString());
                    personCanonical = claim.Claimer.Canonical;

                    Thread.CurrentThread.CurrentUICulture = oldCulture;
                }

                invoice.AddItem("Exp #" + claim.Identity.ToString() + ": " + claim.Description, -claim.AmountCents);
                claim.Repaid = true;
                claim.Open = false;
            }

            // Create transaction

            FinancialTransaction transaction = FinancialTransaction.Create(Organization.PPSEid, DateTime.Now,
                                                                           "Outbound Invoice #" + invoice.Identity +
                                                                           ": " + personCanonical);
            transaction.AddRow(Organization.PPSE.FinancialAccounts.DebtsExpenseClaims, -invoice.AmountCents, _currentUser);
            transaction.AddRow(Organization.PPSE.FinancialAccounts.AssetsOutboundInvoices, invoice.AmountCents, _currentUser);

            transaction.Dependency = invoice;

            // Create event

            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoiceCreated,
                                                       _currentUser.Identity, 1, 1, 0, invoice.Identity,
                                                       string.Empty);


            PopulateGrid();
            this.GridDebts.Rebind();
        }

    }

}


