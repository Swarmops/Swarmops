<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" ValidateRequest="false"
    CodeFile="MembershipSettings.aspx.cs" Inherits="Pages_Account_MembershipSettings"
    Title="PirateWeb - Membership Settings" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v3/SetPersonPassword.ascx" TagName="SetPersonPassword"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Change password" Description="Change your PirateWeb password"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <uc1:SetPersonPassword ID="SetPersonPassword1" runat="server" />
    </div>
</asp:Content>
