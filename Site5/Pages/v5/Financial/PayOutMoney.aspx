<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="PayOutMoney.aspx.cs" Inherits="Swarmops.Frontend.Pages.Financial.PayOutMoney" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
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
            '/Images/Icons/iconshock-balloon-undo-16px-hot.png',
            '/Images/Icons/iconshock-greentick-16px.png',
            '/Images/Icons/iconshock-redcross-16px.png',
            '/Images/Icons/iconshock-balloon-undo-16px.png'
        ]);

        $(document).ready(function () {

            $('#CheckOptionsShowPrevious').change(function() {
                var checked = $('#CheckOptionsShowPrevious').prop('checked');

                if (checked) {
                    $('.rowPrevious').show();
                } else {
                    $('.rowPrevious').hide();
                }
            });

            $('#TablePayableCosts').datagrid(
                {
                    rowStyler: function (index, rowData) {
                        if (rowData.databaseId != null) {
                            return { class: "rowPrevious row" + rowData.itemId.replace(/\|/g, '') };
                        }

                        if (rowData.itemId != null) {
                            return { class: "row" + rowData.itemId.replace(/\|/g, '') };
                        }

                        return '';
                    },

                    onLoadSuccess: function () {
                        $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                        $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-balloon-no-16px-disabled.png");
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                        $(".LocalIconDenied").attr("src", "/Images/Icons/iconshock-cross-16px.png");
                        $(".LocalIconUndo").attr("src", "/Images/Icons/iconshock-balloon-undo-16px.png");
                        $(".LocalIconApproved.LocalPrototype, .LocalIconUndo.LocalPrototype, .LocalIconDenied.LocalPrototype, .LocalIconApproval.LocalPrevious, .LocalIconDenial.LocalPrevious, .LocalIconDenied.LocalPrevious").css("display", "none");
                        $(".LocalIconApproval, .LocalIconUndo").css("cursor", "pointer");

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

                        $(".LocalIconUndo").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-16px-hot.png");
                            }
                        });

                        $(".LocalIconUndo").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-16px.png");
                            }
                        });

                        $(".LocalIconApproval").click(function () {
                            if ($(this).attr("rel") != "loading") {

                                var jsonData = {};
                                jsonData.protoIdentity = $(this).attr("baseid");

                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $("#IconDenial" + $(this).attr("baseid").replace(/\|/g,'')).fadeTo(1000, 0.01);
                                
                                $.ajax({
                                    type: "POST",
                                    url: "/Pages/v5/Financial/PayOutMoney.aspx/ConfirmPayout",
                                    data: $.toJSON(jsonData),
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: $.proxy(function (msg) {
                                        var baseid = $(this).attr("baseid").replace(/\|/g, '');
                                        $(this).attr("databaseid", msg.d.AssignedId);
                                        $(this).attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                                        $(this).attr("rel", "active");
                                        $(this).hide();
                                        $("#IconApproved" + baseid).fadeIn(100);
                                        $("#IconDenial" + baseid).css('opacity', 1.0).hide();
                                        $("#IconUndo" + baseid).fadeIn(100);
                                        $('.row' + baseid).animate({ color: "#888" }, 400);
                                        alertify.success(msg.d.DisplayMessage);
                                    }, this)
                                });
                            }
                        });

                        $(".LocalIconUndo").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                var jsonData = {};
                                jsonData.databaseId = $("#IconApproval" + $(this).attr("baseid").replace(/\|/g, '')).attr("databaseid");

                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $("#IconApproved" + $(this).attr("baseid").replace(/\|/g, '')).fadeTo(1000, 0.01);

                                $.ajax({
                                    type: "POST",
                                    url: "/Pages/v5/Financial/PayOutMoney.aspx/UndoPayout",
                                    data: $.toJSON(jsonData),
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: $.proxy(function (msg) {
                                        if (msg.d.Success) {
                                            var baseid = $(this).attr("baseid").replace(/\|/g, '');
                                            $(this).attr("src", "/Images/Icons/iconshock-balloon-undo-16px.png");
                                            $(this).attr("rel", "");
                                            $(this).hide();
                                            $("#IconApproved" + baseid).css('opacity', 1.0).hide();
                                            $("#IconApproval" + baseid).fadeIn(100);
                                            $("#IconDenial" + baseid).fadeIn(100);
                                            $('.row' + baseid).animate({ color: "#000" }, 100);
                                            alertify.log(msg.d.DisplayMessage);
                                        } else {
                                            $(this).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                            alertify.error(msg.d.DisplayMessage);
                                            // TODO: Add alert box?
                                        }
                                    }, this)
                                });

                            }
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
        .LocalIconApproval, .LocalIconApproved {
            padding-right: 3px;
        }
        body.rtl .LocalIconApproval, body.rtl .LocalIconApproved {
            padding-left: 3px;
            padding-right: initial;
        }
        .rowPrevious {
            color: #888;
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelPayOutMoneyHeader" Text="XYZ Costs Awaiting Payment" /></h2>
    <table id="TablePayableCosts" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,fitColumns:true,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-PayableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'due',width:70"><asp:Label ID="LabelGridHeaderDue" runat="server" Text="XYZ Due"/></th>  
                <th data-options="field:'recipient',width:150,sortable:true"><asp:Label ID="LabelGridHeaderRecipient" runat="server" Text="XYZ Beneficiary" /></th>
                <th data-options="field:'bank',width:70"><asp:Label ID="LabelGridHeaderBank" runat="server" Text="XYZ Bank" /></th>  
                <th data-options="field:'account',width:120,sortable:true"><asp:Label ID="LabelGridHeaderAccount" runat="server" Text="XYZ Account" /></th>
                <th data-options="field:'reference',width:130,sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderReference" runat="server" Text="XYZ Reference" /></th>
                <th data-options="field:'amount',width:80,align:'right'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                <th data-options="field:'action',width:53,align:'center'"><asp:Label ID="LabelGridHeaderPaid" runat="server" Text="XYZPaid" /></th>
            </tr>  
        </thead>
    </table>  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue"><asp:Label ID="LabelSidebarOptions" Text="Options" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content" style="margin-left:5px">
            <div class="link-row-encaps" style="cursor:default; margin-left:2px">
               <span style="position:relative;top:2px;left:1px"><input type="checkbox" id="CheckOptionsShowPrevious" /></span>&nbsp;<label for="CheckOptionsShowPrevious"><asp:Label ID="LabelOptionsShowPrevious" runat="server" Text="Show previous payouts XYZ"/></label>
            </div>
        </div>
    </div>

</asp:Content>

