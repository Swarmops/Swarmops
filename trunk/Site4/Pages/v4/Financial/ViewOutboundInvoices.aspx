<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ViewOutboundInvoices.aspx.cs" Inherits="Pages_v4_Financial_ViewOutboundInvoices" Title="List Outbound Invoices - PirateWeb" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OutboundInvoiceGrid.ascx" TagName="OutboundInvoiceGrid" TagPrefix="pw4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="table_multiple.png" Title="List Outbound Invoices" Description="View the registry of sent invoices and their status in processing" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Select Organization</span><br />
            <div class="DivGroupBoxContents">
                Only <b>Piratpartiet SE</b> for now.
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Outbound Invoices from Piratpartiet SE</span><br />
            <div class="DivGroupBoxContents">
                <pw4:OutboundInvoiceGrid runat="server" ID="GridInvoices" />
            </div>
        </div>
    </div>

</asp:Content>

