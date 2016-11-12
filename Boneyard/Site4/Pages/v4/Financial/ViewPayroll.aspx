<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ViewPayroll.aspx.cs" Inherits="Pages_v4_Accounting_ViewEditPayroll" Title="View Payroll - PirateWeb" %>
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

            function ShowPayrollAdjustmentsForm(id, rowIndex) {
                var grid = $find("<%=GridPayroll.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditPayrollAdjustments.aspx?PayrollItemId=" + id, "PayrollAdjustmentForm");
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
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="PayrollAdjustmentForm" runat="server"
                Title="Edit Payroll Item..." Height="450px" Width="550px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline">
        <ClientEvents OnResponseEnd="RadAjaxManager_OnResponseEnd" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridPayroll" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        Style="position: absolute; left: 0px" meta:resourcekey="RadAjaxLoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <pw4:PageTitle Icon="money_euro.png" Title="View/Edit Payroll" Description="View and edit the list of people receiving every month" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Payroll of Piratpartiet SE</span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridPayroll" runat="server" 
            OnItemCreated="GridPayroll_ItemCreated" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" 
            PageSize="50" onneeddatasource="GridPayroll_NeedDataSource">
<MasterTableView AutoGenerateColumns="False" datakeynames="Identity">
    <Columns>
        <telerik:GridBoundColumn HeaderText="Employee" DataField="PersonCanonical" UniqueName="ColumnCanonical" />
        <telerik:GridTemplateColumn HeaderText="Reports To" UniqueName="TemplateColumn3">
            <ItemTemplate>
                <asp:Label ID="LabelReportsTo" runat="server"></asp:Label>
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridBoundColumn HeaderText="Salary" DataField="BaseSalary" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
        <telerik:GridTemplateColumn HeaderText="Monthly Cost"  HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn3">
            <ItemTemplate>
                <asp:Label ID="LabelMonthlyCost" runat="server"></asp:Label>
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Budget" UniqueName="column">
            <ItemTemplate>
                <asp:Label ID="LabelBudget" runat="server"></asp:Label>
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridBoundColumn HeaderText="Hired" DataField="EmployedDate" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column3" />
        <telerik:GridTemplateColumn HeaderText="" UniqueName="columnAdjust">
            <ItemTemplate>
                <asp:HyperLink ID="LinkEdit" runat="server" Text="Adjustments..."></asp:HyperLink>
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

