<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RequestNewPassword.aspx.cs" ViewStateEncryptionMode="Always" Inherits="Pages_Public_FI_People_RequestNewPassword" MasterPageFile="~/PirateWeb-v4-menuless.master" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title id="pageTitle" runat="server">Reset password.</title>

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
            Reset password.</h1>
        <asp:Wizard ID="Wizard1" runat="server" StepStyle-HorizontalAlign="NotSet" ActiveStepIndex="3" BorderStyle="None" DisplaySideBar="False" Width="600px" Height="146px" OnNextButtonClick="Wizard1_NextButtonClick" OnFinishButtonClick="Wizard1_FinishButtonClick" meta:resourcekey="Wizard1Resource1">
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Next" meta:resourcekey="StartNextButtonResource1" />
            </StartNavigationTemplate>
            <WizardSteps>
                <asp:WizardStep ID="WizardStep0" runat="server" Title="Request password reset code." meta:resourcekey="WizardStep0Resource1">
                    <table>
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="LabelExplanation" runat="server" meta:resourcekey="LabelExplanationResource1">
                                By entering the code you stated when registering your membership in the box below,
                                you will get a mail with a code that enables you to reset your password for PirateWeb.</asp:Label>
                                <br />
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="LabelEmail" runat="server" meta:resourcekey="LabelEmailResource1">e-mail:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBoxEmail" runat="server" Width="315px" meta:resourcekey="TextBoxEmailResource1"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="LabelErrEmail" runat="server" Font-Bold="True" ForeColor="Red" meta:resourcekey="LabelErrEmailResource1" Visible="false">Invalid e-mail address</asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="Kod skickad." meta:resourcekey="WizardStep1Resource1">
                    <asp:Label ID="LabelCodeHasBeenSent" runat="server" meta:resourcekey="LabelCodeHasBeenSentResource1">
                If the address is found in our database, you will get a mail shortly with a link containing a code.<br /><br />
                Check your inbox for the mail.<br /><br />
                Click on the link in the mail, or press the button below and enter the code manually.
                    </asp:Label>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Enter reset code" meta:resourcekey="WizardStep2Resource1">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="LabelGiveCode" runat="server" Text="Label" meta:resourcekey="LabelGiveCodeResource1">
                            Enter the code you got in the e-mail:
                                </asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="TextBoxCode" runat="server" Width="229px" meta:resourcekey="TextBoxCodeResource1"></asp:TextBox>
                                <asp:Label ID="LabelErrCode" runat="server" Font-Bold="True" ForeColor="Red" meta:resourcekey="LabelErrCodeResource1" Visible="false">Invalid code<br />(Wrong code, too old or already used.)</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep3" runat="server" Title="Enter new password." meta:resourcekey="WizardStep3Resource1">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="LabelStatePassword" runat="server" Text="Label" meta:resourcekey="LabelStatePasswordResource1">
                            Enter your new password:
                                </asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="TextBoxPassword1" runat="server" TextMode="Password" Width="100%" meta:resourcekey="TextBoxPassword1Resource1"></asp:TextBox>
                            </td>
                            <td rowspan="1">
                                &nbsp;&nbsp;
                                <asp:Label ID="LabelErrPassword1" runat="server" Font-Bold="True" ForeColor="Red" meta:resourcekey="LabelErrPassword1Resource1" Visible="false">
                                Minimum 5 characters</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td rowspan="1">
                                <asp:Label ID="LabelStatePassword2" runat="server" Text="Label" meta:resourcekey="LabelStatePassword2Resource1">please repeat:
                                </asp:Label>
                            </td>
                            <td class="style4">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="TextBoxPassword2" runat="server" TextMode="Password" Width="100%" meta:resourcekey="TextBoxPassword2Resource1"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;
                                
                                <asp:Label ID="LabelErrPassword2" runat="server" Font-Bold="True" ForeColor="Red" meta:resourcekey="LabelErrPassword2Resource1" Visible="false">
                                Both must be the same</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep4" runat="server" Title="Done!" meta:resourcekey="WizardStep4Resource1">
                    <asp:Label ID="Label1" runat="server" Text="Label" meta:resourcekey="Label1Resource1">
                Your new password has been set.<br />
                <br />
                Welcome back to PirateWeb!<br />
                    </asp:Label>
                </asp:WizardStep>
            </WizardSteps>
            <FinishNavigationTemplate>
                <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Log in" TabIndex="99" meta:resourcekey="FinishButtonResource1" />
            </FinishNavigationTemplate>
            <StepNavigationTemplate>
                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next step" TabIndex="98" meta:resourcekey="StepNextButtonResource1" />
            </StepNavigationTemplate>
        </asp:Wizard>
        <br />
    </div>
</asp:Content>
