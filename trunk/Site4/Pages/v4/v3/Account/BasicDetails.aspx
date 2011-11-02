<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="BasicDetails.aspx.cs" Inherits="Pages_Account_BasicDetails" Title="PirateWeb - Member Details - Basic Details"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v3/PersonBasicDetails.ascx" TagName="PersonBasicDetails"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Edit basic data" Description="Edit the basic data about me"
            runat="server" ID="PageTitle" meta:resourcekey="PageTitle" />
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline"
            DefaultLoadingPanelID="RadAjaxLoadingPanel1" 
            meta:resourcekey="RadAjaxManager1Resource1">
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" Skin="Web20"
            meta:resourcekey="RadAjaxLoadingPanel1Resource1">
        </telerik:RadAjaxLoadingPanel>
        <uc1:PersonBasicDetails ID="PersonDetails" runat="server"></uc1:PersonBasicDetails>
    </div>
</asp:Content>
