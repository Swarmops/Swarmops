<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.ValidateReceipts" Codebehind="ValidateReceipts.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
	<script type="text/javascript" src="/Scripts/fancybox/jquery.fancybox-1.3.4.js"></script>
    <script type="text/javascript" src="/Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.snipe.js"></script>
	<link rel="stylesheet" type="text/css" href="/Scripts/fancybox/jquery.fancybox-1.3.4.css" media="screen" />

    
    <script type="text/javascript">
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-medium.gif',
            '/Images/Abstract/ajaxloader-48x36px.gif',
            '/Images/Icons/iconshock-balloon-yes-128x96px-disabled.png',
            '/Images/Icons/iconshock-balloon-yes-128x96px-gold.png',
            '/Images/Icons/iconshock-balloon-no-128x96px-hot.png',
            '/Images/Icons/iconshock-green-tick-128x96px.png',
            '/Images/Icons/iconshock-red-cross-128x96px.png',
            '/Images/Icons/iconshock-red-cross-circled-128x96px.png',
            '/Images/Icons/iconshock-balloon-undo-128x96px.png',
            '/Images/Icons/iconshock-search-256px.png'
        ]);

        $(document).ready(function () {
            $('#TableAttestableCosts').datagrid(
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

                        $(".LocalIconDox").attr('src', '/Images/Icons/iconshock-balloon-examine-128x96px.png');
                        $(".LocalIconApproval").attr('src', '/Images/Icons/iconshock-balloon-yes-128x96px.png');
                        $(".LocalIconApproved").attr('src', '/Images/Icons/iconshock-green-tick-128x96px.png').hide();
                        $(".LocalIconDenial").attr('src', '/Images/Icons/iconshock-balloon-no-128x96px.png');
                        $(".LocalIconDenied").attr('src', '/Images/Icons/iconshock-red-cross-128x96px.png').hide();
                        $(".LocalIconUndo").attr('src', '/Images/Icons/iconshock-balloon-undo-128x96px.png').hide();
                        $(".LocalIconWait").attr('src', '/Images/Abstract/ajaxloader-48x36px.gif').hide();

                        $(".LocalIconApproval").click(function () {
                            var itemId = $(this).attr("baseid");
                            $(this).hide();
                            $("#IconWait" + itemId).show();
                            $("#IconDenial" + itemId).fadeTo(1000, 0.01);

                            SwarmopsJS.proxiedAjaxCall(
                                "/Pages/v5/Financial/ValidateReceipts.aspx/Validate",
                                { identifier: itemId },
                                this,
                                function (result) {
                                    if (result.Success) {
                                        var itemId = $(this).attr("baseid");
                                        $('.row' + itemId).addClass("action-list-item-approved");
                                        $("#IconWait" + itemId).hide();
                                        $("#IconDenial" + itemId).hide();
                                        $("#IconApproved" + itemId).fadeTo(200, 0.5); // half opacity is intentional
                                        $("#IconUndo" + itemId).fadeTo(1000, 1); // the longer delay is intentional
                                        alertify.success(result.DisplayMessage);
                                    } else {
                                        // There's probably a concurrency error.
                                        // The socket handler will take care of updating the UI on
                                        // receiving the cause of the concurrency error.

                                        alertify.log(result.DisplayMessage);
                                       
                                    }
                                }
                            );
                        });

                        $(".LocalIconUndo").click(function () {
                            $(this).hide();
                            var itemId = $(this).attr("baseid");
                            $("#IconApproved" + itemId).fadeTo(1000, 0.01);
                            $("#IconWait" + itemId).show();

                            SwarmopsJS.proxiedAjaxCall(
                                "/Pages/v5/Financial/ValidateReceipts.aspx/RetractValidation",
                                { identifier: $(this).attr("baseid") },
                                this,
                                function(result) {
                                    if (result.Success) {
                                        var itemId = $(this).attr("baseid");
                                        $('.row' + itemId).removeClass("action-list-item-approved");
                                        $("#IconWait" + itemId).hide();
                                        $("#IconApproved" + itemId).hide();
                                        $("#IconApproval" + itemId).fadeTo(200, 1);
                                        $("#IconDenial" + itemId).fadeTo(200, 1);
                                        alertify.log(result.DisplayMessage);

                                    } else {
                                        // There's probably a concurrency error.
                                        // The socket handler will take care of updating the UI on
                                        // receiving the cause of the concurrency error.

                                        alertify.log(result.DisplayMessage);
                                    }
                                }
                            );

                        });


                        $(".LocalIconDenial").click(function () {
                            alert('Denying validation is not yet implemented, but you can just leave the unwanted cost here until it is.');
                        });

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
                    }
                }
            );
        });

    </script>
    
    <style type="text/css">
        /* any extra style goes here */
    </style>
    

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelAttestCostsHeader" Text="XYZ Costs Awaiting Your Approval" /></h2>
    <table id="TableAttestableCosts" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-ValidatableReceipts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <asp:Literal runat="server" ID="LiteralDescriptionThStart" /><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /><asp:Literal runat="server" ID="LiteralDescriptionThClose" Text="</th>" />  
                <asp:Literal runat="server" ID="LiteralBudgetThStart" /><asp:Label ID="LabelGridHeaderBudget" runat="server" Text="XYZ Budget" /><asp:Literal runat="server" ID="LiteralBudgetThClose" Text="</th>" />
                <asp:Literal runat="server" ID="LiteralExtraTags" />
                <th data-options="field:'amountRequested',width:90,align:'right',sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderRequested" runat="server" Text="XYZ Requested" /></th>
                <th data-options="field:'dox',width:40,align:'center'"><asp:Label ID="LabelGridHeaderDocs" runat="server" Text="Doxyz" /></th>
                <th data-options="field:'actions',width:68,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="Axyztion" /></th>
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

