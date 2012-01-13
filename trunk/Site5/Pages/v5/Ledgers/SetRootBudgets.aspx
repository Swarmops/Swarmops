<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="SetRootBudgets.aspx.cs" Inherits="Pages_v5_Ledgers_SetRootBudgets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

<h2><asp:Label ID="LabelRootBudgetHeader" Text="Root Budgets For Org [LOC]" runat="server" /></h2>

<div class="entrylabels" style="margin-right:30px;width:200px">
    <strong><asp:Label ID="LabelYear" Text="Year [LOC]" runat="server" /></strong><br/>
    <asp:Repeater ID="RepeaterAccountNames" runat="server" OnItemDataBound="RepeaterAccountNames_ItemDataBound" ><ItemTemplate><asp:Label ID="LabelAccountName" runat="server" Text="Unset Account Name" /> <em>(<asp:Label ID="LabelAccountType" runat="server" Text="Type [LOC]" />)</em><br /></ItemTemplate></asp:Repeater>
</div>
<div class="entryfields">
    <strong><asp:DropDownList ID="DropYears" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropYears_SelectedIndexChanged" /></strong>&nbsp;<br/>
    <asp:Repeater ID="RepeaterAccountBudgets" OnItemDataBound="RepeaterAccountBudgets_ItemDataBound" runat="server"><ItemTemplate><asp:TextBox ID="TextBudget" runat="server" Text="Not set" />&nbsp;<br/></ItemTemplate></asp:Repeater>
</div>

<div style="clear:both"></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">

    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelDashboardInfo" Text="You have not localized the dashboard information box." runat="server" /></a>
        </div>
    </div>
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActions" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content"><!--
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/Vote.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-vote-16px.png')"></div>
                <asp:Label ID="LabelActionVote" runat="server" />
            </div>
            <div class="link-row-encaps" onclick="document.location='/Pages/v5/Governance/ListMotions.aspx';" >
                <div class="link-row-icon" style="background-image:url('/Images/PageIcons/iconshock-motions-16px.png')"></div>
                <asp:Label ID="LabelActionListMotions" runat="server" />
            </div>-->
        </div>
    </div>
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodo" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelActionItemsHere" runat="server" />
        </div>
    </div>
        


</asp:Content>

