<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="InspectLedgers.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.InspectLedgers" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <script type="text/javascript">
    	$(document).ready(function () {

	        $('#gridLedgers').datagrid(
	        {
	            onLoadSuccess: function () {
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
	                        index: rowCount-1,
	                        field: 'description',
	                        colspan: 3
	                    });
	                    //$('#gridOutstandingAccounts').dataGrid('mergeCells', {index: rowCount, field:'id', colspan: 5});
	                    // Footer cells could not be merged in datagrid :(

	                }

	                // Enable various actions on icon

	                $('img.LocalIconFlag').click(function () {
	                    transactionId = $(this).attr("txId");
	                    onFlagTransaction("Add Tx Id here");
	                });
	                $('img.LocalIconInspect').click(function () {
	                    transactionId = $(this).attr("txId");
	                    onInspectTransaction(transactionId);
	                });
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
    	            $('#tableAccountPlan').treegrid('reload');
    	            transactionDirty = false;
    	        }
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

        function reloadData()
        {
            var selectedYear = $('#<%=DropYears.ClientID %>').val();
            var selectedMonth = $('#<%=DropMonths.ClientID %>').val();

            $('#gridLedgers').datagrid({ url: 'Json-InspectLedgerData.aspx?Year=' + selectedYear + "&Month=" + selectedMonth + "&AccountId=" + accountId});

        	$('#imageLoadIndicator').show();
	        $('div.datagrid').css('opacity', 0.4);

	        $('#gridOutstandingAccounts').datagrid('reload');
        }

        function onInspectTransaction(transactionId) {
            window.scrollTo(0, 0);
            $('body').css('overflow-y', 'hidden');
            $('#divModalCover').fadeIn();

            alertify.log ('<asp:Label ID="LabelInspectNotAvailable" runat="server" />');
        }

        function onFlagTransaction(transactionId) {
            alertify.log('<asp:Label ID="LabelFlagNotAvailable" runat="server" />');

        }

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
            <div class="content">
                <div class="divIconCloseModal"><img id="IconCloseEdit" src="/Images/Icons/iconshock-cross-16px.png" /></div><h2><asp:Literal ID="LiteralEditHeader" runat="server"/></h2>
                <table id="Table1" class="easyui-datagrid" style="width:100%;height:300px"
                data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-InspectLedgerData.aspx'"
                idField="id">
                <thead>  
                    <tr>  
                        <th data-options="field:'id',width:50,align:'right'"><asp:Label ID="Label1" runat="server" Text="ID#"/></th>  
                        <th data-options="field:'datetime',width:90,sortable:true"><asp:Label ID="Label2" runat="server" Text="XYZ DateTime" /></th>
                        <th data-options="field:'description',width:270"><asp:Label ID="Label3" runat="server" Text="XYZ Description" /></th>  
                        <th data-options="field:'deltaPos',width:70,align:'right'"><asp:Label ID="Label4" runat="server" Text="XYZ Debit" /></th>
                        <th data-options="field:'deltaNeg',width:70,align:'right'"><asp:Label ID="Label5" runat="server" Text="XYZ Credit" /></th>
                        <th data-options="field:'balance',width:80,align:'right'"><asp:Label ID="Label6" runat="server" Text="XYZ Balance" /></th>
                    </tr>  
                </thead>
            </table>  
                <div id="DivModalFields" class="entryFields">
                    ModalFields
                </div>
                <div class="entryLabels">
                    entryLabels
                </div>
            </div>
        </div>
    </div>



</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

