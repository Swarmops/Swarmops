<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" AutoEventWireup="true"
    CodeFile="SubscriptionSettings.aspx.cs" Inherits="Pages_Public_SE_SubscriptionSettings"
    Title="PirateWeb - Subscriptions" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="../../../../Controls/v4/v3/Subscriptions.ascx" TagName="Subscriptions"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1 class="ContentHeader">
            <asp:Label ID="labelMembershipsHeader" runat="server" Text="Subscriptions" meta:resourcekey="labelMembershipsHeaderResource1"></asp:Label></h1>
        <br />
        <asp:Panel ID="Panel1" runat="server" Visible="true">
            <asp:Label ID="Label1" runat="server" Text="Invalid call"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server" Visible="false">
            <uc1:Subscriptions ID="Subscriptions1" runat="server" />
        </asp:Panel>
    </div>
</asp:Content>
