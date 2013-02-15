<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ViewOutstandingAccounts.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.ViewOutstandingAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="ViewOutstandingAccountsHeader" Text="XYZ View Outstanding" />&nbsp;<asp:DropDownList runat="server" ID="DropAccounts"/>&nbsp;<asp:DropDownList runat="server" ID="DropYears"/>&nbsp;<img alt="Loading" src="/Images/Abstract/ajaxloader-blackcircle.gif" ID="imageLoadIndicator" /></h2>

    <table id="GridOutstandingAccounts" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true,url:'Json-OpenAccounts.aspx'"
        idField="itemId">
        <thead>  
            <tr>  
                <th data-options="field:'ID',width:70"><asp:Label ID="LabelGridHeaderDue" runat="server" Text="ID"/></th>  
                <th data-options="field:'created',width:70,sortable:true"><asp:Label ID="LabelGridHeaderRecipient" runat="server" Text="XYZ Created" /></th>
                <th data-options="field:'due',width:70"><asp:Label ID="LabelGridHeaderBank" runat="server" Text="XYZ Due" /></th>  
                <th data-options="field:'personorg',width:140,sortable:true"><asp:Label ID="LabelGridHeaderAccount" runat="server" Text="XYZ Person / Organization" /></th>
                <th data-options="field:'reference',width:180,sortable:true,order:'asc'"><asp:Label ID="LabelGridHeaderDescription" runat="server" Text="XYZ Description" /></th>
                <th data-options="field:'amount',width:100,align:'right'"><asp:Label ID="LabelGridHeaderAmount" runat="server" Text="XYZ Amount" /></th>
                <th data-options="field:'action',width:43,align:'center'"><asp:Label ID="LabelGridHeaderPaid" runat="server" Text="XYZPaid" /></th>
            </tr>  
        </thead>
    </table>  

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

