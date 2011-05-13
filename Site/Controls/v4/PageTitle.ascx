<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PageTitle.ascx.cs" Inherits="Controls_v4_PageTitle" %>

<div id="DivPageTitle" style="width:100%">
<table border="0" cellpadding="0" cellspacing="0">
<tr>
<td>&nbsp;&nbsp;&nbsp;</td>
<td id="CellPageTitleIcon"><asp:Image AlternateText="Icon for this page" ID="IconPage" runat="server" ImageAlign="Baseline" CssClass="PageTitleIcon" /></td>
<td id="CellPageTitle"><asp:Label ID="LabelPageTitle" runat="server"/></td>
<td id="CellPageTitleDescription"><asp:Label ID="LabelPageDescription" runat="server" /></td>
</tr>
</table>
</div>