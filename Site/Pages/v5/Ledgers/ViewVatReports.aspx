<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="ViewVatReports.aspx.cs" CodeFile="ViewVatReports.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.ViewVatReports" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts Package="FancyBox" runat="server" ID="ScriptFancyBox"/>
    <Swarmops5:DocumentDownloader ID="DownloadSupport" runat="server"/>

	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableVatReport').treegrid(
	        {
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                $('#imageLoadIndicator').hide();
	                $('span.loadingHeader').hide();
              
	                $(".LocalViewDox").attr('src', '/Images/Icons/iconshock-balloon-examine-128x96px.png');
	                $(".LocalDownloadDox").attr('src', '/Images/Icons/iconshock-balloon-download-128x96px.png');

	                $(".LocalViewDox").click(function () {
	                    $("a.FancyBox_Gallery[data-fancybox='" + $(this).attr("data-txid") + "']").first().click();
	                });

	                $(".LocalDownloadDox").click(function() {
	                    downloadDocument($(this).attr("data-docid"), $(this).attr("data-docname"));
	                });

	            }
	        });

	        $('#<%=DropReports.ClientID %>').change(function () {
	            var selectedReportId = $('#<%=DropReports.ClientID %>').val();
	            console.log("Selected Report Id: " + selectedReportId);

	            $('#tableVatReport').treegrid({ url: 'Json-VatReportData.aspx?ReportId=' + selectedReportId });
        	    $('#imageLoadIndicator').show();
	            $('div.datagrid').css('opacity', 0.4);

	            //$('#tableAnnualReport').treegrid('reload');
	        });
            
	        SwarmopsJS.fancyBoxInit("a.FancyBox_Gallery");

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
    
    <asp:Panel runat="server" ID="PanelShowVatReports">
    <h2><div class="elementFloatFar"><%= CurrentOrganization.Currency.DisplayCode %></div><asp:Label ID="LabelContentHeader" runat="server" />&nbsp;<asp:DropDownList runat="server" ID="DropReports"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>
    <table id="tableVatReport" title="" class="easyui-treegrid" style="width:680px"  
        url="Json-VatReportData.aspx?ReportId=<%=this.InitialReportId %>"
        rownumbers="false"
        animate="true"
        fitColumns="true" showFooter="true" 
        idField="id">
        <thead> 
            <tr>
                <th field="txid" width="50"><asp:Literal ID="LiteralHeaderTransactionId" Text="Tx#" runat="server"/></th>  
                <th field="datetime" width="55"><asp:Literal ID="LiteralHeaderDateTime" runat="server" /></th>  
                <th field="description" width="205"><asp:Literal ID="LiteralHeaderDescription" runat="server" /></th>  
                <th field="turnover" width="100" align="right"><asp:Literal ID="LiteralHeaderTurnover" runat="server" /></th>
                <th field="outbound" width="100" align="right"><asp:Literal ID="LiteralHeaderVatOutbound" runat="server" /></th>
                <th field="inbound" width="100" align="right"><asp:Literal ID="LiteralHeaderVatInbound" runat="server" /></th>  
                <th field="dox" width="68" align="center"><asp:Literal ID="LiteralHeaderDox" runat="server" /></th>
            </tr>  
        </thead>  
    </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="PanelShowNoVatReports" Visible="false">
        <h2><asp:Label runat="server" ID="LabelHeaderNoVatReportsToDisplay"/></h2>
        <div style="float: left; margin-right: 10px"><img src="/Images/Icons/iconshock-cross-96px.png"/>
        </div>
        <br/>
        <asp:Label runat="server" ID="LabelNoVatReportsToDisplay"/>
        <div style="clear:both"></div> <!-- fills up the rest of the space of the warning box -->
    </asp:Panel>
    
    <div style="display:none">
    <!-- a href links for FancyBox to trigger on -->
    
    <asp:Repeater runat="server" ID="RepeaterLightboxItems">
        <ItemTemplate>
            <a href="/Pages/v5/Support/StreamUpload.aspx?DocId=<%# Eval("DocId") %>&hq=1&VatReportKey=<%=VatReportKey %>" data-caption="<%# Eval("Title") %>" class="FancyBox_Gallery" data-fancybox="<%# Eval("BaseId") %>">&nbsp;</a>
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

    
