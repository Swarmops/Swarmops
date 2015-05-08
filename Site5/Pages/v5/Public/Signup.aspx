<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Signup.aspx.cs" Inherits="Swarmops.Frontend.Pages.Public.Signup" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.smartWizard-2.0.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.json.min.js"></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />

    <!-- page title -->
	<title><%=Organization.Name %> - Signup</title>

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="WizardStyle.css" rel="stylesheet" type="text/css" />

    <!-- initialize all modal popups -->
    <script language="javascript" type="text/javascript">
        $(function () {
            $("a[rel*=leanModal]").leanModal();
        });
    </script>
    
    <!-- hide "previous" button -->
    
    <style type="text/css">
	    .swMain .buttonPrevious 
        {
	         display: none;
	    }
        .PermissionsErrorCredentials 
        {
             font-weight: bold;
        }
        .PermissionsErrorResults 
        {
            font-size: 200%;
            text-align: center;
        }
    </style>

</head>
<body>
    <form id="form2" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerBlahblah" />
	    <script type="text/javascript">


	        $(document).ready(function () {


	            // Initialize Smart Wizard	
	            $('#wizard').smartWizard({
	                transitionEffect: 'fade',
	                keyNavigation: false,
	                onLeaveStep: leaveAStepCallback,
	                onFinish: onFinishCallback
	            });

	            function leaveAStepCallback(obj) {
	                if (disableNext) {
	                    return false;
	                }

	                var stepNum = obj.attr('rel');
	                return validateStep(stepNum);
	            }

	            function onFinishCallback(obj) {
	                // $('#LOGINBUTTONID').click();
	            }

	            function validateStep(stepNumber) {
	                var isValid = true; // assume true and set false underway

	                if (stepNumber == 1) {
	                    isValid = true;
	                    // Blah

	                } else if (stepNumber == 2) {
	                    isValid = true; // assume true, make false as we go


	                } else if (stepNumber == 3) {
	                    isValid = true;

	                } else if (stepNumber == 4) {
	                    isValid = true; // assume true, make false as we go

	                } else if (stepNumber == 5) {
	                    // If we get here, we're always good

	                    isValid = true;
	                }

	                return isValid;
            }

	    });  // end of document.ready

        var disableNext = false;

        function DisableNext() {
            $('a.buttonNext').addClass("buttonDisabled");
            disableNext = true;
        }

        function EnableNext() {
            $('a.buttonNext').removeClass("buttonDisabled");
            disableNext = false;
        }

        function ClickNext() {
            $('a.buttonNext').click();
        }



	</script>
	
    <!-- Main menu, dynamically constructed -->

	<div class="center980px">
        <div class="topmenu" style="margin-top: -4px; padding-left: 30px; padding-top: 12px; color: white; font-family: Ubuntu; font-weight: 300; font-size: 24px; letter-spacing: 1px">
            SIGNING UP FOR <%= Organization.Name.ToUpperInvariant() %>
        </div>
        
        <div class="mainbar">
            <!--<div id="page-icon-encaps"><asp:Image ID="IconPage" runat="server" ImageUrl="~/Images/PageIcons/iconshock-ignitionkey-40px.png" /></div><h1><asp:Label ID="LabelPageTitle" Text="Installation" runat="server" /></h1>-->
        
        <div class="box">
            <div class="content">
                
                <div id="wizard" class="swMain">
  			        <ul>
  				        <li><a href="#step-1">
                        <label class="stepNumber">1</label>
                        <span class="stepDesc">
                           Welcome!<br />
                           <small>A short introduction</small>
                        </span>                   
                    </a></li>
  				        <li><a href="#step-2">
                        <label class="stepNumber">2</label>
                        <span class="stepDesc">
                           About you<br />
                           <small>Tell us a little about yourself</small>
                        </span>
                    </a></li>
  				        <li><a href="#step-3">
                        <label class="stepNumber">3</label>
                        <span class="stepDesc">
                           Create logon<br />
                           <small>Pick a password</small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-4">
                        <label class="stepNumber">4</label>
                        <span class="stepDesc">
                           Activity<br />
                           <small>How much do you wish to engage?</small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-5">
                        <label class="stepNumber">5</label>
                        <span class="stepDesc">
                           Complete<br />
                           <small>All done. Let's login!</small>
                        </span>                   
                    </a></li>
  			        </ul>
  			        <div id="step-1">	
                        <h2>Welcome to <%= Organization.Name %></h2>
                        <p>This is the organization's custom welcome text. It has not yet been written; it is set in Admin / Org Settings.</p>
                    </div>
  			        <div id="step-2">
  			            <h2>Initializing database</h2>
  			            
                    </div>                      
  			        <div id="step-3">
  			            <h2>Initializing database</h2>
                    </div>
                    <div id="step-4">
                        <h2>Creating the first user</h2>	
                    </div>
  			        <div id="step-5" style="display:none">
                    </div>
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
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActionsHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelSidebarActionsContent" runat="server" />
        </div>
    </div>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodoHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" onclick="return false;" >
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-databaseconnect-16px.png')"></div>
                <asp:Label ID="LabelSidebarTodoConnectDatabase" runat="server" />
            </div>
        </div>
    </div>
        </div>
        
	</div>

	</form>
</body>
</html>





        


