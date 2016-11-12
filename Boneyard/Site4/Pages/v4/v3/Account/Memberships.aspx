<%@ Import Namespace="Activizr.Logic.Structure" %>

<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="Memberships.aspx.cs" Inherits="Pages_Account_Memberships" Title="PirateWeb - Member Details - Membership Listing"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Your Memberships" Description="Renew or terminate your memberships"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <br />
        <br />
        <asp:GridView ID="gridMemberships" runat="server" AutoGenerateColumns="False" OnRowCommand="GridMemberships_RowCommand"
            DataKeyNames="MembershipId" meta:resourcekey="gridMembershipsResource1" OnRowDataBound="gridMemberships_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Organization" meta:resourcekey="TemplateFieldResource1">
                    <ItemTemplate>
                        <%#Organization.FromIdentity((int) Eval("OrganizationId")).Name%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="MemberSince" DataFormatString="{0:d}" HeaderText="Member Since"
                    HtmlEncode="False" meta:resourcekey="BoundFieldResource1" />
                <asp:TemplateField HeaderText="Expires" SortExpression="Expires" meta:resourcekey="TemplateFieldResource2">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Expires", "{0:d}") %>' meta:resourcekey="Label1Resource1"></asp:Label>
                        <asp:Label ID="LabelExpiredFlag" runat="server" meta:resourcekey="LabelExpiredFlagResource1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Extend?" 
                    meta:resourcekey="TemplateFieldResource4">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButtonExtend" runat="server" CausesValidation="False" CommandArgument='<%# Container.DataItemIndex %>'
                            CommandName="Extend" Text="Extend 1 year" 
                            meta:resourcekey="LinkButtonExtendResource1"></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="ActionLink" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Terminate?" ShowHeader="False" meta:resourcekey="TemplateFieldResource3">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButtonTerminate" runat="server" CausesValidation="False"
                            CommandName="Terminate" Text="Terminate" CommandArgument='<%# Container.DataItemIndex %>'
                            OnClientClick='return (confirm("Sure you want to terminate the membership?"));'
                            meta:resourcekey="LinkButtonTerminateResource1"></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="ActionLink" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" 
            Text="If you have any questions about your memberships, please contact your closest local officer or medlemsservice@piratpartiet.se" 
            meta:resourcekey="Label2Resource1"></asp:Label>
    </div>
</asp:Content>
