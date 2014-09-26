<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccessDenied.aspx.cs" Inherits="Security_AccessDenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <asp:Image runat="server" ImageAlign="Left" ID="DiscoImage" ImageUrl="~/Images/Icons/iconshock-disconnect-96px.png" /><h3><asp:Label runat="server" ID="LabelAccessDeniedHeader" Text="[LOC]" /></h3><asp:Literal ID="LiteralAccessDeniedRant" Text="You have arrived at this page because you do not have the required clearance to access a page you selected. [LOC]" runat="server" />

    <div style="clear:both"></div>
    <br />Todo: Assist in changing organizations.<br/><br/>
    Todo: List actual security clearances.

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

