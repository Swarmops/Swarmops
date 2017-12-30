<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.BalanceTransactions" Codebehind="BalanceTransactions.aspx.cs" CodeFile="BalanceTransactions.aspx.cs" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="DropDown" Src="~/Controls/v5/Base/DropDown.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ModalDialog" Src="~/Controls/v5/Base/ModalDialog.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <script language="javascript" type="text/javascript">
        $(document).ready( function() {
            $('#gridTransactions').datagrid({
                onLoadSuccess: function() {

                    $('img.LocalIconFix').click(function(e) {
                        transactionId = $(this).attr("txId");
                        onFixTransaction(transactionId);
                        e.stopPropagation(); // prevents checking of row
                    });

                }
            });

            $('input:radio[name="TxOptions"]').change(function() {
                var pickedButtonName = $(this).val();
                $('div.radioOption').slideUp();
                $('div#radioOption' + pickedButtonName).slideDown();
            });

            $('#buttonExecuteBalance').val(buttonBalanceValue);
            $('#buttonExecutePayout').val(buttonPayoutValue);
            $('#buttonExecutePayoutForeign').val(buttonPayoutForeignValue);
            $('#buttonExecuteOutboundInvoice').val(buttonOutboundInvoiceValue);
            $('#buttonExecuteVatReport').val(buttonVatReportValue);
            $('#buttonExecutePurchase').val(buttonPurchaseValue);

            if (isVatEnabled) {
                $(".onVatEnabled").show();
            }

        });

        var isVatEnabled = <%= CurrentOrganization.VatEnabled? "true": "false" %>;
        var transactionId = 0;

        function onFixTransaction(newTransactionId) {
            transactionId = newTransactionId;
            $('span#spanModalTransactionId').text("<%=Resources.Global.Global_LoadingPlaceholder%>");
            SwarmopsJS.ajaxCall(
                '/Pages/v5/Ledgers/BalanceTransactions.aspx/GetTransactionDisplayIdentity',
                { transactionId: transactionId },
                function(result) {
                     $('span#spanModalTransactionId').text(result);
                });
            $('input:radio[name="TxOptions"]').prop('checked', false);
            $('div.radioOption').hide();
            <%= this.DialogTx.ClientID %>_open();

            $('#spanTransactionUnbalancedBy').text('[...]');
            $('span#spanModalTransactionDate').text('[...]');
            <%=this.DropOpenPayouts.ClientID%>_loadData([{ id: 0, text: "<%=Resources.Global.Global_LoadingPlaceholder%>" }]);
            <%=this.DropOpenPayouts.ClientID%>_val('0');
            <%=this.DropOpenPayoutsForeign.ClientID%>_loadData([{ id: 0, text: "<%=Resources.Global.Global_LoadingPlaceholder%>" }]);
            <%=this.DropOpenPayoutsForeign.ClientID%>_val('0');

            SwarmopsJS.ajaxCall(
                "/Pages/v5/Ledgers/BalanceTransactions.aspx/GetTransactionMatchability",
                { transactionId: transactionId },
                function(data) {
                    $('#spanTransactionUnbalancedBy').text(data.DifferingAmount);
                    $('span#spanModalTransactionDate').text(data.TransactionDate);
                    $('input#inputTextPurchaseDescription').val(data.TransactionDescription);
                    $('input#inputTextPurchaseAmount').val(data.AmountAsPurchase); 

                    if (data.OpenPayoutData == null) // amount is positive
                    {
                        $('#divPositiveDifference').show();
                        $('#divNegativeDifference').hide();

                        if (data.OpenOutboundInvoiceData != null) {
                            if (data.OpenOutboundInvoiceData.ExactMatches.length > 0) {
                                $('#divOutboundInvoice').show();
                                <%=this.DropOpenOutboundInvoices.ClientID%>_loadData(data.OpenOutboundInvoiceData.ExactMatches);
                                <%=this.DropOpenOutboundInvoices.ClientID%>_text("<%=Resources.Global.Global_SelectOne%>");
                            } else {
                                $('#divOutboundInvoice').hide();
                                <%=this.DropOpenOutboundInvoices.ClientID%>_loadData({});
                                <%=this.DropOpenOutboundInvoices.ClientID%>_text("<%=Resources.Global.Global_NoMatch%>");
                            }
                        }

                    } else {                            // amount is negative
                        $('#divPositiveDifference').hide();
                        $('#divNegativeDifference').show();

                        if (data.OpenPayoutData != null) {
                            if (data.OpenPayoutData.ExactMatches.length > 0) {
                                <%=this.DropOpenPayouts.ClientID%>_loadData(data.OpenPayoutData.ExactMatches);
                                <%=this.DropOpenPayouts.ClientID%>_text("<%=Resources.Global.Global_SelectOne%>");
                            } else {
                                <%=this.DropOpenPayouts.ClientID%>_loadData({});
                                <%=this.DropOpenPayouts.ClientID%>_text("<%=Resources.Global.Global_NoMatch%>");
                            }

                            if (data.OpenPayoutData.TolerantMatches.length > 0) {
                                <%=this.DropOpenPayoutsForeign.ClientID%>_loadData(data.OpenPayoutData.TolerantMatches);
                                <%=this.DropOpenPayoutsForeign.ClientID%>_text("<%=Resources.Global.Global_SelectOne%>");
                            } else {
                                <%=this.DropOpenPayoutsForeign.ClientID%>_loadData({});
                                <%=this.DropOpenPayoutsForeign.ClientID%>_text("<%=Resources.Global.Global_NoMatch%>");
                            }

                        }

                    }

                    if (data.OpenVatReportData != null && data.OpenVatReportData.length > 0) {
                        $('#divVatReport').show();
                        <%=this.DropOpenVatReports.ClientID%>_loadData(data.OpenVatReportData);
                        <%=this.DropOpenVatReports.ClientID%>_text("<%=Resources.Global.Global_SelectOne%>");
                    } else {
                        $('#divVatReport').hide();
                        <%=this.DropOpenVatReports.ClientID%>_loadData({});
                        <%=this.DropOpenVatReports.ClientID%>_text("<%=Resources.Global.Global_NoMatch%>");
                    }

                });

        }

        function onBalanceTransaction() {
            var accountId = <%=this.DropBudgetBalance.ClientID%>_val();

            if (accountId > 0) {
                <%= this.DialogTx.ClientID %>_close();

                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/BalanceTransactions.aspx/BalanceTransactionManually",
                    { transactionId: transactionId, accountId: accountId },
                    function () {
                        $('#gridTransactions').datagrid('reload');
                    });

            }
        }

        function onMatchOpenPayout() {
            var payoutId = <%=this.DropOpenPayouts.ClientID%>_val();

            if (payoutId > 0) {
                <%= this.DialogTx.ClientID %>_close();
                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/BalanceTransactions.aspx/MatchTransactionOpenPayout",
                    { transactionId: transactionId, payoutId: payoutId },
                    function () {
                        $('#gridTransactions').datagrid('reload');
                    });

            }
        }

        function onMatchOpenPayoutForeign() {
            var payoutId = <%=this.DropOpenPayoutsForeign.ClientID%>_val();

            if (payoutId > 0) {
                <%= this.DialogTx.ClientID %>_close();
                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/BalanceTransactions.aspx/MatchTransactionOpenPayoutForeign",
                    { transactionId: transactionId, payoutId: payoutId },
                    function () {
                        $('#gridTransactions').datagrid('reload');
                    });

            }
        }

        function onMatchOpenOutboundInvoice() {
            var invoiceId = <%=this.DropOpenOutboundInvoices.ClientID%>_val();

            if (invoiceId > 0) {
                <%= this.DialogTx.ClientID %>_close();
                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/BalanceTransactions.aspx/MatchTransactionOpenOutboundInvoice",
                    { transactionId: transactionId, invoiceId: invoiceId },
                    function () {
                        $('#gridTransactions').datagrid('reload');
                    });

            }
        }


        function onMatchOpenVatReport() {
            var vatReportId = <%=this.DropOpenVatReports.ClientID%>_val();

            if (vatReportId > 0) {
                <%= this.DialogTx.ClientID %>_close();
                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/BalanceTransactions.aspx/MatchTransactionOpenVatReport",
                    { transactionId: transactionId, vatReportId: vatReportId },
                    function () {
                        $('#gridTransactions').datagrid('reload');
                    });

            }
        }

        function onCreateDirectPurchase() {
            var budgetId = <%=this.DropBudgetsPurchase.ClientID%>_val();
            var vatAmount = <%=this.CurrencyPurchaseVat.ClientID%>_val();
            var txDescription = $("#inputTextPurchaseDescription").val();

            if (budgetId > 0) {
                SwarmopsJS.ajaxCall(
                    "/Pages/v5/Ledgers/BalanceTransactions.aspx/MarkDirectPurchase",
                    {
                        transactionId: transactionId,
                        budgetId: budgetId,
                        vatAmountString: vatAmount,
                        newDescription: txDescription,
                        guid: uploadGuid
                    },
                    function(result) {
                        if (result.Success) {
                            // close dialog, clear fields, reload
                            // Keep the budget selection
                            <%=this.UploadPurchase.ClientID%>_clear();
                            <%=this.CurrencyPurchaseVat.ClientID%>_setValue("");
                            
                            <%= this.DialogTx.ClientID %>_close();
                            $('#gridTransactions').datagrid('reload');

                        } else {
                            alertify.error(result.DisplayMessage);
                        }
                    });
            }

            // TODO HERE: AJAX CALL

        }

        var buttonBalanceValue = SwarmopsJS.unescape('<%=this.Localized_ButtonBalance%>');
        var buttonPayoutValue = SwarmopsJS.unescape('<%=this.Localized_ButtonPayout%>');
        var buttonPayoutForeignValue = SwarmopsJS.unescape('<%=this.Localized_ButtonPayoutForeign%>');
        var buttonOutboundInvoiceValue = SwarmopsJS.unescape('<%=this.Localized_ButtonOutboundInvoice%>');
        var buttonVatReportValue = SwarmopsJS.unescape('<%=this.Localized_ButtonVatReport%>');
        var buttonPurchaseValue = SwarmopsJS.unescape('<%=this.Localized_ButtonPurchase%>');

        var uploadGuid = '<%=this.UploadPurchase.GuidString%>';

    </script>
    
    <style type="text/css">
        .LocalIconFix {
            cursor: pointer;
        }
        div.radioOption {
            margin-top: -20px;
            padding-bottom: 10px;
            padding-left: 12px;
            margin-right: 10px;
        }
        body.ltr div.radioOption {
            padding-left: initial;
            margin-right: initial;
            padding-right: 12px;
            margin-left: 10px;
        }

    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
  
        <h2><asp:Label ID="LabelHeaderUnbalancedTransactions" runat="server" /></h2>
    
        <table id="gridTransactions" class="easyui-datagrid" style="width: 680px; height: 500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'/Pages/v5/Ledgers/Json-UnbalancedTransactions.aspx'"
        idField="id">
        <thead>  
            <tr>
                <!--<th field="checkTx" checkbox="true"></th>-->
                <th data-options="field:'id',width:60,align:'right'"><asp:Label ID="LabelGridHeaderId" runat="server" Text="ID#"/></th>  
                <th data-options="field:'dateTime',width:90,sortable:true"><asp:Label ID="LabelGridHeaderDateTime" runat="server" Text="XYZ DateTime" /></th>
                <th data-options="field:'accountName',width:170"><asp:Label ID="LabelGridHeaderAccountName" runat="server" Text="XYZ Debit" /></th>
                <th data-options="field:'description',width:280"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>  
                <th data-options="field:'delta',width:100,align:'right'"><asp:Label ID="LabelGridHeaderDelta" runat="server" Text="XYZ Delta" /></th>
                <th data-options="field:'action',width:53,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZAct" /></th>
            </tr>  
        </thead>
    </table>  
    
    
    <Swarmops5:ModalDialog runat="server" ID="DialogTx">
        <DialogCode>
            <h2><asp:Literal runat="server" ID="LiteralModalHeader" Text="Matching/Balancing transaction #foobar XYZ" /></h2>
            <p><asp:Literal ID="LabelDoYouWishTo" runat="server" Text="The balance is off by SEK +6,141.14. is Do you wish to... XYZ" /></p>
            <div id="divVatReport">
                <p><input type="radio" id="RadioVatReport" name="TxOptions" value="VatReport" /><label for="RadioVatReport">&nbsp;<asp:Label runat="server" ID="LabelRadioVatReport" Text="Match to an open VAT report? XYZ" /></label></p>
                <div id="radioOptionVatReport" class="radioOption">
                    <div class="entryFields">
                        <Swarmops5:DropDown ID="DropOpenVatReports" runat="server" />&#8203;<br/>
                        <input type="button" value='#VatReport#' class="buttonAccentColor" onclick="onMatchOpenVatReport(); return false;" id="buttonExecuteVatReport"/>
                    </div>
                    <div class="entryLabels">
                        <asp:Label runat="server" ID="LabelDescribeVatReport" Text="Match to open VAT report XYZ" />
                    </div>
                </div>
            </div>
            <div id="divNegativeDifference">
                <p><input type="radio" id="RadioPayout" name="TxOptions" value="Payout" /><label for="RadioPayout">&nbsp;<asp:Label runat="server" ID="LabelRadioPayout" Text="Match this balance to an open payout? XYZ" /></label></p>
                <div id="radioOptionPayout" class="radioOption">
                    <div class="entryFields">
                        <Swarmops5:DropDown ID="DropOpenPayouts" runat="server" ListType="All" />&#8203;<br/>
                        <input type="button" value='#Payout#' class="buttonAccentColor" onclick="onMatchOpenPayout(); return false;" id="buttonExecutePayout"/>
                    </div>
                    <div class="entryLabels">
                        <asp:Label runat="server" ID="LabelDescribePayout" Text="Match to payout XYZ" />
                    </div>
                </div>
                <p><input type="radio" id="RadioPayoutForeign" name="TxOptions" value="PayoutForeign" /><label for="RadioPayoutForeign">&nbsp;<asp:Label runat="server" ID="LabelRadioPayoutForeign" Text="Match to an open foreign-currency payout? XYZ" /></label></p>
                <div id="radioOptionPayoutForeign" class="radioOption">
                    <div class="entryFields">
                        <Swarmops5:DropDown ID="DropOpenPayoutsForeign" runat="server" ListType="All" />&#8203;<br/>
                        <input type="button" value='#PayoutForeign#' class="buttonAccentColor" onclick="onMatchOpenPayoutForeign(); return false;" id="buttonExecutePayoutForeign"/>
                    </div>
                    <div class="entryLabels">
                        <asp:Label runat="server" ID="LabelDescribePayoutForeign" Text="Match to payout XYZ" />
                    </div>
                </div>
            </div>
            <div id="divPositiveDifference">
                <div id="divOutboundInvoice">
                    <p><input type="radio" id="RadioOutboundInvoice" name="TxOptions" value="OutboundInvoice" /><label for="RadioOutboundInvoice">&nbsp;<asp:Label runat="server" ID="LabelRadioOutboundInvoice" Text="Match to an open outbound invoice? XYZ" /></label></p>
                    <div id="radioOptionOutboundInvoice" class="radioOption">
                        <div class="entryFields">
                            <Swarmops5:DropDown ID="DropOpenOutboundInvoices" runat="server" />&#8203;<br/>
                            <input type="button" value='#PaymentInvoice#' class="buttonAccentColor" onclick="onMatchOpenOutboundInvoice(); return false;" id="buttonExecuteOutboundInvoice"/>
                        </div>
                        <div class="entryLabels">
                            <asp:Label runat="server" ID="LabelDescribeOutboundInvoice" Text="Match to outbound invoice XYZ" />
                        </div>
                    </div>
                </div>
            </div>
            <p><input type="radio" id="RadioPurchase" name="TxOptions" value="Purchase" /><label for="RadioPurchase">&nbsp;<asp:Label runat="server" ID="LabelRadioPurchase" Text="Mark this as a direct-from-account purchase? XYZ" /></label></p>
            <div id="radioOptionPurchase" class="radioOption">
                <div class="entryFields">
                    <div class="stacked-input-control"><input type="text" value="" readonly="readonly" disabled="disabled" class="alignRight" id="inputTextPurchaseAmount"/></div>
                    <Swarmops5:ComboBudgets Layout="Vertical" ID="DropBudgetsPurchase" runat="server" ListType="InvoiceableIn" />
                    <div class="stacked-input-control"><input type="text" value="" id="inputTextPurchaseDescription"/></div>
                    <div class="onVatEnabled" style="display: none"><Swarmops5:CurrencyTextBox ID="CurrencyPurchaseVat" runat="server" Layout="Vertical"/></div>
                    <Swarmops5:FileUpload ID="UploadPurchase" runat="server"/>
                    <input type="button" value='#Balance#' class="buttonAccentColor" onclick="onCreateDirectPurchase(); return false;" id="buttonExecutePurchase"/>
                </div>
                <div class="entryLabels">
                    <div class="stacked-input-control"><asp:Label runat="server" ID="LabelDescribePurchaseAmount" Text="Direct purchase amount (CUR) XYZ" /></div>
                    <div class="stacked-input-control"><asp:Label runat="server" ID="LabelDescribePurchaseBudget" Text="Charge purchase to this budget XYZ" /></div>
                    <div class="stacked-input-control"><asp:Label runat="server" ID="LabelDescribePurchaseDescriptionUpdate" Text="Update transaction description XYZ" /></div>
                    <div class="onVatEnabled" style="display:none"><div class="stacked-input-control"><asp:Label runat="server" ID="LabelDescribePurchaseVatAmount" Text="VAT part of the amount XYZ" /></div></div>
                    <div class="stacked-input-control"><asp:Label runat="server" ID="LabelDescribePurchaseUploadReceipt" Text="Upload the receipt or other documentation XYZ" /></div>
                </div>
                <div style="clear:both"></div>
            </div>
            <p><input type="radio" id="RadioBalance" name="TxOptions" value="Balance" /><label for="RadioBalance">&nbsp;<asp:Label runat="server" ID="LabelRadioBalance" Text="Balance the transaction manually? XYZ" /></label></p>
            <div id="radioOptionBalance" class="radioOption">  <!-- this should go last -->
                <div class="entryFields">
                    <Swarmops5:ComboBudgets ID="DropBudgetBalance" runat="server" Layout="Vertical" ListType="All" />
                    <div class="stacked-input-control"><input type="button" value='#Balance#' class="buttonAccentColor" onclick="onBalanceTransaction(); return false;" id="buttonExecuteBalance"/></div>
                </div>
                <div class="entryLabels">
                    <asp:Label runat="server" ID="LabelDescribeBalance" Text="Balance the difference against XYZ" />
                </div>
                <div style="clear:both"></div>
            </div>
            <div id="radioOptionPayment2" class="radioOption"></div>
            <div id="radioOptionPaymentForeign" class="radioOption"></div>
            <div id="divHiddenTodoFutureSprint" style="display:none">
                <p><input type="radio" id="RadioExistingPayment" name="TxOptions" value="ExistingPayment" /><label for="RadioExistingPayment"><asp:Label runat="server" ID="LabelRadioExistingPayment" Text="Match this balance to a recorded payment, uploaded in a payments file? XYZ" /></label></p>
                <div id="radioOptionExistingPayment" class="radioOption">
                    <div class="entryFields">
                        <Swarmops5:DropDown ID="DropExistingPayments" runat="server" ListType="All" />&#8203;<br/>
                        <input type="button" value='<asp:Literal ID="LiteralButtonExistingPayment" runat="server" Text="MatchXYZ" />' class="buttonAccentColor" id="button2"/>
                    </div>
                    <div class="entryLabels">
                        <asp:Label runat="server" ID="LabelDescribeExistingPayment" Text="Match existing payment XYZ" />
                    </div>
                </div>
                <p><input type="radio" id="RadioExpectedPayment" name="TxOptions" value="ExpectedPayment" /><label for="RadioExpectedPayment"><asp:Label runat="server" ID="LabelRadioExpectedPayment" Text="Match this balance to an expected payment that has not been previously uploaded? XYZ" /></label></p>
                <div id="radioOptionExpectedPayment" class="radioOption">
                    <div class="entryFields">
                        <Swarmops5:DropDown ID="DropExpectedPayments" runat="server" ListType="All" />&#8203;<br/>
                        <input type="button" value='<asp:Literal ID="LiteralButtonExpectedPayment" runat="server" Text="MatchXYZ" />' class="buttonAccentColor" id="button3"/>
                    </div>
                    <div class="entryLabels">
                        <asp:Label runat="server" ID="LabelDescribeExpectedPayments" Text="Match expected payment XYZ" />
                    </div>
                </div>
            </div>
        </DialogCode>
     </Swarmops5:ModalDialog>




</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

