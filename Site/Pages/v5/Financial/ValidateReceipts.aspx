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

                        $(".LocalIconApproval").attr('src', '/Images/Icons/iconshock-balloon-yes-128x96px.png');
                        $(".LocalIconApproved").attr('src', '/Images/Icons/iconshock-green-tick-128x96px.png').hide();
                        $(".LocalIconDenial").attr('src', '/Images/Icons/iconshock-balloon-no-128x96px.png');
                        $(".LocalIconDenied").attr('src', '/Images/Icons/iconshock-red-cross-128x96px.png').hide();
                        $(".LocalIconUndo").attr('src', '/Images/Icons/iconshock-balloon-undo-128x96px.png').hide();


                        $(".LocalIconApproval").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");
                                $("#IconDenial" + $(this).attr("baseid")).fadeTo(1000, 0.01);
                                var thisIcon = this;
                                $.ajax({
                                    type: "POST",
                                    url: "/Pages/v5/Financial/ValidateReceipts.aspx/Validate",
                                    data: "{'identifier': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        $(thisIcon).attr("src", "/Images/Icons/iconshock-balloon-yes-128x96px.png");
                                        $(thisIcon).attr("rel", "active");
                                        $(thisIcon).hide();
                                        $("#IconDenial" + $(thisIcon).attr("baseid")).hide();
                                        $("#IconApproved" + $(thisIcon).attr("baseid")).fadeTo(200, 1);
                                        $("#IconUndo" + $(thisIcon).attr("baseid")).fadeTo(1000, 1); // the longer delay is intentional
                                        alertify.success(unescape(msg.d));
                                    }
                                });
                            }
                        });

                        $(".LocalIconUndo").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");
                                var thisIcon = this;
                                $("#IconApproved" + $(this).attr("baseid")).fadeTo(1000, 0.01);
                                $.ajax({
                                    type: "POST",
                                    url: "/Pages/v5/Financial/ValidateReceipts.aspx/Devalidate",
                                    data: "{'identifier': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        $(thisIcon).attr("src", "/Images/Icons/iconshock-balloon-undo-128x96px.png");
                                        $(thisIcon).css("display", "none");
                                        $(thisIcon).attr("rel", "");
                                        $("#IconApproved" + $(thisIcon).attr("baseid")).hide();
                                        $("#IconApproval" + $(thisIcon).attr("baseid")).fadeTo(200, 1);
                                        $("#IconDenial" + $(thisIcon).attr("baseid")).fadeTo(200, 1);
                                        $("#" + $(thisIcon).attr("rel"), "");
                                        alertify.log(unescape(msg.d).replace('+', ' '));
                                    }
                                });

                            }
                        });


                        $(".LocalIconDenial").click(function () {
                            alert('Denying validation is not yet implemented, but you can just leave the unwanted cost here until it is.');
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
                    }
                }
            );
        });

    </script>
    
     <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelAttestCostsHeader" Text="XYZ Costs Awaiting Your Attestation" /></h2>
    <table id="TableAttestableCosts" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-ValidatableReceipts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <asp:Literal runat="server" ID="LiteralDescriptionThStart" /><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /><asp:Literal runat="server" ID="LiteralDescriptionThClose" Text="</th>" />  
                <asp:Literal runat="server" ID="LiteralBudgetThStart" /><asp:Label ID="LabelGridHeaderBudget" runat="server" Text="XYZ Budget" /><asp:Literal runat="server" ID="LiteralBudgetThClose" Text="</th>" />
                <asp:Literal runat="server" ID="LiteralExtraTags" />
                <th data-options="field:'amountRequested',width:80,align:'right',sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderRequested" runat="server" Text="XYZ Requested" /></th>
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

