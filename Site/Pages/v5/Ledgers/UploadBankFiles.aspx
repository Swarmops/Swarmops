<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Site.Pages.Ledgers.UploadBankFiles" CodeFile="UploadBankFiles.aspx.cs" Codebehind="UploadBankFiles.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="/Scripts/jquery.fileupload/jquery.iframe-transport.js" type="text/javascript" language="javascript"></script>
    <!-- The basic File Upload plugin -->
    <script src="/Scripts/jquery.fileupload/jquery.fileupload.js" type="text/javascript" language="javascript"></script>

    <style type="text/css">
        
        /* add custom styles here */
         
    </style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

    <script type="text/javascript">

        $(document).ready(function() {

        });

        function onAccountChange(oldAccountId, newAccountId) {
            if (newAccountId != 0) {
                $('#<%=this.UploadFile.ClientID %>_ButtonUploadVisible').fadeIn();

                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/UploadBankFiles.aspx/GetAutomationProfileName",
                    {
                        guid: guid, accountId: newAccountId
                    },
                    function(result) {
                        $('#SpanInstructions').text(result.DisplayMessage);
                    });
            } else {
                $('#<%=this.UploadFile.ClientID %>_ButtonUploadVisible').fadeOut();
                $('#SpanInstructions').text("");
            }
        }

        function uploadCompletedCallback() {
            $('#DivProcessing').fadeIn();
            $('#DivProgressProcessing').progressbar({ value: 0, max: 100 });
            $('#DivProgressFake').progressbar({ value: 0, max: 100 });
            $('#DivProgressFake').progressbar({ value: 100 }); // this guy is swapped in on completion
            $('#DivProcessingFake').slideUp(); // in case second, third... file
            halfway = false;

            var selectedAccountId = <%=this.DropAccounts.ClientID%>_val();

            SwarmopsJS.ajaxCall (
                "/Pages/v5/Ledgers/UploadBankFiles.aspx/InitializeProcessing",
                { guid: guid, accountId: selectedAccountId },
                function (result) {
                    if (result.Success)
                    {
	                    setTimeout('updateProgressProcessing();', 1000);
	                }
                }
            );
        }


        function confirmFirstUpload() {
            
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
	                    $('#DivProcessing').hide();
	                    $('#DivProgressProcessingFake').show();
	                    $('#DivProcessingFake').show();
	                    
                        $.ajax({
                            type: "POST",
                            url: "/Pages/v5/Ledgers/UploadBankFiles.aspx/GetReportedImportResults",
                            data: "{'guid': '<%= this.UploadFile.GuidString %>'}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg2) {
                                    $('#DivUploadResultsGood').hide();
                                    $('#DivUploadResultsBad').hide();
                                    $('#DivUploadResultsQuestionable').hide();
                                    $('#DivUploadResultsPayments').hide();

                                    $('#DivUploadResults' + msg2.d.Category + 'Text').html(msg2.d.Html);
                                    $('#DivUploadResults' + msg2.d.Category).show();

                                    $('#<%=this.UploadFile.ClientID %>_ButtonUploadVisible').hide();

                                    $('#SpanUploadMore').show();
                                    $('#SpanUploadFirst').hide();
                                    <%=this.DropAccounts.ClientID %>_val('0');
                                    <%=this.UploadFile.ClientID%>_clear();
                                    $('#SpanInstructions').text('');
                                    setTimeout("$('#DivProgressFake').slideUp().fadeOut();", 1000);
                                    setTimeout("$('#DivUploadBankFile').slideDown();", 1000);

                                }
                        });
                        
	                } else {

	                    if (msg.d > 50 && !halfway) {
	                        halfway = true;
	                        $('#DivProgressFake').show(); // hidden if 2nd, 3rd... file
	                        $('#DivUploadBankFile').slideUp().fadeOut();
                        }

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
                error: function(msg) {
                    // Retry on call error, too
                    setTimeout('updateProgressProcessing();', 1000);
                }
	        });
        }


        var progressReceived = false;

        var guid = '<%=this.UploadFile.GuidString%>';

        var halfway = false;

        var currentYear = <%=DateTime.Today.Year %>;


    </script>
    
    <div id="DivProcessingFake" style="display:none; margin-bottom:10px">
        <h2><asp:Label ID="LabelProcessingComplete" runat="server" /></h2>
        <div id="DivProgressFake" style="width:100%"></div>

        <div id="DivUploadResultsGood" style="display:none"><div id="DivUploadResultsGoodText"></div></div>

        <div id="DivUploadResultsBad" style="display:none">
            <div style="float:left;margin-right:10px"><img src="/Images/Icons/iconshock-cross-96px.png" /></div><div id="DivUploadResultsBadText"></div>
        </div>
        
        <div id="DivUploadResultsQuestionable" style="display:none">
            <div style="float:left; margin-right: 10px; padding-top: 10px;padding-bottom:20px"><img src="/Images/Icons/iconshock-warning-96px.png" /></div><div id="DivUploadResultsQuestionableText"></div>
        </div>
        
        <div id="DivUploadResultsPayments" style="display:none">
            <div style="float:left;margin-right:10px"><img src="/Images/Icons/iconshock-cashregister-96px.png" /></div><div id="DivUploadResultsPaymentsText"></div>
        </div>
        
        <br clear="all"/>
    </div>

    <div id="DivUploadBankFile">
        <h2><span id="SpanUploadFirst"><asp:Label runat="server" ID="LabelUploadBankFileHeader" /></span><span id="SpanUploadMore" style="display:none"><asp:Label runat="server" ID="LabelUploadMore" /></span></h2>
        <div id="DivPrepData">
        
            <div class="data-entry-fields">
                <Swarmops5:AjaxDropDown OnClientChange="onAccountChange" runat="server" ID="DropAccounts"/>
                <div class="stacked-input-control"><span id="SpanInstructions"></span></div>
                <Swarmops5:FileUpload runat="server" ID="UploadFile" Filter="NoFilter" DisplayCount="8" HideTrigger="true" ClientUploadCompleteCallback="uploadCompletedCallback" />
            </div>
        
            <div class="data-entry-labels">
                <asp:Label runat="server" ID="LabelBankAccount" /><br/>
                <asp:Label runat="server" ID="LabelProfile" /><br/>
                <asp:Label runat="server" ID="LabelUploadBankFile" />
            </div>
        </div>
    
        <br clear="all"/>
    </div>
    
    <div id="DivProcessing" style="display:none">
        <h2><asp:Label runat="server" ID="LabelProcessing" /></h2>
        <div id="DivProgressProcessing" style="width:100%"></div>
    </div>

    <br/>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>
