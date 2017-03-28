<%@ Page Language="C#" AutoEventWireup="true" Inherits="Swarmops.Pages.Security.ResetPassword" Codebehind="ResetPassword.aspx.cs" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->

    <script language="javascript" type="text/javascript" src="/Scripts/jquery.2.0.3.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery-ui.1.11.1.min.js" ></script>

    <script language="javascript" type="text/javascript" src="/Scripts/jquery.tmpl.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.toggles.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.color.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.animate-shadow.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/hoverIntent.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/superfish.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.json.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/qrcode.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.qrcode.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/odometer.js" ></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />
    
    <!-- page title -->
	<title>Swarmops - Reset Password</title>
    
    <!-- favicon -->
    <link rel="shortcut icon" href="/Images/swarmops-logo-128px.png" type="image/png"/>

	<!-- style sheet, followed by script style sheets -->

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />

    <link href="/Style/jquery.toggles.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.core.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.default.css" rel="stylesheet" type="text/css" />
    <link href="/Style/superfish.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.1/themes/smoothness/jquery-ui.css" />
    <link href="/Style/odometer-theme-car.css" rel="stylesheet" type="text/css" />
    
    <!-- external packages that are commonly used (on practically all pages) -->
    <Swarmops5:ExternalScripts ID="ExternalScriptEasyUI" Package="easyui" runat="server" />
    <link href="/Style/v5-easyui-overrides.css" rel="stylesheet" type="text/css" />
    
    <!-- Swarmops common JS functions, incl. EasyUI behavior overrides -->
    <script language="javascript" type="text/javascript" src="/Scripts/Swarmops-v5.js" ></script>


</head>
<body>
    <form id="form2" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerBlahblah" />
	    <script type="text/javascript">

	        $(document).ready(function () {

	            /* document.ready goes here */

	        });

	        function resetPassword() {
	            var jsonData = {};
	            jsonData.mailAddress = $('#<%=this.TextMailAddress.ClientID%>').val();
	            jsonData.ticket = $('#<%=this.TextTicket.ClientID%>').val();
	            jsonData.newPassword = $('#<%=this.TextPassword1.ClientID%>').val();

	            if (jsonData.newPassword != $('#<%=this.TextPassword2.ClientID%>').val()) {
	                alertify.error(SwarmopsJS.unescape('<%= this.Localize_NewPasswordsDontMatch %>'));
	                return false;
	            }

	            if (jsonData.newPassword == "") {
	                alertify.error(SwarmopsJS.unescape('<%=this.Localize_NoEmpty%>'));
	                return false;
	            }

                $.ajax({
                    type: "POST",
                    url: "/Security/ResetPassword.aspx/PerformReset",
                    data: $.toJSON(jsonData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,  // blocks until function returns - race conditions otherwise
                    success: function (msg) {
                        if (msg.d) {
                            document.location = "/"; // success, auth cookies have been set - redirect to Dashboard
                        } else {
                            alertify.dialog(SwarmopsJS.unescape('<%=this.Localize_ResetPasswordFailed%>'));
                        }
                    },
                    error: function (msg) {
                        alertify.error(SwarmopsJS.unescape('<%=this.Localize_GenericAjaxError%>'));
                    }
                });

                return false; // prevent page submission
            }

    	</script>
	

	
    <!-- Main menu, emptied out here -->
        
	<div class="center980px">
	    <div class="currentuserinfo"><div style="background-image: url('/Images/Icons/iconshock-user-16px.png'); background-repeat: no-repeat; padding-left: 16px; float: left"><asp:Label ID="LabelCurrentUserName" runat="server" /> | </div><div style="background-image: url('/Images/Icons/iconshock-workchair-16px.png'); background-repeat: no-repeat; padding-left: 17px; float: left"><asp:Label ID="LabelCurrentOrganizationName" runat="server" /> |&nbsp;</div><div style="background-image: url('/Images/Icons/iconshock-gamepad-16px.png'); background-repeat: no-repeat; padding-left: 20px; float: left"><asp:Label ID="LabelPreferences" runat="server" /> |&nbsp;</div><asp:Image ID="ImageCultureIndicator" runat="server" ImageUrl="~/Images/Flags/uk-24px.png" /></div>
        <div class="logoimage"><a href="/"><img style="border: none" src="/Security/Images/Swarmops-Logo.png" alt="Swarmops Logo" /></a></div>
        <div class="break"></div>
        <div class="topmenu">
            <div class="searchbox"><asp:TextBox ID="SearchBox" ReadOnly="true" runat="server" /></div>
        </div>
        
        <div class="mainbar">
            <div class="box">
                <div class="content">
                    <h2><asp:Label ID="LabelContentTitle" runat="server" /></h2>
                    <div id="DivMailEntry">
                        <div class="entryFields">
                            <asp:TextBox runat="server" ID="TextMailAddress" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextTicket" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextPassword1" TextMode="Password" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextPassword2" TextMode="Password" />&#8203;<br/>
                            <asp:Button ID="ButtonRequest" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="return resetPassword();" Text="XYZ Request"/>
                        </div>
                        <div class="entryLabels">
                            <asp:Label ID="LabelMail" runat="server" /><br />
                            <asp:Label ID="LabelTicket" runat="server" /><br />
                            <asp:Label ID="LabelPassword1" runat="server" /><br />
                            <asp:Label ID="LabelPassword2" runat="server" /><br />
                        </div>
                    </div>
                    <div style="clear:both"></div>
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
   
        </div>
        
	</div>

	</form>

    <!-- some javascript in footer -->
    
    <script language="javascript" type="text/javascript" src="/Scripts/alertify.min.js" ></script>

</body>
</html>





        


