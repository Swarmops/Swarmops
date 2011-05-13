<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="MailQ.aspx.cs" Inherits="Pages_v4_admin_MailQ" %>
<%@ Import Namespace="Activizr.Logic.Support" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
<div class="bodyContent">
        Server Time:
        <%
            Response.Write(DateTime.Now.ToString());
        %><br />
        Last HeartBeat:
        <%
        try 
        {
            Response.Write(Persistence.Key["PirateBot-L-Heartbeat"]);
        }
        catch
        {}
        %><br />
        Current mail queue (top 500):<asp:Table Style="width: 100%;" runat="server" ID="tab"
            CellSpacing="0" BorderStyle="Solid" BorderWidth="1px" GridLines="Both">
        </asp:Table>
        <asp:Button ID="Button1" runat="server" Text="Refresh" OnClick="Button1_Click" />
</div>
</asp:Content>

