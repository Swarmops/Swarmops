<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="AddInternalPollCandidate.aspx.cs" Inherits="Pages_v4_Admin_AddInternalPollCandidate" Title="Add Internal Poll Candidate - PirateWeb" %>
<%@ Register Src="~/Controls/v4/ComboPerson.ascx" TagName="ComboPerson" TagPrefix="pw4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

<div class="DivMainContent">
    <asp:Panel ID="PanelCallForCandidatesIntro" runat="server">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Lägg till kandidat</span><br />
        <div class="DivGroupBoxContents">
            <span style="line-height:120%">Du lägger till kandidater till valet "<asp:Label ID="LabelPollName1" runat="server" />". För varje kandidat behövs ett <b>foto</b> som duger till pressen och en <b>reklamtext</b> om kandidaturen.</span>
        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelCandidacySelector" runat="server" >
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Kandidatur för <asp:Label ID="LabelPollName2" runat="server" /></span><br />
        <div class="DivGroupBoxContents">
            Vem lägger du till som kandidat? <pw4:ComboPerson ID="PersonCandidate" OnSelectedPersonChanged="PersonCandidate_SelectedPersonChanged" runat="server" />
        </div>
    </div>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdateCandidacy" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Panel ID="PanelCandidacy" runat="server">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Kandidatur för <asp:Label ID="LabelCandidateName1" runat="server" /></span><br />
        <div class="DivGroupBoxContents">
        <asp:Image ImageAlign="Right" AlternateText="Kandidatporträtt" runat="server" ID="ImageCandidatePhoto" /><b>Foto</b>
            i presskvalitet (JPG, minst 600x800, högst 10M stor fil, porträttsform):
        <telerik:RadUpload ID="UploadPhoto" Runat="server" ControlObjectsVisibility="None" 
                    MaxFileInputsCount="1" EnableFileInputSkinning="false"
                    TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" /><asp:Button ID="ButtonUpload" 
                                    runat="server" Text="Upload/replace photo"
                    onclick="ButtonUpload_Click" CausesValidation="False" />><br /><br />
                    
        Fotografens namn: <asp:TextBox ID="TextPhotographer" runat="server" /> <br />
        <asp:CheckBox ID="CheckPhotoOk" runat="server" Text="Fotot är ok att sprida fritt, så länge fotografen anges." />&nbsp; 
            <br /><br />
        
        <b>Reklamtext</b> för kandidaturen:<br />
        <asp:TextBox ID="TextCandidacy" runat="server" TextMode="MultiLine" Rows="4" /><br />
        <br />
        
        Kandidatens <b>personnummer</b>: <asp:TextBox ID="TextPersonalNumber" runat="server" /> (behövs om valet är t.ex. för valsedlar)<br />
        
        Om kandidaten har en <b>blogg</b> (eller annan sida med mer information), vad är <b>namnet</b> och <b>adressen</b>?<br />
        <div style="float:left">Bloggens namn:&nbsp;&nbsp;<br />Adress:</div>
        <asp:TextBox ID="TextBlogName" runat="server" />&nbsp;<br /><asp:TextBox ID="TextBlogUrl" runat="server" />&nbsp;<br /><br />
      
        <br /><asp:Button ID="ButtonSubmit" runat="server" Text="Lägg till kandidaten" 
                onclick="ButtonSubmit_Click" /> 

        </div>
    </div>
    </asp:Panel>
    <asp:Panel ID="PanelAlreadyCandidate" Visible="false" runat="server">
    <div class="DivGroupBox">
        <span class="DivGroupBoxTitle">Kandidatur klar för <asp:Label ID="LabelCandidateName2" runat="server" /></span><br />
        <div class="DivGroupBoxContents">
            <asp:Label ID="LabelCandidateName3" runat="server" /> är redan kandidat i "<asp:Label ID="LabelPollName3" runat="server" />".
        </div>
    </div>
    </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="PersonCandidate" EventName="SelectedPersonChanged" />
        <asp:PostBackTrigger ControlID="ButtonSubmit" />
    </Triggers>
    </asp:UpdatePanel>

</div>


</asp:Content>

