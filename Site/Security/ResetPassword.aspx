<%@ Page Language="C#" AutoEventWireup="true" Inherits="Swarmops.Pages.Security.ResetPassword" Codebehind="ResetPassword.aspx.cs" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.smartWizard-3.3.1.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/alertify.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.json.min.js"></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />

    <!-- page title -->
	<title>Resetting Password</title>

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.core.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.default.css" rel="stylesheet" type="text/css" />
    <link href="WizardStyle.css" rel="stylesheet" type="text/css" />

    <!-- favicon -->
    <link rel="shortcut icon" href="/Images/swarmops-favicon.png" type="image/png"/>
    
    <!-- external packages that are commonly used (on practically all pages) -->
    
    <!-- UGLY HACK: Control ExternalScripts requires authentication for some reason. This is a bug. But to get alpha-09 out on time, we're
        circumventing the bug by hardcoding the hosted scripts - this needs fixing. -->

    <script src="//hostedscripts.falkvinge.net/staging/easyui/jquery.easyui.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="//hostedscripts.falkvinge.net/staging/easyui/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="//hostedscripts.falkvinge.net/staging/easyui/themes/default/easyui.css" />
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

        
    <div class="topmenu" style="margin-top: -4px; padding-top: 12px; color: white; font-family: Ubuntu; font-weight: 300; font-size: 24px; letter-spacing: 1px">
      	<div class="center980px">
            <span style="padding-left: 15px; padding-right: 15px"><asp:Label ID="LabelHeader" runat="server" /></span>
        </div>
    </div>
        
	<div class="center980px">
        
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
                            <asp:Button ID="ButtonRequest" runat="server" CssClass="button-accent-color suppress-input-focus" OnClientClick="return resetPassword();" Text="XYZ Request"/>
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





        


