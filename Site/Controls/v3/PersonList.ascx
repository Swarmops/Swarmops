<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonList.ascx.cs" Inherits="Controls_v3_PersonList" %>
<asp:GridView ID="Grid" runat="server" DataSourceID="PeopleDataSource" 
    AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="Identity" 
    OnRowCommand="Grid_RowCommand" PageSize="20" AllowSorting="True" onrowdatabound="Grid_RowDataBound">
    <Columns>
        <asp:BoundField DataField="PersonId" HeaderText="#" SortExpression="PersonId" />
        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
        <asp:BoundField DataField="PostalCodeAndCity" HeaderText="Postal Code, City" 
            SortExpression="PostalCode" />
        <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
        <asp:BoundField DataField="BirthDate" HeaderText="Birthdate" 
            DataFormatString="{0:d}" HtmlEncode="False" SortExpression="Birthdate" />
        <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
            <EditItemTemplate></EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="LabelPhone" runat="server" Text='<%# Eval("Phone") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:ButtonField CommandName="ViewEdit" HeaderText="View / Edit" Text="View / Edit" >
            <ItemStyle CssClass="ActionLink" />
        </asp:ButtonField>
    </Columns>
    <PagerSettings Mode="NumericFirstLast" PageButtonCount="20" />
</asp:GridView>
<asp:ObjectDataSource ID="PeopleDataSource" runat="server"
    SelectMethod="SelectSortedStatic" 
    TypeName="Activizr.Logic.DataObjects.PeopleDataObject" SortParameterName="sort"
    OnSelecting="PeopleDataSource_Selecting" 
    DataObjectTypeName="Activizr.Logic.Pirates.Person"
    OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
        <asp:Parameter Name="people" Type="Object" />
        <asp:Parameter Name="sort" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>
