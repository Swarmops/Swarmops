<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="CreateOutboundInvoice.aspx.cs" Inherits="Pages_v4_Financial_CreateOutboundInvoice" Title="Inbound Invoice Received - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OutboundInvoiceGrid.ascx" TagName="OutboundInvoiceGrid" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="table_add.png" Title="Create Outbound Invoice" Description="Demand payment from another organization"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
            <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Create a new invoice</span><br />
            <div class="DivGroupBoxContents">
                
                <b>Step 1:</b> What <b>organization</b> is sending this invoice?<br />
                <div style="float:left;width:100px">Organization:</div><asp:DropDownList ID="DropOrganizations" runat="server"><asp:ListItem Selected="True" Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br />
                <b>Step 2:</b> <b>Who</b> is this invoice for, and how do we <b>reach them</b>?<br />
                <div style="float:left;width:100px">Customer:<br />Domestic?<br />Mail Address:<br />Paper Address:</div>
                <asp:TextBox ID="TextCustomer" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextCustomer_Required" runat="server" EnableClientScript="false" 
                                    ErrorMessage="Please enter the customer's name." 
                    ControlToValidate="TextCustomer" /><br />
                    <asp:CheckBox ID="CheckDomestic" Checked="true" runat="server" Text="This is a domestic customer." />&nbsp;
                                
                                <br /><asp:TextBox ID="TextMailAddress" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                    ID="Validator_TextMailAddress_Required" runat="server" EnableClientScript="false"
                    Display="Dynamic"                
                    ErrorMessage="Please enter the email address for invoicing." 
                    ControlToValidate="TextMailAddress" />
                                <br /><asp:TextBox ID="TextPaperAddress" TextMode="MultiLine" Rows="5" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                    ID="Validator_TextPaperAddress_Required" runat="server" EnableClientScript="false"
                    Display="Dynamic"                
                    ErrorMessage="Please enter the paper address for invoicing." 
                    ControlToValidate="TextPaperAddress" />
                                <br />
                <b>Step 3:</b> Does the organization want a <b>reference</b> on the invoice (such as a name, budget, or P.O. number)?<br />
                 <div style="float:left;width:100px">Reference:</div>
                <asp:TextBox ID="TextTheirReference" runat="server" />&nbsp;(optional)<br />
                <b>Step 4:</b> What <b>budget</b> whould be credited within <asp:Label ID="Label1" runat="server">the organization</asp:Label>?<br />
                                <div style="float:left;width:100px">Budget:</div>
                <pw4:FinancialAccountTreeDropDown ID="DropBudgets" runat="server" OnSelectedNodeChanged="DropBudgets_SelectedNodeChanged" />
                                
                                &nbsp;<asp:Label ID="LabelBudgetOwner" runat="server" />
                                <asp:CustomValidator ID="Validator_DropBudgets_Custom" runat="server" 
                                    ErrorMessage="Please select a budget." 
                                    onservervalidate="Validator_DropBudgets_Custom_ServerValidate" /><br />
                <b>Step 5:</b> When is <b>payment</b> due?<br />
                <div style="float:left;width:100px">Due</div><telerik:RadDatePicker ID="DatePicker" runat="server" />&nbsp;<asp:CustomValidator 
                                    ID="Validator_DatePicker_Custom" runat="server" 
                                    ErrorMessage="Please select a valid date for payment."
                                    onservervalidate="Validator_DatePicker_Custom_ServerValidate" /><br />
                <b>Step 6:</b> Enter the <b>invoice items</b> and their <b>amounts</b> (in SEK).<br />
                
                <asp:UpdatePanel ID="UpdateItems" runat="server" UpdateMode="Always"><ContentTemplate>
                <div style="float:left;width:100px"><asp:Literal ID="LiteralLeftItemSpacer" Text="&nbsp;" runat="server" /></div><asp:Literal ID="LiteralItems" runat="server" /><asp:TextBox ID="TextNewItemDescription" runat="server" />&nbsp;<asp:TextBox ID="TextNewItemAmount" runat="server" />&nbsp;<asp:Button 
                        ID="ButtonAddItem" runat="server" Text="Add" onclick="ButtonAddItem_Click" /><br />
                </ContentTemplate>
                </asp:UpdatePanel>

                <b>Step 7:</b> All done? Register the invoice for mailout to the customer.<br />
                <div style="float:left;width:100px">All done?</div>
                <asp:Button ID="ButtonSubmitInvoice" runat="server" Text="All Done!" 
                    onclick="ButtonSubmitInvoice_Click" />&nbsp;<br />
                
                
            </div>
        </div>

        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Recent outbound invoices</span><br />
            <div class="DivGroupBoxContents">
                <pw4:OutboundInvoiceGrid runat="server" ID="GridOutboundInvoices" />
            </div>
        </div>
    </div>


</asp:Content>

