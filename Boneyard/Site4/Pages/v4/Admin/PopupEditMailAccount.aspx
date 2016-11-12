<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditMailAccount.aspx.cs"
    Inherits="Pages_v4_Admin_PopupEditMailAccount" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Edit Mail Server Account</title>
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

        function ShowKill() {
            document.getElementById('DivKillValidation').style.display = 'inline';
        }

        function HideKill() {
            document.getElementById('DivKillValidation').style.display = 'none';
        }
        
    </script>

</head>
<body style="background-color: #f8e7ff; padding-left: 10px; padding-top: 10px; padding-right: 10px;
    padding-bottom: 10px; line-height: 30px">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1"
        UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ButtonAvail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TextBoxAccount" />
                    <telerik:AjaxUpdatedControl ControlID="DropDownMailDomain" />
                    <telerik:AjaxUpdatedControl ControlID="ButtonAvail" />
                    <telerik:AjaxUpdatedControl ControlID="LabelAvailability" />
                    <telerik:AjaxUpdatedControl ControlID="LabelValidateError" />
                    <telerik:AjaxUpdatedControl ControlID="LabelSaveError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonValidate">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TextBoxForward" />
                    <telerik:AjaxUpdatedControl ControlID="ButtonValidate" />
                    <telerik:AjaxUpdatedControl ControlID="LabelValidateError" />
                    <telerik:AjaxUpdatedControl ControlID="LabelSaveError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonSaveChanges">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LabelSaveError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div>
        <br />
        <table id="mainTable" runat="server">
            <tr>
                <td>
                    <asp:Literal ID="Literal1" runat="server">Account:</asp:Literal>
                </td>
                <td style="vertical-align: super;">
                    <asp:Label ID="LabelAccount" runat="server" Text="Label"></asp:Label>
                    <span runat="server" visible="false" id="addSpan" style="vertical-align: super;">
                        <asp:TextBox ID="TextBoxAccount" runat="server" Width="189px"></asp:TextBox>
                        <span style="vertical-align: super;">@</span><asp:DropDownList ID="DropDownMailDomain"
                            runat="server">
                            <asp:ListItem>piratpartiet.se</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="ButtonAvail" runat="server" Text="Available?" OnClick="ButtonAvail_Click" />
                        &nbsp;
                        <asp:Label ID="LabelAvailability" runat="server" Text=" "></asp:Label>
                    </span>
                </td>
            </tr>
            <tr id="pwTR" runat="server">
                <td>
                    New password:
                </td>
                <td>
                    <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password" Width="188px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Forward to:
                </td>
                <td>
                    <asp:TextBox ID="TextBoxForward" runat="server" Width="189px"></asp:TextBox>
                    &nbsp;
                    <asp:Button ID="ButtonValidate" runat="server" Text="Validate" OnClick="ButtonValidate_Click" />
                    &nbsp;
                    <asp:Label ID="LabelValidateError" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                <asp:Button ID="ButtonSaveChanges" runat="server" Text="Save Changes" 
                        OnClick="ButtonSaveChanges_Click" Width="100px" />
                </td>
                <td>
                    <asp:Label ID="LabelSaveError" runat="server" Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
        <asp:Button ID="ButtonDelete" runat="server" Text="Delete Account" OnClientClick="return (confirm('Do you really want to delete this account?'));"
                        OnClick="ButtonDelete_Click" Width="100px" />
                </td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>
        &nbsp;
    </div>
    </form>
</body>
</html>
