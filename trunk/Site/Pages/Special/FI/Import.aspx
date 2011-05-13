<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" EnableEventValidation="false"
    CodeFile="Import.aspx.cs" Inherits="Pages_Special_FI_Import" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="DivMainContent">
        <asp:Label ID="Label1" runat="server"> This page imports a tab delimited file of people into the database. Format is:</asp:Label>
        <table style="width: 100%;border:1px solid silver;">
            <tr>
                <td>
                    first names
                </td>
                <td>
                    last name
                </td>
                <td>
                    date of birth
                </td>
                <td>
                    email
                </td>
                <td>
                    city (municipality)
                </td>
                <td>
                    address
                </td>
                <td>
                    postal code
                </td>
                <td>
                    city (postal)
                </td>
                <td>
                    phone number
                </td>
                <td>
                    date joined
                </td>
                <td>
                    active
                </td>
            </tr>
        </table>
        <asp:Label ID="Label2" runat="server" 
            
            Text="">Enter the data in the field order as above, do not enter the column headings. <br />email combined with birthdate is regarded as primary key</asp:Label>
        <br />
        <br />
        <table style="width: 100%;">
            <tr>
                <td>
                    Import to org:
                    <asp:DropDownList ID="DropDownListOrgs" runat="server">
                    </asp:DropDownList>
                </td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="Label3" runat="server" 
            Text="paste tab delimited content into textbox below."></asp:Label>
                </td>
                <td>
        <asp:Label ID="Label4" runat="server" 
            Text="result &amp; errors"></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
        <asp:TextBox ID="TextBoxImport" runat="server" Rows="20" TextMode="MultiLine" 
            Width="519px"></asp:TextBox>
                </td>
                <td>
        <asp:TextBox ID="TextBoxResult" runat="server" Rows="20" TextMode="MultiLine" 
            Width="519px" ReadOnly="True"></asp:TextBox>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            </table>
        <br />
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="Import" onclick="Button1_Click" />
    </div>
</asp:Content>
