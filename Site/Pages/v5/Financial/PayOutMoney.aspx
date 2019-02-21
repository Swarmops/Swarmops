<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Financial.PayOutMoney" Codebehind="PayOutMoney.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <!-- Monospace font for OCR view -->

    <link href='https://fonts.googleapis.com/css?family=Droid+Sans+Mono' rel='stylesheet' type='text/css'>
        
    <!-- regular JS -->

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
            '/Images/Icons/iconshock-balloon-undo-128x96px.png'
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

                        if (gridsLoaded == 4) {  // onLoadSuccess is triggered _twice_ per grid for some arcane reason

                            $('#divOcrView').hide();

                            $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                            $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-green-tick-128x96px.png").css("opacity", 0.5);
                            $(".LocalIconDenied").attr("src", "/Images/Icons/iconshock-red-cross-circled-128x96px.png");
                            $(".LocalIconWait").attr("src", "/Images/Abstract/ajaxloader-48x36px.gif").hide();
                            $(".LocalIconUndo").attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                            $(".LocalIconApproved.LocalPrototype, .LocalIconUndo.LocalPrototype, .LocalIconDenied.LocalPrototype, .LocalIconApproval.LocalPaid, .LocalIconDenial.LocalPaid, .LocalIconDenied.LocalPaid").hide();
                            $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-balloon-no-128x96px-disabled.png");
                            $(".LocalIconApproval, .LocalIconUndo, .LocalIconDenial").css("cursor", "pointer");

                            $(".LocalIconApproval").click(function () {

                                <%=this.ModalConfirmPayment.ClientID%>_open();

                                /*
                                var itemId = $(this).attr("baseid");
                                var prototypeId = $(this).attr("protoid");
                                $(this).hide();
                                $(".IconWait" + itemId).show();
                                $(".IconDenial" + itemId).fadeTo(1000, 0.01);

                                SwarmopsJS.proxiedAjaxCall(
                                    "/Pages/v5/Financial/PayOutMoney.aspx/ConfirmPayout",
                                    { protoIdentity: prototypeId },
                                    this,
                                    function (result) {
                                        if (result.Success) {
                                            var itemId = $(this).attr("baseid");
                                            $('.row' + itemId).addClass("action-list-item-approved");
                                            $("#IconApproval" + itemId).attr("databaseid", result.AssignedId);
                                            $(".IconWait" + itemId).hide();
                                            $(".IconDenial" + itemId).hide();
                                            $(".IconApproved" + itemId).fadeTo(200, 0.5); // half opacity is intentional
                                            $(".IconUndo" + itemId).fadeTo(1000, 1); // the longer delay is intentional
                                            alertify.success(result.DisplayMessage);
                                        } else {
                                            // There's probably a concurrency error.
                                            // The socket handler will take care of updating the UI on
                                            // receiving the cause of the concurrency error.

                                            alertify.log(result.DisplayMessage);

                                        }
                                    }
                                );*/
                            });


                            $(".LocalIconUndo").click(function() {
                                $(this).hide();
                                var itemId = $(this).attr("baseid");
                                $(".IconApproved" + itemId).fadeTo(1000, 0.01);
                                $(".IconWait" + itemId).show();

                                SwarmopsJS.proxiedAjaxCall(
                                    "/Pages/v5/Financial/PayOutMoney.aspx/UndoPayout",
                                    { databaseId: $("#IconApproval" + itemId).attr("databaseid") },
                                    this,
                                    function(result) {
                                        if (result.Success) {
                                            var itemId = $(this).attr("baseid");
                                            $('.row' + itemId).removeClass("action-list-item-approved");
                                            $(".IconWait" + itemId).hide();
                                            $(".IconApproved" + itemId).hide();
                                            $(".IconApproval" + itemId).fadeTo(200, 1);
                                            $(".IconDenial" + itemId).fadeTo(200, 1);
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

            
        });

        var gridsLoaded = 0;

    </script>

    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
        .datagrid-row .ocrFont {  /* this is necessary for OCR to work as expected */
            font-family: Droid Sans Mono;
            font-weight: bold;
            font-size: 120%;
            letter-spacing: 1px;
        }
        .LocalIconApproval, .LocalIconApproved {
            padding-right: 3px;
        }
        body.rtl .LocalIconApproval, body.rtl .LocalIconApproved {
            padding-left: 3px;
            padding-right: initial;
        }
        .rowPrevious {
            color: #AAA;
            display: none;
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
                    <th data-options="field:'bank',width:160"><asp:Label ID="LabelGridHeaderCurrencyMethod" runat="server" Text="XYZ Bank" /></th>  
                    <th data-options="field:'amount',width:85,align:'right'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                    <th data-options="field:'action',width:68,align:'center'"><asp:Label ID="LabelGridHeaderPay" runat="server" Text="XYZPaid" /></th>
                </tr>  
            </thead>
        </table>
    </div>  
    <div id="divOcrView">
        <h2><asp:Label runat="server" ID="LabelPayOutMoneyOcrHeader" Text="XYZ Costs In Ocr View" /></h2>
        <table id="TablePayableCostsOcr" class="easyui-datagrid" style="width:680px;height:500px"
            data-options="rownumbers:false,singleSelect:false,fit:false,fitColumns:true,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-PayableCostsOcr.aspx'"
            idField="itemId">
            <thead>  
                <tr>  
                    <th data-options="field:'due',width:70"><asp:Label ID="LabelGridHeaderDue2" runat="server" Text="XYZ Due"/></th>  
                    <th data-options="field:'reference',width:165, align:'right'"><asp:Label ID="LabelGridHeaderReferenceOcr" runat="server" Text="XYZ Reference" /></th>
                    <th data-options="field:'amount',width:180, align: 'right'"><asp:Label ID="LabelGridHeaderAmountOcr" runat="server" Text="XYZ Amount" /></th>  
                    <th data-options="field:'account',width:150, align: 'right'"><asp:Label ID="LabelGridHeaderAccountOcr" runat="server" Text="XYZ Account" /></th>
                    <th data-options="field:'action',width:68,align:'center'"><asp:Label ID="LabelGridHeaderPaid2" runat="server" Text="XYZPaid" /></th>
                </tr>  
            </thead>
        </table>
    </div>
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
    
    <Swarmops5:ModalDialog ID="ModalConfirmPayment" runat="server" >
        <DialogCode>
            <h2>Testing</h2>
            Foobar high chaparral
        </DialogCode>
    </Swarmops5:ModalDialog>

</asp:Content>

