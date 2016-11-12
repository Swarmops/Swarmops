<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="ListInvalidEmail.aspx.cs" Inherits="Pages_v4_ListInvalidEmail" Title="List BadEmails - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Import Namespace="Activizr.Logic.Pirates" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">

            function ShowTransactionForm(id, rowIndex) {
                var grid = $find("<%=GridPeople.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                //window.radopen("PopupEditBadEmail.aspx?BadEmailId=" + id, "BadEmailForm");
                return false;
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("Rebind");
                }
                else {
                    $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("RebindAndNavigate");
                }
            }
        </script>

    </telerik:RadCodeBlock>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="BadEmailForm" runat="server"
                Title="Edit/Add BadEmail" Height="400px" Width="500px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="None" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1"
        OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridPeople" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="GridPeople">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridPeople" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonSetUnreachable">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridPeople" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <pw4:PageTitle Icon="cameras.png" Title="List Invalid e-Mail" Description="List and handle badly formatted e-mail addresses"
        runat="server" ID="PageTitle" />
    <asp:Panel ID="UpdatePanel" runat="server">
        <div class="DivMainContent">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">
                    <asp:Label ID="LabelTransactionsTitle" runat="server" Text="Invalid addresses" /></span><br />
                <div class="DivGroupBoxContents">
                    <asp:Panel ID="gridPanel" runat="server">
                        <telerik:RadGrid ID="GridPeople" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                            GridLines="None" OnItemCommand="GridPeople_ItemCommand"
                            Skin="Web20" AllowMultiRowSelection="True" 
                            onitemdatabound="GridPeople_ItemDataBound">
                            <MasterTableView DataKeyNames="Identity" ShowFooter="false">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Identity" HeaderText="Id#" UniqueName="Identity"
                                        HeaderStyle-Width="5%" />
                                    <telerik:GridBoundColumn DataField="Name" HeaderText="Person" UniqueName="Name" />
                                    <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" UniqueName="Phone" />
                                    <telerik:GridBoundColumn DataField="Email" HeaderText="Email" UniqueName="Email" />
                                    <telerik:GridBoundColumn DataField="Member" HeaderText="Member" UniqueName="Member" />
                                    <telerik:GridBoundColumn UniqueName="IsInvalid" DataField="EMailIsInvalid" HeaderStyle-Width="5%"
                                        HeaderText="Is Marked Invalid" />
                                    <telerik:GridBoundColumn UniqueName="IsUnreachable" DataField="MailUnreachable" HeaderStyle-Width="5%"
                                        HeaderText="Is Marked Unreachable" />
                                    <telerik:GridTemplateColumn UniqueName="MarkUnreachable" HeaderText="Mark as unreachable"
                                        HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="cbMarkUnreachable" runat="server" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="SendSMS" HeaderText="Send SMS" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="cbSendSMS" runat="server" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="ActionResult" HeaderText="Result" UniqueName="ActionResult" />
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                        <!--
                <telerik:GridTemplateColumn UniqueName="ManageColumn">
                    <ItemTemplate>
                        <asp:HyperLink ID="ManageLink" runat="server" Text="Edit BadEmail..."></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>-->
                        <asp:Button ID="ButtonSetUnreachable" runat="server" Text="Execute action" OnClick="ButtonSetUnreachable_Click" />
                    </asp:Panel>
                    <asp:Panel ID="continueLoadPanel" runat="server">
                        <asp:Button ID="btnContinue" runat="server" Text="Continue Loading" 
                            onclick="btnContinue_Click" />
                    </asp:Panel>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="ButtonRefresh" runat="server" Text="Refresh from database" OnClick="ButtonButtonRefresh_Click" />
                    <asp:CheckBox ID="CheckBoxNonMembers" runat="server" Text="Non-members" />
                    <asp:CheckBox ID="CheckBoxUnreachable" runat="server" Text="Also unreachable" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
