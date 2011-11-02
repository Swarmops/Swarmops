<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="CreateInboundInvoice.aspx.cs" Inherits="Pages_v4_Financial_CreateInboundInvoice" Title="Inbound Invoice Received - PirateWeb" %>

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
<%@ Register Src="~/Controls/v4/InboundInvoiceGrid.ascx" TagName="InboundInvoiceGrid" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="table_add.png" Title="Received Invoice" Description="Register a new, received invoice for attestation and payment"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
            <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Register a new inbound invoice</span><br />
            <div class="DivGroupBoxContents">
                
                <b>Step 1:</b> What <b>organization</b> is the invoice addressed to?<br />
                <div style="float:left;width:100px">Organization:</div><asp:DropDownList ID="DropOrganizations" runat="server"><asp:ListItem Selected="True" Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br />
                <b>Step 2:</b> <b>Who</b> sent this invoice, and what is their <b>account number</b>?<br />
                <div style="float:left;width:100px">Supplier:<br />Account:</div>
                <asp:TextBox ID="TextSupplier" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextSupplier_Required" runat="server" EnableClientScript="false" 
                                    ErrorMessage="Please enter the supplier's name." 
                    ControlToValidate="TextSupplier" />
                                
                                <br /><asp:TextBox ID="TextAccount" runat="server" />&nbsp;(e.g. <i>Bg 1234-5678</i>)&nbsp;<asp:RequiredFieldValidator 
                    ID="Validator_TextAccount_Required" runat="server" EnableClientScript="false" 
                    Display="Dynamic"
                    ErrorMessage="Please enter the supplier's account for payment." 
                    ControlToValidate="TextAccount" />
                                <br />
                <b>Step 3:</b> What <b>amount</b> is the invoice, when is payment <b>due</b>, and what is its <b>reference</b> (OCR sequence or invoice#)?<br />
                <div style="float:left;width:100px">Amount:<br />Due date:<br />Reference:<br />Ref type:</div>
                <asp:TextBox ID="TextAmount" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextAmount_Required" runat="server" EnableClientScript="false"
                                    ErrorMessage="Please type an amount." InitialValue="0,00" 
                    ControlToValidate="TextAmount" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="Validator_TextAmount_Custom" runat="server" 
                                    ErrorMessage="Please type a numeric amount." 
                    Display="Dynamic" 
                    onservervalidate="Validator_TextAmount_Custom_ServerValidate" />
                                <br /><telerik:RadDatePicker ID="DatePicker" runat="server" />&nbsp;<asp:CustomValidator 
                                    ID="Validator_DatePicker_Custom" runat="server" 
                                    ErrorMessage="Please select a valid date for payment."
                                    onservervalidate="Validator_DatePicker_Custom_ServerValidate" />
                                <br /><asp:TextBox ID="TextReference" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextReference_Required" runat="server" EnableClientScript="false" 
                                    ErrorMessage="Please type the invoice reference (OCR sequence or invoice number)." 
                    ControlToValidate="TextReference" />
                                <br /><asp:DropDownList ID="DropReferenceType" runat="server"><asp:ListItem Selected="True" Value="0">-- Select one--</asp:ListItem><asp:ListItem Value="OCR">OCR sequence</asp:ListItem><asp:ListItem Value="Serial">Invoice Number</asp:ListItem></asp:DropDownList>&nbsp;
                                <asp:CompareValidator ControlToValidate="DropReferenceType" Operator="NotEqual" ValueToCompare="0" runat="server" EnableClientScript="false"
                                ErrorMessage="Please select a reference type." />
                                <br />
                <b>Step 4:</b> What <b>budget</b> should be charged within <asp:Label ID="LabelOrganizationCopy" runat="server">the organization</asp:Label>? The budget owner must attest.<br />
                <div style="float:left;width:100px">Budget:</div>
                <pw4:FinancialAccountTreeDropDown ID="DropBudgets" runat="server" OnSelectedNodeChanged="DropBudgets_SelectedNodeChanged" />
                                
                                &nbsp;<asp:Label ID="LabelBudgetOwner" runat="server" />
                                <asp:CustomValidator ID="Validator_DropBudgets_Custom" runat="server" 
                                    ErrorMessage="Please select a budget." 
                                    onservervalidate="Validator_DropBudgets_Custom_ServerValidate" /><br />
                <b>Step 5:</b> Upload <b>a scan</b> of the invoice document or documents.<br />
                <div style="float:left;width:100px">Documents:<br />Upload:<br />&nbsp;</div><asp:Label Visible="false" ID="TemporaryDocumentIdentity" Text="0" runat="server" />
                <pw4:DocumentList ID="DocumentList" runat="server" />
                                <asp:CustomValidator ID="Validator_DocumentList_Custom" runat="server" 
                                    ErrorMessage="Please upload documentation supporting the expense." 
                                    onservervalidate="Validator_DocumentList_Custom_ServerValidate"></asp:CustomValidator>
                                <br />
                <telerik:RadUpload ID="Upload" Runat="server" ControlObjectsVisibility="None" 
                    MaxFileInputsCount="1" EnableFileInputSkinning="false"
                    TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" /><asp:Button ID="ButtonUpload" 
                                    runat="server" Text="Upload file" 
                    onclick="ButtonUpload_Click" CausesValidation="False" />&nbsp;<br clear="all"/>
                <b>Step 6:</b> All done? Register the invoice for attestation by the budget owner and payout to the supplier.<br />
                <div style="float:left;width:100px">All done?</div>
                <asp:Button ID="ButtonSubmitClaim" runat="server" Text="All Done!" 
                    onclick="ButtonSubmitClaim_Click" />&nbsp;<br />
                
                
            </div>
        </div>

        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Recent inbound invoices</span><br />
            <div class="DivGroupBoxContents">
                <pw4:InboundInvoiceGrid runat="server" ID="GridInboundInvoices" />
            </div>
        </div>
    </div>


</asp:Content>

