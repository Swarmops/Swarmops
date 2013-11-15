<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AttestCosts.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Financial.AttestCosts" %>
<%@ Register src="~/Controls/v5/UI/ExternalScripts.ascx" tagname="ExternalScripts" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="datagrid" runat="server" />
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
            '/Images/Icons/iconshock-balloon-yes-16px-hot.png',
            '/Images/Icons/iconshock-balloon-no-16px-hot.png',
            '/Images/Icons/iconshock-greentick-16px.png',
            '/Images/Icons/iconshock-redcross-16px.png',
            '/Images/Icons/undo-16px.png'
        ]);

        $(document).ready(function () {
            $('#TableAttestableCosts').datagrid(
                {
                    onLoadSuccess: function () {
                        $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                        $(".LocalIconApproved, .LocalIconDenied").css("display", "none");
                        $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-balloon-no-16px.png");
                        $(".LocalIconApproval, .LocalIconApproved, .LocalIconDenial").css("cursor", "pointer");

                        $(".LocalIconApproval").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-16px-hot.png");
                            }
                        });

                        $(".LocalIconApproval").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                            }
                        });

                        $(".LocalIconApproved").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/undo-16px.png");
                            }
                        });

                        $(".LocalIconApproved").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                            }
                        });

                        $(".LocalIconApproval").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $("#IconDenial" + $(this).attr("baseid")).fadeTo(1000, 0.01).css("cursor", "default");
                                var thisIcon = this;
                                $.ajax({
                                    type: "POST",
                                    url: "AttestCosts.aspx/Attest",
                                    data: "{'identifier': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        $(thisIcon).css("display", "none");
                                        $(thisIcon).attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                                        $(thisIcon).attr("rel", "active");
                                        $("#IconApproved" + $(thisIcon).attr("baseid")).fadeIn(100);
                                        alertify.success(unescape(msg.d));
                                    }
                                });
                            }
                        });

                        $(".LocalIconApproved").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                var thisIcon = this;
                                $.ajax({
                                    type: "POST",
                                    url: "AttestCosts.aspx/Deattest",
                                    data: "{'identifier': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        $(thisIcon).css("display", "none");
                                        $(thisIcon).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                        $(thisIcon).attr("rel", "");
                                        $("#IconApproval" + $(thisIcon).attr("baseid")).fadeIn(100);
                                        $("#" + $(thisIcon).attr("rel"), "");
                                        alertify.log(unescape(msg.d).replace('+', ' '));
                                        $("#IconDenial" + $(thisIcon).attr("baseid")).fadeTo(100, 1).css("cursor", "pointer");
                                    }
                                });

                            }
                        });




                        $(".LocalIconDenial").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-16px-hot.png");
                            }
                        });

                        $(".LocalIconDenial").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-no-16px.png");
                            }
                        });


                        $(".LocalIconDenial").click(function () {
                            alert('Denying attestation is not yet implemented, but you can just leave the unwanted cost here until it is.');
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

