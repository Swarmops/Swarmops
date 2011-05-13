<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RequestNewPassword.aspx.cs" ViewStateEncryptionMode="Always"
    Inherits="Pages_Public_DK_People_RequestNewPassword" MasterPageFile="~/PirateWeb-v4-menuless.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title id="pageTitle" runat="server">Återställning av lösenord.</title>

    <script type="text/javascript">

    </script>

    <style type="text/css">
        .style4
        {
            height: 26px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div style="padding-left: 7%">
        <h1 id="h1Caption" runat="server">
            Återställning av lösenord.</h1>
        <asp:Wizard ID="Wizard1" runat="server" StepStyle-HorizontalAlign="NotSet" 
            ActiveStepIndex="0" BorderStyle=None 
            DisplaySideBar="False"  Width="600px" Height="146px" OnNextButtonClick="Wizard1_NextButtonClick"
            OnFinishButtonClick="Wizard1_FinishButtonClick"  >
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" 
                    Text="Next" />
            </StartNavigationTemplate>
            <WizardSteps>
                <asp:WizardStep ID="WizardStep0" runat="server" Title="Begär kod för återställning.">
                    <table >
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="LabelExplanation" runat="server">Genom att ange den e-mailadress du uppgav när du registrerade dig som medlem i Piratpartiet här nedan, så får du ett mail med en kod du kan använda för att återställa ditt bortglömda lösenord.</asp:Label>
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
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Ange återställningskod.">
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
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Ange nytt lösenord.">
                    <table >
                        <tr>
                            <td>
                                <asp:Label ID="LabelStatePassword" runat="server" Text="Label">
                            Ange ditt nya lösenord:
                                </asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="TextBoxPassword1" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
                            </td>
                            <td rowspan="1">
                                &nbsp;&nbsp;
                                <asp:Label ID="LabelErrPassword1" runat="server" Font-Bold="True" 
                                    ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td rowspan="1
                            ">
                                <asp:Label ID="LabelStatePassword2" runat="server" Text="Label">repetera:</asp:Label>
                            </td>
                            <td class="style4">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="TextBoxPassword2" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;
                                <asp:Label ID="LabelErrPassword2" runat="server" Font-Bold="True" 
                                    ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="Klart!">
                    <asp:Label ID="Label1" runat="server" Text="Label">
                Ditt nya lösenord är nu satt.<br />
                <br />
                Välkommen in på PirateWeb igen!<br />
                    </asp:Label>
                </asp:WizardStep>
            </WizardSteps>
            <FinishNavigationTemplate>
                <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Logga in" TabIndex=99 />
            </FinishNavigationTemplate>
            <StepNavigationTemplate>
                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Nästa steg" TabIndex=98 />
            </StepNavigationTemplate>
        </asp:Wizard>
        <br />
    </div>
</asp:Content>
