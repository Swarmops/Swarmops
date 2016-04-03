<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="BudgetActual.aspx.cs" Inherits="Pages_v5_Ledgers_BudgetActual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

	<script type="text/javascript">

	    $(document).ready(function () { 

	        $('#tableBudgetActual').treegrid(
	        {
	            onBeforeExpand: function (foo) {
	                $('span.budgetactualdata-collapsed-' + foo.id).fadeOut('fast', function () {
	                    $('span.budgetactualdata-expanded-' + foo.id).fadeIn('slow');
	                });
	            },

	            onBeforeCollapse: function (foo) {
	                $('span.budgetactualdata-expanded-' + foo.id).fadeOut('fast', function () {
	                    $('span.budgetactualdata-collapsed-' + foo.id).fadeIn('slow');
	                });
	            },

	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();
	                $('span.allYearsHeader').show();

	                var selectedYear = $('#<%=DropYears.ClientID %>').val();
	                
                    if (selectedYear == currentYear) 
                    {
                        $('span.previousYearsHeader').hide();
                        $('span.currentYearHeader').show();
                    }
                    else
                    {
                        $('#previousLastActual').text(actualLoc + ' ' + (selectedYear - 1));
                        $('#previousBudget').text(budgetLoc + ' ' + selectedYear);
                        $('#previousYtd').text(actualLoc + ' ' + selectedYear);

                        $('span.currentYearHeader').hide();
                        $('span.previousYearsHeader').show();
                    }
	            }
	        });

	        $('#<%=DropYears.ClientID %>').change(function () {
	            var selectedYear = $('#<%=DropYears.ClientID %>').val();

	            $('#tableBudgetActual').treegrid({ url: 'Json-BudgetActualData.aspx?Year=' + selectedYear });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            $('#tableBudgetActual').treegrid('reload');
	        });


	        $('div.datagrid').css('opacity', 0.4);
	    });

	    var currentYear = <%=DateTime.Today.Year %>;

	    var budgetLoc = '<asp:Literal ID="LiteralBudget" runat="server" />';
	    var actualLoc = '<asp:Literal ID="LiteralActual" runat="server" />';

	</script>
    <style>
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
    <h2><div class="elementFloatFar"><%= CurrentOrganization.Currency.DisplayCode %></div><div class="elementFloatNear"><asp:Label ID="LabelContentHeader" runat="server" /> <asp:DropDownList runat="server" ID="DropYears"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></div>&nbsp;</h2>
    <table id="tableBudgetActual" title="" class="easyui-treegrid" style="width:680px;height:600px"
        url="Json-BudgetActualData.aspx"
        rownumbers="false"
        animate="true" showFooter="true" fitColumns="true"
        idField="id" treeField="name">
    <thead>  
        <tr>  
            <th field="name" width="178"><asp:Literal ID="LiteralHeaderAccountName" runat="server"/></th>  
            <th field="lastYearActual" width="80" align="right"><span class="previousYearsHeader" id="previousLastActual" style="display:none"></span><span class="currentYearHeader" style="display:none"><asp:Literal ID="LiteralHeaderLastYearActual" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>  
            <th field="yearBudget" width="80" align="right"><span class="previousYearsHeader" id="previousBudget" style="display:none"></span><span class="currentYearHeader" style="display:none" id="currentQ1"><asp:Literal ID="LiteralHeaderBudget" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
            <th field="yearActual" width="80" align="right"><span class="previousYearsHeader" id="previousYtd" style="display:none"></span><span class="currentYearHeader" style="display:none" id="currentQ2"><asp:Literal ID="LiteralHeaderActual" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
            <th field="flags" width="40" align="right"><span class="allYearsHeader" style="display:none" id="currentFlags"><asp:Literal ID="LiteralHeaderFlags" runat="server" /></span><span class="loadingHeader">&mdash;</span></th>
        </tr>  
    </thead>  
</table> 
<div style="clear:both"></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarDownload" Text="Download This" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <div class="link-row-encaps" id="linkDownloadReport" onclick="document.location='placeholder';" >
                <div class="link-row-icon" style="background-image:url('/Images/Icons/iconshock-downarrow-16px.png');background-position:-1px -1px"></div>
                <span id="spanDownloadText">ProfitLossxxxx-yyyymmdd.csv</span>
            </div>
        </div>
    </div>

</asp:Content>

