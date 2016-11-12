<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="UploadFinancialData.aspx.cs" Inherits="Activizr.Site.Pages.v4.Financial.UploadFinancialData"
    Title="Import Payment Data - PirateWeb" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" IsSticky="false">
        <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>'
            style="border: none" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="DropOrganizations">
                <UpdatedControls>
                    
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <pw4:PageTitle Icon="book_add.png" Title="Upload Bank Files" Description="Upload various data files from financial institutions"
        runat="server" ID="PageTitle" />
        
    <asp:Label ID="LabelAccessDenied" runat="server" Visible="false">
    You do not have access to this page.
    </asp:Label>
    
    <asp:Panel ID="pageContent" runat="server">
        <div class="DivMainContent">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Import Parameters</span><br />
                <div class="DivGroupBoxContents">
                    <asp:Panel ID="TopPartPanel" runat="server">
                        <div style="float: left">
                            Organization&nbsp;&nbsp;<br />
                            Import filter&nbsp;&nbsp;<br />
                            File&nbsp;&nbsp;</div>
                        <div>
                            <asp:DropDownList ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                                AutoPostBack="True">
                                <asp:ListItem Selected="True" Value="0">-- Select --</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;<br />
                            <asp:DropDownList ID="DropFilters" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropFilters_SelectedIndexChanged">
                                <asp:ListItem Selected="True">-- Select --</asp:ListItem>
                                <asp:ListItem Value="1">Paypal file</asp:ListItem>
                                <asp:ListItem Value="2">Payson file</asp:ListItem>
                            </asp:DropDownList>&nbsp;<br />
                            <telerik:RadUpload ID="Upload" runat="server" ControlObjectsVisibility="None" 
                                MaxFileInputsCount="2" EnableFileInputSkinning="false" 
                                TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" /><asp:Button ID="ButtonUpload" 
                                                runat="server" Text="Upload file" 
                                onclick="ButtonUpload_Click" CausesValidation="False" />&nbsp;<br clear="all"/>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
