<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Pages_v5_Ledgers_CloseLedgers" Codebehind="CloseLedgers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

    <asp:Panel ID="PanelCannotClose" Visible="false" runat="server">
        <h2><asp:Label ID="LabelCannotCloseHeader" Text="Cannot Close Ledgers LOC" runat="server" /></h2>
        <asp:Panel ID="PanelErrorImage" Visible="false" runat="server">
            <asp:Image ID="Image1" ImageUrl="~/Images/Icons/iconshock-cross-96px.png" ImageAlign="Left" runat="server" />
        </asp:Panel>
        <asp:Label ID="LabelCannotCloseLedgersReason" Text="Cannot close ledgers: There are unbalanced transactions." runat="server" />
    </asp:Panel>

    <asp:Panel runat="server" ID="PanelSuccess">
        <p>Ledgers have been closed.</p>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

