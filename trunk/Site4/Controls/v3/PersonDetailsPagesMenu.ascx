<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonDetailsPagesMenu.ascx.cs" Inherits="Controls_v3_PersonDetailsPagesMenu" %>
<p>
<b><asp:Label ID="Label1" runat="server" Text="Other Data Pages"></asp:Label>:</b>
[ <asp:HyperLink ID="LinkBasicDetails" NavigateUrl="~/Pages/v3/Members/BasicDetails.aspx" runat="server">Basic Details</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkAccountSettings" NavigateUrl="~/Pages/v3/Members/MembershipSettings.aspx" runat="server">Account Settings</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkMemberships" NavigateUrl="~/Pages/v3/Members/Memberships.aspx" runat="server">Memberships</asp:HyperLink> ]
[ <asp:HyperLink ID="LinkRolesResponsibilities" NavigateUrl="~/Pages/v3/Members/RolesResponsibilities.aspx" runat="server">Roles and Responsibilities</asp:HyperLink> ]
</p>
