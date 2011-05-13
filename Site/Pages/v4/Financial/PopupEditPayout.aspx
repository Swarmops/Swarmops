<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditPayout.aspx.cs" Inherits="Pages_v4_Financial_PopupEditPayout" %>
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
    <form id="Form1" runat="server" method="post">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:Label CssClass="Heading2" ID="LabelHeader" runat="server" />
    <p>Would you like to...</p>
    <p><asp:RadioButton ID="RadioManualMap" AutoPostBack="true" GroupName="Actions" Checked="true" 
            runat="server" Text="Manually map this payout to a transaction?" 
            oncheckedchanged="RadioManualMap_CheckedChanged" /></p>
    <p style="padding-left:50px">What transaction? 
        <asp:DropDownList AutoPostBack="true" ID="DropTransactions" runat="server" 
            onselectedindexchanged="DropTransactions_SelectedIndexChanged" /></p>
    <p><asp:RadioButton ID="RadioManualMerge" runat="server" GroupName="Actions" AutoPostBack="true" 
            Text="Merge this open payout with another?" 
            oncheckedchanged="RadioManualMerge_CheckedChanged" /></p>
    <p style="padding-left:50px">What payout? <asp:DropDownList AutoPostBack="true" 
            ID="DropPayouts" runat="server" 
            onselectedindexchanged="DropPayouts_SelectedIndexChanged" /></p>
    <p><asp:RadioButton ID="RadioUndoPayout" runat="server" GroupName="Actions" AutoPostBack="true" 
            Text="Undo the payout altogether?" 
            oncheckedchanged="RadioUndoPayout_CheckedChanged" /></p>
    <hr />
    <p><b>If you press 'Execute', the following will happen:</b></p>
    <p style="line-height:120%"><asp:Label ID="LabelActionDescription" runat="server" Text="Nothing." /></p>
    <p style="text-align:right"><asp:Button ID="ButtonExecute" runat="server" 
            Enabled="false" Text="Execute Nothing" onclick="ButtonExecute_Click" /></p>
    </form>
</body>
</html>
