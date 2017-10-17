<%@ Page Language="C#" AutoEventWireup="true" Inherits="Swarmops.Pages.Security.Login" Codebehind="Login.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.1/jquery-ui.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.tmpl.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.toggles.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.color.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.animate-shadow.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.qtip.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/hoverIntent.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/superfish.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.json.min.js" ></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />
    
    <!-- favicon -->
    <link rel="shortcut icon" href="/Images/swarmops-favicon.png" type="image/png"/>

	<!-- style sheet, followed by script style sheets -->
    <asp:PlaceHolder ID="NeededForCacheMarkToParse1" runat="server">
        <link href="/Style/style-v5.css?CacheId=<%= _cacheVersionMark %>" rel="stylesheet" type="text/css" />
    </asp:PlaceHolder>
    <link href="/Style/jquery.qtip.css" rel="stylesheet" type="text/css" />
    <link href="/Style/jquery.toggles.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.core.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.default.css" rel="stylesheet" type="text/css" />
    <link href="/Style/superfish.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.1/themes/smoothness/jquery-ui.css" />
    
    <!-- external packages that are commonly used (on practically all pages) -->
    <Swarmops5:ExternalScripts ID="ExternalScriptEasyUI" Package="easyui" runat="server" />
    <asp:PlaceHolder ID="NeededForCacheMarkToParse2" runat="server">
        <link href="/Style/v5-easyui-overrides.css?CacheId=<%= _cacheVersionMark %>" rel="stylesheet" type="text/css" />
    </asp:PlaceHolder>
    
    <!-- Swarmops common JS functions, incl. EasyUI behavior overrides -->
    <script language="javascript" type="text/javascript" src="/Scripts/Swarmops-v5.js?CacheId=<%= _cacheVersionMark %>" ></script>

    <!-- page title -->

	<title>Swarmops Beta - Login</title>

    <!-- custom styles -->
    
    <style type="text/css">

        
    </style>

</head>
<body class="login-page">
    <form id="form2" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerBlahblah" />
	    <script type="text/javascript">

    	    $(document).ready(function() {

    	        /* document.ready goes here */

    	        setTimeout(function() {
    	            recheckLogin();
    	        }, 1000);


    	        $('.InputManualCredentials').on('input', function() {
    	            onInputCredentials();
    	        });

    	        $('#TextLogin').focus();

    	    }); // end of document.ready


    	    var manualCredentialsTestTrigger;

    	    function onInputCredentials() {
    	        clearTimeout(manualCredentialsTestTrigger);
    	        $('#TextLogin, #TextPass').css('background-image', 'none');

    	        manualCredentialsTestTrigger = setTimeout(function() {
    	            testManualCredentials();
    	        }, 1000);
    	    }

    	    function testManualCredentials() {
    	        // Submit credentials to server. If valid, they will validate through the recheckLogin call.

    	        var jsonData = {};
    	        jsonData.credentialsLogin = $('#TextLogin').val();
    	        jsonData.credentialsPass = $('#TextPass').val();
    	        jsonData.credentials2FA = $('#Text2FA').val();
    	        jsonData.logonUriEncoded = bitIdUri;

    	        if (jsonData.credentialsLogin.length == 0 || jsonData.credentialsPass.length == 0) {
	                return; // empty pass or login
	            }

    	        $.ajax({
    	            type: "POST",
    	            url: "Login.aspx/TestCredentials",
    	            data: $.toJSON(jsonData),
    	            contentType: "application/json; charset=utf-8",
    	            dataType: "json",
    	            success: function(msg) {
    	                if (msg.d == "Success") {
    	                    // Good credentials. TODO: Add manual 2FA challenge if applicable.
    	                    $('#TextLogin, #TextPass').css('background-image', "url('/Security/Images/iconshock-greentick-16px.png')").css('background-position', 'right center').css('background-repeat', 'no-repeat');
    	                } else if (msg.d == "Fail") {
	                        // inform user of bad credentials by means of a red cross on _both_ boxes
	                        $('#TextLogin, #TextPass').css('background-image', "url('/Security/Images/iconshock-cross-12px.png')").css('background-position', 'right center').css('background-repeat', 'no-repeat');
	                    } else if (msg.d == "BitIdRequired") {
	                        alertify.alert(SwarmopsJS.unescape('<%=this.Localized_BitIdRequired_Dialog%>'));
	                    } else {
	                        // wut?
	                    }
    	            },
    	            error: function(msg) {
    	                // retry after a second
    	                setTimeout(function() {
    	                    testManualCredentials();
    	                }, 1000);
    	            }

    	        });
    	    }

    	    function recheckLogin() {

    	        var jsonData = {};
    	        jsonData.uriEncoded = bitIdUri;
    	        jsonData.nonce = bitIdNonce;

    	        $.ajax({
    	            type: "POST",
    	            url: "Login.aspx/TestLogin",
    	            data: $.toJSON(jsonData),
    	            contentType: "application/json; charset=utf-8",
    	            dataType: "json",
    	            success: function(msg) {
    	                if (msg.d) {
    	                    // Login confirmed, fetch auth cookie from nonce in-context
    	                    document.location = "/Security/FinalizeLogin?Nonce=" + bitIdNonce;
    	                } else {
    	                    // Retry twice per second
    	                    setTimeout(function() {
    	                        recheckLogin();
    	                    }, 500);
    	                }
    	            },
    	            error: function(msg) {
    	                // we don't want the polling to die just because of a transient error
    	                setTimeout(function() {
    	                    recheckLogin();
    	                }, 1000);
    	            }

    	        });
    	    }

    	    function toManualLogin() {
    	        $('#divLoginQr').slideUp();
    	        $('#divLoginManual').slideDown();
    	        $('#TextLogin').focus();
    	        $('#paraSwitchManualLogin').hide();
    	        $('#paraSwitchBitIdLogin').show();

	            // return false; // says we handled the click, don't process further
    	    }

    	    function toBitIdLogin() {
    	        $('#divLoginQr').slideDown();
    	        $('#divLoginManual').slideUp();
    	        $('#paraSwitchManualLogin').show();
    	        $('#paraSwitchBitIdLogin').hide();
    	    }

    	    var bitIdUri = '<asp:Literal ID="LiteralUri" runat="server" />';
    	    var bitIdNonce = '<asp:Literal ID="LiteralNonce" runat="server" />';

    	    var linkSelfSignup = '<asp:Literal ID="LiteralSelfSignupLink" runat="server" />';

    	</script>
	

	<div class="center980px">

        <div class="login-page-logo"><asp:Image ID="ImageLogo" runat="server" ImageUrl="/Images/Swarmops-logo-256px.png" Width="128"/></div>        
            <div class="box qrlogin">
                <div class="content">
                    <div align="center" id="divLoginQr"><asp:Image ID="ImageBitIdQr" runat="server"/></div>
                    <div id="divLoginManual" style="display:none">
                        <h2><asp:Label runat="server" ID="LabelManualLoginHeader">Manual Login Header XYZ</asp:Label></h2>
                        <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding-bottom:5px">
                            <tr><td><asp:Literal ID="LiteralCredentialsUser" runat="server" />&nbsp;&nbsp;</td><td align="right"><input id="TextLogin" class="InputManualCredentials" type="text" /></td></tr>
                            <tr><td><asp:Literal ID="LiteralCredentialsPass" runat="server" />&nbsp;&nbsp;</td><td align="right"><input id="TextPass" class="InputManualCredentials" type="password" /></td></tr>
                            <tr style="display: none"><td><asp:Literal id="LiteralCredentials2FA" runat="server" />&nbsp;&nbsp;</td><td><input id="Text2FA" class="InputManualCredentials" type="password" /></td></tr>
                        </table>
                        
                        <p align="right"><a href="/Security/RequestPasswordReset"><asp:Label ID="LabelForgotPassword" runat="server">Help, I forgot my password!</asp:Label></a></p>
                    </div>
                </div>
            </div>
        
            <p align="center" id="paraSwitchManualLogin"><br/><a href="javascript:toManualLogin();"><asp:Label runat="server" ID="LabelUseManualLogin">Using Manual Password Login?</asp:Label></a></p>
            <p align="center" id="paraSwitchBitIdLogin" style="display:none"><br/><a href="javascript:toBitIdLogin();"><asp:Label runat="server" ID="LabelUseBitIdLogin">Using BitID Authentication?</asp:Label></a></p>
        

        
            <asp:Panel ID="PanelCheat" runat="server" Visible="false">
    
                <div class="box">
                    <div class="content" style="line-height: 14px">
                        <h2>Dev's Cheat Button</h2>
                        <p>Since we're running on localhost, on a nonstandard port, with a debugger attached, and under Windows, this is clearly not a production environment. Since it's unlikely that the outside Internet has access to this machine, which means you can't login with BitID, a cheat button has been provided for you.</p><p>Press the button below to log on as Sandbox Administrator.</p>
                
                        <div align="right"><asp:Button ID="ButtonCheat" runat="server" OnClick="ButtonCheat_Click" Text="Cheat Button" /></div>
                    </div>
                </div>
            </asp:Panel>

        </div>                
	</form>

    <!-- some javascript in footer -->
    
    <script language="javascript" type="text/javascript" src="/Scripts/alertify.min.js" ></script>

</body>
</html>





        


