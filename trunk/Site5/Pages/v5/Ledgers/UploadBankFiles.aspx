<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="UploadBankFiles.aspx.cs" Inherits="Swarmops.Site.Pages.Ledgers.UploadBankFiles" %>
<%@ Register src="~/Controls/v5/UI/ExternalScripts.ascx" tagname="ExternalScripts" tagprefix="Swarmops5" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="/Scripts/jquery.fileupload/jquery.iframe-transport.js" type="text/javascript" language="javascript"></script>
    <!-- The basic File Upload plugin -->
    <script src="/Scripts/jquery.fileupload/jquery.fileupload.js" type="text/javascript" language="javascript"></script>
	<link rel="stylesheet" type="text/css" href="/Style/v5-easyui-elements.css">

    <style type="text/css">
        
        /* custom styles were used in Telerik version of page */
         
    </style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

    <script type="text/javascript">

        $(document).ready(function() {
            $('#<%=DropAccounts.ClientID %>').change(function() {
                var selectedAccountId = $('#<%=DropAccounts.ClientID %>').val();

                if (selectedAccountId != 0) {
                    $('#<%=this.UploadFile.ClientID %>_ButtonUploadVisible').fadeIn();
                } else {
                    $('#<%=this.UploadFile.ClientID %>_ButtonUploadVisible').fadeOut();
                }
            });

        });

        function uploadCompletedCallback() {
            $('#DivProcessing').fadeIn();
            $('#DivProgressProcessing').progressbar({ value: 0, max: 100 });
            $('#DivProgressFake').progressbar({ value: 0, max: 100 });
            $('#DivProgressFake').progressbar({ value: 100 }); // this guy is swapped in on completion
            halfway = false;

            $.ajax({
                type: "POST",
                url: "UploadBankFiles.aspx/InitializeProcessing",
                data: "{'guid': '<%= this.UploadFile.GuidString %>', 'accountIdString':'" + $('#<%= this.DropAccounts.ClientID %>').val() + "'}",
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
	                    $('#DivProcessing').hide();
                        $('#DivProcessingFake').show();
	                    
	                    // Add processing results here
	                } else {

	                    if (msg.d > 50 && !halfway) {
	                        halfway = true;
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
                            $.ajax({
                                type: "POST",
                                url: "UploadBankFiles.aspx/GetProcessingStatistics",
                                data: "{'guid': '<%= this.UploadFile.GuidString %>'}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg2) {/*
                                    $('#SpanFirstTx').text(msg2.d.FirstTransaction);
                                    $('#SpanLastTx').text(msg2.d.LastTransaction);
                                    $('#SpanTxCount').text(msg2.d.TransactionCount);*/
                                }
                            });
                        }

                        setTimeout('updateProgressProcessing();', 1000);
                    }
	            }
	        });
        }


        var progressReceived = false;

        var halfway = false;

        var currentYear = <%=DateTime.Today.Year %>;


    </script>
    
    <div id="DivProcessingFake" style="display:none">
        <h2><asp:Label ID="LabelProcessingComplete" runat="server" /></h2>
        <div id="DivProgressFake" style="width:100%"></div>

        <div id="DivUploadResultsGood">
       
        </div>

        <div id="DivUploadResultsBad">
            <div float="left"><img src="/Images/Icons/iconshock-cross-96px.png" /></div><div id="DivUploadResultsBadText"></div>
        </div>
        
        <div id="DivUploadResultsQuestionable">
            <div float="left"><img src="/Images/Icons/iconshock-warning-96px.png" /></div><div id="DivUploadResultsQuestionableText"></div>
        </div>
    </div>

    <div id="DivUploadBankFile">
        <h2><asp:Label runat="server" ID="LabelUploadBankFileHeader" /></h2>
        <div id="DivPrepData">
        
            <div class="entryFields">
                <asp:DropDownList runat="server" ID="DropAccounts"/>
                (Account statement)<br/>
                (Instructions - TODO)<br/>
                <Swarmops5:FileUpload runat="server" ID="UploadFile" Filter="NoFilter" DisplayCount="8" HideTrigger="true" ClientUploadCompleteCallback="uploadCompletedCallback" /></div>
        
            <div class="entryLabels">
                <asp:Label runat="server" ID="LabelBankAccount" /><br/>
                <asp:Label runat="server" ID="LabelFileType" /><br/>
                <asp:Label runat="server" ID="LabelInstructions" /><br/>
                <asp:Label runat="server" ID="LabelUploadBankFile" />
            </div>
        </div>
    
        <br clear="all"/>
    </div>
    
    <div id="DivProcessing" style="display:none">
        <h2><asp:Label runat="server" ID="LabelProcessing" /></h2>
        <div id="DivProgressProcessing" style="width:100%"></div>
    </div>


    <div id="DivStepSuccessComplete" style="display:none">
        <h2>Resynchronization complete</h2>
        <p>All Swarmops records (<span id="SpanRecordsTotal">[...]</span>) have been successfully resynchronized with the master file.</p>
        <p>You may <a href="/">return to dashboard</a> if you like.</p>
    </div>
    <div id="DivStepSuccessPartial" style="display:none">
        <h2>Resynchronization partially complete</h2>
        <p>Most Swarmops records (<span id="SpanRecordsSuccess">[...]</span>) have been successfully resynchronized with the master file. <strong>Some (<span id="SpanRecordsFail">[...]</span>) have not.</strong></p>
        <p>Due to dependencies, manual action is required for the remaining records. Run resynchronization <a href="#" onclick="location.reload();">again</a> to see which records could not be automatically resynchronized.</p>
    </div>
    <div id="DivStepFailure" style="display:none">
        <h2>Resynchronization partially complete</h2>
        <p>Most Swarmops records (<span id="Span1">[...]</span>) have been successfully resynchronized with the master file. <strong>Some (<span id="Span2">[...]</span>) have not.</strong></p>
        <p>Due to dependencies, manual action is required for the remaining records. Run resynchronization <a href="#" onclick="location.reload();">again</a> to see which records could not be automatically resynchronized.</p>
    </div>

    <br/>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

