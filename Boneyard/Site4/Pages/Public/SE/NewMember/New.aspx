<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New.aspx.cs" Inherits="Pages_Public_SE_NewMember_New"
    CodePage="65001" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title runat="server">PirateWeb - Välkommen till Piratpartiet och Ung Pirat</title>
    <link href="~/style/PirateWeb-v3.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body
        {
            text-align: center;
            min-width: 520px;
            background-image: url(  'NewMember_Background.jpg' );
        }
        #wrapper
        {
            margin: 0 auto;
            width: 520px;
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

        function DisplayPulInfo() {
            document.getElementById('PulInfoLink').style.display = "none";
            document.getElementById('PulInfo').style.display = "";
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <span style="font: 16pt Impact;">
        <telerik:RadAjaxManager ID="RadAjaxManager3" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1"
            UpdatePanelsRenderMode="Inline" meta:resourcekey="RadAjaxManager3Resource1">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="TextPostal">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextPostal" />
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalError" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalErrorUnknown" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommun" />
                        <telerik:AjaxUpdatedControl ControlID="DropDownKommun" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommunError" />
                        <telerik:AjaxUpdatedControl ControlID="DropCountries" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="DropDownKommun">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextPostal" />
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalError" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalErrorUnknown" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommun" />
                        <telerik:AjaxUpdatedControl ControlID="DropDownKommun" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommunError" />
                        <telerik:AjaxUpdatedControl ControlID="DropCountries" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="TextCity">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextPostal" />
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalError" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalErrorUnknown" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommun" />
                        <telerik:AjaxUpdatedControl ControlID="DropDownKommun" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommunError" />
                        <telerik:AjaxUpdatedControl ControlID="DropCountries" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="DropCountries">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextPostal" />
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalError" />
                        <telerik:AjaxUpdatedControl ControlID="LabelPostalErrorUnknown" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommun" />
                        <telerik:AjaxUpdatedControl ControlID="DropDownKommun" />
                        <telerik:AjaxUpdatedControl ControlID="LabelKommunError" />
                        <telerik:AjaxUpdatedControl ControlID="DropCountries" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" Skin="WebBlue" runat="server"
            meta:resourcekey="RadAjaxLoadingPanel1Resource1">
        </telerik:RadAjaxLoadingPanel>
    </span>
    <div class="Header" style="text-align: left; background: white">
        <div style="float: right; position: relative; top: 6pt; padding-right: 8px">
            V&auml;lkommen som medlem!</div>
        <span style="font: 16pt Impact; padding-left: 8px">PIRATEWEB</span></div>
    <div id="wrapper" style="margin-top: 50px">
        <asp:Wizard ID="Wizard" runat="server" ActiveStepIndex="0" Width="500px" Height="300px"
            StartNextButtonText="Forts&auml;tt >>" StepNextButtonText="Forts&auml;tt >>"
            StepPreviousButtonText="<< Backa" FinishPreviousButtonText="Osynlig" FinishCompleteButtonText="Klart!"
            OnNextButtonClick="Wizard_NextButtonClick" 
            OnFinishButtonClick="Wizard_FinishButtonClick" 
            onprerender="Wizard_PreRender">
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server">
                    <div style="padding-right: 1px; ">
                        <span style="padding-top: 1px; padding-bottom: 1px; "><b>Välkommen att bli 
                        medlem i Piratpartiet och Ung Pirat!</b>&nbsp;Medlemskap är kostnadsfritt, men vi 
                        uppmuntrar frivilliga donationer. Mer om det senare.</div>
                 
                        <p>Innan vi börjar, vilken eller vilka organisationerna vill du gå med i?</p>
                    <asp:RadioButton ID="RadioParty" runat="server" Text="Piratpartiet, kostnadsfritt."
                        GroupName="OrganizationSelect" />
                    <br />
                    <br />
                    <asp:RadioButton ID="RadioYouthLeague" runat="server" Text="Ung Pirat (Piratpartiets ungdomsförbund) och Piratpartiet, kostnadsfritt."
                        GroupName="OrganizationSelect" />
                    <br />
                    <br />
                    <asp:Label ID="LabelOrganizationError" runat="server" CssClass="ErrorMessage"></asp:Label>
                    <br />
                    <br />
                    Om du redan är medlem men glömt ditt lösenord, pröva helst<br />
                    <asp:HyperLink ID="LinkLosenord" runat="server" Font-Size="Small" NavigateUrl="~/Pages/Public/SE/People/RequestNewPassword.aspx">Återställ lösenord...</asp:HyperLink>
                    istället för att registrera dig på nytt här.<p class="SmallText" style="margin-bottom: 0px">
                        <span id="PulInfoLink"><a href="javascript:DisplayPulInfo();" style="color: Silver">
                            Information om hur dina personuppgifter hanteras</a></span><span id="PulInfo" style="display: none"><b>Information
                                om personuppgifter</b>:<br />
                                Vi kommer att be om n&aring;gra av dina personuppgifter till v&aring;rt medlemsregister.
                                De uppgifterna, och andra du l&auml;mnar senare, hanteras av f&ouml;reningarnas
                                aktiva funktion&auml;rer f&ouml;r omr&aring;det d&auml;r du bor. De kan ocks&aring;
                                ses av medlemssystemets administrat&ouml;rer, som kan vara aktiva i piratpartier
                                i andra l&auml;nder. Vi l&auml;mnar aldrig, n&aring;gonsin, ut personuppgifter till
                                tredje part. N&auml;r du g&aring;r ur partiet eller Ung Pirat och beg&auml;r det,
                                s&aring; raderas den identifierande informationen (bara statistisk data som postnummer
                                och f&ouml;delse&aring;r sparas).</span></p>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server">
                    <p>
                        F&ouml;r att betala din medlemsavgift, skicka SMS-meddelandet <b>
                            <asp:Label ID="LabelSmsMessageText" runat="server" Text="PP MEDLEM"></asp:Label></b>
                        till nummer <b>72550</b>. Fem kronor dras p&aring; din telefonr&auml;kning, och
                        du kommer att f&aring; en <b>kvittokod</b>. Skriv in den nedan.</p>
                    <p>
                        (Om du bor utanf&ouml;r Sverige, eller har vissa sorters kontantkort, s&aring; kan
                        det h&auml;r vara sv&aring;rt. <a href="http://www2.piratpartiet.se/medlemsavgift">Klicka
                            i s&aring; fall h&auml;r</a> f&ouml;r andra s&auml;tt att betala och bli medlem
                        manuellt.)</p>
                    <br />
                    <b>Kvittokod f&ouml;r medlemsavgiften:</b>
                    <asp:TextBox ID="TextPaymentCode" runat="server" Width="74px"></asp:TextBox>
                    <br />
                    <br />
                    <asp:Label ID="LabelPaymentError" runat="server" CssClass="ErrorMessage"></asp:Label>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep3" meta:resourcekey="WizardStep3Resource1">
                    <p>
                        <asp:Literal ID="Literal26" runat="server" meta:resourcekey="Literal26Resource1">Skriv in dina <strong>medlemsuppgifter</strong> f&ouml;r</asp:Literal>
                        <asp:Label ID="LabelOrganization" runat="server" Text="orgnamn" meta:resourcekey="LabelOrganizationResource1"></asp:Label>:</p>
                    <table>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal5" runat="server" meta:resourcekey="Literal5Resource1">Namn</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextName" runat="server" Columns="30" meta:resourcekey="TextNameResource1"></asp:TextBox>&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:Label ID="LabelNameError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Skriv Namn" meta:resourcekey="LabelNameErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal6" runat="server" meta:resourcekey="Literal6Resource1">Gatuadress</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextStreet" runat="server" Columns="30" meta:resourcekey="TextStreetResource1"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelStreetError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Skriv Gata" meta:resourcekey="LabelStreetErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal7" runat="server" meta:resourcekey="Literal7Resource1">Postnr, ort</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextPostal" runat="server" Columns="3" meta:resourcekey="TextPostalResource1"
                                    AutoPostBack="True" OnTextChanged="TextPostal_TextChanged"></asp:TextBox>&nbsp;<asp:TextBox
                                        ID="TextCity" runat="server" Columns="20" meta:resourcekey="TextCityResource1"
                                        AutoPostBack="True" OnTextChanged="TextCity_TextChanged"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelPostalError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Skriv Postnr" meta:resourcekey="LabelPostalErrorResource2"></asp:Label>
                                <asp:Label ID="LabelCityError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Skriv Ort" meta:resourcekey="Label1Resource1"></asp:Label>
                                <asp:Label ID="LabelPostalErrorUnknown" runat="server" CssClass="ErrorMessage" Text="Okänt Postnr"
                                    Visible="False" meta:resourcekey="LabelPostalErrorUnknownResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="LabelKommun" runat="server" Text="Kommun" Visible="true" meta:resourcekey="LabelKommunResource1"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownKommun" runat="server" OnSelectedIndexChanged="DropDownKommun_SelectedIndexChanged"
                                    Visible="true" meta:resourcekey="DropDownKommunResource1">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="LabelKommunError" runat="server" CssClass="ErrorMessage" Text="" Visible="False"
                                    meta:resourcekey="LabelKommunErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal8" runat="server" meta:resourceKey="Literal8Resource1" Text="Land"></asp:Literal>
                                :
                            </td>
                            <td colspan="2">
                                <asp:DropDownList ID="DropCountries" runat="server" meta:resourceKey="DropCountriesResource1"
                                    AutoPostBack="True" OnSelectedIndexChanged="DropCountries_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal9" runat="server" meta:resourcekey="Literal9Resource1">(Mobil)telefon</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextPhone" runat="server" Columns="30" meta:resourcekey="TextPhoneResource1"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelPhoneError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Skriv Telefon" meta:resourcekey="LabelPhoneErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal10" runat="server" meta:resourcekey="Literal10Resource1">Email</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextEmail" runat="server" Columns="30" meta:resourcekey="TextEmailResource1"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelEmailError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Skriv Email" meta:resourcekey="LabelEmailErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal11" runat="server" meta:resourcekey="Literal11Resource1">F&ouml;delsedatum</asp:Literal>
                                :&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="TextBirthDay" runat="server" Columns="1" meta:resourcekey="TextBirthDayResource1"></asp:TextBox>&nbsp;<asp:DropDownList
                                    ID="DropBirthMonths" runat="server" meta:resourcekey="DropBirthMonthsResource1">
                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource1">januari</asp:ListItem>
                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">februari</asp:ListItem>
                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource3">mars</asp:ListItem>
                                    <asp:ListItem Value="4" meta:resourcekey="ListItemResource4">april</asp:ListItem>
                                    <asp:ListItem Value="5" meta:resourcekey="ListItemResource5">maj</asp:ListItem>
                                    <asp:ListItem Value="6" meta:resourcekey="ListItemResource6">juni</asp:ListItem>
                                    <asp:ListItem Value="7" meta:resourcekey="ListItemResource7">juli</asp:ListItem>
                                    <asp:ListItem Value="8" meta:resourcekey="ListItemResource8">augusti</asp:ListItem>
                                    <asp:ListItem Value="9" meta:resourcekey="ListItemResource9">september</asp:ListItem>
                                    <asp:ListItem Value="10" meta:resourcekey="ListItemResource10">oktober</asp:ListItem>
                                    <asp:ListItem Value="11" meta:resourcekey="ListItemResource11">november</asp:ListItem>
                                    <asp:ListItem Value="12" meta:resourcekey="ListItemResource12">december</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:TextBox ID="TextBirthYear" runat="server" Columns="4" meta:resourcekey="TextBirthYearResource1"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelBirthdateError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Felaktigt datum" meta:resourcekey="LabelBirthdateErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <span style="font-size: 80%">(t.ex. "21 januari 1982")</span>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal12" runat="server" meta:resourcekey="Literal12Resource1">Jag &auml;r...</asp:Literal>
                            </td>
                            <td>
                                <asp:DropDownList ID="DropGenders" runat="server" meta:resourcekey="DropGendersResource1">
                                    <asp:ListItem Selected="True" Value="Unknown" Text=" V&#228;lj en..." meta:resourcekey="ListItemResource13"></asp:ListItem>
                                    <asp:ListItem Value="Male" Text="Man" meta:resourcekey="ListItemResource14"></asp:ListItem>
                                    <asp:ListItem Value="Female" Text="Kvinna" meta:resourcekey="ListItemResource15"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="LabelGenderError" runat="server" CssClass="ErrorMessage" Visible="false"
                                    Text="Välj ett" meta:resourcekey="LabelGenderErrorResource1"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep4">
                    <p>
                        Nu &auml;r vi n&auml;stan klara.</p>
                    <p>
                        <b>V&auml;lj det l&ouml;senord</b> du vill ha n&auml;r du loggar p&aring; medlemssystemet
                        <b>PirateWeb</b>, t.ex. f&ouml;r att uppdatera dina medlemsuppgifter eller best&auml;lla
                        nyhetsbrev.</p>
                    <br />
                    <table>
                        <tr>
                            <td>
                                V&auml;lj ett l&ouml;senord:
                            </td>
                            <td>
                                <asp:TextBox ID="TextPassword1" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Skriv samma igen:
                            </td>
                            <td>
                                <asp:TextBox ID="TextPassword2" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Label ID="LabelPasswordError" runat="server" CssClass="ErrorMessage" Visible="false"
                        Text="Skriv Password" meta:resourcekey="LabelPasswordErrorResource2"></asp:Label>
                    <asp:Label ID="LabelPasswordErrorSame" runat="server" CssClass="ErrorMessage" Text="Bägge måste vara lika"
                        Visible="False" meta:resourcekey="LabelPasswordErrorSameResource1"></asp:Label>
                    <asp:Label ID="LabelPasswordErrorLength" runat="server" CssClass="ErrorMessage" Text="Minst 5 tecken"
                        Visible="False" meta:resourcekey="LabelPasswordErrorLengthResource1"></asp:Label>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep5">
                    <p>
                        Till sist, <b>hur aktiv</b> vill du vara i piratrörelsen?</p>
                    <table>
                        <tr>
                            <td valign="top" width="120px">
                                <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityPassive"
                                    Text="Passiv" Checked="true" />
                            </td>
                            <td>
                                Du får nyhetsbrev, lokala utskick och kalleleser till möten, men inte information
                                om blixtaktioner och liknande.
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" width="120px">
                                <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityActivist"
                                    Text="Aktivist" />
                            </td>
                            <td>
                                När partiet gör blixtaktioner och valrörelse, så får du information om det (utan
                                något som helst krav på deltagande) via mail eller SMS. Det här är en indikation
                                på att du är intresserad av att vara aktiv medlem.
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" width="120px">
                                <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityOfficer"
                                    Text="Funktionär" />&nbsp;&nbsp;
                            </td>
                            <td>
                                Om du vill bli funktionär fullt ut och ta ansvar för en del av arbetet i din kommun,
                                eller rentav leda arbetet i din kommun, så är det mycket välkommet.
                            </td>
                        </tr>
                    </table>
                    <br />
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="WizardStep0">
                    <div style="width: 100%; text-align: center; vertical-align: middle;">
                        <br />
                        <asp:Label ID="LabelBecomeMember" runat="server" Font-Bold="True" 
                            Font-Size="X-Large" >Bli medlem i Piratpartiet <br />eller Ung Pirat</asp:Label>
                        <br />
                        <br />
                        <br />
                        <asp:Label ID="LabelStartRegister" runat="server">Börja medlemsregistrering genom<br />att trycka på 'Fortsätt >>'</asp:Label></div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep6">
                    <p>
                        <b>V&auml;lkommen som medlem!</b></p>
                    <p>
                        Du &auml;r nu medlem i:<br />
                        <asp:Label ID="LabelMemberOfOrganizations" runat="server" Text="Label"></asp:Label></p>
                    <p>Piratpartiet finansieras av frivilliga bidrag fr&aring;n personer som dig. Vi har ingen medlemsavgift, utan alla bidrar med det som de kan och vill. <b>Ge g&auml;rna ett m&aring;nadsbidrag</b> genom att v&auml;lja ett belopp:</p>
                        
<div style="float: left; width: 80px;">
<form action="https://www.paypal.com/cgi-bin/webscr" method="post">
		<input name="cmd" value="_s-xclick" type="hidden"> <input alt="Bidra med 50:- per månad" name="submit" src="http://docs.piratpartiet.se/buttons/monthlydonations/Monthly-SEK50.png" type="image" border="0"> <img alt="" src="https://www.paypal.com/en_US/i/scr/pixel.gif" width="1" height="1" border="0"> <input name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----" type="hidden"></form>

</div>
<div style="float: left; width: 80px;">
<form action="https://www.paypal.com/cgi-bin/webscr" method="post">
		<input name="cmd" value="_s-xclick" type="hidden"> <input alt="Bidra med 100:- per månad" name="submit" src="http://docs.piratpartiet.se/buttons/monthlydonations/Monthly-SEK100.png" type="image" border="0"> <img alt="" src="https://www.paypal.com/en_US/i/scr/pixel.gif" width="1" height="1" border="0"> <input name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----" type="hidden"></form>
</div>
<div style="float: left; width: 80px;">
<form action="https://www.paypal.com/cgi-bin/webscr" method="post">
		<input name="cmd" value="_s-xclick" type="hidden"> <input alt="Bidra med 200:- per månad" name="submit" src="http://docs.piratpartiet.se/buttons/monthlydonations/Monthly-SEK200.png" type="image" border="0"> <img alt="" src="https://www.paypal.com/en_US/i/scr/pixel.gif" width="1" height="1" border="0"> <input name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----" type="hidden"></form>
</div>

	<br clear="all">                        
                        
                    <p>Vi vet att PayPal &auml;r onda och arbetar p&aring; att g&aring; bort fr&aring;n dem. Om du hellre månadsbidrar genom din Internetbank, s&aring; finns instruktioner p&aring; <a href="http://www.piratpartiet.se/guldpirat/">den h&auml;r sidan</a>.</p>
                    <asp:Panel ID="PanelRepeatLink" runat="server" Visible="false">
                        <br />
                        eller<br />
                        <asp:HyperLink ID="RepeatLink" runat="server">Registrera ytterligare ny medlem</asp:HyperLink><br />
                    </asp:Panel>
                    <p>
                        Om du inte f&aring;tt dina mail inom ett par timmar, beror det oftast p&aring; &ouml;veraggressiva
                        spamskydd. Kontakta oss d&aring; p&aring; medlemsservice@piratpartiet.se, s&aring;
                        kan vi avhj&auml;lpa situationen.</p>
                </asp:WizardStep>
            </WizardSteps>
            <FinishPreviousButtonStyle CssClass="Invisible" />
            <StepStyle VerticalAlign="Top" />
            <StepNavigationTemplate>
                <asp:Button ID="ButtonAbort" runat="server" CausesValidation="False" CommandName="Cancel"
                    Text="Avbryt" UseSubmitBehavior="False" OnClick="ButtonAbort_Click" />
                <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                    Text="&lt;&lt; Backa" UseSubmitBehavior="False" />
                <asp:Button ID="StepNextButton" runat="server" UseSubmitBehavior="True" CommandName="MoveNext"
                    Text="Forts&auml;tt &gt;&gt;" />
            </StepNavigationTemplate>
            <FinishNavigationTemplate>
                <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Klart!" />
            </FinishNavigationTemplate>
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Forts&#228;tt &gt;&gt;" />
            </StartNavigationTemplate>
        </asp:Wizard>
    </div>
    <asp:Label ID="LabelReferrer" runat="server" Visible="false" />
    </form>
</body>
</html>
