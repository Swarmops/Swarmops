<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccountPlan.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.AccountPlan" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
    <script src="/Scripts/jquery.switchButton.js" language="javascript" type="text/javascript"></script>

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

	                $(".IconEdit").mouseover(function() {
                        $(this).attr("src", "/Images/Icons/iconshock-wrench-16px-hot.png");
	                });

	                $(".IconEdit").mouseout(function() {
                        $(this).attr("src", "/Images/Icons/iconshock-wrench-16px.png");
	                });

	                $(".IconEdit").click(function() {
	                    var accountId = $(this).attr("accountId");
	                    var accountType = accountId.substring(0,1);  // A, D, I, C
	                    accountId = accountId.substring(1);
	                    
                        if (accountType == 'A') {
                            $('#DivEditAssetLabels').show();
                            $('#DivEditAssetControls').show();
                            $('#DivEditProfitLossLabels').hide();
                            $('#DivEditProfitLossControls').hide();
                        } else if (accountType == 'D') {
                            $('#DivEditAssetLabels').hide();
                            $('#DivEditAssetControls').hide();
                            $('#DivEditProfitLossLabels').hide();
                            $('#DivEditProfitLossControls').hide();
                        } else {  // P&L account
                            $('#DivEditAssetLabels').hide();
                            $('#DivEditAssetControls').hide();
                            $('#DivEditProfitLossLabels').show();
                            $('#DivEditProfitLossControls').show();
                        }

	                    window.scrollTo(0, 0);
	                    $('#divModalCover').fadeIn();
	                    
                        $.ajax({
                            type: "POST",
                            url: "AccountPlan.aspx/GetAccountData",
                            data: "{'accountId': '" + escape(accountId) + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (msg) {
                                console.log(msg.d);

                                $('#CheckAccountActive').switchButton( {checked: msg.d.Open });
                                $('#CheckAccountExpensable').switchButton( {checked: msg.d.Expensable });
                                $('#CheckAccountAdministrative').switchButton( {checked: msg.d.Administrative });
                                $('#TextAccountName').val(msg.d.AccountName);
                                $('#TextAccountBudget').val(msg.d.Budget);
                                
                                // TODO: Owner, parent account, automation
                            }
                        });

	                });
	        
	            }
	        });

	        $('div.datagrid').css('opacity', 0.4);
	        
	        $("#IconCloseEdit").click(function() {
	            $('#divModalCover').fadeOut();
	        });

	        $('.EditCheck').switchButton(
	            {
	                height:16,
                    width:30,
	                button_width:16,
	                on_label: "<% =Resources.Global.Global_Yes.ToUpperInvariant() %>",
	                off_label: "<% =Resources.Global.Global_No.ToUpperInvariant() %>"
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
	    .checkboxSpacer {
		    float: left;width: 100px;height: 20px;
	    }
    </style>

    <link rel="stylesheet" type="text/css" href="/Style/v5-easyui-elements.css">
    <link rel="stylesheet" type="text/css" href="/Style/jquery.switchButton.css">

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
                <div class="entryFields"><input type="text" id="TextAccountName" />&nbsp;<br />
                    <Swarmops5:ComboBudgets ID="DropBudgets" runat="server" />&nbsp;<br/>
                    &nbsp;<br/>
                    <div id="DivEditProfitLossControls"><Swarmops5:ComboBudgets ID="DropOwner" runat="server" />&nbsp;<br/>
                    <input type="text" id="TextAccountBudget" style="text-align: right"/>&nbsp;<br/>
                    &nbsp;<br/>
                    <div class="checkboxSpacer"></div><input type="checkbox" class="EditCheck" id="CheckAccountActive"/>
                    <div class="checkboxSpacer"></div><input type="checkbox" class="EditCheck" id="CheckAccountExpensable"/>
                    <div class="checkboxSpacer"></div><input type="checkbox" class="EditCheck" id="CheckAccountAdministrative"/>
                    &nbsp;<br/></div>
                    <div id="DivEditAssetControls"><asp:DropDownList runat="server" ID="DropAccountUploadFormats"/>&nbsp;<br/>
                    <input type="text" id="TextAutomationPaymentTag" readonly="readonly"/>&nbsp;<br/></div>
                </div>
                <div class="entryLabels">Account name<br/>
                    Parent account or group<br/>
                    <div id="DivEditProfitLossLabels"><h2>Daily operations</h2>
                    Owner<br/>
                    Budget (balance is SEK foo)<br/>
                    <h2>Switches</h2>
                    Active<br/>
                    Expensable<br/>
                    Administrative<br/></div>
                    <div id="DivEditAssetLabels"><h2>Automation</h2>
                    File upload profile<br/>
                    <span id="SpanUploadParameterName">Upload parameter, if any</span></div>
                </div>
            </div>
        </div>
    </div>

    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    
</asp:Content>

