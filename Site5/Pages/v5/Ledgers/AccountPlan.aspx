<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccountPlan.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.AccountPlan" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />

	<script type="text/javascript">

	    $(document).ready(function () {
        
	        $('#tableAccountPlan').treegrid(
	        {
	            onBeforeExpand: function (foo) {
	                $('span.accountplandata-collapsed-' + foo.id).fadeOut('fast', function () {
	                    $('span.accountplandata-expanded-' + foo.id).fadeIn('slow');
	                });
	            },

	            onBeforeCollapse: function (foo) {
	                $('span.accountplandata-expanded-' + foo.id).fadeOut('fast', function () {
	                    $('span.accountplandata-collapsed-' + foo.id).fadeIn('slow');
	                });
	            },
	            
	            onLoadSuccess: function () {
	                $('div.datagrid').css('opacity', 1);
	                
	                $(".IconEdit").click(function() {
	                    $('#divModalCover').fadeIn();
	                });
	        
	            }
	        });

	        $('div.datagrid').css('opacity', 0.4);
	        
	        $("#IconCloseEdit").click(function() {
	            $('#divModalCover').fadeOut();
	        });

	    });

        function modalShow() {
            $('#divModalCover').fadeIn();
        }

	    var currentYear = <%=DateTime.Today.Year %>;

	</script>
    <style>
	    .IconEdit {
		    cursor: pointer;
	    }
	    #IconCloseEdit {
		    cursor: pointer;
		    
	    }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="LabelContentHeader" runat="server" /></h2>
    <table id="tableAccountPlan" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-AccountPlanData.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="accountName">
        <thead>  
            <tr>  
                <th field="accountName" width="240"><asp:Literal ID="LiteralHeaderAccountName" runat="server"/>Account Name</th>  
                <th field="owner" width="160" align="left">Owner</th>  
                <th field="balance" width="80" align="right">Balance</th>
                <th field="budget" width="80" align="right">Budget</th>
                <th field="class" width="65" align="center">Flags</th>
                <th field="action" width="40" align="center">Edit</th>  
            </tr>  
        </thead>  
    </table>


    <div id="divModalCover" class="modalcover">
        <div class="box modal">
            <div class="content">
                <div style="float:right;margin-top: 2px;margin-right: -5px"><img id="IconCloseEdit" src="/Images/Icons/iconshock-cross-16px.png" /></div><h2>Editing account (Under Construction/Test)</h2>
            </div>
        </div>
    </div>

    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    
</asp:Content>

