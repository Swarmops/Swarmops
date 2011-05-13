<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" Debug="true"
    CodeFile="SokAdress.aspx.cs" Inherits="Pages_Special_SE_SokAdress" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        Klistra in lista (adress &lt;tab&gt; postnr [ort] )<br />
        (Resultaten är filtrerade enligt dina rättigheter i PW)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server"
            Text="Hämta" onclick="Button1_Click" /><br />
        <asp:TextBox ID="TextBox1" runat="server" Rows="40" TextMode="MultiLine" Height="413px"
            Width="590px"></asp:TextBox>
        <table id="outputTab" runat="server">
        </table>
    </div>
</asp:Content>
