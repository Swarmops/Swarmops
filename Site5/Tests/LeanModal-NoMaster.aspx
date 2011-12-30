<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LeanModal-NoMaster.aspx.cs" Inherits="Tests_LeanModal_NoMaster" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script language="javascript" type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
<script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
    
    
#lean_overlay {
    position: fixed;
    z-index:100;
    top: 0px;
    left: 0px;
    height:100%;
    width:100%;
    background: #000;
    display: none;
}

    
    </style>

    <script type="text/javascript" language="javascript">

        $(function () {
            $("#go").leanModal();
        });
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a id="go" href="#overlay">Test!</a>
    </div>
    </form>


    <div id="overlay" style="opacity:0;display:none">Samuel Jackson Lorem Ipsum</div>
</body>
</html>

