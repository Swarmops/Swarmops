<%@ Page Language="C#" AutoEventWireup="true" Inherits="Swarmops.Pages.Security.RequestPasswordReset" Codebehind="RequestPasswordReset.aspx.cs" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.json.min.js"></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />

    <!-- page title -->
	<title>Swarmops Alpha - Request Password Reset</title>

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.core.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.default.css" rel="stylesheet" type="text/css" />

    <!-- custom styles -->
    
    <style type="text/css">

        div#DivSuccessMaybe {
            display: none;
        }
        
    </style>

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

                $.ajax({
                    type: "POST",
                    url: "/Security/RequestPasswordReset.aspx/RequestTicket",
                    data: $.toJSON(jsonData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,  // blocks until function returns - race conditions otherwise
                    success: function (msg) {
                        if (msg.d) {
                            $('#DivSuccessMaybe').slideDown();
                            $('#DivMailEntry').slideUp();
                        } // ignore a "false" for now - it means an invalid (syntactically wrong) mail addr
                    },
                    error: function(msg) {
                        alertify.error("<%=Resources.Global.Error_AjaxCallException%>");
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
        </div>
        
        <div class="mainbar">
            <div class="box">
                <div class="content">
                    <h2><asp:Label ID="LabelContentTitle" runat="server" /></h2>
                    <div id="DivMailEntry">
                        <div class="entryFields">
                            <asp:TextBox runat="server" ID="TextMailAddress" />&#8203;<br/>
                            <asp:Button ID="ButtonRequest" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="return resetPassword();" Text="XYZ Request"/>
                        </div>
                        <div class="entryLabels">
                            <asp:Label ID="LabelMail" runat="server" /><br />
                        </div>
                    </div>
                    <div style="clear:both"></div>
                    <div id="DivSuccessMaybe">
                        <asp:Label ID="LabelSuccessMaybe" runat="server" /><br/><br/><!-- some ugly formatting solutions on the one-off pages -->
                    </div>
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





        


