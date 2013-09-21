<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonDetailPopup.ascx.cs" Inherits="Swarmops.Controls.Swarm.PersonDetailPopup" %>
<%@ Register Src="~/Controls/v5/Swarm/ComboPerson.ascx" TagName="ComboPerson" TagPrefix="act5" %>

<asp:UpdatePanel ID="UpdateSetter" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<div style="width:96px;height:96px;float:right"><asp:Image ID="ImageAvatar" runat="server" Height="96" Width="96" AlternateText="Avatar" /></div>

<div style="padding:10px;overflow:hidden">
<asp:Panel ID="PanelRead" runat="server" Visible="true">
<span style="line-height:150%"><asp:Label ID="LabelPersonName" runat="server" /><br /><asp:Label ID="LabelPersonIdentity" runat="server" /></span><br /><br /><asp:LinkButton runat="server" Text="Change Person LOC..." ID="ButtonSetNew" Onclick="ButtonSetNew_Click" />
</asp:Panel>

<asp:Panel ID="PanelWrite" runat="server" Visible="false">
<span style="line-height:150%"><asp:Label ID="LabelWriteLabel" runat="server" Text="Change to:" /><br /><act5:ComboPerson ID="PersonNew" runat="server" OnSelectedPersonChanged="PersonNew_SelectedPersonChanged" /></span><br />
<div style="float:right"><asp:LinkButton ID="ButtonConfirmPerson" runat="server" Text="Confirm LOC" OnClick="ButtonConfirmPerson_Click" Visible="false" /></div>
<asp:LinkButton ID="ButtonCancel" runat="server" Text="Cancel LOC" OnClick="ButtonCancel_Click" />
</asp:Panel>
</div>

</ContentTemplate>
<Triggers>
    <asp:AsyncPostBackTrigger ControlID="ButtonSetNew" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="PersonNew" EventName="SelectedPersonChanged" />
    <asp:AsyncPostBackTrigger ControlID="ButtonCancel" EventName="Click" />
    <asp:PostBackTrigger ControlID="ButtonConfirmPerson" />
</Triggers>
</asp:UpdatePanel>
