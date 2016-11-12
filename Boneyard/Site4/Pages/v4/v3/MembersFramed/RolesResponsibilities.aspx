<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="RolesResponsibilities.aspx.cs" Inherits="Pages_Members_RolesResponsibilities"
    Title="PirateWeb - Member Details - Roles and Responsibilities" meta:resourcekey="PageResource1" %>

<%@ Import Namespace="Activizr.Logic.Security" %>
<%@ Import Namespace="Activizr.Logic.Structure" %>
<%@ Import Namespace="Activizr.Logic.Pirates" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/v3/PersonDetailsPagesMenu.ascx" TagName="PersonDetailsPagesMenu"
    TagPrefix="uc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <br />
        <asp:Label ID="labelCurrentMember" runat="server"></asp:Label>
        <br />
        <asp:Label ID="label4" runat="server" Text="System Roles" 
            meta:resourcekey="label4Resource1"></asp:Label>
        <br />
        <asp:GridView ID="GridSysRoles" runat="server" AutoGenerateColumns="False" 
            DataSourceID="SystemRolesDatasource" meta:resourcekey="GridSysRolesResource1">
            <Columns>
                <asp:BoundField DataField="Type" HeaderText="Role Type" 
                    meta:resourcekey="BoundFieldResource3" />
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="SystemRolesDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="Select" TypeName="Activizr.Logic.DataObjects.RolesDataObject">
            <SelectParameters>
                <asp:QueryStringParameter Name="personId" QueryStringField="id" Type="Int32" />
                <asp:Parameter DefaultValue="System" Name="roleClass" Type="Object" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <hr />
        <asp:Label ID="labelOrganizationResponsibilitiesSubheader" runat="server" 
            Text="Organisational Roles" 
            meta:resourcekey="labelOrganizationResponsibilitiesSubheaderResource1"></asp:Label>
        <br />
        <asp:GridView ID="GridOrgRoles" runat="server" AutoGenerateColumns="False" DataSourceID="OrgRolesDatasource"
            OnRowCommand="GridOrgRoles_RowCommand" 
            meta:resourcekey="GridOrgRolesResource1">
            <Columns>
                <asp:TemplateField HeaderText="Organization" 
                    meta:resourcekey="TemplateFieldResource4">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" 
                            Text='<%# Organization.FromIdentity((int) Eval("OrganizationId")).Name %>' 
                            meta:resourcekey="Label1Resource2"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Type" HeaderText="Role Type" 
                    meta:resourcekey="BoundFieldResource2" />
                <asp:TemplateField HeaderText="Delete?" ShowHeader="False" 
                    meta:resourcekey="TemplateFieldResource5">
                    <ControlStyle CssClass="ActionLink" />
                    <ItemTemplate>
                        <asp:LinkButton ID="ActionDelete" runat="server" CausesValidation="False" CommandName="DeleteManual"
                            CommandArgument='<%# Eval("Identity") %>' Text="Delete" 
                            Visible='<%# Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name)).GetAuthority().CanEditOrgRolesForOrg((int) Eval("OrganizationId")) %>' 
                            meta:resourcekey="ActionDeleteResource2"></asp:LinkButton>
                        <asp:Literal ID="Literal1" Text="&nbsp;&nbsp;&mdash;" runat="server" 
                            Visible='<%# !Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name)).GetAuthority().CanEditOrgRolesForOrg((int) Eval("OrganizationId")) %>' 
                            meta:resourcekey="Literal1Resource2"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="OrgRolesDatasource" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="Select" TypeName="Activizr.Logic.DataObjects.RolesDataObject">
            <SelectParameters>
                <asp:QueryStringParameter Name="personId" QueryStringField="id" Type="Int32" />
                <asp:Parameter DefaultValue="Organization" Name="roleClass" Type="Object" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <asp:Panel ID="AddOrgRolePanel" runat="server" 
            meta:resourcekey="AddOrgRolePanelResource1">
            <asp:Label ID="label3" runat="server" Text="Add Org Role" 
                meta:resourcekey="label3Resource1"></asp:Label>
            <asp:DropDownList ID="DropOrganizationsOrg" runat="server" AutoPostBack="True" 
                OnSelectedIndexChanged="DropOrganizationsOrg_SelectedIndexChanged" 
                meta:resourcekey="DropOrganizationsOrgResource1">
            </asp:DropDownList>
            <asp:DropDownList ID="DropRolesOrg" runat="server" 
                meta:resourcekey="DropRolesOrgResource1">
            </asp:DropDownList>
            <asp:Button ID="ButtonAddOrgRole" runat="server" Text="Add Organisational Role" 
                OnClick="ButtonAddOrgRole_Click" meta:resourcekey="ButtonAddOrgRoleResource1" />
            <br />
        </asp:Panel>
        <hr />
        <asp:Label ID="LabelLocalResponsibilitiesSubheader" runat="server" 
            Text="Local Roles" 
            meta:resourcekey="LabelLocalResponsibilitiesSubheaderResource1"></asp:Label><br />
        <br />
        <asp:GridView ID="GridLocalRoles" runat="server" AutoGenerateColumns="False" DataKeyNames="RoleId"
            DataSourceID="LocalRolesDataSource" 
            OnRowCommand="gridNodeRoles_RowCommand" 
            meta:resourcekey="GridLocalRolesResource1">
            <Columns>
                <asp:TemplateField HeaderText="Organization" 
                    meta:resourcekey="TemplateFieldResource1">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" 
                            Text='<%# Organization.FromIdentity((int) Eval("OrganizationId")).Name %>' 
                            meta:resourcekey="Label1Resource1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Geography" 
                    meta:resourcekey="TemplateFieldResource2">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" 
                            Text='<%# Geography.FromIdentity((int) Eval("GeographyId")).Name %>' 
                            meta:resourcekey="Label2Resource1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Type" HeaderText="Role Type" 
                    meta:resourcekey="BoundFieldResource1" />
                <asp:TemplateField HeaderText="Delete?" ShowHeader="False" 
                    meta:resourcekey="TemplateFieldResource3">
                    <ControlStyle CssClass="ActionLink" />
                    <ItemTemplate>
                        <asp:LinkButton ID="ActionDelete" runat="server" CausesValidation="False" CommandName="DeleteManual"
                            CommandArgument='<%# Eval("Identity") %>' Text="Delete" 
                            Visible='<%# Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name)).GetAuthority().
                    HasLocalRoleAtOrganizationGeography(Organization.FromIdentity((int) Eval("OrganizationId")),
                                                        Geography.FromIdentity((int) Eval("GeographyId")),Authorization.Flag.Default) %>' 
                            meta:resourcekey="ActionDeleteResource1"></asp:LinkButton>
                        <asp:Literal ID="Literal1" Text="&nbsp;&nbsp;&mdash;" runat="server" 
                            Visible='<%# !(Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name)).GetAuthority().
                     HasLocalRoleAtOrganizationGeography(Organization.FromIdentity((int) Eval("OrganizationId")),
                                                         Geography.FromIdentity((int) Eval("GeographyId")),Authorization.Flag.Default)) %>' 
                            meta:resourcekey="Literal1Resource1"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="LocalRolesDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="Select" TypeName="Activizr.Logic.DataObjects.RolesDataObject">
            <SelectParameters>
                <asp:QueryStringParameter Name="personId" QueryStringField="id" Type="Int32" />
                <asp:Parameter DefaultValue="Local" Name="roleClass" Type="Object" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <asp:Panel ID="AddLocalRolePanel" runat="server" 
            meta:resourcekey="AddLocalRolePanelResource1">
            <asp:Label ID="labelAddRole" runat="server" Text="Add Local Role" 
                meta:resourcekey="labelAddRoleResource1"></asp:Label>
            <asp:DropDownList ID="DropOrganizationsLocal" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="DropOrganizationsNode_SelectedIndexChanged" 
                meta:resourcekey="DropOrganizationsLocalResource1">
            </asp:DropDownList>
            <asp:DropDownList ID="DropGeographies" runat="server" 
                meta:resourcekey="DropGeographiesResource1">
            </asp:DropDownList>
            <asp:DropDownList ID="DropRolesLocal" runat="server" 
                meta:resourcekey="DropRolesLocalResource1">
            </asp:DropDownList>
            <asp:Button ID="ButtonAddRoleLocal" runat="server" OnClick="ButtonAddRole_Click"
                Text="Add Local Role" meta:resourcekey="ButtonAddRoleLocalResource1" /></asp:Panel>
    </div>
</asp:Content>
