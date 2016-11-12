<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MediaEntriesFra.aspx.cs" Inherits="Pages_Special_SE_MediaEntriesFra" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>Bloggat om FRA nyligen (data fr&aring;n <a href="http://knuff.se/q/FRA">knuff.se</a>): <asp:Literal ID="literalBlogOutput" runat="server" /></p>
        <p>Gammelmedia om FRA nyligen: <asp:Literal ID="literalOldMediaOutput" runat="server" /></p>
    </div>
    </form>
</body>
</html>
