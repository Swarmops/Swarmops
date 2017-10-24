<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="ViewVatReports.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.ViewVatReports" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableVatReport').treegrid(
	        {
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();
              
	            }
	        });

	        $('#<%=DropReports.ClientID %>').change(function () {
	            var selectedReportId = $('#<%=DropReports.ClientID %>').val();

	            $('#tableVatReport').treegrid({ url: 'Json-VatReportData.aspx?ReportId=' + selectedReportId });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            //$('#tableAnnualReport').treegrid('reload');
	        });


	        $('div.datagrid').css('opacity', 0.4);
	    });

	    function formatGray(val, row, index) {
	        if (val === undefined) {
	            return undefined;
	        }
	        return "<span style='color:#AAA'>" + val + "</span>";
	    }

	    var currentYear = <%=DateTime.Today.Year %>;

	</script>
    <style>
	    .content h2 select {
		    font-size: 16px;
            font-weight: bold;
            color: #1C397E;
            letter-spacing: 1px;
	    }
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
	    }
   	    table.datagrid-ftable {
		    font-weight: 500;
	    }

    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <h2><div class="elementFloatFar"><%= CurrentOrganization.Currency.DisplayCode %></div><asp:Label ID="LabelContentHeader" runat="server" />&nbsp;<asp:DropDownList runat="server" ID="DropReports"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>
    <table id="tableVatReport" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-VatReportData.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true" showFooter="true" 
        idField="id" treeField="txid">
        <thead> 
            <tr>
                <th field="txid" width="40"><asp:Literal ID="LiteralHeaderTransactionId" Text="Tx#" runat="server"/></th>  
                <th field="datetime" width="100"><asp:Literal ID="LiteralHeaderDateTime" runat="server" /></th>  
                <th field="description" width="280"><asp:Literal ID="LiteralHeaderDescription" runat="server" /></th>  
                <th field="turnover" width="140" align="right"><asp:Literal ID="LiteralHeaderTurnover" runat="server" /></th>
                <th field="vatoutbound" width="110" align="right"><asp:Literal ID="LiteralHeaderVatOutbound" runat="server" /></th>
                <th field="vatinbound" width="100" align="right"><asp:Literal ID="LiteralHeaderVatInbound" runat="server" /></th>  
                <th field="dox" width="40" align="center"><asp:Literal ID="LiteralHeaderDox" runat="server" /></th>
            </tr>  
        </thead>  
    </table> 
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarDownload" Text="Download This" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" id="linkDownloadReport" onclick="document.location='placeholder';" >
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-downarrow-16px.png');background-position:-1px -1px"></div>
                UNDER CONSTRUCTION
            </div>
        </div>
    </div>


</asp:Content>

    
