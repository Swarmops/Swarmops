<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ViewExpenseClaims.aspx.cs" Inherits="Pages_v4_Accounting_ViewEditExpenseClaims" Title="Untitled Page" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function RadAjaxManager_OnResponseEnd(sender, eventArgs) {
            }

            function ShowExpenseClaimForm(id, rowIndex) {
                var grid = $find("<%=GridExpenseClaims.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditExpenseClaim.aspx?ExpenseClaimId=" + id, "ExpenseClaimForm");
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
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline">
        <ClientEvents OnResponseEnd="RadAjaxManager_OnResponseEnd" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridExpenseClaims" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        Style="position: absolute; left: 0px" meta:resourcekey="RadAjaxLoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>



    <pw4:PageTitle Icon="coins.png" Title="View/Edit Expense Claims" Description="View outstanding and historical expense claims, and fix problems with them" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">All expense claims for Piratpartiet SE</span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridExpenseClaims" runat="server" 
            OnItemCreated="GridExpenseClaims_ItemCreated" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" 
            PageSize="50" onneeddatasource="GridExpenseClaims_NeedDataSource">
<MasterTableView AutoGenerateColumns="False" datakeynames="Identity">
    <Columns>
        <telerik:GridBoundColumn HeaderText="Claim#" DataField="Identity" UniqueName="column1" />
        <telerik:GridBoundColumn HeaderText="Claimer" DataField="ClaimerCanonical" UniqueName="ColumnCanonical" />
        <telerik:GridBoundColumn HeaderText="Claimed" DataField="CreatedDateTime" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column3" />
        <telerik:GridBoundColumn HeaderText="Amount" DataField="Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4">
        </telerik:GridBoundColumn>
        <telerik:GridTemplateColumn HeaderText="Budget/Year" UniqueName="column">
            <ItemTemplate>
                <asp:Label ID="LabelBudgetYear" runat="server"></asp:Label>
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Claimed" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn3">
            <ItemTemplate>
                <asp:Image ID="ImageClaimed" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Attested" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn2">
            <ItemTemplate>
                <asp:Image ID="ImageAttested" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Validated" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" 
            UniqueName="TemplateColumn1">
            <ItemTemplate>
                <asp:Image ID="ImageDocumented" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Repaid" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn">
            <ItemTemplate>
                <asp:Image ID="ImageRepaid" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="View/Edit..." HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn">
            <ItemTemplate>
                <asp:HyperLink ID="LinkEdit" runat="server">View/Edit...</asp:HyperLink>
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
        </telerik:RadGrid></div>
    </div>
    </div>


</asp:Content>

