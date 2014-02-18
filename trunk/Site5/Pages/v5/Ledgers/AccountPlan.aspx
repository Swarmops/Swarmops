<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccountPlan.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.AccountPlan" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />

	<script type="text/javascript">

	    $(document).ready(function () {
        
	        $('#tableAccountPlan').treegrid(
	        {
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	            }
	        });

	        $('div.datagrid').css('opacity', 0.4);
	    });

	    var currentYear = <%=DateTime.Today.Year %>;

	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="LabelContentHeader" runat="server" /></h2>
    <table id="tableAccountPlan" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-AccountPlanData.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="false"
        idField="id" treeField="accountName">
    <thead>  
        <tr>  
            <th field="accountName" width="240"><asp:Literal ID="LiteralHeaderAccountName" runat="server"/>Account Name</th>  
            <th field="budget" width="160" align="left">Owner</th>  
            <th field="balance" width="80" align="right">Balance</th>
            <th field="budget" width="80" align="right">Budget</th>
            <th field="class" width="80" align="center">Flags</th>
            <th field="action" width="40" align="center">Edit</th>  
        </tr>  
    </thead>  
</table> 
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    
</asp:Content>

