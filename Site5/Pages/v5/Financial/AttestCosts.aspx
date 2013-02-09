<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AttestCosts.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Financial.AttestCosts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://hostedscripts.falkvinge.net/easyui/jquery.easyui.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/datagrid.css"/>
    
    <script type="text/javascript">
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-medium.gif',
            '/Images/Icons/iconshock-redcross-16px.png',
            '/Images/Icons/iconshock-redcross-16px-desat.png',
            '/Images/Icons/iconshock-greentick-16px.png',
            '/Images/Icons/iconshock-greentick-16px-desat.png',
            '/Images/Icons/iconshock-glass-16px-hot.png'
        ]);

        $(document).ready(function () {
            $('#TableAttestableCosts').datagrid(
                {
                    onLoadSuccess: function () {
                        $(".LocalIconApproval").attr("src", "/Images/Icons/iconshock-greentick-16px-desat-light.png");
                        $(".LocalIconApproved").attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                        $(".LocalIconApproved, .LocalIconDenied").css("display", "none");
                        $(".LocalIconDenial").attr("src", "/Images/Icons/iconshock-redcross-16px-desat-light.png");
                        $(".LocalIconApproval, .LocalIconApproved, .LocalIconDenial").css("cursor", "pointer");

                        $(".LocalIconApproval").mouseover(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-greentick-16px-desat.png");
                            }
                        });
                        $(".LocalIconApproval").mouseout(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("src", "/Images/Icons/iconshock-greentick-16px-desat-light.png");
                            }
                        });
                        $(".LocalIconApproval").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $("#IconDenial" + $(this).attr("baseid")).fadeTo(1000, 0.01).css("cursor","default");
                                $(this).fadeOut(2000, function () {
                                    $(this).css("display", "none");
                                    $(this).attr("src", "/Images/Icons/iconshock-greentick-16px-desat-light.png");
                                    $(this).attr("rel", "active");
                                    $("#IconApproved" + $(this).attr("baseid")).fadeIn(100);
                                    alertify.success("Foo approved.");
                                });

                            }
                        });
                        $(".LocalIconApproved").click(function () {
                            if ($(this).attr("rel") != "loading") {
                                $(this).attr("rel", "loading");
                                $(this).attr("src", "/Images/Abstract/ajaxloader-medium.gif");
                                $(this).fadeOut(2000, function () {
                                    $(this).css("display", "none");
                                    $(this).attr("src", "/Images/Icons/iconshock-greentick-16px.png");
                                    $(this).attr("rel", "");
                                    $("#IconApproval" + $(this).attr("baseid")).fadeIn(100);
                                    $("#" + $(this).attr("rel"), "");
                                    alertify.log("Foo de-attested.");
                                    $("#IconDenial" + $(this).attr("baseid")).fadeTo(100, 1).css("cursor", "pointer");
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
    <h2><asp:Label runat="server" ID="LabelAttestCostsHeader" Text="Attest Costs XYZ" /></h2>
    <table id="TableAttestableCosts" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-AttestableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'item',width:60">Item</th>  
                <th data-options="field:'beneficiary',width:120,sortable:true">Beneficiary</th>  
                <th data-options="field:'description',width:160">Description</th>  
                <th data-options="field:'budgetName',width:160,sortable:true">Budget</th>
                <th data-options="field:'amountRequested',width:80,align:'right',sortable:true,order:'asc'">Requested</th>
                <th data-options="field:'dox',width:40,align:'center'">Docs</th>
                <th data-options="field:'actions',width:53,align:'center'">Action</th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

