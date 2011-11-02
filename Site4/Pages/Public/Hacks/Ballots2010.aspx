<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ballots2010.aspx.cs" Inherits="Pages_Public_Hacks_Ballots2010" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>
<%@ Import Namespace="Activizr.Logic.Structure"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Piratpartiets valsedlar 2010</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Repeater ID="RepaterBallots" runat="server" OnItemDataBound="RepeaterBallots_ItemDataBound">
        <ItemTemplate>
        <pre>
Valsedel #<%# Eval("Identity") %> med namnet "<%# Eval("Name") %>"
Giltig i: <%# ((Geography)Eval("Geography")).Name %>
Antal exemplar: <%# Eval("Count") %>
Kostnad: <%# ((int)Eval("Count"))*40/1000 + 650 %>
Leveransadress:
<%# Eval("DeliveryAddress") %>

Kandidater:<asp:Repeater ID="RepeaterCandidates" runat="server">
<ItemTemplate>
<%# Eval("Position") %>. <%# Eval("Ok") %> <%# ((Person)Eval("Person")).Canonical %>, <%# Eval("CityName") %>, <%# Eval("Age") %> år</ItemTemplate></asp:Repeater>

-----------------------------------------------------

        </pre>
        </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
