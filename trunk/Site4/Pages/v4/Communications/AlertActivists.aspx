<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="AlertActivists.aspx.cs" Inherits="Pages_v4_AlertActivists" Title="Alert Activists - PirateWeb"
    ValidateRequest="false" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" DefaultLoadingPanelID="RadAjaxLoadingPanel1"
        UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="GeographyTree">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LabelGeographies" />
                    <telerik:AjaxUpdatedControl ControlID="PanelRestOfPage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="CheckSms">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRestOfPage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="CheckMail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRestOfPage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonTest">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRestOfPage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonSend">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRestOfPage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

        <script type="text/javascript">

            function disableMe(btn) {
                btn.disabled = true;
                if (btn.type.toLowerCase() == "submit")
                    __doPostBack(btn.name, "");
                return true;

            }

            function updateSmsLength(elem) {
                var lengthBox = document.getElementById("SmSCharsLeft");
                lengthBox.value = (156 - elem.value.length);
            }
        </script>

    </telerik:RadScriptBlock>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Web20"
        IsSticky="false">
        <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>'
            style="border: 0px;" />
    </telerik:RadAjaxLoadingPanel>
    <pw4:PageTitle Icon="bell.png" Title="Alert Activists" Description="Alerts activists over SMS and/or mail"
        runat="server" ID="PageTitle" />
    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Alert activists where?</span><br />
            <div class="DivGroupBoxContents">
                <pw4:GeographyTreeDropDown ID="GeographyTree" runat="server" Skin="Web20" OnSelectedNodeChanged="GeographyTreeDropDown_SelectedNodeChanged" />
                &nbsp;&nbsp;<asp:Label ID="LabelGeographies" runat="server">Select a geography.</asp:Label>
            </div>
        </div>
        <asp:Panel ID="PanelRestOfPage" runat="server">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Send what messages?</span><br />
                <div class="DivGroupBoxContents">
                    <asp:Panel ID="SmsPanel" runat="server">
                        <asp:CheckBox ID="CheckSms" runat="server" Text="Send SMS" Checked="false" OnCheckedChanged="CheckSms_CheckedChanged"
                            AutoPostBack="True" />
                        &nbsp;&nbsp;&nbsp;(Chars left=<input type="text" id="SmSCharsLeft" value="156" disabled="disabled"
                            size="3" style="font-size: small;" />)
                        <asp:Panel ID="PanelSmsText" runat="server" CssClass="VAlignMid">
                            <asp:TextBox CssClass="FullWidth" Columns="80" MaxLength="156" ID="TextSms" runat="server" /></asp:Panel>
                        <hr />
                    </asp:Panel>
                    <asp:CheckBox ID="CheckMail" runat="server" Text="Send mail" Checked="true" AutoPostBack="True"
                        OnCheckedChanged="CheckMail_CheckedChanged" /><br />
                    <asp:Panel ID="PanelMailText" runat="server">
                        Subject: <i>Piratpartiet:</i>
                        <asp:TextBox Columns="80" ID="TextMailSubject" runat="server" /><br />
                        <asp:TextBox CssClass="FullWidth" TextMode="MultiLine" Columns="80" Rows="10" ID="TextMailBody"
                            runat="server" />
                    </asp:Panel>
                    The cost of sending the SMS messages is <b>SEK
                        <asp:Label ID="LabelSmsCost" runat="server" /></b> and will be charged to the
                    budget of <b>
                        <asp:Label ID="LabelBudget" runat="server" /></b>.<br />
                    <div style="text-align: right">
                        <asp:Button ID="ButtonTest" runat="server" Text="Test (send to me)" OnClick="ButtonTest_Click" />
                        <asp:Button ID="ButtonSend" runat="server" Text="Send!" OnClick="ButtonSend_Click"
                            OnClientClick="disableMe(this)" /></div>
                    <asp:Label ID="ErrorMsg" runat="server" ForeColor="#FF0000" Text=""></asp:Label>
                </div>
            </div>
            <asp:Panel ID="PanelFinal" runat="server" Visible="false">
                <div class="DivGroupBox">
                    <span class="DivGroupBoxTitle">All done, transmission initiated</span><br />
                    <div class="DivGroupBoxContents">
                        Your message has been committed to the database and transmission to activists has
                        already begun.
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelTestSent" runat="server" Visible="false">
                <div class="DivGroupBox">
                    <span class="DivGroupBoxTitle">Test message</span><br />
                    <div class="DivGroupBoxContents">
                        Test message was sent.
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Content>
