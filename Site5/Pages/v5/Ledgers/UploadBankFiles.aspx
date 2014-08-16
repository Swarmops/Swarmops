<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="UploadBankFiles.aspx.cs" Inherits="Swarmops.Site.Pages.Ledgers.UploadBankFiles" %>
<%@ Register src="~/Controls/v5/UI/ExternalScripts.ascx" tagname="ExternalScripts" tagprefix="Swarmops5" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

<style type="text/css">
input.FileTypeImage
{
	width:90px;
	height:45px;
	border: 1px solid #1C397E !important;
	margin-right:20px;
	margin-top:7px;
	margin-bottom:6px;
	float:left;
}

input.FileTypeImageSelected
{
    box-shadow: 0px 0px 2px 2px #FFBC37;
    -moz-box-shadow: 0px 0px 2px 2px #FFBC37;
    -webkit-box-shadow: 0px 0px 2px 2px #FFBC37;
    border: 1px solid #C78B15 !important;
}

input.UnselectedType
{
	opacity:0.2;
}

div.Invisible
{
	display:none;
	opacity:0;
}

div.Visible
{
	dispay:inline;
	opacity:1;
}

div.BankUploadInstructionsImage
{
	float:right;
}

#lean_overlay {
    position: fixed;
    z-index:10000;
    top: 0px;
    left: 0px;
    height:100%;
    width:100%;
    background: #000;
    display: none;
}

#ModalDownloadInstructions
{
	display:none;
	background:white;
	width:720px;
    position: fixed;
    z-index: 11000;
    left: 50%;
    margin-left: -360px;
    top: 160px;
    opacity: 1;
    padding: 10px;
    border-radius: 5px;
    -moz-border-radius: 5px;
    -webkit-border-radius: 5px;
    box-shadow: 0px 0px 4px rgba(0,0,0,0.7);
    -webkit-box-shadow: 0 0 4px rgba(0,0,0,0.7);
    -moz-box-shadow: 0 0px 4px rgba(0,0,0,0.7);
}
</style>

<script type="text/javascript">
    if (!(jQuery().leanModal)) {
        alert('LeanModal plug-in has not been successfully loaded!');
    }
</script>

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
            $('#DivStepUpload').slideUp().fadeOut();
            $('#DivStepProcessing').fadeIn();
            $('#DivProgressProcessing').progressbar({ value: 0, max: 100 });

            $.ajax({
                type: "POST",
                url: "UploadBankAccount.aspx/InitializeProcessing",
                data: "{'guid': '<%= this.UploadFile.GuidString %>', 'accountIdString':'" + $('#<%= this.DropAccounts.ClientID %>').val() + "'}",
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                setTimeout('updateProgressProcessing();', 1000);
	            }
	        });

            // Set timeout to update progress bar
        }

        function updateProgressProcessing() {

            $.ajax({
                type: "POST",
                url: "UploadBankFiles.aspx/GetProcessingProgress",
                data: "{'guid': '<%= this.UploadFile.GuidString %>'}",
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function (msg) {
	                if (msg.d > 99) {
	                    $("#DivProgressProcessing .progressbar-value").animate(
                            {
                                width: "100%"
                            }, { queue: false });
	                    $("#tableResyncPreview").treegrid(  // <<---- THIS needs to replace
	                        { url: 'Json-ResyncPreview.aspx?Guid=<%= this.UploadFile.GuidString %>' });
                        $('#DivStepPreview').fadeIn();
                        $('#DivStepProcessing').slideUp();
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
                            $.ajax({
                                type: "POST",
                                url: "UploadBankFiles.aspx/GetProcessingStatistics",
                                data: "{'guid': '<%= this.UploadFile.GuidString %>'}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg2) {
                                    $('#SpanFirstTx').text(msg2.d.FirstTransaction);
                                    $('#SpanLastTx').text(msg2.d.LastTransaction);
                                    $('#SpanTxCount').text(msg2.d.TransactionCount);
                                }
                            });
                        }

                        setTimeout('updateProgressProcessing();', 1000);
                    }
	            }
	        });
        }


        var progressReceived = false;

        var currentYear = <%=DateTime.Today.Year %>;


    </script>

    <div id="DivUploadResults">
    </div>
    <div id="DivUploadBankFile">
        <h2>Upload Bank File</h2>
        <div id="DivPrepData">
        
            <div class="entryFields">
                <asp:DropDownList runat="server" ID="DropAccounts"/>
                Dropdown<br/>
                Instructions<br/>
                <Swarmops5:FileUpload runat="server" ID="UploadFile" Filter="NoFilter" DisplayCount="8" HideTrigger="true" ClientUploadStartedCallback="uploadStartedCallback" ClientUploadCompleteCallback="uploadCompletedCallback" /></div>
        
            <div class="entryLabels">
                Bank account<br/>
                File type<br/>
                Instructions<br/>
                Upload bank file
            </div>
        </div>
    
        <br clear="all"/>
    </div>
    
    <div id="DivStepProcessing" style="display:none">
        <h2>Step 2/4: Processing uploaded file...</h2>
        <div id="DivProgressProcessing" style="width:100%"></div>
        <ul style="margin-left:20px"><li>The first transaction in the file was on <span id="SpanFirstTx">[...]</span>.</li>
        <li>The last transaction in file was on <span id="SpanLastTx">[...]</span>.</li>
        <li>There are <span id="SpanTxCount">[...]</span> transactions in the file.</li>
        </ul>
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

