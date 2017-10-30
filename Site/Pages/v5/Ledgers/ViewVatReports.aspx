<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="ViewVatReports.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.ViewVatReports" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />    

	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableVatReport').datagrid(
	        {
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();
              
	                $(".LocalViewDox").click(function () {
	                    $("a.FancyBox_Gallery[rel='" + $(this).attr("baseid") + "']").first().click();
	                });

	            }
	        });

	        $('#<%=DropReports.ClientID %>').change(function () {
	            var selectedReportId = $('#<%=DropReports.ClientID %>').val();
	            console.log("Selected Report Id: " + selectedReportId);

	            $('#tableVatReport').datagrid({ url: 'Json-VatReportData.aspx?ReportId=' + selectedReportId });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            //$('#tableAnnualReport').treegrid('reload');
	        });

            
	        $("a.FancyBox_Gallery").fancybox({
	            'overlayShow': true,
	            'transitionIn': 'fade',
	            'transitionOut': 'fade',
	            'type': 'image',
	            'opacity': true
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
   	        padding-top: 2px;
	    }

    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <h2><div class="elementFloatFar"><%= CurrentOrganization.Currency.DisplayCode %></div><asp:Label ID="LabelContentHeader" runat="server" />&nbsp;<asp:DropDownList runat="server" ID="DropReports"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>
    <table id="tableVatReport" title="" class="easyui-treegrid" style="width:680px"  
        url="Json-VatReportData.aspx?ReportId=<%=this.InitialReportId %>"
        rownumbers="false"
        animate="true"
        fitColumns="true" showFooter="true" 
        idField="id">
        <thead> 
            <tr>
                <th field="txid" width="60"><asp:Literal ID="LiteralHeaderTransactionId" Text="Tx#" runat="server"/></th>  
                <th field="datetime" width="80"><asp:Literal ID="LiteralHeaderDateTime" runat="server" /></th>  
                <th field="description" width="280"><asp:Literal ID="LiteralHeaderDescription" runat="server" /></th>  
                <th field="turnover" width="140" align="right"><asp:Literal ID="LiteralHeaderTurnover" runat="server" /></th>
                <th field="outbound" width="110" align="right"><asp:Literal ID="LiteralHeaderVatOutbound" runat="server" /></th>
                <th field="inbound" width="100" align="right"><asp:Literal ID="LiteralHeaderVatInbound" runat="server" /></th>  
                <th field="dox" width="60" align="center"><asp:Literal ID="LiteralHeaderDox" runat="server" /></th>
            </tr>  
        </thead>  
    </table> 
    
    <div style="display:none">
    <!-- a href links for FancyBox to trigger on -->
    
    <asp:Repeater runat="server" ID="RepeaterLightboxItems">
        <ItemTemplate>
            <a href="/Pages/v5/Support/StreamUpload.aspx?DocId=<%# Eval("DocId") %>&hq=1&VatReportKey=<%=VatReportKey %>" title="<%# Eval("Title") %>" class="FancyBox_Gallery" rel="<%# Eval("BaseId") %>">&nbsp;</a>
        </ItemTemplate>
    </asp:Repeater>

    </div>

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

    
