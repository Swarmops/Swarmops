<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="ImportBankTransactions.aspx.cs" Inherits="Pages_v4_ImportBankTransactions"
    Title="Import Bank Transactions - PirateWeb" %>

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
    <pw4:PageTitle Icon="book_add.png" Title="Import Transactions From Bank" Description="Imports financial transactions from online banks by scraping web pages"
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
                            Asset Account</div>
                        <div style="float: left">
                            &nbsp;&nbsp;<asp:DropDownList ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                                AutoPostBack="True">
                                <asp:ListItem Selected="True" Value="0">-- Select --</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;<br />
                            &nbsp;&nbsp;<asp:DropDownList ID="DropAssetAccount" runat="server" />
                        </div>
                        <div style="float: right; border-left: solid 1px #808080; padding-left: 10px; text-align: right;
                            padding-bottom: 5px">
                            Deposits smaller than
                            <asp:TextBox Columns="10" runat="server" ID="TextDepositLimit" />
                            are automatically
                            <asp:DropDownList ID="DropAutoDeposits" runat="server" />
                            <br />
                            Withdrawals smaller than
                            <asp:TextBox Columns="10" runat="server" ID="TextWithdrawalLimit" />
                            are automatically
                            <asp:DropDownList ID="DropAutoWithdrawals" runat="server" />
                        </div>
                        <div style="clear: both; border-top: solid 1px #808080; margin-top: 5px; padding-top: 5px">
                            <span>Import filter:</span>
                            <asp:DropDownList ID="DropFilters" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropFilters_SelectedIndexChanged">
                                <asp:ListItem Selected="True">-- Select --</asp:ListItem>
                                <asp:ListItem Value="SEB.se">SEB (Swedish bank)</asp:ListItem>
                                <asp:ListItem Value="Paypal">Paypal</asp:ListItem>
                            </asp:DropDownList>
                            <br />
                            <span style="line-height: 120%"><asp:Label ID="LabelFilterInstructions" runat="server" Text="Select an organization, then the other import parameters." /></span>
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
