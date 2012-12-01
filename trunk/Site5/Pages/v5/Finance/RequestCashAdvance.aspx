<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="RequestCashAdvance.aspx.cs" Inherits="Pages_v5_Finance_RequestCashAdvance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryLabels">
        Amount (SEK)<br/>
        Purpose<br/>
        Budget<br/>
    </div>
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextAmount" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="TextPurpose" />&nbsp;<br/>
        <asp:TextBox runat="server" ID="Budget" />&nbsp;<br/>
        <asp:Button runat="server" CssClass="buttonAccentColor" Text="Request"/>
    </div>
    <div class="entryvalidation">
        For example, <strong>1000</strong>.<br/>
        Describe briefly. For example, "Bus tickets to rally."<br/>
        Where this money will come from.<br/>
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

