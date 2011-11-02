<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MemberSignup.aspx.cs" Inherits="Pages_Public_NO_MemberSignup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>PirateWeb - V&auml;lkommen till Piratpartiet i Norge</title>
<link href="~/style/PirateWeb-v3.css" rel="stylesheet" type="text/css" />    
<style type="text/css">

body {
	text-align: center;
	min-width: 520px;
	background-image:url('NewMember_Background.jpg');
}

#wrapper {
	margin:0 auto;
	width:520px;
	text-align: left;
	background: white;
	border: 2px solid #404040;
	padding: 20px;
	padding-left: 25px;
}

.Invisible
{
    display: none;
}

.SmallText
{
    font: 7pt Tahoma;
}

#PulInfoLink
{
    font-size: 8pt;
}

</style>
<script language="javascript" type="text/javascript">

function DisplayPulInfo()
{
    document.getElementById ('PulInfoLink').style.display = "none";
    document.getElementById ('PulInfo').style.display = "";
}

</script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="Header" style="text-align: left; background: white"><div style="float:right; position: relative; top: 6pt; padding-right: 8px">V&auml;lkommen som medlem!</div>
<span style="font: 16pt Impact; padding-left: 8px">PIRATEWEB</span></div>
    <div id="wrapper" style="margin-top: 50px">
    <asp:wizard ID="Wizard" runat="server" ActiveStepIndex="2" Width="500px" Height="300px" StartNextButtonText="Forts&auml;tt >>" StepNextButtonText="Forts&auml;tt >>" StepPreviousButtonText="<< Backa" FinishPreviousButtonText="Osynlig" FinishCompleteButtonText="Klart!" OnNextButtonClick="Wizard_NextButtonClick" OnFinishButtonClick="Wizard_FinishButtonClick"> <WizardSteps> <asp:WizardStep ID="WizardStep1" runat="server">
        <p><strong>V&auml;lkommen att bli medlem i <b>Norska</b> Piratpartiet!</strong> Medlemskap &auml;r kostnadsfritt, men vi uppmuntrar frivilliga donationer. Mer om det senare.</p><p>Den h&auml;r sidan &auml;r p&aring; svenska f&ouml;r att den inte &ouml;versatts &auml;nnu. Det kommer.</p><br />
        <asp:RadioButton ID="RadioParty" Checked="true" Visible="false" runat="server" Text="Piratpartiet, kostnadsfritt." GroupName="OrganizationSelect" />
        <br />
        <asp:RadioButton ID="RadioYouthLeague" Visible="false" runat="server" Text="Jag &#228;r under 26, och vill g&#229; med i Ung Pirat (och Piratpartiet), kostnadsfritt." GroupName="OrganizationSelect" />
        <br />
        <br />
        <asp:Label ID="LabelOrganizationError" runat="server" CssClass="ErrorMessage"></asp:Label>
        <br />
        <br /><p class="SmallText" style="margin-bottom: 0px"><span id="PulInfoLink"><a href="javascript:DisplayPulInfo();" style="color:Silver">Information om hur dina
        personuppgifter hanteras</a></span><span id="PulInfo" style="display:none"><b>Information om
        personuppgifter</b>:<br />Vi kommer att be om n&aring;gra av dina personuppgifter till v&aring;rt medlemsregister.
        De uppgifterna, och andra du l&auml;mnar senare, hanteras av f&ouml;reningarnas aktiva funktion&auml;rer
        f&ouml;r omr&aring;det d&auml;r du bor. De kan ocks&aring; ses av medlemssystemets administrat&ouml;rer, som kan
        vara aktiva i piratpartier i andra l&auml;nder. Vi l&auml;mnar aldrig, n&aring;gonsin, ut personuppgifter till
        tredje part. N&auml;r du g&aring;r ur partiet och beg&auml;r det, s&aring; raderas den identifierande
        informationen (bara statistisk data som postnummer och f&ouml;delse&aring;r sparas).</span></p>
        </asp:WizardStep> <asp:WizardStep ID="WizardStep2" runat="server">
            <p>F&ouml;r att betala din medlemsavgift, skicka SMS-meddelandet
            <b><asp:Label ID="LabelSmsMessageText" runat="server" Text="PP MEDLEM"></asp:Label></b>
            till nummer <b>72550</b>. Fem kronor dras p&aring; din telefonr&auml;kning, och du kommer att
            f&aring; en <b>kvittokod</b>. Skriv in den nedan.</p>
            <p>(Om du bor utanf&ouml;r Sverige, eller har vissa sorters kontantkort, s&aring; kan det h&auml;r
            vara sv&aring;rt. <a href="http://www2.piratpartiet.se/medlemsavgift">Klicka i s&aring; fall h&auml;r</a> f&ouml;r andra s&auml;tt att betala och bli medlem manuellt.)</p>
            <br />
            <b>Kvittokod f&ouml;r medlemsavgiften:</b>
            <asp:TextBox ID="TextPaymentCode" runat="server" Width="74px"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="LabelPaymentError" runat="server" CssClass="ErrorMessage"></asp:Label>
        </asp:WizardStep> 
        <asp:WizardStep runat="server" ID="WizardStep3">
            <p>Skriv in dina <strong>medlemsuppgifter</strong> f&ouml;r <asp:Label ID="LabelOrganization" runat="server" Text="Label"></asp:Label>:</p>
            <table>
                <tr>
                    <td>Namn:</td>
                    <td><asp:TextBox ID="TextName" runat="server" Columns="30"></asp:TextBox>&nbsp;&nbsp;</td>
                    <td><asp:Label ID="LabelNameError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
                <tr>
                    <td>Gatuadress:</td>
                    <td><asp:TextBox ID="TextStreet" runat="server" Columns="30"></asp:TextBox></td>
                    <td><asp:Label ID="LabelStreetError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
                <tr>
                    <td>Postnr, ort:</td>
                    <td><asp:TextBox ID="TextPostal" runat="server" Columns="3"></asp:TextBox>&nbsp;<asp:TextBox ID="TextCity" runat="server" Columns="20"></asp:TextBox></td>
                    <td><asp:Label ID="LabelPostalError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
                <tr>
                    <td>Land:</td>
                    <td colspan="2">
                        <asp:DropDownList ID="DropCountries" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Telefon:</td>
                    <td><asp:TextBox ID="TextPhone" runat="server" Columns="30"></asp:TextBox></td>
                    <td><asp:Label ID="LabelPhoneError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
                <tr>
                    <td>Email:</td>
                    <td><asp:TextBox ID="TextEmail" runat="server" Columns="30"></asp:TextBox></td>
                    <td><asp:Label ID="LabelEmailError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
                <tr>
                    <td>F&ouml;delsedatum:&nbsp;&nbsp;</td>
                    <td><asp:TextBox ID="TextBirthDay" runat="server" Columns="1"></asp:TextBox>&nbsp;<asp:DropDownList
                        ID="DropBirthMonths" runat="server">
                        <asp:ListItem Value="1">januari</asp:ListItem>
                        <asp:ListItem Value="2">februari</asp:ListItem>
                        <asp:ListItem Value="3">mars</asp:ListItem>
                        <asp:ListItem Value="4">april</asp:ListItem>
                        <asp:ListItem Value="5">maj</asp:ListItem>
                        <asp:ListItem Value="6">juni</asp:ListItem>
                        <asp:ListItem Value="7">juli</asp:ListItem>
                        <asp:ListItem Value="8">augusti</asp:ListItem>
                        <asp:ListItem Value="9">september</asp:ListItem>
                        <asp:ListItem Value="10">oktober</asp:ListItem>
                        <asp:ListItem Value="11">november</asp:ListItem>
                        <asp:ListItem Value="12">december</asp:ListItem>
                    </asp:DropDownList>&nbsp;<asp:TextBox ID="TextBirthYear" runat="server" Columns="4"></asp:TextBox></td>
                    <td><asp:Label ID="LabelBirthdateError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td><span style="font-size: 80%">(t.ex. "21 januari 1982")</span></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Jag &auml;r...</td>
                    <td>
                        <asp:DropDownList ID="DropGenders" runat="server">
                            <asp:ListItem Selected="True" Value="Unknown">V&#228;lj en...</asp:ListItem>
                            <asp:ListItem Value="Male">Man</asp:ListItem>
                            <asp:ListItem Value="Female">Kvinna</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td><asp:Label ID="LabelGenderError" runat="server" CssClass="ErrorMessage"></asp:Label></td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep runat="server" ID="WizardStep4">
            <p>Nu &auml;r vi n&auml;stan klara.</p>
            
            <p>Till sist, <b>v&auml;lj det l&ouml;senord</b> du vill ha n&auml;r du loggar p&aring; medlemssystemet <b>PirateWeb</b>,
            t.ex. f&ouml;r att uppdatera dina medlemsuppgifter eller best&auml;lla nyhetsbrev.</p>
            <br />
            <table>
                <tr>
                    <td>V&auml;lj ett l&ouml;senord:</td>
                    <td>
                        <asp:TextBox ID="TextPassword1" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Skriv samma igen:</td>
                    <td>
                        <asp:TextBox ID="TextPassword2" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Label ID="LabelPasswordError" runat="server" CssClass="ErrorMessage"></asp:Label>
        </asp:WizardStep>
        <asp:WizardStep runat="server" ID="WizardStep5">
            <p><b>V&auml;lkommen som medlem!</b></p>
            
            <p>Du &auml;r nu medlem i:<br />
            <asp:Label ID="LabelMemberOfOrganizations" runat="server" Text="Label"></asp:Label></p>
            <p>Du kommer strax att f&aring; e-mail som ber&auml;ttar mer. Under tiden kan du v&auml;lja att g&aring;
            till n&aring;got av f&ouml;ljande st&auml;llen:</p>
            <br />
            <a href="http://www.piratpartiet.biz">Piratpartiets hemsida</a><br />
            <br />
            <p>Om du inte f&aring;tt dina mail inom ett par timmar, beror det oftast p&aring; &ouml;veraggressiva spamskydd. Kontakta oss d&aring; p&aring; medlemsservice@piratpartiet.se, s&aring; kan vi avhj&auml;lpa situationen.</p>
            </asp:WizardStep>
    </WizardSteps> 
        <FinishPreviousButtonStyle CssClass="Invisible" />
        <StepStyle VerticalAlign="Top" />
        <StepNavigationTemplate>
            <asp:Button ID="ButtonAbort" runat="server" CausesValidation="False" CommandName="Cancel"
                Text="Avbryt" UseSubmitBehavior="False" OnClick="ButtonAbort_Click" />
            <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                Text="&lt;&lt; Backa" UseSubmitBehavior="False" />
            <asp:Button ID="StepNextButton" runat="server" UseSubmitBehavior="True" CommandName="MoveNext" Text="Forts&auml;tt &gt;&gt;" />
        </StepNavigationTemplate>
        <FinishNavigationTemplate>
            <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Klart!" />
        </FinishNavigationTemplate>
        <StartNavigationTemplate>
            <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Forts&#228;tt &gt;&gt;" />
        </StartNavigationTemplate>
    </asp:wizard>    
    </div>
    <asp:Label ID="LabelReferrer" runat="server" Visible="false" />
    </form>
</body>
</html>

