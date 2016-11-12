<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="Pages_Members_List" Title="PirateWeb - List Members" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/v3/PersonList.ascx" TagName="PersonList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1 class="ContentHeader">
            <asp:Label ID="LabelHeader" runat="server" Text="List Members" meta:resourcekey="LabelHeaderResource1"></asp:Label>
        </h1>
        <table style="width: 100%;">
            <tr>
                <td>
                    <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged" meta:resourcekey="DropOrganizationsResource1">
                    </asp:DropDownList>
                    <br />
                    <asp:DropDownList ID="DropGeographies" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropGeographies_SelectedIndexChanged" meta:resourcekey="DropGeographiesResource1">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="ButtonList" runat="server" OnClick="ButtonList_Click" Text="List" meta:resourcekey="ButtonListResource1" />
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Button ID="ButtonDupeList" runat="server" OnClick="ButtonDupeList_Click" Text="Find Duplicates (by email, phone, name)" meta:resourcekey="ButtonDupeListResource1" />
                </td>
                <td>
                        <asp:Label ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Duplicate and expiry finder limited to max 2500 people at a time because of performance."></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Button ID="ButtonExpiringList" runat="server" OnClick="ButtonExpiringList_Click" Text="Find expiring memberships" meta:resourcekey="ButtonExpiringListResource1" />
                    &nbsp;<span class="normalText">
                        <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" Text="within "></asp:Literal><asp:TextBox ID="ExpiryDays" runat="server"></asp:TextBox>
                        <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" Text="days"></asp:Literal></span>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
        <hr />
        <h2 class="ContentHeader">
            <asp:Label ID="LabelListHeader" runat="server" meta:resourcekey="LabelListHeaderResource1"></asp:Label>
            <asp:Label ID="LabelDupeHeader" runat="server" Visible="False" Text="Possible Duplicates" meta:resourcekey="LabelDupeHeaderResource1"></asp:Label>
            <asp:Label ID="LabelExpiryHeader" runat="server" Visible="False" Text="Expiring members" meta:resourcekey="LabelExpiryHeaderResource1"></asp:Label></h2>
        <uc1:PersonList ID="PersonList" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .normalText
        {
            font-size: small;
        }
        #Text1
        {
            width: 41px;
        }
        #ExpiryDays
        {
            width: 55px;
        }
    </style>
</asp:Content>
