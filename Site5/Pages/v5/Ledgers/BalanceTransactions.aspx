<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="BalanceTransactions.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.BalanceTransactions" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="CurrencyTextBox" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ModalDialog" Src="~/Controls/v5/Base/ModalDialog.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <script language="javascript" type="text/javascript">
        $(document).ready( function() {
            $('#gridTransactions').datagrid({
                onLoadSuccess: function() {

                    $('img.LocalIconFix').click(function() {
                        transactionId = $(this).attr("txId");
                        onFixTransaction(transactionId);
                    });

                }
            });
        });

        var transactionId = 0;

        function onFixTransaction(newTransactionId) {
            transactionId = newTransactionId;
        }
    </script>

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
                    <div class="divIconCloseModal"><img id="IconCloseEdit" src="/Images/Icons/iconshock-cross-16px.png" /></div><h2><asp:Literal ID="LiteralEditHeader" runat="server"/></h2>
                    <table id="gridTransaction" class="easyui-datagrid" style="width: 910px"
                    data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:true,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-InspectLedgerTxData.aspx'"
                    idField="id">
                        <thead>
                            <tr>
                                <th data-options="field:'accountName',width:270"><asp:Label ID="LabelGridHeaderAccountName2" runat="server" Text="XYZ Description" /></th>  
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

                        
        </DialogCode>
     </Swarmops5:ModalDialog>




</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

