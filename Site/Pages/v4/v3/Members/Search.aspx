<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="Search.aspx.cs" Inherits="Pages_Members_Search" Title="PirateWeb - Search for members"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/v3/PersonList.ascx" TagName="PersonList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1 class="ContentHeader">
            <asp:Label ID="labelSearchHeader" runat="server" Text="Sök medlemmar" meta:resourcekey="labelSearchHeaderResource1"></asp:Label></h1>
        <asp:Panel runat="server" DefaultButton="ButtonSearch" meta:resourcekey="PanelResource1">
            <table border="0" cellpadding="2">
                <tr>
                    <td align="right">
                        <asp:Label ID="labelNamePattern" runat="server" Text="Namn:" meta:resourcekey="labelNamePatternResource1"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextNamePattern" runat="server" meta:resourcekey="TextNamePatternResource1"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="LabelBirthDate" runat="server" Text="Födelsedatum:" meta:resourcekey="LabelBirthDateResource1"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="textPersonalNumber" runat="server" meta:resourcekey="textPersonalNumberResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="labelEmailPattern" runat="server" Text="e-post:" meta:resourcekey="labelEmailPatternResource1"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextEmailPattern" runat="server" meta:resourcekey="TextEmailPatternResource1"></asp:TextBox>
                    </td>
                    <td align="right">
                        &nbsp;
                        <asp:Label ID="LabelCity0" runat="server" Text="PostNummer:" 
                            meta:resourcekey="LabelCity0Resource1"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="TextPostalCodePattern" runat="server" 
                            meta:resourcekey="TextPostalCodePatternResource1" ></asp:TextBox>
                    </td>
                    
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="labelMemberNumber" runat="server" Text="Medlems #:" meta:resourcekey="labelMemberNumberResource1"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextMemberNumber" runat="server" meta:resourcekey="TextMemberNumberResource1"></asp:TextBox>
                    </td>
                    <td align="right">
                        <asp:Label ID="LabelCity" runat="server" Text="Ort:" meta:resourcekey="LabelCityResource1"></asp:Label>
                        &nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextCityPattern" runat="server" meta:resourcekey="textCityResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="ButtonSearch" runat="server" Text="Search" OnClick="ButtonSearch_Click"
                            meta:resourcekey="ButtonSearchResource1" />
                    </td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;<asp:CheckBox ID="CheckBoxActivist" runat="server" 
                            meta:resourcekey="CheckBox1Resource1" Text="Aktivist" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <hr />
        &nbsp;<uc1:PersonList ID="PersonList" runat="server" />
        <br />
        <asp:Literal ID="labelSearchHints" runat="server" Text="labelSearchHints" meta:resourcekey="labelSearchHintsResource1"></asp:Literal>
    </div>
</asp:Content>
