<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="InspectLedgers.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Ledgers.InspectLedgers" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ComboBudgets" Src="~/Controls/v5/Financial/ComboBudgets.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
    <link rel="stylesheet" type="text/css" href="/Style/v5-easyui-elements.css" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    
    <h2>Inspect <Swarmops5:ComboBudgets ID="DropBudgets" AccountListType="All" runat="server" /> for <asp:DropDownList runat="server" ID="DropYears"/> <asp:DropDownList runat="server" ID="Months"/></h2>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

