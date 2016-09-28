<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Tests_TestBookkeepingIntegrity" Codebehind="TestBookkeepingIntegrity.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
<br/>
<div class="entryLabelsAdmin" style="width:150px">Organization<br/>Year<br/>&nbsp;<br/>EOY Balance<br/>EOLY Owncapital<br/>EOCY Owncapital<br/>Diff<br/>Actual OC Delta<br/>Results-all<br/>Results-noted</div>
<div class="entryFieldsAdmin">
    <asp:Label ID="LabelThisOrganization" runat="server" /><br/>
    <asp:DropDownList runat="server" ID="DropYear" AutoPostBack="true" OnSelectedIndexChanged="DropYear_SelectedIndexChanged" />&nbsp;<br/>&nbsp;<br/>
    <asp:Label ID="LabelEoyBalance" runat="server" /><br/>
    <asp:Label ID="LabelEolyOwnCapital" runat="server" /><br/>
    <asp:Label ID="LabelEocyOwnCapital" runat="server" /><br/>
    <asp:Label ID="LabelOwnCapitalDiff" runat="server" /><br/>
    <asp:Label ID="LabelOwnCapitalDelta" runat="server" /><br/>
    <asp:Label ID="LabelResultsAll" runat="server" /><br/>
    <asp:Label ID="LabelResultsNoted" runat="server" />
</div>

<div style="clear:both"></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
    <h2 class="blue">INFO<span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            This page is for diagnostic tests of the integrity of the Activizr system. It is not intended for everyday use and is not accessible from the menu.
        </div>
    </div>
    
</asp:Content>

