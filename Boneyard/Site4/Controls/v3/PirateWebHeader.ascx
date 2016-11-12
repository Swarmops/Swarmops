<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PirateWebHeader.ascx.cs" Inherits="Controls_PirateWebHeader" %>
<div class="Header"><div style="float:right; position: relative; top: 6pt; padding-right: 8px"><asp:Label ID="labelLoggedInAs" runat="server" Text="labelLoggedInAs"></asp:Label> <b><asp:Label ID="labelCurrentUserName" runat="server" Text="labelCurrentUserName"></asp:Label></b>. [<asp:HyperLink ID="linkLogout" runat="server" NavigateUrl="~/Pages/Security/Logout.aspx">linkLogout</asp:HyperLink>]</div>
<span style="font: 16pt Impact; padding-left: 8px">PIRATEWEB</span></div>

