<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="MailServerAccounts.aspx.cs" Inherits="Pages_v4_Admin_MailServerAccounts"
    Title="Edit mail accounts on mail.piratpartiet.se" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function ShowEditForm(id) {

                window.radopen("PopupEditMailAccount.aspx?state=Edit&account=" + escape(id), "EditAccountForm");
                return false;
            }

            function ShowAddForm(id) {

                window.radopen("PopupEditMailAccount.aspx?state=Add&account=" + escape(id), "EditAccountForm");
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
            <telerik:RadWindow Skin="Web20" ID="EditAccountForm" runat="server" Title="" Height="450px"
                Width="700px" Left="50px" ReloadOnShow="true" Modal="true" Behaviors="Resize,Move,Close" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonFind">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonAdd">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ButtonAdd" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Find Account</span><br />
            <div class="DivGroupBoxContents">
                <div style="float: left">
                    Account name&nbsp;&nbsp;</div>
                &nbsp;
                <asp:TextBox ID="TextBoxFindAccount" runat="server" Width="253px"></asp:TextBox>
                &nbsp;
                <asp:Button ID="ButtonFind" runat="server" Text="Find" OnClick="ButtonFind_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="ButtonAdd" runat="server" Text="Add new account" />
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Result</span><br />
            <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" GridLines="None"
                OnItemDataBound="RadGrid1_ItemDataBound" AllowPaging="True" OnNeedDataSource="RadGrid1_NeedDataSource"
                PageSize="20">
                <MasterTableView>
                    <RowIndicatorColumn>
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn>
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Account" UniqueName="Account" DataField="account">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Forwarded to" UniqueName="ForwardedTo">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="ForwardedFrom" UniqueName="ForwardedFrom">
                        </telerik:GridBoundColumn>
                        
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </div>
</asp:Content>
