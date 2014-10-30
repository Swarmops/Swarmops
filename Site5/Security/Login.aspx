<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Swarmops.Pages.Security.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />

    <!-- page title -->
	<title>Swarmops Alpha - Login</title>

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.css" rel="stylesheet" type="text/css" />

    <!-- custom styles -->
    
    <style type="text/css">

    </style>

</head>
<body>
    <form id="form2" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerBlahblah" />
	    <script type="text/javascript">

	    $(document).ready(function () {

	        /* document.ready goes here */

	        setTimeout(function() {
	            recheckLogin();
	        }, 1000);

	    });  // end of document.ready

	    function recheckLogin() {
	        alertify.log("Checking");
	        $.ajax({
	            type: "POST",
	            url: "Login.aspx/TestLogin",
	            data: '{"uriEncoded": "' + bitIdUri + '","nonce": "' + bitIdNonce + '"}',
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                alertify.success("Success " + msg.d);
	                if (msg.d) {
	                    alert("Successful login! Redirecting...");
	                    document.location = "/";
	                } else {
	                    setTimeout(function () {
	                        recheckLogin();
	                    }, 500);
	                }
	            },
	            error: function (msg) {
	                alertify.error("Error " + msg.d);
                    // we don't want the polling to die just because of a transient error
	                setTimeout(function () {
	                    recheckLogin();
	                }, 1000);
	            }

	        });
        }

	    var bitIdUri = '<asp:Literal ID="LiteralUri" runat="server" />';
	    var bitIdNonce = '<asp:Literal ID="LiteralNonce" runat="server" />';

	</script>
	

	
    <!-- Main menu, emptied out here -->

	<div class="center980px">
	    <div class="currentuserinfo"><div style="background-image:url('/Images/Icons/iconshock-user-16px.png');background-repeat:no-repeat;padding-left:16px;float:left"><asp:Label ID="LabelCurrentUserName" runat="server" /> | </div><div style="background-image:url('/Images/Icons/iconshock-workchair-16px.png');background-repeat:no-repeat;padding-left:17px;float:left"><asp:Label ID="LabelCurrentOrganizationName" runat="server" /> |&nbsp;</div><div style="background-image:url('/Images/Icons/iconshock-gamepad-16px.png');background-repeat:no-repeat;padding-left:20px;float:left"><asp:Label ID="LabelPreferences" runat="server" /> |&nbsp;</div><asp:Image ID="ImageCultureIndicator" runat="server" ImageUrl="~/Images/Flags/uk-24px.png" /></div>
        <div class="logoimage"><a href="/"><img style="border:none" src="/Security/Images/Swarmops-Logo.png" alt="Swarmops Logo" /></a></div>
        <div class="break"></div>
        <div class="topmenu">
            <div class="searchbox"><asp:TextBox ID="SearchBox" ReadOnly="true" runat="server" /></div>
        </div>
        
        <div class="mainbar">
            <div id="page-icon-encaps"><asp:Image ID="IconPage" runat="server" ImageUrl="~/Security/Images/iconshock-fingerprint-scanner-40px.png" /></div><h1><asp:Label ID="LabelPageTitle" Text="Login" runat="server" /></h1>
        
            <div class="box">
                <div class="content">
                
                    <h2><asp:Label ID="LabelHeader" runat="server" Text="XYZ Login with blockchain technology (BitID)" /></h2>
                    <div align="center"><asp:Image ID="ImageBitIdQr" runat="server"/></div>
                </div>
            </div>
        
        </div>
        <div class="sidebar">
    <h2 class="blue"><asp:Label ID="LabelSidebarInfoHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
        <asp:Label ID="LabelSidebarInfoContent" runat="server" />
        </div>
    </div>
    
    <h2 class="blue"><asp:Label ID="LabelSidebarManualLoginHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content" style="line-height:24px">
            <div style="float:left">User<br/>Pass</div><div style="float:right"><input type="text" />&nbsp;<br/><input type="text"/></div>
            <div style="clear:both"></div>
        </div>
    </div>
    
    <asp:Panel ID="PanelCheat" runat="server" Visible="false">
        <h2 class="blue">Dev's Cheat Button<span class="arrow"></span></h2>
    
        <div class="box">
            <div class="content" style="line-height:14px">
                <p>Since we're running on localhost, on a nonstandard port, with a debugger attached, and under Windows, this is clearly not a production environment. Since it's unlikely that the outside Internet has access to this machine, which means you can't login with BitID, a cheat button has been provided for you.</p><p>Press the button below to log on as Sandbox Administrator.</p>
                
                <div align="right"><asp:Button ID="ButtonCheat" runat="server" OnClick="ButtonCheat_Click" Text="Cheat Button" /></div>
            </div>
        </div>
    </asp:Panel>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarHelpHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="return false;" >
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-databaseconnect-16px.png')"></div>
                <asp:Label ID="LabelSidebarResetPassword" runat="server" />
            </div>
        </div>
    </div>
        </div>
        
	</div>

	</form>

    <!-- some javascript in footer -->
    
    <script language="javascript" type="text/javascript" src="/Scripts/alertify.min.js" ></script>

</body>
</html>





        


