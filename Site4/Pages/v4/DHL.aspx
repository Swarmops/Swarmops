<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="DHL.aspx.cs" Inherits="Pages_v4_ActivePirate_DHL" Title="PirateWeb - DHL" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <asp:Label ID="labelWasSent" runat="server" Font-Size="Large" Text="Din begäran har skickats."
            Visible="False"></asp:Label>
        <asp:Wizard ID="Wizard1" runat="server" BackColor="#F8E7FF" BorderColor="#B5C7DE"
            BorderWidth="1px" Font-Names="Verdana" Font-Size="1em" CellPadding="3" CellSpacing="5"
            StartNextButtonText="Starta" StepNextButtonText="Nästa" StepPreviousButtonText="Backa"
            ActiveStepIndex="0" FinishCompleteButtonText="Skicka Begäran" FinishDestinationPageUrl="DHL.aspx?Steps=Done"
            FinishPreviousButtonText="Backa" Width="100%" OnFinishButtonClick="Wizard1_FinishButtonClick"
            OnNextButtonClick="Wizard1_NextButtonClick" OnSideBarButtonClick="Wizard1_NextButtonClick">
            <StepStyle Font-Size="1em" ForeColor="#333333" Width="100%" />
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="Inledning">
                    <b>Här förbokar du frakt som inte är alltför brådskande, fraktsedlar som ska sitta på
                        varje paket kommer med posten inom ett par dagar. </b>
                    <br />
                    <br />
                    Fyll i formuläret nedan för att ge oss uppgifterna till en sändning. Frakthandlingar
                    kommer sedan skrivas ut och skickas till dig med posten inom ett par dagar. För
                    frakt som betalas av partiet använder vi generellt samfakturering, i övrigt kan
                    du välja att få fraktkostnaden på en faktura som skickas till dig. Beroende på fraktbolagens
                    rutiner kan fakturan dröja 7-30 dagar.
                    <br />
                    <br />
                    Frågor besvaras av jonatan.kindh@piratpartiet.se (Tel: 0731-56 39 35)<br />
                    <br />
                    Tillsammans med frakthandlingar finns också den webadress och de uppgifter du behöver
                    för att via nätet boka upphämtning av DHL/Posten.<br />
                    <br />
                    Fraktsedlar skickas till den avsändaradress som anges.
                    <br />
                    <br />
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Fraktsätt">
                    <h3>
                        Fraktsätt<br />
                    </h3>
                    När du skickar saker (t.ex. kampanjmaterial eller valsedlar) för Piratpartiet eller
                    Ung Pirat bör vi såklart använda det billigaste fraktsättet är det är möjligt.
                    <br />
                    Paket under 2 kg kan oftast skickas som stort brev från ditt närmaste postombud.
                    När transporten behöver göras med paket bör följande prioritering användas, prisskillnaden
                    är stor och den första är billigast<br />
                    &nbsp;<asp:RadioButtonList ID="Typ" runat="server">
                        <asp:ListItem Value="Paket" Selected="True">
1 .DHL Paket - DHL hämtar 
        dagtid hos avsändaren och lämnar dagtid hos mottagaren utan föregående avisering 
        (upphämtning bokas senast kl 12 samma dag som upphämtning ska ske, utkörningstid 
        kan inte styras).            </asp:ListItem>
                        <asp:ListItem Value="Servicepoint">2. DHL Servicepoint - DHL hämtar dagtid och 
                        levererar till en servicepoint nära mottagaren, avisering via sms eller e-post. 
                        (upphämtning bokas senast kl 12 samma dag som upphämtning ska ske)            </asp:ListItem>
                        <asp:ListItem Value="Postpaket">3. Posten Postpaket - 5% rabatt på ordinarie 
                        priser, men dyrare än DHL. Paketet kan hämtas av posten eller lämnas hos 
                        valfritt ombud, levereras till ett ombud nära mottagaren, aviseras via e-post, 
                        sms eller brev (mot avgift).
                        </asp:ListItem>
                    </asp:RadioButtonList>
                    <h3>
                        Avisering</h3>
                    <asp:RadioButtonList ID="Avisering" runat="server">
                        <asp:ListItem Value="Ingen" Selected="True">Ingen avisering, DHL Paket</asp:ListItem>
                        <asp:ListItem Value="SMS">SMS</asp:ListItem>
                        <asp:ListItem Value="epost">e-post</asp:ListItem>
                        <asp:ListItem Value="Brev">Brev, endast posten, avgift 7 kr.</asp:ListItem>
                    </asp:RadioButtonList>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Avsändare">
                    <h3>
                        Avsändare</h3>
                    <table style="width: 100%;" runat="server">
                        <tr>
                            <td style="width: 130px">
                                Organisation:
                            </td>
                            <td colspan="2">
                                <asp:DropDownList ID="ddOrganisation" runat="server">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 130px">
                                &nbsp; c/o (Ditt namn):
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="avsNamn" runat="server" Width="283px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 130px">
                                Adress:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="avsAddr" runat="server" Width="283px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 130px">
                                Postnr, ort:
                            </td>
                            <td style="width: 53px">
                                <asp:TextBox ID="avsPostCode" runat="server" Columns="5"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="avsPostCity" runat="server" Width="228px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 130px">
                                Telefon:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="avsPhone" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 130px">
                                e-post:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="avsEmail" runat="server" Width="283px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="Fakturering">
                    <h3>
                        Fakturering:</h3>
                    <table id="Table1" style="width: 100%;" runat="server">
                        <tr>
                            <td colspan="2">
                                &nbsp;<asp:RadioButtonList ID="fakturering" runat="server">
                                    <asp:ListItem Value="Samfakt." Selected="True">Samfaktureras centralt till PP</asp:ListItem>
                                    <asp:ListItem Value="Separatfakt.">Fakureras avsändaren på separat faktura</asp:ListItem>
                                </asp:RadioButtonList>
                                <br />
                                Om samfakturerat, ange vems budget som ska belastas, dvs distrikt, valkrets etc.
                                <br />
                                <asp:TextBox ID="budget" runat="server" Width="283px"></asp:TextBox>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep5" runat="server" Title="Mottagare">
                    <h3>
                        Mottagare</h3>
                    <table id="Table2" style="width: 100%; vertical-align: top" runat="server">
                        <tr>
                            <td style="width: 121px">
                                Namn:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="mottNamn" runat="server" Width="283px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 121px">
                                Adress:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="mottAdress1" runat="server" Width="283px"></asp:TextBox><br />
                                <asp:TextBox ID="mottAdress2" runat="server" Width="283px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 121px">
                                Postnr, ort:
                            </td>
                            <td style="width: 44px">
                                <asp:TextBox ID="mottPostCode" runat="server" Columns="5"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="mottCity" runat="server" Width="235px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 121px">
                                Telefon:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="mottPhone" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 121px">
                                Portkod<br />
                                (vid utkörning):
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="mottCode" runat="server" Width="126px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 121px">
                                e-post:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="mottEMail" runat="server" Width="283px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep6" runat="server" Title="Leveransanvisning">
                    <h3>
                        Om leveransen</h3>
                    <table id="Table3" style="width: 100%; vertical-align: top" runat="server">
                        <tr>
                            <td colspan="2">
                                Ev: leveransanvisning (sånt föraren kan behöva veta vid utkörning):<br />
                                <asp:TextBox ID="leveransAnv" runat="server" Rows="5" TextMode="MultiLine" Width="397px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 263px">
                                Antal kollin:
                            </td>
                            <td>
                                <asp:TextBox ID="antKolli" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 263px">
                                Kollivikt i kg, skilj värden med / om det är flera olika:
                            </td>
                            <td>
                                <asp:TextBox ID="vikt" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 263px">
                                Innehåll:
                            </td>
                            <td>
                                <asp:TextBox ID="innehDekl" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep7" runat="server" Title="Övr. Uppl.">
                    Övriga upplysningar (till den som fixar fram fraktsedlarna):<br />
                    <asp:TextBox ID="ovrUppl" runat="server" Rows="6" TextMode="MultiLine" Width="56%"></asp:TextBox>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep8" runat="server" Title="Sammanställning">
                    <div id="SummaryPanel" runat="server">
                    </div>
                </asp:WizardStep>
            </WizardSteps>
            <SideBarButtonStyle Font-Names="Verdana" ForeColor="White" />
            <NavigationButtonStyle BackColor="White" BorderColor="#507CD1" BorderStyle="Solid"
                BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284E98" />
            <SideBarStyle BackColor="#612C7C" Font-Size="0.9em" VerticalAlign="Top" Width="100px" />
            <HeaderStyle BackColor="#284E98" BorderColor="#EFF3FB" BorderStyle="Solid" BorderWidth="2px"
                Font-Bold="True" Font-Size="0.9em" ForeColor="White" HorizontalAlign="Center" />
            <StepNavigationTemplate>
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <asp:Label ID="ErrorMsgLabel" runat="server" Text="" ForeColor="Red" Font-Bold="true"></asp:Label>
                        </td>
                        <td>
                            <asp:Button ID="StepPreviousButton" runat="server" BackColor="White" BorderColor="#507CD1"
                                BorderStyle="Solid" BorderWidth="1px" CausesValidation="False" CommandName="MovePrevious"
                                Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284E98" Text="Backa" />
                            <asp:Button ID="StepNextButton" runat="server" BackColor="White" BorderColor="#507CD1"
                                BorderStyle="Solid" BorderWidth="1px" CommandName="MoveNext" Font-Names="Verdana"
                                Font-Size="0.8em" ForeColor="#284E98" Text="Nästa" />
                        </td>
                    </tr>
                </table>
            </StepNavigationTemplate>
        </asp:Wizard>
    </div>
</asp:Content>
