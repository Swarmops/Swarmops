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


public partial class Controls_v4_OutboundInvoiceGrid : System.Web.UI.UserControl
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
                "Unspecified organization in Controls_v4_OutboundInvoiceList.Page_PreRender");
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
        OutboundInvoices invoices = OutboundInvoices.ForOrganization(this.Organization, true);
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
            OutboundInvoice invoice = (OutboundInvoice)e.Item.DataItem;

            if (invoice == null)
            {
                return;
            }

            Image imagePaid = (Image)e.Item.FindControl("ImagePaidClosed");

            imagePaid.ImageUrl = invoice.Open ? imageUrlTodo : imageUrlTick;

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "http://data.piratpartiet.se/Forms/DisplayOutboundInvoice.aspx?Reference=" + invoice.Reference + "&Culture=" + (invoice.Domestic? invoice.Organization.DefaultCountry.Culture.Replace("-", "").ToLower(): "enus");
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
