<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Ledgers.BalanceSheet" Codebehind="BalanceSheet.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableProfitLoss').treegrid(
	        {
	            onBeforeExpand: function (foo) {
	                $('span.annualreportdata-collapsed-' + foo.id).fadeOut('fast', function () {
	                    $('span.annualreportdata-expanded-' + foo.id).fadeIn('slow');
	                });
	            },

	            onBeforeCollapse: function (foo) {
	                $('span.annualreportdata-expanded-' + foo.id).fadeOut('fast', function () {
	                    $('span.annualreportdata-collapsed-' + foo.id).fadeIn('slow');
	                });
	            },

	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();

	                var selectedYear = $('#<%=DropYears.ClientID %>').val();

	                $('div#linkDownloadReport').attr("onclick", "document.location='Csv-BalanceData.aspx?Year=" + selectedYear + "';");
	                $('#spanDownloadText').text(SwarmopsJS.unescape('<%= this.Localized_DownloadFileName %>') + selectedYear + "-<%=DateTime.Today.ToString("yyyyMMdd") %>.csv");
                    $('#headerStartYear').text(SwarmopsJS.unescape('<%= this.Localized_StartYear %>'.replace('XXXX',selectedYear)));
              
                    if (selectedYear == currentYear) 
                    {
                        $('span.previousYearsHeader').hide();
                        $('span.currentYearHeader').show();
                    }
                    else
                    {
                        $('#previousYtd').text(SwarmopsJS.unescape('<%= this.Localized_EndYear %>'.replace('XXXX', selectedYear)));

                        $('span.currentYearHeader').hide();
                        $('span.previousYearsHeader').show();
                    }
	                
                    $('span.commonHeader').show();
	            }
	        });

	        $('#<%=DropYears.ClientID %>').change(function () {
	            var selectedYear = $('#<%=DropYears.ClientID %>').val();

	            $('#tableProfitLoss').treegrid({ url: 'Json-BalanceSheetData.aspx?Year=' + selectedYear });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            //$('#tableProfitLoss').treegrid('reload');
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
    
    <h2><div class="float-far"><%= CurrentOrganization.Currency.DisplayCode %></div><asp:Label ID="LabelContentHeader" runat="server" /> <asp:DropDownList runat="server" ID="DropYears"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>
    <table id="tableProfitLoss" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-BalanceSheetData.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="name">
        <thead>  
            <tr>  
                <th field="name" width="178"><asp:Literal ID="LiteralHeaderAccountName" runat="server"/></th>  
                <th field="lastYear" width="80" align="right"><span class="commonHeader" id="headerStartYear" style="display:none"></span><span class="loadingHeader">&mdash;</span></th>  
                <th field="q1" width="80" align="right"><span class="commonHeader" id="headerQ1" style="display:none"><asp:Literal ID="LiteralHeaderQ1" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
                <th field="q2" width="80" align="right"><span class="commonHeader" id="headerQ2" style="display:none"><asp:Literal ID="LiteralHeaderQ2" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
                <th field="q3" width="80" align="right"><span class="commonHeader" id="headerQ3" style="display:none"><asp:Literal ID="LiteralHeaderQ3" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>  
                <th field="q4" width="80" align="right"><span class="commonHeader" id="headerQ4" style="display:none"><asp:Literal ID="LiteralHeaderQ4" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
                <th field="ytd" width="80" align="right"><span class="previousYearsHeader" id="previousYtd" style="display:none"></span><span class="currentYearHeader" style="display:none"><asp:Literal ID="LiteralHeaderYtd" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
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

