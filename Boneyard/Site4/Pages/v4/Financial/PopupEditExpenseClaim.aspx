<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditExpenseClaim.aspx.cs" Inherits="Pages_v4_Financial_PopupEditExpenseClaim" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Expense Claim</title>
    
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
    <p>Are you sure you would like to <b>kill</b> this expense claim?</p><p>This will remove it from the list of outstanding claims, undo its accounting, and notify the claimer that it has been rejected. (Good reasons for this include duplicate or bogus claims.)</p>
    <br /><br />
    <div style="float:right"><asp:LinkButton ID="ButtonConfirmedKill" runat="server" 
            Text="Yes, kill it." onclick="ButtonConfirmedKill_Click" /></div>
    <asp:HyperLink ID="LinkAbortKill" runat="server" NavigateUrl="javascript:HideKill();" Text="Oops. No." />
    </div>
    <div>
    <div style="width:100px;float:left">Claim by:<br />Claim date:<br />Expense date:<br />Description:<br />Amount:<br />Budget:<br />Attested:<br />Documents:<br />Validated:<br />&nbsp;</div>
    <asp:Label ID="LabelClaimer" runat="server" />&nbsp;<br />
    <asp:Label ID="LabelClaimDate" runat="server" />&nbsp;<br />
    <telerik:RadDatePicker ID="DateExpense" runat="server" 
                onselecteddatechanged="DateExpense_SelectedDateChanged" />&nbsp;<asp:CustomValidator 
                ID="Validator_DateExpense_Custom" Display="Dynamic" runat="server" OnServerValidate="Validator_DateExpense_Custom_ServerValidate" ErrorMessage="A valid date is required." /><br />
    <asp:TextBox ID="TextDescription" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                ID="Validator_TextDescription_Required" runat="server" ControlToValidate="TextDescription"
                ErrorMessage="Required." Display="Dynamic" />
            <br />
    <asp:TextBox ID="TextAmount" runat="server" ontextchanged="TextAmount_TextChanged" />&nbsp;<asp:CustomValidator 
                ID="Validator_TextAmount_Custom" Display="Dynamic" ControlToValidate="TextAmount" OnServerValidate="Validator_TextAmount_Custom_ServerValidate" runat="server" ErrorMessage="A valid amount is required." />
            <br />
    <pw4:FinancialAccountTreeDropDown ID="DropAccounts" OnSelectedNodeChanged="DropAccounts_SelectedNodeChanged" runat="server" />
            <asp:CustomValidator Display="Dynamic" ID="Validator_DropAccounts_Custom" OnServerValidate="Validator_DropAccounts_Custom_ServerValidate" runat="server" ErrorMessage="Required."></asp:CustomValidator>
            &nbsp;<br />
    <asp:Label ID="LabelAttested" runat="server" />&nbsp;<asp:LinkButton ID="LinkAttest" Visible="false" runat="server" Text="Attest expense?" />&nbsp;<br />
    <pw4:DocumentList ID="DocumentList" UseShortForm="true" runat="server" />&nbsp;<br />
    <asp:Label ID="LabelValidated" runat="server" />&nbsp;<asp:LinkButton ID="LinkValidate" Visible="false" runat="server" Text="Confirm validity of documents?" />&nbsp;<asp:HyperLink ID="LinkKillEnable" runat="server" Text="Kill expense claim?" NavigateUrl="javascript:ShowKill();" /> <br />
    <asp:Button ID="ButtonSaveChanges" runat="server" Text="Save Changes" 
                onclick="ButtonSaveChanges_Click" />&nbsp;
    </div>
    
    <br /><asp:Image ID="ImageWarning" runat="server" ImageAlign="TextTop" ImageUrl="~/Images/Public/Silk/error.png" /> There is no access control on this page yet, but there is tracking.<br />
    <asp:Panel ID="PanelWarningReattestation" Visible="false" runat="server"><asp:Image ID="Image1" runat="server" ImageAlign="TextTop" ImageUrl="~/Images/Public/Silk/error.png" /> Changing the amount or budget invalidates the attestation.<br /></asp:Panel>
    <asp:Panel ID="PanelWarningRevalidation" Visible="false" runat="server"><asp:Image ID="Image2" runat="server" ImageAlign="TextTop" ImageUrl="~/Images/Public/Silk/error.png" /> Changing the amount or date invalidates the validation.<br /></asp:Panel>
    </form>
</body>
</html>
