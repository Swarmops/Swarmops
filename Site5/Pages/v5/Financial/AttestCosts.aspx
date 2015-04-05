<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AttestCosts.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Financial.AttestCosts" %>
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
            '/Images/Abstract/ajaxloader-medium.gif',
            '/Images/Icons/iconshock-balloon-yes-128x96px-hot.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-hot-disabled.png',
            '/Images/Icons/iconshock-balloon-no-128x96px-hot.png',
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px-hot.png'
        ]);

        $(document).ready(function () {
            $('#TableAttestableCosts').datagrid(
                {
                    rowStyler: function (index, rowData) {
                        if (rowData.previous != null) {
                            return { class: "rowPrevious row" + rowData.itemId };
                        }

                        if (rowData.itemId != null) {
                            return { class: "row" + rowData.itemId.replace(/\|/g, '') };
                        }

                        return '';
                    },

                    onLoadSuccess: function () {
                        $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png"); // initialize as disabled until budgets known
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-green-tick-128x96px.png").css("opacity", 0.5);
                        $(".LocalIconDenied").attr("src", "/Images/Icons/iconshock-red-cross-circled-128x96px.png");
                        $(".LocalIconUndo").attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                        $(".LocalIconApproved.LocalNew, .LocalIconUndo.LocalNew, .LocalIconDenied.LocalNew, .LocalIconApproval.LocalPreviouslyAttested, .LocalIconDenial.LocalPreviouslyAttested, .LocalIconDenied.LocalPreviouslyAttested").css("display", "none");
                        $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");
                        $(".LocalIconApproval, .LocalIconUndo, .LocalIconDenial").css("cursor", "pointer");

                        $(".LocalIconApproval").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                if ($(this).hasClass("LocalFundsInsufficient")) {
                                    $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-hot-disabled.png");
                                } else {
                                    $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-hot.png");
                                }
                            }
                        });

                        $(".LocalIconApproval").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                if ($(this).hasClass("LocalFundsInsufficient")) {
                                    $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png");
                                } else {
                                    $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                                }
                            }
                        });

                        $(".LocalIconUndo").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px-hot.png");
                            }
                        });

                        $(".LocalIconUndo").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                            }
                        });

                        $(".LocalIconDenial").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px-hot.png");
                            }
                        });

                        $(".LocalIconDenial").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");
                            }
                        });

                        $(".LocalIconDenial").click(function() {
                            if ($(this).attr("rel") != "loading" && $("#IconApproval" + $(this).attr("baseid")) != "loading") {
                                recordId = $(this).attr("baseid");
                                var amountRequested = $("#IconApproval" + recordId).attr('amount');
                                accountId = $("#IconApproval" + recordId).attr('accountid');
                                $('div.radioOption').hide();
                                $('input:radio[name="ModalOptions"]').prop('checked', false);
                                SwarmopsJS.formatCurrency(amountRequested, function (data) { $('#<%=this.TextCorrectAmount.ClientID%>_Input').val(data); });
                                $('<%=this.TextDenyReason.ClientID%>').val(''); // empty reason
                                <%=this.DialogDeny.ClientID%>_open();
                            }
                        });

                        $(".LocalIconApproval").click(function () {
                            if ($(this).hasClass("LocalFundsInsufficient")) {
                                alertify.error (decodeURIComponent('<asp:Literal id="LiteralErrorInsufficientBudget" runat="server" />'));
                                return;
                            }

                            if ($(this).attr("rel") != "loading" && $("#IconDenial" + $(this).attr("baseid")) != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");
                                $("#IconDenial" + $(this).attr("baseid")).fadeTo(1000, 0.01).css("cursor", "default");
                                $.ajax({
                                    type: "POST",
                                    url: "/Pages/v5/Financial/AttestCosts.aspx/Attest",
                                    data: "{'identifier': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: $.proxy(function (msg) {
                                        var baseid = $(this).attr("baseid");
                                        if (msg.d.Success) {
                                            $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                                            $(this).attr("rel", "active");
                                            $(this).hide();
                                            $("#IconApproved" + baseid).fadeTo(250, 0.5);
                                            $("#IconDenial" + baseid).finish().css("display", "none").css("opacity", 1.0);
                                            $("#IconUndo" + baseid).fadeIn(100);
                                            $('.row' + baseid).animate({ color: "#AAA" }, 400);
                                            alertify.success(msg.d.DisplayMessage);

                                            var accountId = $(this).attr("accountid");
                                            var funds = parseFloat($(this).attr("amount"));
                                            budgetRemainingLookup[accountId] -= funds;
                                            setAttestability();
                                            recheckBudgets(); // will double-check budgets against server
                                        } else {
                                            // failure, likely from attesting too quickly and overrunning budget
                                            $(this).attr("rel", "");
                                            $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png");
                                            $("#IconDenial" + baseid).css('opacity', 1.0).css("cursor", "pointer");
                                            alertify.error(msg.d.DisplayMessage);

                                            recheckBudgets();
                                        }
                                    }, this)
                                });
                            }
                        });

                        $(".LocalIconUndo").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");
                                $("#IconApproved" + $(this).attr("baseid")).fadeTo(1000, 0.01);
                                $.ajax({
                                    type: "POST",
                                    url: "/Pages/v5/Financial/AttestCosts.aspx/Deattest",
                                    data: "{'identifier': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: $.proxy(function (msg) {
                                        if (msg.d.Success) {
                                            var baseid = $(this).attr("baseid");
                                            $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                                            $(this).attr("rel", "");
                                            $(this).hide();
                                            $("#IconApproved" + baseid).finish().css("opacity", 0.5).css("display", "none");
                                            $("#IconApproval" + baseid).fadeIn(100);
                                            $("#IconDenial" + baseid).fadeIn(100).css("cursor", "pointer");
                                            $('.row' + baseid).animate({ color: "#000" }, 100);
                                            alertify.log(msg.d.DisplayMessage);

                                            var accountId = $("#IconApproval" + baseid).attr("accountid");
                                            var funds = parseFloat($("#IconApproval" + baseid).attr("amount"));
                                            budgetRemainingLookup[accountId] += funds;
                                            setAttestability();
                                            recheckBudgets(); // will double-check budgets against server
                                        } else {
                                            $(this).attr("src", "/Images/Icons/iconshock-greentick-128x96px.png");
                                            alertify.error(msg.d.DisplayMessage);
                                            // TODO: Add alert box?
                                        }
                                    }, this)
                                });

                            }
                        });




                        $(".LocalIconDenial").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px-hot.png");
                            }
                        });

                        $(".LocalIconDenial").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");
                            }
                        });

                        $(".LocalViewDox").click(function () {
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
                            setAttestability();
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


            SwarmopsJS.ajaxCall("/Pages/v5/Financial/AttestCosts.aspx/GetRemainingBudgets", {}, function(data) {
                data.forEach(function(accountData, dummy1, dummy2) {
                    budgetRemainingLookup[accountData.AccountId] = accountData.Remaining;
                });

                if (budgetRemainingLookup.rowsLoaded == true) {
                    setAttestability();
                }

                budgetRemainingLookup.budgetsLoaded = true;
            });
        });


        function recheckBudgets() {
            SwarmopsJS.ajaxCall("/Pages/v5/Financial/AttestCosts.aspx/GetRemainingBudgets", {}, function(data) {
                data.forEach(function(accountData, dummy1, dummy2) {
                    budgetRemainingLookup[accountData.AccountId] = accountData.Remaining;
                });

                setAttestability();
            });
        }


        function setAttestability() {

            $('.LocalIconApproval').each(function() {
                var accountId = $(this).attr('accountid');
                var amountRequested = $(this).attr('amount');

                var fundsInBudget = budgetRemainingLookup[accountId];
                if (fundsInBudget >= amountRequested) {
                    $(this).removeClass("LocalFundsInsufficient");
                    if ($(this).attr("rel") != "loading") {
                        $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                    }
                }
                else if (!$(this).hasClass("LocalFundsInsufficient")) {
                    $(this).addClass("LocalFundsInsufficient");
                    if ($(this).attr("rel") != "loading") {
                        $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png");
                    }
                }

            });

            budgetRemainingLookup.attestabilityInitialized = true;
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
                "/Pages/v5/Financial/AttestCosts.aspx/RebudgetItem",
                { recordId: recordId, newAccountId: newAccountId },
                function(data) {
                    // this is when the change is completed
                    $('#TableAttestableCosts').datagrid('reload');
                });
        }

        function onAttestCorrectedAmount() {
            if (recordId[0] == 'S') // Salary - cannot change amount this way
            {
                alertify.error(decodeURIComponent('<asp:Literal ID="LiteralCannotCorrectSalary" runat="server" />'));
            }

            SwarmopsJS.ajaxCall(
                "/Pages/v5/Financial/AttestCosts.aspx/AttestCorrectedItem",
                { recordId: recordId, amountString: $('#<%=this.TextCorrectAmount.ClientID%>_Input').val() },
                function(result) {
                    console.log(result);
                    if (!result.Success) {
                        alertify.error(result.DisplayMessage);
                    } else {
                        // Succeeded, attested for new amount. Since amount was changed, reload grid
                        $('#TableAttestableCosts').datagrid('reload');
                        <%= this.DialogDeny.ClientID %>_close();
                    }
                });
        }

        var recordId = '';
        var accountId = 0;
        var budgetRemainingLookup = {};

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
    <h2><asp:Label runat="server" ID="LabelAttestCostsHeader" Text="XYZ Costs Awaiting Your Attestation" /></h2>
    <table id="TableAttestableCosts" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-AttestableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'item',width:60"><asp:Label ID="LabelGridHeaderItem" runat="server" Text="XYZ Item"/></th>  
                <th data-options="field:'beneficiary',width:120,sortable:true"><asp:Label ID="LabelGridHeaderBeneficiary" runat="server" Text="XYZ Beneficiary" /></th>  
                <th data-options="field:'description',width:160"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>  
                <th data-options="field:'budgetName',width:160,sortable:true"><asp:Label ID="LabelGridHeaderBudget" runat="server" Text="XYZ Budget" /></th>
                <th data-options="field:'amountRequested',width:80,align:'right',sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderRequested" runat="server" Text="XYZ Requested" /></th>
                <th data-options="field:'dox',width:40,align:'center'"><asp:Label ID="LabelGridHeaderDocs" runat="server" Text="Doxyz" /></th>
                <th data-options="field:'actions',width:53,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="Axyztion" /></th>
            </tr>  
        </thead>
    </table>
    
    <Swarmops5:ModalDialog ID="DialogDeny" runat="server" >
        <DialogCode>
            <h2><asp:Label ID="LabelModalDenyHeader" runat="server" Text="Fix Problems Or Deny Attestation XYZ" /></h2>
            <p><asp:Literal ID="LabelWhatProblem" runat="server" Text="What seems to be the problem? XYZ" /></p>
            <p><input type="radio" id="RadioDeny" name="ModalOptions" value="Deny" onclick="$('#<%=this.TextDenyReason.ClientID%>').focus();" /><label for="RadioDeny"><asp:Label runat="server" ID="LabelRadioDeny" Text="I will not attest this record. It is scratched. XYZ" /></label></p>
            <div id="radioOptionDeny" class="radioOption">
                <div class="entryFields">
                    <asp:TextBox ID="TextDenyReason" runat="server" TextMode="MultiLine" Rows="3" Placeholder="My hovercraft is full of eels" />&#8203;<br/>
                    <input type="button" value='<asp:Literal ID="LiteralButtonDeny" runat="server" Text="RebudgetXYZ" />' class="buttonAccentColor" onclick="onDenyRecord(); return false;" id="buttonExecuteDeny"/>
                </div>
                <div class="entryLabels">
                    <asp:Label runat="server" ID="LabelDescribeDeny" Text="Optional explanation to submitter: XYZ" />
                </div>
                <div style="clear:both"></div>
            </div>
            <p><input type="radio" id="RadioCorrect" name="ModalOptions" value="Correct" onclick="$('#<%=this.TextCorrectAmount.ClientID%>_Input').focus().select();" /><label for="RadioCorrect"><asp:Label runat="server" ID="LabelRadioCorrect" Text="I will attest, but for a different amount. XYZ" /></label></p>
            <div id="radioOptionCorrect" class="radioOption">
                <div class="entryFields">
                    <Swarmops5:CurrencyTextBox ID="TextCorrectAmount" runat="server" />&#8203;<br/>
                    <input type="button" value='<asp:Literal ID="LiteralButtonCorrect" runat="server" Text="AmountXYZ" />' class="buttonAccentColor" onclick="onAttestCorrectedAmount(); return false;" id="buttonExecuteCorrectedAmount"/>
                </div>
                <div class="entryLabels">
                    <asp:Label runat="server" ID="LabelDescribeCorrect" Text="What amount are you attesting instead (SEK)? XYZ" />
                </div>
                <div style="clear:both"></div>
            </div>
            <p><input type="radio" id="RadioRebudget" name="ModalOptions" value="Rebudget" /><label for="RadioRebudget"><asp:Label runat="server" ID="LabelRadioRebudget" Text="This record should be charging a different budget. XYZ" /></label></p>
            <div id="radioOptionRebudget" class="radioOption">
                <div class="entryFields">
                    <Swarmops5:ComboBudgets ID="DropBudgetsRebudget" runat="server" ListType="Expensable" />&#8203;<br/>
                    <input type="button" value='<asp:Literal ID="LiteralButtonRebudget" runat="server" Text="RebudgetXYZ" />' class="buttonAccentColor" onclick="onRebudgetRecord(); return false;" id="buttonExecuteRebudget"/>
                </div>
                <div class="entryLabels">
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
            <a href="/Pages/v5/Support/StreamUpload.aspx?DocId=<%# Eval("DocId") %>" title="<%# Eval("Title") %>" class="FancyBox_Gallery" rel="<%# Eval("BaseId") %>">&nbsp;</a>
        </ItemTemplate>
    </asp:Repeater>

    </div>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

