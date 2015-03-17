<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AccountPlan.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.AccountPlan" %>
<%@ Import Namespace="Swarmops.Logic.Financial" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboPeople" Src="~/Controls/v5/Swarm/ComboPeople.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="TextCurrency" Src="~/Controls/v5/Financial/CurrencyTextBox.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ModalDialog" Src="~/Controls/v5/Base/ModalDialog.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="/Scripts/jquery.switchButton.js" language="javascript" type="text/javascript"></script>

	<script type="text/javascript">

	    $(document).ready(function () {

	        $('#tableAccountPlan').treegrid(
	        {
	            onBeforeExpand: function(foo) {
	                $('span.accountplandata-collapsed-' + foo.id).fadeOut('fast', function() {
	                    $('span.accountplandata-expanded-' + foo.id).fadeIn('slow');
	                });
	            },

	            onBeforeCollapse: function(foo) {
	                $('span.accountplandata-expanded-' + foo.id).fadeOut('fast', function() {
	                    $('span.accountplandata-collapsed-' + foo.id).fadeIn('slow');
	                });
	            },

	            onLoadSuccess: function() {
	                $('div.datagrid').css('opacity', 1);

	                $(".IconEdit").mouseover(function() {
	                    $(this).attr("src", "/Images/Icons/iconshock-wrench-16px-hot.png");
	                });

	                $(".IconEdit").mouseout(function() {
	                    $(this).attr("src", "/Images/Icons/iconshock-wrench-16px.png");
	                });

	                $(".IconAdd, .LinkAdd").click(function() {
	                    accountType = $(this).attr("accountType");
	                    addAccount();
	                });

	                $(".IconEdit").click(function() {
	                    accountId = $(this).attr("accountId");
	                    accountType = accountId.substring(0, 1); // A, D, I, C
	                    accountId = accountId.substring(1);

	                    beginEditAccount();
	                });

	                if ($('#CheckOptionsShowInactive').prop("checked"))
	                {
	                    $('.RowInactive').show();
	                }
	            },

	            rowStyler: function (rowData) {
	                if (rowData.rowCssClass != null)
	                {
	                    return { class: rowData.rowCssClass };
	                }
	                if (rowData.inactive == "true")
	                {
	                    return { class: 'RowInactive' };
	                }
	            }
	        });

            $('#<%=this.DropOwner.ClientID %>_SpanPeople span.combo input.combo-text').keydown(function (e) {
                // Clear the owner avatar on any keypress
                $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-image', "none");
            });


	        $('div.datagrid').css('opacity', 0.4);
	        
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
	                url: "/Pages/v5/Ledgers/AccountPlan.aspx/SetAccountName",
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
	                url: "/Pages/v5/Ledgers/AccountPlan.aspx/SetAccountBudget",
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

	        $('#<%=CurrencyInitialBalance.ClientID%>_Input').blur(function() {
	            var newAccountInitialBalance = $(this).val();

	            if (modalAccountInitialBalance == newAccountInitialBalance) {
	                return; // nothing changed, nothing to do
	            }

	            if (accountType == 'D' && newAccountInitialBalance[0] != '-') {
	                alertify.alert('<asp:Literal ID="LiteralDebtBalancesAreNegative" runat="server" />');
	                newAccountInitialBalance = "-" + newAccountInitialBalance;
	            }

	            var jsonData = {};
	            jsonData.accountId = accountId;
	            jsonData.newInitialBalanceString = newAccountInitialBalance;

	            $(this).css('background-color', '#FFFFE0');
	            $.ajax({
	                type: "POST",
	                url: "/Pages/v5/Ledgers/AccountPlan.aspx/SetAccountInitialBalance",
	                data: $.toJSON(jsonData),
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: $.proxy(function(msg) {
	                    if (msg.d.Result == 3 || msg.d.Result == 4) { // Invalid or NoPermission
	                        $(this).css('background-color', '#FFA0A0');
	                        alertify.error("There was an error attempting to set your initial balance."); // TODO: Localize
	                    } else {  // msg.d.Result should be Changed here
	                        $(this).css('background-color', '#E0FFE0');
	                        modalAccountInitialBalance = msg.d.NewData;
	                        accountDirty = true;
	                    }
	                    $(this).val(modalAccountInitialBalance);
	                    $(this).animate({ backgroundColor: "#FFFFFF" }, 250);
	                }, this),
	                error: $.proxy(function(msg) {
	                    alertify.error("There was an error calling the server to set your initial balance. Is the server reachable?"); // TODO: Localize
	                    $(this).val(modalAccountInitialBalance);
	                    $(this).css('background-color', '#FFA0A0');
	                    $(this).animate({ backgroundColor: "#FFFFFF" }, 250);
	                }, this)
	            });
	        });

	        $('#CheckOptionsShowInactive').change(function() {
	            if ($(this).prop("checked"))
	            {
	                $('.RowInactive').slideDown();
	            }
	            else
	            {
	                $('.RowInactive').slideUp();
	            }
	        });

	    });


        function onDialogClose() {
            if (accountDirty) {
                $('#tableAccountPlan').treegrid('reload');
                accountDirty = false;
            }
        }


	    function addAccount() {
	        $.ajax({
	            type: "POST",
	            url: "/Pages/v5/Ledgers/AccountPlan.aspx/CreateAccount",
	            data: "{'accountType': '" + accountType + "'}",
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            async: false, // ugly but necessary to prevent races
	            success: function(msg) {
	                accountId = msg.d;
	                accountType = accountType.substring(0, 1);
	                accountDirty = true; // just created, so update needed
	                beginEditAccount();
	            },
	            error: function() {
	                alertify.error("There was an error calling the server to create the account. Is the server reachable?"); // TODO: Localize
	            }
	        });
	    }


	    function beginEditAccount() {
	        var accountTree = $('#<%=DropParents.ClientID %>_DropBudgets');
	        accountTreeLoaded = false;
	        accountTree.combotree('setText', '');

	        if ((accountType == 'A' || accountType == 'D') && firstFiscalYear > ledgersClosedUntil) {
	            $('#DivEditInitLabels').show();
	            $('#DivEditInitControls').show();
	        } else {
	            $('#DivEditInitLabels').hide();
	            $('#DivEditInitControls').hide();
	        }

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
	        } else { // P&L account
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
	        $('#<%=CurrencyInitialBalance.ClientID%>_Input').val('...').css('background-color', '#DDD');
	        $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').attr('placeholder', '...');

	        accountTree.combotree('setText', '...');
	        parentAccountName = '';

	        window.scrollTo(0, 0);
	        $('body').css('overflow-y', 'hidden');
	        $('#<%=this.DialogAccount.ClientID %>_divModalCover').fadeIn();

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

	                            var jsonData = {};
	                            jsonData.accountId = escape(accountId);
	                            jsonData.switchName = $(this).attr("rel");
	                            jsonData.switchValue = $(this).prop('checked');

	                            var callParameters = $.toJSON(jsonData);

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
	                                url: "/Pages/v5/Ledgers/AccountPlan.aspx/SetAccountSwitch",
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
	                                        // update the "inactive count" display for all success calls, even though we only need it for "active"
	                                        updateInactiveCount();
	                                    } else {
	                                        suppressSwitchChangeAction = true;
	                                        $(this).click();
	                                        suppressSwitchChangeAction = false;
	                                        $(this).parent().css('box-shadow', '0 0 1px 2px rgba(255,96,96,0.8)');
	                                        $(this).parent().animate({
	                                            boxShadow: '0 0 15px 3px rgba(0,255,0,0)'
	                                        }, 750);
	                                        alertify.error("The server refused setting the switch as requested."); // TODO: Localize
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
	            url: "/Pages/v5/Ledgers/AccountPlan.aspx/GetAccountData",
	            data: "{'accountId': '" + escape(accountId) + "'}",
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function(msg) {

	                suppressSwitchChangeAction = true;
	                $("#CheckAccountActive").prop("checked", msg.d.Active).change();
	                $("#CheckAccountExpensable").prop("checked", msg.d.Expensable).change();
	                $("#CheckAccountAdministrative").prop("checked", msg.d.Administrative).change();
	                suppressSwitchChangeAction = false;

	                modalAccountName = msg.d.AccountName;
	                $('#TextAccountName').val(msg.d.AccountName).css('background-color', '#FFF');

	                modalAccountBudget = msg.d.Budget;
	                $('#TextAccountBudget').val(msg.d.Budget).css('background-color', '#FFF');

	                modalAccountInitialBalance = msg.d.InitialBalance;
	                $('#<%=CurrencyInitialBalance.ClientID%>_Input').val(msg.d.InitialBalance).css('background-color', '#FFF');

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

	    }

	    function updateInactiveCount()
	    {
	        $.ajax({
	            type: "POST",
	            url: "/Pages/v5/Ledgers/AccountPlan.aspx/GetInactiveAccountCount",
	            data: "{}",
	            contentType: "application/json; charset=utf-8",
	            dataType: "json",
	            success: function(msg) {
	                $("#SpanInactiveCount").text(msg.d);
	            }
	        });
	    }

        function modalShow() {
            $('#<%=this.DialogAccount.ClientID %>_divModalCover').fadeIn();
        }

        function onOwnerChange(personId) {
            $('span#<%= DropOwner.ClientID %>_SpanPeople span input.combo-text').css('background-color', '#FFFFE0');
            $.ajax({
                type: "POST",
                url: "/Pages/v5/Ledgers/AccountPlan.aspx/SetAccountOwner",
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
	        if (currentText.length < 3 && parentAccountName.length > 2) {
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
	            url: "/Pages/v5/Ledgers/AccountPlan.aspx/SetAccountParent",
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

        ac
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
	    var firstFiscalYear = <%=CurrentOrganization.FirstFiscalYear %>;
	    var ledgersClosedUntil = <%=CurrentOrganization.Parameters.FiscalBooksClosedUntilYear %>;

	    var modalAccountName = "";
	    var modalAccountBudget = "";
	    var modalAccountInitialBalance = "";
	    var accountId = 0;
	    var accountType = '';
	    var accountDirty = false;

	    var checkboxesInitialized = false;
	    var parentDropInitialized = false;
	    var suppressSwitchChangeAction = false;
	    var accountTreeLoaded = false;
	    var parentAccountName = '';

	</script>
    <style type="text/css">
	    .IconEdit, .IconAdd {
		    cursor: pointer;
	    }
	    #IconCloseEdit {
		    cursor: pointer;
		    
	    }
        .SpanGroupName {
            font-weight: 700;
        }
	    .CheckboxContainer {
		    float: right; padding-top: 4px;padding-right: 8px;
	    }
        .RowInactive
        {
            color: #C0C0C0;
            display: none;
        }
        .RowProjectedLoss, .RowProjectedProfit
        {
            font-weight:500;
        }

    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="BoxTitle" runat="server" /></h2>
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

    
    <Swarmops5:ModalDialog ID="DialogAccount" OnClientClose="onDialogClose" runat="server">
        <DialogCode>
            <h2 id="HeaderModal"><asp:Literal ID="LiteralHeaderEditingAccount" runat="server"/></h2>
            <div id="DivModalFields" class="entryFields"><input type="text" id="TextAccountName" />&nbsp;<br />
                <Swarmops5:ComboBudgets ID="DropParents" runat="server" OnClientLoaded="onAccountTreeLoaded" SuppressPrompt="True" OnClientSelect="onAccountTreeSelect" />&nbsp;<br/>
                <div id="DivEditProfitLossControls">&nbsp;<br/>
                <Swarmops5:ComboPeople ID="DropOwner" OnClientSelect="onOwnerChange" runat="server" />&nbsp;<br/>
                <input type="text" id="TextAccountBudget" style="text-align: right"/>&nbsp;<br/>
                &nbsp;<br/>
                <label for="CheckAccountActive"><asp:Literal ID="LiteralLabelActiveShort" runat="server"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Active" class="EditCheck" id="CheckAccountActive"/></div><br/>
                <label for="CheckAccountExpensable"><asp:Literal ID="LiteralLabelExpensableShort" runat="server"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Expensable" class="EditCheck" id="CheckAccountExpensable"/></div><br/>
                <label for="CheckAccountAdministrative"><asp:Literal ID="LiteralLabelAdministrativeShort" runat="server"/></label><div class="CheckboxContainer"><input type="checkbox" rel="Administrative" class="EditCheck" id="CheckAccountAdministrative"/></div>
                &nbsp;<br/></div>
                <div id="DivEditInitControls"><Swarmops5:TextCurrency ID="CurrencyInitialBalance" runat="server" />&nbsp;<br/></div>
                <div id="DivEditAssetControls">
                    &nbsp;<br/>
                    <asp:DropDownList runat="server" ID="DropAccountUploadFormats"/>
                    <input type="text" id="TextAutomationPaymentTag" readonly="readonly"/>&nbsp;<br/>
                </div>
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
                <div id="DivEditInitLabels"><asp:Literal ID="LiteralLabelInitialAmount" runat="server"/></div> 
                <div id="DivEditAssetLabels"><h2><asp:Literal ID="LiteralLabelHeaderAutomation" runat="server"/></h2>
                <asp:Literal ID="LiteralLabelFileUploadProfile" runat="server"/><br/>
                <span id="SpanUploadParameterName">Upload parameter, if any</span></div> 
            </div>
       </DialogCode>
    </Swarmops5:ModalDialog>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    
    <h2 class="blue"><asp:Label ID="LabelSidebarOptions" Text="Options" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content" style="margin-left:5px">
            <div class="link-row-encaps" style="cursor:default; margin-left:2px">
               <span style="position:relative;top:2px;left:1px"><input type="checkbox" id="CheckOptionsShowInactive" /></span>&nbsp;<label for="CheckOptionsShowInactive"><asp:Label ID="LabelOptionsShowInactive" runat="server" Text="Show inactive accounts XYZ"/></label>
            </div>
        </div>
    </div>

</asp:Content>

