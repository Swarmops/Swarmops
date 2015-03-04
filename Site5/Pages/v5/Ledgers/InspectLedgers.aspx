<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="InspectLedgers.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.InspectLedgers" %>
<%@ Import Namespace="System.Net.Mime" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.snipe.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />
    
    <script type="text/javascript">
        $(document).ready(function() {

            $('#gridLedgers').datagrid(
            {
                onLoadSuccess: function() {
                    $('div.datagrid').css('opacity', 1);
                    $('#imageLoadIndicator').hide();
                    $('span.loadingHeader').hide();

                    // Merge inbound, outbound balances

                    var rowCount = $('#gridLedgers').datagrid('getRows').length;
                    if (rowCount > 0) {
                        $('#gridLedgers').datagrid('mergeCells', {
                            index: 0,
                            field: 'description',
                            colspan: 3
                        });
                        $('#gridLedgers').datagrid('mergeCells', {
                            index: rowCount - 1,
                            field: 'description',
                            colspan: 3
                        });
                        //$('#gridOutstandingAccounts').dataGrid('mergeCells', {index: rowCount, field:'id', colspan: 5});
                        // Footer cells could not be merged in datagrid :(

                    }

                    // Enable various actions on icon

                    $('img.LocalIconFlag').click(function() {
                        transactionId = $(this).attr("txId");
                        onFlagTransaction("Add Tx Id here");
                    });
                    $('img.LocalIconInspect').click(function() {
                        transactionId = $(this).attr("txId");
                        onInspectTransaction(transactionId);
                    });
                }
            });

            $('#gridTransaction').datagrid({
                onLoadSuccess: function() {
                        
                    // Dynamic reloading screws up resizing for some reason, so we'll have to resize the tx grid manually.

                    var heightBody = $('div#divModalCover table.datagrid-btable').height();
                    $('div#divModalCover div.datagrid-body').height(heightBody);

                    var heightHeaders = $('div#divModalCover div.datagrid-header').height();
                    $('div#divModalCover div.datagrid-view').height(heightBody + heightHeaders + 20); // +20 adds some margin on the bottom. It's an arbitrary number.
                    $('div#divModalCover div.datagrid-wrap').height(heightBody + heightHeaders + 20);

                    $('div#divModalBox').height($('div#divModalBox div.content').height() + 20);

                }
            });

            $('#<%=DropYears.ClientID %>').change(function () {
                reloadData();
            });

            $('#<%=DropMonths.ClientID %>').change(function () {
                reloadData();
            });

            $("#IconCloseEdit").click(function () {
                $('#divModalCover').fadeOut();

                if (transactionDirty) {
                    $('#gridLedgers').datagrid('reload');
                    transactionDirty = false;
                }
            });

            $('#ButtonAddTransactionRow').click(function() {
                addTransactionRow();
            });

            $('div.datagrid').css('opacity', 0.4);
        });
	    
        var accountId = 0;
        var transactionId = 0;
        var transactionDirty = false;

        function onAccountSelected(newAccountId) {
            accountId = newAccountId;
            reloadData();
        }

        function addTransactionRow() {
            var amountString = $('#<%=TextInsertAmount.ClientID%>_Input').val();
            var rowAccountId = 0;
            var selectedAccountNode = $('#<%=this.BudgetAddRow.ClientID%>_DropBudgets').combotree('tree').tree('getSelected');
            
            if (selectedAccountNode == null || selectedAccountNode.id < 1) {
                alertify.error (unescape('<asp:Literal ID="LiteralErrorAddRowSelectAccount" runat="server" />'));
                return;
            }

            if (canWriteRows) {

                transactionDirty = true;

                var jsonData = {};
                jsonData.txId = transactionId;
                jsonData.accountId = selectedAccountNode.id;
                jsonData.amountString = amountString;

                $.ajax({
                    type: "POST",
                    url: "/Pages/v5/Ledgers/InspectLedgers.aspx/AddTransactionRow",
                    data: $.toJSON(jsonData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg) {
                        if (msg.d) { // true = success
                            $('#gridTransaction').datagrid('reload');
                            prefillUnbalancedAmount();
                        } else {
                            alertify.error("<%= Resources.Global.Error_AjaxCallException %>");
                        }
                    },
                    error: function(msg) {
                        alertify.error("<%= Resources.Global.Error_AjaxCallException %>");
                    }
                });
                }
            }

            function reloadData()
            {
                var selectedYear = $('#<%=DropYears.ClientID %>').val();
            var selectedMonth = $('#<%=DropMonths.ClientID %>').val();

            $('#gridLedgers').datagrid({ url: 'Json-InspectLedgerData.aspx?Year=' + selectedYear + "&Month=" + selectedMonth + "&AccountId=" + accountId});

            $('#imageLoadIndicator').show();
            $('div.datagrid').css('opacity', 0.4);

            // $('#gridOutstandingAccounts').datagrid('reload');
        }

        function prefillUnbalancedAmount() {

            var jsonData = {};
            jsonData.txId = transactionId;

            $.ajax({
                type: "POST",
                url: "/Pages/v5/Ledgers/InspectLedgers.aspx/GetUnbalancedAmount",
                data: $.toJSON(jsonData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    if (msg.d.length > 1) {
                        $('#<%=this.TextInsertAmount.ClientID%>_Input').val(msg.d);
                    } else {
                        alertify.error("<%= Resources.Global.Error_AjaxCallException %>");
                    }
                },
                error: function(msg) {
                    alertify.error("<%= Resources.Global.Error_AjaxCallException %>");
                    }
            });
                }

                function onInspectTransaction(transactionId) {
                    window.scrollTo(0, 0);
                    $('body').css('overflow-y', 'hidden');
                    $('#divModalCover').fadeIn();
                    $('span#spanModalTransactionId').text(transactionId);

                    $('#gridTransaction').datagrid({ url: 'Json-InspectLedgerTxData.aspx?TxId=' + transactionId });

                    var jsonData = {};
                    jsonData.txId = transactionId;

                    if (canWriteRows) {
                        prefillUnbalancedAmount();
                    }

                    if (canSeeDetail || canWriteRows) {
                        $.ajax({
                            type: "POST",
                            url: "/Pages/v5/Ledgers/InspectLedgers.aspx/GetTransactionTracking",
                            data: $.toJSON(jsonData),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function(msg) {
                                if (msg.d.length > 1) {
                                    $('#divTransactionTrackingDetails').html(msg.d);
                                    $('#divTransactionTrackingDetails').show();
                                    $('#divEditTransaction').hide();

                                    $("a.FancyBox_Gallery").fancybox({
                                        'overlayShow': true,
                                        'transitionIn': 'fade',
                                        'transitionOut': 'fade',
                                        'type': 'image',
                                        'opacity': true
                                    });

                                    $("a.linkViewDox").click(function() {
                                        $("a.FancyBox_Gallery[rel='" + $(this).attr("objectId") + "']").first().click();
                                    });


                                } else {
                                    $('#divTransactionTracking').hide();

                                    if (canWriteRows) {
                                        $('#divEditTransaction').show();
                                    } else {
                                        $('#divEditTransaction').hide();
                                    }
                                }
                            },
                            error: function(msg) {
                                alertify.error("<%= Resources.Global.Error_AjaxCallException %>");
                    }
                });
                } else {
                    $('#divEditTransaction').hide();
                    $('#divTransactionTracking').hide();
                }
            }

            function onFlagTransaction(transactionId) {
                alertify.log('<asp:Label ID="LabelFlagNotAvailable" runat="server" />');
            }

            // the variables below only handle the cosmetics and not actual access
            var canSeeDetail = <asp:Literal ID="LiteralDetailAccess" runat="server" />;
            var canWriteRows = <asp:Literal ID="LiteralWriteAccess" runat="server" />;
            var canAuditTx = <asp:Literal ID="LiteralAuditAccess" runat="server" />;

            var ledgersClosedUntil = <asp:Literal ID="LiteralLedgersClosedUntil" runat="server" />;
            var currentYear = 0;

	</script>


    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
	    }
   	    table.datagrid-ftable {
		    font-weight: 500;
	    }

        .LocalIconInspect, .LocalIconFlag {
            cursor:pointer;
            position: relative;
            top: 4px;
        }

        .spanAnnoyingBlink {
            -webkit-animation: blink 0.5s linear infinite;
            animation: blink 0.5s linear infinite;
        }

        @keyframes blink {  
            0% { color: darkred; }
            100% { color: red; }
        }
        @-webkit-keyframes blink {  
            0% { color: darkred; }
            100% { color: red; }
        }

        span.hiddenDocLinks {
            display: none;
        }

    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <h2><asp:Label ID="LabelHeaderInspect" runat="server" /> <Swarmops5:ComboBudgets ID="DropBudgets" OnClientSelect="onAccountSelected" ListType="All" runat="server" /> <asp:Label ID="LabelHeaderInspectFor" runat="server" /> <asp:DropDownList runat="server" ID="DropYears"/> <asp:DropDownList runat="server" ID="DropMonths"/></h2>
    
        <table id="gridLedgers" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-InspectLedgerData.aspx'"
        idField="id">
        <thead>  
            <tr>  
                <th data-options="field:'id',width:50,align:'right'"><asp:Label ID="LabelGridHeaderId" runat="server" Text="ID#"/></th>  
                <th data-options="field:'datetime',width:90,sortable:true"><asp:Label ID="LabelGridHeaderDateTime" runat="server" Text="XYZ DateTime" /></th>
                <th data-options="field:'description',width:270"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>  
                <th data-options="field:'deltaPos',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaPositive" runat="server" Text="XYZ Debit" /></th>
                <th data-options="field:'deltaNeg',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaNegative" runat="server" Text="XYZ Credit" /></th>
                <th data-options="field:'balance',width:80,align:'right'"><asp:Label ID="LabelGridHeaderBalance" runat="server" Text="XYZ Balance" /></th>
                <th data-options="field:'action',width:43,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZAct" /></th>
            </tr>  
        </thead>
    </table>  
    
    
        <div id="divModalCover" class="modalcover">
        <div id="divModalBox" class="box modal">
            <div class="content" style="overflow:hidden">
                <div class="divIconCloseModal"><img id="IconCloseEdit" src="/Images/Icons/iconshock-cross-16px.png" /></div><h2><asp:Literal ID="LiteralEditHeader" runat="server"/></h2>
                <table id="gridTransaction" class="easyui-datagrid" style="width:910px"
                data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:true,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-InspectLedgerTxData.aspx'"
                idField="id">
                    <thead>
                        <tr>
                            <th data-options="field:'accountName',width:270"><asp:Label ID="LabelGridHeaderAccountName" runat="server" Text="XYZ Description" /></th>  
                            <th data-options="field:'deltaPos',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaPositive2" runat="server" Text="XYZ Debit" /></th>
                            <th data-options="field:'deltaNeg',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaNegative2" runat="server" Text="XYZ Credit" /></th>
                            <th data-options="field:'dateTime',width:90"><asp:Label ID="LabelGridHeaderDateTimeEntered" runat="server" Text="XYZ DateTime" /></th>
                            <th data-options="field:'initials',width:50"><asp:Label ID="LabelGridHeaderInitials" runat="server" Text="ID#"/></th>  
                        </tr>
                    </thead>
                </table>
                
                <div id="divEditTransaction">
                    <h2><asp:Label ID="LabelAddTransactionRowsHeader" runat="server" /></h2>
                    <span class="content"><h2 style="border-bottom: none"><asp:Label ID="LabelAddRowAccount" runat="server" /><Swarmops5:ComboBudgets ID="BudgetAddRow" ListType="All" runat="server" />, <asp:Label ID="LabelAddRowAmount" runat="server" /> <Swarmops5:CurrencyTextBox ID="TextInsertAmount" runat="server" /> <span class="elementFloatFar"><input id="ButtonAddTransactionRow" type="button" value=' <asp:Literal ID="LiteralAddRowButton" runat="server" /> '/></span></h2></span>
                    
                </div>
                <div id="divTransactionTracking">
                <h2><asp:Label ID="LabelTrackedTransactionHeader" runat="server" /></h2>
                <div id="divTransactionTrackingDetails"></div>
                </div>

            </div>
        </div>
    </div>



</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>
