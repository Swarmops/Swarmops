<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RelativeDistroStatus.aspx.cs" Inherits="Charts_RelativeDistroStatus" %>
<%@ Register TagPrefix="dotnet" Namespace="dotnetCHARTING" Assembly="dotnetCHARTING"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Ballot Distribution Status</title>
    <style type="text/css">
        .sparse
        {
            margin-right: 20px;
            margin-left: 10px;
        }
        *
        {
            font-family: Arial, Helvetica, sans-serif;
        }
    </style>
</head>
<body style="margin:0">
    <form id="form1" runat="server">
    <asp:Panel ID="NavPanel" runat="server">

    <asp:LinkButton class="sparse" ID="LinkButton1" runat="server" 
        onclick="LinkButton1_Click">Förhandslokaler </asp:LinkButton>
    <asp:LinkButton  class="sparse" ID="LinkButton2" runat="server" 
        onclick="LinkButton2_Click">Vallokaler</asp:LinkButton>
    <asp:CheckBox ID="cbWeighted" class="sparse"  runat="server" AutoPostBack=true 
        Text="Viktat per antal invånare" oncheckedchanged="cbWeighted_CheckedChanged" />
    
    <asp:CheckBox ID="cbSort" runat="server" Text="Sortera" AutoPostBack=true 
        oncheckedchanged="cbSort_CheckedChanged" />
    
    <br />
    </asp:Panel>      
    <dotnet:Chart ID="Chart" runat="server"></dotnet:Chart>
    <asp:Literal ID="LiteralXml" runat="server" />
    </form>
</body>
</html>
