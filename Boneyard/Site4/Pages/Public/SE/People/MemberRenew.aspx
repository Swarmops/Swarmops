<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeFile="MemberRenew.aspx.cs" Inherits="Pages_Public_SE_People_MemberRenew"
    Title="Frnya medlemskap - PirateWeb" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <pw4:PageTitle ID="PageTitle" runat="server" Title="Förnyelse av medlemskap" Description="Här förnyar du dina medlemskap i Piratpartiet och Ung Pirat"
        Icon="clock_go.png" />
    <div class="DivMainContent">
        <asp:Panel ID="PanelSuccess" runat="server">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Tack för att du förnyar
                    <%= DateTime.Now.Year.ToString() %>!</span><br />
                <div class="DivGroupBoxContents">
                    <p>
                        <span style="line-height: 120%">Du har <b>förnyat</b> medlemskapet i
                            <asp:Literal ID="LiteralRenewedOrganizations" runat="server" />. Det går nu ut <b>
                                <asp:Label ID="LabelExpires" runat="server" /></b>.</span></p>
                    <asp:Panel ID="PanelLeft" runat="server" Visible="false">
                        <p>
                            Du har <b>lämnat</b> medlemskapet i
                            <asp:Literal ID="LiteralLeftOrganizations" runat="server" />.</p>
                    </asp:Panel>
                    <asp:Panel ID="PanelTransferred" runat="server" Visible="false">
                        <p>
                            Du har <b>flyttat</b> medlemskapet i <b>
                                <asp:Label ID="LabelTransferOldOrganization" runat="server" /></b> till <b>
                                    <asp:Label ID="LabelTransferNewOrganization" runat="server" /></b>.</p>
                    </asp:Panel>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="PanelFail" runat="server">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Ett fel uppstod</span><br />
                <div class="DivGroupBoxContents">
                    <p>
                        <span style="line-height: 120%">Tyvärr kunde vi inte tolka parametrarna. Säkerhetskoden
                            stämmer inte. Det kan vara så att du redan har förnyat för i år, men operatörerna
                            av PirateWeb har ändå kontaktats.</span></p>
                    <p>
                        <span style="line-height: 120%">Samtidigt ska det här inte hindra dig från att fortsätta
                            vara medlem. Ett automatiskt mail har också skickats till dig från Medlemsservice,
                            där du får frågan om du vill färnya. <b>Genom att du svarar "ja" på det mailet och skickar
                                tillbaka det</b>, så kommer våra funktionärer att förnya ditt medlemskap manuellt.</span></p>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
