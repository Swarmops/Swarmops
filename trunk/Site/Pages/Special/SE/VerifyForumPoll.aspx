<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="VerifyForumPoll.aspx.cs" Inherits="Pages_Special_SE_VerifyForumPoll" Title="Verify Forum Poll - PirateWeb" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="../../../Controls/v4/WSGeographyTreeDropDown.ascx" TagName="WSGeographyTreeDropDown" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ButtonLookup">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <pw4:PageTitle Icon="book_go.png" Title="Verify Forum Poll" Description="Checks who voted in a forum poll and where they come from." runat="server" ID="PageTitle" />
    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Forum Poll Id</span><br />
            <div class="DivGroupBoxContents">
                Enter the forum url for the poll:
                <asp:TextBox ID="TextForumUrl" runat="server" Width="432px" />
                <asp:Button ID="ButtonLookup" Text="Lookup" runat="server" OnClick="ButtonLookup_Click" />
                <br />
                In case of a geograpically limited poll, specify valid geography:<br />
                <uc1:wsgeographytreedropdown id="WSGeographyTreeDropDown1" runat="server" />
                <br />
                Right to vote is counted 2 months back from:
                <asp:TextBox ID="TextBoxDate" runat="server"></asp:TextBox>
            </div>
        </div>
    </div>
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Results</span><br />
        <div class="DivGroupBoxContents">
            <asp:Panel ID="Panel1" runat="server">
                <asp:Literal ID="LiteralResults" runat="server" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
