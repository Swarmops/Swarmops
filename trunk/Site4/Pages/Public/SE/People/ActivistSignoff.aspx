<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ActivistSignoff.aspx.cs" ViewStateEncryptionMode="Always"
    Inherits="Pages_Public_SE_People_ActivistSignoff" MasterPageFile="~/PirateWeb-v4-menuless.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title id="pageTitle" runat="server">Avsluta aktiviststatus.</title>

    <script type="text/javascript">

    </script>

    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div style="padding-left: 7%">
        <h1 id="h1Caption" runat="server">
            Avsluta aktiviststatus</h1>
        <asp:Wizard ID="Wizard1" runat="server" StepStyle-HorizontalAlign="NotSet" 
            ActiveStepIndex="0" BorderStyle=None 
            DisplaySideBar="False"  Width="600px" Height="146px" OnNextButtonClick="Wizard1_NextButtonClick"
            OnFinishButtonClick="Wizard1_FinishButtonClick">
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Nästa steg" />
            </StartNavigationTemplate>
            <WizardSteps>
                <asp:WizardStep ID="WizardStep0" runat="server" Title="Begär kod för återställning.">
                    <table >
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="LabelExplanation" runat="server">
                                Du har tidigare tecknat dig för att vara Piratpartiaktivist, dvs. att få snabba meddelanden via SMS och mail när det händer något där det behövs aktivisthjälp.
                                <br />
                                <br />
                                Om du inte längre vill ha dessa specialmeddelanden så kan du på den här sidan avsluta din aktiviststatus.
                                <br />
                                <br />
                                Genom att ange den e-mailadress du uppgav när du registrerade dig, så får du ett mail med en länk du kan använda för att göra denna ändring.
                                </asp:Label>
                                <br />
                            </td>
                            <td>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="LabelEmail" runat="server">e-mail:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBoxEmail" runat="server" Width="315px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelErrEmail" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="Kod skickad.">
                    <asp:Label ID="LabelCodeHasBeenSent" runat="server">
                Om adressen finns i vårt register så skickas nu ett mail med en kod för att återställa lösenord.<br /><br />
                Vänta in mailet med din kod.<br /><br />
                När det kommer så innehåller det en länk som du kan klicka på för att komma vidare, 
                eller, om det inte skulle fungera, så kan du klicka på knappen nedan och fylla i 
                koden från mailet manuellt.
                    </asp:Label>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Ange avslutningskod.">
                    <table >
                        <tr>
                            <td>
                                <asp:Label ID="LabelGiveCode" runat="server" Text="Label">
                            Ange koden du fick i e-mailet:
                                </asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="TextBoxCode" runat="server" Width="229px"></asp:TextBox>
                                <asp:Label ID="LabelErrCode" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
         
                <asp:WizardStep ID="WizardStep4" runat="server" Title="Klart!">
                    <asp:Label ID="Label1" runat="server" Text="Label">
SÅ, nu är du inte längre aktivist. När du får tid och lust igen så är du välkommen tillbaka!
                    </asp:Label>
                </asp:WizardStep>
            </WizardSteps>
            <FinishNavigationTemplate>
            </FinishNavigationTemplate>
            <StepNavigationTemplate>
                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Nästa steg" />
            </StepNavigationTemplate>
        </asp:Wizard>
        <br />
    </div>
</asp:Content>
