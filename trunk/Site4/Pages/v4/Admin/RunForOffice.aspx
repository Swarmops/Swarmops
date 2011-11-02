<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="RunForOffice.aspx.cs" Inherits="Pages_v4_Admin_RunForOffice" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

<div class="DivMainContent">
    <asp:Panel ID="PanelCallForCandidatesIntro" runat="server">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Så du vill bli riksdagspolitiker?</span><br />
        <div class="DivGroupBoxContents">
            <span style="line-height:120%">För att kandidera i primärvalet till riksdagsvalsedlarna behöver du ett <b>foto</b> som duger till pressen och en <b>reklamtext</b> på 300 tecken.</span>
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelCallForCandidatesClosed" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Kandideringen är stängd</span><br />
        <div class="DivGroupBoxContents">
            <span style="line-height:120%">Kandideringen till riksdagsvalsedlarna är nu stängd. Om du har kandiderat, lycka till i primärvalet!</span>
        </div>
    </div>
    </asp:Panel>

    <asp:Panel ID="PanelCandidacy" runat="server" >
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Din kandidatur</span><br />
        <div class="DivGroupBoxContents">
        <img src="../../../Handlers/DisplayPortrait.aspx" alt="Kandidatporträtt" align="right" /><b>Foto</b> 
            i presskvalitet (JPG, minst 600x800, högst 10M stor fil, porträttsform):
        <telerik:RadUpload ID="UploadPhoto" Runat="server" ControlObjectsVisibility="None" 
                    MaxFileInputsCount="1" EnableFileInputSkinning="false"
                    TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" /><asp:Button ID="ButtonUpload" 
                                    runat="server" Text="Upload/replace photo"
                    onclick="ButtonUpload_Click" CausesValidation="False" />&nbsp;<asp:CustomValidator 
                runat="server" Display="Dynamic" Text="Du måste ha ett foto." ID="ValidatorPhotoRequired"
                onservervalidate="ValidatorPhotoRequired_ServerValidate" /><br /><br />
                    
        Fotografens namn: <asp:TextBox ID="TextPhotographer" runat="server" /> <asp:RequiredFieldValidator ID="ValidatorPhotographerNameRequired" ControlToValidate="TextPhotographer" Display="Dynamic" runat="server" Text="Ange fotografens namn." /><br />
        <asp:CheckBox ID="CheckPhotoOk" runat="server" Text="Fotot är ok att sprida fritt, så länge fotografen anges." />&nbsp; 
            <asp:CustomValidator ID="ValidatorPhotoOkRequired" runat="server" 
                Display="Dynamic" Text="Du måste intyga detta." 
                onservervalidate="ValidatorPhotoOkRequired_ServerValidate" /><br /><br />
        
        <b>Reklamtext</b> för din interna kandidatur (max 300 tecken):<br />
        <asp:TextBox ID="TextCandidacy" runat="server" TextMode="MultiLine" Rows="4" /><br />
        <asp:RequiredFieldValidator ID="ValidatorCandidacyRequired" runat="server" Display="Dynamic" Text="Du behöver skriva en kandidatur!" ControlToValidate="TextCandidacy" />
            <asp:CustomValidator ID="ValidatorCandidacyLength" runat="server" 
                Display="Dynamic" Text="Texten är för lång (x tecken, max 300)." 
                onservervalidate="ValidatorCandidacyLength_ServerValidate" /> <br />
        
        Ditt <b>personnummer</b>: <asp:TextBox ID="TextPersonalNumber" runat="server" /> (behövs för valsedelsbeställning) <asp:RequiredFieldValidator ID="ValidatorPersonalNumberRequired" ControlToValidate="TextPersonalNumber" runat="server" Text="Du måste fylla i personnummer." /><br />
        
        Din <b>T-shirt-storlek</b>: <asp:DropDownList ID="DropTShirtSizes" runat="server">
        <asp:ListItem Selected="True" Value="0">-- Välj--</asp:ListItem>
        <asp:ListItem Value="XXL">XX-Large</asp:ListItem>
        <asp:ListItem Value="XL">X-Large</asp:ListItem>
        <asp:ListItem Value="L">Large</asp:ListItem>
        <asp:ListItem Value="M">Medium</asp:ListItem>
        <asp:ListItem Value="S">Small</asp:ListItem>
        </asp:DropDownList> (för senare bruk) <asp:CompareValidator runat="server" ControlToValidate="DropTShirtSizes" ID="ValidatorTShirtSizes" Operator="NotEqual" ValueToCompare="0" Text="Välj en storlek." Display="Dynamic" /><br /><br />
        
        Om du har en <b>blogg</b> (eller annan sida med mer information om dig), vad är <b>namnet</b> och <b>adressen</b>?<br />
        <div style="float:left">Bloggens namn:&nbsp;&nbsp;<br />Adress:</div>
        <asp:TextBox ID="TextBlogName" runat="server" />&nbsp;<br /><asp:TextBox ID="TextBlogUrl" runat="server" />&nbsp;<br /><br />
        
        <div style="float:left"><asp:CheckBox ID="CheckUnderstand1" runat="server" />&nbsp;</div><span style="line-height:120%;position:relative;top:5px">Jag har läst och förstått <a target="_blank" href="http://www.piratpartiet.se/principer">principprogrammet</a> och framför allt <a href="http://www.piratpartiet.se/vagmastarstallning">vågmästarstrategin</a>, som Piratpartiets framgång är helt beroende av.</span><br clear="all" />
        <div style="float:left;clear:both"><asp:CheckBox ID="CheckUnderstand2" runat="server" />&nbsp;</div><span style="line-height:120%;position:relative;top:5px">Jag förstår att Lobbyn och andra partiers politiker kommer att börja försöka etablera personliga relationer med mig för att få mig att känna som en svikare om jag inte går dem tillmötes, trots att jag valts för något helt annat, och att trycket bitvis kan vara <a target="_blank" href="http://sv.wikipedia.org/wiki/Fredrick_Federley">mycket hårt</a>.</span><br clear="all" />
        
        <br /><asp:Button ID="ButtonSubmit" runat="server" Text="Ja, jag kandiderar!" 
                onclick="ButtonSubmit_Click" /> 
            <asp:CustomValidator Text="Du måste kryssa i de två kryssrutorna." 
                ID="ValidatorChecksRequired" runat="server" Display="Dynamic" 
                onservervalidate="ValidatorChecksRequired_ServerValidate" />
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelCandidate" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Lycka till i valet!</span><br />
        <div class="DivGroupBoxContents">
            Du är nu kandidat i primärvalet inför 2010. Lycka till!
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelNotEligible" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Du kan inte ställa upp</span><br />
        <div class="DivGroupBoxContents">
            Du kan tyvärr inte ställa upp i primärvalet inför 2010.
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelEditCandidacy" runat="server" Visible="false">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Vill du ändra din kandidattext?</span><br />
        <div class="DivGroupBoxContents">
            Här är din nuvarande kandidattext. Om du vill ändra något, så skriv om den och tryck "Spara". Maxlängden är 300 tecken.<br />
            <asp:TextBox ID="TextEditCandidacy" runat="server" TextMode="MultiLine" Rows="4" /><br />
            <asp:RequiredFieldValidator ID="ValidatorEditCandidacyRequired" runat="server" Display="Dynamic" Text="Du behöver skriva en kandidatur!" ControlToValidate="TextEditCandidacy" />
                <asp:CustomValidator ID="ValidatorEditCandidacyLength" runat="server" 
                    Display="Dynamic" Text="Texten är för lång (x tecken, max 300)." 
                    onservervalidate="ValidatorEditCandidacyLength_ServerValidate" /><br />
             <asp:Button Text="Spara" ID="ButtonSaveChangedCandidacy" runat="server" OnClick="ButtonSaveChangedCandidacy_Click" />
        </div>
    </div>
    </asp:Panel>

</div>


</asp:Content>

