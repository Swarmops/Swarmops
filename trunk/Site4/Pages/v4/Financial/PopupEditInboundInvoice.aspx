<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditInboundInvoice.aspx.cs" Inherits="Pages_v4_Financial_PopupEditInboundInvoice" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Inbound Invoice</title>
    
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />
        
    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().Close();
            GetRadWindow().BrowserWindow.refreshGrid(args);
        }
		
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz as well)
				
            return oWindow;
        }

        function CancelEdit()
        {
            GetRadWindow().Close();		
        }
        
        function ShowKill()
        {
            document.getElementById('DivKillValidation').style.display='inline';
        }
        
        function HideKill()
        {
            document.getElementById('DivKillValidation').style.display='none';
        }
        
    </script>
</head>
<body style="background-color: #f8e7ff; padding-left:10px; padding-top:10px; padding-right: 10px; padding-bottom: 10px; line-height: 30px">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="DivKillValidation" style="display:none;line-height:normal;margin:50px 50px 50px 50px;border:solid 5px red;padding:12px 15px 20px 15px;width:340px;position:absolute;background-color:#FFE0E0">
    <p>Are you sure you would like to <b>kill</b> this inbound invoice?</p><p>This will remove it from the list of outstanding invoices and undo its accounting. (Good reasons for this include duplicate or bogus invoices.)</p>
    <br /><br />
    <div style="float:right"><asp:LinkButton ID="ButtonConfirmedKill" runat="server" 
            Text="Yes, kill it." onclick="ButtonConfirmedKill_Click" /></div>
    <asp:HyperLink ID="LinkAbortKill" runat="server" NavigateUrl="javascript:HideKill();" Text="Oops. No." />
    </div>
    <div>
    <div style="width:100px;float:left">Invoice from:<br />Received date:<br />Due date:<br />Amount:<br />Budget:<br />Attested:<br />Documents:<br />&nbsp;</div>
    <asp:Label ID="LabelInvoiceFrom" runat="server" />&nbsp;<br />
    <asp:Label ID="LabelInvoiceReceivedDate" runat="server" />&nbsp;<br />
    <telerik:RadDatePicker ID="DateInvoiceDue" runat="server" 
                onselecteddatechanged="DateInvoiceDue_SelectedDateChanged" />&nbsp;<asp:CustomValidator 
                ID="Validator_DateInvoiceDue_Custom" Display="Dynamic" runat="server" OnServerValidate="Validator_DateInvoiceDue_Custom_ServerValidate" ErrorMessage="A valid date is required." /><br />
    <asp:TextBox ID="TextAmount" runat="server" ontextchanged="TextAmount_TextChanged" />&nbsp;<asp:CustomValidator 
                ID="Validator_TextAmount_Custom" Display="Dynamic" ControlToValidate="TextAmount" OnServerValidate="Validator_TextAmount_Custom_ServerValidate" runat="server" ErrorMessage="A Active amount is required." />
            <br />
    <pw4:FinancialAccountTreeDropDown ID="DropAccounts" OnSelectedNodeChanged="DropAccounts_SelectedNodeChanged" runat="server" />
            <asp:CustomValidator Display="Dynamic" ID="Validator_DropAccounts_Custom" OnServerValidate="Validator_DropAccounts_Custom_ServerValidate" runat="server" ErrorMessage="Required."></asp:CustomValidator>
            &nbsp;<br />
    <asp:Label ID="LabelAttested" runat="server" />&nbsp;<asp:HyperLink ID="LinkKillEnable" runat="server" Text="Kill this invoice?" NavigateUrl="javascript:ShowKill();" /><br />
    <pw4:DocumentList ID="DocumentList" runat="server" />&nbsp;<br />
    <asp:Button ID="ButtonSaveChanges" runat="server" Text="Save Changes" 
                onclick="ButtonSaveChanges_Click" />&nbsp;
    </div>
    
    <br />
    <asp:Panel ID="PanelWarningReattestation" Visible="false" runat="server"><asp:Image ID="Image1" runat="server" ImageAlign="TextTop" ImageUrl="~/Images/Public/Silk/error.png" /> Changing the amount or budget invalidates the attestation.<br /></asp:Panel>
    </form>
</body>
</html>
