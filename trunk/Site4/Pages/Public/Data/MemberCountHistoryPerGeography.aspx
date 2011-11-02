<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" AutoEventWireup="true"
    CodeFile="MemberCountHistoryPerGeography.aspx.cs" Inherits="Pages_Public_Data_MemberCountHistoryPerGeography"
    Title="Member Count History - PirateWeb" %>

<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register src="../../../Controls/v4/WSGeographyTreeDropDown.ascx" tagname="WSGeographyTreeDropDown" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
     <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" 
         UpdatePanelsRenderMode="Inline" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <ClientEvents OnRequestStart="Masterpage_RadAjaxManager_OnRequestStart" OnResponseEnd="Masterpage_RadAjaxManager_OnResponseEnd" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="PickDateTime">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LabelStartDate"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonGetCount">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LabelCountResults" 
                        LoadingPanelID="RadAjaxLoadingPanel1"/>
                    <telerik:AjaxUpdatedControl ControlID="LabelStartDate" />
                    <telerik:AjaxUpdatedControl ControlID="PanelGraphUrl" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ButtonGenerateGraphUrl">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelGraphUrl" 
                        LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadCodeBlock runat="server">

        <script type="text/javascript">

            function RadAjaxManager_RequestStart(sender, eventArgs) {
            }

            function RadAjaxManager_ResponseEnd(sender, eventArgs) {
            }

        </script>

    </telerik:RadCodeBlock>
    <div style="z-index: 100">
        <pw4:PageTitle ID="PageTitle" runat="server" Title="Member Count History / Area"
            Description="Resource for press/media to access historical data" Icon="chart_line.png" />
        <div class="DivMainContent">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">What is this?</span><br />
                <div class="DivGroupBoxContents">
                    You are seeing a public part of the Pirate Party's membership management system.
                    You can access some statistical data from here.
                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Get a historic member count</span><br />
                <div class="DivGroupBoxContents">
                    What was the total membercount of 
                        <asp:DropDownList ID="DropDownListOrg" runat="server">
                        
                        </asp:DropDownList>
                        within the area of
                    <uc1:WSGeographyTreeDropDown ID="GeographyTreeDropDown1" runat="server" />
                    at
                    <telerik:RadDateTimePicker ID="PickDateTime" runat="server" MinDate="2006-01-01"
                        Skin="Web20">
                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>
                        <TimeView CellSpacing="-1" runat="server">
                        </TimeView>
                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" runat="server"
                            ViewSelectorText="x" Skin="Web20">
                        </Calendar>
                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                    </telerik:RadDateTimePicker>
                    ?
                    <asp:Button ID="ButtonGetCount" runat="server" Text="Get" OnClick="ButtonGetCount_Click" />&nbsp;<b><asp:Label
                        ID="LabelCountResults" runat="server" />
                    </b>
                    <br />
                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Get printable graphics</span><br />
                <div class="DivGroupBoxContents">
                    Generate a press quality day-by-day graph from <b>
                        <asp:Label ID="LabelStartDate" runat="server" /></b> above to
                    <telerik:RadDatePicker Skin="Web20" ID="PickEndDate" runat="server" />
                    ,<br />
                    adapted for
                    <asp:TextBox ID="TextWidth" runat="server" />
                    by
                    <asp:TextBox ID="TextHeight" runat="server" />
                    <asp:DropDownList ID="DropUnit" runat="server">
                        <asp:ListItem Text="cm" Selected="True" />
                        <asp:ListItem Text="in" />
                    </asp:DropDownList>
                    at 300 dpi:
                    <asp:Button ID="ButtonGenerateGraphUrl" runat="server" OnClick="ButtonGenerateGraphUrl_Click"
                        Text="Generate" /><br />
                    <asp:Panel Visible="false" ID="PanelGraphUrl" runat="server">
                        Your graph is available at
                        <asp:HyperLink ID="LinkGraph" runat="server" />. Download or view as usual.</asp:Panel>
                </div>
            </div>
        </div>
    </div>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" 
         Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
</asp:Content>
