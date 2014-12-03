<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AddPerson.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Swarm.AddPerson" %>
<%@ Register TagPrefix="Swarmops5" TagName="DropDown" Src="~/Controls/v5/Base/DropDown.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function() {
            UpdatePostalPrefix('XX', $('#<%=DropCountries.ClientControlID%>').val());

            $('#<%=this.TextPostal.ClientID%>').on('input', function() {
                if (CheckPostalCode()) {
                    $('#<%=this.TextDateOfBirth.ClientID%>').focus();
                }
            });
        });

        function CheckPostalCode() {
            var currentPostalCode = $('#<%=this.TextPostal.ClientID%>').val();
            if (currentPostalCode in postalCodeLookup) {
                var cityId = postalCodeLookup[currentPostalCode];
                $('#<%=this.TextCity.ClientID%>').attr("disabled", "disabled").val(cityNameLookup[cityId].Name);
                $('#spanDetectedGeo').text(geographyNameLookup[cityNameLookup[cityId].GeoId]);
                return true;
            } else {
                $('#<%=this.TextCity.ClientID%>').removeAttr("disabled").val('');
                $('#spanDetectedGeo').text('');
            }
            return false;
        }

        function AnimatePostalCodeLength() {
            var postalTargetLength = postalCodeLength * 15 - 5;
            var cityTargetLength = 235 - 15 * postalCodeLength;

            $('#<%=this.TextPostal.ClientID%>').animate({ width: postalTargetLength + "px" });
            $('#<%=this.TextCity.ClientID%>').animate({ width: cityTargetLength + "px" });

        }

        function UpdatePostalPrefix(oldValue, newValue) {
            $('#spanCountryPrefix').text(newValue);
            $('#<%=this.TextCity.ClientID%>').removeAttr("disabled").val('');
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
                success: function (msg) {
                    postalCodeLookup = {};
                    cityNameLookup = {};
                    geographyNameLookup = {};

                    postalCodeLength = msg.d.PostalCodeLength;
                    msg.d.PostalCodes.forEach(function (element, index, array) {
                        postalCodeLookup[element.Code] = element.CityId;
                    });
                    msg.d.CityNames.forEach(function(element, index, array) {
                        cityNameLookup[element.Id] = {};
                        cityNameLookup[element.Id].Name = element.Name;
                        cityNameLookup[element.Id].GeoId = element.GeographyId;
                    });
                    msg.d.Geographies.forEach(function(element, index, array) {
                        geographyNameLookup[element.Id] = element.Name;
                    });

                    AnimatePostalCodeLength();
                    CheckPostalCode();
                },
                error: function (msg) {
                    console.log(msg);
                    geographyNameLookup = {};
                    postalCodeLookup = {};
                    cityNameLookup = {};
                }
            });
        }

        var postalCodeLookup = {};
        var cityNameLookup = {};
        var geographyNameLookup = {};
        var postalCodeLength = 0;

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2>Adding [Regular]</h2>
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextName" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" OnClientChange="UpdatePostalPrefix" ID="DropCountries"/>&#8203;<br/>        
        <asp:TextBox runat="server" ID="TextMail" />&#8203;<br/>
        <asp:TextBox runat="server" ID="TextPhone" />&#8203;<br/>
        &nbsp;<br/>
        <asp:TextBox runat="server" ID="TextStreet1" />&#8203;<br/>
        <asp:TextBox runat="server" ID="TextStreet2" />&#8203;<br/>
        <div style="float:right;margin-right:5px"><asp:TextBox runat="server" ID="TextPostal" />&nbsp;<asp:TextBox runat="server" ID="TextCity" /></div><div style="width:40px;overflow:hidden"><span id="spanCountryPrefix">XX</span>&ndash;</div>
        <span id="spanDetectedGeo">...</span>&nbsp;<br/>
        &nbsp;<br/>
        <asp:TextBox runat="server" ID="TextDateOfBirth" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropGenders" />&#8203;<br/>
        &nbsp;<br/>
        <asp:Label ID="LabelExpiry" runat="server" Text="YYYY-MM-DD" />&#8203;<br/>
    </div>
    <div class="entryLabels">
        Name (First Last)<br />
        Country<br />
        Mail<br />
        Phone<br />
        <h2>ADDRESS</h2>
        Street 1<br />
        Street 2<br />
        Postal Code, City<br />
        <span id="SpanGeoDetected">Geography detected</span><span id="SpanGeoSelect" style="display:none">Select Geograhpy</span><br />
        <h2>STATISTICAL DATA</h2>
        Date of Birth<br />
        Legal gender<br />
        <h2>SIGNING UP AS</h2>
        [Regular] until<br />
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>