<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccessDenied.aspx.cs" Inherits="Security_AccessDenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <asp:Image runat="server" ImageAlign="Left" ID="DiscoImage" ImageUrl="~/Images/Icons/iconshock-disconnect-96px.png" /><h3>Access Denied [LOC]</h3><asp:Literal ID="LiteralAccessDeniedRant" Text="You have arrived at this page because you do not have the required clearance to access a page you selected. [LOC]" runat="server" />

    <div style="clear:both"></div>
    <br />Todo: Assist in changing organizations.

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" Text="[LOC]" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
        <asp:Label ID="LabelAccessDeniedInfo" runat="server" Text="[LOC]" />
        </div>
    </div>
</asp:Content>

