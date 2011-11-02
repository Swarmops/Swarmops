<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="Exceptions.aspx.cs" Inherits="Pages_v4_admin_Exceptions" %>
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
        Exceptions from monobot (last 500)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :<asp:Button 
            ID="Button2" runat="server" Text="Refresh" OnClick="Button1_Click" />
        <asp:Table Style="width: 100%;" runat="server" ID="tab"
            CellSpacing="0" BorderStyle="Solid" BorderWidth="1px" GridLines="Both" 
            EnableViewState="False">
        </asp:Table>
        <asp:Button ID="Button1" runat="server" Text="Refresh" OnClick="Button1_Click" />
</div>
</asp:Content>

