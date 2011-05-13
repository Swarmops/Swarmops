<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="SubscriptionSettings.aspx.cs" Inherits="Pages_Account_SubscriptionSettings"
    Title="PirateWeb - Subscriptions" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register src="../../../../Controls/v4/v3/Subscriptions.ascx" tagname="Subscriptions" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Edit subscriptions" Description="Edit my subscriptions"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <br />
        <uc1:Subscriptions ID="Subscriptions1" runat="server" />
        <br />
    </div>
</asp:Content>
