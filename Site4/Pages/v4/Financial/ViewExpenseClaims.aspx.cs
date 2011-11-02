using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

public partial class Pages_v4_Accounting_ViewEditExpenseClaims : PageV4Base
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
        ExpenseClaims allClaims = ExpenseClaims.FromOrganization(Organization.PPSE);

        allClaims.Sort(SortGridItems);

        this.GridExpenseClaims.DataSource = allClaims;
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
        // Set the images for the status indicators.

        // TODO: The warning lights.

        string imageUrlTodo = "~/Images/Public/Fugue/icons-shadowless/minus-small.png";
        string imageUrlTick = "~/Images/Public/Fugue/icons-shadowless/tick.png";
        string imageUrlFail = "~/Images/Public/Fugue/icons-shadowless/cross-circle-frame.png";

        if (e.Item is GridDataItem)
        {
            ExpenseClaim claim = (ExpenseClaim) e.Item.DataItem;

            if (claim == null)
            {
                return;
            }

            Image imageClaimed = (Image) e.Item.FindControl("ImageClaimed");
            Image imageAttested = (Image) e.Item.FindControl("ImageAttested");
            Image imageDocumented = (Image) e.Item.FindControl("ImageDocumented");
            Image imageRepaid = (Image) e.Item.FindControl("ImageRepaid");
            Label labelBudgetYear = (Label) e.Item.FindControl("LabelBudgetYear");

            imageClaimed.ImageUrl = claim.Claimed ? imageUrlTick : imageUrlTodo;
            imageAttested.ImageUrl = claim.Attested ? imageUrlTick : imageUrlTodo;
            imageDocumented.ImageUrl = claim.Validated ? imageUrlTick : imageUrlTodo;

            if (claim.Open == false && (claim.Repaid == false || claim.Validated == false))
            {
                imageRepaid.ImageUrl = imageUrlFail;
            }
            else
            {
                imageRepaid.ImageUrl = claim.Repaid ? imageUrlTick : imageUrlTodo;
            }

            if (claim.BudgetId == 0)
            {
                labelBudgetYear.Text = "UNBUDGETED!";
            }
            else
            {
                labelBudgetYear.Text = claim.Budget.Name + ", " + claim.BudgetYear.ToString();
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";


            if (claim.Open)
            {
                editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
                                                               claim.Identity, e.Item.ItemIndex);
            }
            else
            {
                editLink.Visible = false;
            }


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

}


