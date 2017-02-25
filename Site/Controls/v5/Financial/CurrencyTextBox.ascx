<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Financial.CurrencyTextBox" Codebehind="CurrencyTextBox.ascx.cs" %>
<%@ Import Namespace="Swarmops.Common.Enums" %>

<!-- TODO: Add nice autocomplete stuff -->

 <% if (this.Layout == LayoutDirection.Vertical) { %><div class="stacked-input-control"><% } %>
    <asp:TextBox ID="Input" runat="server" CssClass="alignRight" /><asp:HiddenField ID="NativeCurrency" runat="server"/><asp:HiddenField ID="NativeAmount" runat="server" />
 <% if (this.Layout == LayoutDirection.Vertical) { %></div><% } %>