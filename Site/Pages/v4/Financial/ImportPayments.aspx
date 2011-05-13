<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="ImportPayments.aspx.cs" Inherits="Pages_v4_ImportPayments"
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
                    <telerik:AjaxUpdatedControl ControlID="TopPartPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <pw4:PageTitle Icon="book_add.png" Title="Import Payments From Bank" Description="Imports payment data (accounts receivable) from bank"
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
                            Organization<br />
                            Import filter</div>
                        <div>
                            &nbsp;&nbsp;<asp:DropDownList ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                                AutoPostBack="True">
                                <asp:ListItem Selected="True" Value="0">-- Select --</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;<br />
                            &nbsp;&nbsp;<asp:DropDownList ID="DropFilters" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropFilters_SelectedIndexChanged">
                                <asp:ListItem Selected="True">-- Select --</asp:ListItem>
                                <asp:ListItem Value="SEBgmax">SE "BgMax" (Bankgiro data)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </asp:Panel>
                </div>
            </div>
            <asp:UpdatePanel ID="UpdatePastePanels" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                        <asp:Panel runat="server" ID="PanelPasteText">
                        <div class="DivGroupBox">
                            <span class="DivGroupBoxTitle">Paste Area</span><br />
                            <div class="DivGroupBoxContents">
                                <asp:TextBox TextMode="MultiLine" Rows="20" ID="TextData" runat="server" /><br />
                                <div style="float: right">
                                    <asp:Button ID="ButtonProcessText" runat="server" Text="Import" OnClick="ButtonProcessText_Click" /></div>
                                <asp:Label ID="LabelImportResultText" runat="server" Text="Paste contents and import."/>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DropFilters" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
</asp:Content>
