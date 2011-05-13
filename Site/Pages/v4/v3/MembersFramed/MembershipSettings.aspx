<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="MembershipSettings.aspx.cs" Inherits="Pages_Members_MembershipSettings"
    Title="PirateWeb - Member Details - Settings" %>

<%@ Register Src="~/Controls/v3/SetPersonPassword.ascx" TagName="SetPersonPassword"
    TagPrefix="uc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <br />
        <br />
        <uc1:SetPersonPassword ID="ControlSetPassword" runat="server" />
    </div>
</asp:Content>
