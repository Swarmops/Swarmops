<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Signup.aspx.cs" Inherits="Swarmops.Frontend.Pages.Public.Signup" %>


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

        input:not([type="radio"]), select {
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


        .swMain ul.anchor li a.disabled, .swMain ul.anchor li a.disabled:hover {  /* cross out step 5 if it is skipped - if so, it gets marked disabled */
            background: 
                   linear-gradient(to top left,
                       rgba(0,0,0,0) 0%,
                       rgba(0,0,0,0) calc(50% - 2px),
                       rgba(0,0,0,.2) 50%,
                       rgba(0,0,0,0) calc(50% + 2px),
                       rgba(0,0,0,0) 100%),
                   linear-gradient(to top right,
                       rgba(0,0,0,0) 0%,
                       rgba(0,0,0,0) calc(50% - 2px),
                       rgba(0,0,0,.2) 50%,
                       rgba(0,0,0,0) calc(50% + 2px),
                       rgba(0,0,0,0) 100%);
        }


        .entryFieldsAdmin, .entryLabelsAdmin {
            font-size: 16px;
        }


        .entryLabelsAdmin {
            width: 236px;
            font-weight: 300;
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


    	    var currentGeographyId = 0;


    	    $(document).ready(function () {

    	        // Doc.Ready instance 2. Smart Wizard initialization.
    	        // This is a separate Doc.Ready as I hope to break out the
    	        // above code some day to a common file between this file
    	        // and Swarm / Add.  (TODO.)


    	        $('#wizard').smartWizard({
    	            transitionEffect: 'fade',
    	            labelNext: "<asp:Literal ID='LiteralWizardNextButton' runat='server' />",
    	            labelFinish: "<asp:Literal ID='LiteralWizardFinishButton' runat='server' />",
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
	                    isValid = ValidateTextField('#<%= TextPassword1.ClientID %>', "<asp:Literal runat='server' ID='LiteralErrorNeedPassword' />");

	                    if (isValid) {
	                        if ($('#<%=TextPassword1.ClientID%>').val() != $('#<%=TextPassword2.ClientID%>').val()) {
	                            isValid = false;
	                            alertify.error("<asp:Literal runat='server' ID='LiteralErrorPasswordMismatch' />");
	                            $('#<%=TextPassword1.ClientID%>,#<%=TextPassword2.ClientID%>').addClass("entryError");
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
    	                    alertify.error("<asp:Literal runat='server' ID='LiteralErrorSelectActivationLevel' />");
    	                }

	                    if (selectedOption != "RadioActivationVolunteer" && isValid) {
	                        suppressChecks = true; // prevents foreverlooping into this check
	                        $('#wizard').smartWizard('goToStep', 6);
	                        setTimeout(function() {
	                            $('a[rel="5"]').addClass("disabled").removeClass("selected");
	                            $('a.buttonNext').addClass("buttonDisabled");  // hacks because SmartWizard doesn't handle skipping steps
	                        }, 250);
	                        suppressChecks = false;
	                    }

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


    	    function setLanguage(cultureCode) {
	            document.location = document.location + "&Culture=" + cultureCode;
	        }

    	    var suppressChecks = false;

    	</script>
	
    <!-- Main menu, dynamically constructed -->

	<div class="center980px">
        <div class="topmenu" style="margin-top: -4px; padding-top: 12px; color: white; font-family: Ubuntu; font-weight: 300; font-size: 24px; letter-spacing: 1px">
            <span style="padding-left: 30px; padding-right: 30px"><asp:Label ID="LabelHeader" runat="server" /></span>
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
                        <p>This is the organization's custom welcome text. It has not yet been written; it is set in Admin / Org Settings.</p><br/><br/>
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
                            <div class="elementFloatFar" style="margin-right: -1px"><asp:TextBox runat="server" ID="TextPostal" />&nbsp;<asp:TextBox runat="server" ID="TextCity" /></div><div style="width: 35px; overflow: hidden"><span id="spanCountryPrefix">XX</span>&ndash;</div>
                            <span id="spanDetectedGeo">...</span>&nbsp;<br/>
                            <asp:TextBox runat="server" ID="TextDateOfBirth" />&#8203;<br/>
                            <asp:DropDownList runat="server" ID="DropGenders" />&#8203;<br/>
                        </div>
                    </div>                      
  			        <div id="step-3">
  			            <h2><asp:Label runat="server" ID="LabelYourLogon" /></h2>
                        <p><asp:Label ID="LabelYourLogonText" runat="server" /></p>
                        <div class="entryLabelsAdmin">
                            <asp:Label ID="LabelLoginKey" runat="server" /><br />
                            <asp:Label ID="LabelPassword1" runat="server" /><br />
                            <asp:Label ID="LabelPassword2" runat="server" /><br />
                        </div>
  			            <div class="entryFieldsAdmin">
  			                <span id="spanMailLoginKey">...</span>&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextPassword1" TextMode="Password" />&#8203;<br/>
                            <asp:TextBox runat="server" ID="TextPassword2" TextMode="Password" />&#8203;<br/>
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
                          <!-- add datagrid with checkboxes here -->
                          <p><asp:Label ID="LabelVolunteerLevelIntro" runat="server" /></p>
                    </div>
  			        <div id="step-6">
                    </div>
      		    </div>

            </div>
        </div>
        
        </div>
        <div class="sidebar">
            
    <div id="divLanguageSelector">

    <h2 class="blue">Language<span class="arrow"></span></h2>
    <div class="box">
        <div class="content">
            &nbsp;<a href="javascript:setLanguage('ar-SA')"><img src="/Images/Flags/Arabic-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('pt-BR')"><img src="/Images/Flags/br-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('zh-CHS')"><img src="/Images/Flags/cn-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('de-DE')"><img src="/Images/Flags/de-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('da-DK')"><img src="/Images/Flags/dk-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('es-ES')"><img src="/Images/Flags/es-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('fi-FI')"><img src="/Images/Flags/fi-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('fr-FR')"><img src="/Images/Flags/fr-24px.png"/></a><br/>
            &nbsp;<a href="javascript:setLanguage('el-GR')"><img src="/Images/Flags/gr-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('hi-IN')"><img src="/Images/Flags/in-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('is-IS')"><img src="/Images/Flags/is-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('nl-NL')"><img src="/Images/Flags/nl-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('nb-NO')"><img src="/Images/Flags/no-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('ru-RU')"><img src="/Images/Flags/ru-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('sv-SE')"><img src="/Images/Flags/se-24px.png"/></a>&#8203;
            <a href="javascript:setLanguage('en-US')"><img src="/Images/Flags/us-24px.png"/></a>&#8203;
        </div>
    </div>

    </div>

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
    
    <div id="divTodoDerpage" style="display:none">

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





        


