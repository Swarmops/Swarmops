<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="AttestCosts.aspx.cs" Inherits="Pages_v4_Financial_AttestCosts" Title="Attest Expense Claims, Invoices - PirateWeb" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function RadAjaxManager_OnResponseEnd(sender, eventArgs) {
            }

            function ShowExpenseClaimForm(id, rowIndex) {
                var grid = $find("<%=GridAttestables.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditExpenseClaim.aspx?ExpenseClaimId=" + id, "ExpenseClaimForm");
                return false;
            }

            function ShowInboundInvoiceForm(id, rowIndex) {
                var grid = $find("<%=GridAttestables.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditInboundInvoice.aspx?InboundInvoiceId=" + id, "InboundInvoiceForm");
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
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="ExpenseClaimForm" runat="server"
                Title="Edit Expense Claim" Height="450px" Width="500px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="Close" />
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="InboundInvoiceForm" runat="server"
                Title="Edit Inbound Invoice" Height="450px" Width="500px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline">
        <ClientEvents OnResponseEnd="RadAjaxManager_OnResponseEnd" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridAttestables" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        Style="position: absolute; left: 0px" meta:resourcekey="RadAjaxLoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>



    <pw4:PageTitle Icon="coins.png" Title="Attest Expense Claims, Invoices, Salaries" Description="Attest that your budgets may be charged with these line items" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Costs awaiting your attestation</span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridAttestables" runat="server" 
            OnItemCreated="GridAttestables_ItemCreated" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
            PageSize="20" onneeddatasource="GridAttestables_NeedDataSource">
        <MasterTableView AutoGenerateColumns="False" datakeynames="Identity">
            <Columns>
                <telerik:GridClientSelectColumn HeaderText="OK?" UniqueName="ColumnCheckOk" />
                <telerik:GridBoundColumn HeaderText="Item" DataField="IdentityDisplay" UniqueName="columnFoo" />
                <telerik:GridBoundColumn HeaderText="Beneficiary" DataField="Beneficiary" UniqueName="column3" />
                <telerik:GridTemplateColumn HeaderText="Description" UniqueName="ColumnDescription">
                    <ItemTemplate>
                        <asp:Label ID="LabelDescription" runat="server" /> <pw4:DocumentList ID="DocList" UseShortForm="true" runat="server" Visible="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Budget" DataField="BudgetName" UniqueName="columnBudget" />
                <telerik:GridTemplateColumn HeaderText="Budget Remaining" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="ColumnBudgetRemaining">
                    <ItemTemplate>
                        <asp:Label ID="LabelBudgetRemaining" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Amount Requested" DataField="AmountRequestedDecimal" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
                <telerik:GridTemplateColumn HeaderText="Won't attest..." HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn">
                    <ItemTemplate>
                        <asp:HyperLink ID="LinkEdit" runat="server">Won't&nbsp;attest...</asp:HyperLink>
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
        
        <p><asp:Button ID="ButtonAttest" runat="server" Text="Attest selected" 
                onclick="ButtonAttest_Click" /> Attest the selected costs for payment.</p></div>
    </div>
    
    </div>


                <!--<telerik:GridTemplateColumn HeaderText="Won't attest..." HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn">
                    <ItemTemplate>
                        <asp:HyperLink ID="LinkEdit" runat="server">Won't&nbsp;attest...</asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>-->


</asp:Content>

