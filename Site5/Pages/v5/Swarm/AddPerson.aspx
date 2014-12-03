<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AddPerson.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Swarm.AddPerson" %>
<%@ Register TagPrefix="Swarmops5" TagName="DropDown" Src="~/Controls/v5/Base/DropDown.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function() {
            UpdatePostalPrefix('XX', $('#<%=DropCountries.ClientControlID%>').val());

            $('#<%=this.TextPostal.ClientID%>').on ('input', function() {
                var currentPostalCode = $('#<%=this.TextPostal.ClientID%>').val();
                if (currentPostalCode in postalCodeLookup) {
                    $('#<%=this.TextCity.ClientID%>').attr("disabled","disabled").val(postalCodeLookup[currentPostalCode]);
                    $('#<%=this.TextDateOfBirth.ClientID%>').focus();
                }
            });

            alert('bah');
        });

        function UpdatePostalPrefix(oldValue, newValue) {
            $('#SpanCountryPrefix').text(newValue);

            // Get postal code data for country. Heavy op; should not be done from mobile

            var jsonData = {};
            jsonData.countryCode = newValue;
            alert(newValue);

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.aspx/GetPostalCodes",
                data: $.toJSON(jsonData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    postalCodeLookup = {};
                    msg.d.forEach(function (element, index, array) {
                        postalCodeLookup[element.Code] = element.Name;
                    });
                },
                error: function (msg) {
                    console.log(msg);
                    postalCodeLookup = {};
                }
            });
        }

        var postalCodeLookup = {};
        var postalCodeLength = 0

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
        <div style="float:right;margin-right:5px"><asp:TextBox runat="server" ID="TextPostal" />&nbsp;<asp:TextBox runat="server" ID="TextCity" /></div><div style="width:40px;overflow:hidden"><span id="SpanCountryPrefix">XX</span>&ndash;</div>
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
        <h2>DEMOGRAPHIC DATA</h2>
        Date of Birth<br />
        Legal gender<br />
        <h2>SIGNING UP AS</h2>
        [Regular] until<br />
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>