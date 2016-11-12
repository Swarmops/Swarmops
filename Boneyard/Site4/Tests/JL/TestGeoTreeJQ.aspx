<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="TestGeoTreeJQ.aspx.cs"
    Inherits="Tests_JL_TestGeoTreeJQ" %>

<%@ Register Src="~/jQuery/ServerControls/TreeDropdown.ascx" TagName="TreeDropdown"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <uc1:TreeDropdown ID="GeographyDropdown1" runat="server" />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
    </div>
  
</asp:Content>
