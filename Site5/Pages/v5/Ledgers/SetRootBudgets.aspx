<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="SetRootBudgets.aspx.cs" Inherits="Pages_v5_Ledgers_SetRootBudgets" %>
<%@ Register src="~/Controls/v5/Pirates/PersonDetailPopup.ascx" tagName="PersonDetailPopup" tagPrefix="act5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">

<h2><asp:Label ID="LabelRootBudgetHeader" Text="Root Budgets For Org [LOC]" runat="server" /><div style="float:right"><asp:DropDownList ID="DropYears" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropYears_SelectedIndexChanged" /></div></h2>

<div class="entrylabels" style="margin-right:30px;width:200px">
    <strong><asp:Label ID="LabelAccountHeader" Text="Account [LOC]" runat="server" /></strong><br/>
    <asp:Repeater ID="RepeaterAccountNames" runat="server" OnItemDataBound="RepeaterAccountNames_ItemDataBound" ><ItemTemplate><asp:Label ID="LabelAccountName" runat="server" Text="Unset Account Name" /> <em>(<asp:Label ID="LabelAccountType" runat="server" Text="Type [LOC]" />)</em><br /></ItemTemplate></asp:Repeater>
    <em><asp:Label ID="LabelYearlyResultLabel" runat="server" Text="Yearly Result [LOC]" /></em>
</div>

<div class="entryfields">
    <strong>Budget, Owner [LOC]</strong><br/>
    <asp:Repeater ID="RepeaterAccountBudgets" OnItemDataBound="RepeaterAccountBudgets_ItemDataBound" runat="server"><ItemTemplate><asp:HiddenField runat="server" ID="HiddenAccountId" /><asp:TextBox ID="TextBudget" runat="server" Text="Not set" />&nbsp;&nbsp;<span style="border-bottom: dashed 1px #808080;cursor:pointer"><asp:Label ID="LabelBudgetOwner" runat="server" /></span><telerik:RadToolTip ID="ToolTip" runat="server"  AnimationDuration="150" AutoCloseDelay="200000" ShowDelay="0"
            EnableShadow="true" HideDelay="1" Width="288px" Height="96px" HideEvent="ManualClose" OffsetX="30" OffsetY="0"
            RelativeTo="Element" Animation="Slide" Position="TopCenter" ShowCallout="true" TargetControlID="LabelBudgetOwner" RenderInPageRoot="true" ShowEvent="OnClick"
            Skin="Telerik"><act5:PersonDetailPopup runat="server" ID="PersonDetail" /></telerik:RadToolTip><br/></ItemTemplate></asp:Repeater>
    <asp:TextBox ID="TextYearlyResult" Text="Yearly Result (fill in programmatically)" runat="server" ReadOnly="true" Enabled="false" />&nbsp;<br/>
    <asp:Button ID="ButtonSetBudgets" Text="Set New Budgets [LOC]" OnClick="ButtonSetBudgets_Click" runat="server" />&nbsp;
</div>

<div class="entrylabels" style="margin-left:20px;width:100px;text-align:right">
<strong><asp:Label ID="LabelActualsHeader" Text="Actuals [LOC]" runat="server" /></strong><br/>
<asp:Repeater ID="RepeaterAccountActuals" runat="server" OnItemDataBound="RepeaterAccountActuals_ItemDataBound"><ItemTemplate><asp:Label ID="LabelAccountActuals" Text="Unset actuals" runat="server" /><br/></ItemTemplate></asp:Repeater>
    <asp:Label ID="LabelYearlyResultActuals" runat="server" Text="Yearly Result [LOC]" /><br/>
</div>

<div style="clear:both"></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">

    <h2 class="blue"><asp:Label ID="LabelSidebarInfo" runat="server" Text="INFO [Loc]" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelDashboardInfo" Text="You have not localized the dashboard information box." runat="server" /></a>
        </div>
    </div>
    
    <h2 class="blue"><asp:Label ID="LabelSidebarActions" Text="ACTIONS [Loc]" runat="server" /><span class="arrow"></span></h2>
    
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
    
    <h2 class="orange"><asp:Label ID="LabelSidebarTodo" Text="TODO [Loc]" runat="server" /><span class="arrow"></span></h2>
    
    <div class="box">
        <div class="content">
            <asp:Label ID="LabelActionItemsHere" runat="server" />
        </div>
    </div>
        


</asp:Content>

