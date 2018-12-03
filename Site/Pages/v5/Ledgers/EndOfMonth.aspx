<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeBehind="EndOfMonth.aspx.cs" Inherits="Swarmops.Frontend.Pages.Ledgers.EndOfMonth" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" runat="server">
    
    <script type="text/javascript">


        $(document).ready(function () {

            var rowCount = 0;

            <%=this.JavascriptDocReady%>

            $('img.eomitem-document').attr('src', '/Images/Icons/iconshock-balloon-invoice-128x96.png');

            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="itemGroupHeader">Upload&nbsp;external&nbsp;data&nbsp;and&nbsp;match&nbsp;accounts</span>',
                itemId: 'GroupExternal'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 2,
                type: 'body',
                field: 'itemGroupName'
            });


            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Upload/Fetch bank transaction data (FORMAT) up until [lastdatelastmonth]',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Upload bank statement (PDF) for [lastmonth]',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Resolve unmatched ledger transactions',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="itemGroupHeader">Taxes&nbsp;and&nbsp;Payroll</span>',
                itemId: 'GroupTaxesPayroll'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 2,
                type: 'body',
                field: 'itemGroupName'
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'VAT Report for [lastmonth]',
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            /*

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Payroll processing for [thismonth]',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                actions: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Submit to tax authorities',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                actions: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });

            */

            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="itemGroupHeader">Annual&nbsp;Reports</span>',
                itemId: 'GroupAnnual'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 4,
                type: 'body',
                field: 'itemGroupName'
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Close ledgers for [year]',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });


            $('#TableEomItems').datagrid('appendRow', {
                itemGroupName: '<span class="itemGroupHeader">Send&nbsp;to&nbsp;Accountants,&nbsp;Shareholders,&nbsp;etc.</span>',
                itemId: 'SendReports'
            });

            rowCount = $('#TableEomItems').datagrid('getRows').length;

            $('#TableEomItems').datagrid('mergeCells', {
                index: rowCount - 1,
                colspan: 4,
                type: 'body',
                field: 'itemGroupName'
            });

            $('#TableEomItems').datagrid('appendRow', {
                itemName: 'Send all reports as required',
                docs: "<img src='/Images/Icons/iconshock-red-cross-sphere-128x96px.png' data-test-id='Sockets-Browser' class='test-failed' style='display:none' height='20px' />",
                action: "<img src='/Images/Icons/iconshock-yellow-sphere-30pct-128x96px.png' data-test-id='Sockets-Browser' class='test-running' style='display:inline' height='20px' />"
            });


        });


        // Function: Generate VAT report

        // Function: Match all mismatched transactions

        // Function: Upload bank statement PDF for accountId x

        // Function: Close YEAR if a new year

        // Function: Send EOM papers to accountants etc


    </script>

    <style type="text/css">
        .itemGroupHeader {
            font-size: 125%;
            font-weight: 500;
        }

        .datagrid-row-selected,.datagrid-row-over{
            background:transparent;
        }
    </style>


</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2>End-of-Month routine</h2>

    <table id="TableEomItems" class="easyui-datagrid" style="width:680px;height:500px"
        data-options="rownumbers:false,singleSelect:false,nowrap:false,fit:false,loading:false,selectOnCheck:true,checkOnSelect:true"
        idField="itemId">
        <thead>
            <tr>
                <th data-options="field:'itemGroupName',width:42">&nbsp;</th>
                <th data-options="field:'itemName',width:562">Todo</th>
                <th data-options="field:'action',width:55,align:'center'">Action</th>
            </tr>  
        </thead>
    </table>  
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

