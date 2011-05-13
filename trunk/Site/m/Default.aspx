<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="m_Pages_Members_List"
    CodePage="28591" %>

<%@ Register Src="~/Controls/v3/mPersonList.ascx" TagName="PersonList" TagPrefix="uc1" %>
<?xml version="1.0" encoding="iso-8859-1" ?>
<!DOCTYPE html PUBLIC "-//WAPFORUM//DTD XHTML Mobile 1.0//EN" "http://www.wapforum.org/DTD/xhtml-mobile10.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Style-Type" content="text/css" />
    <meta name="viewport" content="width=device-width,user-scalable=no" />
    <title>PirateWeb List Members/Area</title>
</head>
<body>
    <form id="form1" runat="server">
    Org:<br />
    <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged">
    </asp:DropDownList>
    <br />
    Geography:<br />
    <asp:DropDownList ID="DropGeographies" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropGeographies_SelectedIndexChanged">
    </asp:DropDownList>
    <br />
    <asp:Button ID="ButtonList" runat="server" OnClick="ButtonList_Click" Text="List" />&nbsp;<asp:CheckBox ID="CheckBoxRemember" runat="server" Text="remember" />
    <br /><a href="Search.aspx">Go to Search</a>
    <hr />
    <uc1:PersonList ID="PersonList" runat="server" />
    </form>
</body>
</html>
