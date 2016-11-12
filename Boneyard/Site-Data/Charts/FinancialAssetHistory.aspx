<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FinancialAssetHistory.aspx.cs" Inherits="Pages_Public_Charts_FinancialAssetHistory" %>
<%@ Register TagPrefix="dotnet" Namespace="dotnetCHARTING" Assembly="dotnetCHARTING"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Financial Asset History - Piratpartiet SE</title>
</head>
<body style="margin:0">
    <form id="form1" runat="server">
    <dotnet:Chart ID="Chart" runat="server"></dotnet:Chart>
    </form>
</body>
</html>
