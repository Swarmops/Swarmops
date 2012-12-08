﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ProfitLossStatement.aspx.cs" Inherits="Pages_v5_Ledgers_ProfitLossStatement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://hostedscripts.falkvinge.net/easyui/jquery.easyui.min.js" type="text/javascript"></script>
	<link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/icon.css">
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/tree.css"/>
    <link rel="stylesheet" type="text/css" href="https://hostedscripts.falkvinge.net/easyui/themes/default/datagrid.css"/>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div style="float:right"><asp:DropDownList runat="server" ID="DropYears"/></div>
    <h2>P&L for Piratpartiet For Year</h2>
    <table id="test" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="Json-ProfitLossData.aspx"
        rownumbers="false"
        animate="true"
        idField="id" treeField="name">
    <thead>  
        <tr>  
            <th field="name" width="195">Account</th>  
            <th field="lastYear" width="80" align="right">Last Year</th>  
            <th field="q1" width="80" align="right">Q1</th>
            <th field="q2" width="80" align="right">Q2</th>
            <th field="q3" width="80" align="right">Q3</th>  
            <th field="q4" width="80" align="right">Q4</th>
            <th field="ytd" width="80" align="right">YTD</th>
        </tr>  
    </thead>  
</table> 
<div style="clear:both"></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

