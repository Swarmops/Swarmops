<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.ListInvoicesInbound" Codebehind="ListInvoicesInbound.aspx.cs" %>
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
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px.png'
        ]);

        /* -- commented out -- do we need attestation logic for this page?
        loadUninitializedBudgets(); // no need to wait for doc.ready to load operating params

        SwarmopsJS.ajaxCall("/Pages/v5/Financial/AttestCosts.aspx/GetRemainingBudgets", {}, function(data) {
            data.forEach(function(accountData, dummy1, dummy2) {
                budgetRemainingLookup[accountData.AccountId] = accountData.Remaining;
            });

            if (budgetRemainingLookup.rowsLoaded == true) {
                setApprovability();
            }

            budgetRemainingLookup.budgetsLoaded = true;
        });*/

        // Doc.ready:

        $(document).ready(function() {

            // we're still in document.ready

            $('input:radio[name="ModalOptions"]').change(function () {
                var pickedButtonName = $(this).val();
                $('div.radioOption').slideUp();
                $('div#radioOption' + pickedButtonName).slideDown();
            });

            $("a.FancyBox_Gallery").fancybox({
                'overlayShow': true,
                'transitionIn': 'fade',
                'transitionOut': 'fade',
                'type': 'image',
                'opacity': true
            });

            $('#tableInboundInvoices').datagrid(
                {
                    onLoadSuccess: function() {

                        $(".LocalIconDox").attr('src', '/Images/Icons/iconshock-balloon-examine-128x96px.png');

                        $(".LocalIconDox").click(function() {

                            $("a.FancyBox_Gallery[rel='" + $(this).attr("baseid") + "']").first().click();


                        });
                    }
                }
            );
        });



        function setApprovability() {

            $('.LocalIconApproval').each(function() {
                var accountId = $(this).attr('accountid');
                var amountRequested = $(this).attr('amount');
                var fundsInBudget = -budgetRemainingLookup[accountId];

                // console.log("attestability checking accountid " + accountId + ", amount requested is " + amountRequested + ", funds in budget is " + fundsInBudget);

                if (fundsInBudget >= amountRequested || budgetUninitializedLookup[accountId] == true) {
                    // console.log("- removing insufficience marker");
                    $(this).removeClass("LocalFundsInsufficient");
                    if ($(this).attr("rel") != "loading") {
                        $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                    }
                }
                else {
                    if ($(this).attr("rel") != "loading") {
                        $(this).attr("src", approvalOverdraftIcon);
                    }

                    if (!$(this).hasClass("LocalFundsInsufficient")) {
                        // console.log("- adding insufficience marker");
                        $(this).addClass("LocalFundsInsufficient");
                        if ($(this).attr("rel") != "loading") {
                            $(this).attr("src", approvalOverdraftIcon);
                        }
                    }
                }

            });

            budgetRemainingLookup.attestabilityInitialized = true;
        }

        var recordId = '';
        var accountId = 0;
        var budgetRemainingLookup = {};
        var budgetUninitializedLookup = {};
        var uninitializedPopupDisplayed = false;

        // The variable below is advisory for the UI - actual access control is done server-side
        var canOverdraftBudgets = <asp:Literal ID="LiteralCanOverdraftBudgets" runat="server" />;

        var approvalOverdraftIcon = '/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png';
        var approvalOverdraftIconHover = approvalOverdraftIconHover;

        var buttonRebudgetValue = SwarmopsJS.unescape('<asp:Literal ID="LiteralButtonRebudget" runat="server" Text="RebudgetXYZ" />');
        var buttonDenyValue = SwarmopsJS.unescape('<asp:Literal ID="LiteralButtonDeny" runat="server" Text="RebudgetXYZ" />');
        var buttonCorrectValue = SwarmopsJS.unescape('<asp:Literal ID="LiteralButtonCorrect" runat="server" Text="AmountXYZ" />');

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
    <h2><asp:Label runat="server" ID="LabelListInboundInvoicesHeader" Text="XYZ List Inbound Invoices" /></h2>
    <table id="tableInboundInvoices" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,fitWidth:true,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-ListInvoicesInbound.aspx'"
        idField="itemId">
        <thead>
            <tr>  
                <th data-options="field:'item',width:60"><asp:Label ID="LabelGridHeaderItem" runat="server" Text="XYZ Item"/></th>  
                <th data-options="field:'dueDate',width:60"><asp:Label ID="LabelGridHeaderDueDate" runat="server" Text="XYZ DueDate" /></th>  
                <th data-options="field:'sender',width:140,sortable:true"><asp:Label ID="LabelGridHeaderBeneficiary" runat="server" Text="XYZ Beneficiary" /></th>  
                <th data-options="field:'budget',width:140,sortable:true"><asp:Label ID="LabelGridHeaderBudget" runat="server" Text="XYZ Budget" /></th>  
                <th data-options="field:'amount',width:100,align:'right',sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderRequested" runat="server" Text="XYZ Requested" /></th>
                <th data-options="field:'progress',width:76,align:'center'"><asp:Label ID="LabelGridHeaderProgress" runat="server" Text="XYZ Progress" /></th>
                <th data-options="field:'dox',width:40,align:'center'"><asp:Label ID="LabelGridHeaderDocs" runat="server" Text="Doxyz" /></th>
                <th data-options="field:'actions',width:53,align:'center'"><asp:Label ID="LabelGridHeaderActions" runat="server" Text="XYZAction" /></th>
            </tr>  
        </thead>
    </table>
    
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

