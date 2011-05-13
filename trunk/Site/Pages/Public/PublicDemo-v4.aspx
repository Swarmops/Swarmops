<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PublicDemo-v4.aspx.cs" Inherits="PirateWeb_v4_PublicDemo" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PirateWeb - Pirate Party Administration - Public Demo</title>
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <div>
        <div id="HeaderBar">
        <img src="/Images/Public/v4/PirateWeb-v4-header.png" alt="PirateWeb logo" />
    </div>
    <div id="DivMainMenu">
        <telerik:RadMenu id="MainMenu" runat="server" Skin="Web20" Font-Bold="true" BackColor="#66008C">
                <Items>
                    <telerik:RadMenuItem Text="People" AccessKey="P" ClickedImageUrl="" ExpandedImageUrl="" ImageUrl="">
                        <Items>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/vcard_add.png" Text="Add Member" AccessKey="M"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/vcards.png" Text="List Members" AccessKey="L"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/vcard_find.png" Text="Find Member" AccessKey="F"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/vcards_alert.png" Text="Invalid Members"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/vcards_delete.png" Text="Expired Members"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/group_key.png" Text="List Key People" AccessKey="O"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/bell_add.png" Text="Add Activist"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/television_add.png" Text="Add Subscriber"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/television_find.png" Text="Find Subscriber"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/user_add.png" Text="Add Person"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/group.png" Text="List People"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/user_find.png" Text="Find Person"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Communications" AccessKey="C">
                        <Items>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/email_open.png" Text="Send Geo-based Mail" AccessKey="M"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/email_open_image.png" Text="Send Newsletter"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/email_edit.png" Text="Edit Automails"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/bell.png" Text="Alert Activists" AccessKey="A"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/newspaper_add.png" Text="Send Press Release"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/overlays.png" Text="Edit Press Templates"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/camera_add.png" Text="Add Reporter"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/camera_find.png" Text="Find Reporter"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/cameras.png" Text="List Reporters"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/camera_link.png" Text="List Press Categories"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Organization" AccessKey="O">
                        <Items>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_organisation_add.png" Text="Add Organization"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_organisation.png" Text="List Organizations" AccessKey="O"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/world.png" Text="Geography Tree" AccessKey="G"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/world_edit.png" Text="List Countries"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_pie_add.png" Text="Create Internal Poll"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_pie.png" Text="View Poll Results"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/coins_add.png" Text="Create ExpenseClaim" AccessKey="E"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/coins.png" Text="List Expenses"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/money.png" Text="Pay Expenses"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/book_add.png" Text="Create Transaction Manually" AccessKey="T"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/book_go.png" Text="Import Transactions from Bank" AccessKey="T"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/book.png" Text="View Bookkeeping"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_bar.png" Text="View Budget" AccessKey="B"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_bar_edit.png" Text="Edit Budget"></telerik:RadMenuItem>
                      </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Elections" AccessKey="E">
                        <Items>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/chart_pie_add.png" Text="Register New Election" AccessKey="N"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="True"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/building.png" Text="Voting Locations" AccessKey="N"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/ballot_yellow.png" Text="National Ballots" AccessKey="N"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/ballot_blue.png" Text="Regional Ballots" AccessKey="N"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/ballot_white.png" Text="Local Ballots" AccessKey="N"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/flag_red.png" Text="Tasks Per CityName" AccessKey="W"></telerik:RadMenuItem>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/ruby.png" Text="Etc" AccessKey="P"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Administration" AccessKey="A">
                        <Items>
                            <telerik:RadMenuItem ImageUrl="~/Images/Public/Silk/ruby.png" Text="To Be Determined" AccessKey="N"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>

                </Items>
            </telerik:RadMenu>
        </div>
        <div id="DivContent" style="margin-left: 20px; margin-top: 120px; margin-right: 40px">
            <asp:Label ID="Label1" runat="server" 
                Text="This is a quick and dirty demonstration of the capabilities of the PirateWeb administration tool."></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" 
                Text="One of its key functions isn't discernible through the menu interface -- the fact that it allows for automated signup of new members and activists, and automatically assigns them to the correct geography and sub-organization, based on their postal code, and notifies the appropriate officers."></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label3" runat="server" 
                Text="Officers can be assigned responsibilities for a particular organization, or a geographical subset of an organization. They will see only the members in that part of the organization, and be able to administer that particular part."></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label4" runat="server" 
                Text="This allows the organization to grow organically: a district lead appoints circuit leads below him or her, a circuit lead appoints city leads, a city lead appoints precinct leads, etc. Not requiring centralized officer management, but pushing it as far out to the edges as possible, has been key for manageability."></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label5" runat="server" 
                Text="Shown here is the version 4 interface which is still under construction. All functions listed are present in the engine, but currently accessible from a much rougher interface. Some have been migrated to version 4."></asp:Label>
        </div>
    </div>
    </form>
</body>
</html>
