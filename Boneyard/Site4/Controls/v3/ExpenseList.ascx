<%@ Import Namespace="Activizr.Logic.Pirates"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ExpenseList.ascx.cs" Inherits="Controls_ExpenseList" %>
<asp:GridView ID="GridExpenses" runat="server" AllowPaging="True" AutoGenerateColumns="False"
    DataSourceID="ExpenseDataSource" DataKeyNames="Identity" 
    OnRowCommand="GridExpenses_RowCommand" meta:resourcekey="GridExpensesResource1">
    <Columns>
        <asp:BoundField DataField="Identity" HeaderText="#&amp;nbsp;&amp;nbsp;" 
            HtmlEncode="False" meta:resourcekey="BoundFieldResource1" />
        <asp:BoundField DataField="ExpenseDate" DataFormatString="{0:d}" HeaderText="Date"
            HtmlEncode="False" meta:resourcekey="BoundFieldResource2" />
        <asp:TemplateField HeaderText="Person Claiming" 
            meta:resourcekey="TemplateFieldResource1">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" 
                    Text='<%# ((Person) Eval("Claimer")).Name %>' 
                    meta:resourcekey="Label1Resource1"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Description" HeaderText="Description" 
            meta:resourcekey="BoundFieldResource3" />
        <asp:BoundField DataField="Amount" DataFormatString="{0:#,##0.00}" HeaderText="Amount"
            HtmlEncode="False" meta:resourcekey="BoundFieldResource4">
            <ItemStyle HorizontalAlign="Right" />
        </asp:BoundField>
        <asp:BoundField DataField="LastEventType" HeaderText="Status" 
            meta:resourcekey="BoundFieldResource5" />
        <asp:ButtonField CommandName="Approve" HeaderText="Approve?" Text="Approve" 
            meta:resourcekey="ButtonFieldResource1">
            <ItemStyle CssClass="ActionLink" />
        </asp:ButtonField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="ExpenseDataSource" runat="server" DataObjectTypeName="Activizr.Logic.Financial.ExpenseClaim"
    OnSelecting="ExpenseDataSource_Selecting" SelectMethod="SelectOpenByClaimer"
    TypeName="Activizr.Logic.DataObjects.ExpensesDataObject">
</asp:ObjectDataSource>
