<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Admin.Debug.Sockets" Codebehind="DebugSockets.aspx.cs" %>
<%@ Register src="~/Controls/v5/Base/ComboGeographies.ascx" tagname="ComboGeographies" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
   
    <script type="text/javascript"> 
    

        $(document).ready(function () {

        });


    </script>

    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields" style="padding-top:4px">
        &nbsp;
    </div>
    <div class="entryLabels" style="padding-top:10px">
        &nbsp;
    </div>
    <h2 style="padding-top:15px"><asp:Label ID="LabelMatchingPeopleInX" runat="server" /> (<span id="spanHitCount">0</span>)</h2>
    <table id="TableSearchResults" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true"
        idField="itemId">
        <thead>
            <tr>
                <th data-options="field:'name',width:210"><asp:Label ID="LabelGridHeaderName" runat="server" Text="XYZ Name"/></th>  
                <th data-options="field:'geographyName',width:150,sortable:true"><asp:Label ID="LabelGridHeaderGeography" runat="server" Text="XYZ Geography" /></th>
                <th data-options="field:'mail',width:105,sortable:true"><asp:Label ID="LabelGridHeaderMail" runat="server" Text="XYZ Mail" /></th>  
                <th data-options="field:'phone',width:100,sortable:true"><asp:Label ID="LabelGridHeaderPhone" runat="server" Text="XYZ Phone" /></th>
                <th data-options="field:'notes',width:50"><asp:Label ID="LabelGridHeaderNotes" runat="server" Text="XYZ Notes" /></th>
                <th data-options="field:'actions',width:43,align:'center'"><asp:Label ID="LabelGridHeaderAction" runat="server" Text="XYZ Actions" /></th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

