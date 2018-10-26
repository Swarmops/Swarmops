<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Admin.Troubleshooting.DebugSockets" Codebehind="Sockets.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
   
    <script type="text/javascript"> 
    

        $(document).ready(function () {
            $('#TableTestResults').datagrid('appendRow', {
                testName: 'Test Name',
                red: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' style='display:inline' height='20px' />",
                yellow: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' style='display:inline' height='20px' />",
                green: "<img src='/Images/Icons/iconshock-green-tick-sphere-128x96px.png' style='display:inline' height='20px' />"
            });

            <%=this.ProgressTests.ClientID%>_begin();
        });


    </script>

    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2>Troubleshooting Sockets...</h2>
    
    <Swarmops5:ProgressBar ID="ProgressTests" runat="server"/>

    <table id="TableTestResults" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true"
        idField="testId">
        <thead>
            <tr>
                <th data-options="field:'testGroup',width:54">Group</th>
                <th data-options="field:'testName',width:500">Test</th>
                <th data-options="field:'red',width:42,align:'center'"><img src="/Images/Icons/iconshock-red-cross-128x96px.png" height="20px" style="position: relative; top: 2px"/></th>  
                <th data-options="field:'yellow',width:42,align:'center'">&nbsp;</th>
                <th data-options="field:'green',width:42,align:'center'"><img src="/Images/Icons/iconshock-green-tick-128x96px.png" height="20px"style="position: relative; top: 2px" /></th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

