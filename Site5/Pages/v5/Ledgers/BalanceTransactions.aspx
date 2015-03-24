<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="BalanceTransactions.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.BalanceTransactions" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
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
        });

        var transactionId = 0;

        function onFixTransaction(newTransactionId) {
            transactionId = newTransactionId;
            $('span#spanModalTransactionId').text("<%=Resources.Global.Global_LoadingPlaceholder%>");
            SwarmopsJS.formatInteger(transactionId, function (result) { $('span#spanModalTransactionId').text(result); });
            $('input:radio[name="TxOptions"]').prop('checked', false);
            $('div.radioOption').hide();
            <%= this.DialogTx.ClientID %>_open();

            $('#spanTransactionUnbalancedBy').text('[...]');
            $('span#spanModalTransactionDate').text('[...]');
            <%=this.DropOpenPayouts.ClientID%>_loadData([{ id: 0, text: "<%=Resources.Global.Global_LoadingPlaceholder%>" }]);
            <%=this.DropOpenPayouts.ClientID%>_val('0');

            SwarmopsJS.ajaxCall(
                "/Pages/v5/Ledgers/BalanceTransactions.aspx/GetTransactionMatchability",
                { transactionId: transactionId },
                function(data) {
                    $('#spanTransactionUnbalancedBy').text(data.DifferingAmount);
                    $('span#spanModalTransactionDate').text(data.TransactionDate);

                    if (data.OpenPayoutData.length > 0) {
                        <%=this.DropOpenPayouts.ClientID%>_loadData(data.OpenPayoutData);
                        <%=this.DropOpenPayouts.ClientID%>_text("<%=Resources.Global.Global_SelectOne%>");
                    } else {
                        <%=this.DropOpenPayouts.ClientID%>_loadData({});
                        <%=this.DropOpenPayouts.ClientID%>_text("<%=Resources.Global.Global_NoMatch%>");
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
                <th field="checkTx" checkbox="true"></th>
                <th data-options="field:'id',width:60,align:'right'"><asp:Label ID="LabelGridHeaderId" runat="server" Text="ID#"/></th>  
                <th data-options="field:'dateTime',width:170,sortable:true"><asp:Label ID="LabelGridHeaderDateTime" runat="server" Text="XYZ DateTime" /></th>
                <th data-options="field:'accountName',width:170"><asp:Label ID="LabelGridHeaderAccountName" runat="server" Text="XYZ Debit" /></th>
                <th data-options="field:'description',width:210"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>  
                <th data-options="field:'delta',width:100,align:'right'"><asp:Label ID="LabelGridHeaderDelta" runat="server" Text="XYZ Delta" /></th>
                <th data-options="field:'action',width:43,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZAct" /></th>
            </tr>  
        </thead>
    </table>  
    
    
    <Swarmops5:ModalDialog runat="server" ID="DialogTx">
        <DialogCode>
            <h2><asp:Literal runat="server" ID="LiteralModalHeader" Text="Matching/Balancing transaction #foobar XYZ" /></h2>
            <p><asp:Literal ID="LabelDoYouWishTo" runat="server" Text="The balance is off by SEK +6,141.14. is Do you wish to... XYZ" /></p>
            <p><input type="radio" id="RadioBalance" name="TxOptions" value="Balance" /><label for="RadioBalance"><asp:Label runat="server" ID="LabelRadioBalance" Text="Balance the transaction manually? XYZ" /></label></p>
            <div id="radioOptionBalance" class="radioOption">
                <div class="entryFields">
                    <Swarmops5:ComboBudgets ID="DropBudgetBalance" runat="server" ListType="All" />&#8203;<br/>
                    <input type="button" value='<asp:Literal ID="LiteralButtonBalance" runat="server" Text="BalanceXYZ" />' class="buttonAccentColor" onclick="onBalanceTransaction(); return false;" id="buttonExecuteBalance"/>
                </div>
                <div class="entryLabels">
                    <asp:Label runat="server" ID="LabelDescribeBalance" Text="Balance the difference against XYZ" />
                </div>
                <div style="clear:both"></div>
            </div>
            <p><input type="radio" id="RadioPayout" name="TxOptions" value="Payout" /><label for="RadioPayout"><asp:Label runat="server" ID="LabelRadioPayout" Text="Match this balance to an open payout? XYZ" /></label></p>
            <div id="radioOptionPayout" class="radioOption">
                <div class="entryFields">
                    <Swarmops5:DropDown ID="DropOpenPayouts" runat="server" ListType="All" />&#8203;<br/>
                    <input type="button" value='<asp:Literal ID="LiteralButtonPayout" runat="server" Text="MatchXYZ" />' class="buttonAccentColor" onclick="onMatchOpenPayout(); return false;" id="buttonExecutePayout"/>
                </div>
                <div class="entryLabels">
                    <asp:Label runat="server" ID="LabelDescribePayout" Text="Match to payout XYZ" />
                </div>
            </div>
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

