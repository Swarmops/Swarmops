<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LanguageSelector.ascx.cs"
    Inherits="Controls_LanguageSelector" %>
<table id="Table1" runat="server" cellspacing="1" cellpadding="0" 
    class="LanguageSelector" style="float: right;" >
    <tr>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageEnglish" OnClick="ButtonLanguageEnglish_Click"
                ToolTip="English" ImageUrl="~/Images/Public/Flags/uk.png" runat="server">
                <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageSwedish" OnClick="ButtonLanguageSwedish_Click"
                ToolTip="Svenska" ImageUrl="~/Images/Public/Flags/se.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
          </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageFrench" OnClick="ButtonLanguageFrench_Click"
                ToolTip="Francais" ImageUrl="~/Images/Public/Flags/fr.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageGerman" OnClick="ButtonLanguageGerman_Click"
                ToolTip="Deutsch" ImageUrl="~/Images/Public/Flags/de.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageSpanish" OnClick="ButtonLanguageSpanish_Click"
                ToolTip="Espanol" ImageUrl="~/Images/Public/Flags/es.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageRussian" OnClick="ButtonLanguageRussian_Click"
                ToolTip="Russki" ImageUrl="~/Images/Public/Flags/ru.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageJapanese" OnClick="ButtonLanguageJapanese_Click"
                ToolTip="Japanese" ImageUrl="~/Images/Public/Flags/jp.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
        <td>
            <asp:ImageMap HotSpotMode="PostBack" ID="ButtonLanguageFinnish" OnClick="ButtonLanguageFinnish_Click"
                ToolTip="Finnish" ImageUrl="~/Images/Public/Flags/fi.png" runat="server">
                  <asp:RectangleHotSpot Bottom="200" Right="200" />
            </asp:ImageMap>
        </td>
    </tr>
</table>
