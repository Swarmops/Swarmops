<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ViewOutstandingAccounts.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.ViewOutstandingAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://hostedscripts.falkvinge.net/easyui/jquery.easyui.min.js" type="text/javascript"></script>
	<link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/icon.css">
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/tree.css"/>
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/datagrid.css"/>
    
    	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#gridOutstandingAccounts').datagrid(
	        {
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();

	                var rowCount = $('#gridOutstandingAccounts').datagrid('getRows').length;
	                if (rowCount > 0) 
	                {
	                    //$('#gridOutstandingAccounts').dataGrid('mergeCells', {index: rowCount, field:'id', colspan: 5});
	                    // Footer cells could not be merged in datagrid :(
	                }
	            }
	        });

	        $('#<%=DropYears.ClientID %>').change(function () {
	            var selectedYear = $('#<%=DropYears.ClientID %>').val();

                if (selectedYear == 'Now') {
    	            $('#gridOutstandingAccounts').datagrid({ url: 'Json-OutstandingAccounts.aspx' });
                } else {
    	            $('#gridOutstandingAccounts').datagrid({ url: 'Json-OutstandingAccounts.aspx?Year=' + selectedYear });
                }

        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            $('#gridOutstandingAccounts').datagrid('reload');
	        });


	        $('div.datagrid').css('opacity', 0.4);
	    });

	    var currentYear = <%=DateTime.Today.Year %>;

	</script>


    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
	    }
	    .content h2 select {
		    font-size: 16px;
            font-weight: bold;
            color: #1C397E;
            letter-spacing: 1px;
        }
   	    table.datagrid-ftable {
		    font-weight: 500;
	    }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelViewOutstandingAccountsHeader" Text="XYZ View Outstanding" />&nbsp;<asp:DropDownList runat="server" ID="DropAccounts"/>&nbsp;<asp:DropDownList runat="server" ID="DropYears"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>

    <table id="gridOutstandingAccounts" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fitColumns:true,fit:false,showFooter:true,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-OutstandingAccounts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'id',width:40"><asp:Label ID="LabelGridHeaderId" runat="server" Text="ID#"/></th>  
                <th data-options="field:'created',width:90,sortable:true"><asp:Label ID="LabelGridHeaderCreatedDate" runat="server" Text="XYZ Created" /></th>
                <th data-options="field:'expected',width:90"><asp:Label ID="LabelGridHeaderExpectedCloseDate" runat="server" Text="XYZ Due" /></th>  
                <th data-options="field:'recipient',width:140,sortable:true"><asp:Label ID="LabelGridHeaderRecipient" runat="server" Text="XYZ Person / Organization" /></th>
                <th data-options="field:'description',width:180,sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>
                <th data-options="field:'amount',width:90,align:'right'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                <th data-options="field:'action',width:43,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZAct" /></th>
            </tr>  
        </thead>
    </table>  

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

