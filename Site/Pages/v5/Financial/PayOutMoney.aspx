<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Financial.PayOutMoney" Codebehind="PayOutMoney.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <script type="text/javascript"> 
    
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-48x36px.gif',
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px.png',
            '/Images/Icons/iconshock-balloon-no-128x96px-disabled.png',
            '/Images/Icons/iconshock-barcode-128x96px.png'
        ]);

        $(document).ready(function () {

            $('#CheckOptionsShowOcr').change(function () {
                var checked = $('#CheckOptionsShowOcr').prop('checked');

                if (checked) {
                    $('#divOcrView').show();
                    $('#divNormalView').hide();
                } else {
                    $('#divOcrView').hide();
                    $('#divNormalView').show();
                }
            });

            $('#TablePayableCosts, #TablePayableCostsOcr').datagrid(
                {
                    rowStyler: function (index, rowData) {
                        if (rowData.databaseId != null) {
                            return { class: "action-list-item-approved row" + rowData.itemId.replace(/\|/g, "") };
                        }

                        if (rowData.itemId != null) {
                            return { class: "row" + rowData.itemId.replace(/\|/g, "") };
                        }

                        return "";
                    },

                    onLoadSuccess: function () {

                        gridsLoaded++;

                        if (gridsLoaded == 2) {  // onLoadSuccess is triggered _twice_ per grid for some arcane reason

                            $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                            $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-green-tick-128x96px.png").css("opacity", 0.5);
                            $(".LocalIconDenied").attr("src", "/Images/Icons/iconshock-red-cross-circled-128x96px.png");
                            $(".LocalIconWait").attr("src", "/Images/Abstract/ajaxloader-48x36px.gif").hide();
                            $(".LocalIconUndo").attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                            $(".LocalIconOcr").attr("src", "/Images/Icons/iconshock-barcode-128x96px.png").attr("title", SwarmopsJS.unescape('<%=this.Localized_IconTooltip_Barcode%>'));

                            $(".LocalIconApproved.LocalPrototype, .LocalIconUndo.LocalPrototype, .LocalIconDenied.LocalPrototype, .LocalIconApproval.LocalPaid, .LocalIconDenial.LocalPaid, .LocalIconDenied.LocalPaid").hide();
                            $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-balloon-no-128x96px-disabled.png");
                            $(".LocalIconApproval, .LocalIconUndo, .LocalIconDenial").css("cursor", "pointer");

                            $(".LocalIconApproval").click(function () {

                                // TODO: Check UX-level concurrency lock

                                $('#idModalInputRecipient').val(loadingBreadcrumb);
                                $('#idModalInputCurrencyAmount').val(loadingBreadcrumb);
                                $('#idModalReference').val(loadingBreadcrumb);
                                $('#idModalTransferMethod').val(loadingBreadcrumb);
                                $('#idModalDueDate').text(loadingBreadcrumb);

                                // Show or hide the correct count of extra data fields

                                var fieldsRequiredCount = parseInt($(this).attr("data-fieldcount"));

                                for (var index = 0; index < modalExtraFieldCount; index++) {
                                    if (index < fieldsRequiredCount) {
                                        $('#idModalSpanExtraField' + index).show();
                                        $('#idModalSpanExtraLabel' + index).show();
                                    } else {
                                        $('#idModalSpanExtraField' + index).hide();
                                        $('#idModalSpanExtraLabel' + index).hide();
                                    }
                                }

                                // Show or hide the automation toggle

                                var automationAvailable = $(this).attr("data-ocr");
                                if (automationAvailable == "yes") {
                                    $(".modal-automation-toggle").show();
                                } else {
                                    $(".modal-automation-toggle").hide();
                                }

                                $(".input-field-needs-init").each(function() {
                                    $(this).val(loadingBreadcrumb);
                                });


                                <%=this.ModalConfirmPayment.ClientID%>_open();

                                modalPrototypeId = $(this).attr("protoid");
                                modalItemId = $(this).attr("baseid");

                                var prototypeId = $(this).attr("protoid");

                                SwarmopsJS.proxiedAjaxCall(
                                    "/Pages/v5/Financial/PayOutMoney.aspx/GetPaymentTransferInfo",
                                    { prototypeId: prototypeId },
                                    this,
                                    function(result) {
                                        if (result.Success) {
                                            modalPrototypeId = $(this).attr("protoid");
                                            modalItemId = $(this).attr("baseid");

                                            console.log(result);

                                            $('#idModalInputRecipient').val(result.Recipient);
                                            $('#idModalInputCurrencyAmount').val(result.CurrencyAmount);
                                            $('#idModalReference').val($(this).attr("data-reference"));  // load reference from JSON data
                                            $('#idModalTransferMethod').val(result.TransferMethod);

                                            $('#idModalDueDate').text(result.DueBy + ')');

                                            var customFieldCount = parseInt($(this).attr("data-fieldcount"));

                                            for (var index = 0; index < customFieldCount; index++) {
                                                $('#idModalSpanExtraLabel' + index).html(result.TransferMethodLabels[index] + "<br/>");
                                                $('#idModalExtraField' + index).val(result.TransferMethodData[index]);
                                            }

                                            // Add OCR fields if available

                                            if (result.OcrData != null && result.OcrData.length > 0) {
                                                $('#idModalAutomationField1').val(result.OcrData[0]);
                                                $('#idModalAutomationField2').val(result.OcrData[1]);
                                                $('#idModalAutomationField3').val(result.OcrData[2]);
                                            }

                                        } else {
                                            // TODO: Handle server-level concurrency lock
                                        }
                                    },
                                    function() {
                                        // This is the error handler function
                                        //
                                        // TODO: Display an error message and close the modal
                                        //
                                        alert("Error calling GetPaymentTransferInfo");
                                    }
                                );
                            });


                            $(".LocalIconUndo").click(function() {
                                $(this).hide();
                                var itemId = $(this).attr("baseid");
                                $("#IconApproved" + itemId).fadeTo(1000, 0.01);
                                $("#IconWait" + itemId).show();

                                SwarmopsJS.proxiedAjaxCall(
                                    "/Pages/v5/Financial/PayOutMoney.aspx/UndoPayout",
                                    { databaseId: $("#IconApproval" + itemId).attr("databaseid") },
                                    this,
                                    function(result) {
                                        if (result.Success) {
                                            var itemId = $(this).attr("baseid");
                                            $('.row' + itemId).removeClass("action-list-item-approved");
                                            $("#IconWait" + itemId).hide();
                                            $("#IconApproved" + itemId).hide();
                                            $("#IconApproval" + itemId).fadeTo(200, 1);
                                            $("#IconDenial" + itemId).fadeTo(200, 1);
                                            alertify.log(result.DisplayMessage);

                                        } else {
                                            // There's probably a concurrency error.
                                            // The socket handler will take care of updating the UI on
                                            // receiving the cause of the concurrency error.

                                            alertify.log(result.DisplayMessage);
                                        }
                                    }
                                );
                            });



                            /*

                            OLD CODE BELOW FROM PREVIOUS TYPE OF ICON LOGIC, KEPT A WHILE FOR REFERENCE --

                            $(".LocalIconApproval").click(function() {
                                if ($(this).attr("rel") != "loading") {

                                    var jsonData = {};
                                    jsonData.protoIdentity = $(this).attr("baseid");

                                    $(this).attr("rel", "loading");
                                    $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");
                                    $(".IconDenial" + $(this).attr("baseid").replace(/\|/g, '')).fadeTo(1000, 0.01);

                                    $.ajax({
                                        type: "POST",
                                        url: "/Pages/v5/Financial/PayOutMoney.aspx/ConfirmPayout",
                                        data: $.toJSON(jsonData),
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: $.proxy(function(msg) {
                                            var baseid = $(this).attr("baseid").replace(/\|/g, '');
                                            $("#IconApproval" + baseid).attr("databaseid", msg.d.AssignedId);
                                            $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                                            $(this).attr("rel", "active");
                                            $(".IconApproval" + baseid).css('display', 'none');
                                            $(".IconApproved" + baseid).fadeTo(250, 0.5);
                                            $(".IconDenial" + baseid).css('opacity', 1.0).css('display', 'none');
                                            $(".IconUndo" + baseid).fadeIn(100);
                                            $('.row' + baseid).animate({ color: "#AAA" }, 400);
                                            alertify.success(msg.d.DisplayMessage);
                                        }, this)
                                    });
                                }
                            });




                            $(".LocalIconUndo").click(function() {
                                if ($(this).attr("rel") != "loading") {
                                    var jsonData = {};
                                    jsonData.databaseId = $("#IconApproval" + $(this).attr("baseid").replace(/\|/g, '')).attr("databaseid");

                                    $(this).attr("rel", "loading");
                                    $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");
                                    $(".IconApproved" + $(this).attr("baseid").replace(/\|/g, '')).fadeTo(1000, 0.01);

                                    $.ajax({
                                        type: "POST",
                                        url: "/Pages/v5/Financial/PayOutMoney.aspx/UndoPayout",
                                        data: $.toJSON(jsonData),
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: $.proxy(function(msg) {
                                            if (msg.d.Success) {
                                                var baseid = $(this).attr("baseid").replace(/\|/g, '');
                                                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                                                $(this).attr("rel", "");
                                                $(".IconUndo" + baseid).css('display', 'none');
                                                $(".IconApproved" + baseid).css('opacity', 1.0).css('display', 'none');
                                                $(".IconApproval" + baseid).fadeIn(100);
                                                $(".IconDenial" + baseid).fadeTo(100, 1);
                                                $('.row' + baseid).animate({ color: "#000" }, 100);
                                                alertify.log(msg.d.DisplayMessage);
                                            } else {
                                                $(this).attr("src", "/Images/Icons/iconshock-green-tick-128x96px.png");
                                                alertify.error(msg.d.DisplayMessage);
                                                // TODO: Add alert box?
                                            }
                                        }, this)
                                    });

                                }
                            });*/
                        }

                    }
                }
            );

            // Localization

            $('#idModalButtonConfirm').prop("value", SwarmopsJS.unescape('<%=this.Localized_ConfirmDialog_ConfirmPaid%>'));



        });  // End of document.ready()

        function onConfirmModal()
        {

            var approvalIcon = $('#IconApproval' + modalItemId);
            confirmExpenseApproved(approvalIcon);
            <%=this.ModalConfirmPayment.ClientID%>_close();

            modalItemId = "";
            modalPrototypeId = "";
        }


        function confirmExpenseApproved(approvalIcon) {

            var itemId = $(approvalIcon).attr("baseid");
            var prototypeId = $(approvalIcon).attr("protoid");

            if (prototypeId === undefined || prototypeId === null || prototypeId.Length < 2) {
                alert("PrototypeId is empty. Aborting operation. This should not happen.");
                return;
            }

            $(approvalIcon).hide();
            $("#IconWait" + itemId).show();
            $("#IconDenial" + itemId).fadeTo(1000, 0.01);

            SwarmopsJS.proxiedAjaxCall(
                "/Pages/v5/Financial/PayOutMoney.aspx/ConfirmPayout",
                { protoIdentity: prototypeId },
                approvalIcon,
                function (result) {
                    if (result.Success) {
                        var itemId = $(this).attr("baseid");
                        $('.row' + itemId).addClass("action-list-item-approved");
                        $("#IconApproval" + itemId).attr("databaseid", result.AssignedId);
                        $("#IconWait" + itemId).hide();
                        $("#IconDenial" + itemId).hide();
                        $("#IconApproved" + itemId).fadeTo(200, 0.5); // half opacity is intentional
                        $("#IconUndo" + itemId).fadeTo(1000, 1); // the longer delay is intentional
                        alertify.success(result.DisplayMessage);
                    } else {
                        // There's probably a concurrency error.
                        // The socket handler will take care of updating the UI on
                        // receiving the cause of the concurrency error.

                        alertify.log(result.DisplayMessage);

                    }
                }
            );
        }

        var gridsLoaded = 0;
        var loadingBreadcrumb = "[...]";
        var modalPrototypeId = "";
        var modalItemId = "";
        var modalExtraFieldCount = 5;  // this is the count of showable/hideable fields

    </script>

    <style type="text/css">

        /* Include OCR-B font for OCR fields */

        @font-face {
            font-family: Ocrb;
            src: url('/Style/Fonts/OCR-B-regular-web.woff');
        }

        .ocr-font {  /* this is necessary for OCR to work as expected */
            font-family: Ocrb;
            font-size: 120%;
            letter-spacing: 0px;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divNormalView">
        <h2><asp:Label runat="server" ID="LabelPayOutMoneyHeader" Text="XYZ Costs Awaiting Payment" /></h2>
        <table id="TablePayableCosts" class="easyui-datagrid" style="width:680px;height:500px"
            data-options="rownumbers:false,singleSelect:false,fit:false,fitColumns:true,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-PayableCosts.aspx'"
            idField="itemId">
            <thead>  
                <tr>  
                    <th data-options="field:'due',width:70"><asp:Label ID="LabelGridHeaderDue" runat="server" Text="XYZ Due"/></th>  
                    <th data-options="field:'recipient',width:160,sortable:true"><asp:Label ID="LabelGridHeaderRecipient" runat="server" Text="XYZ Beneficiary" /></th>
                    <th data-options="field:'transferInfo',width:160"><asp:Label ID="LabelGridHeaderCurrencyMethod" runat="server" Text="XYZ Bank" /></th>  
                    <th data-options="field:'amount',width:85,align:'right'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                    <th data-options="field:'ocrAvailable',width:32"><img src="/Images/Icons/iconshock-barcode-128x96px.png" class="status-icon grid-header-status-icon"/></th>  
                    <th data-options="field:'action',width:68,align:'center'"><asp:Label ID="LabelGridHeaderPay" runat="server" Text="XYZPaid" /></th>
                </tr>  
            </thead>
        </table>
    </div>  
    
    <Swarmops5:ModalDialog ID="ModalConfirmPayment" runat="server" >
        <DialogCode>
            <h2><asp:Label ID="LabelModalHeader" runat="server" Text="Execute this payout manually now XYZ"/> (<asp:Label runat="server" ID="LabelModalHeaderDue"/> <span id="idModalDueDate"></span></h2>
            <div class="data-entry-fields modal wide">
                <input type="text" id="idModalInputRecipient" class="input-field-needs-init" readonly="readonly" value="Recipient"/>&#8203;<br/>
                <span class="modal-input-regular">
                    <input type="text" id="idModalInputCurrencyAmount" readonly="readonly" class="input-field-needs-init align-for-numbers" value="Amount"/>&#8203;<br/>
                    <input type="text" id="idModalReference" readonly="readonly" class="input-field-needs-init" value="Reference"/>&#8203;<br/>
                    <input type="text" id="idModalTransferMethod" readonly="readonly" class="input-field-needs-init" value="Transfer Method"/>&#8203;<br/>
                </span>
                <span class="modal-input-automated ocr-font">
                    <input type="text" id="idModalAutomationField1" class="input-field-needs-init ocr-font" readonly="readonly" value="Field1"/>&#8203;<br/>
                    <input type="text" id="idModalAutomationField2" class="input-field-needs-init ocr-font" readonly="readonly" value="Field2"/>&#8203;<br/>
                    <input type="text" id="idModalAutomationField3" class="input-field-needs-init ocr-font" readonly="readonly" value="Field3"/>&#8203;<br/>
                </span>
                <div class="modal-extra-data-fields">
                    <span id="idModalSpanExtraField0"><input type="text" id="idModalExtraField0" class="input-field-needs-init" readonly="readonly" value="Extra Field 0"/>&#8203;<br/></span>
                    <span id="idModalSpanExtraField1"><input type="text" id="idModalExtraField1" class="input-field-needs-init" readonly="readonly" value="Extra Field 1"/>&#8203;<br/></span>
                    <span id="idModalSpanExtraField2"><input type="text" id="idModalExtraField2" class="input-field-needs-init" readonly="readonly" value="Extra Field 2"/>&#8203;<br/></span>
                    <span id="idModalSpanExtraField3"><input type="text" id="idModalExtraField3" class="input-field-needs-init" readonly="readonly" value="Extra Field 3"/>&#8203;<br/></span>
                    <span id="idModalSpanExtraField4"><input type="text" id="idModalExtraField4" class="input-field-needs-init" readonly="readonly" value="Extra Field 4"/>&#8203;<br/></span>
                </div>
                <span class="modal-automation-toggle"><Swarmops5:AjaxToggleSlider ID="ToggleModalMachineReadable" runat="server" Label="Show in machine-readable format XYZ" Cookie="MachineReadable" AjaxCallbackUrl="" OnChange="modalToggleMachineReadable"/></span>
                <input type="button" id="idModalButtonConfirm" value="Confirm XYZ" class="button-accent-color suppress-input-focus action-icon-button icon-yes" onclick="onConfirmModal();"/>
            </div>
            <div class="data-entry-labels">
                <asp:Label ID="LabelModalRecipient" runat="server" Text="Recipient XYZ"/><br/>
                <span class="modal-input-regular">
                    <asp:Label ID="LabelModalCurrencyAmount" runat="server" Text="Currency and Amount XYZ"/><br/>
                    <asp:Label ID="LabelModalReference" runat="server" Text="Reference XYZ"/><br/>
                    <asp:Label ID="LabelModalTransferMethod" runat="server" Text="Transfer Method XYZ"/><br/>
                </span>
                <span class="modal-input-automated">
                    <asp:Label runat="server" ID="LabelModalAutomation1" /><br/>
                    <asp:Label runat="server" ID="LabelModalAutomation2" /><br/>
                    <asp:Label runat="server" ID="LabelModalAutomation3" /><br/>
                </span>
                <div class="modal-extra-data-fields">
                    <span id="idModalSpanExtraLabel0">Label 0<br/></span>
                    <span id="idModalSpanExtraLabel1">Label 1<br/></span>
                    <span id="idModalSpanExtraLabel2">Label 2<br/></span>
                    <span id="idModalSpanExtraLabel3">Label 3<br/></span>
                    <span id="idModalSpanExtraLabel4">Label 4<br/></span>
                </div>
                <span class="modal-automation-toggle"><span id="idModalSpanEnableOcrLabel"><asp:Label runat="server" ID="LabelModalOcr1" Text="Are you scanning this payment XYZ?"/></span></span>
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarOptions" Text="Options" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content" style="margin-left:5px">
            <div class="link-row-encaps" style="cursor:default; margin-left:2px">
               <span style="position:relative;top:2px;left:1px"><input type="checkbox" id="CheckOptionsShowOcr" /></span>&nbsp;<label for="CheckOptionsShowOcr"><asp:Label ID="LabelOptionsShowOcr" runat="server" Text="Display OCR ready data XYZ"/></label>
            </div>
        </div>
    </div>
    
</asp:Content>

