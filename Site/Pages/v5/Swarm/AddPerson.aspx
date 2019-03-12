<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Swarm.AddPerson" CodeFile="AddPerson.aspx.cs" %>
<%@ Register TagPrefix="Swarmops5" TagName="DropDown" Src="~/Controls/v5/Base/DropDown.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            UpdatePostalPrefix('XX', $('#<%= DropCountries.ClientControlID %>').val());

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

            if (onDocReadyAlert.length > 0) {
                alertify.log(onDocReadyAlert);
            }
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
                var postalTargetLength = postalCodeLength * 15 - 5;
                var cityTargetLength = 235 - 15 * postalCodeLength;

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

    function UpdatePostalPrefix(newValue, oldValue) {
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

        function ValidateFields() {
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
                            alertify.error(SwarmopsJS.unescape('<%= this.Localized_ErrorDate %>'));
                            $('#<%=this.TextDateOfBirth.ClientID %>').focus();
                        }
                    }
                });
            }

            isValid = ValidateTextField('#<%= TextCity.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ErrorCity %>')) && isValid;
            isValid = ValidateTextField('#<%= TextStreet1.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ErrorStreet %>')) && isValid;
            isValid = ValidateTextField('#<%= TextMail.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ErrorMail %>')) && isValid;
            isValid = ValidateTextField('#<%= TextName.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ErrorName %>')) && isValid; // TODO: Actually validate geography?

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

        var onDocReadyAlert = '<asp:Literal ID="LiteralLoadAlert" runat="server" />';

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
    <div class="data-entry-fields">
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextName" /></div>
        <Swarmops5:DropDown runat="server" OnClientChange=" UpdatePostalPrefix " ID="DropCountries"/>        
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextMail" /></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPhone" /></div>
        <div class="stacked-input-control"></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextStreet1" /></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextStreet2" /></div>
        <div class="small-margin-right float-far"><asp:TextBox runat="server" ID="TextPostal" CssClass="small-margin-right"/><asp:TextBox runat="server" ID="TextCity" /></div><div class="stacked-input-control stacked-text" style="width: 40px; overflow: hidden"><span id="spanCountryPrefix">XX</span>&ndash;</div>
        <div class="stacked-input-control stacked-text"><span id="spanDetectedGeo">...</span></div>
        <div class="stacked-input-control"></div>
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextDateOfBirth" /></div>
        <Swarmops5:DropDown runat="server" ID="DropGenders" />
        <asp:Button ID="ButtonSubmit" runat="server" CssClass="button-accent-color suppress-input-focus" OnClientClick=" return ValidateFields(); " OnClick="ButtonSubmit_Click" Text="Register"/>
    </div>
    <div class="data-entry-labels">
        <asp:Label ID="LabelName" runat="server" /><br />
        <asp:Label ID="LabelCountry" runat="server" /><br />
        <asp:Label ID="LabelMail" runat="server" /><br />
        <asp:Label ID="LabelPhone" runat="server" /><br />
        <h2><asp:Label ID="LabelHeaderAddresss" runat="server" /></h2>
        <asp:Label ID="LabelStreet1" runat="server" /><br />
        <asp:Label ID="LabelStreet2" runat="server" /><br />
        <span id="spanLabelPostal"><asp:Label ID="LabelPostalCode" runat="server" />,&nbsp;</span><asp:Label runat="server" ID="LabelCity" /><br />
        <span id="SpanGeoDetected"><asp:Label ID="LabelGeographyDetected" runat="server" /></span><span id="SpanGeoSelect" style="display: none"><asp:Label ID="LabelSelectGeography" runat="server" Text="XYZ Select Geography" /></span><br />
        <h2><asp:Label ID="LabelHeaderStatData" runat="server" /></h2>
        <asp:Label ID="LabelDateOfBirth" runat="server" /><br />
        <asp:Label ID="LabelLegalGender" runat="server" /><br />
    </div>
    <div style="clear: both"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>