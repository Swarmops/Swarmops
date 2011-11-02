<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="SubscriptionSettings.aspx.cs" Inherits="Pages_Members_SubscriptionSettings"
    Title="PirateWeb - Subscriptions" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register src="../../../../Controls/v4/v3/PersonDetailsPagesMenu.ascx" tagname="PersonDetailsPagesMenu" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1 class="ContentHeader">
            <asp:Label ID="labelMembershipsHeader" runat="server" Text="Subscriptions" 
                meta:resourcekey="labelMembershipsHeaderResource1"></asp:Label></h1>
        <uc1:PersonDetailsPagesMenu ID="PersonDetailsPagesMenu" runat="server" />
        <br />
        <asp:Label ID="labelCurrentMember" runat="server" Text="labelCurrentMember" 
            meta:resourcekey="labelCurrentMemberResource1"></asp:Label><br /><br />
        <asp:Label ID="LabelIntroduction" runat="server" 
            Text="Check the newsletters you want to receive and press Save." 
            meta:resourcekey="LabelIntroductionResource1"></asp:Label><br />
        <br />
        <br />
        <asp:CheckBoxList ID="listSubscriptions" runat="server" 
            meta:resourcekey="listSubscriptionsResource1">
        </asp:CheckBoxList>
        <br />
        <asp:Button ID="ButtonSave" runat="server" Text="Save" 
            OnClick="ButtonSave_Click" meta:resourcekey="ButtonSaveResource1" />
    </div>
</asp:Content>
