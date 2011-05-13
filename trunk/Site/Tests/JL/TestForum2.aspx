<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="TestForum2.aspx.cs" Inherits="Tests_JL_TestForum2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div class="DivMainContent">
    <asp:TextBox ID="TextBox1" runat="server" Height="166px" TextMode="MultiLine" Width="893px"></asp:TextBox>
    <br />
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="Set" onclick="Button1_Click" />
    <asp:Button ID="Button2" runat="server" Text="unSet" onclick="Button2_Click" />
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    <br />
    <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></div>
</asp:Content>

