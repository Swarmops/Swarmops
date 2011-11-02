<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestForum.aspx.cs" Inherits="Tests_JL_TestForum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server" Height="180px" TextMode="MultiLine" Width="859px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
    </div>
    <p>
        <asp:TextBox ID="TextBox2" runat="server" Height="180px" TextMode="MultiLine" Width="859px"></asp:TextBox>
        </p>
    </form>
</body>
</html>
