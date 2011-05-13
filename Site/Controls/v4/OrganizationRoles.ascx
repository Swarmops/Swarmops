<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrganizationRoles.ascx.cs" Inherits="Controls_v4_OrganizationRoles" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:HiddenField ID="HiddenField1" runat="server" />
<telerik:RadGrid ID="RadGrid1" runat="server" 
    DataSourceID="OrganizationRoleDataSource" 
    onitemdatabound="RadGrid1_ItemDataBound" AutoGenerateColumns="False" 
    Skin="Web20" Height="100%" meta:resourcekey="RadGrid1Resource1" 
    GroupingEnabled="False" >
<MasterTableView DataKeyNames="Identity" 
        DataSourceID="OrganizationRoleDataSource" SkinID="Web20" 
        TableLayout="Fixed" >
    <Columns>
        <telerik:GridBoundColumn DataField="OrganisationName" 
            HeaderText="Organisation" SortExpression="OrganisationName" 
            UniqueName="OrganisationName" meta:resourcekey="GridBoundColumnResource1" >
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="GeographyName" HeaderText="Geography" 
            SortExpression="GeographyName" UniqueName="GeographyName" 
            meta:resourcekey="GridBoundColumnResource2">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="RoleName" HeaderText="Role" 
            SortExpression="RoleName" UniqueName="RoleName" 
            meta:resourcekey="GridBoundColumnResource3">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="PersonName" HeaderText="Person" 
            SortExpression="PersonName" UniqueName="PersonName" 
            meta:resourcekey="GridBoundColumnResource4">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="RoleId" DataType="System.Int32" 
            HeaderText="RoleId" ReadOnly="True" SortExpression="RoleId" UniqueName="RoleId" 
            Visible="False" meta:resourcekey="GridBoundColumnResource5">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="PersonId" DataType="System.Int32" 
            HeaderText="PersonId" ReadOnly="True" SortExpression="PersonId" 
            UniqueName="PersonId" Visible="False" 
            meta:resourcekey="GridBoundColumnResource6">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="Type" DataType="System.Int32" 
            HeaderText="Type" ReadOnly="True" SortExpression="Type" UniqueName="Type" 
            Visible="False" meta:resourcekey="GridBoundColumnResource7">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="OrganizationId" DataType="System.Int32" 
            HeaderText="OrganizationId" ReadOnly="True" SortExpression="OrganizationId" 
            UniqueName="OrganizationId" Visible="False" 
            meta:resourcekey="GridBoundColumnResource8">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="GeographyId" DataType="System.Int32" 
            HeaderText="GeographyId" ReadOnly="True" SortExpression="GeographyId" 
            UniqueName="GeographyId" Visible="False" 
            meta:resourcekey="GridBoundColumnResource9">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="Identity" DataType="System.Int32" 
            HeaderText="Identity" ReadOnly="True" SortExpression="Identity" 
            UniqueName="Identity" Visible="False" 
            meta:resourcekey="GridBoundColumnResource10">
        </telerik:GridBoundColumn>
    </Columns>
</MasterTableView>
    <ClientSettings>
        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
        <Resizing AllowColumnResize="True" ResizeGridOnColumnResize="True" />
    </ClientSettings>
</telerik:RadGrid>
<asp:ObjectDataSource ID="OrganizationRoleDataSource" runat="server" 
    OldValuesParameterFormatString="original_{0}" 
    SelectMethod="SelectAllByOrganization" 
    TypeName="Activizr.Interface.DataObjects.OrganizationRolesDataObject">
    <SelectParameters>
        <asp:ControlParameter ControlID="HiddenField1" Name="organizationId" 
            PropertyName="Value" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
