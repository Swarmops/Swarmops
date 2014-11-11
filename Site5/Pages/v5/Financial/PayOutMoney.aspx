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
            '/Images/Icons/iconshock-greentick-16px.png',
            '/Images/Icons/iconshock-redcross-16px.png',
            '/Images/Icons/undo-16px.png'
        ]);

        $(document).ready(function () {
            $('#TablePayableCosts').datagrid(
                {
                    onLoadSuccess: function () {
                        $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                        $(".LocalIconApproved").css("display", "none");
                        $(".LocalIconApproval, .LocalIconApproved").css("cursor", "pointer");

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
                                $("#IconDenial" + $(this).attr("baseid").replace(/\|/g,'')).fadeTo(1000, 0.01).css("cursor", "default");
                                var thisIcon = this;
                                $.ajax({
                                    type: "POST",
                                    url: "PayOutMoney.aspx/ConfirmPayout",
                                    data: "{'protoIdentity': '" + escape($(this).attr("baseid")) + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        $(thisIcon).attr("databaseid", msg.d.AssignedId);
                                        $(thisIcon).css("display", "none");
                                        $(thisIcon).attr("src", "/Images/Icons/iconshock-balloon-yes-16px.png");
                                        $(thisIcon).attr("rel", "active");
                                        $("#IconApproved" + $(thisIcon).attr("baseid").replace(/\|/g, '')).fadeIn(100);
                                        alertify.success(unescape(msg.d.DisplayMessage));
                                    }
                                });
                            }
                        });

                        $(".LocalIconApproved").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                var thisIcon = this;
                                
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $.ajax({
                                    type: "POST",
                                    url: "PayOutMoney.aspx/UndoPayout",
                                    data: "{'databaseId': '" + $("#IconApproval" + $(thisIcon).attr("baseid").replace(/\|/g, '')).attr("databaseid") + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        if (msg.d.Success) {
                                            $(thisIcon).css("display", "none");
                                            $(thisIcon).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                            $(thisIcon).attr("rel", "");
                                            $("#IconApproval" + $(thisIcon).attr("baseid").replace(/\|/g, '')).fadeIn(100);
                                            $("#" + $(thisIcon).attr("rel"), "");
                                            alertify.log(unescape(msg.d.DisplayMessage));
                                        } else {
                                            $(thisIcon).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                            alertify.error(unescape(msg.d.DisplayMessage));
                                            // TODO: Add alert box?
                                        }
                                    }
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelPayOutMoneyHeader" Text="XYZ Costs Awaiting Payment" /></h2>
    <table id="TablePayableCosts" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-PayableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'due',width:70"><asp:Label ID="LabelGridHeaderDue" runat="server" Text="XYZ Due"/></th>  
                <th data-options="field:'recipient',width:150,sortable:true"><asp:Label ID="LabelGridHeaderRecipient" runat="server" Text="XYZ Beneficiary" /></th>
                <th data-options="field:'bank',width:70"><asp:Label ID="LabelGridHeaderBank" runat="server" Text="XYZ Bank" /></th>  
                <th data-options="field:'account',width:120,sortable:true"><asp:Label ID="LabelGridHeaderAccount" runat="server" Text="XYZ Account" /></th>
                <th data-options="field:'reference',width:140,sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderReference" runat="server" Text="XYZ Reference" /></th>
                <th data-options="field:'amount',width:80,align:'right'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                <th data-options="field:'action',width:43,align:'center'"><asp:Label ID="LabelGridHeaderPaid" runat="server" Text="XYZPaid" /></th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

