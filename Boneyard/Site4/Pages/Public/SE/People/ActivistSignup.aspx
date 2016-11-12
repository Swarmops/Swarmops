<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="ActivistSignup.aspx.cs" Inherits="Pages_Public_SE_People_ActivistSignup" Title="PirateWeb - SE - Bli aktivist!" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <pw4:PageTitle ID="PageTitle" runat="server" Title="Bli aktivist!" Description="Bli aktivist i Piratpartiet, anonymt om du vill" Icon="user_star.png" />
    
    <div class="DivMainContent">

    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Om att vara aktivist</span><br />
    <div class="DivGroupBoxContents">
            <asp:Label runat="server" ID="Label1"><span style="line-height: 120%">Att vara aktivist i Piratpartiet innebär att du får information om vad som händer i ditt närområde, så att du kan dyka upp på aktioner och aktiviteter. Du får mail om aktiviteter och SMS när det händer något intressant nära dig. Du kan vara helt anonym (genom att använda t.ex. kontantkort och en gmail-adress).<br /><br />Du är INTE MEDLEM i partiet och finns inte med i några medlemsregister. Partiets funktionärer kan inte slå upp dina personuppgifter, de kan bara skicka meddelanden till aktivisterna i ett visst område och ser aldrig vilka de är.</span></asp:Label>
    </div>
    
    </div>
    
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Var håller du till?</span><br />
    <div class="DivGroupBoxContents">
    <asp:UpdatePanel ID="UpdatePostalCode" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Label runat="server" ID="LabelGeographyFirstPrompt">Först, vad har du för postnummer?</asp:Label>&nbsp;<asp:TextBox ID="TextPostalCode" runat="server" Columns="6" />&nbsp;<asp:Button 
            ID="ButtonSubmitPostalCode" runat="server" Text="OK" 
            onclick="ButtonSubmitPostalCode_Click" />&nbsp;&nbsp;(Bara för aktivister inom Sverige, än så länge. Norge och Finland kommer.)<br />
    <asp:Label runat="server" ID="LabelGeographySecondPrompt" /><b><asp:Label ID="LabelGeographyDeterminedLocation" runat="server" /></b><asp:DropDownList ID="DropCities" runat="server" Visible="false" ><asp:ListItem Text="-- Välj --" Value="0" Selected="True" /></asp:DropDownList>
        <asp:Button ID="ButtonSubmitCity" runat="server" Visible="false" Text="OK" 
            onclick="ButtonSubmitCity_Click" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmitPostalCode" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
    </div>
    </div>
    
    <asp:UpdatePanel ID="UpdateMainQuestions" runat="server">
    <ContentTemplate>
    <asp:Panel ID="PanelMainQuestions" runat="server" Visible="false">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Hur når vi dig?</span><br />
    <div class="DivGroupBoxContents">
    <div style="float:left">Namn<br />Email<br />Mobilnummer&nbsp;&nbsp;<br />&nbsp;</div>
    <div><asp:TextBox ID="TextName" runat="server" />&nbsp;(bara om du vill)<br /><asp:TextBox ID="TextEmail" runat="server" />
        (var noggrann) <asp:RequiredFieldValidator ControlToValidate="TextEmail" runat="server" Text="Skriv din email." ID="ValidatorEmail" /><br /><asp:TextBox ID="TextPhone" runat="server" />&nbsp;(var noggrann)&nbsp;<asp:RequiredFieldValidator ControlToValidate="TextPhone" runat="server" Text="Skriv ditt mobilnummer." ID="ValidatorPhone" /><br />
        <asp:Button ID="ButtonSubmit" runat="server" 
            Text="Klar, gör mig till aktivist!" onclick="ButtonSubmit_Click" CausesValidation="true" />&nbsp;</div>
    </div>
    </div>
    </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmitPostalCode" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmitCity" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdateFinished" runat="server">
    <ContentTemplate>
    <asp:Panel ID="PanelFinished" Visible="false" runat="server">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Välkommen som aktivist!</span><br />
    <div class="DivGroupBoxContents">
    <span style="line-height: 120%">Du är nu aktivist för Piratpartiet i <asp:Label ID="LabelGeographyResult" runat="server" Text="Kommunen skrivs här" />. <b>Välkommen!</b></span>
    </div>
    </div>
    </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
    
    
    </div>
</asp:Content>

