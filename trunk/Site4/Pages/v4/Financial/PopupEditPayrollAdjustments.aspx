<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PopupEditPayrollAdjustments.aspx.cs" Inherits="Pages_v4_PopupEditPayrollAdjustments" %>
<%@ Import Namespace="Activizr.Basic.Types"%>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Page Title Is Set In Page_Load()</title>
    <link href="/Style/PirateWeb-v4.css" rel="stylesheet" type="text/css" />    
    <script type="text/javascript">
        function CloseAndRebind(args) {
            GetRadWindow().Close();
            GetRadWindow().BrowserWindow.refreshGrid(args);
        }
		
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz as well)
				
            return oWindow;
        }

        function CancelEdit()
        {
            GetRadWindow().Close();		
        }
    </script>
</head>

<body style="background-color: #f8e7ff; padding-left:10px; padding-top:10px; padding-right: 10px; padding-bottom: 10px">
    <form id="form1" runat="server" method="post">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    Payroll:&nbsp;&nbsp;<asp:Label ID="LabelPayroll" runat="server" /><br />
    Month:&nbsp;&nbsp;<asp:Label ID="LabelMonth" runat="server" /><br />
    <br />
    <telerik:RadGrid ID="GridProjectedPayroll" runat="server" 
            AutoGenerateColumns="False" GridLines="None" Skin="Web20">
        <HeaderContextMenu>
            <CollapseAnimation Duration="200" Type="OutQuint" />
        </HeaderContextMenu>
        <MasterTableView ShowFooter="false">
            <RowIndicatorColumn>
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="Description" HeaderText="Item" 
                    UniqueName="columnDescript">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"
                    UniqueName="columnDescript">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <FilterMenu>
            <CollapseAnimation Duration="200" Type="OutQuint" />
        </FilterMenu>
    </telerik:RadGrid>
    <br />
    
    Add adjustment:<br />
    <table border="0" cellpadding="0" cellspacing="0">
    <tr><td style="font-size:80%">Description</td><td style="font-size:80%">Amount</td><td style="font-size:80%">Gross/Net</td><td>&nbsp;</td></tr>
    <tr>
        <td><asp:TextBox ID="TextDescription" runat="server" />&nbsp;</td>
        <td><asp:TextBox ID="TextAmount" runat="server"/>&nbsp;</td>
        <td>
            <asp:DropDownList ID="DropType" runat="server">
                <asp:ListItem Value="0" Selected="True">-- Select --</asp:ListItem>
                <asp:ListItem Value="1">Gross (before tax)</asp:ListItem>
                <asp:ListItem Value="2">Net (after tax)</asp:ListItem>
            </asp:DropDownList>&nbsp;
        </td>
        <td><asp:Button ID="ButtonAdd" runat="server" Text="Add" OnClick="ButtonAdd_Click" />&nbsp;</td>
    </tr>
    </table>

    <asp:CompareValidator ID="ValidateDropDown" ControlToValidate="DropType" Operator="NotEqual" ValueToCompare="0" runat="server" Display="Dynamic" Text="Select an adjustment type." />
    <asp:RequiredFieldValidator ID="ValidateDescription" ControlToValidate="TextDescription" runat="server" Display="Dynamic" Text="Enter a description." />
    <asp:RequiredFieldValidator ID="ValidateAmount" ControlToValidate="TextAmount" runat="server" Display="Dynamic" Text="Enter an amount." />



    </form>
</body>
</html>
