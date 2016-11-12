<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditTransaction.aspx.cs" Inherits="Pages_v4_PopupEditTransaction" %>
<%@ Import Namespace="Telerik.Web.UI"%>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Page Title Is Set In Page_Load()</title>
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />    
    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().Close();
            GetRadWindow().BrowserWindow.refreshGrid(args);
        }
		
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz as well)
				
            return oWindow;
        }

        function CancelEdit()
        {
            GetRadWindow().Close();		
        }
    </script>
</head>

<body style="background-color: #f8e7ff; padding-left:10px; padding-top:10px; padding-right: 10px; padding-bottom: 10px">
    <form id="form1" runat="server" method="post">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="Update1" runat="server" UpdateMode="Always">
    <ContentTemplate>
    Date:&nbsp;<telerik:RadDatePicker ID="DatePicker" runat="server" Skin="Web20" /><br /><br />
    Description:&nbsp;&nbsp;<asp:TextBox ID="TextDescription" runat="server" /><br /><br />
    <telerik:RadGrid ID="GridTransactionRows" runat="server" 
            AutoGenerateColumns="False" GridLines="None" Skin="Web20" 
            onitemcreated="GridTransactionRows_ItemCreated">
        <HeaderContextMenu>
            <CollapseAnimation Duration="200" Type="OutQuint" />
        </HeaderContextMenu>
        <MasterTableView DataKeyNames="Identity" ShowFooter="false">
            <RowIndicatorColumn>
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="AccountName" HeaderText="Account" 
                    UniqueName="column">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Credit" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn">
                <ItemTemplate>
                <asp:Label ID="LabelCredit" runat="server" />
                </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Debit" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn1">
                <ItemTemplate>
                <asp:Label ID="LabelDebit" runat="server" />
                </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ItemStyle-HorizontalAlign="Right" 
                    ImageUrl="~/Images/Public/Silk/decline.png" UniqueName="column1">
                </telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
        <FilterMenu>
            <CollapseAnimation Duration="200" Type="OutQuint" />
        </FilterMenu>
        </telerik:RadGrid>
        <br />
        <asp:Panel ID="PanelModifiableTransaction" runat="server">
        <div style="float:right"><asp:Button runat="server" ID="ButtonAdd" Text="Add" onclick="ButtonAdd_Click" /></div>
        Add Row: <asp:DropDownList ID="DropAccounts" runat="server" />, amount <asp:TextBox runat="server" ID="TextAmount" Text="0.00" /></asp:Panel>
        <asp:Panel ID="PanelUnmodifiableTransaction" runat="server" Visible="false">This transaction is tied to an event and can only be modified through that event.</asp:Panel>
        <hr />
      </ContentTemplate>
      </asp:UpdatePanel>

        <asp:Panel ID="PanelDependencyDocuments" runat="server">
        <b>Documents for <asp:Label ID="LabelDependency" runat="server" />:</b> <pw4:DocumentList ID="DocumentsDependency" runat="server" /><br /><br />
        </asp:Panel>
        
        <asp:Panel ID="PanelPayoutDetails" runat="server" Visible="false">
        This tranasction is <b>Payout #<asp:Label ID="LabelPayoutIdentity" runat="server" /></b> (<asp:Label ID="LabelPayoutReference" runat="server" />).
        </asp:Panel>
 
        <b>Transaction Documents:</b> <pw4:DocumentList ID="DocumentList" runat="server" />
        
        <br /><b>Upload document:</b> (png, jpg, or pdf)
        <telerik:RadProgressManager ID="UploadProgressManager" runat="server" 
            EnableEmbeddedBaseStylesheet="False" EnableEmbeddedSkins="False"  />
        <div style="float:right"><asp:Button ID="ButtonUpload" runat="server" Text="Upload" onclick="ButtonUpload_Click" /> </div>
        <telerik:RadUpload ID="Upload" runat="server" 
        BorderStyle="None" 
        ControlObjectsVisibility="None" InputSize="50" MaxFileInputsCount="1" Skin="" 
        TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" />
        <telerik:RadProgressArea runat="server" ID="UploadProgress" 
        ProgressIndicators="TotalProgressBar, TotalProgress, TotalProgressPercent" 
        BorderStyle="None" Skin="Web20" />
        <hr />
        <div style="text-align:right"><asp:Button ID="ButtonClose" runat="server" 
                Text="Close" onclick="ButtonClose_Click" /></div>
    </form>
</body>
</html>
