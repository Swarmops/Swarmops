<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Ledgers.BalanceSheetSimplified" CodeFile="BalanceSheetSimplified.aspx.cs" Codebehind="BalanceSheetSimplified.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableAnnualReport').treegrid(
	        {
	            onBeforeExpand: function (foo) {
	                $('span.profitlossdata-collapsed-' + foo.id).fadeOut('fast', function () {
	                    $('span.profitlossdata-expanded-' + foo.id).fadeIn('slow');
	                });
	            },

	            onBeforeCollapse: function (foo) {
	                $('span.profitlossdata-expanded-' + foo.id).fadeOut('fast', function () {
	                    $('span.profitlossdata-collapsed-' + foo.id).fadeIn('slow');
	                });
	            },

	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();

	                var selectedYear = $('#<%=DropYears.ClientID %>').val();

	                $('div#linkDownloadReport').attr("onclick", "document.location='Csv-BalanceData.aspx?Year=" + selectedYear + "';");
	                $('#spanDownloadText').text(SwarmopsJS.unescape('<%= this.Localized_DownloadFileName %>') + selectedYear + "-<%=DateTime.Today.ToString("yyyyMMdd") %>.csv");
                    $('#headerStartYear').text(SwarmopsJS.unescape('<%= this.Localized_Assets %>'.replace('XXXX',selectedYear)));
              
                    if (selectedYear == currentYear) 
                    {
                        $('span.previousYearsHeader').hide();
                        $('span.currentYearHeader').show();
                    }
                    else
                    {
                        $('#previousYtd').text(SwarmopsJS.unescape('<%= this.Localized_Liabilities %>'.replace('XXXX', selectedYear)));

                        $('span.currentYearHeader').hide();
                        $('span.previousYearsHeader').show();
                    }
	                
                    $('span.commonHeader').show();
	            }
	        });

	        $('#<%=DropYears.ClientID %>').change(function () {
	            var selectedYear = $('#<%=DropYears.ClientID %>').val();

	            $('#tableAnnualReport').treegrid({ url: 'Json-BalanceSheetData.aspx?Year=' + selectedYear });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            $('#tableAnnualReport').treegid('reload');
	        });


	        $('div.datagrid').css('opacity', 0.4);
	    });

	    var currentYear = <%=DateTime.Today.Year %>;

	</script>
    <style>
	    .content h2 select {
		    font-size: 16px;
            font-weight: bold;
            color: #1C397E;
            letter-spacing: 1px;
	    }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <h2><div class="elementFloatFar"><%= CurrentOrganization.Currency.DisplayCode %></div><asp:Label ID="LabelContentHeader" runat="server" /> <asp:DropDownList runat="server" ID="DropYears"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>
    <table id="tableAnnualReport" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-BalanceSheetDataSimplified.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="name">
        <thead>  
            <tr>  
                <th field="name" width="178"><asp:Literal ID="LiteralHeaderAccountName" runat="server"/></th>  
                <th field="assets" width="120" align="right"><span class="commonHeader" id="headerStartYear" style="display:none"><asp:Literal ID="LiteralAssets" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>  
                <th field="assetdelta" width="120" align="right"><span class="commonHeader" id="headerQ1" style="display:none"><asp:Literal ID="LiteralAssetsDelta" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
                <th field="liabilities" width="120" align="right"><span class="commonHeader" id="headerQ2" style="display:none"><asp:Literal ID="LiteralLiabilities" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
                <th field="liabilitydelta" width="120" align="right"><span class="commonHeader" id="headerQ3" style="display:none"><asp:Literal ID="LiteralLiabilitiesDelta" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>  
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
                <span id="spanDownloadText">Balancexxxx-yyyymmdd.csv</span>
            </div>
        </div>
    </div>


</asp:Content>

