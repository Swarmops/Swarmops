<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ledger.ascx.cs" Inherits="Controls_v4_Financial_Ledger" EnableViewState="true" %>

<telerik:RadGrid ID="GridTransactions" runat="server" AllowMultiRowSelection="False" AllowPaging="True" 
    AutoGenerateColumns="False" GridLines="None" Skin="Web20" OnItemCreated="GridTransactions_ItemCreated" PageSize="50" onneeddatasource="GridTransactions_NeedDataSource">
    <HeaderContextMenu>
        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
    </HeaderContextMenu>
    <MasterTableView ShowFooter="false">
        <Columns>
            <telerik:GridBoundColumn HeaderText="Tx#" DataField="FinancialTransactionId" />
            <telerik:GridBoundColumn HeaderText="Date" DataField="TransactionDateTime" DataFormatString="{0:yyyy-MM-dd}" />
            <telerik:GridBoundColumn HeaderText="Date Time" DataField="TransactionDateTime" DataFormatString="{0:yyyy-MM-dd HH:mmm}" />
            <telerik:GridBoundColumn HeaderText="Description" DataField="Description" UniqueName="Description" />
            <telerik:GridTemplateColumn HeaderText="Account" UniqueName="ColumnAccount">
                <ItemTemplate>
                    <asp:Label ID="LabelAccountName" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Credit" HeaderStyle-HorizontalAlign="Right"
                UniqueName="column3" ItemStyle-HorizontalAlign="Right">
                <ItemTemplate>
                    <asp:Label ID="LabelCredit" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Debit" HeaderStyle-HorizontalAlign="Right"
                UniqueName="column3" ItemStyle-HorizontalAlign="Right">
                <ItemTemplate>
                    <asp:Label ID="LabelDebit" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Balance" HeaderStyle-HorizontalAlign="Right"
                ItemStyle-HorizontalAlign="Right" UniqueName="column3">
                <ItemTemplate>
                    <asp:Label ID="LabelBalance" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="&nbsp;" UniqueName="ColumnSymbols">
                <ItemTemplate>
                    <asp:Image runat="server" ID="ImageDocumented" /><asp:Image runat="server" ID="ImageLinked" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Ledgered" UniqueName="ColumnLedgered">
                <ItemTemplate>
                    <asp:Label ID="LabelLedgered" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="&nbsp;" UniqueName="ColumnEdit">
                <ItemTemplate>
                    <asp:HyperLink ID="LinkManage" runat="server" Text="View Tx..." />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
         </Columns>
    </MasterTableView>
    <ClientSettings>
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <FilterMenu>
        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
    </FilterMenu>
</telerik:RadGrid>    