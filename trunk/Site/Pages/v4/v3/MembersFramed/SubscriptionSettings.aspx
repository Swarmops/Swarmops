<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="SubscriptionSettings.aspx.cs" Inherits="Pages_Members_SubscriptionSettings" Title="PirateWeb - Subscriptions" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="../../../../Controls/v4/v3/PersonDetailsPagesMenu.ascx" TagName="PersonDetailsPagesMenu" TagPrefix="uc1" %>
<%@ Register src="../../../../Controls/v4/v3/Subscriptions.ascx" tagname="Subscriptions" tagprefix="uc2" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <br />
        <uc2:Subscriptions ID="Subscriptions1" runat="server" />
        <br />
    </div>
</asp:Content>
