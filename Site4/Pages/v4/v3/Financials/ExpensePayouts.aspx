<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" 
    CodeFile="ExpensePayouts.aspx.cs" Inherits="Pages_Financials_ExpensePayouts"
    Title="PirateWeb - Expense Payouts" meta:resourcekey="PageResource1" %>

<%@ Import Namespace="Activizr.Logic.Pirates" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <h1>Expenses are closed for database maintenance. Expect to be back up by Friday.</h1>
    <asp:Panel ID="PanelHide" runat="server" Visible="false">
    <div class="bodyContent">
        <pw4:PageTitle Icon="coins.png" Title="ExpenseClaim Payouts" Description="" runat="server"
            ID="PageTitle" meta:resourcekey="PageTitle" ></pw4:PageTitle>
        <asp:Label ID="LabelAccessDenied" runat="server" Text="Sorry, you do not have access to this functionality. This is where the treasurer of an organization approves receipts for expenses."
            meta:resourcekey="LabelAccessDeniedResource1"></asp:Label>
        <asp:GridView ID="GridPayouts" runat="server" AutoGenerateColumns="False" DataSourceID="ExpensePayoutsDataSource"
            OnRowCommand="GridPayouts_RowCommand" DataKeyNames="Identity" meta:resourcekey="GridPayoutsResource1">
            <Columns>
                <asp:TemplateField HeaderText="Claiming Person" meta:resourcekey="TemplateFieldResource1">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# ((Person) Eval("Person")).Name %>'
                            meta:resourcekey="Label1Resource1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Bank Account" meta:resourcekey="TemplateFieldResource2">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# ((Person) Eval("Person")).BankName + " " + ((Person) Eval("Person")).BankAccount %>'
                            meta:resourcekey="Label2Resource1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Amount" DataFormatString="{0:#,##0.00}" HeaderText="Amount"
                    HtmlEncode="False" meta:resourcekey="BoundFieldResource1">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Receipt Numbers" meta:resourcekey="TemplateFieldResource3">
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# ((string) Eval("ReceiptList")).Replace ("-", "–") %>'
                            meta:resourcekey="Label3Resource1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:ButtonField CommandName="Repaid" HeaderText="Repaid?" Text="Repaid" meta:resourcekey="ButtonFieldResource1">
                    <ItemStyle CssClass="ActionLink" />
                </asp:ButtonField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1" 
                    Text="Nothing to show."></asp:Label>
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:ObjectDataSource ID="ExpensePayoutsDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="SelectByOrganization" TypeName="Activizr.Interface.DataObjects.ExpensePayoutsDataObject">
            <SelectParameters>
                <asp:Parameter DefaultValue="1" Name="organizationId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    </asp:Panel>
</asp:Content>
