<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="AutoPostTest.aspx.vb"
    Inherits="PostTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="style/jHtmlArea.css" rel="stylesheet" type="text/css" />
    <link href="style/jHtmlArea.ColorPickerMenu.css" rel="stylesheet" type="text/css" />

    <script src="/jQuery/js/jquery-1.4.2.min.js" type="text/javascript"></script>

    <script src="/jQuery/js/jquery-ui-1.8.custom.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="/jQuery/jHtmlArea/scripts/jHtmlArea-0.7.0.js"></script>

    <script src="scripts/jHtmlArea.ColorPickerMenu-0.7.0.min.js" type="text/javascript"></script>

    <link rel="Stylesheet" type="text/css" href="style/jHtmlArea.css" />
    <object id="dlgHelper" classid="clsid:3050f819-98b5-11cf-bb82-00aa00bdce0b" width="0px"
        height="0px">
    </object>

    <script type="text/javascript">
        $(function() {
            var a = dlgHelper.blockFormats
            $("#txtText").htmlarea({
                toolbar: ["html", "|",
                        "forecolor",  // <-- Add the "forecolor" Toolbar Button
                        "|", "bold", "italic", "underline", "|", "p", "h1", "h2", "h3", "|", "link", "unlink"] // Overrides/Specifies the Toolbar buttons to show
            });
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager runat="server" ID="sm1">
        </asp:ScriptManager>
        <asp:Literal runat="server" ID="litText"></asp:Literal><br />
        <textarea runat="server" id="txtText" cols="50" rows="15"></textarea>
        <input type="submit" value='manual submit' />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="asp:Button" />
        <asp:LinkButton runat="server" ID="lbSubmit" Text="asp:LinkButton"></asp:LinkButton>
    </div>
    </form>
</body>
</html>
