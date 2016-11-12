<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ParleySignup.aspx.cs" Inherits="Activizr.Site.Pages.Public.Frames.ParleySignup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Embedded IFrame for registering with conference</title>
</head>
<body style="margin:0;padding:0" ID="BodyTag" runat="server">
    <form id="form1" runat="server">
    <div style="line-height:200%">
        <asp:Label ID="LabelConferenceLabel" runat="server">Signup for </asp:Label><b><asp:Label ID="LabelConference" runat="server" /></b><asp:Label ID="LabelConferenceAdjective" runat="server" Text=":" /><br />
        <asp:Panel ID="PanelSignup" runat="server">
            <div style="float:left; margin-right:15px">Name (first, last):<br />
            Email:<br />
            Email (again):<br />
            Options:<br />
            <asp:Literal ID="LiteralOptionsSpacer" runat="server" />
            Sign up?</div>
            <asp:TextBox ID="TextNameFirst" runat="server" />&nbsp;<asp:TextBox ID="TextNameLast" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_NameFirst" ControlToValidate="TextNameFirst" runat="server" Display="Dynamic" Text="Enter a first name." /> <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="TextNameLast" runat="server" Display="Dynamic" Text="Enter a last name." /><br />
            <asp:TextBox ID="TextEmail" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_Email" ControlToValidate="TextEmail" runat="server" Display="Dynamic" Text="Enter your email." /><br />
            <asp:TextBox ID="TextEmail2" runat="server" />&nbsp;<asp:CompareValidator ID="Validator_EmailMatches" ControlToValidate="TextEmail2" ControlToCompare="TextEmail" Operator="Equal" Text="Email entries do not match." Display="Dynamic" runat="server" /><br />
           <asp:PlaceHolder ID="PlaceholderOptions" runat="server" />
            <asp:Button ID="ButtonSignup" OnClick="ButtonSignup_Click" runat="server" Text="Sign up!" />&nbsp;
        </asp:Panel>
        <asp:Panel ID="PanelSignupCompleted" runat="server" Visible="false">
        <asp:Label ID="LabelSignupCompleted" runat="server">Thank you for signing up. You will receive an email asking you to confirm the sign-up; click the link in that email to confirm. Once confirmed, you will receive an invoice to the same email address. Enjoy the conference!</asp:Label>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
