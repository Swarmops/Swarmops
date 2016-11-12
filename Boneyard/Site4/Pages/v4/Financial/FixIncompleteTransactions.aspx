<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="FixIncompleteTransactions.aspx.cs" Inherits="Pages_v4_FixIncompleteTransactions"
    Title="Fix Incomplete Transactions - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/SelectOrganizationLine.ascx" TagName="SelectOrganizationLine"
    TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function RadAjaxManager_OnResponseEnd(sender, eventArgs) {
            }

            function ShowTransactionForm(id, rowIndex) {
                var grid = $find("<%=GridTransactions.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditTransaction.aspx?TransactionId=" + id, "TransactionForm");
                return false;
            }

            function ShowPayoutForm(id, rowIndex) {
                var grid = $find("<%=GridPayouts.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditPayout.aspx?PayoutId=" + id, "PayoutForm");
                return false;
            }

            function ShowOutboundInvoiceForm(id, rowIndex) {
                var grid = $find("<%=GridOutboundInvoices.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupMapOutboundInvoice.aspx?OutboundInvoiceId=" + id, "PayoutForm");
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
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20" Height="16px">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="TransactionForm" runat="server"
                Title="Manage Transaction" Height="400px" Width="500px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="None" />
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="PayoutForm" runat="server"
                Title="Manage Payout" Height="450px" Width="500px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline">
        <ClientEvents OnResponseEnd="RadAjaxManager_OnResponseEnd" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                    <telerik:AjaxUpdatedControl ControlID="GridPayouts" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DropOrganizations">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                    <telerik:AjaxUpdatedControl ControlID="DropAutoBalanceAccount" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonAutoBalance">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridTransactions" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        Style="position: absolute; left: 0px" meta:resourcekey="RadAjaxLoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <pw4:PageTitle Icon="book_error.png" Title="Fix Incomplete Transactions" Description="Fix transactions that are unbalanced or lack documentation"
        runat="server" ID="PageTitle" />
    <asp:Label ID="LabelAccessDenied" runat="server" Visible="false">
    You do not have access to this page.
    </asp:Label>
    <asp:Panel ID="pageContent" runat="server">
        <div class="DivMainContent">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Which organization?</span><br />
                <div class="DivGroupBoxContents">
                    I want to see incomplete transactions for
                    <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged" />
                    .
                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Incomplete transactions</span><br />
                <div class="DivGroupBoxContents">
                    <telerik:RadGrid ID="GridTransactions" runat="server" AllowMultiRowSelection="true" ShowFooter="true"
                        AutoGenerateColumns="False" GridLines="None" OnItemCreated="GridTransactions_ItemCreated">
                        <HeaderContextMenu>
                            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </HeaderContextMenu>
                        <MasterTableView ShowFooter="true" DataKeyNames="Identity">
                            <Columns>
                                <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" />
                                <telerik:GridBoundColumn HeaderText="#" DataField="Identity" />
                                <telerik:GridBoundColumn HeaderText="Date" DataField="DateTime" DataFormatString="{0:yyyy-MM-dd}" />
                                <telerik:GridBoundColumn HeaderText="Description" DataField="Description" UniqueName="Description">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Tx Balance" HeaderStyle-HorizontalAlign="Right"
                                    UniqueName="column3" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="LabelBalance" runat="server" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="LabelBalanceAccumulated" runat="server" />
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Account" UniqueName="column2">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="LabelAccount" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Problems" UniqueName="column3">
                                    <ItemTemplate>
                                        <asp:Label ID="LabelProblems" runat="server" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="LabelProblemTotals" runat="server" />
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="ManageColumn">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ManageLink" runat="server" Text="Fix transaction..."></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                            <EditFormSettings>
                                <EditColumn UniqueName="EditCommandColumn1">
                                </EditColumn>
                            </EditFormSettings>
                        </MasterTableView>
                        <ClientSettings>
                            <Selecting AllowRowSelect="true" />
                        </ClientSettings>
                        <FilterMenu>
                            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </FilterMenu>
                    </telerik:RadGrid>
                    <div style="width: 560px">
                        Auto-balance all selected transactions against<asp:DropDownList ID="DropAutoBalanceAccount"
                            runat="server" />
                        <asp:Button ID="ButtonAutoBalance" runat="server" Text="Balance" OnClick="ButtonAutoBalance_Click" />
                    </div>
                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Open, unmapped payouts for Piratpartiet SE</span><br />
                <div class="DivGroupBoxContents">
                    <telerik:RadGrid ID="GridPayouts" runat="server" 
                            
                            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
                            onneeddatasource="GridPayouts_NeedDataSource" OnItemCreated="GridPayouts_ItemCreated">
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Identity">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="#" DataField="Identity" UniqueName="columnIdentity" />
                                <telerik:GridBoundColumn HeaderText="Expected Date" DataField="ExpectedTransactionDate" UniqueName="columnDate" DataFormatString="{0:yyyy-MM-dd}" />
                                <telerik:GridBoundColumn HeaderText="Recipient" DataField="Recipient" UniqueName="columnRecipient" />
                                <telerik:GridBoundColumn HeaderText="Reference" DataField="Reference" UniqueName="columnReference" />
                                <telerik:GridBoundColumn HeaderText="Amount" DataField="Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
                                 <telerik:GridTemplateColumn UniqueName="ManageColumn" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ManageLink" runat="server" Text="Manual action..."></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                            </Columns>
                            <RowIndicatorColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>

                            <ExpandCollapseColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                        </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>

                    </telerik:RadGrid>

                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Automap payouts?</span><br />
                <div class="DivGroupBoxContents">
                    <p style="line-height: 120%">Click here to automap open payouts to unbalanced transactions. This takes a while but is useful after editing payouts/transactions to map each other.</p>
                    <span style="text-align:right"><asp:Button ID="ButtonAutoRemap" runat="server" OnClick="ButtonAutoRemap_Click" Text="Automap" /></span>
                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Open, unmapped payment groups (inbound) for Piratpartiet SE</span><br />
                <div class="DivGroupBoxContents">
                    <telerik:RadGrid ID="GridPaymentGroups" runat="server" 
                            
                            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
                            onneeddatasource="GridPaymentGroups_NeedDataSource" OnItemCreated="GridPaymentGroups_ItemCreated">
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Identity">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="#" DataField="Identity" UniqueName="columnIdentity" />
                                <telerik:GridBoundColumn HeaderText="Payment Date" DataField="DateTime" UniqueName="columnDate" DataFormatString="{0:yyyy-MM-dd}" />
                                <telerik:GridBoundColumn HeaderText="Payers" DataField="Payers" UniqueName="columnPayers" />
                                <telerik:GridBoundColumn HeaderText="Amount" DataField="AmountDecimal" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
                                 <telerik:GridTemplateColumn UniqueName="ManageColumn" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ManageLink" runat="server" Text="Manual action..."></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                            </Columns>
                            <RowIndicatorColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>

                            <ExpandCollapseColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                        </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>

                    </telerik:RadGrid>

                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Open, unmapped outbound invoices for Piratpartiet SE</span><br />
                <div class="DivGroupBoxContents">
                    <telerik:RadGrid ID="GridOutboundInvoices" runat="server" 
                            
                            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
                            onneeddatasource="GridOutboundInvoices_NeedDataSource" OnItemCreated="GridOutboundInvoices_ItemCreated">
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Identity">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="#" DataField="Identity" UniqueName="columnIdentity" />
                                <telerik:GridBoundColumn HeaderText="Expected Date" DataField="DueDate" UniqueName="columnDate" DataFormatString="{0:yyyy-MM-dd}" />
                                <telerik:GridBoundColumn HeaderText="Recipient" DataField="CustomerName" UniqueName="columnRecipient" />
                                <telerik:GridBoundColumn HeaderText="Amount" DataField="Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
                                 <telerik:GridTemplateColumn UniqueName="ManageColumn" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="ManageLink" runat="server" Text="Manual action..."></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                            </Columns>
                            <RowIndicatorColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>

                            <ExpandCollapseColumn>
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                        </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>

                    </telerik:RadGrid>

                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
