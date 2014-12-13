<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="PopulateCountry2.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.Hacks.PopulateCountry2" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="/Scripts/jquery.fileupload/jquery.iframe-transport.js" type="text/javascript" language="javascript"></script>
    <!-- The basic File Upload plugin -->
    <script src="/Scripts/jquery.fileupload/jquery.fileupload.js" type="text/javascript" language="javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
             
        });

        function uploadCompletedCallback() {
            $('#DivProcessing').slideDown().fadeIn();
            $('#DivProgressProcessing').progressbar({ value: 0, max: 100 });

            $.ajax({
                type: "POST",
                url: "PopulateCountry2.aspx/InitializeProcessing",
                data: "{'guid': '<%= this.UploadFile.GuidString %>'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    setTimeout('updateProgressProcessing();', 1000);
                }
            });
        }

        function updateProgressProcessing() {

            $.ajax({
                type: "POST",
                url: "/Automation/Json-ByGuid.aspx/GetProgress",
                data: "{'guid': '<%= this.UploadFile.GuidString %>'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d > 99) {
                        $("#DivProgressProcessing .progressbar-value").animate(
                            {
                                width: "100%"
                            }, { queue: false });
                        
                        window.location = '/';

                    } else {

	                    // We're not done yet. Keep the progress bar on-screen and keep re-checking.

                            if (msg.d == 1) {
                                $('#DivProgressProcessing').progressbar({ value: 1 });
                            } else {
                                $("#DivProgressProcessing .progressbar-value").animate(
                                {
                                    width: msg.d + "%"
                                }, { queue: false });
                            }

                            if (msg.d > 0 && !progressReceived) {
                                progressReceived = true;
                            }

                            setTimeout('updateProgressProcessing();', 1000);
                        }
	            },
                error: function (msg) {
                    // Retry on call error, too
                    setTimeout('updateProgressProcessing();', 1000);
                }
            });
        }


        var progressReceived = false;

    </script>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="DivEntry">
        <div class="entryFields">
            <Swarmops5:FileUpload runat="server" ID="UploadFile" Filter="NoFilter" DisplayCount="8" ClientUploadCompleteCallback="uploadCompletedCallback" />
        </div>
        
        <div class="entryLabels">
            Upload file for processing<br/><br/>
        </div>
    </div>
    <div clear="both"></div>
    <div id="Processing">
    <div id="DivProgressProcessing"></div>
    </div>

</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

