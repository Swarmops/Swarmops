<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectOrganizationLine.ascx.cs" Inherits="Controls_v4_SelectOrganizationLine" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>

<div class="SelectionLineFocused" id="UglyHackRemoveThis">
<table border="0" cellspacing="0" cellpadding="0">
<tr>
<td>
    <pw4:OrganizationTreeDropDown ID="DropOrganizations" OnSelectedNodeChanged="DropOrganizations_SelectedNodeChanged" runat="server" />
</td>
<td>
   <asp:UpdatePanel ID="UpdateDropDown" runat="server">
        <ContentTemplate>
            &nbsp;&nbsp;<asp:Label ID="LabelOrganizationComment" runat="server" Text="Select an organization." />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="DropOrganizations" EventName="SelectedNodeChanged" />
        </Triggers>
    </asp:UpdatePanel>
</td>
</tr>
</table>
</div>