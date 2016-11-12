<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="SmsInboxSweden.aspx.cs" Inherits="Pages_v4_Communications_SmsInboxSweden"
    Title="Manage SMS Inbox (Sweden) - PirateWeb" %>

<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <pw4:PageTitle Icon="mobile-phone.png" Title="SMS Inbox (Sweden)" Description="List and react to inbound SMS messages (Sweden)"
        runat="server" ID="PageTitle" />
    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">
                <asp:Label ID="LabelTransactionsTitle" runat="server" Text="Current SMS Inbox" /></span><br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridInbox" runat="server" AutoGenerateColumns="False" GridLines="None"
                    Skin="Web20" OnItemCreated="GridInbox_ItemCreated">
                    <HeaderContextMenu>
                        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                    </HeaderContextMenu>
                    <MasterTableView ShowFooter="true" DataKeyNames="CaseId">
                        <Columns>
                            <telerik:GridBoundColumn HeaderText="Case#" DataField="CaseId" />
                            <telerik:GridBoundColumn HeaderText="Phone#" DataField="PhoneNumber" />
                            <telerik:GridBoundColumn HeaderText="Matches person" DataField="PersonCanonical" />
                            <telerik:GridBoundColumn HeaderText="Message" DataField="Message" />
                            <telerik:GridTemplateColumn HeaderText="Action">
                                <ItemTemplate>
                                    <asp:DropDownList ID="DropActions" runat="server">
                                        <asp:ListItem Value="NoAction" Text="No action" Selected="True" />
                                        <asp:ListItem Value="RenewAll" Text="Renew memberships" />
                                        <asp:ListItem Value="TerminateAll" Text="Terminate all relations" />
                                    </asp:DropDownList>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="ButtonPerformActions" runat="server" OnClick="ButtonPerformActions_Click"
                                        Text="Execute actions" />
                                </FooterTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="false" />
                    </ClientSettings>
                    <FilterMenu>
                        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                    </FilterMenu>
                </telerik:RadGrid>
                <!--
                <telerik:GridTemplateColumn UniqueName="ManageColumn">
                    <ItemTemplate>
                        <asp:HyperLink ID="ManageLink" runat="server" Text="Edit reporter..."></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>-->
            </div>
        </div>
    </div>
</asp:Content>
