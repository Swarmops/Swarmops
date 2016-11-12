<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="BasicDetails.aspx.cs" Inherits="Pages_Members_BasicDetails" Title="PirateWeb - Member Details - Basic Details"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v3/GeographyLine.ascx" TagName="GeographyLine" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/v4/v3/PersonDetailsPagesMenu.ascx" TagName="PersonDetailsPagesMenu"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/v3/PersonBasicDetails.ascx" TagName="PersonBasicDetails"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Basic Details" Description="Basic details for a person"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <uc2:PersonDetailsPagesMenu ID="PersonDetailsPagesMenu" runat="server"></uc2:PersonDetailsPagesMenu>
        <uc1:PersonBasicDetails ID="PersonDetails" runat="server"></uc1:PersonBasicDetails>
        <br />
        <br />
        <b>Geography:</b>
        <uc3:GeographyLine ID="GeographyLine" runat="server" />
        &nbsp;
    </div>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"
        DefaultLoadingPanelID="RadAjaxLoadingPanel1" meta:resourcekey="RadAjaxManager1Resource1">
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" Skin="Web20"
        meta:resourcekey="RadAjaxLoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
</asp:Content>
