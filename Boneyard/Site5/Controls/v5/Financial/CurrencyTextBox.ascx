<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CurrencyTextBox.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Financial.CurrencyTextBox" %>

<!-- TODO: Add nice autocomplete stuff -->

<asp:TextBox ID="Input" runat="server" CssClass="alignRight" /><asp:HiddenField ID="NativeCurrency" runat="server"/><asp:HiddenField ID="NativeAmount" runat="server"/>