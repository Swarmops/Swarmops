<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Subscriptions.ascx.cs"
    Inherits="Subscriptions_UserControl" %>
<div>
    <asp:Label ID="LabelIntroduction" runat="server" Text="Check the newsletters you want to receive and press Save."
        meta:resourcekey="LabelIntroductionResource1"></asp:Label><br />
    <br />
    <br />
    <asp:CheckBoxList ID="listSubscriptions" runat="server" meta:resourcekey="listSubscriptionsResource1">
    </asp:CheckBoxList>
    <br />
    <asp:Button ID="ButtonSave" runat="server" Text="Save" OnClick="ButtonSave_Click"
        meta:resourcekey="ButtonSaveResource1" />
</div>
