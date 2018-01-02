<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.InspectLedgers" Codebehind="InspectLedgers.aspx.cs" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ModalDialog" Src="~/Controls/v5/Base/ModalDialog.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.snipe.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />
    
    <script type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();

            $('#divActionAddTransaction').click(function() {
                if (canWriteRows) {
                    <%=this.DialogCreateTx.ClientID%>_open();
                    <%=this.TextCreateTxAmount.ClientID%>_initialize('');
                    <%=this.TextCreateTxDescription.ClientID%>_focus();
                    <%=this.TextCreateTxAmount.ClientID%>_initialize('<%=(0.0).ToString("N2")%>');
                    <%=this.TextCreateTxDateTime.ClientID%>_initialize('<%=DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString()%>');
                    <%=this.DropBudgetsCreateTx.ClientID%>_val(<%=this.DropBudgets.ClientID%>_val());
                } else {
                    alertify.error("You do not have sufficient authority to create transactions.");
                }
            });

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
                        inspectingTransactionId = $(this).attr("txId");
                        onFlagTransaction("Add Tx Id here");
                    });
                    $('img.LocalIconInspect').click(function() {
                        onInspectTransaction($(this).attr("txId"));
                    });
                }
            });

            $('#gridTransaction').datagrid({
                onLoadSuccess: function() {

                    // Dynamic reloading screws up resizing for some reason, so we'll have to resize the tx grid manually.

                    var heightBody = $('div#<%=this.DialogEditTx.ClientID%>_divModalCover table.datagrid-btable').height();

                    if (heightBody != null) 
                    {
                        $('div#<%=this.DialogEditTx.ClientID%>_divModalCover div.datagrid-body').height(heightBody);

                        var heightHeaders = $('div#<%=this.DialogEditTx.ClientID%>_divModalCover div.datagrid-header').height();
                        $('div#<%=this.DialogEditTx.ClientID%>_divModalCover div.datagrid-view').height(heightBody + heightHeaders + 20); // +20 adds some margin on the bottom. It's an arbitrary number.
                        $('div#<%=this.DialogEditTx.ClientID%>_divModalCover div.datagrid-wrap').height(heightBody + heightHeaders + 20);

                        $('div#<%=this.DialogEditTx.ClientID%>_divModalBox').height($('div#<%=this.DialogEditTx.ClientID%>_divModalBox div.content').height() + 20);
                    }
                }
            });

            $('#treeGeneralLedger').treegrid(
                {
                    onLoadSuccess: function() {
                        $("td > div > span.tx-description").each(function() {
                            var parent = $(this).parent();
                            var grandParent = parent.parent();

                            grandParent.attr("colSpan", 4);
                            parent.css("width", "100%");
                        });
                    }
                }
            );

            $('#<%= DropYears.ClientID %>').change(function() {
                reloadInspectData();
            });

            $('#<%= DropMonths.ClientID %>').change(function() {
                reloadInspectData();
            });

            $('#ButtonAddTransactionRow').click(function() {
                addTransactionRow();
            });

            $('#buttonCreateTransaction').click(function() {
                var jsonData = {};
                jsonData.dateTimeString = <%=this.TextCreateTxDateTime.ClientID%>_val();
                jsonData.amountString = <%=this.TextCreateTxAmount.ClientID%>_val();
                jsonData.description = <%=this.TextCreateTxDescription.ClientID%>_val();
                jsonData.budgetId = <%=this.DropBudgetsCreateTx.ClientID%>_val();

                // Disable create button to protect against double-clicking habits
                $("#buttonCreateTransaction").css("visibility", "hidden");

                SwarmopsJS.ajaxCall("/Pages/v5/Ledgers/InspectLedgers.aspx/CreateTransaction",
                    jsonData,
                    function(result) {
                        $("#buttonCreateTransaction").css("visibility", "visible");
                        console.log(result);
                        if (result.Success) {
                            <%=this.DialogCreateTx.ClientID%>_close();
                            onInspectTransaction(result.ObjectIdentity);
                        }
                    },
                    function(errorResult) {
                        $("#buttonCreateTransaction").css("visibility", "visible");
                        alertify.error("Unable to call server to create transaction");
                        // TODO: Show error message and re-enable create button
                    });
            });

            currentYear = $('#<%= DropYears.ClientID %>').val();

            $('div.datagrid').css('opacity', 0.4);

            $('#ButtonAddTransactionRow').val(buttonAddRowValue);

            // end of document.ready
        });

        var accountId = 0;
        var inspectingTransactionId = 0;
        var transactionDirty = false;

        function onModalClose() {
            if (transactionDirty) {
                $('#gridLedgers').datagrid('reload');
                transactionDirty = false;
            }
        }

        function onAccountSelected(newAccountId) {
            accountId = newAccountId;
            reloadInspectData();
        }

        function addTransactionRow() {
            var amountString = <%= TextInsertAmount.ClientID %>_val();
            var rowAccountId = 0;
            var selectedAccountNode = $('#<%= BudgetAddRow.ClientID %>_DropBudgets').combotree('tree').tree('getSelected');

            if (selectedAccountNode == null || selectedAccountNode.id < 1) {
                alertify.error(unescape('<asp:Literal ID="LiteralErrorAddRowSelectAccount" runat="server" />'));
                return;
            }

            if (canWriteRows) {

                transactionDirty = true;

                var jsonData = {};
                jsonData.txId = inspectingTransactionId;
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
                            prefillUnbalancedAmount(inspectingTransactionId);
                        } else {
                            alertify.error("<%= Global.Error_AjaxCallException %>");
                        }
                    },
                    error: function(msg) {
                        alertify.error("<%= Global.Error_AjaxCallException %>");
                    }
                });
            }
        }

        function reloadInspectData() {
            var selectedYear = $('#<%= DropYears.ClientID %>').val();
            var selectedMonth = $('#<%= DropMonths.ClientID %>').val();

            currentYear = selectedYear;
            closedLedgers = (currentYear <= ledgersClosedUntil);

            $('#gridLedgers').datagrid({ url: 'Json-InspectLedgerData.aspx?Year=' + selectedYear + "&Month=" + selectedMonth + "&AccountId=" + accountId });

            $('#imageLoadIndicator').show();
            $('div.datagrid').css('opacity', 0.4);

            // $('#gridOutstandingAccounts').datagrid('reload');
        }

        function prefillUnbalancedAmount(transactionId) {

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
                        <%= this.TextInsertAmount.ClientID %>_initialize(msg.d);
                    } else {
                        alertify.error("<%= Global.Error_AjaxCallException %>");
                    }
                },
                error: function(msg) {
                    alertify.error("<%= Global.Error_AjaxCallException %>");
                }
            });
        }

        function onInspectTransaction(transactionId) {
            window.scrollTo(0, 0);
            $('body').css('overflow-y', 'hidden');
            <%=this.DialogEditTx.ClientID%>_open();
            SwarmopsJS.formatInteger(transactionId, function(result) { $('span#spanModalTransactionId').text(result); });

            $('#gridTransaction').datagrid({ url: 'Json-InspectLedgerTxData.aspx?TxId=' + transactionId });

            var jsonData = {};
            jsonData.txId = transactionId;
            inspectingTransactionId = transactionId;

            if (canWriteRows && !closedLedgers) {
                prefillUnbalancedAmount(transactionId);
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
                            $('#divTransactionTracking').show();
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

                            if (canWriteRows && !closedLedgers) {
                                $('#divEditTransaction').show();
                            } else {
                                $('#divEditTransaction').hide();
                            }
                        }
                    },
                    error: function(msg) {
                        alertify.error("<%= Global.Error_AjaxCallException %>");
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
        var canSeeDetail =  <asp:Literal ID="LiteralDetailAccess" runat="server" />;
        var canWriteRows =  <asp:Literal ID="LiteralWriteAccess" runat="server" />;
        var canAuditTx =  <asp:Literal ID="LiteralAuditAccess" runat="server" />;

        var ledgersClosedUntil =  <asp:Literal ID="LiteralLedgersClosedUntil" runat="server" />;
        var currentYear = 0;

        var closedLedgers = false;

        var buttonAddRowValue = " " + SwarmopsJS.unescape('<asp:Literal ID="LiteralAddRowButton" runat="server" />') + " ";

    </script>


    <style type="text/css">
        .datagrid-row-selected, .datagrid-row-over { background: transparent; }

        table.datagrid-ftable { font-weight: 500; }

        .datagrid-body td{
            vertical-align: text-top;
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

        span.hiddenDocLinks { display: none; }

    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div class="tab" title="<img src='/Images/Icons/iconshock-groups-docs-lists-64px.png' height='64' width='64' />">
            <h2><asp:Label ID="LabelHeaderGeneral" runat="server" /> <asp:DropDownList runat="server" ID="DropGeneralYears"/> <asp:DropDownList runat="server" ID="DropGeneralMonths"/></h2>
    
                <table id="treeGeneralLedger" class="easyui-treegrid" style="width: 680px; height: 500px"
                data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-GeneralLedgerData.aspx?Year=2017&Month=12'"
                idField="id" treeField="id">
                <thead>  
                    <tr>  
                        <th data-options="field:'id',width:90"><asp:Label ID="LabelTreeHeaderId" runat="server" Text="ID#"/></th>  
                        <th data-options="field:'datetime',width:90,sortable:true"><asp:Label ID="LabelTreeHeaderDateTime" runat="server" Text="XYZ DateTime" /></th>
                        <th data-options="field:'txDescription',width:220"><asp:Label ID="LabelTreeHeaderDescriptionAccount" runat="server" Text="XYZ Description, Account" /></th>  
                        <th data-options="field:'deltaPos',width:70,align:'right'"><asp:Label ID="LabelTreeHeaderDeltaPositive" runat="server" Text="XYZ Debit" /></th>
                        <th data-options="field:'deltaNeg',width:70,align:'right'"><asp:Label ID="LabelTreeHeaderDeltaNegative" runat="server" Text="XYZ Credit" /></th>
                        <th data-options="field:'balance',width:80,align:'right'"><asp:Label ID="LabelTreeHeaderBalance" runat="server" Text="XYZ Balance" /></th>
                        <th data-options="field:'action',width:53,align:'center'"><asp:Label ID="LabelTreeHeaderAction" runat="server" Text="XYZAct" /></th>
                    </tr>  
                </thead>
            </table>  
        </div>
        <div class="tab" title="<img src='/Images/Icons/iconshock-search-256px.png' height='64' width='64' />">
            <h2><asp:Label ID="LabelHeaderInspect" runat="server" /> <Swarmops5:ComboBudgets Layout="Horizontal" ID="DropBudgets" OnClientSelect=" onAccountSelected " ListType="All" runat="server" /> <asp:Label ID="LabelHeaderInspectFor" runat="server" /> <asp:DropDownList runat="server" ID="DropYears"/> <asp:DropDownList runat="server" ID="DropMonths"/></h2>
    
                <table id="gridLedgers" class="easyui-datagrid" style="width: 680px; height: 500px"
                data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-InspectLedgerData.aspx'"
                idField="id">
                <thead>  
                    <tr>  
                        <th data-options="field:'id',width:70,align:'right'"><asp:Label ID="LabelGridHeaderId" runat="server" Text="ID#"/></th>  
                        <th data-options="field:'datetime',width:90,sortable:true"><asp:Label ID="LabelGridHeaderDateTime" runat="server" Text="XYZ DateTime" /></th>
                        <th data-options="field:'description',width:250"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>  
                        <th data-options="field:'deltaPos',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaPositive" runat="server" Text="XYZ Debit" /></th>
                        <th data-options="field:'deltaNeg',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaNegative" runat="server" Text="XYZ Credit" /></th>
                        <th data-options="field:'balance',width:80,align:'right'"><asp:Label ID="LabelGridHeaderBalance" runat="server" Text="XYZ Balance" /></th>
                        <th data-options="field:'action',width:43,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZAct" /></th>
                    </tr>  
                </thead>
            </table>  
        </div>
    </div>
   

    <Swarmops5:ModalDialog ID="DialogEditTx" OnClientClose="onModalClose" runat="server">
        <DialogCode>
            <h2><asp:Literal ID="LiteralEditHeader" runat="server"/></h2>

            <table id="gridTransaction" class="easyui-datagrid" style="width: 910px"
            data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:true,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-InspectLedgerTxData.aspx'"
            idField="id">
                <thead>
                    <tr>
                        <th data-options="field:'accountName',width:270"><asp:Label ID="LabelGridHeaderAccountName" runat="server" Text="XYZ Debit" /></th>  
                        <th data-options="field:'deltaPos',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaPositive2" runat="server" Text="XYZ Debit" /></th>
                        <th data-options="field:'deltaNeg',width:70,align:'right'"><asp:Label ID="LabelGridHeaderDeltaNegative2" runat="server" Text="XYZ Credit" /></th>
                        <th data-options="field:'dateTime',width:90"><asp:Label ID="LabelGridHeaderDateTimeEntered" runat="server" Text="XYZ DateTime" /></th>
                        <th data-options="field:'initials',width:50"><asp:Label ID="LabelGridHeaderInitials" runat="server" Text="ID#"/></th>  
                    </tr>
                </thead>
            </table>
                
            <div id="divEditTransaction">
                <h2><asp:Label ID="LabelAddTransactionRowsHeader" runat="server" /></h2>
                <span class="content"><h2 style="border-bottom: none"><asp:Label ID="LabelAddRowAccount" runat="server" /><Swarmops5:ComboBudgets ID="BudgetAddRow" ListType="All" runat="server" Layout="Horizontal" />, <asp:Label ID="LabelAddRowAmount" runat="server" /> <Swarmops5:CurrencyTextBox ID="TextInsertAmount" Layout="Horizontal" runat="server" /> <span class="elementFloatFar"><input id="ButtonAddTransactionRow" type="button" value='#AddRow#'/></span></h2></span>
            
            <div id="divTransactionDocumentation">
                <h2><asp:Label runat="server" ID="LabelTransactionDocumentation"/></h2>
                <asp:Label runat="server" ID="LabelTransactionHasDocumentation" Text="This transaction has X pages of documentation: "/>&nbsp;<span id="spanLabelDocumentationPages"></span>
            </div>
               
            </div>
            <div id="divTransactionTracking">
                <h2><asp:Label ID="LabelTrackedTransactionHeader" runat="server" /></h2>
                <div id="divTransactionTrackingDetails"></div>
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>

        
    <Swarmops5:ModalDialog ID="DialogCreateTx" runat="server">
        <DialogCode>
            <h2><asp:Label runat="server" ID="LabelCreateTxDialogHeader">Creating Transaction XYZ</asp:Label></h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox ID="TextCreateTxDateTime" runat="server"/>
                <Swarmops5:AjaxTextBox ID="TextCreateTxDescription" runat="server"/>
                <Swarmops5:ComboBudgets ID="DropBudgetsCreateTx" ListType="All" runat="server"/>
                <Swarmops5:CurrencyTextBox ID="TextCreateTxAmount" runat="server"/>
                <input type="button" id="buttonCreateTransaction" class="NoInputFocus buttonAccentColor" value="<%= this.Localized_CreateTx %>"/>
            </div>
            <div class="entryLabels">
                <asp:Label runat="server" ID="LabelAddTxDateTime" Text="DateTime XYZ"/><br/>
                <asp:Label runat="server" ID="LabelAddTxDescription" Text="Description XYZ"/><br/>
                <asp:Label runat="server" ID="LabelAddTxFirstRowAccount" Text="Account XYZ"/><br/>
                <asp:Label runat="server" ID="LabelAddTxFirstRowAmount" Text="Amount XYZ"/>
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>


</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <asp:Panel runat="server" ID="PanelCreateTxVisible">
    <h2 class="blue">ACTIONS<span class="arrow"></span></h2>
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" id="divActionAddTransaction">
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-invoice-32px.png'); background-position: -1px -1px; background-size: 16px 16px"></div>
               <asp:Label runat="server" ID="LabelAddTransaction" Text="Add Transaction XYZ"></asp:Label>
            </div>
        </div>
    </div>
    
    </asp:Panel>
</asp:Content>
