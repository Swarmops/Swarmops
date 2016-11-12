<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonDetailsPagesMenu.ascx.cs" Inherits="Controls_PersonDetailsPagesMenu" %>
<p>
<b><asp:Label ID="Label1" runat="server" Text="Other Data Pages"></asp:Label>:</b>
[ <asp:HyperLink ID="LinkBasicDetails" NavigateUrl="BasicDetails.aspx" runat="server">Basic Details</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkAccountSettings" NavigateUrl="MembershipSettings.aspx" runat="server">Account Settings</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkMemberships" NavigateUrl="Memberships.aspx" runat="server">Memberships</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkRolesResponsibilities" NavigateUrl="RolesResponsibilities.aspx" runat="server">Roles and Responsibilities</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkSubscriptions" NavigateUrl="SubscriptionSettings.aspx" runat="server">Subscriptions</asp:HyperLink> ]
    <br />
    <asp:Label ID="LabelSelectedMember" runat="server" Text=""></asp:Label>
</p>
