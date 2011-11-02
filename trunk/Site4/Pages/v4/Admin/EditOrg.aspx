<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    Debug="true" CodeFile="EditOrg.aspx.cs" Inherits="Pages_v4_admin_EditOrg" meta:resourcekey="PageResource1" %>

<%@ Import Namespace="Activizr.Basic.Types" %>
<%@ Import Namespace="Activizr.Logic" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/WSOrgTreeDropDown.ascx" TagName="WSOrgTreeDropDown"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/WSGeographyTreeDropDown.ascx" TagName="WSGeographyTreeDropDown"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/v4/WSGeographyTree.ascx" TagName="WSGeographyTree" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/v4/WSOrgTree.ascx" TagName="WSOrgTree" TagPrefix="uc4" %>
<%@ Register Src="~/Controls/v4/OrganizationRoles.ascx" TagName="OrganizationRoles"
    TagPrefix="uc5" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="uc6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <telerik:RadScriptBlock runat="server">
        <style type="text/css">
            .rgDetailTable
            {
                margin-left: -2px !important;
            }
            .rgDataDiv
            {
                height: 100% !important;
            }
            .formTableCaption
            {
                width: 160px;
            }
            .formTableValue
            {
                width: 450px;
            }
            .formTableTextBox
            {
                width: 300px;
            }
            .style1
            {
                width: 549px;
            }
            .nonAccessNode
            {
                color: Silver;
            }
            .roTextBox INPUT
            {
                border: dotted 1px silver;
            }
            html, body
            {
                height: 100%;
                width: 100%;
                margin-bottom: 0px !important;
                margin-right: 0px !important;
            }
        </style>
        <style type="text/css" runat="server" id="programmedStyleTag">
            #RadMultiPage1.ClientID
            {
                border: 1px solid black;
                z-index: 10;
                position: relative;
                top: -1px;
            }
            #RadTabStrip1.ClientID
            {
                border-bottom: none;
                z-index: 12;
                position: relative;
                display: inline;
            }
        </style>
    </telerik:RadScriptBlock>
    <telerik:RadScriptBlock runat="server">

        <script src="/jQuery/js/jquery-1.4.2.min.js" type="text/javascript"></script>

        <script type="text/javascript">
            function OnRequestStart(sender, eventArgs) { }
            function OnResponseEnd(sender, eventArgs) { setTimeout(doResize, 100); setTimeout(doResize, 500); }

            function verifyDelete() {
                if (confirm("Are you really sure you want to delete this organisation?"))
                    return confirm("\n\n     Really?      \n\n");
                return false;
            }

            function redrawTopDropdown() {
                $find("<%=RadAjaxManager1.ClientID %>").ajaxRequestWithTarget("<%=UpdateTopDrop.UniqueID %>", "");
            }

            var resizeTO = null;

            $(window).ready(function() {
                $(window).resize(function() { clearTimeout(resizeTO); setTimeout(doResize, 200) });
                doResize();
                setTimeout(doResize, 500);
            });

            function doResize() {
                var h = $(window).height();
                var w = $(window).width();
                var jQPanel = $("#<%= BottomPanel.ClientID %>")
                var mainPanelHeight = h - jQPanel.offset().top - 25;
                var mainPanelWidth = w - 25;

                jQPanel.css("height", mainPanelHeight).css("width", mainPanelWidth);

                $(document.body).css("overflow", "hidden");

                var jQPage1 = $("#ctl00_BodyContent_RadMultiPage1");
                if (jQPage1.length > 0) {
                    jQPage1.css("height", h - jQPage1.offset().top - 5).css("width", w - 25).css("overflow", "auto");

                }

                var jQUptakePanel = $("#ctl00_BodyContent_FormView1_PanelUptake");
                if (jQUptakePanel.length > 0) {
                    jQUptakePanel.css("height", h - jQUptakePanel.offset().top - 25).css("flush", "left");
                    // jQUptakePanel.css("max-width", w - jQUptakePanel.offset().left - 45);
                }

                jQUptakePanel = $("#ctl00_BodyContent_FormView1_PanelEditUptake");
                if (jQUptakePanel.length > 0) {
                    jQUptakePanel.css("height", h - jQUptakePanel.offset().top - 90).css("flush", "left");
                    jQUptakePanel.css("width", w - jQUptakePanel.offset().left - 45);
                }

                jQPage1 = $("#ctl00_BodyContent_RadMultiPage2");
                if (jQPage1.length > 0) {
                    jQPage1.css("height", h - jQPage1.offset().top - 5).css("width", w - 25).css("overflow", "auto");
                }

                jQPage1 = $("#ctl00_BodyContent_RadMultiPage3");
                if (jQPage1.length > 0) {
                    jQPage1.css("height", h - jQPage1.offset().top - 5).css("width", w - 25).css("overflow", "auto");
                }

            }
        </script>

    </telerik:RadScriptBlock>
    <style type="text/css">
        .style2
        {
            height: 27px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1"
            meta:resourcekey="RadAjaxManager1Resource2" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="WSOrgTreeDropDownSelectForEdit">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="BottomPanel" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="BottomPanel">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="BottomPanel" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="UpdateTopDrop">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="OnRequestStart" OnResponseEnd="OnResponseEnd" />
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
            meta:resourcekey="RadAjaxLoadingPanel1Resource2">
        </telerik:RadAjaxLoadingPanel>
        <uc6:PageTitle Icon="chart_organisation_add.png" Title="Edit Organisation" Description="Edit organisation structure"
            runat="server" ID="ucPageTitle"></uc6:PageTitle>
        <asp:Panel ID="TopPanel" runat="server" meta:resourcekey="TopPanelResource1">
            <table>
                <tr>
                    <td valign="top" nowrap="nowrap">
                        Select organisation to edit:
                    </td>
                    <td style="vertical-align: top" valign="top">
                        <asp:Panel ID="Panel1" runat="server">
                            <uc1:WSOrgTreeDropDown ID="WSOrgTreeDropDownSelectForEdit" runat="server" OnSelectedNodeChanged="DropSelectForEdit_SelectedNodeChanged"
                                Skin="Web20" />
                        </asp:Panel>
                    </td>
                    <td valign="top">
                        <asp:Literal ID="Directions" runat="server" meta:resourcekey="DirectionsResource1"
                            Text="If you want to add an organisation, select its parent org, and then in next screen &quot;New&quot;"></asp:Literal>
                        <asp:LinkButton ID="UpdateTopDrop" runat="server" OnClick="UpdateTopDrop_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <asp:Panel ID="BottomPanel" runat="server" meta:resourcekey="Panel1Resource2">
            <asp:HiddenField ID="HiddenSelectedOrgID" runat="server" OnValueChanged="HiddenSelectedOrgID_ValueChanged" />
            <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
                SelectedIndex="0" Skin="Web20" meta:resourcekey="RadTabStrip1Resource1" AutoPostBack="True"
                OnTabClick="RadTabStrip1_TabClick">
                <Tabs>
                    <telerik:RadTab runat="server" Text="Organization and Geography" meta:resourcekey="RadTabResource1"
                        Selected="True">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Organization Roles" meta:resourcekey="RadTabResource2">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="SubOrganization Roles" meta:resourcekey="RadTabResource3">
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" SkinID="Web20"
                Width="100%" Height="100%" meta:resourcekey="RadMultiPage1Resource1" RenderSelectedPageOnly="True">
                <telerik:RadPageView ID="RadPageView1" runat="server" meta:resourcekey="RadPageView1Resource1"
                    Selected="True">
                    <table style="position: relative; width: 100%; height: 100%;">
                        <tr>
                            <td>
                                <asp:Label ID="LabelModeEditing" runat="server" Text="Editing record:" Visible="False"
                                    meta:resourcekey="LabelModeEditingResource1"></asp:Label>
                                <asp:Label ID="LabelModeInserting" runat="server" Text="Inserting New Record:" Visible="False"
                                    meta:resourcekey="LabelModeInsertingResource1"></asp:Label>
                                <asp:Label ID="LabelModeCopying" runat="server" Text="Inserting New Record(from copy):"
                                    Visible="False" meta:resourcekey="LabelModeCopyingResource1"></asp:Label>
                                <asp:FormView ID="FormView1" runat="server" DefaultMode="ReadOnly" DataSourceID="OrganizationsDataSource"
                                    Width="100%" DataKeyNames="Identity" OnItemUpdated="FormView1_ItemUpdated" OnDataBound="FormView1_DataBound"
                                    OnItemCommand="FormView1_ItemCommand" OnItemInserting="FormView1_ItemInserting"
                                    OnItemInserted="FormView1_ItemInserted" OnDataBinding="FormView1_DataBinding"
                                    OnItemUpdating="FormView1_ItemUpdating" meta:resourcekey="FormView1Resource1"
                                    OnItemCreated="FormView1_ItemCreated">
                                    <EditItemTemplate>
                                        <table style="position: relative; width: 100%">
                                            <tr>
                                                <td style="width: auto;">
                                                    <table style="position: relative; width: 100%;">
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource1" Text="Identity"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:Label ID="IdentityTextBox" runat="server" Text='<%# Bind("Identity") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource2" Text="Name"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource3" Text="Name Short"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameShortTextBox" runat="server" Text='<%# Bind("NameShort") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource4" Text="Name International"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameInternationalTextBox" runat="server"
                                                                    Text='<%# Bind("NameInternational") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource5" Text="Organizational Parent"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <uc1:WSOrgTreeDropDown ID="WSOrgTreeDropDownParentOrg" runat="server" Skin="Web20"
                                                                    SelectedOrganizationId='<%# Bind("ParentOrganizationId") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal1" runat="server" meta:resourcekey="LiteralResource6" Text="Anchor Geography"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <uc2:WSGeographyTreeDropDown ID="AnchorGeographyDropdown" runat="server" Skin="Web20"
                                                                    SelectedGeographyId='<%#Bind("AnchorGeographyId")%>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="LiteralDefaultCountry" runat="server" Text="Default Country"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:DropDownList ID="DropDefaultCountry" runat="server" SelectedValue='<%# Bind("CountryCode") %>'>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal4" runat="server" meta:resourcekey="LiteralResource7" Text="Domain"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox ID="DomainTextBox" Text='<%# Bind("Domain") %>' runat="server" CssClass="formTableTextBox"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal5" runat="server" Text="Show Names In Notifications"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <%--this is an optional field, do not use Bind, the datasource can not handle it.--%>
                                                                <asp:DropDownList ID="DropDownShowNamesInNotifications" runat="server" OnDataBinding='ShowNames_DataBinding'>
                                                                    <asp:ListItem Value="" Text="Inherit"></asp:ListItem>
                                                                    <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                                                    <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal6" runat="server" Text="Use Membership Payment Status"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <%--this is an optional field, do not use Bind, the datasource can not handle it.--%>
                                                                <asp:DropDownList ID="DropDownUsePaymentStatus" runat="server" OnDataBinding='UsePaymentStatus_DataBinding'>
                                                                    <asp:ListItem Value="" Text="Inherit"></asp:ListItem>
                                                                    <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                                                    <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource8" Text="Mail Prefix"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="MailPrefixTextBox" runat="server" Text='<%# Bind("MailPrefix") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource9" Text="Accepts Members"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:CheckBox ID="AcceptsMembersCheckBox" runat="server" Checked='<%# Bind("AcceptsMembers") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource10" Text="Auto Assign NewMembers"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:CheckBox ID="AutoAssignNewMembersCheckBox" runat="server" Checked='<%# Bind("AutoAssignNewMembers") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal2" runat="server" Text="Functional Mail"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:GridView ID="GridViewMailEdit" runat="server" AutoGenerateColumns="False" Width="100%">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Type">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LabelType" runat="server" Text='<%# Eval("maType") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Name">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox ID="TextBoxName" runat="server" Text='<%# Eval("name") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="eMail">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox ID="TextBoxEmail" runat="server" Text='<%# Eval("email") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                &nbsp;
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:Button ID="UpdateButton" runat="server" CommandName="Update" Text="Update" meta:resourcekey="UpdateButtonResource1" />&nbsp;&nbsp;
                                                                <asp:Button ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                                    Text="Cancel" meta:resourcekey="UpdateCancelButtonResource1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top" style="width: auto;">
                                                    <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Uptake geographies"
                                                        meta:resourcekey="Label3Resource1"></asp:Label>
                                                    <br />
                                                    <asp:Panel ID="PanelEditUptake" runat="server" Height="300px" ScrollBars="Auto" BorderStyle="Solid"
                                                        BorderWidth="1px" BorderColor="Black" meta:resourcekey="PanelEditUptakeResource1">
                                                        <uc3:WSGeographyTree ID="UptakeGeoTree" runat="server" />
                                                    </asp:Panel>
                                                    <br />
                                                    <asp:Literal ID="UptakeExpl" runat="server" meta:resourcekey="UptakeExplResource1"
                                                        Text="Green nodes = Currently belonging to this org&lt;br 
                                                /&gt; Gray nodes = Currently belonging to another organisation, point at them to see wich.&lt;br 
                                                /&gt;Gray with green borders = currently belonging to both this and another org.&lt;br 
                                                 /&gt;Change by checking/unchecking nodes."></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </EditItemTemplate>
                                    <InsertItemTemplate>
                                        <table style="position: relative; width: 100%">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource12" Text="Name"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource13" Text="Name Short"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameShortTextBox" runat="server" Text='<%# Bind("NameShort") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource14" Text="Name International"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameInternationalTextBox" runat="server"
                                                                    Text='<%# Bind("NameInternational") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource15" Text="Organizational Parent"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="ParentIdentityTextBox" runat="server"
                                                                    ReadOnly="True" meta:resourcekey="ParentIdentityTextBoxResource1" /><asp:HiddenField
                                                                        ID="ParentIdentityHidden" Value='<%# Bind("ParentOrganizationId") %>' runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource16" Text="Anchor Geography"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <uc2:WSGeographyTreeDropDown ID="AnchorGeographyDropdown" runat="server" Skin="Web20"
                                                                    SelectedGeographyId='<%# Bind("AnchorGeographyId") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="LiteralDefaultCountry" runat="server" Text="Default Country"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:DropDownList ID="DropDefaultCountry" runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal3" runat="server" meta:resourcekey="LiteralResource17" Text="Domain"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="DomainTextBox" runat="server" Text='<%# Bind("Domain") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource18" Text="Mail Prefix"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="MailPrefixTextBox" runat="server" Text='<%# Bind("MailPrefix") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource19" Text="Accepts Members"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:CheckBox ID="AcceptsMembersCheckBox" runat="server" Checked='<%# Bind("AcceptsMembers") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource20" Text="Auto Assign NewMembers"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:CheckBox ID="AutoAssignNewMembersCheckBox" runat="server" Checked='<%# Bind("AutoAssignNewMembers") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                &nbsp;
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" meta:resourcekey="InsertButtonResource1" />&nbsp;&nbsp;
                                                                <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                                    Text="Cancel" meta:resourcekey="InsertCancelButtonResource1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                </td>
                                                <td valign="top">
                                                    <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Uptake geographies"
                                                        meta:resourcekey="Label3Resource2"></asp:Label>
                                                    <br />
                                                    <asp:Panel ID="PanelEditUptake" runat="server" Height="300px" ScrollBars="Auto" BorderStyle="Solid"
                                                        BorderWidth="1px" BorderColor="Black" meta:resourcekey="PanelEditUptakeResource2">
                                                        <uc3:WSGeographyTree ID="UptakeGeoTree" runat="server" />
                                                    </asp:Panel>
                                                    <br />
                                                    <asp:Literal ID="UptakeExpl" runat="server" meta:resourcekey="UptakeExplResource2"
                                                        Text="Gray nodes = Currently belonging to another organisation, point at them to see wich.&lt;br 
                                                 /&gt;Change by checking/unchecking nodes."></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </InsertItemTemplate>
                                    <ItemTemplate>
                                        <table style="position: relative; width: 95%">
                                            <tr>
                                                <td valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource21" Text="Identity"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:Label ID="IdentityTextBox" runat="server" Text='<%# Bind("Identity") %>' ReadOnly="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource26" Text="Name"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource22" Text="Name Short"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameShortTextBox" runat="server" Text='<%# Bind("NameShort") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource23" Text="Name International"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="NameInternationalTextBox" runat="server"
                                                                    Text='<%# Bind("NameInternational") %>' ReadOnly="True" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource24" Text="Organizational Parent"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="ParentIdentityTextBox" runat="server"
                                                                    Text='<%# Eval("ParentName") %>' ReadOnly="True" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource25" Text="Anchor GeographyId"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="PrimaryGeographyTextBox" runat="server"
                                                                    Text='<%# Eval("AnchorName") %>' ReadOnly="True" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="LiteralDefaultCountry" runat="server" Text="Default Country"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="DefaultCountryTextBox" runat="server"
                                                                    Text='<%# Eval("CountryCode") %>' ReadOnly="True" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource27" Text="Domain"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="DomainTextBox" runat="server" Text='<%# Eval("Domain") %>'
                                                                    ReadOnly="True" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal5" runat="server" Text="Show Names In Notifications"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <%--this is an optional field, do not use Bind, the datasource can not handle it.--%>
                                                                <asp:DropDownList ID="DropDownShowNamesInNotifications" runat="server" OnDataBinding='ShowNames_DataBinding'
                                                                    Enabled="false">
                                                                    <asp:ListItem Value="" Text="Inherit"></asp:ListItem>
                                                                    <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                                                    <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource28" Text="Mail Prefix"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue roTextBox">
                                                                <asp:TextBox CssClass="formTableTextBox" ID="MailPrefixTextBox" runat="server" Text='<%# Eval("MailPrefix") %>'
                                                                    ReadOnly="True" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource29" Text="Accepts Members"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue ">
                                                                <asp:CheckBox ID="AcceptsMembersCheckBox" runat="server" Checked='<%# Eval("AcceptsMembers") %>'
                                                                    Enabled="False" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal runat="server" meta:resourcekey="LiteralResource30" Text="Auto Assign NewMembers"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue ">
                                                                <asp:CheckBox ID="AutoAssignNewMembersCheckBox" runat="server" Checked='<%# Bind("AutoAssignNewMembers") %>'
                                                                    Enabled="false" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                <asp:Literal ID="Literal2" runat="server" Text="Functional Mail"></asp:Literal>
                                                                :
                                                            </td>
                                                            <td class="formTableValue">
                                                                <asp:GridView ID="GridViewMailDisplay" runat="server" Width="100%" AutoGenerateColumns="False"
                                                                    OnRowDataBound="OnRowDataBound_GridViewMailDisplay">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Type">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LabelType" runat="server" Text='<%# Eval("maType") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LabelName" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="eMail">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LabelEmail" runat="server" Text='<%# Eval("email") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formTableCaption">
                                                                &nbsp;
                                                            </td>
                                                            <td class="formTableValue" nowrap="nowrap">
                                                                <br />
                                                                <asp:Button ID="ButtonEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                                                    Text="Edit" meta:resourcekey="Button3Resource1" />&nbsp;&nbsp;
                                                                <asp:Button ID="ButtonNew" runat="server" CausesValidation="False" CommandName="New"
                                                                    Text="New" meta:resourcekey="Button4Resource1" />&nbsp;&nbsp;
                                                                <asp:Button ID="ButtonCopy" runat="server" CausesValidation="False" CommandName="Copy"
                                                                    Text="Copy to new" meta:resourcekey="Button1Resource1" />&nbsp;&nbsp;
                                                                <asp:Button ID="ButtonDelete" runat="server" CausesValidation="False" CommandName="DeleteOrg"
                                                                    OnClientClick="verifyDelete()" Text="Delete" meta:resourcekey="ButtonDeleteResource1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <asp:Panel ID="PanelUptake" runat="server" meta:resourcekey="PanelUptakeResource1"
                                                        Height="400px" ScrollBars="Auto">
                                                        <div>
                                                            <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Uptake geographies"
                                                                meta:resourcekey="Label3Resource3"></asp:Label>
                                                        </div>
                                                        <asp:GridView ID="GridViewDisplayUptakes" runat="server" AutoGenerateColumns="False"
                                                            DataKeyNames="OrgId,GeoId" DataSourceID="UptakesDataSource" meta:resourcekey="GridView1Resource1"
                                                            OnRowDataBound="GridViewDisplayUptakes_RowDataBound" OnDataBinding="GridViewDisplayUptakes_DataBinding"
                                                            OnRowDeleting="GridViewDisplayUptakes_RowDeleting">
                                                            <Columns>
                                                                <asp:BoundField DataField="OrgId" HeaderText="OrgId" SortExpression="OrgId" Visible="False"
                                                                    meta:resourcekey="BoundFieldResource1" />
                                                                <asp:BoundField DataField="GeoId" HeaderText="GeoId" SortExpression="GeoId" Visible="False"
                                                                    meta:resourcekey="BoundFieldResource2" />
                                                                <asp:BoundField DataField="GeoName" HeaderText="Geo Name" SortExpression="GeoName"
                                                                    meta:resourcekey="BoundFieldResource3">
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Other Stakeholders" meta:resourcekey="TemplateFieldResource2">
                                                                    <ItemTemplate>
                                                                        <asp:GridView ID="GridViewOtherUptakes" runat="server" AutoGenerateColumns="False"
                                                                            DataKeyNames="OrgId,GeoId" DataSourceID="OtherUptakesDatasource" ShowHeader="False"
                                                                            OnRowDataBound="GridViewOtherUptakes_RowDataBound" OnRowDeleting="GridViewOtherUptakes_RowDeleting"
                                                                            Width="100%">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="OrgId" HeaderText="OrgId" SortExpression="OrgId" Visible="False"
                                                                                    meta:resourcekey="BoundFieldResource4" />
                                                                                <asp:BoundField DataField="GeoId" HeaderText="GeoId" SortExpression="GeoId" Visible="False"
                                                                                    meta:resourcekey="BoundFieldResource5" />
                                                                                <asp:BoundField DataField="GeoName" HeaderText="GeoName" SortExpression="GeoName"
                                                                                    Visible="False" meta:resourcekey="BoundFieldResource6" />
                                                                                <asp:BoundField DataField="OrgName" HeaderText="OrgName" SortExpression="OrgName"
                                                                                    meta:resourcekey="BoundFieldResource7" />
                                                                                <asp:TemplateField HeaderText="Delete" meta:resourcekey="TemplateFieldResource1">
                                                                                    <ItemTemplate>
                                                                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" Text="Delete"
                                                                                            meta:resourcekey="lnkDeleteResource1"></asp:LinkButton>
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="50px" />
                                                                                    <ItemStyle Width="50px" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("GeoId") %>' />
                                                                        <asp:ObjectDataSource ID="OtherUptakesDatasource" runat="server" DataObjectTypeName="Activizr.Logic.DataObjects.OrganizationsDataObject+UptakeGeography"
                                                                            DeleteMethod="DeleteUptake" SelectMethod="SelectOrgOthersUptakeForGeo" TypeName="Activizr.Logic.DataObjects.OrganizationsDataObject">
                                                                            <SelectParameters>
                                                                                <asp:ControlParameter ControlID="HiddenSelectedOrgID" Name="orgid" PropertyName="Value"
                                                                                    Type="Int32" />
                                                                                <asp:ControlParameter ControlID="HiddenField1" Name="geoid" PropertyName="Value"
                                                                                    Type="Int32" />
                                                                            </SelectParameters>
                                                                        </asp:ObjectDataSource>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                </asp:FormView>
                            </td>
                        </tr>
                    </table>
                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageView2" runat="server" Height="100%" meta:resourcekey="RadPageView2Resource1"
                    Visible="False">
                    <br />
                    <uc5:OrganizationRoles ID="OrganizationRoles1" runat="server" Scope="Mine" />
                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageView3" runat="server" Height="100%" meta:resourcekey="RadPageView3Resource1"
                    Visible="False">
                    <br />
                    <uc5:OrganizationRoles ID="OrganizationRoles2" runat="server" Scope="SubOrgs" />
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </asp:Panel>
        <br />
    </div>
    <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" Skin="Web20" meta:resourcekey="RadFormDecorator1Resource1" />
    <asp:ObjectDataSource ID="UptakesDataSource" runat="server" DataObjectTypeName="Activizr.Logic.DataObjects.OrganizationsDataObject+UptakeGeography"
        DeleteMethod="DeleteUptake" InsertMethod="AddUptake" SelectMethod="SelectOrgMineUptake"
        TypeName="Activizr.Logic.DataObjects.OrganizationsDataObject" OnDeleted="UptakesDataSource_Deleted"
        OnInserted="UptakesDataSource_Inserted">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormView1" Name="orgid" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="DuplicatingDataSource" runat="server" SelectMethod="Select"
        InsertMethod="AddOrganization" UpdateMethod="AddDuplicateOrganization" TypeName="Activizr.Logic.DataObjects.OrganizationsDataObject"
        DataObjectTypeName="Activizr.Logic.DataObjects.OrganizationsDataObject+Org"
        OnUpdated="DuplicatingDataSource_Updated">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenSelectedOrgID" Name="orgid" PropertyName="Value"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="OrganizationsDataSource" runat="server" SelectMethod="Select"
        UpdateMethod="UpdateOrganisation" InsertMethod="AddOrganization" TypeName="Activizr.Logic.DataObjects.OrganizationsDataObject"
        DataObjectTypeName="Activizr.Logic.DataObjects.OrganizationsDataObject+Org"
        OnInserted="OrganizationsDataSource_Inserted" OnDeleted="OrganizationsDataSource_Deleted"
        OnUpdated="OrganizationsDataSource_Updated" OnDataBinding="OrganizationsDataSource_DataBinding"
        OnUpdating="OrganizationsDataSource_Updating">
        <SelectParameters>
            <asp:ControlParameter ControlID="HiddenSelectedOrgID" Name="orgid" PropertyName="Value"
                Type="Int32" />
        </SelectParameters>
        <InsertParameters>
            <asp:Parameter Name="org" Type="Object" />
        </InsertParameters>
    </asp:ObjectDataSource>
</asp:Content>
