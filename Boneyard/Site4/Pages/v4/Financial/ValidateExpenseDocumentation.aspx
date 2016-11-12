<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ValidateExpenseDocumentation.aspx.cs" Inherits="Pages_v4_Financial_ValidateExpenseDocumentation" Title="Validate Expense Claim Documentation - PirateWeb" %>
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



    <pw4:PageTitle Icon="coins.png" Title="Validate Expense Claims" Description="Examine the documentation attached to expense claims and confirm them as valid" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Unvalidated expense claims for Piratpartiet SE</span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridExpenseClaims" runat="server" 
            OnItemCreated="GridExpenseClaims_ItemCreated" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
            PageSize="20" onneeddatasource="GridExpenseClaims_NeedDataSource">
        <MasterTableView AutoGenerateColumns="False" datakeynames="Identity">
            <Columns>
                <telerik:GridClientSelectColumn HeaderText="OK?" UniqueName="ColumnCheckOk" />
                <telerik:GridBoundColumn HeaderText="Claim#" DataField="Identity" UniqueName="column1" />
                <telerik:GridBoundColumn HeaderText="Date" DataField="ExpenseDate" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column3" />
                <telerik:GridBoundColumn HeaderText="Description" DataField="Description" UniqueName="columnDescription" />
                <telerik:GridBoundColumn HeaderText="Amount" DataField="Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
                <telerik:GridTemplateColumn HeaderText="Documentation" UniqueName="ColumnDox">
                    <ItemTemplate>
                        <pw4:DocumentList runat="server" ID="DocumentListClaim" UseShortForm="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Doesn't&nbsp;validate..." HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn">
                    <ItemTemplate>
                        <asp:HyperLink ID="LinkEdit" runat="server">Doesn't&nbsp;validate...</asp:HyperLink>
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
        
        <p><asp:Button ID="ButtonValidate" runat="server" Text="Validate selected claims" 
                onclick="ButtonValidate_Click" /> This certifies you have validated the documentation.</p></div>
    </div>
    </div>


</asp:Content>

