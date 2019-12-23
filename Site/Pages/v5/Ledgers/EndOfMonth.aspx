<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="EndOfMonth.aspx.cs" Inherits="Swarmops.Frontend.Pages.Ledgers.EndOfMonth" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" runat="server">
    
    <script type="text/javascript">


        $(document).ready(function () {

            var rowCount = 0;

            <%=this.JavascriptDocReady%>

            // set action icons to their respective initial icons

            $('img.eomitem-wrench').attr('src', '/Images/Icons/iconshock-balloon-wrench-128x96px.png');
            $('img.eomitem-document').attr('src', '/Images/Icons/iconshock-balloon-invoice-128x96px.png');
            $('img.eomitem-upload').attr('src', '/Images/Icons/iconshock-balloon-up-128x96px.png');
            $('img.eomitem-approve').attr('src', '/Images/Icons/iconshock-balloon-yes-128x96px.png');

            // pointer cursor over action icons

            $('img.action').attr('cursor', 'hand');

            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="item-group-header">Upload&nbsp;external&nbsp;data&nbsp;and&nbsp;match&nbsp;accounts</span>',
                itemId: 'GroupExternal'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 2,
                type: 'body',
                field: 'itemGroupName'
            });


            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Upload/Fetch bank transaction data (FORMAT) up until [lastdatelastmonth]',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Upload bank statement (PDF) for [lastmonth]',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Resolve unmatched ledger transactions',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="item-group-header">Taxes&nbsp;and&nbsp;Payroll</span>',
                itemId: 'GroupTaxesPayroll'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 2,
                type: 'body',
                field: 'itemGroupName'
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'VAT Report for [lastmonth]',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            /*

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Payroll processing for [thismonth]',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                actions: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Submit to tax authorities',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                actions: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            */

            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="item-group-header">Annual&nbsp;Reports</span>',
                itemId: 'GroupAnnual'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 2,
                type: 'body',
                field: 'itemGroupName'
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Close ledgers for [year]',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });


            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="item-group-header">Send&nbsp;to&nbsp;Accountants,&nbsp;Shareholders,&nbsp;etc.</span>',
                itemId: 'SendReports'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 2,
                type: 'body',
                field: 'itemGroupName'
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Send all reports as required',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('.action').click(function() {
                var itemId = $(this).attr('data-item');
                var callbackFunction = $(this).attr('data-callback');

                if ($(this).hasClass('is-upload')) {

                    // Trigger upload, then trigger ajax call on complete

                    if (activeUpload != null) {
                        // upload is already in progress; don't allow two concurrent uploads since we're using one control
                        // todo: error message or some UX to explain this?

                        return;
                    }

                    triggeredUpload = $(this).attr('data-item');
                    <%=this.UploadControl.ClientID%>_triggerUpload();

                } else {

                    // Ajax call right away

                    $('img.action-icon[data-item="' + itemId + '"]').hide();
                    $('img.status-icon-pleasewait[data-item="' + itemId + '"]').show();

                    SwarmopsJS.ajaxCall(
                        "/Pages/v5/Ledgers/EndOfMonth.aspx/" + callbackFunction,
                        { itemId: itemId },
                        // Success function
                        $.proxy(function (result) {
                            var itemId = $(this).attr('data-item');
                            if (result.Success) {
                                markItemCompleted(itemId);
                                if (result.DisplayMessage !== undefined && result.DisplayMessage != null && result.DisplayMessage.length > 2) {
                                    alertify.alert(result.DisplayMessage);
                                }
                            } else {
                                // Restore action icons.
                                // Todo: display DisplayMessage in dialog or error notice.
                                $('img.status-icon-pleasewait[data-item="' + itemId + '"]').hide();
                                $('img.action-icon[data-item="' + itemId + '"]').show();
                            }
                        }, this),
                        // Failure (exception) function
                        $.proxy(function () {
                            var itemId = $(this).attr('data-item');
                            $('img.status-icon-pleasewait[data-item="' + itemId + '"]').hide();
                            $('img.action-icon[data-item="' + itemId + '"]').show();
                        }, this)
                    );
                }

            });

            $('.action-skip > a').click(function () {
                var itemId = $(this).parent().parent().attr("data-item");

                var prompt = localized_skipPromptGeneric;
                if (itemId.startsWith('BankStatement')) {
                    prompt = localized_skipPromptBankStatement;
                }

                alertify.set({
	                labels: {
	                    ok: localized_skipYesResponse,
	                    cancel: localized_skipNoResponse
	                },
	                buttonFocus: 'cancel'
	            });

                alertify.confirm(prompt,
                    $.proxy(function(response) {
                        if (response) {
                            var itemId = $(this).parent().parent().attr("data-item");

                            var dependentItemId = $('span.action-list-item[data-dependson="' + itemId + '"]').attr("data-item");

                            $('img.action-icon[data-item="' + itemId + '"]').hide();
                            $('img.status-icon-completed[data-item="' + itemId + '"]').attr("src", "/Images/Icons/iconshock-red-cross-128x96px.png").fadeIn();

                            $('span.action-list-item[data-item="' + itemId + '"]').addClass('action-list-item-completed');
                            $('span.action-list-item[data-item="' + dependentItemId + '"]').removeClass('action-list-item-disabled');
                            $('img.action-icon[data-item="' + dependentItemId + '"]').removeClass('action-icon-disabled');
                        }
                    }, this));
            });

        });

        function markItemCompleted(itemId) {

            var groupId = $('span.action-list-item[data-item="' + itemId + '"]').attr('data-group');

            console.log("GroupId is " + groupId);

            $('span.action-list-item[data-item="' + itemId + '"]').addClass('action-list-item-completed');
            $('img.status-icon-pleasewait[data-item="' + itemId + '"]').hide();
            $('img.status-icon-completed[data-item="' + itemId + '"]').fadeIn();

            // If any items depended on this one, enable them now

            var dependentItemId = $('span.action-list-item[data-dependson="' + itemId + '"]').attr("data-item");
            $('span.action-list-item[data-item="' + dependentItemId + '"]').removeClass('action-list-item-disabled');
            $('img.action-icon[data-item="' + dependentItemId + '"]').removeClass('action-icon-disabled');

            // Check if group is complete

            var selector = ".action-list-item:not(.action-list-item-completed):not(.action-list-item-disabled)[data-group='" + groupId + "']";
            if ($(selector).length == 0) // no further actions in this group enabled
            {
                // mark the group as completed
                $(".group-status-icon[data-group='" + groupId + "']").fadeIn();
            }

        }

        function clientStartedUpload() {
            
            if (triggeredUpload == null) {
                // invalid state
                return;
            }
            activeUpload = triggeredUpload;
            triggeredUpload = null;

            // switch action icon upload to status icon waiting
            $('img.action-icon[data-item="' + activeUpload + '"]').hide();
            $('img.status-icon-pleasewait[data-item="' + activeUpload + '"]').show();

            // If there's a skip option, disable it at this time
            $('span.action-list-item[data-item="' + activeUpload + '"] span.action-skip').addClass('action-skip-disabled');
        }

        function clientFailedUpload() {
            console.log("Client aborted or failed upload");

            if (activeUpload == null) {
                // invalid state
                return;
            }

            $('img.status-icon-pleasewait[data-item="' + activeUpload + '"]').hide();
            $('img.action-icon[data-item="' + activeUpload + '"]').show();

            // If upload is bank statement, show an error dialog

            var itemType = activeUpload.substring(0, activeUpload.indexOf('-'));

            if (itemType == "BankTransactionData") {
                // Show a pretty dialog telling the user that things went to unpretty shit

                alert("Checkpoint hit");

            }

            activeUpload = null;
        }

        function clientFinishedUpload() {

            if (activeUpload == null) {
                // invalid state
                return;
            }

            var itemType = activeUpload.substring(0, activeUpload.indexOf('-'));

            // Note that the function to call is selected dynamically!

            SwarmopsJS.ajaxCall(
                "/Pages/v5/Ledgers/EndOfMonth.aspx/Upload" + itemType,
                { guid: uploadGuid, itemId: activeUpload },
                function(result) {
                    if (result.Success) {

                        markItemCompleted(activeUpload);

                        // If there are items that depend on this one,
                        // disable their "skip" option now (because there's
                        // a first document in the series)

                        var itemIterator = $('.action-list-item[data-dependson="' + activeUpload + '"]');

                        while (itemIterator.length > 0) {
                            // We're only supporting a 1:1 chain at this time, no 1:n dependencies

                            var itemId = $(itemIterator).attr('data-item');
                            $('span.action-list-item[data-item="' + itemId + '"] span.action-skip').addClass('action-skip-disabled');
                            itemIterator = $('.action-list-item[data-dependson="' + itemId + '"]');
                        }

                        activeUpload = null;

                    } else {
                        alert(result.DisplayMessage); // temporary debug function
                        // Todo: add more error handling later, maybe
                        clientFailedUpload();
                    }
                },
                function(error) {
                    clientFailedUpload();
                });
          
        }

        var uploadGuid = '<%=this.UploadControl.GuidString%>';

        var triggeredUpload = null;
        var activeUpload = null;

        var localized_skipNoResponse = SwarmopsJS.unescape('<%=this.Localized_SkipNo%>');
        var localized_skipYesResponse = SwarmopsJS.unescape('<%=this.Localized_SkipYes%>');
        var localized_skipPromptBankStatement = SwarmopsJS.unescape('<%=this.Localized_SkipPrompt_BankStatement%>');
        var localized_skipPromptGeneric = SwarmopsJS.unescape('<%=this.Localized_SkipPrompt_Generic%>');

        var uploadGuid = '<%=this.UploadControl.GuidString%>';

        // Function: Match all mismatched transactions

        // Function: Upload bank statement PDF for accountId x

        // Function: Close YEAR if a new year

        // Function: Send EOM papers to accountants etc


    </script>

    <style type="text/css">
    </style>


</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2><asp:Label runat="server" ID="LabelHeader"></asp:Label></h2>
    <div style="display: none"><Swarmops5:FileUpload ID="UploadControl" ClientUploadFailedCallback="clientFailedUpload" ClientUploadStartedCallback="clientStartedUpload" ClientUploadCompleteCallback="clientFinishedUpload" runat="server"/></div>

    <table id="TableEomItems" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:false,checkOnSelect:false"
        idField="itemId">
        <thead>
            <tr>
                <th data-options="field:'itemGroupName',width:42">&nbsp;</th>
                <th data-options="field:'itemName',width:562">Todo</th>
                <th data-options="field:'action',width:55,align:'center'">Action</th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

