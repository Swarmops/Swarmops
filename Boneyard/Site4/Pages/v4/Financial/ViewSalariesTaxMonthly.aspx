<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ViewSalariesTaxMonthly.aspx.cs" Inherits="Pages_v4_ViewSalariesTaxMonthly" Title="View Monthly Salary Taxes - PirateWeb" %>
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
            var grid = $find("<%=GridSalaryTaxData.ClientID%>");
            
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

    <pw4:PageTitle Icon="money_delete.png" Title="View Salary Tax Data" Description="View monthly salary tax data" runat="server" ID="PageTitle" />
    <asp:UpdatePanel ID="UpdatePanel" runat="server" >
    <ContentTemplate>
    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle"><asp:Label ID="LabelOrganizationYearTitle" runat="server" Text="Piratpartiet SE salary tax data" /></span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridSalaryTaxData" runat="server" AllowMultiRowSelection="False" 
            AutoGenerateColumns="False" GridLines="None" Skin="Default"
            OnItemCreated="GridSalaryTaxData_ItemCreated" OnColumnCreated="GridSalaryTaxData_ColumnCreated"
            OnItemDataBound="GridSalaryTaxData_ItemDataBound">
        <HeaderContextMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </HeaderContextMenu>

        <MasterTableView DataKeyNames="Identity,ParentIdentity,Name" HierarchyDefaultExpanded="false" HierarchyLoadMode="Client" AllowSorting="true">
            <SelfHierarchySettings ParentKeyName="ParentIdentity" KeyName="Identity" />
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Account" DataField="Name" UniqueName="Name">
                    <ItemTemplate>
                        <div style="width:250px;"><asp:Label ID="LabelRowLabel" runat="server" Visible="true"/></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Salary Gross" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGross" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;"><asp:Label ID="LabelSalaryGross" runat="server" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="1945-1983" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGrossMain" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;"><asp:Label ID="LabelSalaryGrossMain" runat="server" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="31.42%" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGrossMainTax" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;"><asp:Label ID="LabelSalaryGrossMainTax" runat="server" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="1984-" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGrossYoung" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;">---</div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="15.49%" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGrossYoungTax" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;">---</div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="1938-1944" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGrossOld" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;">---</div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="10.21%" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryGrossOldTax" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;">---</div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Deducted" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="SalaryDeductedTax" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;"><asp:Label ID="LabelSalaryDeducted" runat="server" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Taxman Total" HeaderStyle-HorizontalAlign="Right"
                    UniqueName="TaxmanTotal" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:60px;"><asp:Label ID="LabelTaxmanTotal" runat="server" /></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn UniqueName="ManageColumn" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <div style="width:100px;"><asp:HyperLink ID="ManageLink" runat="server" Text="Tax Form"></asp:HyperLink></div>
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
                    <telerik:AjaxUpdatedControl ControlID="GridSalaryTaxData" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="GridSalaryTaxData">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridSalaryTaxData" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
 
    </ContentTemplate>
    <Triggers>
    </Triggers>
    </asp:UpdatePanel>

    <asp:Label ID="LabelTest" runat="server" />

</asp:Content>

