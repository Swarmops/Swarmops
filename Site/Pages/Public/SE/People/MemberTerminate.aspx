<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeFile="MemberTerminate.aspx.cs" Inherits="Pages_Public_SE_People_MemberTerminate"
    Title="Frnya medlemskap - PirateWeb" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <pw4:PageTitle ID="PageTitle" runat="server" Title="Avsluta medlemskap" Description="Här avslutar du dina medlemskap i Piratpartiet och Ung Pirat"
        Icon="clock_delete.png" />
    <div class="DivMainContent">
        <asp:Panel ID="PanelSuccess" runat="server">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Du avslutar nu ditt medlemskap.</span><br />
                <div class="DivGroupBoxContents">
                    <p>
                        <span style="line-height: 120%">Du har <b>lämnar</b> nu medlemskapet i
                            <asp:Literal ID="LiteralRenewedOrganizations" runat="server" />. </span>
                    </p>
                    <p>
                        <span style="line-height: 120%">Tryck på knappen för att utföra.</span></p>
                    <asp:Button ID="ButtonTerminate" runat="server" Text="Avsluta" />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="PanelDone" runat="server" Visible="false">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Medlemskap Avslutat</span><br />
                <div class="DivGroupBoxContents">
                    <p>
                        <span style="line-height: 120%">Ditt medlemskap i
                            <asp:Literal ID="LiteralRenewedOrganizations2" runat="server" />
                            är nu avslutat</span></p>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="PanelFail" runat="server" Visible="false">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Ett fel uppstod</span><br />
                <div class="DivGroupBoxContents">
                    <p>
                        <span style="line-height: 120%">Tyvärr kunde vi inte tolka parametrarna. Säkerhetskoden
                            stämmer inte. </span>
                    </p>
                    <p>
                        <span style="line-height: 120%">Ett automatiskt mail har skickats till dig från Medlemsservice,
                            där du får frågan om du vill avsluta. <b>Genom att du svarar "ja" på det mailet och
                                skickar tillbaka det</b>, så kommer våra funktionärer att avsluta ditt medlemskap
                            manuellt.</span></p>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
