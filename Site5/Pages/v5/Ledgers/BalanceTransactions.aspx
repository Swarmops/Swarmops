<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="BalanceTransactions.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.BalanceTransactions" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>
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
        });

        var transactionId = 0;

        function onFixTransaction(newTransactionId) {
            transactionId = newTransactionId;
            SwarmopsJS.formatInteger(transactionId, function(result) { $('span#spanModalTransactionId').text(result); });
            <%= this.DialogTx.ClientID %>_open();
        }
    </script>
    
    <style type="text/css">
        .LocalIconFix {
            cursor: pointer;
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
                <th data-options="field:'accountName',width:170,align:'right'"><asp:Label ID="LabelGridHeaderAccountName" runat="server" Text="XYZ Debit" /></th>
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
            <p><asp:RadioButton runat="server" ID="RadioBalance" GroupName="TxOptions" Text="Balance the transaction manually? XYZ" /></p>
            <p><asp:RadioButton runat="server" ID="RadioPayout" GroupName="TxOptions" Text="Match this balance to an open payout? XYZ" /></p>
            <p><asp:RadioButton runat="server" ID="RadioExistingPayment" GroupName="TxOptions" Text="Match this balance to a recorded payment? XYZ" /></p>
            <p><asp:RadioButton runat="server" ID="RadioExpectedPayment" GroupName="TxOptions" Text="Match this balance to an expected, unrecorded payment? XYZ" /></p>
        </DialogCode>
     </Swarmops5:ModalDialog>




</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

