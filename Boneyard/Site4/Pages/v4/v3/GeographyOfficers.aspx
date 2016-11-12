<%@ Page Title="" Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="GeographyOfficers.aspx.cs" Inherits="Pages_v4_v3_GeographyOfficers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <pw4:PageTitle Icon="user_edit.png" Title="Lokala kontaktuppgifter" Description=""
            runat="server" ID="PageTitle" />

          <div id="divContent" runat="server">
            
        </div>
    </div>
</asp:Content>