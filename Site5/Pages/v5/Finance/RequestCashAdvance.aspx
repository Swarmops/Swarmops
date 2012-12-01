<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="RequestCashAdvance.aspx.cs" Inherits="Pages_v5_Finance_RequestCashAdvance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextAmount" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextPurpose" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="Budget" />&nbsp;<br/>
        <h3>&nbsp;</h3><!-- placeholder -->
        <asp:TextBox runat="server" ID="TextBank" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextClearing" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextAccount" />&nbsp;<br/>
        <asp:Button ID="Button1" runat="server" CssClass="buttonAccentColor" Text="Request"/>
    </div>
    <div class="entryLabels">
        Amount (SEK)<br/>
        Purpose<br/>
        Budget<br/>
        <h3>Your bank details</h3>
        Bank<br/>
        Clearing#<br/>
        Account#
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

