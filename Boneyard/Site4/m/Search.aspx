<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="m_Pages_Members_Search"
    CodePage="28591" %>

<%@ Register Src="~/Controls/v3/mPersonList.ascx" TagName="PersonList" TagPrefix="uc1" %>
<?xml version="1.0" encoding="iso-8859-1" ?>
<!DOCTYPE html PUBLIC "-//WAPFORUM//DTD XHTML Mobile 1.0//EN" "http://www.wapforum.org/DTD/xhtml-mobile10.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Style-Type" content="text/css" />
    <meta name="viewport" content="width=device-width,user-scalable=no" />
    <title>PirateWeb Search Members</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel runat="server" DefaultButton="ButtonSearch">
        <asp:Label ID="labelNamePattern" runat="server" Text="labelNamePattern"></asp:Label>
        <br />
        <asp:TextBox ID="TextNamePattern" runat="server">
        </asp:TextBox>
        <br />
        <asp:Label ID="labelEmailPattern" runat="server" Text="labelEmailPattern"></asp:Label>
        <br />
        <asp:TextBox ID="TextEmailPattern" runat="server">
        </asp:TextBox>
        <br />
        <%--<asp:Label ID="LabelBirthDate" runat="server" Text="labelBirthDate"></asp:Label>
        <br />
        <asp:TextBox ID="textPersonalNumber" runat="server">
        </asp:TextBox>
        <br /> --%>
        <asp:Label ID="labelMemberNumber" runat="server" Text="labelMemberNumber"></asp:Label>
        <br />
        <asp:TextBox ID="TextMemberNumber" runat="server">
        </asp:TextBox>
        <br />
        <asp:Button ID="ButtonSearch" runat="server" Text="Search" OnClick="ButtonSearch_Click" />&nbsp;<asp:CheckBox ID="CheckBoxRemember" runat="server" Text="remember" />
        <br />
        <a href="Default.aspx">Go to List by area</a>
    </asp:Panel>
    <hr /><uc1:PersonList ID="PersonList" runat="server" />
    <br />
    <asp:Label ID="labelSearchHints" runat="server" Text="labelSearchHints"></asp:Label>
    </form>
</body>
</html>
