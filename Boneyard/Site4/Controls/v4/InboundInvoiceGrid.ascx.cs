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
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Controls_v4_InboundInvoiceGrid : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ViewState[this.ClientID] == null)
        {
            this.ViewState[this.ClientID] = organizationId;
        }

        if (organizationId == 0)
        {
            organizationId = (int) this.ViewState[this.ClientID];
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (organizationId == 0)
        {
            throw new InvalidOperationException(
                "Unspecified organization in Controls_v4_InboundInvoiceList.Page_PreRender");
        }

        PopulateGrid();
    }



    private int organizationId;

    public Organization Organization
    {
        set
        {
            organizationId = value.Identity;
            this.ViewState[this.ClientID] = organizationId;
        }
        get
        {
            return Organization.FromIdentity(organizationId);
        }
    }

    private void PopulateGrid()
    {
        InboundInvoices invoices = InboundInvoices.ForOrganization(this.Organization, true);
        invoices.Reverse();

        this.GridInvoices.DataSource = invoices;
        this.GridInvoices.Rebind();
    }


    public void Reload()
    {
        PopulateGrid();
    }


    protected void GridInvoices_ItemCreated(object sender, GridItemEventArgs e)
    {
        // Set the images for the status indicators.

        const string imageUrlTodo = "~/Images/Public/Fugue/icons-shadowless/minus-small.png";
        const string imageUrlTick = "~/Images/Public/Fugue/icons-shadowless/tick.png";
        const string imageUrlFail = "~/Images/Public/Fugue/icons-shadowless/cross-circle-frame.png";

        if (e.Item is GridDataItem)
        {
            InboundInvoice invoice = (InboundInvoice)e.Item.DataItem;

            if (invoice == null)
            {
                return;
            }

            Controls_v4_DocumentList docList = (Controls_v4_DocumentList)e.Item.FindControl("DocumentList");

            if (docList != null)
            {
                docList.Documents = Documents.ForObject(invoice);
            }

            Image imageAttested = (Image)e.Item.FindControl("ImageAttested");
            Image imagePaid = (Image)e.Item.FindControl("ImageClosedPaid");

            imageAttested.ImageUrl = invoice.Attested ? imageUrlTick : imageUrlTodo;

            if (invoice.Open == false)
            {
                imagePaid.ImageUrl = invoice.Attested ? imageUrlTick : imageUrlFail;
            }
            else
            {
                imagePaid.ImageUrl = imageUrlTodo;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";

            if (invoice.Open)
            {
                editLink.Attributes["onclick"] = String.Format("return ShowInboundInvoiceForm('{0}','{1}');",
                                                               invoice.Identity, e.Item.ItemIndex);
            }
            else
            {
                editLink.Visible = false;
            }

        }




    }



    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            //this.GridTransactions.MasterTableView.SortExpressions.Clear();
            //this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }

}
