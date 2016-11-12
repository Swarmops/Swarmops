<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ImportTaxLevels.aspx.cs" Inherits="Pages_v4_Admin_ImportTaxLevels" Title="Import Tax Levels - PirateWeb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Import Parameters</span><br />
            <div class="DivGroupBoxContents">
                <div style="float: left">Country&nbsp;&nbsp;</div>
                <asp:DropDownList ID="DropCountries" runat="server"><asp:ListItem Text="Sweden SE" Value="SE" Selected="True" /></asp:DropDownList>&nbsp;
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Paste Tax Data (format: Sweden)</span><br />
            <asp:TextBox ID="TextTaxData" runat="server" Rows="15" TextMode="MultiLine" />
            <div style="float: right">
                <asp:Button ID="ButtonProcessData" runat="server" Text="Import" 
                    onclick="ButtonProcessData_Click" /></div>
            <p><asp:Label ID="LabelImportResultText" runat="server" Text="Paste tax data and import."/></p>

        </div>
    </div>


</asp:Content>

