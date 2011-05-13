<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BudgetUsageBar.aspx.cs"
    Inherits="Pages.v4.Financial.BudgetUsageBar" %>

<%@ Register TagPrefix="dotnet" Namespace="dotnetCHARTING" Assembly="dotnetCHARTING" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Budget Usage Bar - for use in IFrame</title>
    <style>
    .bodyText
    {
            font-family: Arial, Helvetica, sans-serif;
        }
    </style>
</head>
<body style="border:0">
    <form id="form1" runat="server">
    <dotnet:Chart ID="Chart" runat="server" />
    </form>
</body>
</html>
