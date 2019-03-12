<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Admin.Diagnostics" Codebehind="Diagnostics.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
   
    <style type="text/css">
        .testGroupHeader {
            font-size: 125%;
            font-weight: 500;
        }
    </style>

    <script type="text/javascript"> 
    

        $(document).ready(function () {

            <%=this.JavascriptDocReady%>

            $('#TableTestResults').datagrid('appendRow', {
                testGroupName: '<span class="testGroupHeader">Socket&nbsp;tests</span>',
                testId: 'Group'
            });

            var rowCount = $('#TableTestResults').datagrid('getRows').length;

            $('#TableTestResults').datagrid('mergeCells', {
                index: rowCount-1,
                colspan: 5,
                type: 'body',
                field: 'testGroupName'
            });

            $('#TableTestResults').datagrid('appendRow', {
                testName: 'Test Browser',
                red: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                yellow: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />",
                green: "<img src='/Images/Icons/iconshock-green-tick-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-passed' style='display:none' height='20px' />"
            });

            $('#TableTestResults').datagrid('appendRow', {
                testName: 'Test Frontend',
                red: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Frontend' class='test-failed' style='display:none' height='20px' />",
                yellow: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Frontend' class='test-running' style='display:inline' height='20px' />",
                green: "<img src='/Images/Icons/iconshock-green-tick-sphere-128x96px.png' data-test-id='Sockets-Frontend' class='test-passed' style='display:none' height='20px' />"
            });

            $('#TableTestResults').datagrid('appendRow', {
                testName: 'Test Backend',
                red: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Backend' class='test-failed' style='display:none' height='20px' />",
                yellow: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Backend' class='test-running' style='display:inline' height='20px' />",
                green: "<img src='/Images/Icons/iconshock-green-tick-sphere-128x96px.png' data-test-id='Sockets-Backend' class='test-passed' style='display:none' height='20px' />"
            });

            <%=this.ProgressTests.ClientID%>_begin();
            <%=this.ProgressTests.ClientID%>_show();
        });

        function onUITestPassed(testId)
        {
            $('img.test-running[data-test-id="' + testId + '"').fadeOut();
            $('img.test-passed[data-test-id="' + testId + '"').show();
        }

        function onUITestFailed(guid, testId)
        {
            $('img.test-passed[data-test-id="' + testId + '"').fadeOut();
            $('img.test-running[data-test-id="' + testId + '"').fadeOut();
            $('img.test-failed[data-test-id="' + testId + '"').show();
        }

        var uiTestPageGuid = '<%=this.PageGuid%>';

    </script>

    <style type="text/css">
        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
   
    <Swarmops5:ProgressBar ID="ProgressTests" runat="server" Header="Running tests..."/>

    <table id="TableTestResults" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true"
        idField="testId">
        <thead>
            <tr>
                <th data-options="field:'testGroupName',width:54">Group</th>
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

