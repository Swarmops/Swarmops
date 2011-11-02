<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditBudgetOwner.aspx.cs" Inherits="Pages_v4_Accounting_PopupEditBudgetOwner" %>
<%@ Import Namespace="Telerik.Web.UI"%>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/ComboPerson.ascx" TagName="ComboPerson" TagPrefix="pw4" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Editing Budget...</title>
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
    </script>
</head>

<body style="background-color: #f8e7ff; padding-left:10px; padding-top:10px; padding-right: 10px; padding-bottom: 10px">
    <form id="form1" runat="server" method="post">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <table border="0">
    <tr><td>Organization:&nbsp;</td><td><asp:Label ID="LabelOrganization" runat="server" /></td></tr>
    <tr><td>Account:&nbsp;</td><td><b><asp:Label ID="LabelAccount" runat="server" /></b></td></tr>

    <tr><td>Account Owner:&nbsp;</td><td><pw4:ComboPerson ID="ComboOwner" runat="server" /></td></tr>
    <tr><td>&nbsp;</td><td><asp:Button ID="ButtonSetOwner" Text="Set Owner" runat="server" 
                    onclick="ButtonSetOwner_Click" /></td></tr>
    </table>
    <asp:Panel ID="PanelSubAccounts" runat="server" Visible="true">
    <p>There are <b><asp:Label ID="LabelSubAccountCount" runat="server" /></b> to this account with a combined budget of <b><asp:Label ID="LabelSubAccountBudget" runat="server" /></b>. They are not affected by operations here.</p>
    </asp:Panel>
    <p>Perhaps you would like to create a sub-account to this account and transfer part of the budget there. Useful for organizing. Do note that you are editing the organization's account plan, however: don't overdo it.</p>
    </form>
</body>
</html>
