<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ListReporters.aspx.cs" Inherits="Pages_v4_ListReporters" Title="List Reporters - PirateWeb" %>
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
        function ShowTransactionForm(id, rowIndex)
        {
            var grid = $find("<%=GridReporters.ClientID%>");
            
            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);
                    
            window.radopen("PopupEditReporter.aspx?ReporterId=" + id, "ReporterForm");
            return false;
        }
        
        
        function ShowTransactionFormDelayed (transactionId)
        {
            setTimeout ('window.radopen("PopupEditTransaction.aspx?TransactionId=' + transactionId + '", "TransactionForm");', 200);
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

    <pw4:PageTitle Icon="cameras.png" Title="List Reporters" Description="List and add reporters that receive press releases" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle"><asp:Label ID="LabelTransactionsTitle" runat="server" Text="All reporters" /></span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridReporters" runat="server" 
            AutoGenerateColumns="False" GridLines="None" Skin="Web20" 
            onitemcreated="GridReporters_ItemCreated" 
            onitemcommand="GridReporters_ItemCommand">
        <HeaderContextMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </HeaderContextMenu>

        <MasterTableView ShowFooter="false" DataKeyNames="ReporterId">
            <Columns>
                <telerik:GridBoundColumn HeaderText="Reporter" DataField="Name" />
                <telerik:GridBoundColumn HeaderText="Email" DataField="Email" />
                <telerik:GridTemplateColumn HeaderText="Categories">
                    <ItemTemplate>
                        <asp:Label ID="LabelCategories" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ItemStyle-HorizontalAlign="Right" 
                    ImageUrl="~/Images/Public/Fugue/icons-shadowless/cross-circle.png" UniqueName="DeleteCommand" 
                    CommandName="Delete">
                    <ItemStyle HorizontalAlign="Right" />
                </telerik:GridButtonColumn>
            </Columns>

        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="false" />
        </ClientSettings>
        <FilterMenu>
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </FilterMenu>
    </telerik:RadGrid>

                <!--
                <telerik:GridTemplateColumn UniqueName="ManageColumn">
                    <ItemTemplate>
                        <asp:HyperLink ID="ManageLink" runat="server" Text="Edit reporter..."></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>-->
    

    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle"><asp:Label ID="LabelAddReporter" runat="server" Text="Add reporter" /></span><br />
    <div class="DivGroupBoxContents">
    <div style="width:100px; float:left">Name:<br />Media:<br />Email:<br />Categories:</div>
    <asp:TextBox ID="TextReporterName" runat="server" /> (empty if for the editorial staff)<br />
    <asp:TextBox ID="TextMediaName" runat="server" />&nbsp;<br />
    <asp:TextBox ID="TextEmail" runat="server" />&nbsp;<br />
    <asp:CheckBoxList ID="CheckListCategories" runat="server" Height="100" />
    <div style="width:100px; float:left">Done?</div><asp:Button ID="ButtonAdd" 
            runat="server" Text="Add" onclick="ButtonAdd_Click" />
    </div>
    </div>
    </div>
    </div>

  
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="ReporterForm" runat="server" Title="Edit/Add Reporter" Height="400px"
                        Width="500px" Left="150px" ReloadOnShow="true" Modal="true" Behaviors="None" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridReporters" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="GridReporters">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridReporters" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
 
</asp:Content>

