<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OutboundInvoiceGrid.ascx.cs" Inherits="Controls_v4_OutboundInvoiceGrid" %>

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

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function RadAjaxManager_OnResponseEnd(sender, eventArgs) {
            }

            function ShowInboundInvoiceForm(id, rowIndex) {
                var grid = $find("<%=GridInvoices.ClientID%>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("PopupEditInboundInvoice.aspx?InboundInvoiceId=" + id, "InboundInvoiceForm");
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

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Web20" Height="16px">
        <Windows>
            <telerik:RadWindow VisibleStatusbar="false" Skin="Web20" ID="InboundInvoiceForm" runat="server"
                Title="Edit Inbound Invoice" Height="450px" Width="500px" Left="150px" ReloadOnShow="true"
                Modal="true" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline">
        <ClientEvents OnResponseEnd="RadAjaxManager_OnResponseEnd" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="GridInvoices" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        Style="position: absolute; left: 0px" meta:resourcekey="RadAjaxLoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

                <telerik:RadGrid ID="GridInvoices" runat="server" GridLines="None" 
                    onitemcreated="GridInvoices_ItemCreated" >
<MasterTableView autogeneratecolumns="False" DataKeyNames="Identity">
    <Columns>
        <telerik:GridBoundColumn HeaderText="Invoice#" UniqueName="column1" DataField="Identity" />
        <telerik:GridBoundColumn HeaderText="Created" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column2" DataField="CreatedDateTime" />
        <telerik:GridBoundColumn HeaderText="Payment Due" DataFormatString="{0:yyyy-MM-dd}" UniqueName="columnDue" DataField="DueDate" />
        <telerik:GridBoundColumn HeaderText="Customer" UniqueName="column3" DataField="CustomerName" />
        <telerik:GridBoundColumn HeaderText="Amount" 
            UniqueName="column" DataFormatString="{0:N2}" DataField="Amount">
            <HeaderStyle HorizontalAlign="Right" />
            <ItemStyle HorizontalAlign="Right" />
        </telerik:GridBoundColumn>
        <telerik:GridTemplateColumn HeaderText="Paid/Closed" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn2">
            <ItemTemplate>
                <asp:Image ID="ImagePaidClosed" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" UniqueName="TemplateColumn">
            <ItemTemplate>
                <asp:HyperLink ID="LinkEdit" Target="_blank" runat="server">View...</asp:HyperLink>
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

