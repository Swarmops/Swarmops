<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.FileExpenseClaim" CodeFile="FileExpenseClaim.aspx.cs" Codebehind="FileExpenseClaim.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/FileUpload.ascx" tagname="FileUpload" tagprefix="Swarmops5" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="Currency" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ScriptFancyBox" Package="FancyBox" runat="server"/>
    
    <style type="text/css">
         .LocalEditExpenseClaim {
             cursor: pointer;
         }
         #divExpensifySomethingMissing {
             font-size: 140%;
             margin-top: 10px;    
         }
         #divExpensifyReadySubmit {
             margin-top: 10px;
         }
         #spanLabelExpensifySubmit {
             line-height: 64px;
         }
    </style>

    <script type="text/javascript">

        $(document).ready(function () {

            $('#divTabs').tabs();

            $('#buttonModalProceed').click(function() {
                // first, validate fields

                var budgetId = <%=this.ComboExpensifyBudgets.ClientID%>_val();

                if (budgetId == 0) {
                    alertify.error("You need to select a budget.");
                    return;
                }

                var description = $('#textModalExpensifyDescription').val();

                if (description.length < 1) {
                    alertify.error("You need to describe the expense claim.");
                    return;
                }

                // TODO: check for invalid amounts

                var amount = <%=this.CurrencyModalExpensifyAmount.ClientID%>_val();
                var amountVat = <%=this.CurrencyModalExpensifyAmountVat.ClientID%>_val();
                <%=this.DialogEditExpenseClaim.ClientID%>_close();

                SwarmopsJS.ajaxCall(
                    '/Pages/v5/Financial/FileExpenseClaim.aspx/ExpensifyRecordProceed',
                    {
                        masterGuid: '<%=this.UploadExpensify.GuidString%>',
                        recordGuid: currentlyEditedRecordGuid,
                        description: description,
                        amountString: amount,
                        amountVatString: amountVat,
                        budgetId: budgetId
                    },
                    function(result) {
                        if (result.Success) {

                            displayExpensifyDataGrid(result.DataUpdate, result.FooterUpdate);

                            if (result.Guid.length > 1) // there's a new record
                            {
                                currentlyEditedRecordGuid = result.Guid;
                                displayExpensifyRecord(result);
                            }

                            displaySubmitPrompt(result.SubmitPrompt);

                        } else {
                            <%=this.DialogEditExpenseClaim.ClientID%>_open(); // re-open same dialog
                            alertify.error("There was an error submitting the data. Please fix.");
                        }
                    });
            });

            $('#buttonModalDelete').click(function () {
                <%=this.DialogEditExpenseClaim.ClientID%>_close();

                SwarmopsJS.ajaxCall(
                    '/Pages/v5/Financial/FileExpenseClaim.aspx/ExpensifyRecordDelete',
                    { masterGuid: '<%=this.UploadExpensify.GuidString%>', recordGuid: currentlyEditedRecordGuid },
                    function(result) {
                        if (result.Success) {
                            // A record has been deleted. It is possible that a new one is displayed.

                            displayExpensifyDataGrid(result.DataUpdate, result.FooterUpdate);

                            if (result.Guid.length > 1) // there's a new record
                            {
                                currentlyEditedRecordGuid = result.Guid;
                                displayExpensifyRecord(result);
                            }
                            else if (result.DataUpdate.length == 0) {
                                // There are no more records

                                // No data, so prepare for another upload

                                $('#spanLabelExpensifySomethingMissing').text(SwarmopsJS.unescape('<%=this.Localized_Expensify_NoRecords%>'));
                                $('#divExpensifySomethingMissing').show();
                                $('#divExpensifyReadySubmit').hide();
                                $('#divExpensifyUploadAnotherHeader').show();
                                $('#divExpensifyUploadFile').show();
                                $('#divExpensifyInstructions').hide();
                                $('#divUploadExpensify').slideDown();
                                setTimeout(function() {
                                    $('#divExpensifyResults').slideUp();
                                }, 5000);

                            } else {
                                displaySubmitPrompt(result.SubmitPrompt);
                            }
                        }
                    });
            });

        });  // end of doc.ready



        function validateFields() {
            var isValid = true;
            
            isValid = validateTextField('#<%=this.TextAccount.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankAccount %>')) && isValid;
            isValid = validateTextField('#<%=this.TextClearing.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankClearing %>')) && isValid;
            isValid = validateTextField('#<%=this.TextBank.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_BankName %>')) && isValid;

            if ($('#<%=this.ComboBudgets.ClientID %>_DropBudgets').combotree('tree').tree('getSelected') == null) {
                isValid = false;
                $('#<%=this.ComboBudgets.ClientID %>_SpanBudgets').addClass("entryError");
                alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Budget %>'));
            }

            isValid = validateTextField('#<%=this.TextPurpose.ClientID %>', SwarmopsJS.unescape('<%= this.Localized_ValidationError_Purpose %>')) && isValid;

            var jsonData = {};
            jsonData.amount = <%=this.CurrencyAmount.ClientID %>_val();

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.aspx/IsAmountValid",
                data: $.toJSON(jsonData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,  // blocks until function returns - race conditions otherwise
                success: function (msg) {
                    if (msg.d != true) {
                        isValid = false;
                        $('#<%=this.CurrencyAmount.ClientID %>_TextInput').addClass("entryError");
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Amount %>'));
                        <%=this.CurrencyAmount.ClientID %>_focus();
                    }
                }
            });

            $.ajax({
                type: "POST",
                url: "/Automation/FieldValidation.aspx/AreDocumentsUploaded",
                data: "{'guidString': '<%=this.FileUpload.GuidString %>'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,  // blocks until function returns - race conditions otherwise
                success: function (msg) {
                    if (msg.d != true) {
                        isValid = false;
                        $('#TextAmount').addClass("entryError");
                        alertify.error(SwarmopsJS.unescape('<%= this.Localized_ValidationError_Documents %>'));
                    }
                }
            });

            return isValid;
        }
        
        function validateTextField (fieldId, message) {
            if ($(fieldId).val().length == 0) {
                alertify.error(message);
                $(fieldId).addClass("entryError");
                $(fieldId).focus();
                return false;
            }

            return true;
        }

        function editExpensifyClaim(expenseClaimGuid) {
            SwarmopsJS.ajaxCall("/Pages/v5/Financial/FileExpenseClaim.aspx/GetExpensifyRecord",
                {
                    masterGuid: '<%=this.UploadExpensify.GuidString%>',
                    recordGuid: expenseClaimGuid
                },
                function(result) {
                    if (result.Success) {
                        currentlyEditedRecordGuid = result.Guid;
                        displayExpensifyRecord(result);
                    }
                });
        }

        function displayExpensifyRecord(record) {
            <%=this.ComboExpensifyBudgets.ClientID%>_val(record.BudgetId);
            <%=this.CurrencyModalExpensifyAmount.ClientID%>_initialize(record.Amount);
            <%=this.CurrencyModalExpensifyAmountVat.ClientID%>_initialize(record.AmountVat);
            $('#textModalExpensifyDescription').val(record.Description);
            <%=this.DialogEditExpenseClaim.ClientID%>_open();
            $('#imgModalDocument').attr('src', '/Pages/v5/Support/StreamUpload.aspx?DocId=' + record.DocumentId);
            $('#imgModalDocument').attr('data-zoom-image', '/Pages/v5/Support/StreamUpload.aspx?DocId=' + record.DocumentId + '&hq=1');
            $('.zoomContainer').remove();
            $('#imgModalDocument').removeData('elevateZoom');
            $('#imgModalDocument').removeData('zoomImage');
            setTimeout(function () {
                $('#imgModalDocument').elevateZoom({
                    zoomType: "lens",
                    cursor: "crosshair",
                    zoomWindowFadeIn: 200,
                    zoomWindowFadeOut: 200,
                    lensShape: "round",
                    lensSize: 150
                });
            }, 50); // delay this slightly for race condition reasons
        }


        function displayExpensifyDataGrid(rows, footer) {

            var gridData = {
                rows: rows,
                footer: footer
            };

            $('#expensifyDataGrid').datagrid('loadData', gridData);

            $('#expensifyDataGrid').datagrid('mergeCells', {
                index: 0,
                field: 'BudgetText',
                colspan: 3,
                type: 'footer'
            });

            // Bind to enable document viewing

            SwarmopsJS.fancyBoxInit('.FancyBox_Gallery');

            $(".LocalIconViewDoc").click(function () {
                $("a.FancyBox_Gallery[data-fancybox='" + $(this).attr("firstDocId") + "']").first().click();
            });

            // Hook in edit-expense popup

            $(".LocalEditExpenseClaim").click(function () {
                editExpensifyClaim($(this).attr("data-guid"));
            });
        }

        function displaySubmitPrompt(prompt) {
            if (prompt.length > 1) {
                $('#buttonExpensifySubmit').val(SwarmopsJS.unescape('<%=this.Localized_ConfirmDialog_Submit%>'));
                $('#spanLabelExpensifySubmit').text(prompt);
                $('#divExpensifySomethingMissing').slideUp();
                $('#divExpensifyReadySubmit').slideDown();
            }
        }


        var expensifyProcessingHalfway = false;
        var currentlyEditedRecordGuid = '';

        function onExpensifyUpload() {
            $('#divExpensifyUploadFile').slideUp();
            $('#divExpensifyResultsGood').hide();
            <%=this.ProgressExpensify.ClientID%>_reset();

            SwarmopsJS.ajaxCall
                ("/Pages/v5/Financial/FileExpenseClaim.aspx/InitializeExpensifyProcessing",
                {
                    guidFiles: '<%= this.UploadExpensify.GuidString%>',
                    guidProgress: '<%= this.ProgressExpensify.Guid%>'
                },
                function(result) {
                    if (result.Success) {
                        <%=this.ProgressExpensify.ClientID%>_fadeIn();
                        <%=this.ProgressExpensify.ClientID%>_begin();  // starts listening / polling for progress
                    }
                });
            
        }

        function onExpensifyProgressHalfway() {
            $('#divUploadExpensify').slideUp().fadeOut(); // hide the upload panel
        }

        function onExpensifyProgressComplete() {
            // Get results

            $('#divExpensifyUploadHeader').hide(); // this should be hidden at this time regardless of result

            SwarmopsJS.ajaxCall('/Pages/v5/Financial/FileExpenseClaim.aspx/GetExpensifyUploadResult',
                { guid: '<%=this.UploadExpensify.GuidString%>' },
                function(result) {
                    if (result.Success) {

                        // Make a neat transition to success view

                        $('#divExpensifyResultsBad').hide();
                        $('#divExpensifyResultsGood').show();
                        <%=this.ProgressExpensify.ClientID%>_hide();
                        <%=this.ProgressExpensify.ClientID%>_reset();
                        <%=this.ProgressExpensifyFake.ClientID%>_show();
                        $('#divUploadExpensify').hide();
                        $('#divExpensifyUploadAnotherHeader').show();
                        $('#divExpensifyResults').slideDown("slow", function() {
                            <%=this.ProgressExpensifyFake.ClientID%>_fadeOut();
                        });

                        // If there's no data at all, sort of abort and ask for a new file

                        if (result.Data.length == 0)
                        {
                            // No data, so prepare for another upload

                            $('#spanLabelExpensifySomethingMissing').text(SwarmopsJS.unescape('<%=this.Localized_Expensify_NoRecords%>'));
                            $('#divExpensifySomethingMissing').show();
                            $('#divExpensifyReadySubmit').hide();
                            $('#divExpensifyUploadAnotherHeader').show();
                            $('#divExpensifyUploadFile').show();
                            $('#divExpensifyInstructions').hide();
                            $('#divUploadExpensify').slideDown();
                        }
                        else
                        {

                            // Else, display processed data

                            $('#divExpensifySomethingMissing').show();
                            $('#divExpensifyReadySubmit').hide();

                            $('#spanLabelExpensifySomethingMissing').text(SwarmopsJS.unescape('<%=this.Localized_Expensify_NeedBudgetsForAll%>'));

                            $('#expensifyDataGrid').datagrid({
                                onBeginEdit: function(index, row) {
                                    var ed = $(this).datagrid('getEditor', { index: index, field: 'productid' });
                                    $(ed.target).combotree({
                                        url: '...',
                                        value: row.productid
                                    });
                                },
                                onEndEdit: function(index, row) {
                                    var ed = $(this).datagrid('getEditor', { index: index, field: 'productid' });
                                    row.productname = $(ed.target).combotree('getText');
                                }
                            });

                            // Fill in documents (hidden)

                            $('#divDocumentsHidden').html(result.Documents);

                            displayExpensifyDataGrid(result.Data, result.Footer);
                        }

                    } else {

                        // Make a brutal transition to failure view

                        <%=this.ProgressExpensify.ClientID%>_fadeOut();
                        <%=this.ProgressExpensify.ClientID%>_hide(); // both fadeOut + hide necessary
                        <%=this.ProgressExpensify.ClientID%>_reset();
                        <%=this.ProgressExpensifyFake.ClientID%>_hide();

                        $('#divExpensifyResultsBad').show();
                        $('#divExpensifyResultsGood').hide();
                        $('#divExpensifyResults').show();
                        $('#divExpensifyInstructions').hide();
                        $('#divExpensifyUploadAnotherHeader').show();
                        $('#divExpensifyUploadFile').show();
                        $('#divExpensifyResultsBadText').html(result.DisplayMessage);

                        $('#divUploadExpensify').fadeIn().slideDown(); // aborts the slideUp probably in progress
                    }

                    // Regardless of whether result is good or bad, reset the upload control

                    <%=this.UploadExpensify.ClientID%>_clear();
                });


        }


    </script>
    
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="easyui-tabs" id="divTabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-invoice-256px.png' width='56' height='56' style='padding-top:4px'>">
            <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
            <asp:HiddenField ID="HiddenTagSetIdentifiers" runat="server"/>
            <div class="entryFields">
                <Swarmops5:Currency runat="server" ID="CurrencyAmount" />
                <span class="ifVatEnabled"><Swarmops5:Currency runat="server" ID="CurrencyVat" /></span>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextPurpose" /></div>
                <Swarmops5:ComboBudgets ID="ComboBudgets" runat="server" />
                <asp:Repeater ID="RepeaterTagDrop" runat="server"><ItemTemplate><span id="SpanDropTags<%# Eval("TagSetId") %>"><select class="easyui-combotree" url="/Automation/Json-TransactionTagsTree.aspx?TagSetId=<%# Eval("TagSetId") %>" name="DropTags<%# Eval("TagSetId") %>" id="DropTags<%# Eval("TagSetId") %>" animate="true" style="width:300px"></select></span>&nbsp;<br/></ItemTemplate></asp:Repeater>

                <div class="stacked-input-control"></div><!-- placeholder for label-side H2 -->
        
                <!-- file upload begins here -->
        
                <Swarmops5:FileUpload ID="FileUpload" runat="server" Filter="ImagesOnly" />

                <!-- file upload ends -->

                <div class="stacked-input-control"></div><!-- placeholder for label-side H2 -->
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextBank" />&#8203;<br/></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextClearing" />&#8203;<br/></div>
                <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextAccount" />&nbsp;<br/></div>
                <asp:Button ID="ButtonRequest" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="return validateFields();" OnClick="ButtonRequest_Click" Text="Request"/>
            </div>
            <div class="entryLabels">
                <asp:Label runat="server" ID="LabelAmount" /><br/>
                <span class="ifVatEnabled"><asp:Label runat="server" ID="LabelVat"/><br/></span>
                <asp:Label runat="server" ID="LabelPurpose" /><br/>
                <asp:Label runat="server" ID="LabelBudget" /><br/>
                <asp:Repeater ID="RepeaterTagLabels" runat="server"><ItemTemplate><%# Eval("TagSetLocalizedName") %><br/></ItemTemplate></asp:Repeater>
                <h2><asp:Label runat="server" ID="LabelHeaderImageFiles" /></h2>
                <asp:Label runat="server" ID="LabelImageFiles" /><br/>
                <h2><asp:Label runat="server" ID="LabelHeaderBankDetails" /></h2>
                <asp:Label runat="server" ID="LabelBankName" /><br/>
                <asp:Label runat="server" ID="LabelBankClearing" /><br/>
                <asp:Label runat="server" ID="LabelBankAccount" />
            </div>
            <div style="clear:both"></div>
        </div>
        <div title="<img src='/Images/Icons/expensify-icon-official.png' width='40' height='40' style='padding-top:12px'>">
            
            <Swarmops5:ProgressBarFake ID="ProgressExpensifyFake" runat="server"/>

            <div id="divExpensifyResults" style="display:none; margin-bottom:10px">
               
                <h2><asp:Label ID="LabelExpensifyProcessingComplete" runat="server" /></h2>

                <div id="divExpensifyResultsGood" style="display:none">
                    <table id="expensifyDataGrid" class="easyui-datagrid" style="width:680px"
                        data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:true,loading:false,selectOnCheck:true,checkOnSelect:true"
                        idField="expenseId">
                        <thead>  
                            <tr>
                                <th data-options="field:'BudgetText',width:200"><asp:Label ID="LabelExpensifyHeaderBudget" runat="server" Text="XYZ Budget" /></th>  
                                <th data-options="field:'CreatedDateTime',width:50"><asp:Label ID="LabelExpensifyHeaderDate" runat="server" Text="XYZ Created" /></th>
                                <th data-options="field:'Description',width:200"><asp:Label ID="LabelExpensifyHeaderDescription" runat="server" Text="XYZ Description" /></th>
                                <th data-options="field:'AmountVat',width:80,align:'right',hidden:<%=(!CurrentOrganization.VatEnabled).ToString().ToLowerInvariant() %>"><asp:Label ID="LabelExpensifyHeaderVat" runat="server" Text="XYZ Vat" /></th>
                                <th data-options="field:'Amount',width:80,align:'right'"><asp:Label ID="LabelExpensifyHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                                <th data-options="field:'Actions',width:60,align:'center'"><asp:Label ID="LabelExpensifyHeaderDocs" runat="server" Text="XYZAct" /></th>
                            </tr>  
                        </thead>
                    </table>  
                    
                    <div id="divExpensifySomethingMissing">
                        <span id="spanLabelExpensifySomethingMissing">Need budgets for all</span>
                    </div>
                    
                    <div id="divExpensifyReadySubmit" style="display:none">
                        <div class="entryFields">
                            <input type="button" class="buttonAccentColor NoInputFocus" id="buttonExpensifySubmit" value="Submit"/>
                        </div>
                        <div class="entryLabels">
                            <span id="spanLabelExpensifySubmit">Ready to submit</span>
                        </div>
                    </div>

                </div>

                <div id="divExpensifyResultsBad" style="display:none">
                    <div style="float:left;margin-right:10px"><img src="/Images/Icons/iconshock-cross-96px.png" /></div><div id="divExpensifyResultsBadText"></div>
                </div>

                <br clear="all"/>
            </div>

            
            <div id="divUploadExpensify">
                <div id="divExpensifyInstructions">
                    <h2><asp:Label runat="server" ID="LabelExpensifyUploadHeader" /></h2>

                    <p><asp:Label runat="server" ID="LabelExpensifyInstructions1"/></p>
                    <p><asp:Label runat="server" ID="LabelExpensifyInstructions2"/></p>
                </div>

                <div id="divExpensifyUploadAnotherHeader" style="display:none"><h2><asp:Label runat="server" ID="LabelExpensifyUploadAnotherHeader" Text="Upload Another XYZ" /></h2></div>
        
                <div id="divExpensifyUploadFile">
                    <div class="entryFields">
                        <Swarmops5:FileUpload runat="server" ID="UploadExpensify" Filter="NoFilter" DisplayCount="8" ClientUploadCompleteCallback="onExpensifyUpload" />
                    </div>
                    <div class="entryLabels">
                        <div class="stacked-input-control"><asp:Label runat="server" ID="LabelExpensifyCsv" /></div>
                    </div>
                </div>
    
                <br clear="all"/>
            </div>

            <Swarmops5:ProgressBar ID="ProgressExpensify" runat="server" OnClientProgressHalfwayCallback="onExpensifyProgressHalfway" OnClientProgressCompleteCallback="onExpensifyProgressComplete"/>
            
            <div id="divDocumentsHidden"></div>

        </div>
    </div>

    <Swarmops5:ModalDialog ID="DialogEditExpenseClaim" runat="server">
        <DialogCode>
            <h2>Editing Expense Claim</h2>
            <div class="elementFloatFar" style="width: 200px;padding:10px"><img id="imgModalDocument" style="border: 2px solid #888; width:100%"/></div>
            <div class="entryFields">
                <div class="stacked-input-control"><input type="text" id="textModalExpensifyDescription" /></div>
                <Swarmops5:ComboBudgets ID="ComboExpensifyBudgets" ListType="Expensable" runat="server"/>
                <Swarmops5:Currency ID="CurrencyModalExpensifyAmount" runat="server"/>
                <div class="ifVatEnabled"><Swarmops5:Currency ID="CurrencyModalExpensifyAmountVat" runat="server"/></div>
                <input type="button" id="buttonModalProceed" class="buttonAccentColor HalfWidth NoInputFocus" value="Proceed &gt;&gt;"/><input type="button" id="buttonModalDelete" class="buttonAccentColor Red HalfWidth NoInputFocus" value="Delete"/>
            </div>
            <div class="entryLabels">Description<br/>Budget charged<br />Expense amount<br/><div class="ifVatEnabled">VAT amount of the total</div></div>
        </DialogCode>
    </Swarmops5:ModalDialog>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

