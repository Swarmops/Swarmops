<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="FindPeople.aspx.cs" Inherits="Pages_v4_FindPeople" Title="Find/List People - PirateWeb" %>

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
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function ShowManageForm(id, rowIndex) {
                var grid = $find("<%=GridPeople.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditPerson.aspx?PersonId=" + id, "EditPersonDialog");
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
    <pw4:PageTitle Icon="group.png" Title="Find/List People" Description="Search for or list members, activists, newsletter subscribers, etc"
        runat="server" ID="PageTitle" />
    <asp:UpdatePanel ID="Update" runat="server">
        <ContentTemplate>
            <div class="DivMainContent">
                <div class="DivGroupBox">
                    <span class="DivGroupBoxTitle">What are you looking for?</span><br />
                    <div class="DivGroupBoxContents">
                        <asp:UpdatePanel ID="UpdatePanelParameters" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="float: left">
                                    Person ID#:<br />
                                    Name:<br />
                                    Email:<br />
                                    Phone:<br />
                                    Birthyear(s):&nbsp;&nbsp;<br />
                                    Geography:</div>
                                <div style="float: left">
                                    <asp:TextBox ID="TextPersonId" runat="server" />&nbsp;<br />
                                    <asp:TextBox ID="TextName" runat="server" />
                                    (partial ok)&nbsp;<br />
                                    <asp:TextBox ID="TextEmail" runat="server" />
                                    (partial ok)&nbsp;<br />
                                    <asp:TextBox ID="TextPhone" runat="server" />&nbsp;<br />
                                    <asp:TextBox ID="TextBirthyears" runat="server" />&nbsp;<br />
                                    <asp:UpdatePanel runat="server" ID="geoUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <pw4:GeographyTreeDropDown ID="DropGeographies" runat="server" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    &nbsp;<br />
                                    <asp:Button ID="ButtonSearch" runat="server" Text="Search" />
                                </div>
                                <div style="float: right; border-left: solid 1px #808080; padding-left: 10px; padding-bottom: 5px">
                                    <b>Find what kind of pirates?</b><br />
                                    <asp:RadioButton GroupName="SearchFor" ID="RadioMembers" Checked="true" runat="server"
                                        Text="Members in " />&nbsp;<pw4:OrganizationTreeDropDown ID="DropOrganizations" runat="server" />
                                    <br />
                                    ...include
                                    <asp:CheckBox ID="CheckIncludeExpired" Enabled="false" Text="expired members" runat="server" /><br />
                                    <asp:RadioButton GroupName="SearchFor" ID="RadioActivists" Enabled="false" runat="server"
                                        Text="Activists (never tied to an organization)" /><br />
                                    <asp:RadioButton GroupName="SearchFor" ID="RadioSubscribers" Enabled="false" runat="server"
                                        Text="Subscribers to" />&nbsp;<asp:DropDownList ID="DropNewsletters" Enabled="false"
                                            runat="server">
                                            <asp:ListItem Text="-- Select newsletter --" />
                                        </asp:DropDownList>
                                    <br />
                                    <asp:RadioButton GroupName="SearchFor" ID="RadioPeople" Enabled="false" runat="server"
                                        Text="Search across all people in database" /><br />
                                </div>
                                <div style="clear: both">
                                </div>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="DivGroupBox">
                    <span class="DivGroupBoxTitle">Results</span><br />
                    <div class="DivGroupBoxContents">
                        <telerik:RadGrid ID="GridPeople" runat="server" AllowMultiRowSelection="True" AutoGenerateColumns="False"
                            GridLines="None" Skin="Web20" OnItemCreated="GridPeople_ItemCreated">
                            <HeaderContextMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </HeaderContextMenu>
                            <MasterTableView ShowFooter="false" DataKeyNames="Identity">
                                <Columns>
                                    <telerik:GridBoundColumn HeaderText="#" DataField="Identity" HeaderStyle-HorizontalAlign="Right"
                                        ItemStyle-HorizontalAlign="Right" />
                                    <telerik:GridBoundColumn HeaderText="Person" DataField="Name" />
                                    <telerik:GridTemplateColumn UniqueName="Geography" HeaderText="Geography">
                                        <ItemTemplate>
                                            <asp:Label ID="LabelGeography" runat="server" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="Email" HeaderText="Mail" UniqueName="Mail" />
                                    <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" UniqueName="Phone" />
                                    <telerik:GridBoundColumn DataField="Birthdate" HeaderText="Birthdate" UniqueName="Birthdate"
                                        DataFormatString="{0:yyyy-MM-dd}" />
                                    <telerik:GridTemplateColumn UniqueName="EditColumn">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="EditLink" runat="server" Text="View / Edit..."></asp:HyperLink>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <EditFormSettings>
                                    <EditColumn UniqueName="EditCommandColumn1">
                                    </EditColumn>
                                </EditFormSettings>
                            </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                            <FilterMenu>
                                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                            </FilterMenu>
                        </telerik:RadGrid>
                    </div>
                </div>
            </div>
            <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20">
                <Windows>
                    <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="ManageVolunteerDialog"
                        runat="server" Title="Manage Volunteer" Height="400px" Width="600px" Left="150px"
                        ReloadOnShow="true" Modal="true" />
                </Windows>
            </telerik:RadWindowManager>
            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="GridOwner">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="GridOwner" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
