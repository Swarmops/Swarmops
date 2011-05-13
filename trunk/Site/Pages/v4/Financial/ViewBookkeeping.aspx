<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="ViewBookkeeping.aspx.cs" Inherits="Pages_v4_ViewBookkeeping" Title="View Bookkeeping - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function ShowTransactionForm(id, rowIndex) {
                var grid = $find("<%=GridTransactions.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditTransaction.aspx?TransactionId=" + id, "TransactionForm");
                return false;
            }


            function ShowTransactionFormDelayed(transactionId) {
                setTimeout('window.radopen("PopupEditTransaction.aspx?TransactionId=' + transactionId + '", "TransactionForm");', 200);
                return false;
            }

            function ShowAddAccountForm(orgid) {

                window.radopen("PopupAddAccount.aspx?OrganizationId=" + orgid, "TransactionForm");
                return false;
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("Rebind");
                }
                else {
                    $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("RebindAndNavigate");
                }
            }
        </script>

    </telerik:RadCodeBlock>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="TransactionForm" runat="server"
                Title="Manage Transaction" Height="500px" Width="550px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="Move,Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"
        SkinID="Web20" OnAjaxRequest="RadAjaxManager1_AjaxRequest" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="topPanel" />
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                    <telerik:AjaxUpdatedControl ControlID="CreateTransactionPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DropOrganizations">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="topPanel" />
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                    <telerik:AjaxUpdatedControl ControlID="CreateTransactionPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DropAccounts">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                    <telerik:AjaxUpdatedControl ControlID="CreateTransactionPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonViewTransactions">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                    <telerik:AjaxUpdatedControl ControlID="PanelCreateTransaction" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonCreateTransaction">
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        IsSticky="false">
    </telerik:RadAjaxLoadingPanel>
    <pw4:PageTitle Icon="book.png" Title="View / Edit Bookkeeping" Description="View financial records per account, edit transactions, and create transactions"
        runat="server" ID="PageTitle" />
    <asp:Label ID="LabelAccessDenied" runat="server" Visible="false">
    You do not have access to this page.
    </asp:Label>
    <asp:Panel ID="pageContent" runat="server">
        <div class="DivMainContent">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Select Organization and Account</span><br />
                <div class="DivGroupBoxContents">
                    <asp:Panel ID="topPanel" runat="server">
                        <table cellspacing="0" cellpadding="0" border="0" width="90%">
                            <tr>
                                <td nowrap>
                                    Organization:
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                                        AutoPostBack="true">
                                        <asp:ListItem Value="0" Text="-- Select organisation --" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap">
                                    View account:
                                </td>
                                <td nowrap="nowrap">
                                    <asp:DropDownList ID="DropAccounts" runat="server" OnSelectedIndexChanged="DropAccounts_SelectedIndexChanged"
                                        AutoPostBack="true" />
                                </td>
                                <td nowrap="nowrap">
                                    &nbsp;&nbsp;from:
                                </td>
                                <td nowrap="nowrap">
                                    <telerik:RadDatePicker runat="server" ID="DateStart" Width="8em" />
                                </td>
                                <td nowrap="nowrap">
                                    &nbsp;&nbsp;until:
                                </td>
                                <td nowrap="nowrap">
                                    <telerik:RadDatePicker runat="server" ID="DateEnd" Width="8em" />
                                </td>
                                <td>&nbsp;&nbsp;</td>
                                <td>
                                    <asp:Button ID="ButtonViewTransactions" runat="server" Text="View" OnClick="ButtonViewTransactions_Click" />
                                </td>
                                <td width="80%" align="right">
                                    <asp:Button ID="ButtonCreateAccount" runat="server" OnClick="ButtonCreateAccount_Click"
                                        Text="Create new Account" Visible="False" />
                                </td>
                            </tr></table></asp:Panel></div></div><div class="DivGroupBox">
                <span class="DivGroupBoxTitle">
                    <asp:Label ID="LabelTransactionsTitle" runat="server" Text="Transactions" /></span><br />
                <div class="DivGroupBoxContents">
                    <telerik:RadGrid ID="GridTransactions" runat="server" AllowMultiRowSelection="False"
                        AutoGenerateColumns="False" GridLines="None" Skin="Web20" OnItemCreated="GridTransactions_ItemCreated">
                        
                        <HeaderContextMenu>
                            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </HeaderContextMenu>
                        <MasterTableView ShowFooter="false">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Tx#" DataField="FinancialTransactionId" />
                                <telerik:GridBoundColumn HeaderText="Date Time" DataField="TransactionDateTime" DataFormatString="{0:yyyy-MM-dd HH:mmm}" />
                                <telerik:GridBoundColumn HeaderText="Description" DataField="Description" UniqueName="Description" />
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
                                        <asp:Label ID="LabelBalance" runat="server" Text="TBI" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn UniqueName="ManageColumn">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ManageLink" runat="server" Text="Edit transaction..."></asp:HyperLink></ItemTemplate></telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings>
                            <Selecting AllowRowSelect="true" />
                        </ClientSettings>
                        <FilterMenu>
                            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </FilterMenu>
                    </telerik:RadGrid>
                </div>
            </div>
            <asp:Panel ID="CreateTransactionPanel" runat="server" Visible="false">
                <div class="DivGroupBox">
                    <span class="DivGroupBoxTitle">Create Transaction Manually</span><br />
                    <div class="DivGroupBoxContents">
                        <asp:Panel ID="PanelCreateTransaction" runat="server">
                            Create a new transaction with the following base data:<br />
                            <div style="float: left">
                                Date<br />
                                Description&nbsp;<br />
                                &nbsp;<br />
                                Account<br />
                                Amount<br />
                            </div>
                            <div style="float: left">
                                <telerik:RadDatePicker ID="DateCreate" runat="server" />
                                &nbsp;<br />
                                <asp:TextBox ID="TextDescriptionCreate" runat="server" /><br />
                                <asp:DropDownList ID="DropAccountsCreate" runat="server" />
                                <b>
                                    <asp:Label ID="LabelAccountCreate" runat="server" Visible="false" /></b>&nbsp;<br />
                                <asp:TextBox ID="TextAmountCreate" runat="server" Text="0,00" />&nbsp;<br />
                                <asp:Button ID="ButtonCreateTransaction" runat="server" Text="Create" OnClick="ButtonCreateTransaction_Click" />&nbsp;<br />
                            </div>
                            <div style="clear: both">
                                A transaction edit window will open where you can balance the transaction against
                                other accounts.</div></asp:Panel></div></div></asp:Panel></asp:Panel></asp:Content>