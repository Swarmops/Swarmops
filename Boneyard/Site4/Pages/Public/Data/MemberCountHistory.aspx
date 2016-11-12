<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4-menuless.master" AutoEventWireup="true" CodeFile="MemberCountHistory.aspx.cs" Inherits="Pages_Public_Data_MemberCountHistory" Title="Member Count History - PirateWeb" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">
    <pw4:PageTitle ID="PageTitle" runat="server" Title="Member Count History" Description="Resource for press/media to access historical data" Icon="chart_line.png" />
    
    <asp:UpdatePanel ID="UpdateMain" runat="server">
    <ContentTemplate>
    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">What is this?</span><br />
    <div class="DivGroupBoxContents">
            You are seeing a public part of the Pirate Party's membership management system. You can access some statistical data from here.
    </div>
    
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Get a historic member count</span><br />
    <div class="DivGroupBoxContents">
            What was the total membercount of <b>Piratpartiet SE</b> at 
            <telerik:RadDateTimePicker ID="PickDateTime" runat="server" 
                MinDate="2006-01-01" Skin="Web20" >
<TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

<TimeView CellSpacing="-1" runat="server" ></TimeView>

<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False"  runat="server"
                    ViewSelectorText="x" Skin="Web20"></Calendar>

<DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
            </telerik:RadDateTimePicker>? <asp:Button ID="ButtonGetCount" runat="server" 
                Text="Get" onclick="ButtonGetCount_Click" /> <b><asp:Label ID="LabelCountResults" runat="server" /></b>
    </div>
    
    </div>
        <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Get printable graphics</span><br />
    <div class="DivGroupBoxContents">

    Generate a press quality day-by-day graph from <b><asp:Label ID="LabelStartDate" runat="server" /></b> above to <telerik:RadDatePicker Skin="Web20" ID="PickEndDate" runat="server" />,<br />
    adapted for <asp:TextBox ID="TextWidth" runat="server" /> by <asp:TextBox ID="TextHeight" runat="server" /> <asp:DropDownList ID="DropUnit" runat="server"><asp:ListItem Text="cm" Selected="True" /><asp:ListItem Text="in" /></asp:DropDownList> at 300 dpi: <asp:Button ID="ButtonGenerateGraphUrl" runat="server" OnClick="ButtonGenerateGraphUrl_Click" Text="Generate" /><br />
    
    <asp:Panel Visible="false" ID="PanelGraphUrl" runat="server">
    Your graph is available at <asp:HyperLink ID="LinkGraph" runat="server" />. Download or view as usual.</asp:Panel>  

    </div>
    </div>
    </div>
    
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ButtonGetCount" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ButtonGenerateGraphUrl" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>

