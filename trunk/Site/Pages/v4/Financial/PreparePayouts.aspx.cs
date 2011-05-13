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
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Logic.Security;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_PreparePayouts : PageV4Base
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
        Payouts payouts = Payouts.Construct(Organization.PPSEid);

        this.GridPayouts.DataSource = payouts;
    }


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.CreatedDateTime, claim1.CreatedDateTime);
    }


    protected void GridPayouts_ItemDataBound(object sender, GridItemEventArgs e)
    {
        // Don't care -- use ItemCreated
    }


    protected void GridPayouts_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            Payout payout = (Payout) e.Item.DataItem;

            if (payout == null)
            {
                return;
            }

            Label labelDueDate = (Label)e.Item.FindControl("LabelDueDate");
            if (payout.ExpectedTransactionDate < DateTime.Now)
            {
                labelDueDate.Text = "ASAP";
            }
            else
            {
                labelDueDate.Text = payout.ExpectedTransactionDate.ToString("yyyy-MM-dd");
            }


            /*
            Controls_v4_DocumentList docList = (Controls_v4_DocumentList) e.Item.FindControl("DocumentListClaim");

            if (docList != null)
            {
                docList.Documents = Documents.ForObject(claim);
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
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
            this.GridPayouts.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }


    protected void ButtonPay_Click(object sender, EventArgs e)
    {
        List<string> identityStrings = new List<string>();

        if (_authority.HasPermission(Permission.CanPayOutMoney, Organization.PPSEid, -1, Authorization.Flag.ExactOrganization))
        {

            foreach (string indexString in this.GridPayouts.SelectedIndexes)
            {
                // Creating the payout closes the invoices and expense claims.

                int index = Int32.Parse(indexString);
                string protoIdentity = (string) this.GridPayouts.MasterTableView.DataKeyValues[index]["ProtoIdentity"];
                Payout payout = Payout.CreateFromProtoIdentity(_currentUser, protoIdentity);

                Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.PayoutCreated,
                                                           _currentUser.Identity, 1, 1, 0, payout.Identity,
                                                           protoIdentity);
            }

            PopulateGrid();
            this.GridPayouts.Rebind();
        }

    }
}


