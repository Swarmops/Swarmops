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
using Activizr.Logic.Financial;
using Activizr.Logic.Support;
using Telerik.Web.UI;
using Activizr.Logic.Structure;
using Activizr.Logic.Security;

public partial class Pages_v4_Financial_ValidateExpenseDocumentation : PageV4Base
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
        ExpenseClaims allClaims = ExpenseClaims.ForOrganization(Organization.PPSE);

        ExpenseClaims unvalidatedClaims = new ExpenseClaims();

        // LINQ would be nice here. "Where Validated=0".

        foreach (ExpenseClaim claim in allClaims)
        {
            if (!claim.Validated)
            {
                unvalidatedClaims.Add(claim);
            }
        }

        unvalidatedClaims.Sort(SortGridItems);

        this.GridExpenseClaims.DataSource = unvalidatedClaims;
    }


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.CreatedDateTime, claim1.CreatedDateTime);
    }


    protected void GridExpenseClaims_ItemDataBound(object sender, GridItemEventArgs e)
    {
        // Don't care -- use ItemCreated
    }


    protected void GridExpenseClaims_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            ExpenseClaim claim = (ExpenseClaim) e.Item.DataItem;

            if (claim == null)
            {
                return;
            }

            Controls_v4_DocumentList docList = (Controls_v4_DocumentList) e.Item.FindControl("DocumentListClaim");

            if (docList != null)
            {
                docList.Documents = Documents.ForObject(claim);
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
                                                           claim.Identity, e.Item.ItemIndex);

        }
    }

    protected void GridExpenseClaims_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
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
            this.GridExpenseClaims.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }


    protected void ButtonValidate_Click(object sender, EventArgs e)
    {
        List<string> identityStrings = new List<string>();

        foreach (string indexString in this.GridExpenseClaims.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int claimId = (int)this.GridExpenseClaims.MasterTableView.DataKeyValues[index]["Identity"];
            ExpenseClaim claim = ExpenseClaim.FromIdentity(claimId);

            // Mark as validated

            if (_authority.HasPermission(Permission.CanDoEconomyTransactions, Organization.PPSEid, -1, Authorization.Flag.ExactOrganization))
            {
                claim.Validate(_currentUser);
                Activizr.Logic.Support.PWEvents.CreateEvent(
                    EventSource.PirateWeb, EventType.ExpenseValidated, _currentUser.Identity,
                    claim.OrganizationId, 0, claim.ClaimingPersonId, claimId, string.Empty);
            }
        }

        // TODO: Create event, so that expenser is informed. Use 'identityStrings'.

        this.GridExpenseClaims.Rebind();

    }
}


