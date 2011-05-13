<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="Impersonate.aspx.cs" Inherits="Pages_v4_Admin_Impersonate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Panel ID="Panel1" runat="server">
            <asp:Label ID="Label2" runat="server" 
    Text="Person# to impersonate : "></asp:Label>
            &nbsp;<asp:TextBox ID="TextBoxPersId" runat="server"></asp:TextBox>
&nbsp;
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                Text="Start Impersonating" />
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server">
            <asp:Button ID="Button2" runat="server" Text="Stop Impersonating" 
                onclick="Button2_Click" />
        </asp:Panel>
    </div>
</asp:Content>
