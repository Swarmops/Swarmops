<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ViewInboundInvoices.aspx.cs" Inherits="Pages_v4_Financial_ViewInboundInvoices" Title="Untitled Page" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/InboundInvoiceGrid.ascx" TagName="InboundInvoiceGrid" TagPrefix="pw4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="table_multiple.png" Title="List/Search Invoices" Description="View the registry of received invoices and their status in processing" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Select Organization</span><br />
            <div class="DivGroupBoxContents">
                Only <b>Piratpartiet SE</b> for now.
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Inbound Invoices for Piratpartiet SE</span><br />
            <div class="DivGroupBoxContents">
                <pw4:InboundInvoiceGrid runat="server" ID="GridInvoices" />
            </div>
        </div>
    </div>

</asp:Content>

