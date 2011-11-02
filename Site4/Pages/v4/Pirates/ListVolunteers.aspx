<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeFile="ListVolunteers.aspx.cs" Inherits="Pages_v4_ListVolunteers"
    Title="List Volunteers - PirateWeb" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/SelectOrganizationLine.ascx" TagName="SelectOrganizationLine"
    TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20" Behavior="Default"
        InitialBehavior="None" Left="" meta:resourcekey="RadWindowManager1Resource1"
        Style="display: none;" Top="">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="False" Skin="Web20" ID="ManageVolunteerDialog"
                runat="server" Title="Manage Volunteer" Height="400px" Width="600px" Left="150px"
                ReloadOnShow="True" Modal="True" Behavior="Default" InitialBehavior="None" meta:resourcekey="ManageVolunteerDialogResource1"
                NavigateUrl="" Style="display: none;" Top="" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        meta:resourcekey="RadAjaxManager1Resource1" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                    <telerik:AjaxUpdatedControl ControlID="GridLeadGeography" />
                    <telerik:AjaxUpdatedControl ControlID="GridReports" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="GridOwner">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonAssign">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                    <telerik:AjaxUpdatedControl ControlID="GridLeadGeography" />
                    <telerik:AjaxUpdatedControl ControlID="GridReports" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonRevertViceAndAdmin">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                    <telerik:AjaxUpdatedControl ControlID="GridLeadGeography" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonRevertDirects">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                    <telerik:AjaxUpdatedControl ControlID="GridReports" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonRevertOthers">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                    <telerik:AjaxUpdatedControl ControlID="GridOther" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function ShowManageForm(id, rowIndex) {
                var grid = $find("<%=GridOwner.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupManageVolunteer.aspx?VolunteerId=" + id, "ManageVolunteerDialog");
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
    <pw4:PageTitle Icon="user_star.png" Title="List Volunteers" Description="List and handle people who have volunteered for duty"
        meta:resourcekey="PageTitle" runat="server" ID="PageTitle" />
    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">
                <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Volunteers you are responsible for"></asp:Literal></span><br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridOwner" runat="server" AllowMultiRowSelection="True" AutoGenerateColumns="False"
                    GridLines="None" Skin="Web20" OnItemCommand="GridOwner_ItemCommand" OnItemCreated="Grid_ItemCreated"
                    meta:resourcekey="GridOwnerResource1">
                    <%--<HeaderContextMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </HeaderContextMenu>--%>
                    <ValidationSettings ValidationGroup="1" />
                    <MasterTableView DataKeyNames="Identity">
                        <Columns>
                            <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" meta:resourcekey="GridClientSelectColumnResource1" />
                            <telerik:GridBoundColumn HeaderText="Volunteer" DataField="Name" meta:resourcekey="GridBoundColumnResource1"
                                UniqueName="Name">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="Geography" HeaderText="Geography" meta:resourcekey="GridTemplateColumnResource1">
                                <ItemTemplate>
                                    <asp:Label ID="LabelGeographyParent" runat="server" meta:resourcekey="LabelGeographyParentResource1" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" UniqueName="column3"
                                meta:resourcekey="GridBoundColumnResource2">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="OpenedDateTime" HeaderText="Volunteered"
                                meta:resourcekey="GridTemplateColumnResource2">
                                <ItemTemplate>
                                    <asp:Label ID="LabelOpenedDateTime" runat="server" meta:resourcekey="LabelOpenedDateTimeResource1" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="ManageColumn" meta:resourcekey="GridTemplateColumnResource3">
                                <ItemTemplate>
                                    <asp:HyperLink ID="ManageLink" runat="server" Text="Process volunteer..." meta:resourcekey="ManageLinkResource1"></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn UniqueName="EditCommandColumn1">
                            </EditColumn>
                        </EditFormSettings>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <%--<FilterMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </FilterMenu>--%>
                </telerik:RadGrid>
                <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" Text="Assign selected volunteers to"></asp:Literal>
                <asp:DropDownList ID="DropNewOwner" runat="server" meta:resourcekey="DropNewOwnerResource1"
                    Style="width: auto;" ValidationGroup="1">
                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="Select..."></asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="ButtonAssign" Text="Ok" runat="server" OnClick="ButtonAssign_Click"
                    meta:resourcekey="ButtonAssignResource1" ValidationGroup="1" />
                <asp:CompareValidator ValueToCompare="1" Operator="GreaterThanEqual" ControlToValidate="DropNewOwner"
                    runat="server" ID="ValidatorDropNewOwner" Text="Select a new owner." meta:resourcekey="ValidatorDropNewOwnerResource1"
                    ValidationGroup="1" />
                <asp:Button ID="ButtonAssignToDistrictLeads" runat="server" Text="District Lead"
                    Visible="False" OnClick="ButtonAssignToDistrictLeads_Click" meta:resourcekey="ButtonAssignToDistrictLeadsResource1"
                    ValidationGroup="1" />
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="ButtonAutoAssign" Text="Autoassign" runat="server" OnClick="ButtonAutoAssign_Click"
                    Visible="False" meta:resourcekey="ButtonAutoAssignResource1" ValidationGroup="1" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="ButtonMarkAsDone" Text="Mark as processed" runat="server" Visible="False"
                    ValidationGroup="1" onclick="ButtonMarkAsDone_Click" />
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">
                <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" Text="Volunteers your vices and admins are responsible for"></asp:Literal>
            </span>
            <br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridLeadGeography" runat="server" AllowMultiRowSelection="True"
                    AutoGenerateColumns="False" GridLines="None" Skin="Web20" OnItemCreated="Grid_ItemCreated"
                    meta:resourcekey="GridLeadGeographyResource1">
                    <%--<HeaderContextMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </HeaderContextMenu>--%>
                    <MasterTableView DataKeyNames="Identity">
                        <Columns>
                            <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" meta:resourcekey="GridClientSelectColumnResource1" />
                            <telerik:GridBoundColumn HeaderText="Volunteer" DataField="Name" meta:resourcekey="GridBoundColumnResource3"
                                UniqueName="Name">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="Geography" HeaderText="Geography" meta:resourcekey="GridTemplateColumnResource4">
                                <ItemTemplate>
                                    <asp:Label ID="LabelGeographyParent" runat="server" meta:resourcekey="LabelGeographyParentResource2" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" UniqueName="column3"
                                meta:resourcekey="GridBoundColumnResource4">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="OpenedDateTime" HeaderText="Volunteered"
                                meta:resourcekey="GridTemplateColumnResource5">
                                <ItemTemplate>
                                    <asp:Label ID="LabelOpenedDateTime" runat="server" meta:resourcekey="LabelOpenedDateTimeResource2" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="OwnerName" HeaderText="Responsible" UniqueName="column"
                                meta:resourcekey="GridBoundColumnResource5">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn UniqueName="EditCommandColumn1">
                            </EditColumn>
                        </EditFormSettings>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <%--<FilterMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </FilterMenu>--%>
                </telerik:RadGrid>
                <asp:Button ID="ButtonRevertViceAndAdmin" runat="server" Text="Revert marked to me"
                    OnClick="ButtonRevertViceAndAdmin_Click" meta:resourcekey="ButtonRevertViceAndAdminResource1" />
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">
                <asp:Literal ID="Literal5" runat="server" meta:resourcekey="Literal5Resource1" Text="Volunteers your direct reports are responsible for"></asp:Literal>
            </span>
            <br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridReports" runat="server" AllowMultiRowSelection="True" AutoGenerateColumns="False"
                    GridLines="None" Skin="Web20" OnItemCreated="Grid_ItemCreated" meta:resourcekey="GridReportsResource1">
                    <MasterTableView DataKeyNames="Identity">
                        <Columns>
                            <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" meta:resourcekey="GridClientSelectColumnResource1" />
                            <telerik:GridBoundColumn HeaderText="Volunteer" DataField="Name" meta:resourcekey="GridBoundColumnResource6"
                                UniqueName="Name">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="Geography" HeaderText="Geography" meta:resourcekey="GridTemplateColumnResource6">
                                <ItemTemplate>
                                    <asp:Label ID="LabelGeographyParent" runat="server" meta:resourcekey="LabelGeographyParentResource3" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" UniqueName="column3"
                                meta:resourcekey="GridBoundColumnResource7">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="OpenedDateTime" HeaderText="Volunteered"
                                meta:resourcekey="GridTemplateColumnResource7">
                                <ItemTemplate>
                                    <asp:Label ID="LabelOpenedDateTime" runat="server" meta:resourcekey="LabelOpenedDateTimeResource3" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="OwnerName" HeaderText="Responsible" UniqueName="column"
                                meta:resourcekey="GridBoundColumnResource8">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn UniqueName="EditCommandColumn1">
                            </EditColumn>
                        </EditFormSettings>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                </telerik:RadGrid>
                <asp:Button ID="ButtonRevertDirects" runat="server" Text="Revert marked to me" OnClick="ButtonRevertDirects_Click"
                    meta:resourcekey="ButtonRevertDirectsResource1" />
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">
                <asp:Literal ID="Literal4" runat="server" meta:resourcekey="Literal4Resource1" Text="Other volunteers in your area of authority"></asp:Literal>
            </span>
            <br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridOther" runat="server" AllowMultiRowSelection="True" AutoGenerateColumns="False"
                    GridLines="None" Skin="Web20" OnItemCreated="Grid_ItemCreated" meta:resourcekey="GridOtherResource1">
                    <%--<HeaderContextMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </HeaderContextMenu>--%>
                    <MasterTableView DataKeyNames="Identity">
                        <Columns>
                            <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" meta:resourcekey="GridClientSelectColumnResource1" />
                            <telerik:GridBoundColumn HeaderText="Volunteer" DataField="Name" meta:resourcekey="GridBoundColumnResource9"
                                UniqueName="Name">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="Geography" HeaderText="Geography" meta:resourcekey="GridTemplateColumnResource8">
                                <ItemTemplate>
                                    <asp:Label ID="LabelGeographyParent" runat="server" meta:resourcekey="LabelGeographyParentResource4" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" UniqueName="column3"
                                meta:resourcekey="GridBoundColumnResource10">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="OpenedDateTime" HeaderText="Volunteered"
                                meta:resourcekey="GridTemplateColumnResource9">
                                <ItemTemplate>
                                    <asp:Label ID="LabelOpenedDateTime" runat="server" meta:resourcekey="LabelOpenedDateTimeResource4" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="OwnerName" HeaderText="Responsible" UniqueName="column"
                                meta:resourcekey="GridBoundColumnResource11">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn UniqueName="EditCommandColumn1">
                            </EditColumn>
                        </EditFormSettings>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <%--  <FilterMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </FilterMenu> --%>
                </telerik:RadGrid>
                <asp:Button ID="ButtonRevertOthers" runat="server" Text="Revert marked to me" OnClick="ButtonRevertOthers_Click"
                    meta:resourcekey="ButtonRevertOthersResource1" />
            </div>
        </div>
    </div>
</asp:Content>
