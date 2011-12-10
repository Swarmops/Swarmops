<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LanguageSelector.ascx.cs" Inherits="Activizr.Controls.Base.LanguageSelector" %>

<div style="margin:10px; margin-top:6px">

    <div style="border-bottom: solid 1px #C78B15; color: #FFBC37; text-align: left; padding-bottom: 4px; margin-bottom:8px; letter-spacing: 1px; font-size: 14px; font-weight: bold">

        <asp:Label ID="LabelSelectLanguage" runat="server" />

    </div>
        <div style="float:left;width:74px; border-right: solid 1px #FFBC37; padding-right: 5px; margin-right: 5px">
            <asp:Image CssClass="ImageFlag" runat="server" ID="ImageDK" ImageUrl="~/Images/Flags/dk.png" /><asp:LinkButton ID="LinkDanish" runat="server" Text="Dansk" OnClick="LinkDanish_Click" /><br />
            <asp:Image CssClass="ImageFlag" runat="server" ID="ImageDE" ImageUrl="~/Images/Flags/de.png" /><asp:LinkButton ID="LinkGerman" runat="server" Text="Deutsch" OnClick="LinkGerman_Click" /><br />
            <asp:Image CssClass="ImageFlag" runat="server" ID="ImageUK" ImageUrl="~/Images/Flags/uk.png" /><asp:LinkButton ID="LinkEnglish" runat="server" Text="English" OnClick="LinkEnglish_Click" /><br />
            <asp:Image CssClass="ImageFlag" runat="server" ID="ImageES" ImageUrl="~/Images/Flags/es.png" /><asp:LinkButton ID="LinkSpanish" runat="server" Text="Español" OnClick="LinkSpanish_Click" /><br />
            <asp:Image CssClass="ImageFlag" runat="server" ID="ImageFR" ImageUrl="~/Images/Flags/fr.png" /><asp:LinkButton ID="LinkFrench" runat="server" Text="Français" OnClick="LinkFrench_Click" /><br />
            <asp:Image CssClass="ImageFlag" runat="server" ID="ImageIT" ImageUrl="~/Images/Flags/it.png" /><asp:LinkButton ID="LinkItalian" runat="server" Text="Italiano" OnClick="LinkItalian_Click" /><br />
        </div>
        
        <asp:Image CssClass="ImageFlag" runat="server" ID="ImageNL" ImageUrl="~/Images/Flags/nl.png" /><asp:LinkButton ID="LinkDutch" runat="server" Text="Nederl." OnClick="LinkDutch_Click" /><br />
        <asp:Image CssClass="ImageFlag" runat="server" ID="ImagePL" ImageUrl="~/Images/Flags/pl.png" /><asp:LinkButton ID="LinkPolish" runat="server" Text="Polish" OnClick="LinkPolish_Click" /><br />
        <asp:Image CssClass="ImageFlag" runat="server" ID="ImagePT" ImageUrl="~/Images/Flags/pt.png" /><asp:LinkButton ID="LinkPortuguese" runat="server" Text="Portugués" OnClick="LinkPortuguese_Click" /><br />
        <asp:Image CssClass="ImageFlag" runat="server" ID="ImageRU" ImageUrl="~/Images/Flags/ru.png" /><asp:LinkButton ID="LinkRussian" runat="server" Text="Россию" OnClick="LinkRussian_Click" /><br />
        <asp:Image CssClass="ImageFlag" runat="server" ID="ImageFI" ImageUrl="~/Images/Flags/fi.png" /><asp:LinkButton ID="LinkFinnish" runat="server" Text="Suomi" OnClick="LinkFinnish_Click" /><br />
        <asp:Image CssClass="ImageFlag" runat="server" ID="ImageSE" ImageUrl="~/Images/Flags/se.png" /><asp:LinkButton ID="LinkSwedish" runat="server" Text="Svenska" OnClick="LinkSwedish_Click" /><br />

</div>