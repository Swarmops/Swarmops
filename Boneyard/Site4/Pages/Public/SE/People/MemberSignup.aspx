<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" AutoEventWireup="true" CodeFile="MemberSignup.aspx.cs" Inherits="Pages_Public_SE_People_MemberSignup" Title="Bli medlem i Piratpartiet Sverige" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

        <telerik:RadAjaxManager ID="RadAjaxManager3" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1"
            UpdatePanelsRenderMode="Inline" meta:resourcekey="RadAjaxManager3Resource1">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="TextPostalCode">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextPostalCode" />
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelGeographies" />
                        <telerik:AjaxUpdatedControl ControlID="DropGeographies" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="TextCity">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextPostal" />
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelGeographies" />
                        <telerik:AjaxUpdatedControl ControlID="DropGeographies" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="DropCountries">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="TextCity" />
                        <telerik:AjaxUpdatedControl ControlID="LabelGeographies" />
                        <telerik:AjaxUpdatedControl ControlID="DropGeographies" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" Skin="WebBlue" runat="server"
            meta:resourcekey="RadAjaxLoadingPanel1Resource1">
        </telerik:RadAjaxLoadingPanel>


    <div class="DivMainContent">

    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Välkommen som medlem!</span><br />
    <div class="DivGroupBoxContents">
            <asp:Label runat="server" ID="Label1"><span class="Continuous">Att vara medlem i Piratpartiet innebär att du blir en del av Sveriges kraftigaste politiska röst för integritet, kultur och kunskap. <b>Medlemskapet är kostnadsfritt</b>, men vi uppmuntrar till frivilliga donationer vare sig du är medlem eller inte. Mer om det mot slutet av processen.</span></asp:Label>
    </div>
    
    </div>
    <div class="DivGroupBox" style="margin-top: 25px; width: 800px; height:320px; margin-left: auto; margin-right: auto">
    <span class="DivGroupBoxTitle">Fyll i...</span><br />
    <div class="DivGroupBoxContents" style="position:relative; top:-30px;">

        <asp:Wizard ID="Wizard" runat="server" ActiveStepIndex="0" DisplaySideBar="false" StartNextButtonText="Nästa >>" StepPreviousButtonText="<< Backa" StepNextButtonText="Nästa >>" FinishCompleteButtonText="Klar >>" FinishPreviousButtonText="<< Backa" OnFinishButtonClick="WizardNext_Click" OnNextButtonClick="WizardNext_Click">
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 1">
                    <div style="height:260px;width:100%">
                        <h2 class="Continuous">Steg 1 av 7: Vad heter du, och var bor du?</h2>
                        <div style="float:left">För- och efternamn&nbsp;&nbsp;<br />Gata&nbsp;&nbsp;<br />Postnummer,&nbsp;ort&nbsp;&nbsp;<br /><asp:Label ID="LabelGeographies" runat="server" Text="Kommun" />&nbsp;&nbsp;<br />Land&nbsp;&nbsp;</div>
                        <asp:TextBox ID="TextFirstName" runat="server" />&nbsp;<asp:TextBox ID="TextLastName" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_FirstName" runat="server" Text="Skriv ditt förnamn. " Display="Dynamic" ControlToValidate="TextFirstName" SetFocusOnError="true" /><asp:RequiredFieldValidator ID="Validator_LastName" runat="server" Text="Skriv ditt efternamn." Display="Dynamic" ControlToValidate="TextLastName" SetFocusOnError="true" /><br />
                        <asp:TextBox ID="TextStreet" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_Street" runat="server" Text="Skriv din gatuadress. " Display="Dynamic" ControlToValidate="TextStreet" SetFocusOnError="true" /><br />
                        <asp:TextBox ID="TextPostalCode" runat="server" OnTextChanged="TextPostalCode_Changed" AutoPostBack="true" />&nbsp;<asp:TextBox ID="TextCity" runat="server"/>&nbsp;<asp:RequiredFieldValidator ID="Validator_PostalCode" runat="server" Text="Skriv ditt postnummer. " Display="Dynamic" ControlToValidate="TextPostalCode" SetFocusOnError="true" /><asp:RequiredFieldValidator ID="Validator_City" runat="server" Text="Skriv din postort. " Display="Dynamic" ControlToValidate="TextCity" SetFocusOnError="true" /><br />
                        <asp:DropDownList ID="DropGeographies" runat="server" />&nbsp;<br />
                        <asp:DropDownList ID="DropCountries" runat="server" />&nbsp;<br /><br />
                        <span class="Continuous">Dina personuppgifter lagras i ett medlemsregister som hanteras av Piratpartiets funktionärer där du bor och av systemets administratörer, som kan vara aktiva i piratpartier i andra länder. Normalt anonymiseras dina uppgifter tre månader efter ditt medlemskap gått ut. Undantaget är om du varit inblandad i partiets ekonomi, då måste vi spara uppgifterna i tio år för Skatteverkets räkning.</span><br />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2">
                    <div style="height:260px;width:100%">
                        <h2>Steg 2 av 7: Lite statistiska uppgifter</h2>
                        <div style="float:left">Födelsedatum:&nbsp;&nbsp;<br />Jag är...</div>
                        <asp:DropDownList ID="DropBirthYear" runat="server" />&nbsp;&ndash;&nbsp;<asp:DropDownList ID="DropBirthMonth" runat="server" />&nbsp;&ndash;&nbsp;<asp:DropDownList ID="DropBirthDay" runat="server" />&nbsp;<asp:CustomValidator ID="Validate_Birthdate" runat="server" OnServerValidate="Validate_Birthdate_ServerValidate" Display="Dynamic" Text="Ange ett giltigt datum." ControlToValidate="DropBirthYear" /><br />
                        <asp:DropDownList ID="DropGenders" runat="server">
                            <asp:ListItem Selected="True" Value="0" Text=" V&#228;lj en..." meta:resourcekey="ListItemResource13"></asp:ListItem>
                            <asp:ListItem Value="Male" Text="Man" meta:resourcekey="ListItemResource14"></asp:ListItem>
                            <asp:ListItem Value="Female" Text="Kvinna" meta:resourcekey="ListItemResource15"></asp:ListItem>
                        </asp:DropDownList>&nbsp;<asp:CompareValidator ID="Validator_DropGenders" runat="server" Display="Dynamic" Operator="NotEqual" ValueToCompare="0" ControlToValidate="DropGenders" Text="Ange ditt kön." EnableClientScript="false" /><br /><br />

                        <span class="Continuous">Detta frågar vi efter för att få statistik över vår medlemskår. Statistiken är offentlig och har publicerats i flera ansedda tidningar, likväl som på Wikipedia.</span><br />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Step 3">
                    <div style="height:260px;width:100%">
                        <h2>Steg 3 av 7: Hur kontaktar vi dig?</h2>
                        <div style="float:left">Email:&nbsp;&nbsp;<br />Email,&nbsp;igen:&nbsp;&nbsp;<br />(Mobil)telefon:&nbsp;&nbsp;</div>
                        <asp:TextBox ID="TextEmail" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_Email" runat="server" ControlToValidate="TextEmail" Display="Dynamic" Text="Ange din mailadress." /><br />
                        <asp:TextBox ID="TextEmailRepeat" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_EmailRepeat" runat="server" ControlToValidate="TextEmailRepeat" Display="Dynamic" Text="Repetera din mailadress." /><asp:CompareValidator ID="Validator_CompareEmails" runat="server" ControlToValidate="TextEmailRepeat" ControlToCompare="TextEmail" Operator="Equal" Text="Mailadresserna är olika!" Display="Dynamic" /><br />
                        <asp:TextBox ID="TextPhone" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_Phone" runat="server" ControlToValidate="TextPhone" Display="Dynamic" Text="Ange ditt mobilnummer." /><br /><br />

                        <span class="Continuous">Detta frågar vi efter för att kunna kontakta dig manuellt, men också för att kunna skicka ut nyhetsbrev och mail eller SMS om lokala aktiviter. Du kan ställa in vad du vill ha senare. Som standard är mailskickande på och SMS-skickande av.</span><br />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="Step 4">
                    <div style="height:260px;width:100%">
                        <h2>Steg 4 av 7: Vad vill du ha för lösen till medlemssystemet PirateWeb?</h2>
                        <div style="float:left">Välj ett lösen:&nbsp;&nbsp;<br />Samma&nbsp;igen:&nbsp;&nbsp;</div>
                        <asp:TextBox ID="TextPassword" TextMode="Password" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_Password" runat="server" ControlToValidate="TextPassword" Display="Dynamic" Text="Välj ett lösen." /><br />
                        <asp:TextBox ID="TextPasswordRepeat" runat="server" TextMode="Password" />&nbsp;<asp:RequiredFieldValidator ID="Validator_RepeatPassword" runat="server" ControlToValidate="TextPasswordRepeat" Display="Dynamic" Text="Repetera ditt lösen." /><asp:CompareValidator ID="Validator_ComparePasswords" runat="server" ControlToValidate="TextPasswordRepeat" ControlToCompare="TextPassword" Operator="Equal" Text="Lösenorden är olika!" Display="Dynamic" /><br /><br />

                        <span class="Continuous">Detta använder du för att logga på medlemssystemet, där du t.ex. begär ersättning för utlägg eller bestämmer vilka nyhetsbrev du vill ha, eller om du vill se hur din lokala organisation ser ut.</span><br />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep5" runat="server" Title="Step 5">
                    <div style="height:260px;width:100%">
                        <h2>Steg 5 av 7: Hur aktiv vill du vara som medlem?</h2>
                        <table>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityPassive"
                                        Text="Passiv" Checked="true" />&nbsp;
                                </td>
                                <td valign="top">
                                    <span class="Continuous">Du får nyhetsbrev, lokala utskick och kalleleser till möten, men inte information
                                    om blixtaktioner och liknande.</span>&nbsp;
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityActivist"
                                        Text="Aktivist" />&nbsp;<br />
                                </td>
                                <td valign="top">
                                <span class="Continuous">När partiet gör blixtaktioner och valrörelse, så får du information om det (utan
                                    något som helst krav på deltagande) via mail och SMS. Det här är en indikation
                                    på att du är intresserad av att vara aktiv medlem.</span><br />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="ActivityLevel" ID="RadioActivityOfficer"
                                        Text="Funktionär" />&nbsp;&nbsp;<br />
                                </td>
                                <td valign="top">
                                    <span class="Continuous">Om du vill bli funktionär fullt ut och ta ansvar för en del av arbetet i din kommun,
                                    eller rentav leda arbetet i din kommun, så är det mycket välkommet.</span><br />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep6" runat="server" Title="Step 6">
                    <div style="height:260px;width:100%">
                        <h2>Steg 6 av 7: Vilken sorts funktionär vill du vara?</h2>
                        <table>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="OfficerType" ID="RadioOfficerTypeCampaign"
                                        Text="Kampanj" />
                                </td>
                                <td valign="top">
                                    Arbetar med och ansvarar för partiets lokala aktivister och kampanjer. Detta är den mest extroverta rollen.<br />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="OfficerType" ID="RadioOfficerTypeMedia"
                                        Text="Media" />
                                </td>
                                <td valign="top">
                                    Arbetar med lokal gammelmedia och nymedia för att exponera partiets aktiviteter och politik.<br />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="OfficerType" ID="RadioOfficerTypeMemberCare"
                                        Text="Medlemsvård" />
                                </td>
                                <td valign="top">
                                    Arbetar med att lokala medlemmar kontaktas, utbildas, tas hand om och vårdas.<br />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="OfficerType" ID="RadioOfficerTypeInformation"
                                        Text="Information" Checked="true" />
                                </td>
                                <td valign="top">
                                    <span class="Continuous">Arbetar med att publicera information så att alla har den information de behöver om det lokala arbetet. Detta är den minst extroverta rollen.</span><br />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top" width="120px">
                                    <asp:RadioButton runat="server" GroupName="OfficerType" ID="RadioManagement"
                                        Text="Ledning" />
                                </td>
                                <td valign="top">
                                    Arbetar med att avlasta lokal ledning för att få helheten i de fyra rollerna att fungera.
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep7" runat="server" Title="Step 7" StepType="Finish">
                    <div style="height:260px;width:100%">
                        <h2>Steg 7 av 7: Ung Pirat</h2>
                        <span class="Continuous">Du är under 26 år och kan därför dessutom bli medlem i ungdomsförbundet Ung Pirat. Kryssa i rutan om du vill bli medlem. Om du inte vill bli medlem är det bara att gå vidare.</span><br /><br />
                        <asp:CheckBox ID="CheckUngPirat" runat="server" Text="Ung Pirat" /><br />
                        <br /><span class="Continuous"><b>Dina personuppgifter</b>: Om du vill bli medlem i ungdomsförbundet kommer Piratpartiet att skicka de personuppgifter du uppgivit till Ung Pirat.</span><br />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStepComplete" runat="server" StepType="Complete" Title="Complete">
                    <div style="height:260px;width:100%">
                        <h2>Klart!</h2>
                        <span class="Continuous">Välkommen som medlem i Piratpartiet! Du kommer strax att få mer information i mail från partiledning och från dina lokala funktionärer. Om du vill ge en frivillig donation till partiet varje månad, så välj ett belopp nedan:</span><br /><br />
                        
                                                
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

	                    <br clear="all"><br />
                        
                        <span class="Continuous">Länkarna går till PayPal som sätter upp månadsdonationer. Du kan när som helst avbryta dem. (Vi vet att PayPal inte är ett okej företag och arbetar på att hitta en annan lösning.)</span>
                    </div>
                </asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>
    
    </div>
    </div>
    </div>

</asp:Content>

