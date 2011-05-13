<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DisplayInternalPollVotes.aspx.cs" Inherits="Data_DisplayInternalPollVotes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Title is set in Page_Load, or should be</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <pre>Verification  PersonIds for selected candidates, in order
<asp:Repeater ID="RepeaterVotes" runat="server" 
            onitemdatabound="RepeaterVotes_ItemDataBound"><ItemTemplate>
<%# Eval("VerificationCode") %>  <asp:Repeater ID="RepeaterVoteDetails" runat="server" ><ItemTemplate><%# personIdLookup[(int) Container.DataItem] %> </ItemTemplate></asp:Repeater></ItemTemplate></asp:Repeater>

<asp:Label ID="LabelVoteCount" runat="server" /> votes total.
        </pre>
    </div>
    </form>
</body>
</html>
