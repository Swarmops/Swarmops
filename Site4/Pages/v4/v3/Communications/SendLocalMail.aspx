<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="SendLocalMail.aspx.cs" Inherits="Pages_Communications_SendLocalMail" CodePage="65001" 
    Title="PirateWeb - Send Local Mail" ValidateRequest="false" meta:resourcekey="PageResource1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">

    <script type="text/javascript">
        function doConfirm(elem, txt) {
            //Hack to handle OnClientClick behavior under RAD
            if (confirm(txt)) {
                __doPostBack(elem.name, '')
            }
            return false;
        }
    </script>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="DropRecipients">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadAjaxPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="Table" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DropOrganizations">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadAjaxPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="Table" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DropGeographies">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadAjaxPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="Table" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonPreview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="MainPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" IsSticky="true"
        Style="position: absolute; top: 79px; left: 470px; width: 108px; height: 79px;">
        <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>'
            style="border: 0px;" />
    </telerik:RadAjaxLoadingPanel>
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Send Local Mail" Description="Edit and send mail to members in a geographic area"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <br />
        <asp:Panel ID="MainPanel" runat="server">
            <asp:Label ID="LabelSendToPrompt" runat="server" Text="Send to " meta:resourcekey="LabelSendToPromptResource1"></asp:Label>
            <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" >
                <asp:DropDownList ID="DropRecipients" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropRecipients_SelectedIndexChanged"
                    meta:resourcekey="DropRecipientsResource1">
                    <asp:ListItem Selected="True" Value="Members" meta:resourcekey="ListItemResource1">Members of...</asp:ListItem>
                    <asp:ListItem Value="Officers" meta:resourcekey="ListItemResource2">Officers of...</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                    meta:resourcekey="DropOrganizationsResource1">
                </asp:DropDownList>
                <asp:Label ID="LabelOrganizationsInGeographies1" runat="server" Text="in" meta:resourcekey="LabelOrganizationsInGeographies1Resource1"></asp:Label>
                <asp:DropDownList ID="DropGeographies" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropGeographies_SelectedIndexChanged"
                    meta:resourcekey="DropGeographiesResource1">
                </asp:DropDownList>
            </telerik:RadAjaxPanel>
            <hr />
            <asp:Table ID="Table" runat="server" meta:resourcekey="TableResource1">
                <asp:TableRow meta:resourcekey="TableRowResource1">
                    <asp:TableCell meta:resourcekey="TableCellResource1"> <b>From:</b> </asp:TableCell>
                    <asp:TableCell meta:resourcekey="TableCellResource2">
                        <asp:Label ID="LabelSender" runat="server" Text="Label" meta:resourcekey="LabelSenderResource1"></asp:Label><asp:Label
                            ID="LabelMailAddressSuffix" runat="server" Text="Label" meta:resourcekey="LabelMailAddressSuffixResource1"></asp:Label></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow meta:resourcekey="TableRowResource2">
                    <asp:TableCell meta:resourcekey="TableCellResource3"> <b>To:</b> </asp:TableCell>
                    <asp:TableCell meta:resourcekey="TableCellResource4">
                        <asp:Label ID="LabelSelectedRecipients" runat="server" Text="Members of " meta:resourcekey="LabelSelectedRecipientsResource1"></asp:Label><asp:Label
                            ID="LabelSelectedOrganization" runat="server" Text="Label" meta:resourcekey="LabelSelectedOrganizationResource1"></asp:Label><asp:Label
                                ID="LabelOrganizationsInGeographies2" runat="server" Text=" in " meta:resourcekey="LabelOrganizationsInGeographies2Resource1"></asp:Label><asp:Label
                                    ID="LabelSelectedGeography" runat="server" Text="Label" meta:resourcekey="LabelSelectedGeographyResource1"></asp:Label><asp:Label
                                        ID="LabelRecipientCount" runat="server" Text="Label" meta:resourcekey="LabelRecipientCountResource1"></asp:Label></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server" meta:resourcekey="TableRowResource3">
                    <asp:TableCell runat="server" meta:resourcekey="TableCellResource5">
                        <b>
                            <asp:Literal ID="Literal1" runat="server" Text="Subject" meta:resourcekey="Literal1Resource1"></asp:Literal>:</b>
                    </asp:TableCell>
                    <asp:TableCell runat="server" meta:resourcekey="TableCellResource6">
                        <asp:Label runat="server" Text="Label" ID="LabelSelectedOrganizationMailPrefix" meta:resourcekey="LabelSelectedOrganizationMailPrefixResource1"></asp:Label>:
                        <asp:TextBox ID="TextSubject" runat="server"></asp:TextBox>
                        &nbsp;&nbsp;<asp:RequiredFieldValidator ID="ValidatorTextSubject" ControlToValidate="TextSubject" runat="server" />
                        </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <hr />
            <div style="float: right; margin-left: 20px; border-left: solid 2px #808080; padding-left: 20px;
                width: 500px">
                <asp:Literal ID="LiteralPreview" runat="server" meta:resourcekey="LiteralPreviewResource1"></asp:Literal>
                <br />
            </div>
            <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" Text="Body"></asp:Literal>
            :<br />
            <asp:TextBox ID="TextBody" runat="server" Columns="40" meta:resourcekey="TextBodyResource1"
                Rows="15" TextMode="MultiLine"></asp:TextBox>
            <br />
            <br />
        </asp:Panel>
        <asp:Button ID="ButtonPreview" runat="server" meta:resourcekey="ButtonPreviewResource1"
            OnClick="ButtonPreview_Click" Text="Preview" />
        &nbsp;&nbsp;<asp:Button ID="ButtonSend" runat="server" meta:resourcekey="ButtonSendResource1"
            OnClick="ButtonSend_Click" OnClientClick="return doConfirm(this,'You are sure that you are all finished with this mail, and that it should be placed on the outbound queue now?');"
            Text="Send" />
        <br />
        <br />
        <asp:Literal ID="Literal3" runat="server" Text="Allowed markup" meta:resourcekey="Literal3Resource1"></asp:Literal>:
        <br />
        <b>
            <asp:Literal ID="Literal4" runat="server" Text="Link" meta:resourcekey="Literal4Resource1"></asp:Literal></b>:
        [a href="http://url"]linked text[/a]
        <br />
        <b>
            <asp:Literal ID="Literal5" runat="server" Text="Headings" meta:resourcekey="Literal5Resource1"></asp:Literal></b>:
        [h1]text[/h1], [h2]text[/h2], [h3]text[/h3]
        <br />
        <b>
            <asp:Literal ID="Literal6" runat="server" Text="Other" meta:resourcekey="Literal6Resource1"></asp:Literal></b>:
        [b]bold text[/b], [i]italic text[/i], [blockquote]quoted block[/blockquote]
        <br />
        <b>
            <asp:Literal ID="Literal7" runat="server" Text="New Line" meta:resourcekey="Literal7Resource1"></asp:Literal></b>:
        [br]
    </div>
</asp:Content>
