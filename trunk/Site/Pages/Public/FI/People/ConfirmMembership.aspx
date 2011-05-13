<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfirmMembership.aspx.cs"
    Inherits="Pages_Public_FI_People_ConfirmMembership" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            font-family: Arial, Helvetica, sans-serif;
            font-size: medium;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="Div1" class="style1" visible="false" runat="server">
        Sorry, that code was not valid. Please contact us <a href="https://www.piraattipuolue.fi/puolue/yhteystiedot">
            https://www.piraattipuolue.fi/puolue/yhteystiedot</a> to solve the problem.</div>
    <div id="Div2" class="style1" visible="true" runat="server">
        Your membership is confirmed, please wait for redirection...</div>
    </form>
</body>
</html>
