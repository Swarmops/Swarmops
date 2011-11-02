<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupAddAccount.aspx.cs"
    Inherits="Pages_v4_PopupAddAccount" %>

<%@ Import Namespace="Telerik.Web.UI" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Page Title Is Set In Page_Load()</title>
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().Close();
            GetRadWindow().BrowserWindow.refreshGrid(args);
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz as well)

            return oWindow;
        }

        function CancelEdit() {
            GetRadWindow().Close();
        }
    </script>

    <style type="text/css">
        .formTableCaption
        {
            width: 101px;
        }
    </style>

    </head>
<body style="background-color: #f8e7ff; padding-left: 10px; padding-top: 10px; padding-right: 10px;
    padding-bottom: 10px">
    <form id="form1" runat="server" method="post">
    <asp:Label ID="Label1" runat="server" Text="Add Account" CssClass="Heading2"></asp:Label><br />
    <asp:Label ID="Label2" runat="server" Text=" for organisation:"></asp:Label>
    <asp:Label ID="LabelOrgName" runat="server" Text="" style="font-weight:bolder"></asp:Label>
    
    <br />
    <br />
    <table >
        <tr>
            <td class="formTableCaption">
                Account Name</td>
            <td colspan="3">
                <asp:TextBox ID="TextAccountName" runat="server" Width="269px" Wrap="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="formTableCaption">
                Account type:</td>
            <td colspan="3">
                <asp:DropDownList ID="DropAccountType" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="formTableCaption">
                Sub Account of:</td>
            <td colspan="3">
                 <asp:DropDownList ID="DropAccounts" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="formTableCaption">
                &nbsp;</td>
            <td >
        </td>
            <td >&nbsp;</td>
            <td align="right" >
                <asp:Button ID="ButtonAdd" runat="server" Text="Add" OnClick="ButtonAdd_Click" 
                    Width="58px" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="ButtonClose" runat="server" Text="Cancel" OnClick="ButtonClose_Click" /></td>
        </tr>
    </table>
    <br />
    <br />
    </form>
</body>
</html>
