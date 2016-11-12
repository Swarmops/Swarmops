<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="AuditGeneralLedger.aspx.cs" Inherits="Pages_v4_Financial_AuditGeneralLedger" EnableViewState="true" Title="Audit General Ledger - PirateWeb" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTree.ascx" TagName="FinancialAccountTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/Financial/Ledger.ascx" TagName="Ledger" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server" />

<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function ShowTransactionForm(id, rowIndex) {
                window.radopen("PopupAuditTransaction.aspx?TransactionId=" + id, "TransactionForm");
                return false;
            }


            function ShowTransactionFormDelayed(transactionId) {
                setTimeout('window.radopen("PopupAuditTransaction.aspx?TransactionId=' + transactionId + '", "TransactionForm");', 200);
                return false;
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("Rebind");
                }
                else {
                    $find("<%=RadAjaxManager1.ClientID%>").ajaxRequest("RebindAndNavigate");
                }
            }
        </script>

    </telerik:RadCodeBlock>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20" Style="z-index: 7001">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="TransactionForm" runat="server"
                Title="Manage Transaction" Height="500px" Width="550px" Left="150px" Top="140px" ReloadOnShow="true"
                Modal="true" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"
        SkinID="Web20">
    </telerik:RadAjaxManager>



<pw4:PageTitle Icon="book_key.png" Title="Audit General Ledger" Description="View every financial transaction in the ledger" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select organization, time period, and accounts</span><br />
    <div class="DivGroupBoxContents">
        <div style="float:left">Organization:&nbsp;<br />From date:<br />To date:</div>
        <div style="float:left"><asp:DropDownList ID="DropOrganizations" runat="server" ><asp:ListItem Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br /><telerik:RadDatePicker ID="DateStart" runat="server" Skin="Web20" />&nbsp;<br /><telerik:RadDatePicker ID="DateEnd" runat="server" Skin="Web20" />&nbsp;</div>
        <div style="margin-left:10px;border-left: solid 1px #660087;padding-left:10px;float:left">
            <asp:RadioButton Text="All Accounts" ID="RadioAllAccounts" GroupName="Accounts" runat="server" />&nbsp;<br />
            <asp:RadioButton Text="Account Group" ID="RadioAccountGroup" GroupName="Accounts" runat="server" />&nbsp;<br />
            <asp:RadioButton Text="Specific account:" runat="server" ID="RadioSpecificAccount" GroupName="Accounts" />&nbsp;
        </div>
        <div style="float:left;margin-right:10px;border-right:solid 1px #660087;padding-right:10px">&nbsp;<br />
            <asp:DropDownList ID="DropAccountGroups" runat="server" />&nbsp;<br />
            <pw4:FinancialAccountTreeDropDown ID="DropAccounts" runat="server" />&nbsp;
        </div>
        <div style="float:right">
            &nbsp;<br />&nbsp;<br />&nbsp;<asp:Button Text="Filter" ID="ButtonFilter" 
                runat="server" onclick="ButtonFilter_Click" />
        </div>
        <asp:RadioButton Text="All transactions" ID="RadioAllTransactions" runat="server" GroupName="Transactions" />&nbsp;<br />
        <asp:RadioButton Text="Credits" ID="RadioCreditingTransactions" GroupName="Transactions" runat="server" />&nbsp;<br />
        <asp:RadioButton Text="Credits > 1,000" ID="RadioCreditingTransactionsLarge" GroupName="Transactions" runat="server" />&nbsp;
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">General Ledger for Piratpartiet SE</span>
      <div class="DivGroupBoxContents">

        <pw4:Ledger ID="Ledger" runat="server" />    
    
    </div>
    </div>

</asp:Content>

