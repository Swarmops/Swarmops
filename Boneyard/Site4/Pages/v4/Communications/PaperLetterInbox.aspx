<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="PaperLetterInbox.aspx.cs" Inherits="Pages_v4_Communications_PaperLetterInbox" Title="Paper Letters To Me - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="table.png" Title="Paper Letter Inbox" Description="View received paper letters addressed to me"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">My paper letter inbox</span><br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridLetters" runat="server" GridLines="None" 
                    onitemcreated="GridLetters_ItemCreated" >
<MasterTableView autogeneratecolumns="False" DataKeyNames="Identity">
    <Columns>
        <telerik:GridBoundColumn HeaderText="Letter#" UniqueName="column1" DataField="Identity" />
        <telerik:GridBoundColumn HeaderText="Received" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column2" DataField="ReceivedDate" />
        <telerik:GridBoundColumn HeaderText="From" UniqueName="column3" DataField="FromName" />
        <telerik:GridTemplateColumn HeaderText="Address" UniqueName="ColumnAddress">
            <ItemTemplate>
                <asp:Label ID="LabelAddress" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Scans of letter" UniqueName="ColumnDox">
            <ItemTemplate>
                <pw4:DocumentList runat="server" ID="DocumentList" UseShortForm="true" />
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
                </telerik:RadGrid>
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Letters to the organization (Piratpartiet SE)</span><br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="RadGrid1" runat="server" GridLines="None" 
                    onitemcreated="GridLetters_ItemCreated" >
<MasterTableView autogeneratecolumns="False" DataKeyNames="Identity">
    <Columns>
        <telerik:GridBoundColumn HeaderText="Letter#" UniqueName="column1" DataField="Identity" />
        <telerik:GridBoundColumn HeaderText="Received" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column2" DataField="ReceivedDate" />
        <telerik:GridBoundColumn HeaderText="From" UniqueName="column3" DataField="FromName" />
        <telerik:GridTemplateColumn HeaderText="Address" UniqueName="ColumnAddress">
            <ItemTemplate>
                <asp:Label ID="LabelAddress" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Scans of letter" UniqueName="ColumnDox">
            <ItemTemplate>
                <pw4:DocumentList runat="server" ID="DocumentList" UseShortForm="true" />
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
                </telerik:RadGrid>
            </div>
        </div>
    </div>


</asp:Content>

