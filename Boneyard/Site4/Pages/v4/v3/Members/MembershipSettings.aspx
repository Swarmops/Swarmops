<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="MembershipSettings.aspx.cs" Inherits="Pages_Members_MembershipSettings"
    Title="PirateWeb - Member Details - Settings" %>

<%@ Register Src="~/Controls/v4/v3/PersonDetailsPagesMenu.ascx" TagName="PersonDetailsPagesMenu"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/v3/SetPersonPassword.ascx" TagName="SetPersonPassword"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1 class="ContentHeader">
            <asp:Label ID="LabelHeader" runat="server" Text=""></asp:Label>
        </h1>
        <uc2:PersonDetailsPagesMenu ID="PersonDetailsPagesMenu" runat="server" />
        <uc1:SetPersonPassword ID="ControlSetPassword" runat="server" />
    </div>
</asp:Content>
