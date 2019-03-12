<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.AttestCosts" Codebehind="ApproveCosts.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/ModalDialog.ascx" tagname="ModalDialog" tagprefix="Swarmops5" %>
<%@ Register src="~/Controls/v5/Financial/ComboBudgets.ascx" tagname="ComboBudgets" tagprefix="Swarmops5" %>
<%@ Register src="~/Controls/v5/Financial/CurrencyTextBox.ascx" tagname="CurrencyTextBox" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />

    
    <script type="text/javascript">
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-48x36px.gif',
            '/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-gold.png',
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px.png'
        ]);

        loadUninitializedBudgets(); // no need to wait for doc.ready to load operating params

        SwarmopsJS.ajaxCall("/Pages/v5/Financial/ApproveCosts.aspx/GetRemainingBudgets", {}, function(data) {
            data.forEach(function(accountData, dummy1, dummy2) {
                budgetRemainingLookup[accountData.AccountId] = accountData.Remaining;
            });

            if (budgetRemainingLookup.rowsLoaded == true) {
                setApprovability();
            }

            budgetRemainingLookup.budgetsLoaded = true;
        });

        // Doc.ready:

        $(document).ready(function () {
            $('#tableApprovableCosts').datagrid(
                {
                    rowStyler: function (index, rowData) {
                        if (rowData.approved != null) {
                            return { class: "action-list-item-approved row" + rowData.itemId };
                        }

                        if (rowData.itemId != null) {
                            return { class: "row" + rowData.itemId.replace(/\|/g, '') };
                        }

                        return '';
                    },

                    onLoadSuccess: function () {
                        budgetRemainingLookup.attestabilityInitialized = false;

                        $(".LocalIconDox").attr('src', '/Images/Icons/iconshock-balloon-examine-128x96px.png');

                        $(".LocalIconApproval").attr("src", '/Images/Icons/iconshock-balloon-yes-128x96px.png');  // Hidden initially; wait shown instead
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-green-tick-128x96px.png").css("opacity", 0.5);
                        $(".LocalIconDenied").attr("src", "/Images/Icons/iconshock-red-cross-circled-128x96px.png").hide();
                        $(".LocalIconUndo").attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                        $(".LocalIconWait").attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");  // initializes as wait cursor here until budgets loaded
                        $(".LocalIconApproval.LocalNew, .LocalIconApproved.LocalNew, .LocalIconUndo.LocalNew, .LocalIconDenied.LocalNew, .LocalIconApproval.LocalApproved, .LocalIconWait.LocalApproved, .LocalIconDenial.LocalApproved, .LocalIconDenied.LocalApproved").hide();
                        $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");


                        $(".LocalIconDenial").click(function() {
                            recordId = $(this).attr("baseid");
                            var amountRequested = $("#IconApproval" + recordId).attr('amount');
                            accountId = $("#IconApproval" + recordId).attr('accountid');
                            $('div.radioOption').hide();
                            $('input:radio[name="ModalOptions"]').prop('checked', false);
                            SwarmopsJS.formatCurrency(amountRequested, function (data) { <%=this.TextCorrectAmount.ClientID%>_val(data); });
                            $('<%=this.TextDenyReason.ClientID%>').val(''); // empty reason
                            <%=this.DialogDeny.ClientID%>_open();
                        });

                        $(".LocalIconApproval").click(function() {
                            if ($(this).hasClass("LocalFundsInsufficient")) {
                                if (!canOverdraftBudgets) {
                                    alertify.error(SwarmopsJS.unescape('<%=this.Localized_Error_InsufficientBudget%>'));
                                    return;
                                }

                                // Handle confirm-overdraft case

                                alertify.set({
                                    labels: {
                                        ok: SwarmopsJS.unescape('<%=this.Localized_ConfirmOverdraftNo%>'),
                                        cancel: SwarmopsJS.unescape('<%=this.Localized_ConfirmOverdraftYes%>')
                                    }
                                });

                                alertify.confirm(SwarmopsJS.unescape('<%=this.Localized_ConfirmOverdraftPrompt%>'),
                                    $.proxy(function(response) {
                                        if (!response) {
                                            // user clicked the RED button, which is "confirm overdraft"

                                            onExpenseApproval(this);
                                        }
                                    }, this));

                                return; // Do not process here - must wait for confirm dialog to return
                            }

                            // Handle normal case

                            onExpenseApproval(this);
                        });

                        $(".LocalIconUndo").click(function () {
                            $(this).hide();
                            var itemId = $(this).attr("baseid");
                            $("#IconApproved" + itemId).fadeTo(1000, 0.01);
                            $("#IconWait" + itemId).show();

                            var accountId = $("#IconApproval" + itemId).attr("accountid");
                            var funds = parseFloat($("#IconApproval" + itemId).attr("amount"));
                            budgetRemainingLookup[accountId] -= funds;

                            SwarmopsJS.proxiedAjaxCall
                            (
                                "/Pages/v5/Financial/ApproveCosts.aspx/RetractApproval",
                                { identifier: $(this).attr("baseid") },
                                this,
                                function(result) {
                                    if (result.Success) {
                                        var itemId = $(this).attr("baseid");
                                        $('.row' + itemId).removeClass("action-list-item-approved");
                                        $("#IconApproval" + itemId).removeClass("LocalApproved");
                                        $("#IconWait" + itemId).hide();
                                        $("#IconApproved" + itemId).hide();
                                        $("#IconApproval" + itemId).fadeTo(200, 1);
                                        $("#IconDenial" + itemId).fadeTo(200, 1);
                                        alertify.log(result.DisplayMessage);

                                        recheckBudgets(); // will double-check budgets against server
                                    } else {
                                        // There's probably a concurrency error.
                                        // The socket handler will take care of updating the UI on
                                        // receiving the cause of the concurrency error.

                                        alertify.log(result.DisplayMessage);
                                    }
                                }

                            );  // ends proxiedAjaxCall

                        });  // ends .click(function() {



                        $(".LocalIconDox").click(function () {
                            $("a.FancyBox_Gallery[rel='" + $(this).attr("baseid") + "']").first().click();
                        });

                        $("a.FancyBox_Gallery").fancybox({
                            'overlayShow': true,
                            'transitionIn': 'fade',
                            'transitionOut': 'fade',
                            'type': 'image',
                            'opacity': true
                        });


                        // Check if budgets have been fetched, and if so, initialize attestability

                        if (budgetRemainingLookup.budgetsLoaded == true) {
                                setApprovability();
                            }

                            budgetRemainingLookup.rowsLoaded = true;
                        }
                }
            );

            // we're still in document.ready

            $('input:radio[name="ModalOptions"]').change(function () {
                var pickedButtonName = $(this).val();
                $('div.radioOption').slideUp();
                $('div#radioOption' + pickedButtonName).slideDown();
            });

            $('#buttonExecuteRebudget').val(buttonRebudgetValue);
            $('#buttonExecuteDeny').val(buttonDenyValue);
            $('#buttonExecuteCorrectedAmount').val(buttonCorrectValue);


            // If we're running with admin privileges, allow overdraft override
            // This is a UX thing, the actual check is done server side, of course

            if (canOverdraftBudgets) {
                approvalOverdraftIcon = '/Images/Icons/iconshock-balloon-yes-128x96px-gold.png';
            }
        });


        function onExpenseApproval(approvalIcon) {

            var itemId = $(approvalIcon).attr("baseid");
            $(approvalIcon).hide();
            $('#IconWait' + itemId).show();
            $('#IconDenial' + itemId).fadeTo(1000, 0.01);

            var accountId = $("#IconApproval" + itemId).attr("accountid");
            var funds = parseFloat($("#IconApproval" + itemId).attr("amount"));
            budgetRemainingLookup[accountId] += funds;
            setApprovability(itemId);

            if (budgetUninitializedLookup[accountId] == true && uninitializedPopupDisplayed == false) {

                alertify.set({
                    labels: {
                        ok: SwarmopsJS.unescape('<%=this.Localized_ConfirmDialog_Ok%>')
                    }
                });

                alertify.alert(SwarmopsJS.unescape('<%=this.Localized_WarnUninitializedBudget%>'));
                uninitializedPopupDisplayed = true;
            }

            SwarmopsJS.proxiedAjaxCall (
                "/Pages/v5/Financial/ApproveCosts.aspx/ApproveItem",
                { identifier: $(approvalIcon).attr("baseid") },
                approvalIcon,
                function (result) {
                    var itemId = $(this).attr("baseid");

                    if (result.Success) {
                        $(this).hide();
                        $(this).addClass("LocalApproved");
                        $('.row' + itemId).addClass("action-list-item-approved");

                        $("#IconWait" + itemId).hide();
                        $("#IconDenial" + itemId).hide();
                        $("#IconApproved" + itemId).fadeTo(200, 0.5); // half opacity is intentional
                        $("#IconUndo" + itemId).fadeTo(1000, 1); // the longer delay is intentional
                        alertify.success(result.DisplayMessage);

                        recheckBudgets(); // will double-check budgets against server

                    } else {

                        // failure, likely from approving too quickly and overrunning budget,
                        // or from a concurrency error

                        // In the case of a concurrency error, resetting the UI here might cause
                        // race conditions, but the far more common case will be the overrun
                        // budget race condition, in which case this produces the desired result

                        $('.row' + itemId).removeClass("action-list-item-approved");
                        $("#IconWait" + itemId).hide();
                        $("#IconApproved" + itemId).hide();
                        $("#IconApproval" + itemId).fadeTo(1000, 1); // the longer delay is intentional
                        $("#IconDenial" + itemId).fadeTo(1000, 1); // the longer delay is intentional

                        alertify.error(result.DisplayMessage);

                        recheckBudgets();
                    }

                }  // ends success function parameter

            ); // ends proxiedAjaxCall() call

        } // ends onExpenseApproval() definition



        function recheckBudgets() {
            SwarmopsJS.ajaxCall("/Pages/v5/Financial/ApproveCosts.aspx/GetRemainingBudgets", {}, function(data) {
                data.forEach(function(accountData, dummy1, dummy2) {
                    budgetRemainingLookup[accountData.AccountId] = accountData.Remaining;
                    // console.log("Rechecking budget " + accountData.AccountId + ": remaining is " + accountData.Remaining);
                });

                setApprovability();
            });
        }

        function loadUninitializedBudgets() {
            SwarmopsJS.ajaxCall("/Pages/v5/Financial/ApproveCosts.aspx/GetUninitializedBudgets", {}, function(data) {
                console.log(data);
                data.forEach(function(accountData, dummy1, dummy2) {
                    budgetUninitializedLookup[accountData] = true;
                    // console.log("Rechecking budget " + accountData.AccountId + ": remaining is " + accountData.Remaining);
                });
            });
        }


        function setApprovability(exceptForId) {

            $('.LocalIconApproval').each(function() {

                // Only process items that aren't approved at this time

                if (!$(this).hasClass('LocalApproved')) {

                    var accountId = $(this).attr('accountid');
                    var amountRequested = $(this).attr('amount');
                    var itemId = $(this).attr('baseid');
                    var fundsInBudget = -budgetRemainingLookup[accountId];

                    if (itemId != exceptForId) {

                        // console.log("attestability checking accountid " + accountId + ", amount requested is " + amountRequested + ", funds in budget is " + fundsInBudget);

                        if (fundsInBudget >= amountRequested || budgetUninitializedLookup[accountId] == true) {
                            // console.log("- removing insufficience marker");
                            $(this).removeClass("LocalFundsInsufficient");
                            $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png").show();
                        } else {
                            $(this).attr("src", approvalOverdraftIcon).show();
                            $(this).addClass("LocalFundsInsufficient");
                        }
                        $('#IconWait' + itemId).hide();
                    }
                }

            });

            budgetRemainingLookup.attestabilityInitialized = true;
        }


        function onDenyRecord() {
            var reason = $('#<%=this.TextDenyReason.ClientID%>').val();

            // hide yes/no icons, show waiting
            
            $('#IconApproval' + recordId).fadeOut(1000,0.01);
            $('#IconDenial' + recordId).hide();
            $('#IconWait' + recordId).hide();
            <%= this.DialogDeny.ClientID %>_close();

            SwarmopsJS.proxiedAjaxCall(
                "/Pages/v5/Financial/ApproveCosts.aspx/DenyItem",
                { recordId: recordId, reason: reason },
                $('#IconDenied' + recordId),
                function(result) {
                    if (result.Success) {
                        $(this).fadeTo(1000, 1);
                        $('.row' + $(this).attr('baseid')).addClass("action-list-item-denied");
                    } else {
                        // Failure can happen for many reasons, all bad, so we're just reloading the
                        // entire grid to cover our bases
                        alertify.error(result.DisplayMessage);
                        recheckBudgets();
                        $('#tableApprovableCosts').datagrid('reload');
                    }
                }
            );
        }

        

        function onRebudgetRecord() {
            var newAccountId = <%=this.DropBudgetsRebudget.ClientID%>_val();
            if (newAccountId == 0) {
                alertify.error(decodeURIComponent('<asp:Literal ID="LiteralPleaseSelectBudget" runat="server" />'));
                return;
            }
            if (recordId[0] == 'S') // Salary - cannot rebudget
            {
                alertify.error(decodeURIComponent('<asp:Literal ID="LiteralCannotRebudgetSalary" runat="server" />'));
            }


            // We have a valid budget, and there are no more fail conditions, so close the modal, issue the change, and when
            // returned, reload the grid data

            <%= this.DialogDeny.ClientID %>_close();
            SwarmopsJS.ajaxCall(
                "/Pages/v5/Financial/ApproveCosts.aspx/RebudgetItem",
                { recordId: recordId, newAccountId: newAccountId },
                function(data) {
                    // this is when the change is completed
                    $('#tableApprovableCosts').datagrid('reload');
                });
        }

        function onApproveCorrectedAmount() {
            if (recordId[0] == 'S') // Salary - cannot change amount this way
            {
                alertify.error(decodeURIComponent('<asp:Literal ID="LiteralCannotCorrectSalary" runat="server" />'));
            }

            SwarmopsJS.ajaxCall(
                "/Pages/v5/Financial/ApproveCosts.aspx/ApproveCorrectedItem",
                { recordId: recordId, amountString: <%=this.TextCorrectAmount.ClientID%>_val() },
                function(result) {
                    console.log(result);
                    if (!result.Success) {
                        alertify.error(result.DisplayMessage);
                    } else {
                        // Succeeded, attested for new amount. Since amount was changed, reload grid and budgets
                        recheckBudgets();
                        $('#tableApprovableCosts').datagrid('reload');
                        <%= this.DialogDeny.ClientID %>_close();
                    }
                });
        }

        var recordId = '';
        var accountId = 0;
        var budgetRemainingLookup = {};
        var budgetUninitializedLookup = {};
        var uninitializedPopupDisplayed = false;

        // The variable below is advisory for the UI - actual access control is done server-side
        var canOverdraftBudgets = <%=this.Logic_CanOverdraftBudgets %>;

        var approvalOverdraftIcon = '/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png';

        var buttonRebudgetValue = SwarmopsJS.unescape('<%=this.Localized_ButtonRebudget%>');
        var buttonDenyValue = SwarmopsJS.unescape('<%=this.Localized_ButtonDeny%>');
        var buttonCorrectValue = SwarmopsJS.unescape('<%=this.Localized_ButtonDeny%>');

    </script>
    
     <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
        .rowPrevious {
            color: #AAA;
        }

        div.radioOption {
            margin-top: -20px;
            padding-bottom: 10px;
            padding-left: 12px;
            margin-right: 10px;
        }
        body.ltr div.radioOption {
            padding-left: initial;
            margin-right: initial;
            padding-right: 12px;
            margin-left: 10px;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelAttestCostsHeader" Text="XYZ Costs Awaiting Your Approval" /></h2>
    <table id="tableApprovableCosts" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-ApprovableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'item',width:60"><asp:Label ID="LabelGridHeaderItem" runat="server" Text="XYZ Item"/></th>  
                <th data-options="field:'beneficiary',width:120,sortable:true"><asp:Label ID="LabelGridHeaderBeneficiary" runat="server" Text="XYZ Beneficiary" /></th>  
                <th data-options="field:'description',width:145"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>  
                <th data-options="field:'budgetName',width:160,sortable:true"><asp:Label ID="LabelGridHeaderBudget" runat="server" Text="XYZ Budget" /></th>
                <th data-options="field:'amountRequested',width:80,align:'right',sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderRequested" runat="server" Text="XYZ Requested" /></th>
                <th data-options="field:'dox',width:40,align:'center'"><asp:Label ID="LabelGridHeaderDocs" runat="server" Text="Doxyz" /></th>
                <th data-options="field:'actions',width:68,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="Axyztion" /></th>
            </tr>  
        </thead>
    </table>
    
    <Swarmops5:ModalDialog ID="DialogDeny" runat="server" >
        <DialogCode>
            <h2><asp:Label ID="LabelModalDenyHeader" runat="server" Text="Fix Problems Or Deny Approval XYZ" /></h2>
            <p><asp:Literal ID="LabelWhatProblem" runat="server" Text="What seems to be the problem? XYZ" /></p>
            <p><input type="radio" id="RadioDeny" name="ModalOptions" value="Deny" onclick="$('#<%=this.TextDenyReason.ClientID%>').focus();" /><label for="RadioDeny"><asp:Label runat="server" ID="LabelRadioDeny" Text="I will not attest this record. It is scratched. XYZ" /></label></p>
            <div id="radioOptionDeny" class="radioOption">
                <div class="data-entry-fields">
                    <asp:TextBox ID="TextDenyReason" runat="server" TextMode="MultiLine" Rows="3" Placeholder="My hovercraft is full of eels" />&#8203;<br/>
                    <input type="button" value='#Deny#' class="button-accent-color" onclick="onDenyRecord(); return false;" id="buttonExecuteDeny"/>
                </div>
                <div class="data-entry-labels">
                    <asp:Label runat="server" ID="LabelDescribeDeny" Text="Optional explanation to submitter: XYZ" />
                </div>
                <div style="clear:both"></div>
            </div>
            <p><input type="radio" id="RadioCorrect" name="ModalOptions" value="Correct" onclick="<%=this.TextCorrectAmount.ClientID%>_focus();" /><label for="RadioCorrect"><asp:Label runat="server" ID="LabelRadioCorrect" Text="I will attest, but for a different amount. XYZ" /></label></p>
            <div id="radioOptionCorrect" class="radioOption">
                <div class="data-entry-fields">
                    <Swarmops5:CurrencyTextBox ID="TextCorrectAmount" runat="server" />
                    <input type="button" value='#Correct#' class="button-accent-color" onclick=" onApproveCorrectedAmount(); return false;" id="buttonExecuteCorrectedAmount"/>
                </div>
                <div class="data-entry-labels">
                    <asp:Label runat="server" ID="LabelDescribeCorrect" Text="What amount are you attesting instead (SEK)? XYZ" /><br/>
                    <div class="ifVatEnabled"><asp:Label runat="server" ID="LabelDescribeCorrectNoVat" Text="(The VAT must not be included! XYZ)"/></div>
                </div>
                <div style="clear:both"></div>
            </div>
            <p><input type="radio" id="RadioRebudget" name="ModalOptions" value="Rebudget" /><label for="RadioRebudget"><asp:Label runat="server" ID="LabelRadioRebudget" Text="This record should be charging a different budget. XYZ" /></label></p>
            <div id="radioOptionRebudget" class="radioOption">
                <div class="data-entry-fields">
                    <Swarmops5:ComboBudgets ID="DropBudgetsRebudget" runat="server" ListType="Expensable" />&#8203;<br/>
                    <input type="button" value='#Rebudget#' class="button-accent-color" onclick="onRebudgetRecord(); return false;" id="buttonExecuteRebudget"/>
                </div>
                <div class="data-entry-labels">
                    <asp:Label runat="server" ID="LabelDescribeRebudget" Text="Move the record to this budget: XYZ" />
                </div>
                <div style="clear:both"></div>
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>

    <div style="display:none">
    <!-- a href links for FancyBox to trigger on -->
    
    <asp:Repeater runat="server" ID="RepeaterLightboxItems">
        <ItemTemplate>
            <a href="/Pages/v5/Support/StreamUpload.aspx?DocId=<%# Eval("DocId") %>&hq=1" title="<%# Eval("Title") %>" class="FancyBox_Gallery" rel="<%# Eval("BaseId") %>">&nbsp;</a>
        </ItemTemplate>
    </asp:Repeater>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

