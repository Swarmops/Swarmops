<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="Donate.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Financial.Donate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
    <p><asp:Label runat="server" ID="LabelExplainBitcoinDonation" /></p>
    <div align="center"><asp:Image ID="ImageBitcoinQr" runat="server"/></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

