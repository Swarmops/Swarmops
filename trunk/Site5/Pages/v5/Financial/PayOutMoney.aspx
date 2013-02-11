<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="PayOutMoney.aspx.cs" Inherits="Swarmops.Frontend.Pages.Financial.PayOutMoney" %>

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
            '/Images/Icons/iconshock-balloon-yes-16px-hot.png',
            '/Images/Icons/iconshock-balloon-no-16px-hot.png',
            '/Images/Icons/iconshock-greentick-16px.png',
            '/Images/Icons/iconshock-redcross-16px.png',
            '/Images/Icons/undo-16px.png'
        ]);
    </script>

    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelPayOutMoneyHeader" Text="XYZ Costs Awaiting Payment" /></h2>
    <table id="TableAttestableCosts" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-PayableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'due',width:60"><asp:Label ID="LabelGridHeaderDue" runat="server" Text="XYZ Due"/></th>  
                <th data-options="field:'recipient',width:120,sortable:true"><asp:Label ID="LabelGridHeaderRecipient" runat="server" Text="XYZ Beneficiary" /></th>
                <th data-options="field:'bank',width:160"><asp:Label ID="LabelGridHeaderBank" runat="server" Text="XYZ Bank" /></th>  
                <th data-options="field:'account',width:160,sortable:true"><asp:Label ID="LabelGridHeaderAccount" runat="server" Text="XYZ Account" /></th>
                <th data-options="field:'reference',width:80,sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderReference" runat="server" Text="XYZ Reference" /></th>
                <th data-options="field:'amount',width:40,align:'center'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                <th data-options="field:'action',width:53,align:'center'"><asp:Label ID="LabelGridHeaderPaid" runat="server" Text="XYZPaid" /></th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

