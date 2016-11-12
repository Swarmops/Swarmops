<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="EditMailTemplate.aspx.cs" Inherits="Pages_v4_EditMailTemplate" ValidateRequest="false"
    EnableEventValidation="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/WSOrgTreeDropDown.ascx" TagName="WSOrgTreeDropDown"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/WSOrgTree.ascx" TagName="WSOrgTree" TagPrefix="uc4" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .topbutton
        {
            margin-left: 5px;
            margin-right: 25px;
        }
    </style>

    <script type="text/javascript">
        function OnClientCommandExecuting(editor, args) {
            var name = args.get_name();
            var val = args.get_value();


            if (name == "PlaceHolders") {
                editor.pasteHtml(val);
                args.set_cancel(true);
            }
        }

        document.onkeyup = function(e) {
            var evnt = e;
            if (document.all) {
                var evnt = window.event;
            }
            if (evnt.keyCode == "S".charCodeAt(0) && evnt.ctrlKey == 1) {
                var btn = document.getElementById('<%=ButtonSave.ClientID %>');
                if (btn != null)
                    btn.click();
            }
        }



    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">

    <script type="text/javascript">
        function copyDDvalue(elem, targetID) {
            var target = document.getElementById(targetID);
            target.value = elem.options[elem.selectedIndex].text;
        }

        function doConfirm(elem, txt) {
            //Hack to handle OnClientClick behavior under RAD
            if (confirm(txt)) {
                __doPostBack(elem.name, '')
            }
            return false;
        }
    </script>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="WSOrgTreeDropDown1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TopPanel" />
                    <telerik:AjaxUpdatedControl ControlID="EditPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonOpen">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TopPanel" />
                    <telerik:AjaxUpdatedControl ControlID="EditPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TopPanel" />
                    <telerik:AjaxUpdatedControl ControlID="EditPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonSaveAs">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TopPanel" />
                    <telerik:AjaxUpdatedControl ControlID="EditPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonCancelEdit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TopPanel" />
                    <telerik:AjaxUpdatedControl ControlID="EditPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonCancelSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TopPanel" />
                    <telerik:AjaxUpdatedControl ControlID="EditPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Edit Mail Template" Description="Edit basic mail templates"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <div class="bodyContent">
            <asp:Panel ID="MainPanel" runat="server">
                <div style="border-right: 1px silver solid; border-bottom: 1px silver solid; border-left: 1px silver solid;
                    border-top: 1px silver solid; padding-bottom: 0px; margin: 5px; padding: 5px;">
                    <asp:Panel ID="TopPanel" runat="server">
                        <asp:Panel ID="TopPanelSpecify" runat="server">
                            <table>
                                <tr>
                                    <td>Template name:</td><td>
                                    <asp:DropDownList ID="DropDownName" runat="server">
                                    </asp:DropDownList>
                                    </td><td>&nbsp;&nbsp;</td><td>Country:</td><td>
                                    <asp:DropDownList ID="DropDownCountry" runat="server">
                                    </asp:DropDownList>
                                    </td><td>&nbsp;&nbsp;</td><td>Organisation (optional)</td><td>
                                    <asp:Panel ID="Panel1" runat="server">
                                        <uc1:WSOrgTreeDropDown ID="WSOrgTreeDropDown1" runat="server" OnSelectedNodeChanged="WSOrgTreeDropDown1_SelectedNodeChanged" />
                                    </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="TopPanelSelect" runat="server">
                            <table>
                                <tr>
                                    <td>Template:</td><td>
                                    <asp:DropDownList ID="DropDownListTemplates" runat="server">
                                    </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="TopPanelView" runat="server">
                            <table>
                                <tr>
                                    <td>
                                    <asp:Label ID="Label1" runat="server">Template name:</asp:Label>
                                    <asp:Label ID="LabelTemplateName" runat="server" Text="Template Name" Font-Bold="true"></asp:Label>
                                    </td><td>&nbsp;&nbsp;
                                    <asp:Label ID="Label2" runat="server">Country:</asp:Label>
                                    <asp:Label ID="LabelCountryName" runat="server" Text="Country Name" Font-Bold="true"></asp:Label>
                                    </td><td>&nbsp;&nbsp;
                                    <asp:Label ID="Label3" runat="server">Organisation (optional):</asp:Label>
                                    <asp:Label ID="LabelOrgName" runat="server" Text="Org Name" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="TopPanelButtons" runat="server">
                            <asp:Button ID="ButtonOpen" runat="server" Text="Open" OnClick="ButtonOpen_Click"
                                CssClass="topbutton" />
                            <asp:Button ID="ButtonSave" runat="server" Text="Save" OnClick="ButtonSave_Click"
                                CssClass="topbutton" AccessKey="s" />
                            <asp:Button ID="ButtonSaveAs" runat="server" Text="Save as" OnClick="ButtonSaveAs_Click"
                                CssClass="topbutton" />
                            <asp:Button ID="ButtonCancelEdit" runat="server" Text="Cancel" OnClick="ButtonCancelEdit_Click"
                                CssClass="topbutton" />
                            <asp:Button ID="ButtonCancelSaveAs" runat="server" Text="Cancel" OnClick="ButtonCancelSaveAs_Click"
                                CssClass="topbutton" />
                        </asp:Panel>
                    </asp:Panel>
                </div>
                <asp:Panel ID="EditPanel" runat="server">
                    <table width="100%">
                        <tr>
                            <td width="5%">
                            <asp:Label ID="LabelSubject" runat="server">Subject</asp:Label>:</td><td>
                            <asp:TextBox ID="TextBoxSubject" runat="server" Style="width: 100%;"></asp:TextBox>
                            </td><td width="5px">&nbsp; </td>
                        </tr>
                    </table>
                    <hr />
                    <table width="100%">
                        <tr>
                            <td>
                            <telerik:RadEditor ID="RadEditor1" runat="server" StripFormattingOptions="All" Width="100%"
                                OnClientCommandExecuting="OnClientCommandExecuting" ToolsFile="~/Pages/v4/v3/Communications/ToolsFile.xml"
                                ContentAreaCssFile="~/Pages/v4/admin/EditorContent.css">
                                <Content>
                                </Content>
                            </telerik:RadEditor>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
    <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" Skin="Web20" />
</asp:Content>
