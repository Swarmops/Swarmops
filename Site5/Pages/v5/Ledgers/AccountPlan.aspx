<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccountPlan.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.AccountPlan" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboPeople" Src="~/Controls/v5/Swarm/ComboPeople.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
    <script src="/Scripts/jquery.switchButton.js" language="javascript" type="text/javascript"></script>
    <link rel="stylesheet" href="/Style/jquery.switchButton.css" type="text/css" />

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
	                    accountId = $(this).attr("accountId");
	                    accountType = accountId.substring(0,1);  // A, D, I, C
	                    accountId = accountId.substring(1);

	                    var accountTree = $('#<%=DropParents.ClientID %>_DropBudgets');
	                    accountTreeLoaded = false;
	                    accountTree.combotree('setText', '');

	                    
                        if (accountType == 'A') {
                            $('#DivEditAssetLabels').show();
                            $('#DivEditAssetControls').show();
                            $('#DivEditProfitLossLabels').hide();
                            $('#DivEditProfitLossControls').hide();
                            accountTree.combotree('reload', '/Automation/Json-FinancialAccountsTree.aspx?AccountType=Asset&ExcludeId=' + accountId);
                        } else if (accountType == 'D') { // Debt, aka Liability
                            $('#DivEditAssetLabels').hide();
                            $('#DivEditAssetControls').hide();
                            $('#DivEditProfitLossLabels').hide();
                            $('#DivEditProfitLossControls').hide();
                            accountTree.combotree('reload', '/Automation/Json-FinancialAccountsTree.aspx?AccountType=Debt&ExcludeId=' + accountId);
                        } else {  // P&L account
                            $('#DivEditAssetLabels').hide();
                            $('#DivEditAssetControls').hide();
                            $('#DivEditProfitLossLabels').show();
                            $('#DivEditProfitLossControls').show();
                            
                            if (accountType == 'I') {
                                accountTree.combotree('reload', '/Automation/Json-FinancialAccountsTree.aspx?AccountType=Income&ExcludeId=' + accountId);
                            } else { // C for Costs aka Expenses
                                accountTree.combotree('reload', '/Automation/Json-FinancialAccountsTree.aspx?AccountType=Cost&ExcludeId=' + accountId);
                            }
                        }

	                    // Clear out data in modal form, gray out to indicate being loaded

                        $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#DDD');
                        $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#DDD');
	                    $('#TextAccountBudget').val('...').css('background-color', '#DDD');
	                    $('#TextAccountName').val('...').css('background-color', '#DDD');
	                    $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').attr('placeholder', '...');
	                    accountTree.combotree('setText', '...');
	                    parentAccountName = '';

	                    window.scrollTo(0, 0);
	                    $('body').css('overflow-y', 'hidden');
	                    $('#divModalCover').fadeIn();

	                    setTimeout(function() {
	                        $('#divModalBox').animate({ "height": ($('#DivModalFields').outerHeight() + $('#HeaderModal').outerHeight()) + 40 + "px" }, 50);
	                    }, 50); // set timeout to after checkboxes initialized, if this is the first show of modal

	                    if (!checkboxesInitialized && (accountType == 'I' || accountType == 'C')) {
	                        // This is a weird construct, but comes from the switchbuttons needing to be visible when initialized.
                            // Hence, it can't be done in document.Ready but need to be done on first show.
	                        setTimeout(function() {
	                            $('.EditCheck')
	                                .switchbutton({
	                                    checkedLabel: '<%=Resources.Global.Global_On.ToUpperInvariant() %>',
	                                    uncheckedLabel: '<%=Resources.Global.Global_Off.ToUpperInvariant() %>',
	                                })
	                                .change(function() {
	                                    if (!suppressSwitchChangeAction) {
	                                        $(this).parent().css('box-shadow', '0 0 1px 0 #FFFFC0');
	                                        var callParameters = "{'accountId': '" + escape(accountId) + "', 'switchName':'" + $(this).attr("rel") + "','switchValue':'" + $(this).prop('checked') + "'}";

	                                        // If changing Active, set Expensable.Enabled to Active.Checked. If false, set Expensable.Checked to false, too

	                                        if ($(this).attr("rel") == "Active") {
	                                            if (!$(this).prop('checked') && $("#CheckAccountExpensable").prop("checked")) {
	                                                $("#CheckAccountExpensable").prop("checked", false).change();
	                                            }
	                                        }

                                            // If changing Expensable to true, make sure that Active is true, too

	                                        if ($(this).attr("rel") == "Expensable") {
	                                            if ($(this).prop('checked') && !$("#CheckAccountActive").prop("checked")) {
	                                                $("#CheckAccountActive").prop("checked", true).change();
	                                            }
	                                        }

	                                        $.ajax({
	                                            type: "POST",
	                                            url: "AccountPlan.aspx/SetAccountSwitch",
	                                            data: callParameters,
	                                            contentType: "application/json; charset=utf-8",
	                                            dataType: "json",
	                                            success: $.proxy(function(msg) {
	                                                if (msg.d) { // saved ok
	                                                    $(this).parent().css('box-shadow', '0 0 1px 2px rgba(96,255,96,0.5)');
	                                                    $(this).parent().animate({
	                                                        boxShadow: '0 0 10px 2px rgba(0,255,0,0)'
	                                                    }, 250);
	                                                    accountDirty = true;
	                                                } else {
	                                                    suppressSwitchChangeAction = true;
	                                                    $(this).click();
	                                                    suppressSwitchChangeAction = false;
	                                                    $(this).parent().css('box-shadow', '0 0 1px 2px rgba(255,96,96,0.8)');
	                                                    $(this).parent().animate({
	                                                        boxShadow: '0 0 15px 3px rgba(0,255,0,0)'
	                                                    }, 750);
	                                                    alertify.error("The server refused setting the switch as requested.");  // TODO: Localize
	                                                }
	                                                $('#TextAccountName').animate({ backgroundColor: "#FFFFFF" }, 250);
	                                            }, this),
	                                            error: $.proxy(function() {
	                                                alertify.error("There was an error calling the server to set the switch. Is the server reachable?"); // TODO: Localize
	                                                suppressSwitchChangeAction = true;
	                                                $(this).click();
	                                                suppressSwitchChangeAction = false;
	                                                $(this).parent().css('box-shadow', '0 0 1px 2px rgba(255,96,96,0.8)');
	                                                $(this).parent().animate({
	                                                    boxShadow: '0 0 15px 3px rgba(0,255,0,0)'
	                                                }, 750);
	                                            }, this)
	                                        });
	                                    }
	                               });
	                        }, 1);
	                        checkboxesInitialized = true;
	                    }

	                    if (!parentDropInitialized) {
	                        parentDropInitialized = true;
	                        // Another weird construct for same reason. Show and hide the box once, and it'll init properly next.
	                        setTimeout(function() {
	                            $('#<%=this.DropParents.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
	                            $('#<%=this.DropParents.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
	                        }, 100);
                        }

	                    $.ajax({
                            type: "POST",
                            url: "AccountPlan.aspx/GetAccountData",
                            data: "{'accountId': '" + escape(accountId) + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (msg) {

                                suppressSwitchChangeAction = true;
                                $("#CheckAccountActive").prop("checked", msg.d.Active).change();
                                $("#CheckAccountExpensable").prop("checked", msg.d.Expensable).change();
                                $("#CheckAccountAdministrative").prop("checked", msg.d.Administrative).change();
                                suppressSwitchChangeAction = false;

                                modalAccountName = msg.d.AccountName;
                                $('#TextAccountName').val(msg.d.AccountName).css('background-color', '#FFF');

                                modalAccountBudget = msg.d.Budget;
                                $('#TextAccountBudget').val(msg.d.Budget).css('background-color', '#FFF');

                                $('#SpanTextCurrency').text(msg.d.CurrencyCode);
                                $('#SpanEditBalance').text(msg.d.Balance);
                                parentAccountName = msg.d.ParentAccountName;
                                setAccountTreeText(parentAccountName);

                                $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#FFF');
                                $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-image', "url('" + msg.d.AccountOwnerAvatarUrl + "')");
                                $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').attr('placeholder', msg.d.AccountOwnerName);
                                $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').val('');
                            }
                        });

	                });
	        
	            }
	        });

            $('#<%=this.DropOwner.ClientID %>_SpanPeople span.combo input.combo-text').keydown(function (e) {
                // Clear the owner avatar on any keypress
                $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-image', "none");
            });


	        $('div.datagrid').css('opacity', 0.4);
	        
	        $("#IconCloseEdit").click(function() {
	            $('#divModalCover').fadeOut();

	            if (accountDirty) {
	                $('#tableAccountPlan').treegrid('reload');
	                accountDirty = false;
	            }
	        });

	        $('#TextAccountName').blur(function() {

	            var newAccountName = $('#TextAccountName').val().trim();
	            if (modalAccountName == newAccountName) {
	                return; // nothing changed, nothing to do
	            }

	            if (newAccountName == "") { // no, we're not changing to an empty name
	                $('#TextAccountName').css('background-color', '#FFA0A0');
	                $('#TextAccountName').val(modalAccountName);
	                $('#TextAccountName').animate({ backgroundColor: "#FFFFFF" }, 250);
	                return;
	            }

	            $('#TextAccountName').css('background-color', '#FFFFE0');
	            $.ajax({
	                type: "POST",
	                url: "AccountPlan.aspx/SetAccountName",
	                data: "{'accountId': '" + escape(accountId) + "', 'name':'" + escape(newAccountName) + "'}",
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function(msg) { // TODO: This needs three return values - changed, broken and changed, or not changed
	                    if (msg.d) { // saved
	                        $('#TextAccountName').css('background-color', '#E0FFE0');
	                        modalAccountName = $('#TextAccountName').val(); // race condition because async. Matters?
	                        accountDirty = true;
	                    } else {
	                        $('#TextAccountName').css('background-color', '#FFA0A0');
	                        $('#TextAccountName').val(modalAccountName);
	                        alertify.error("Cannot change accounts that have transactions in closed ledgers - yet.");  // TODO: Localize
	                    }
	                    $('#TextAccountName').animate({ backgroundColor: "#FFFFFF" }, 250);
	                },
	                error: function() {
	                    alertify.error("There was an error calling the server to set the account name. Is the server reachable?"); // TODO: Localize
	                    $('#TextAccountName').val(modalAccountName);
	                    $('#TextAccountName').css('background-color', '#FFA0A0');
	                    $('#TextAccountName').animate({ backgroundColor: "#FFFFFF" }, 250);
	                }
	            });
	        });

	        $('#TextAccountBudget').blur(function() {
	            var newAccountBudget = $('#TextAccountBudget').val();
	            if (modalAccountBudget == newAccountBudget) {
	                return; // nothing changed, nothing to do
	            }

	            if (accountType == 'C' && newAccountBudget[0] != '-') {
	                alertify.alert('<asp:Literal ID="LiteralExpensesBudgetsAreNegative" runat="server" />');
                    newAccountBudget = "-" + newAccountBudget;
	            }

	            $('#TextAccountBudget').css('background-color', '#FFFFE0');
	            $.ajax({
	                type: "POST",
	                url: "AccountPlan.aspx/SetAccountBudget",
	                data: "{'accountId': '" + escape(accountId) + "', 'budget':'" + escape(newAccountBudget) + "'}",
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function(msg) { 
	                    if (msg.d.Result == 3 || msg.d.Result == 4) { // Invalid or NoPermission
	                        $('#TextAccountBudget').css('background-color', '#FFA0A0');
	                        alertify.error("There was an error attempting to set your proposed budget."); // TODO: Localize
	                    } else {  // msg.d.Result should be Changed here
	                        $('#TextAccountBudget').css('background-color', '#E0FFE0');
	                        modalAccountBudget = msg.d.NewData;
	                        accountDirty = true;
	                    }
	                    $('#TextAccountBudget').val(modalAccountBudget);
	                    $('#TextAccountBudget').animate({ backgroundColor: "#FFFFFF" }, 250);
	                },
	                error: function() {
	                    alertify.error("There was an error calling the server to set your proposed budget. Is the server reachable?"); // TODO: Localize
	                    $('#TextAccountBudget').val(modalAccountBudget);
	                    $('#TextAccountBudget').css('background-color', '#FFA0A0');
	                    $('#TextAccountBudget').animate({ backgroundColor: "#FFFFFF" }, 250);
	                }
	            });
	        });

	    });

        function modalShow() {
            $('#divModalCover').fadeIn();
        }

        function onOwnerChange(personId) {
            $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#FFFFE0');
            $.ajax({
                type: "POST",
                url: "AccountPlan.aspx/SetAccountOwner",
                data: "{'accountId':'" + escape(accountId) + "','newOwnerId':'" + escape(personId) + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    if (msg.d) { // all went ok
                        $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#E0FFE0');
                        accountDirty = true;
                    } else {  
                        $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#FFA0A0');
                        // TODO: Reset combo box
                        alertify.error("There was an error setting the budget owner."); // TODO: Localize
                    }
                    $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').animate({ backgroundColor: "#FFFFFF" }, 250);
                },
                error: function() {
                    alertify.error("There was an error making the call to set budget owner. Is the server available?");
                    $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#FFA0A0');
                    // TODO: Reset combo box
                    $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').animate({ backgroundColor: "#FFFFFF" }, 250);
                }
            });
        }


	    function setAccountTreeText (text) {
	        var accountTree = $('#<%=DropParents.ClientID %>_DropBudgets');
	        var currentText = accountTree.combotree('getText');
	        if (currentText.length < 3) {
	            accountTree.combotree('setText', parentAccountName);
	            $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#FFF');
	        } else if (!accountTreeLoaded) {
	            setTimeout(function() {
	                setAccountTreeText(text);
	            }, 250);
	        }
        }


	    function onAccountTreeSelect(parentAccountId) {
	        if (!accountTreeLoaded) {
	            return;
	        }

	        $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#FFFFE0');
	        $.ajax({
	            type: "POST",
	            url: "AccountPlan.aspx/SetAccountParent",
	            data: "{'accountId':'" + escape(accountId) + "','parentAccountId':'" + escape(parentAccountId) + "'}",
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function(msg) {
	                if (msg.d) { // all went ok
	                    $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#E0FFE0');
                        accountDirty = true;
                    } else {  
	                    $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#FFA0A0');
	                    $('#<%=DropParents.ClientID %>_DropBudgets').combotree('setText', parentAccountName);
                        alertify.error("Cannot reparent accounts with transactions in closed ledgers - yet."); // TODO: Localize
                    }
                    $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').animate({ backgroundColor: "#FFFFFF" }, 250);
                },
                error: function() {
                    alertify.error("There was an error making the call to set budget parent. Is the server available?");
                    $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#FFA0A0');
                    $('#<%=DropParents.ClientID %>_DropBudgets').combotree('setText', parentAccountName);
                    $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').animate({ backgroundColor: "#FFFFFF" }, 250);
                }
            });
        }


	    function onAccountTreeLoaded() {
	        var accountTree = $('#<%=DropParents.ClientID %>_DropBudgets');
	        // accountTree.combotree('select', 0); // clear any previous selection
	        accountTreeLoaded = true;
	        if (parentAccountName.length > 0) {
	            accountTree.combotree('setText', parentAccountName);
	            $('span#<%= DropParents.ClientID %>_SpanBudgets span input.combo-text').css('background-color', '#FFF');
            }
	    }


        var currentYear = <%=DateTime.Today.Year %>;

	    var modalAccountName = "";
	    var modalAccountBudget = "";
	    var accountId = 0;
	    var accountType = '';
	    var accountDirty = false;

	    var checkboxesInitialized = false;
	    var parentDropInitialized = false;
	    var suppressSwitchChangeAction = false;
	    var accountTreeLoaded = false;
	    var parentAccountName = '';

	</script>
    <style>
	    .IconEdit {
		    cursor: pointer;
	    }
	    #IconCloseEdit {
		    cursor: pointer;
		    
	    }
	    .CheckboxContainer {
		    float: right; padding-top: 4px;padding-right: 8px;
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
                <th field="accountName" width="240"><asp:Literal ID="LiteralHeaderAccountName" runat="server"/></th>  
                <th field="owner" width="160" align="left"><asp:Literal ID="LiteralHeaderOwner" runat="server"/></th>  
                <th field="balance" width="80" align="right"><asp:Literal ID="LiteralHeaderBalance" runat="server"/></th>
                <th field="budget" width="80" align="right"><asp:Literal ID="LiteralHeaderBudget" runat="server"/></th>
                <th field="class" width="65" align="center"><asp:Literal ID="LiteralHeaderFlags" runat="server"/></th>
                <th field="action" width="40" align="center"><asp:Literal ID="LiteralHeaderEdit" runat="server"/></th>  
            </tr>  
        </thead>  
    </table>


    <div id="divModalCover" class="modalcover">
        <div id="divModalBox" class="box modal">
            <div class="content">
                <div style="float:right;margin-top: 2px;margin-right: -5px"><img id="IconCloseEdit" src="/Images/Icons/iconshock-cross-16px.png" /></div><h2 id="HeaderModal"><asp:Literal ID="LiteralHeaderEditingAccount" runat="server"/></h2>
                <div id="DivModalFields" class="entryFields"><input type="text" id="TextAccountName" />&nbsp;<br />
                    <Swarmops5:ComboBudgets ID="DropParents" runat="server" OnClientLoaded="onAccountTreeLoaded" OnClientSelect="onAccountTreeSelect" />&nbsp;<br/>
                    &nbsp;<br/>
                    <div id="DivEditProfitLossControls"><Swarmops5:ComboPeople ID="DropOwner" OnClientSelect="onOwnerChange" runat="server" />&nbsp;<br/>
                    <input type="text" id="TextAccountBudget" style="text-align: right"/>&nbsp;<br/>
                    &nbsp;<br/>
                    <label for="CheckAccountActive"><asp:Literal ID="LiteralLabelActiveShort" runat="server"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckAccountActive"/></div><br/>
                    <label for="CheckAccountExpensable"><asp:Literal ID="LiteralLabelExpensableShort" runat="server"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Expensable" class="EditCheck" id="CheckAccountExpensable"/></div><br/>
                    <label for="CheckAccountAdministrative"><asp:Literal ID="LiteralLabelAdministrativeShort" runat="server"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Administrative" class="EditCheck" id="CheckAccountAdministrative"/></div>
                    &nbsp;<br/></div>
                    <div id="DivEditAssetControls"><asp:DropDownList runat="server" ID="DropAccountUploadFormats"/>
                    <input type="text" id="TextAutomationPaymentTag" readonly="readonly"/>&nbsp;<br/></div>
                </div>
                <div class="entryLabels"><asp:Literal ID="LiteralLabelAccountName" runat="server"/><br/>
                    <asp:Literal ID="LiteralLabelParent" runat="server"/><br/>
                    <div id="DivEditProfitLossLabels"><h2><asp:Literal ID="LiteralLabelHeaderDailyOperations" runat="server"/></h2>
                    <asp:Literal ID="LiteralLabelOwner" runat="server"/><br/>
                    <asp:Literal ID="LiteralLabelBudgetBalance" runat="server"/><br/>
                    <h2><asp:Literal ID="LiteralLabelHeaderConfiguration" runat="server"/></h2>
                    <asp:Literal ID="LiteralLabelActiveLong" runat="server"/><br/>
                    <asp:Literal ID="LiteralLabelExpensableLong" runat="server"/><br/>
                    <asp:Literal ID="LiteralLabelAdministrativeLong" runat="server"/><br/></div>
                    <div id="DivEditAssetLabels"><h2><asp:Literal ID="LiteralLabelHeaderAutomation" runat="server"/></h2>
                    <asp:Literal ID="LiteralLabelFileUploadProfile" runat="server"/><br/>
                    <span id="SpanUploadParameterName">Upload parameter, if any</span></div> 
                </div>
            </div>
        </div>
    </div>

    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    
</asp:Content>

