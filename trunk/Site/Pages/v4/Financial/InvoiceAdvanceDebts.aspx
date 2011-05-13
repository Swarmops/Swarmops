﻿<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="InvoiceAdvanceDebts.aspx.cs" Inherits="Pages_v4_Financial_InvoiceAdvanceDebts" Title="Invoice unpaid advance debts - PirateWeb" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="money-red.png" Title="Invoice Advance Debts" Description="List advance payments that haven't been settled" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Advance debts to Piratpartiet SE</span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridDebts" runat="server" 
            OnItemCreated="GridPayouts_ItemCreated" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
            PageSize="20" onneeddatasource="GridPayouts_NeedDataSource">
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="ProtoIdentity">
            <Columns>
                <telerik:GridClientSelectColumn HeaderText="Invoice?" UniqueName="ColumnCheckOk" />
                <telerik:GridBoundColumn HeaderText="Debtor" DataField="PersonCanonical" UniqueName="columnRecipient" />
                <telerik:GridBoundColumn HeaderText="Debt" DataField="DebtDecimal" UniqueName="column3" DataFormatString="{0:N2}" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn HeaderText="Claims" DataField="IdentitiesString" UniqueName="columnDescription" />
                <telerik:GridBoundColumn HeaderText="From Date" DataField="EarliestDate" UniqueName="columnDescription" DataFormatString="{0:yyyy-MM-dd}" />
                <telerik:GridTemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:HyperLink ID="LinkPartialInvoice" runat="server">Partial invoice...</asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>

            <ExpandCollapseColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>
        </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="True" />
            </ClientSettings>

        </telerik:RadGrid>
        
        <p><asp:Button ID="ButtonInvoice" runat="server" Text="Invoice selected debts" 
                onclick="ButtonInvoice_Click" /> Send invoices to regulate the debts checked above.</p></div>
    </div>
    </div>


</asp:Content>

