<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditAccount.aspx.cs" Inherits="Pages_v4_Accounting_PopupEditAccount" %>
<%@ Import Namespace="Telerik.Web.UI"%>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Editing Account...</title>
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />    
    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().Close();
            // GetRadWindow().BrowserWindow.refreshGrid(args);
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
    <tr><td>Transaction Tag:&nbsp;</td><td><asp:TextBox ID="TextTransactionTag" runat="server" /></td></tr>
    <tr><td>&nbsp;</td><td><asp:Button ID="ButtonSetTag" Text="Set Tag" runat="server" 
                    onclick="ButtonSetTag_Click" /></td></tr>
    <tr><td>Account Geography:&nbsp;</td><td><pw4:GeographyTreeDropDown ID="DropGeographies" runat="server" /></td></tr>
    <tr><td>&nbsp;</td><td><asp:Button ID="ButtonSetGeography" Text="Set Geography" runat="server" 
                    onclick="ButtonSetGeography_Click" /></td></tr>
    </table>
    </form>
</body>
</html>
