<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AddPerson.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Swarm.AddPerson" %>
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

        var onDocReadyAlert = '<asp:Literal ID="LiteralLoadAlert" runat="server" />';

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextName" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" OnClientChange=" UpdatePostalPrefix " ID="DropCountries"/>&#8203;<br/>        
        <asp:TextBox runat="server" ID="TextMail" />&#8203;<br/>
        <asp:TextBox runat="server" ID="TextPhone" />&#8203;<br/>
        &nbsp;<br/>
        <asp:TextBox runat="server" ID="TextStreet1" />&#8203;<br/>
        <asp:TextBox runat="server" ID="TextStreet2" />&#8203;<br/>
        <div style="float: right; margin-right: 5px"><asp:TextBox runat="server" ID="TextPostal" />&nbsp;<asp:TextBox runat="server" ID="TextCity" /></div><div style="width: 40px; overflow: hidden"><span id="spanCountryPrefix">XX</span>&ndash;</div>
        <span id="spanDetectedGeo">...</span>&nbsp;<br/>
        &nbsp;<br/>
        <asp:TextBox runat="server" ID="TextDateOfBirth" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropGenders" />&#8203;<br/>
        <asp:Button ID="ButtonSubmit" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick=" return ValidateFields(); " OnClick="ButtonSubmit_Click" Text="Register"/>
    </div>
    <div class="entryLabels">
        <asp:Label ID="LabelName" runat="server" /><br />
        <asp:Label ID="LabelCountry" runat="server" /><br />
        <asp:Label ID="LabelMail" runat="server" /><br />
        <asp:Label ID="LabelPhone" runat="server" /><br />
        <h2><asp:Label ID="LabelHeaderAddresss" runat="server" /></h2>
        <asp:Label ID="LabelStreet1" runat="server" /><br />
        <asp:Label ID="LabelStreet2" runat="server" /><br />
        <span id="spanLabelPostal" style="display: inline-block; overflow: hidden"><asp:Label ID="LabelPostalCode" runat="server" />,&nbsp;</span><span style="display: inline-block; overflow: hidden"><asp:Label runat="server" ID="LabelCity" /></span><br />
        <span id="SpanGeoDetected"><asp:Label ID="LabelGeographyDetected" runat="server" /></span><span id="SpanGeoSelect" style="display: none"><asp:Label ID="LabelSelectGeography" runat="server" Text="XYZ Select Geography" /></span><br />
        <h2><asp:Label ID="LabelHeaderStatData" runat="server" /></h2>
        <asp:Label ID="LabelDateOfBirth" runat="server" /><br />
        <asp:Label ID="LabelLegalGender" runat="server" /><br />
    </div>
    <div style="clear: both"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>