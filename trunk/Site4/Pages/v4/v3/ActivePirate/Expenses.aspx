<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="Expenses.aspx.cs" Inherits="Pages_ActivePirate_Expenses" Title="PirateWeb - Expenses" %>

<%@ Register Src="~/Controls/v3/ExpenseList.ascx" TagName="ExpenseList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1>Expenses are closed for database maintenance. Expect to be back up by Friday.</h1>
        <asp:Panel ID="PanelHide" runat="server" Visible="false">        
        <h1 class="ContentHeader">
            <asp:Label ID="LabelExpensesHeader" runat="server" Text="Expenses for Active Pirates"></asp:Label></h1>

        <h2 class="ContentHeader">
            Open Expenses</h2>
        <asp:Label ID="LabelOpenExpensesParagraph" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <uc1:ExpenseList ID="ListOpenExpenses" runat="server" />
        <br />
        <hr />
        <!-- New Expense table -->
        <h2 class="ContentHeader">
            Claim a New Expense</h2>
        <asp:Label ID="LabelNewExpenseParagraph" runat="server" Text="Label"></asp:Label><br />
        <br />
        </asp:Panel>
        <asp:Panel ID="Panel1" DefaultButton="ButtonClaimExpense" runat="server" Visible="false">
            <asp:Table ID="Table" runat="server" CellSpacing="0">
                <asp:TableRow ID="RowExpenseBy" runat="server">
                    <asp:TableCell ID="CellExpenseByLabel" runat="server">
                        <asp:Label ID="LabelExpenseBy" runat="server" Text="ExpenseClaim By"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="CellExpenseByText" runat="server" CssClass="EditCell">
                        <asp:TextBox ID="TextExpenseBy" runat="server" Columns="30" ReadOnly="true"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">&nbsp;
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="RowOrganization" runat="server">
                    <asp:TableCell ID="CellNameLabel" runat="server">
                        <asp:Label ID="LabelOrganization" runat="server" Text="Label"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="CellOrganizationDrop" runat="server" CssClass="EditCell">
                        <asp:DropDownList ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                            AutoPostBack="true">
                        </asp:DropDownList>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell1" runat="server">
                        &nbsp;&nbsp;<asp:Label ID="LabelNameMessage" runat="server" Text=""></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="RowGeography" runat="server">
                    <asp:TableCell ID="CellStreetLabel" runat="server">
                        <asp:Label ID="LabelGeography" runat="server" Text="Label"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="CellGeogaphyDrop" runat="server" CssClass="EditCell">
                        <asp:DropDownList ID="DropGeographies" runat="server">
                        </asp:DropDownList>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        &nbsp;&nbsp;<asp:Label ID="LabelGeographyMessage" runat="server" Text=""></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="RowDate" runat="server">
                    <asp:TableCell ID="CellPostalLabel" runat="server">
                        <asp:Label ID="LabelDate" runat="server" Text="Label"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="CellPostalText" runat="server" CssClass="EditCell">
                        <asp:TextBox ID="TextDate" runat="server" Columns="15"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server">
                        &nbsp;&nbsp;<asp:Label ID="LabelDateMessage" runat="server" Text="" CssClass="ErrorMessage"></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="RowDescription" runat="server">
                    <asp:TableCell ID="CellCountryLabel" runat="server">
                        <asp:Label ID="LabelDescription" runat="server" Text="Label"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="CellDescriptionText" runat="server" CssClass="EditCell">
                        <asp:TextBox ID="TextDescription" runat="server" Columns="30"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        &nbsp;&nbsp;<asp:Label ID="LabelDescriptionMessage" runat="server" Text="" CssClass="ErrorMessage"></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="RowAmount" runat="server">
                    <asp:TableCell ID="CellEmailLabel" runat="server">
                        <asp:Label ID="LabelAmount" runat="server" Text="Label"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="CellEmailText" runat="server" CssClass="EditCell">
                        <asp:TextBox ID="TextAmount" runat="server" Columns="15"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server">
                        &nbsp;&nbsp;<asp:Label ID="LabelAmountMessage" runat="server" Text="" CssClass="ErrorMessage"></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell20" runat="server">
            &nbsp;
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell21" runat="server" CssClass="EditCell">
                        <asp:Button ID="ButtonClaimExpense" runat="server" Text="Button" OnClick="ButtonClaimExpense_Click" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell22" runat="server">
            &nbsp;&nbsp;
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </div>
</asp:Content>
