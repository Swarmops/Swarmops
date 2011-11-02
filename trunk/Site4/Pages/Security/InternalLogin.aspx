<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InternalLogin.aspx.cs" Inherits="Pages_Security_InternalLogin"
    EnableViewState="false" EnableViewStateMac="false" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <asp:Panel ID="LoginPanel" runat="server" Visible="false">
        <form id="form2" action="https://pirateweb.net/Pages/Security/SubsystemLogin.aspx" method="post" enctype="application/x-www-form-urlencoded">
        Userid:
        <input name="username" type="text" /><br />
        Password:
        <input name="password" type="password" /><br />
        <input type="hidden" name="redirect" value="https://pirateweb.net/Pages/Security/InternalLogin.aspx" />
        <input type="submit" value="logon" />
        </form>
    </asp:Panel>
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hTicket" runat="server" />
        <asp:Panel ID="ResultPanel" runat="server" Visible="false">
            <div id="resultdiv" runat="server">
            </div>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
