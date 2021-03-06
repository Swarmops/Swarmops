﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Public.Signup" CodeFile="Signup.aspx.cs" CodeBehind="Signup.aspx.cs" %>

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
	<title><%=Organization.Name %> - Signup</title>
    
    <!-- favicon -->
    <link rel="shortcut icon" href="/Images/swarmops-favicon.png" type="image/png"/>

    <link href="/Style/style-v5.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.core.css" rel="stylesheet" type="text/css" />
    <link href="/Style/alertify.default.css" rel="stylesheet" type="text/css" />
    <link href="WizardStyle.css" rel="stylesheet" type="text/css" />
    
    <!-- BUG: the external scripts and styles should normally be provided by the ExternalScripts control
         BUG: ...that determines an available URL for the external scripts, but including the component
         BUG: ...on the page forces an authentication, thereby disabling the key function of the page.
         BUG: ...I've spent hours trying to find why adding the ExternalScripts control to the page
         BUG: ...triggers authentication, including adding web.configs everywhere, without results. -->

    <!-- external packages that are commonly used (on practically all pages) -->
    <script src="/Scripts/ExternalScripts/easyui/jquery.easyui.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="/Scripts/ExternalScripts/easyui/themes/icon.css">
    <link rel="stylesheet" type="text/css" href="/Scripts/ExternalScripts/easyui/themes/default/easyui.css">
    
    <!-- Swarmops common JS functions, incl. EasyUI behavior overrides -->
    <script language="javascript" type="text/javascript" src="/Scripts/Swarmops-v5.js" ></script>

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

        input:not([type="radio"]):not([type="checkbox"]), select {
            width: 200px;
            font-size: 16px;
        }

        input[type="radio"] {
            margin-right: 5px;  /* complements margin-left 5px in stylesheet */
            margin-bottom: 5px;
            margin-top: 10px;
        }

        input[type="radio"]+label {
            font-size: 16px;
            position:relative; /* ugly ugly way to circumvent a nonfunctional margin-bottom */
            top: -4px;
        }

        input[type="radio"]:checked+label {
            font-weight: 500; /* increase font-weight for selected radio button labels to Medium */
        }

        input[type="radio"]+label+p {
            padding-left: 24px;
        }

        body.rtl input[type="radio"]+label+p {
            padding-right: 24px;
            padding-left: inherit;
        }


        .data-entry-fields-admin, .entry-labels-admin {
            font-size: 16px;
        }

        .enableAskParticipantStreet {
            display: none;
        }


        .entry-labels-admin {
            font-weight: 300;
        }

        div.stepContainer div.content {
            width: 100%;
        }

    </style>

</head>
<body <asp:Literal ID="LiteralBodyAttributes" runat="server" />>
    <form id="form2" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManagerBlahblah" />
	    <script type="text/javascript">
    	    
	        // Functions copied from Swarm / Add Person and adapted. It would be nice if this could be reconciled to one source,
	        // but ASP.Net server-side includes don't work well, and a JavaScript include isn't compatible with the
            // ASP.Net inline evaluations.

	        $(document).ready(function () {

	            // Set postal code and city to something really small for initial layout

	            $('#<%=this.TextPostal.ClientID%>').width(40);
	            $('#<%=this.TextCity.ClientID%>').width(100);

	            // Set widths of dropdowns equal to textboxen

	            var inputWidth = $('#<%=this.TextName.ClientID%>').width();
	            $('#<%=this.DropCountries.ClientID%>').width(inputWidth -14);
	            $('#<%=this.DropGenders.ClientID%>').width(inputWidth -14);

                // Guess country

	            SwarmopsJS.ajaxCall("/Pages/v5/Public/Signup.aspx/GuessCountry", {}, function(result) {
	                if (result.Success && result.DisplayMessage.length > 0) {
	                    $('#<%=this.DropCountries.ClientID%>').val(result.DisplayMessage);
	                    UpdatePostalPrefix('XX', result.DisplayMessage);
	                } else {
	                    // No dice for some reason - initialize postal codes with default lookup
	                    UpdatePostalPrefix('XX', $('#<%= DropCountries.ClientID %>').val());
	                }
	            });


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
    	            currentGeographyId = cityNameLookup[currentCity];
    	            $('#spanDetectedGeo').text(geographyIdLookup[currentGeographyId]);
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
	                currentGeographyId = cityIdLookup[cityId].GeoId;
    	            $('#spanDetectedGeo').text(geographyIdLookup[currentGeographyId]);
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
    	            var postalTargetLength = postalCodeLength * 10 - 2;
    	            var cityTargetLength = 160 - 10 * postalCodeLength;

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
    	                        $('#<%= TextDateOfBirth.ClientID %>').addClass("data-entry-error");
    	                        alertify.error(SwarmopsJS.unescape("<%=Localize_ErrorDate%>"));
    	                        $('#<%=this.TextDateOfBirth.ClientID %>').focus();
    	                    }
    	                }
    	            });
    	        }

    	        isValid = ValidateTextField('#<%= TextCity.ClientID %>', SwarmopsJS.unescape("<%=Localize_ErrorCity%>")) && isValid;
	            isValid = ValidateTextField('#<%= TextMail.ClientID %>', SwarmopsJS.unescape("<%=Localize_ErrorMail%>")) && isValid;
    	        isValid = ValidateTextField('#<%= TextName.ClientID %>', SwarmopsJS.unescape("<%=Localize_ErrorName%>")) && isValid; // TODO: Actually validate geography?

	            if (askParticipantStreet) {
	                isValid = ValidateTextField('#<%= TextStreet1.ClientID %>', SwarmopsJS.unescape("<%=Localize_ErrorStreet%>")) && isValid;
	            }

    	        if (isValid) {

    	            var jsonData = {};
    	            jsonData.mail = $('#<%= TextMail.ClientID %>').val();

	                $.ajax({
	                    type: "POST",
	                    url: "/Pages/v5/Public/Signup.aspx/CheckMailFree",
	                    data: $.toJSON(jsonData),
	                    contentType: "application/json; charset=utf-8",
	                    dataType: "json",
	                    async: false, // blocks until function returns - race conditions otherwise
	                    success: function(msg) {
	                        if (msg.d.Success != true) {
	                            isValid = false;
	                            $('#<%= this.TextMail.ClientID %>').addClass("data-entry-error");
	                            alertify.error(SwarmopsJS.unescape("<%=Localize_ErrorMailExists%>"));
	                            $('#<%=this.TextMail.ClientID %>').focus();
	                        }
	                    }
	                });
	            }

	            return isValid;
    	    }

    	    function ValidateTextField(fieldId, message) {
    	        $(fieldId).removeClass("data-entry-error");
    	        if ($(fieldId).val().length == 0) {
    	            alertify.error(message);
    	            $(fieldId).addClass("data-entry-error");
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


    	    var currentGeographyId = 0;


    	    $(document).ready(function () {

    	        $('#tableVolunteerPositions').datagrid({ url:'/Pages/v5/Public/Json-SignupVolunteerPositions.aspx?OrganizationId=<%=Organization.Identity%>&GeographyId=0'});

    	        // Doc.Ready instance 2. Smart Wizard initialization.
    	        // This is a separate Doc.Ready as I hope to break out the
    	        // above code some day to a common file between this file
    	        // and Swarm / Add.  (TODO.)

    	        if (askParticipantStreet) {
	                $('.enableAskParticipantStreet').show();
	            }

    	        $('#wizard').smartWizard({
    	            transitionEffect: 'fade',
    	            labelNext: SwarmopsJS.unescape("<%=Localize_WizardNext%>"),
    	            labelFinish: SwarmopsJS.unescape("<%=Localize_WizardFinish%>"),
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
    	            var positionIdArray = [];

	                $('a.buttonFinish').addClass('buttonDisabled').css('cursor', 'wait');

	                if (selectedPositions.length > 0) {
	                    selectedPositions.forEach(function(currentValue, index, array) {
	                        positionIdArray.push(parseInt(currentValue.positionId));
	                    });
	                }

	                SwarmopsJS.ajaxCall("/Pages/v5/Public/Signup.aspx/SignupParticipant",
	                    {
	                        name: $('#<%=this.TextName.ClientID%>').val(),
	                        organizationId: <%= this.Organization.Identity %>,
	                        mail: $('#<%=this.TextMail.ClientID%>').val(),
	                        phone: $('#<%=this.TextPhone.ClientID%>').val(),
	                        password: $('#<%=this.TextPassword1.ClientID%>').val(),
	                        street1: $('#<%=this.TextStreet1.ClientID%>').val(),
	                        street2: $('#<%=this.TextStreet2.ClientID%>').val(),
	                        postalCode: $('#<%=this.TextPostal.ClientID%>').val(),
	                        city: $('#<%=this.TextCity.ClientID%>').val(),
	                        countryCode: $('#<%=this.DropCountries.ClientID%>').val(),
    	                    dateOfBirth: $('#<%=this.TextDateOfBirth.ClientID%>').val(),
    	                    geographyId: currentGeographyId,
    	                    activist: $('input:radio[name="ActivationLevel"]:checked').val() != "RadioActivationPassive",
    	                    gender: $('#<%=this.DropGenders.ClientID%>').val(),
                            positionIdsVolunteer: positionIdArray

	                    },
	                    function() {
	                        // On success, the participant is created and an authentication cookie has been set. Redirect to Dashboard.
	                        document.location = "/";
	                    });
	            }

    	        function validateStep(stepNumber) {
    	            var isValid = true; // assume true and set false underway

    	            if (stepNumber == 1) {
    	                isValid = true;
    	                // Blah

    	                $('div#divLanguageSelector').slideUp().fadeOut();
    	                $('div#divTodoDerpage').slideDown().fadeIn();
    	                setTimeout(function() { $('#<%= TextName.ClientID %>').focus(); }, 250); // after 250ms, set focus to name field

	                } else if (stepNumber == 2) {
	                    isValid = ValidatePersonFields();
	                    if (isValid) {
	                        $('#spanMailLoginKey').text($('#<%=this.TextMail.ClientID%>').val());
	                        setTimeout(function() { $('#<%= TextPassword1.ClientID %>').focus(); }, 250); // after 250ms, set focus to pwd1 field
	                    }

	                } else if (stepNumber == 3) {
	                    isValid = ValidateTextField('#<%= TextPassword1.ClientID %>', SwarmopsJS.unescape("<%=Localize_ErrorNeedPassword%>"));

	                    if (isValid) {
	                        if ($('#<%=TextPassword1.ClientID%>').val() != $('#<%=TextPassword2.ClientID%>').val()) {
	                            isValid = false;
	                            alertify.error(SwarmopsJS.unescape("<%=Localize_ErrorPasswordMismatch%>"));
	                            $('#<%=TextPassword1.ClientID%>,#<%=TextPassword2.ClientID%>').addClass("data-entry-error");
	                            $('#<%=TextPassword1.ClientID%>').focus();
	                        }
	                    }


    	            } else if (stepNumber == 4) {
    	                isValid = true; // assume true, make false as we go

    	                if (suppressChecks) {
	                        return true;
	                    }

    	                var selectedOption = $('input:radio[name="ActivationLevel"]:checked').val();
    	                if (selectedOption === undefined) {
    	                    isValid = false;
    	                    alertify.error(SwarmopsJS.unescape("<%=Localize_ErrorSelectActivationLevel%>"));
    	                }

    	                if (selectedOption == "RadioActivationVolunteer" && isValid) {

                            // If we ARE an officer volunteer...

    	                    setTimeout(function() {
	                            $('#tableVolunteerPositions').datagrid({ url: '/Pages/v5/Public/Json-SignupVolunteerPositions.aspx?OrganizationId=<%=Organization.Identity%>&GeographyId=' + currentGeographyId });
	                        }, 250);
    	                } else {
    	                    // Otherwise, jump to step 6

    	                    // This is NOT how you do it. The SmartWizard doesn't want to do DisableStep (so that a step is skipped), and doesn't do GoToStep either.
                            // These hacks mean that backstepping becomes shit ugly if you go to step 6 and backstep to 4. Need to fix. BUGBUG TODO

    	                    if (!suppressChecks) {
    	                        suppressChecks = true;
    	                        $('#wizard').smartWizard('goToStep', '6');
	                            setTimeout(function() {
	                                $('#step-5').hide();
	                                $('a.buttonNext').addClass('buttonDisabled');
	                                $('a[rel="5"]').removeClass('selected').removeClass('done').addClass('disabled');
	                            }, 250); // needs to be at least 250ms to happen after... what? BUG here, there has to be a better way to do this
	                            suppressChecks = false;
	                        }
	                    }

	                } else if (stepNumber == 5) {
    	                isValid = true; // assume true, make false as we go

    	                if ($('input:radio[name="ActivationLevel"]:checked').val() == "RadioActivationVolunteer") {
	                        // if step 5 active...

	                        selectedPositions = $('#tableVolunteerPositions').datagrid('getChecked');
	                        if (selectedPositions.length == 0) {
	                            isValid = false;
	                            alertify.error(SwarmopsJS.unescape("<%=Localize_ErrorSelectVolunteerPosition%>"));
	                        }
	                    } else {
	                        // If we should just skip step 5
	                        $('a[rel="5"]').addClass("disabled").addClass("crossout");
	                        selectedPositions = {};
	                    }

	                } else if (stepNumber == 6) {
    	                // This will never trigger in the stock signup - Finish doesn't trigger Validate
	                }

    	            return isValid;
    	        }

	            $('#<%=this.DropCountries.ClientID%>').change(function() {
	                UpdatePostalPrefix('oldValueDummy', $('#<%=this.DropCountries.ClientID%>').val());
	            });

    	        $('input:radio[name="ActivationLevel"]').click(function() {
	                if ($('input:radio[name="ActivationLevel"]:checked').val() == "RadioActivationVolunteer") {
	                    $('#wizard').smartWizard('enableStep', '5');
	                    $('a[rel="5"]').removeClass("crossout");
	                } else {
	                    $('#wizard').smartWizard('disableStep', '5');
	                    $('a[rel="5"]').addClass("crossout");
	                    selectedPositions = {};
                    }
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


    	    function setLanguage(cultureCode) {
	            if (document.location.href.indexOf("?OrganizationId") > 0) {
	                document.location = document.location + "&Culture=" + cultureCode;
	            } else {
	                document.location = document.location + "?Culture=" + cultureCode;
	            }
	        }

    	    var suppressChecks = false;
    	    var selectedPositions = {};
	        var askParticipantStreet = <%=this.Organization.Parameters.AskParticipantStreet? "true": "false"%>;

    	</script>
	
    <!-- Main menu, dynamically constructed -->
        
        
    <div class="topmenu" style="margin-top: -4px; padding-top: 12px; color: white; font-family: Ubuntu; font-weight: 300; font-size: 24px; letter-spacing: 1px">
      	<div class="center980px">
            <span style="padding-left: 15px; padding-right: 15px"><asp:Label ID="LabelHeader" runat="server" /></span>
        </div>
    </div>

    <div class="center980px">

        <div class="mainbar">
            <!--<div id="page-icon-encaps"><asp:Image ID="IconPage" runat="server" ImageUrl="~/Images/PageIcons/iconshock-ignitionkey-40px.png" /></div><h1><asp:Label ID="LabelPageTitle" Text="Installation" runat="server" /></h1>-->
        
        <div class="box">
            <div class="content">
                
                <div id="wizard" class="swMain">
  			        <ul>
  				        <li><a href="#step-1">
                        <label class="stepNumber">1</label>
                        <span class="stepDesc">
                            <asp:Label runat="server" ID="LabelStep1Header" /><br />
                            <small><asp:Label runat="server" ID="LabelStep1Text" /></small>
                        </span>                   
                    </a></li>
  				        <li><a href="#step-2">
                        <label class="stepNumber">2</label>
                        <span class="stepDesc">
                            <asp:Label runat="server" ID="LabelStep2Header" /><br />
                            <small><asp:Label runat="server" ID="LabelStep2Text" /></small>
                        </span>
                    </a></li>
  				        <li><a href="#step-3">
                        <label class="stepNumber">3</label>
                        <span class="stepDesc">
                            <asp:Label runat="server" ID="LabelStep3Header" /><br />
                            <small><asp:Label runat="server" ID="LabelStep3Text" /></small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-4">
                        <label class="stepNumber">4</label>
                        <span class="stepDesc">
                            <asp:Label runat="server" ID="LabelStep4Header" /><br />
                            <small><asp:Label runat="server" ID="LabelStep4Text" /></small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-5">
                        <label class="stepNumber">5</label>
                        <span class="stepDesc">
                            <asp:Label runat="server" ID="LabelStep5Header" /><br />
                            <small><asp:Label runat="server" ID="LabelStep5Text" /></small>
                        </span>                   
                     </a></li>
  				        <li><a href="#step-6">
                        <label class="stepNumber">6</label>
                        <span class="stepDesc">
                            <asp:Label runat="server" ID="LabelStep6Header" /><br />
                            <small><asp:Label runat="server" ID="LabelStep6Text" /></small>
                        </span>                   
                    </a></li>
  			        </ul>
  			        <div id="step-1">	
                        <h2><asp:Label ID="LabelWelcomeHeader" runat="server" /></h2>
			              <p><asp:Literal ID="LiteralFirstPageSignup" runat="server"/></p><br/><br/>
                    </div>
  			        <div id="step-2">
  			            <div class="data-entry-fields-admin">
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextName" /></div>
                            <div class="stacked-input-control"><asp:DropDownList runat="server" ID="DropCountries"/></div>
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextMail" /></div>
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPhone" /></div>
                            <div class="enableAskParticipantStreet"><div class="stacked-input-control"><asp:TextBox runat="server" ID="TextStreet1" /></div>
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextStreet2" /></div></div>
                            <div class="stacked-input-control"><div class="float-far"><asp:TextBox runat="server" ID="TextPostal" />&thinsp;<asp:TextBox runat="server" ID="TextCity" /></div><div class="stacked-text" style="width: 22px; overflow-x: hidden"><span id="spanCountryPrefix">XX</span>--</div></div>
                            <div class="stacked-input-control"><span class="stacked-text" id="spanDetectedGeo">...</span></div>
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextDateOfBirth" /></div>
                            <div class="stacked-input-control"><asp:DropDownList runat="server" ID="DropGenders" /></div>
                        </div>
                        <div class="entry-labels-admin">
                            <asp:Label ID="LabelName" runat="server" /><br />
                            <asp:Label ID="LabelCountry" runat="server" /><br />
                            <asp:Label ID="LabelMail" runat="server" /><br />
                            <asp:Label ID="LabelPhone" runat="server" /><br />
                            <div class="enableAskParticipantStreet"><asp:Label ID="LabelStreet1" runat="server" /><br />
                            <asp:Label ID="LabelStreet2" runat="server" /><br /></div>
                            <span id="spanLabelPostal"><asp:Label ID="LabelPostalCode" runat="server" />,&nbsp;</span><asp:Label runat="server" ID="LabelCity" /><br />
                            <span id="SpanGeoDetected"><asp:Label ID="LabelGeographyDetected" runat="server" /></span><span id="SpanGeoSelect" style="display: none"><asp:Label ID="LabelSelectGeography" runat="server" Text="XYZ Select Geography" /></span><br />
                            <asp:Label ID="LabelDateOfBirth" runat="server" /><br />
                            <asp:Label ID="LabelLegalGender" runat="server" /><br />
                        </div>
                    </div>                      
  			        <div id="step-3">
  			            <h2><asp:Label runat="server" ID="LabelYourLogon" /></h2>
                        <p><asp:Label ID="LabelYourLogonText" runat="server" /></p>
  			            <div class="data-entry-fields-admin" style="width:204px;overflow:hidden">
  			                <div class="stacked-input-control"><span id="spanMailLoginKey" style="white-space:nowrap">...</span></div>
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPassword1" TextMode="Password" /></div>
                            <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPassword2" TextMode="Password" /></div>
                        </div>
                        <div class="entry-labels-admin">
                            <asp:Label ID="LabelLoginKey" runat="server" /><br />
                            <asp:Label ID="LabelPassword1" runat="server" /><br />
                            <asp:Label ID="LabelPassword2" runat="server" /><br />
                        </div>
                    </div>
                    <div id="step-4">
                        <h2><asp:Label ID="LabelActivationLevelHeader" runat="server" /></h2>
                        <p><asp:Label ID="LabelActivationLevelIntro" runat="server" /></p>
                        <asp:RadioButton runat="server" ID="RadioActivationPassive" GroupName="ActivationLevel" />
                        <p><asp:Label runat="server" ID="LabelActivationPassiveText" /></p>
                        <asp:RadioButton runat="server" ID="RadioActivationActive" GroupName="ActivationLevel" />
                        <p><asp:Label runat="server" ID="LabelActivationActiveText" /></p>
                        <asp:RadioButton runat="server" ID="RadioActivationVolunteer" GroupName="ActivationLevel" />
                        <p><asp:Label runat="server" ID="LabelActivationVolunteerText" /></p>
                    </div>
  			        <div id="step-5">
                          <h2><asp:Label ID="LabelVolunteerPositionHeader" runat="server" /></h2>
                          <p><asp:Label ID="LabelVolunteerPositionText" runat="server" /></p>
                          <table id="tableVolunteerPositions" class="easyui-datagrid" style="width:460px;height:275px"
                                    data-options="idField:'positionId',singleSelect:false,fitColumns:true,checkOnSelect:true,selectOnCheck:true"
                                    >
                              <thead>
                                  <tr>
                                      <th data-options="field:'ck',checkbox:true"></th>
                                      <th data-options="field:'positionTitle',width:120"><asp:Label ID="LabelVolunteerHeaderPositionTitle" runat="server" /></th>
                                      <th data-options="field:'highestGeography',width:220"><asp:Label runat="server" ID="LabelVolunteerHeaderHighestGeography" /></th>
                                  </tr>
                              </thead>
                          </table>
                          <p><asp:Label ID="LabelVolunteerLevelIntro" runat="server" /></p>
                    </div>
  			        <div id="step-6">
  			            <div id="divStep6NoPayment">
                          <h2><asp:Label ID="LabelFinalizeSignupHeader" runat="server" /></h2>
			                  <p><asp:Literal ID="LiteralLastPageSignup" runat="server"/></p>
  			            </div>
                          <div id="divStep6Payment" style="display:none"><!-- todo --></div>
                    </div>
      		    </div>

            </div>
        </div>
        
        </div>
        <div class="sidebar">
            
                <div class="box"><div class="content">
        <asp:Image runat="server" Width="220" ID="ImageLogo"/>
    </div></div>            



    <div id="divLanguageSelector">

    <h2 class="blue">Language<span class="arrow"></span></h2>
    <div class="box">
        <div class="content">
            &nbsp;<a href="javascript:setLanguage('ar-SA')"><img class="action-icon flag" src="/Images/Flags/Arabic-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('pt-BR')"><img class="action-icon flag" src="/Images/Flags/br-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('zh-CHS')"><img class="action-icon flag" src="/Images/Flags/cn-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('de-DE')"><img class="action-icon flag"  src="/Images/Flags/de-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('yo-NG')"><img class="action-icon flag"  src="/Images/Flags/ng-64px.png"/></a><br />
            &nbsp;<a href="javascript:setLanguage('el-GR')"><img class="action-icon flag"  src="/Images/Flags/gr-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('es-ES')"><img class="action-icon flag"  src="/Images/Flags/es-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('es-VE')"><img class="action-icon flag"  src="/Images/Flags/ve-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('fr-FR')"><img class="action-icon flag" src="/Images/Flags/fr-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('fil-PH')"><img class="action-icon flag" src="/Images/Flags/ph-64px.png"/></a><br />
            &nbsp;<a href="javascript:setLanguage('it-IT')"><img class="action-icon flag" src="/Images/Flags/it-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('nl-NL')"><img class="action-icon flag" src="/Images/Flags/nl-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('pl-PL')"><img class="action-icon flag" src="/Images/Flags/pl-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('pt-PT')"><img class="action-icon flag" src="/Images/Flags/pt-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('ru-RU')"><img class="action-icon flag" src="/Images/Flags/ru-64px.png"/></a><br/>
            &nbsp;<a href="javascript:setLanguage('sr-Cyrl-RS')"><img class="action-icon flag" src="/Images/Flags/rs-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('sr-Latn-RS')"><img class="action-icon flag" src="/Images/Flags/rs-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('sv-SE')"><img class="action-icon flag" src="/Images/Flags/se-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('tr-TR')"><img class="action-icon flag" src="/Images/Flags/tr-64px.png"/></a>&#8203;
            <a href="javascript:setLanguage('en-US')"><img class="action-icon flag" src="/Images/Flags/uk-64px.png"/></a>
        </div>
    </div>

    </div>

    <h2 class="blue"><asp:Label ID="LabelSidebarInfoHeader" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
        <asp:Label ID="LabelSidebarInfoContent" runat="server" />
        </div>
    </div>

    <div id="divTodoDerpage" style="display:none">
    
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
                    <asp:Label ID="LabelSidebarTodo" runat="server" />
                </div>
            </div>
        </div>
    </div>
        
    </div>

	</div>

	</form>
</body>
    
    <!-- active culture: <%=System.Threading.Thread.CurrentThread.CurrentUICulture.EnglishName %> -->

</html>





        


