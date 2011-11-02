<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="AutoMails.aspx.cs" Inherits="Pages_Communications_AutoMails" Title="PirateWeb - Triggered Automails"
    meta:resourcekey="PageResource1" %>

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
        <pw4:PageTitle Icon="email_edit.png" Title="Automatically Triggered Mails" Description="Edit content of automatically sent mails."
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <br />
        <asp:Literal ID="Literal8" runat="server" Text="Select an automail type corresponding to a specific event (like Welcome for new members), an organization and a geography. Type your mail. Preview a lot. When you click Save, that mail will be sent on every event of the corresponding type."
            meta:resourcekey="Literal8Resource1"></asp:Literal>
        <br />
        <asp:Panel ID="MainPanel" runat="server">
            <asp:Label ID="LabelTypePrompt" runat="server" Text="Edit Automail Type" meta:resourcekey="LabelTypePromptResource1"></asp:Label>
            <asp:DropDownList ID="DropRecipients" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropRecipients_SelectedIndexChanged"
                meta:resourcekey="DropRecipientsResource1">
                <asp:ListItem Selected="True" Value="Welcome" meta:resourcekey="ListItemResource1">Welcome</asp:ListItem>
            </asp:DropDownList>
            <asp:Label ID="LabelOrganizationPrompt" runat="server" Text="for organization" meta:resourcekey="LabelOrganizationPromptResource1"></asp:Label>
            <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                meta:resourcekey="DropOrganizationsResource1">
            </asp:DropDownList>
            <asp:Label ID="LabelOrganizationsInGeographies1" runat="server" Text="in" meta:resourcekey="LabelOrganizationsInGeographies1Resource1"></asp:Label>
            <asp:DropDownList ID="DropGeographies" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropGeographies_SelectedIndexChanged"
                meta:resourcekey="DropGeographiesResource1">
            </asp:DropDownList>
            &nbsp;
            <asp:Panel ID="PanelMailContents" runat="server" meta:resourcekey="PanelMailContentsResource1">
                <hr />
                <asp:Table ID="Table" runat="server" meta:resourcekey="TableResource1">
                    <asp:TableRow runat="server" meta:resourcekey="TableRowResource1">
                        <asp:TableCell runat="server" meta:resourcekey="TableCellResource1">
                <b>From:</b>
                        </asp:TableCell>
                        <asp:TableCell runat="server" meta:resourcekey="TableCellResource2">
                            <asp:Label ID="LabelSender" runat="server" Text="Label" meta:resourcekey="LabelSenderResource1"></asp:Label>
                            <asp:Label ID="LabelMailAddressSuffix" runat="server" Text="Label" meta:resourcekey="LabelMailAddressSuffixResource1"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server" meta:resourcekey="TableRowResource2">
                        <asp:TableCell runat="server" meta:resourcekey="TableCellResource3">
                <b>To:</b>
                        </asp:TableCell>
                        <asp:TableCell runat="server" meta:resourcekey="TableCellResource4">
                            <asp:Label ID="LabelRecipients" runat="server" Text="New members of " meta:resourcekey="LabelRecipientsResource1"></asp:Label>
                            <asp:Label ID="LabelSelectedOrganization" runat="server" Text="Label" meta:resourcekey="LabelSelectedOrganizationResource1"></asp:Label>
                            <asp:Label ID="LabelOrganizationsInGeographies2" runat="server" Text=" in " meta:resourcekey="LabelOrganizationsInGeographies2Resource1"></asp:Label>
                            <asp:Label ID="LabelSelectedGeography" runat="server" Text="Label" meta:resourcekey="LabelSelectedGeographyResource1"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server" meta:resourcekey="TableRowResource3">
                        <asp:TableCell runat="server" meta:resourcekey="TableCellResource5">
                            <b>
                                <asp:Literal ID="Literal1" runat="server" Text="Subject" meta:resourcekey="Literal1Resource1"></asp:Literal>
                                :</b>
                        </asp:TableCell>
                        <asp:TableCell runat="server" meta:resourcekey="TableCellResource6">
                            <asp:Label runat="server" Text="Label" ID="LabelSelectedOrganizationMailPrefix" meta:resourcekey="LabelSelectedOrganizationMailPrefixResource1"></asp:Label>
                            :
                            <asp:TextBox ID="TextSubject" runat="server" Enabled="False" Text="Automatically set"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <hr />
               
                <div style="float: right; margin-left: 20px; border-left: solid 2px #808080; padding-left: 20px;
                    width: 500px">
                    <asp:Literal ID="LiteralPreview" runat="server"></asp:Literal><br />
                </div>
                <asp:Literal ID="Literal2" runat="server" Text="Body" meta:resourcekey="Literal2Resource1"></asp:Literal>
                :<br />
                <asp:TextBox ID="TextBody" runat="server" Columns="40" Rows="15" TextMode="MultiLine"
                    meta:resourcekey="TextBodyResource1"></asp:TextBox>
                <br />
                <br />
                            
                        <asp:Button ID="ButtonPreview" runat="server" Text="Preview" OnClick="ButtonPreview_Click"
            meta:resourcekey="ButtonPreviewResource1" />&nbsp;&nbsp;
        <asp:Button ID="ButtonSave" OnClientClick="return doConfirm(this,'You are sure that you are all finished with this mail, and that it should be used as an automail from now on?');"
            runat="server" Text="Save" OnClick="ButtonSave_Click" meta:resourcekey="ButtonSaveResource1" /><br />

            </asp:Panel>
        </asp:Panel>

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
