<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ValidateCanSee.aspx.cs" Inherits="Pages_v4_admin_validatecansee" %>
<%@ Import Namespace="Activizr.Logic.Support" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
<div class="bodyContent">
 <br />
    Can user:<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox> see user:<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Test" OnClick="Button1_Click" /><br />
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
</div>
</asp:Content>

