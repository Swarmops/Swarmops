<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonList.ascx.cs" Inherits="Controls_PersonList" %>
<asp:GridView ID="Grid" runat="server" DataSourceID="PeopleDataSource" AllowPaging="True"
    AutoGenerateColumns="False" DataKeyNames="Identity" OnRowCommand="Grid_RowCommand"
    PageSize="20" AllowSorting="True" OnRowDataBound="Grid_RowDataBound">
    <Columns>
        <asp:BoundField DataField="PersonId" HeaderText="#" SortExpression="PersonId" />
        <asp:TemplateField HeaderText="&nbsp;">
            <ItemTemplate>
                <asp:Image ID="IconPerson" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
        <asp:BoundField DataField="Street" HeaderText="Street" SortExpression="Street" Visible="False" />
        <asp:BoundField DataField="PostalCodeAndCity" HeaderText="Postal Code, City" SortExpression="PostalCode" />
        <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
        <asp:BoundField DataField="BirthDate" HeaderText="Birthdate" DataFormatString="{0:d}"
            HtmlEncode="False" SortExpression="Birthdate" />
        <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
            <ItemTemplate>
                <asp:HyperLink ID="HyperLinkPhone" runat="server"></asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status" Visible="False">
            <ItemTemplate>
                <asp:Label ID="LabelStatus" runat="server" Text=""></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Expires" Visible="False">
            <ItemTemplate>
                <asp:Label ID="LabelExpiry" runat="server" Text=""></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="MemberSince" Visible="False">
            <ItemTemplate>
                <asp:Label ID="LabelMemberSince" runat="server" Text=""></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="View / Edit" ShowHeader="False">
            <ItemTemplate>
                <asp:LinkButton ID="ViewEditButton" runat="server" CausesValidation="false" CommandName="ViewEdit"
                    Text="View / Edit"></asp:LinkButton>
            </ItemTemplate>
            <ItemStyle CssClass="ActionLink" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle CssClass="ui-widget-header" ForeColor="White" Height="23px" VerticalAlign="Middle" />
    <PagerSettings Mode="NumericFirstLast" PageButtonCount="20" />
</asp:GridView>
<asp:ObjectDataSource ID="PeopleDataSource" runat="server" SelectMethod="SelectSortedStatic"
    TypeName="Activizr.Logic.DataObjects.PeopleDataObject" SortParameterName="sort"
    OnSelecting="PeopleDataSource_Selecting" DataObjectTypeName="Activizr.Logic.Pirates.Person"
    OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
        <asp:Parameter Name="people" Type="Object" />
        <asp:Parameter Name="sort" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
