<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ViewBalanceSheet.aspx.cs" Inherits="Pages_v4_ViewBalanceSheet" Title="View Balance Sheet - PirateWeb" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">

<style type="text/css"> 
 .RadGrid td {padding:0}
</style> 

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
    </telerik:RadStyleSheetManager>
   <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
        function ShowBudgetForm(id, rowIndex)
        {
            var grid = $find("<%=GridBudgetAccounts.ClientID%>");
            
            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);
                    
            window.radopen("PopupEditBudget.aspx?FinancialAccountId=" + id, "BudgetForm");
            return false;
        }
        
      
        function refreshGrid(arg)
        {
         if(!arg)
         {
         $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("Rebind");            
            }
            else
            {
         $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("RebindAndNavigate");            
            }
        }
        </script>
    </telerik:RadCodeBlock>

    <pw4:PageTitle Icon="chart_bar.png" Title="View Balance Sheet" Description="View status of balance accounts" runat="server" ID="PageTitle" />
    <asp:UpdatePanel ID="UpdatePanel" runat="server" >
    <ContentTemplate>
    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization and Budget Year</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> and <b>2010</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle"><asp:Label ID="LabelOrganizationYearTitle" runat="server" Text="Piratpartiet SE balance sheet for 2010" /></span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridBudgetAccounts" runat="server" AllowMultiRowSelection="False" 
            AutoGenerateColumns="False" GridLines="None" Skin="Default"
            OnItemCreated="GridBudgetAccounts_ItemCreated" OnColumnCreated="GridBudgetAccounts_ColumnCreated"
            OnItemDataBound="GridBudgetAccounts_ItemDataBound">
        <HeaderContextMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </HeaderContextMenu>

        <MasterTableView DataKeyNames="Identity,ParentFinancialAccountId,Name" HierarchyDefaultExpanded="false" HierarchyLoadMode="Client" AllowSorting="true">
            <SelfHierarchySettings ParentKeyName="ParentFinancialAccountId" KeyName="Identity" />
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Account" DataField="Name" UniqueName="Name">
                    <ItemTemplate>
                        <div style="width:250px;"><asp:Label ID="LabelAccountName" runat="server" Visible="false"/></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="AccountId" DataField="Identity" Visible="false" />
                <telerik:GridBoundColumn HeaderText="ParentAccountId" DataField="ParentFinancialAccountId" Visible="false" />
                <telerik:GridTemplateColumn HeaderText="Inbound Balance" HeaderStyle-HorizontalAlign="Right" UniqueName="ColumnBudgetLastYear" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:75px;"><asp:Label ID="LabelInbound" runat="server"/></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Q1" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="ColumnBalanceDiffQ1" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:75px;"><asp:Label ID="LabelDiffQ1" runat="server" Text="[TBI]" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Q2" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="ColumnBalanceDiffQ2" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:75px;"><asp:Label ID="LabelDiffQ2" runat="server" Text="[TBI]" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Q3" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="ColumnBalanceDiffQ3" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:75px;"><asp:Label ID="LabelDiffQ3" runat="server" Text="[TBI]" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Q4" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="ColumnBalanceDiffQ4" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:75px;"><asp:Label ID="LabelDiffQ4" runat="server" Text="[TBI]" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Outbound Balance" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="ColumnOutbound" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:75px;"><asp:Label ID="LabelOutbound" runat="server" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                
                <telerik:GridTemplateColumn UniqueName="ManageColumn" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:100px;"><asp:HyperLink ID="ManageLink" runat="server" Text="Edit account..."></asp:HyperLink></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowExpandCollapse="true">
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <FilterMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </FilterMenu>
    </telerik:RadGrid>
    

    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle"><asp:Label ID="Label1" runat="server" Text="Bookkeeping debug information" /></span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridDebug" runat="server" AllowMultiRowSelection="False" 
            AutoGenerateColumns="False" GridLines="None" Skin="Default">
        <HeaderContextMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </HeaderContextMenu>

        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn HeaderText="Account" DataField="AccountName" />
                <telerik:GridBoundColumn HeaderText="Expenses" DataField="Expenses" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Invoices" DataField="Invoices" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Salaries" DataField="Salaries" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Payouts" DataField="Payouts" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Total Expected" DataField="ExpectedTotal" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Actual" DataField="Actual" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Difference" DataField="Diff" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
            </Columns>
        </MasterTableView>
        <ClientSettings AllowExpandCollapse="true">
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        <FilterMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </FilterMenu>
    </telerik:RadGrid>
    

    </div>
    </div>
    </div>

  
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="BudgetForm" runat="server" Title="Editing Budget" Height="400px"
                        Width="500px" ReloadOnShow="true" Modal="true" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridBudgetAccounts" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="GridBudgetAccounts">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridBudgetAccounts" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
 
    </ContentTemplate>
    <Triggers>
    </Triggers>
    </asp:UpdatePanel>

    <asp:Label ID="LabelTest" runat="server" />
    
    <!--
    
    <asp:Label ID="LiteralDebugExpenses" runat="server" />
    
    -->

</asp:Content>

