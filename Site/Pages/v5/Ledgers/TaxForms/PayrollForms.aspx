<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.TaxForms.PayrollForms" Codebehind="PayrollForms.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.snipe.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />

	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableTaxData').treegrid(
	        {
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();

	                var selectedCountry = $('#<%=DropCountries.ClientID %>').val();

	                $('span.commonHeader').show();

	                $('img.IconFormMonth').click(function() {
	                    var yearMonth = $(this).attr("yearmonth");
	                    $('a#LinkFancyBox').attr("href", "/Plugins-Stock/Taxes/" + selectedCountry + "/MonthlyTaxForm-" + selectedCountry + ".aspx?YearMonth=" + yearMonth + "&Country=" + selectedCountry);
	                    $('a#LinkFancyBox').click();
	                });
	            }
	        });

	        $("a.FancyBox_Gallery").fancybox({
	            'overlayShow': true,
	            'transitionIn': 'fade',
	            'transitionOut': 'fade',
	            'type': 'image',
	            'opacity': true
	        });

	        $('#<%=DropCountries.ClientID %>').change(function () {
	            var selectedCountry = $('#<%=DropCountries.ClientID %>').val();

	            $('#tableProfitLoss').treegrid({ url: 'Json-Payroll-TaxFormData.aspx?Country=' + selectedCountry });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            $('#tableTaxData').treegid('reload');
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
        .IconFormYear, .IconFormMonth {
            cursor: pointer;
        }
        .IconFormYear {
            display: none;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <h2><asp:Label ID="LabelContentHeader" runat="server" /> <asp:DropDownList runat="server" ID="DropCountries"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>
    <table id="tableTaxData" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-Payroll-TaxFormData.aspx?Country=SE"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="yearMonth">
        <thead>  
            <tr>  
                <th field="yearMonth" width="168"><asp:Literal ID="LiteralTimePeriod" runat="server"/></th>  
                <th field="grossPay" width="90" align="right"><asp:Literal runat="server" ID="LiteralGrossPay" /></th>  
                <th field="additiveTax" width="90" align="right"><asp:Literal ID="LiteralAdditiveTax" runat="server" /></th>
                <th field="costTotal" width="90" align="right"><asp:Literal ID="LiteralCostTotal" runat="server" /></th>
                <th field="deductedTax" width="90" align="right"><asp:Literal ID="LiteralDeductedTax" runat="server" /></th>
                <th field="taxTotal" width="90" align="right"><asp:Literal ID="LiteralTaxTotal" runat="server" /></th>
                <th field="forms" width="40" align="center"><asp:Literal ID="LiteralFormsIcons" runat="server" /></th>
            </tr>  
        </thead>  
    </table> 
    
    <!-- a href link for FancyBox to trigger on -->
    
    <div style="display:none">
        <a href="HiddenLink" id="LinkFancyBox" rel="FancyBox" class="FancyBox_Gallery"></a>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">

</asp:Content>

