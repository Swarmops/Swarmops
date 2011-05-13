<%@ Page Language="C#" EnableEventValidation="false" MasterPageFile="~/PirateWeb-v4-menuless.master" AutoEventWireup="true" CodeFile="OfficerSignup.aspx.cs" Inherits="Pages_Public_SE_People_OfficerSignup" Title="PirateWeb - SE - Funktionärsintresse" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="BodyContent" Runat="Server">
    <pw4:PageTitle ID="PageTitle" runat="server" Title="Bli nyckelperson!" Description="Ta ansvar i Piratpartiet för att organisationen fungerar där du bor" Icon="user_key.png" />
    
    <div class="DivMainContent">

    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Om våra funktionärsroller</span><br />
    <div class="DivGroupBoxContents" style="line-height:120%">
    <table border="0">
    <tr>
        <td valign="top"><b>Kommunledare</b></td>
        <td><span style="line-height:120%">En <b>kommunledare</b> (KL) är en person som tar ansvar för medlemmar och aktivister i sin kommun, och ser till att organisationen fungerar där -- att det delas ut valmaterial som en del av kampanjer, att valsedelsdistributionen fungerar, att de lokala piraterna har en kontaktperson, och som helt enkelt tar ansvar för att det som behöver göras, blir gjort. Det betyder inte att man måste göra allting själv. Det betyder framför allt att man är en länk mellan piraterna i den kommunen och resten av organisationen. Kommunledaren är också en extremt lokal kontaktperson för den lokala pressen.</span><br /></td>
    </tr>
    <tr>
        <td valign="top"><b>Vice&nbsp;kommunledare</b>&nbsp;&nbsp;</td>
        <td><span style="line-height:120%">En <b>vice kommunledare</b> är en person som hjälper och avlastar kommunledaren. Idealiskt finns det minst en viceledare för varje ledare. Det bör inte finnas fler än tre, men vi har ingen formell begränsning.</span><br /></td>
    </tr>
    <tr>
        <td valign="top"><b>Valkretsledare</b></td>
        <td><span style="line-height:120%">En <b>valkretsledare</b> (VL) är snäppet ovanför en kommunledare i organisationen -- en person som samordnar kommunerna i en <b>valkrets</b>, ett område som oftast sammanfaller med länen (utom i Skånes, Stockholms och Västra Götalands län, där det finns flera valkretsar). Valkretsledaren är länken mellan kommunerna och <b>distriktsledaren</b> som sitter i den operativa partiledningen. Valkretsledaren tar dessutom ansvar för medlemmar och aktivister i sitt område, försöker hitta kommunledare där det inte ännu finns, och är också en kontaktperson för lokal media. Vi har valkretsledare <b>nästan</b> överallt idag, men vill ändå veta om du är intresserad av ansvaret.</span><br /></td>
    </tr>
    <tr>
        <td valign="top"><b>Vice&nbsp;valkretsledare</b>&nbsp;&nbsp;</td>
        <td><span style="line-height:120%">En <b>vice valkretsledare</b> är en person som hjälper och avlastar valkretsledaren. Idealiskt finns det, precis som på kommunnivå och andra nivåer, minst en viceledare för varje ledare.</span><br /></td>
    </tr>
    </table>
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Vem är du, och vilket ansvar vill du ha?</span><br />
    <div class="DivGroupBoxContents">
    <asp:UpdatePanel ID="UpdateMainQuestions" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div style="float:left">Var bor du?<br />Namn?<br />Telefon?<br />Är&nbsp;du&nbsp;medlem&nbsp;idag?&nbsp;&nbsp;<br />KL<br />Vice KL<br />VL<br />Vice VL<br />&nbsp;</div>
        <asp:DropDownList ID="DropGeographies" runat="server" />&nbsp;<asp:CompareValidator ID="ValidatorGeographies" runat="server" ControlToValidate="DropGeographies" ValueToCompare="1" Text="Välj kommun." Operator="GreaterThanEqual" />&nbsp;<br />
        <asp:TextBox ID="TextName" runat="server" />&nbsp;<asp:RequiredFieldValidator ControlToValidate="TextName" ID="ValidatorName" runat="server" Text="Skriv ditt namn." /><br />
        <asp:TextBox ID="TextPhone" runat="server" />&nbsp;<asp:RequiredFieldValidator ControlToValidate="TextPhone" ID="ValidatorPhone" runat="server" Text="Skriv ditt telefonnummer." /><br /><asp:DropDownList ID="DropMember" runat="server"><asp:ListItem Selected="True">-- Välj --</asp:ListItem><asp:ListItem Value="Yes">Ja, är medlem redan</asp:ListItem><asp:ListItem Value="No">Nej, men vill bli</asp:ListItem></asp:DropDownList>&nbsp;<br />
        <asp:CheckBox runat="server" ID="CheckKL1" Text="Jag kan tänka mig att bli kommunledare."/>&nbsp;<br />
        <asp:CheckBox runat="server" ID="CheckKL2" Text="Jag kan tänka mig att bli vice kommunledare."/>&nbsp;<br />
        <asp:CheckBox runat="server" ID="CheckVL1" Text="Jag kan tänka mig att bli valkretsledare."/>&nbsp;<br />
        <asp:CheckBox runat="server" ID="CheckVL2" Text="Jag kan tänka mig att bli vice valkretsledare."/>&nbsp;<br />
        <asp:Button ID="ButtonSubmit" runat="server" Text="Ja! Jag vill bli nyckelperson!" 
            CausesValidation="true" onclick="ButtonSubmit_Click" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
    </div>
    </div>

    <asp:UpdatePanel ID="UpdateFinished" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="PanelFinished" Visible="false" runat="server">
        <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Tack!</span><br />
        <div class="DivGroupBoxContents">
            Tack! Ett meddelande har skickats till funktionärer och partiledningen -- någon kommer att kontakta dig inom kort. <b>Välkommen att göra skillnad med Piratpartiet!</b>
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

