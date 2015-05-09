<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Signup.aspx.cs" Inherits="Swarmops.Frontend.Pages.Public.Signup" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <!-- jQuery and plugins -->
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" ></script>
    <script language="javascript" type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.leanModal.min.js" ></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.smartWizard-2.0.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/alertify.min.js"></script>
    <script language="javascript" type="text/javascript" src="/Scripts/jquery.json.min.js"></script>

    <!-- fonts -->
    <link href='https://fonts.googleapis.com/css?family=Permanent+Marker' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:light,regular,500,bold' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Arimo:regular,italic,bold,bolditalic' rel='stylesheet' type='text/css' />

    <!-- page title -->
	<title><%=Organization.Name %> - Signup</title>

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.core.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.default.css" rel="stylesheet" type="text/css" />
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

        .swMain {
            height: 420px !important;
        }

        .stepContainer {
            height: 380px !important;
        }

        div.stepContainer > div.content {
            overflow: initial !important;
        }



        .entryLabelsAdmin {
            width: 280px;
        }

    </style>

</head>
<body>
    <form id="form2" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerBlahblah" />
	    <script type="text/javascript">
    	    
	        // Functions copied from Swarm / Add Person and adapted. It would be nice if this could be reconciled to one source,
	        // but ASP.Net server-side includes don't work well, and a JavaScript include isn't compatible with the
            // ASP.Net inline evaluations.

    	    $(document).ready(function () {
    	        UpdatePostalPrefix('XX', $('#<%= DropCountries.ClientID %>').val());

    	        $('#<%= TextPostal.ClientID %>').on('input', function () {
    	            if (CheckPostalCode()) {
    	                if ($('#<%= TextPostal.ClientID %>').val().length == postalCodeLength) {
    	                    $('#<%= TextDateOfBirth.ClientID %>').focus();
    	                }
    	            }
    	        });
    	        $('#<%= TextCity.ClientID %>').on('input', function () {
    	            CheckPostalCity();
    	        });
    	    });

    	    function CheckPostalCity() {
    	        var currentCity = $('#<%= TextCity.ClientID %>').val().toLowerCase();
    	        if (currentCity in cityNameLookup) {
    	            var geoId = cityNameLookup[currentCity];
    	            $('#spanDetectedGeo').text(geographyIdLookup[geoId]);
    	        } else {
    	            $('#spanDetectedGeo').text('');
    	        }
    	    }

    	    function CheckPostalCode() {
    	        var currentPostalCode = $('#<%= TextPostal.ClientID %>').val().toLowerCase();
    	        if (currentPostalCode.length > postalCodeLengthCheck) {
    	            currentPostalCode = currentPostalCode.substring(0, postalCodeLengthCheck);
    	        }
    	        if (currentPostalCode in postalCodeLookup) {
    	            var cityId = postalCodeLookup[currentPostalCode];
    	            $('#<%= TextCity.ClientID %>').attr("disabled", "disabled").val(cityIdLookup[cityId].Name);
    	            $('#spanDetectedGeo').text(geographyIdLookup[cityIdLookup[cityId].GeoId]);
    	            postalCodeIdentified = true;
    	            return true;
    	        } else if (postalCodeIdentified) {
    	            $('#<%= TextCity.ClientID %>').removeAttr("disabled").val('');
    	            $('#spanDetectedGeo').text('');
    	            postalCodeIdentified = false;
    	        }
    	        return false;
    	    }

    	    function AnimatePostalCodeLength() {
    	        if (postalCodeLength > 0) {
    	            var postalTargetLength = postalCodeLength * 8 - 5;
    	            var cityTargetLength = 135 - 8 * postalCodeLength;

    	            $('#<%= TextPostal.ClientID %>').animate({ width: postalTargetLength + "px" });
    	            $('#<%= TextCity.ClientID %>').animate({ width: cityTargetLength + "px" });

    	            if (!postalCodeVisible) {
    	                setTimeout(function() {
    	                    $('#<%= TextPostal.ClientID %>').fadeIn(200);
    	                }, 100);
    	                $('#spanLabelPostal').animate({ width: 'toggle' }, 400);
    	                postalCodeVisible = true;
    	            }

    	        } else if (postalCodeVisible) {
    	            // Remove postal code altogether, if visible now

    	            $('#<%= TextPostal.ClientID %>').animate({ width: 0 }, 200).fadeOut(100);
    	            $('#<%= TextCity.ClientID %>').animate({ width: "255px" }, 600);
    	            $('#spanLabelPostal').animate({ width: 'toggle' }, 400);

    	            postalCodeVisible = false;
    	        }
    	    }

    	    function UpdatePostalPrefix(oldValue, newValue) {
    	        $('#spanCountryPrefix').text(newValue);
    	        $('#<%= TextCity.ClientID %>').removeAttr("disabled").val('');
    	        $('#spanDetectedGeo').text('');

    	        // Get postal code data for country. Heavy op; should not be done from mobile

    	        var jsonData = {};
    	        jsonData.countryCode = newValue;

    	        $.ajax({
    	            type: "POST",
    	            url: "/Automation/FieldValidation.aspx/GetPostalCodesCities",
    	            data: $.toJSON(jsonData),
    	            contentType: "application/json; charset=utf-8",
    	            dataType: "json",
    	            success: function(msg) {
    	                postalCodeLookup = {};
    	                cityIdLookup = {};
    	                cityNameLookup = {};
    	                geographyIdLookup = {};

    	                postalCodeLength = msg.d.PostalCodeLength;
    	                postalCodeLengthCheck = msg.d.PostalCodeLengthCheck;
    	                $('#<%= TextPostal.ClientID %>').attr('maxlength', postalCodeLength).attr('placeholder', '12345678'.substring(0, postalCodeLength));
    	                msg.d.PostalCodes.forEach(function(element, index, array) {
    	                    postalCodeLookup[element.Code.toLowerCase()] = element.CityId;
    	                });
    	                msg.d.CityNames.forEach(function(element, index, array) {
    	                    cityIdLookup[element.Id] = {};
    	                    cityIdLookup[element.Id].Name = element.Name;
    	                    cityIdLookup[element.Id].GeoId = element.GeographyId;
    	                    cityNameLookup[element.Name.toLowerCase()] = element.GeographyId;
    	                });
    	                msg.d.Geographies.forEach(function(element, index, array) {
    	                    geographyIdLookup[element.Id] = element.Name;
    	                });

    	                AnimatePostalCodeLength();
    	                CheckPostalCode();
    	            },
    	            error: function(msg) {
    	                console.log(msg);
    	                geographyIdLookup = {};
    	                postalCodeLookup = {};
    	                cityIdLookup = {};
    	                cityNameLookup = {};
    	            }
    	        });
    	    }

    	    function ValidatePersonFields() {
    	        var isValid = true;

    	        var dateFieldContents = $('#<%= TextDateOfBirth.ClientID%>').val();
    	        if (dateFieldContents.length > 0) {

    	            var jsonData = {};
    	            jsonData.input = dateFieldContents;

    	            $.ajax({
    	                type: "POST",
    	                url: "/Automation/FieldValidation.aspx/IsDateValid",
    	                data: $.toJSON(jsonData),
    	                contentType: "application/json; charset=utf-8",
    	                dataType: "json",
    	                async: false,  // blocks until function returns - race conditions otherwise
    	                success: function (msg) {
    	                    if (msg.d != true) {
    	                        isValid = false;
    	                        $('#<%= TextDateOfBirth.ClientID %>').addClass("entryError");
    	                        alertify.error("<asp:Literal runat="server" ID="LiteralErrorDate" />");
    	                        $('#<%=this.TextDateOfBirth.ClientID %>').focus();
    	                    }
    	                }
    	            });
    	        }

    	        isValid = ValidateTextField('#<%= TextCity.ClientID %>', "<asp:Literal runat="server" ID="LiteralErrorCity" />") && isValid;
    	        isValid = ValidateTextField('#<%= TextStreet1.ClientID %>', "<asp:Literal runat="server" ID="LiteralErrorStreet" />") && isValid;
    	        isValid = ValidateTextField('#<%= TextMail.ClientID %>', "<asp:Literal runat="server" ID="LiteralErrorMail" />") && isValid;
    	        isValid = ValidateTextField('#<%= TextName.ClientID %>', "<asp:Literal runat="server" ID="LiteralErrorName" />") && isValid; // TODO: Actually validate geography?

    	        return isValid;
    	    }

    	    function ValidateTextField(fieldId, message) {
    	        $(fieldId).removeClass("entryError");
    	        if ($(fieldId).val().length == 0) {
    	            alertify.error(message);
    	            $(fieldId).addClass("entryError");
    	            $(fieldId).focus();
    	            return false;
    	        }

    	        return true;
    	    }

    	    var postalCodeLookup = {};
    	    var cityIdLookup = {};
    	    var cityNameLookup = {};
    	    var geographyIdLookup = {};
    	    var postalCodeLength = 0;
    	    var postalCodeLengthCheck = 0;
    	    var postalCodeVisible = true;
    	    var postalCodeIdentified = false;




    	    $(document).ready(function () {

    	        // Doc.Ready instance 2. Smart Wizard initialization.
    	        // This is a separate Doc.Ready as I hope to break out the
    	        // above code some day to a common file between this file
    	        // and Swarm / Add.  (TODO.)


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

	                    $('#<%= TextName.ClientID %>').focus();

	                } else if (stepNumber == 2) {
	                    isValid = ValidatePersonFields();

	                } else if (stepNumber == 3) {
    	                isValid = true;

    	            } else if (stepNumber == 4) {
    	                isValid = true; // assume true, make false as we go

    	            } else if (stepNumber == 5) {
    	                isValid = true; // assume true, make false as we go

    	            } else if (stepNumber == 6) {
    	                // If we get here, we're always good

    	                isValid = true;
    	            }

    	            return isValid;
    	        }

	            $('#<%=this.DropCountries.ClientID%>').change(function() {
	                UpdatePostalPrefix('oldValueDummy', $('#<%=this.DropCountries.ClientID%>').val());
	            });

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
                           Activity 1<br />
                           <small>How much do you wish to engage?</small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-5">
                        <label class="stepNumber">5</label>
                        <span class="stepDesc">
                           Activity 2<br />
                           <small>What would you like to engage in?</small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-6">
                        <label class="stepNumber">6</label>
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
                        <div class="entryLabelsAdmin">
                            <asp:Label ID="LabelName" runat="server" /><br />
                            <asp:Label ID="LabelCountry" runat="server" /><br />
                            <asp:Label ID="LabelMail" runat="server" /><br />
                            <asp:Label ID="LabelPhone" runat="server" /><br />
                            <asp:Label ID="LabelStreet1" runat="server" /><br />
                            <asp:Label ID="LabelStreet2" runat="server" /><br />
                            <span id="spanLabelPostal" style="display: inline-block; overflow: hidden"><asp:Label ID="LabelPostalCode" runat="server" />,&nbsp;</span><span style="display: inline-block; overflow: hidden"><asp:Label runat="server" ID="LabelCity" /></span><br />
                            <span id="SpanGeoDetected"><asp:Label ID="LabelGeographyDetected" runat="server" /></span><span id="SpanGeoSelect" style="display: none"><asp:Label ID="LabelSelectGeography" runat="server" Text="XYZ Select Geography" /></span><br />
                            <asp:Label ID="LabelDateOfBirth" runat="server" /><br />
                            <asp:Label ID="LabelLegalGender" runat="server" /><br />
                        </div>
  			            <div class="entryFieldsAdmin">
                            <asp:TextBox runat="server" ID="TextName" />&#8203;<br/>
                            <asp:DropDownList runat="server" ID="DropCountries"/>&#8203;<br/>        
                            <asp:TextBox runat="server" ID="TextMail" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextPhone" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextStreet1" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextStreet2" />&#8203;<br/>
                            <div style="float: right; margin-right: -1px"><asp:TextBox runat="server" ID="TextPostal" />&nbsp;<asp:TextBox runat="server" ID="TextCity" /></div><div style="width: 22px; overflow: hidden"><span id="spanCountryPrefix">XX</span>&ndash;</div>
                            <span id="spanDetectedGeo">...</span>&nbsp;<br/>
                            <asp:TextBox runat="server" ID="TextDateOfBirth" />&#8203;<br/>
                            <asp:DropDownList runat="server" ID="DropGenders" />&#8203;<br/>
                        </div>

                    </div>                      
  			        <div id="step-3">
  			            <h2>Your Logon</h2>
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





        


