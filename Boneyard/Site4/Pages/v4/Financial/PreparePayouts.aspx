<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="PreparePayouts.aspx.cs" Inherits="Pages_v4_Financial_PreparePayouts" Title="Validate Expense Claim Documentation - PirateWeb" %>
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

    <pw4:PageTitle Icon="money.png" Title="Prepare Payouts" Description="List payments due from the organization and mark them as filed for payment with the bank" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Unprepared payouts for Piratpartiet SE</span><br />
    <div class="DivGroupBoxContents">
    <telerik:RadGrid ID="GridPayouts" runat="server" 
            OnItemCreated="GridPayouts_ItemCreated" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" GridLines="None" AllowMultiRowSelection="true"
            PageSize="20" onneeddatasource="GridPayouts_NeedDataSource">
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="ProtoIdentity">
            <Columns>
                <telerik:GridClientSelectColumn HeaderText="OK?" UniqueName="ColumnCheckOk" />
                <telerik:GridTemplateColumn HeaderText="Payment due" UniqueName="ColumnDueDate">
                    <ItemTemplate>
                        <asp:Label ID="LabelDueDate" runat="server" Text="Programmatic" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Recipient" DataField="Recipient" UniqueName="columnRecipient" />
                <telerik:GridBoundColumn HeaderText="Bank" DataField="Bank" UniqueName="column3" />
                <telerik:GridBoundColumn HeaderText="Account" DataField="Account" UniqueName="columnDescription" />
                <telerik:GridBoundColumn HeaderText="Reference" DataField="Reference" UniqueName="columnReference" />
                <telerik:GridBoundColumn HeaderText="Amount" DataField="Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N2}" UniqueName="column4" />
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
        
        <p><asp:Button ID="ButtonPay" runat="server" Text="Selected are paid" 
                onclick="ButtonPay_Click" /> This certifies you have filed these payouts with the organization's bank.</p></div>
    </div>
    </div>


</asp:Content>

