<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DisplayInternalPollVoters.aspx.cs" Inherits="Data_DisplayInternalPollVoters" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Page title is set in Page_Load</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <pre>
Voters in the <asp:Label ID="LabelPollName" runat="server" /> poll (initials and MemberId):
<asp:Repeater ID="RepeaterVoters" runat="server"><ItemTemplate>
<%# Container.DataItem %></ItemTemplate></asp:Repeater>

<asp:Label ID="LabelVoterCount" runat="server" /> total voters.
        </pre>
    </div>
    </form>
</body>
</html>
