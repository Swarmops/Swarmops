<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Financial.CurrencyTextBox" Codebehind="CurrencyTextBox.ascx.cs" %>

<!-- TODO: Add nice autocomplete stuff -->

<asp:TextBox ID="Input" runat="server" CssClass="alignRight" /><asp:HiddenField ID="NativeCurrency" runat="server"/><asp:HiddenField ID="NativeAmount" runat="server"/>