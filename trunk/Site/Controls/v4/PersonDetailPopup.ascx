<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonDetailPopup.ascx.cs" Inherits="Controls_v4_PersonDetailPopup" %>
<%@ Register Src="~/Controls/v4/ComboPerson.ascx" TagName="ComboPerson" TagPrefix="pw4" %>

<asp:UpdatePanel ID="UpdateSetter" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<div style="width:96px;height:96px;float:right"><asp:Image ID="ImageAvatar" runat="server" Height="96" Width="96" AlternateText="Avatar" /></div>

<div style="padding:10px;overflow:hidden">
<asp:Panel ID="PanelRead" runat="server" Visible="true">
<span style="line-height:150%"><asp:Label ID="LabelPersonName" runat="server" /><br /><asp:Label ID="LabelPersonIdentity" runat="server" /></span><br /><br /><asp:LinkButton runat="server" Text="Set new owner..." ID="ButtonSetNewOwner" Onclick="ButtonSetNewOwner_Click" />
</asp:Panel>

<asp:Panel ID="PanelWrite" runat="server" Visible="false">
<span style="line-height:150%"><asp:Label ID="LabelWriteLabel" runat="server" Text="Set new owner:" /><br /><pw4:ComboPerson ID="PersonNewOwner" runat="server" OnSelectedPersonChanged="PersonNewOwner_SelectedPersonChanged" /></span><br />
<div style="float:right"><asp:LinkButton ID="ButtonConfirmPerson" runat="server" Text="Confirm" OnClick="ButtonConfirmPerson_Click" Visible="false" /></div>
<asp:LinkButton ID="ButtonCancel" runat="server" Text="Cancel" OnClick="ButtonCancel_Click" />
</asp:Panel>
</div>

</ContentTemplate>
<Triggers>
    <asp:AsyncPostBackTrigger ControlID="ButtonSetNewOwner" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="PersonNewOwner" EventName="SelectedPersonChanged" />
    <asp:AsyncPostBackTrigger ControlID="ButtonCancel" EventName="Click" />
    <asp:PostBackTrigger ControlID="ButtonConfirmPerson" />
</Triggers>
</asp:UpdatePanel>
