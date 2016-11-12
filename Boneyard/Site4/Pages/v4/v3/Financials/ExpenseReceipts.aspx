<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="ExpenseReceipts.aspx.cs" Inherits="Pages_Financials_ExpenseReceipts"
    Title="PirateWeb - Expense Receipts" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v3/ExpenseList.ascx" TagName="ExpenseList" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="coins.png" Title="ExpenseClaim Receipts" Description="" runat="server"
            meta:resourcekey="PageTitle" ID="PageTitle"></pw4:PageTitle>
        <asp:Label ID="LabelAccessDenied" runat="server" Text="Sorry, you do not have access to this functionality. This is where the treasurer of an organization approves receipts for expenses."
            meta:resourcekey="LabelAccessDeniedResource1"></asp:Label>
    <h1>Expenses are closed for database maintenance. Expect to be back up by Friday.</h1>
    <asp:Panel ID="PanelHide" runat="server" Visible="false">
        <uc1:ExpenseList ID="ExpenseList" runat="server" />
    </asp:Panel>
    </div>
</asp:Content>
