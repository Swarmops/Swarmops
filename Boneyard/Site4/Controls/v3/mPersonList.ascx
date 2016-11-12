<%@ Control Language="C#" AutoEventWireup="true" CodeFile="mPersonList.ascx.cs" Inherits="m_Controls_PersonList" %>
<asp:GridView ID="Grid" runat="server" DataSourceID="PeopleDataSource" AllowPaging="True"
    AutoGenerateColumns="False" DataKeyNames="Identity" OnRowCommand="Grid_RowCommand"
    PageSize="20" AllowSorting="True" EnableTheming="False" EnableViewState="False"
    OnRowDataBound="Grid_RowDataBound">
    <Columns>
        <asp:BoundField DataField="PersonId" HeaderText="#" SortExpression="PersonId" />
        <asp:TemplateField HeaderText="Name" SortExpression="Name">
            <ItemTemplate>
                <a id="contLink" runat="server">NNN</a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="CityName" HeaderText="City" SortExpression="CityName" />
        <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
            <ItemTemplate>
                <a id="phoneLink" runat="server">000</a>&nbsp; <a id="smsLink"
                    runat="server">SMS</a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="BirthDate" HeaderText="Born" DataFormatString="{0:yy}"
            HtmlEncode="False" SortExpression="Birthdate">
            <ItemStyle HorizontalAlign="Right" />
        </asp:BoundField>
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
