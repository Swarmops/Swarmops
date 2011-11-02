<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Security_Login"
    CodePage="65001" ValidateRequest="false" %>

<%@ Register Src="~/Controls/v3/LanguageSelector.ascx" TagName="LanguageSelector"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" id="pageHeader">
    <title>Pirate Party Administration - Login Page</title>
    <link href="~/Style/PirateWeb-v3.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .body
        {
            text-align: center;
            min-width: 450px;
            background-image: url(  'Login-Background-1.jpg' );
        }
        .wrapper
        {
            margin: 0 auto;
            width: 450px;
            text-align: left;
            background: white;
            border: 2px solid #404040;
            padding: 20px;
        }
        .LanguageSelector
        {
            /* place the language bar at the top right */
            position: relative;
            top: -18px;
            left: 18px;
        }
    </style>
</head>
<body class="body" runat="server" id="Body">
    <form id="form1" runat="server">
    <div class="Header" style="text-align: left; background: white" id="DivHeader" runat="server">
        <div style="float: right; position: relative; top: 6pt; padding-right: 8px">
            Please Login</div>
        <span style="font: 16pt Impact; padding-left: 8px">PIRATEWEB</span></div>
    <div id="wrapper" class="wrapper" style="margin-top: 125px" runat="server">
        <uc1:LanguageSelector ID="LanguageSelector" runat="server" ShowLanguages="sv,en,fr,de,ru,ja,es" />
        <h1 style="margin-top: 4px; margin-bottom: 8px" id="ContentHeader" runat="server">
            <asp:Label ID="LabelWelcomeHeader" runat="server" Text="Label"></asp:Label></h1>
        <p>
            <asp:Label ID="LabelWelcomeParagraph" runat="server" Text="Label"></asp:Label></p>
        <asp:Panel runat="server" DefaultButton="ButtonLogin">
            <table style="margin-top: 5px; margin-bottom: 10px">
                <tr>
                    <td style="padding-left: 0px">
                        <asp:Label ID="LabelLogin" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="TextLogin" runat="server" Columns="30" Width="180px"></asp:TextBox>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:Label ID="LabelInvalidLogin" runat="server" CssClass="ErrorMessage" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: 0px">
                        <asp:Label ID="LabelPassword" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="TextPassword" runat="server" TextMode="Password" Columns="30" Width="180px"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="padding-top: 4px">
                        <asp:Button ID="ButtonLogin" runat="server" Text="Login" OnClick="ButtonLogin_Click" />
                    </td>
                    <td>
                        &nbsp;
                        <asp:HyperLink ID="LinkLosenord" runat="server" Font-Size="Small" NavigateUrl="~/Pages/Public/SE/People/RequestNewPassword.aspx">
                        I forgot my password...
                        </asp:HyperLink>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
