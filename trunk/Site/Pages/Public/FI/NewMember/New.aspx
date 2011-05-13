<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New.aspx.cs" Inherits="Pages_Public_FI_NewMember_New" Culture="fi-FI" UICulture="fi-FI" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title runat="server">PirateWeb</title>
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
        <telerik:RadAjaxManager ID="RadAjaxManager3" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline" meta:resourcekey="RadAjaxManager3Resource1">
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
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" Skin="WebBlue" runat="server" meta:resourcekey="RadAjaxLoadingPanel1Resource1">
        </telerik:RadAjaxLoadingPanel>
    </span>
    <div class="Header" style="text-align: left; background: white">
        <div style="float: right; position: relative; top: 6pt; padding-right: 8px">
            <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" Text="V&amp;auml;lkommen som medlem!"></asp:Literal>
        </div>
        <span style="font: 16pt Impact; padding-left: 8px">PIRATEWEB</span></div>
    <div id="wrapper" style="margin-top: 50px; height: 371px;">
        <asp:Wizard ID="Wizard" runat="server" ActiveStepIndex="0" Width="500px" Height="333px" StartNextButtonText="Forts&auml;tt >>" StepNextButtonText="Forts&auml;tt >>" StepPreviousButtonText="<< Backa" FinishPreviousButtonText="Osynlig" FinishCompleteButtonText="Klart!" OnNextButtonClick="Wizard_NextButtonClick" OnFinishButtonClick="Wizard_FinishButtonClick" meta:resourcekey="WizardResource1">
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server" meta:resourcekey="WizardStep1Resource1">
                    <%-- 
                    <asp:Literal ID="Literal27" runat="server" meta:resourcekey="Literal27Resource1">Om du redan är medlem men glömt ditt lösenord, pröva helst</asp:Literal>
                    <br />
                    <asp:HyperLink ID="LinkLosenord" runat="server" Font-Size="Small" NavigateUrl="~/Pages/Public/FI/People/RequestNewPassword.aspx" meta:resourcekey="LinkLosenordResource1" Text="Återställ lösenord...">
                    </asp:HyperLink>
                    <asp:Literal ID="Literal29" runat="server" meta:resourcekey="Literal29Resource1">istället för att registrera dig på nytt här.</asp:Literal>--%><p>
                        <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1"><strong>V&auml;lkommen att bli medlem i Piratpartiet eller Ung Pirat!</strong> Medlemskap
                        &auml;r kostnadsfritt, men vi uppmuntrar frivilliga donationer. Mer om det senare.</asp:Literal>
                    </p>
                    <p>
                        <asp:Literal ID="Literal4" runat="server" meta:resourcekey="Literal4Resource1">Innan vi b&ouml;rjar, vilken av organisationerna vill du g&aring; med i?</asp:Literal>
                    </p>
                    <asp:CheckBox ID="cbParty" runat="server" Text="Piratpartiet" GroupName="OrganizationSelect" Checked="true" meta:resourcekey="RadioYouthLeagueResource1" Visible="true" /><br />
                    &nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbPartyAndLocal" runat="server" Text="Min lokala partiförening där sådan finns." GroupName="OrganizationSelect" meta:resourcekey="RadioPartyResource1" Visible="true" Checked="True" />
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:DropDownList ID="DropDownListSubOrg" runat="server">
                    </asp:DropDownList>
                    <br />
                    <br />
                    <asp:CheckBox ID="cbYouthOnly" runat="server" Text="Jag  vill g&#229; med i Ung Pirat (enbart), kostnadsfritt." GroupName="OrganizationSelect" Checked="True" meta:resourcekey="RadioYouthLeagueOnlyResource1" />
                    <br />
                    <br />
                    <asp:Label ID="LabelOrganizationError" runat="server" CssClass="ErrorMessage" Visible="false" meta:resourcekey="LabelOrganizationErrorResource1" Text="Du måste ange en organisation."></asp:Label>
                    <br />
                    <br />
                    <p class="SmallText" style="margin-bottom: 0px">
                        <!-- <span id="PulInfoLink"><a href="javascript:DisplayPulInfo();" style="color: Silver">
                        -->
                        <span id="PulInfoLink"><a target="_blank" href="http://www.piraattipuolue.fi/images/rekisteriseloste.pdf" style="color: Silver">
                            <asp:Literal ID="Literal30" runat="server" meta:resourcekey="Literal30Resource1">Information om hur dina personuppgifter hanteras</asp:Literal>
                        </a></span><span id="PulInfo" style="display: none"><b>
                            <asp:Literal ID="Literal31" runat="server" meta:resourcekey="Literal31Resource1">
                            Information om personuppgifter</asp:Literal></b>:<br />
                            <asp:Literal ID="Literal33" runat="server" meta:resourcekey="Literal33Resource1"> Vi kommer att be om n&aring;gra av dina personuppgifter till v&aring;rt medlemsregister.
                            De uppgifterna, och andra du l&auml;mnar senare, hanteras av f&ouml;reningarnas
                            aktiva funktion&auml;rer f&ouml;r omr&aring;det d&auml;r du bor. De kan ocks&aring;
                            ses av medlemssystemets administrat&ouml;rer, som kan vara aktiva i piratpartier
                            i andra l&auml;nder. Vi l&auml;mnar aldrig, n&aring;gonsin, ut personuppgifter till
                            tredje part. N&auml;r du g&aring;r ur partiet eller Ung Pirat och beg&auml;r det,
                            s&aring; raderas den identifierande informationen (bara statistisk data som postnummer
                            och f&ouml;delse&aring;r sparas).</asp:Literal>
                        </span>
                    </p>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" meta:resourcekey="WizardStep2Resource1">
                    <p>
                        <asp:Literal ID="Literal34" runat="server" meta:resourcekey="Literal34Resource1">F&ouml;r att betala din medlemsavgift, skicka SMS-meddelandet</asp:Literal>
                        <b>
                            <asp:Label ID="LabelSmsMessageText" runat="server" Text="PP MEDLEM" meta:resourcekey="LabelSmsMessageTextResource1"></asp:Label>
                        </b>
                        <asp:Literal ID="Literal32" runat="server" meta:resourcekey="Literal32Resource1">
                            till nummer <b>72550</b>. Fem kronor dras p&aring; din telefonr&auml;kning, och
                            du kommer att f&aring; en <b>kvittokod</b>. Skriv in den nedan.</p>
                    <p>
                        (Om du bor utanf&ouml;r Sverige, eller har vissa sorters kontantkort, s&aring; kan det h&auml;r vara sv&aring;rt. <a href="http://www2.piratpartiet.se/medlemsavgift">Klicka i s&aring; fall h&auml;r</a> f&ouml;r andra s&auml;tt att betala och bli medlem manuellt.)</p>
                    <br />
                    <b>Kvittokod f&ouml;r medlemsavgiften:</b> </asp:Literal>
                    <asp:TextBox ID="TextPaymentCode" runat="server" Width="74px" Visible="false" meta:resourcekey="TextPaymentCodeResource1"></asp:TextBox>
                    <br />
                    <br />
                    <asp:Label ID="LabelPaymentError" runat="server" CssClass="ErrorMessage" meta:resourcekey="LabelPaymentErrorResource1"></asp:Label>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep3" meta:resourcekey="WizardStep3Resource1">
                    <p>
                        <asp:Literal ID="Literal26" runat="server">
                        Osoite, puhelinnumero ja sukupuoli ovat vapaaehtoisia kenttiä, mutta helpottavat tarvittaessa yhteydenottoa ja tilastojen keräämistä.
</strong></asp:Literal>
                        <asp:Label ID="LabelOrganization" runat="server" Visible="false" Text="orgnamn" meta:resourcekey="LabelOrganizationResource1"></asp:Label>:</p>
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
                                <asp:Label ID="LabelNameError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Namn" meta:resourcekey="LabelNameErrorResource1"></asp:Label>
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
                                <asp:Label ID="LabelStreetError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Gata" meta:resourcekey="LabelStreetErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal7" runat="server" meta:resourcekey="Literal7Resource1">Postnr, ort</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextPostal" runat="server" Columns="3" meta:resourcekey="TextPostalResource1" AutoPostBack="True" OnTextChanged="TextPostal_TextChanged"></asp:TextBox>&nbsp;<asp:TextBox ID="TextCity" runat="server" Columns="20" meta:resourcekey="TextCityResource1" AutoPostBack="True" OnTextChanged="TextCity_TextChanged"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelPostalError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Postnr" meta:resourcekey="LabelPostalErrorResource2"></asp:Label>
                                <asp:Label ID="LabelCityError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Ort" meta:resourcekey="Label1Resource1"></asp:Label>
                                <asp:Label ID="LabelPostalErrorUnknown" runat="server" CssClass="ErrorMessage" Text="Okänt Postnr" Visible="False" meta:resourcekey="LabelPostalErrorUnknownResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="LabelKommun" runat="server" Text="Kommun" Visible="False" meta:resourcekey="LabelKommunResource1"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownKommun" runat="server" OnSelectedIndexChanged="DropDownKommun_SelectedIndexChanged" Visible="False" meta:resourcekey="DropDownKommunResource1">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="LabelKommunError" runat="server" CssClass="ErrorMessage" Text="Välj kommun" Visible="False" meta:resourcekey="LabelKommunErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal8" runat="server" meta:resourceKey="Literal8Resource1" Text="Land"></asp:Literal>
                                :
                            </td>
                            <td colspan="2">
                                <asp:DropDownList ID="DropCountries" runat="server" meta:resourceKey="DropCountriesResource1" AutoPostBack="True" OnSelectedIndexChanged="DropCountries_SelectedIndexChanged">
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
                                <asp:Label ID="LabelPhoneError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Telefon" meta:resourcekey="LabelPhoneErrorResource1"></asp:Label>
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
                                <asp:Label ID="LabelEmailError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Email" meta:resourcekey="LabelEmailErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal11" runat="server" meta:resourcekey="Literal11Resource1">F&ouml;delsedatum</asp:Literal>
                                :&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="TextBirthDay" runat="server" Columns="1" meta:resourcekey="TextBirthDayResource1"></asp:TextBox>&nbsp;
                                <asp:DropDownList ID="DropBirthMonths" runat="server" meta:resourcekey="DropBirthMonthsResource1">
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
                                <asp:Label ID="LabelBirthdateError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Felaktigt datum" meta:resourcekey="LabelBirthdateErrorResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <span style="font-size: 80%">("21
                                    <asp:Label ID="LabelMonthExample" runat="server" Text="Label">februari</asp:Label>
                                    1982")</span>
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
                                <asp:Label ID="LabelGenderError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Välj ett" meta:resourcekey="LabelGenderErrorResource1"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep4" meta:resourcekey="WizardStep4Resource1">
                    <p>
                        <asp:Literal ID="Literal13" runat="server" meta:resourcekey="Literal13Resource1"> Nu &auml;r vi n&auml;stan klara.</asp:Literal>
                    </p>
                    <p>
                        <asp:Literal ID="Literal14" runat="server" meta:resourcekey="Literal14Resource1"> <b>V&auml;lj det l&ouml;senord</b> du
                        vill ha n&auml;r du loggar p&aring; medlemssystemet</asp:Literal>
                        <b>PirateWeb</b>,
                        <asp:Literal ID="Literal15" runat="server" meta:resourcekey="Literal15Resource1">t.ex. f&ouml;r att uppdatera dina medlemsuppgifter eller best&auml;lla
                        nyhetsbrev.</asp:Literal>
                    </p>
                    <br />
                    <table>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal16" runat="server" meta:resourcekey="Literal16Resource1">V&auml;lj ett l&ouml;senord</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextPassword1" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal17" runat="server" meta:resourcekey="Literal17Resource1">Skriv samma igen</asp:Literal>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="TextPassword2" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Label ID="LabelPasswordError" runat="server" CssClass="ErrorMessage" Visible="false" Text="Skriv Password" meta:resourcekey="LabelPasswordErrorResource2"></asp:Label>
                    <asp:Label ID="LabelPasswordErrorSame" runat="server" CssClass="ErrorMessage" Text="Bägge måste vara lika" Visible="False" meta:resourcekey="LabelPasswordErrorSameResource1"></asp:Label>
                    <asp:Label ID="LabelPasswordErrorLength" runat="server" CssClass="ErrorMessage" Text="Minst 5 tecken" Visible="False" meta:resourcekey="LabelPasswordErrorLengthResource1"></asp:Label>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep5" meta:resourcekey="WizardStep5Resource1">
                    <p>
                        <asp:Literal ID="Literal18" runat="server" meta:resourcekey="Literal18Resource1">Till sist, <b>hur aktiv</b> vill du vara i piratrörelsen?</asp:Literal>
                    </p>
                    <table>
                        <tr>
                            <td valign="top" width="120px">
                                <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityPassive" Text="Passiv" Checked="true" meta:resourcekey="RadioActivityPassiveResource1" />
                            </td>
                            <td>
                                <asp:Literal ID="Literal19" runat="server" meta:resourcekey="Literal19Resource1">Du får nyhetsbrev, lokala utskick och kalleleser till möten, men inte information
                            om blixtaktioner och liknande.</asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" width="120px">
                                <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityActivist" Text="Aktivist" meta:resourcekey="RadioActivityActivistResource1" />
                            </td>
                            <td>
                                <asp:Literal ID="Literal20" runat="server" meta:resourcekey="Literal20Resource1">När partiet gör blixtaktioner och valrörelse, så får du information om
                            det (utan något som helst krav på deltagande) via mail eller SMS. Det här är en
                            indikation på att du är intresserad av att vara aktiv medlem.</asp:Literal>
                            </td>
                        </tr>
                        <tr runat="server" visible="false" id="trOfficer">
                            <td valign="top" width="120px">
                                <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityOfficer" Text="Funktionär" meta:resourcekey="RadioActivityOfficerResource1" />&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:Literal ID="Literal21" runat="server" meta:resourcekey="Literal21Resource1">Om du vill bli funktionär fullt ut och
                                ta ansvar för en del av arbetet i din kommun, eller rentav leda arbetet i din kommun,
                                så är det mycket välkommet.</asp:Literal>
                            </td>
                        </tr>
                    </table>
                    <br />
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="WizardStep0" meta:resourcekey="WizardStepResource1">
                    <div style="width: 100%; text-align: center; vertical-align: middle;">
                        <br />
                        <asp:Label ID="LabelBecomeMember" runat="server" Font-Bold="True" Font-Size="X-Large" meta:resourcekey="LabelBecomeMemberResource1">Bli medlem i Piratpartiet <br />eller Ung Pirat</asp:Label>
                        <br />
                        <br />
                        <br />
                        <asp:Label ID="LabelStartRegister" runat="server" meta:resourcekey="LabelStartRegisterResource1">Börja medlemsregistrering genom<br />att trycka på 'Fortsätt >>'</asp:Label></div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" ID="WizardStep6" meta:resourcekey="WizardStep6Resource2">
                    <p>
                        <b>
                            <asp:Literal ID="Literal22" runat="server" meta:resourcekey="Literal22Resource1">V&auml;lkommen som medlem!</asp:Literal>
                        </b>
                    </p>
                    <p>
                        <asp:Literal ID="Literal23" runat="server" meta:resourcekey="Literal23Resource1">Du &auml;r nu medlem i</asp:Literal>
                        :<br />
                        <asp:Label ID="LabelMemberOfOrganizations" runat="server" Text="Label" meta:resourcekey="LabelMemberOfOrganizationsResource1"></asp:Label></p>
                    <p>
                        <asp:Literal ID="Literal24" runat="server" meta:resourcekey="Literal24Resource1">Du kommer strax att f&aring; e-mail som ber&auml;ttar mer. Under tiden kan du v&auml;lja
                        att g&aring; till n&aring;got av f&ouml;ljande st&auml;llen</asp:Literal>
                        :</p>
                    <br />
                    Piraattipuolueen kotisivut (<a href="http://http://www.piraattipuolue.fi">http://www.piraattipuolue.fi</a>)
                    <br />
                    Piraattipuolueen blogi (<a href="https://blog.piraattipuolue.fi">https://blog.piraattipuolue.fi</a>)
                    <br />
                    Piraattipuolueen keskustelupalsta (<a href="http://www.piraattipuolue.fi/keskustelupalsta">http://www.piraattipuolue.fi/keskustelupalsta</a>)
                    <asp:Panel ID="PanelRepeatLink" runat="server" meta:resourcekey="PanelRepeatLinkResource1">
                        <br />
                        tai<br />
                        <asp:HyperLink ID="RepeatLink" runat="server" meta:resourcekey="RepeatLinkResource2">Registrera ytterligare ny medlem</asp:HyperLink><br />
                    </asp:Panel>
                    <p>
                        <asp:Literal ID="Literal25" runat="server" meta:resourcekey="Literal25Resource1">Om du inte f&aring;tt dina mail inom ett par timmar, beror det oftast p&aring; &ouml;veraggressiva
                        spamskydd. Kontakta oss d&aring; p&aring; medlemsservice@piratpartiet.se, s&aring; kan vi avhj&auml;lpa situationen.
                        </asp:Literal>
                    </p>
                </asp:WizardStep>
            </WizardSteps>
            <FinishPreviousButtonStyle CssClass="Invisible" />
            <StepStyle VerticalAlign="Top" />
            <StepNavigationTemplate>
                <asp:Button ID="ButtonAbort" runat="server" CausesValidation="False" CommandName="Cancel" Text="Avbryt" UseSubmitBehavior="False" OnClick="ButtonAbort_Click" meta:resourcekey="ButtonAbortResource1" />
                <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious" Text="&lt;&lt; Backa" UseSubmitBehavior="False" meta:resourcekey="StepPreviousButtonResource1" />
                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Forts&auml;tt &gt;&gt;" meta:resourcekey="StepNextButtonResource1" />
            </StepNavigationTemplate>
            <FinishNavigationTemplate>
                <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Klart!" meta:resourcekey="FinishButtonResource1" />
            </FinishNavigationTemplate>
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Forts&#228;tt &gt;&gt;" meta:resourcekey="StartNextButtonResource1" />
            </StartNavigationTemplate>
        </asp:Wizard>
    </div>
    <asp:Label ID="LabelReferrer" runat="server" Visible="False" meta:resourcekey="LabelReferrerResource1" />
    </form>
</body>
</html>
