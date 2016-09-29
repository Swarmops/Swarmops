<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Security_DatabaseUpgradeRequired" Codebehind="DatabaseUpgradeRequired.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <asp:Image runat="server" ImageAlign="Left" ID="DiscoImage" ImageUrl="~/Images/Icons/iconshock-disconnect-96px.png" /><h3><asp:Label runat="server" ID="LabelDbUpgradeRequiredHeader" Text="[LOC]" /></h3><asp:Literal ID="LiteralDbUpgradeRequiredRant" Text="You have arrived at this page because a database upgrade is pending. [LOC]" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

