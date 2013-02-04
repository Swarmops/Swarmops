<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="AttestCosts.aspx.cs" Inherits="Pages_v5_Finance_AttestCosts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://hostedscripts.falkvinge.net/easyui/jquery.easyui.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/datagrid.css"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="LabelAttestCostsHeader" Text="Attest Costs XYZ" /></h2>
    <table id="dg" class="easyui-datagrid" style="width:680px;height:400px"
        data-options="rownumbers:false,singleSelect:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-AttestableCosts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'check',checkbox:true"></th>  
                <th data-options="field:'item',width:60">Item</th>  
                <th data-options="field:'beneficiary',width:120,sortable:true">Beneficiary</th>  
                <th data-options="field:'description',width:160">Description</th>  
                <th data-options="field:'budgetName',width:160,sortable:true">Budget</th>
                <th data-options="field:'amountRequested',width:80,align:'right',sortable:true,order:'asc'">Requested</th>
                <th data-options="field:'wontAttest',width:66,align:'center'">Action</th>
            </tr>  
        </thead>
    </table>  
    <asp:Button runat="server" ID="ButtonAttest" Text="Clickme"/>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

