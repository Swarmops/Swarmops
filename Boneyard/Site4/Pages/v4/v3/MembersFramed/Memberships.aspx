<%@ Import Namespace="Activizr.Logic.Structure" %>

<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="Memberships.aspx.cs" Inherits="Pages_Members_Memberships" Title="PirateWeb - Member Details - Membership Listing"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/v3/PersonDetailsPagesMenu.ascx" TagName="PersonDetailsPagesMenu"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <br />
        <br />
        <asp:GridView ID="gridMemberships" runat="server" AutoGenerateColumns="False" DataSourceID="MembershipsDataSource"
            OnRowCommand="GridMemberships_RowCommand" DataKeyNames="MembershipId" 
            meta:resourcekey="gridMembershipsResource1" 
            onrowdatabound="gridMemberships_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Organization" meta:resourcekey="TemplateFieldResource1">
                    <ItemTemplate>
                        <%#Organization.FromIdentity((int) Eval("OrganizationId")).Name%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="MemberSince" DataFormatString="{0:d}" HeaderText="Member Since"
                    HtmlEncode="False" meta:resourcekey="BoundFieldResource1" />
                <asp:TemplateField HeaderText="Expires" SortExpression="Expires">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Expires", "{0:d}") %>'></asp:Label>
                        <asp:Label ID="LabelExpiredFlag" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:ButtonField CommandName="Extend" HeaderText="Extend?" ShowHeader="True" Text="Extend 1 year"
                    meta:resourcekey="ButtonFieldResource1">
                    <ItemStyle CssClass="ActionLink" />
                </asp:ButtonField>
                <asp:TemplateField HeaderText="Terminate?" ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButtonTerminate" runat="server" CausesValidation="false" 
                            CommandName="Terminate" Text="Terminate" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle CssClass="ActionLink" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        <hr />
        <br />
        <asp:Panel runat="server" ID="PanelAddMembership" DefaultButton="ButtonAddMembership"
            meta:resourcekey="PanelAddMembershipResource1">
            <asp:Label ID="labelAddMembershipHeader" runat="server" Text="labelAddMembershipHeader"
                meta:resourcekey="labelAddMembershipHeaderResource1"></asp:Label>&nbsp;<br />
            <br />
            <asp:Label ID="labelAddMembership" runat="server" Text="labelAddMembership" meta:resourcekey="labelAddMembershipResource1"></asp:Label>&nbsp;<asp:DropDownList
                ID="dropOrganizations" runat="server" meta:resourcekey="dropOrganizationsResource1">
            </asp:DropDownList>
            &nbsp;<asp:Button ID="ButtonAddMembership" runat="server" Text="buttonAddMembership"
                OnClick="ButtonAddMembership_Click" meta:resourcekey="ButtonAddMembershipResource1" /><br />
        </asp:Panel>
    </div>
    <asp:ObjectDataSource ID="MembershipsDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="SelectStatic" TypeName="Activizr.Logic.DataObjects.MembershipsDataObject"
        OnSelecting="MembershipsDataSource_Selecting">
        <SelectParameters>
            <asp:Parameter Name="memberships" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
