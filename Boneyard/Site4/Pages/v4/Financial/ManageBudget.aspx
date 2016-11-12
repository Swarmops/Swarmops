<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ManageBudget.aspx.cs" Inherits="Pages_v4_Financial_ManageBudget" Title="Manage Budget - PirateWeb" EnableViewState="true" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PersonDetailPopup.ascx" TagName="PersonDetailPopup" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/Financial/Ledger.ascx" TagName="Ledger" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

<style type="text/css">

    .RadToolTip { z-index:8001; }
    .RadComboBoxDropDown { z-index:8002; }

</style>

    <pw4:PageTitle Icon="jar.png" Title="Manage Budget" Description="View budget status, suballocate, and predict monthly spending" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Budget And Year</span><br />
    <div class="DivGroupBoxContents">
    Budget: <asp:DropDownList ID="DropBudgets" runat="server" />, year <asp:DropDownList ID="DropYears" runat="server" /> <asp:Button ID="ButtonSelectBudget" runat="server" Text="Select" OnClick="ButtonSelectBudget_Click" />
    <asp:HiddenField ID="HiddenInitBudgetId" runat="server" /><asp:HiddenField ID="HiddenInitYear" runat="server" />
    </div>
    </div>
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Status and Monthly Predictions</span><br />
    <div class="DivGroupBoxContents">
    <div style="width:100%" id="DivIframeContainer">Loading graphics...</div>
    <table border="0" cellpadding="0" cellspacing="2" width="100%">
    <tr>
        <td>&nbsp;</td><td align="center">Jan</td><td align="center">Feb</td><td align="center">Mar</td><td align="center">Apr</td><td align="center">May</td><td align="center">Jun</td><td align="center">Jul</td><td align="center">Aug</td><td align="center">Sep</td><td align="center">Oct</td><td align="center">Nov</td><td align="center">Dec</td><td align="center">&nbsp;</td>
    </tr>
    <tr>
        <td><b>Predict&nbsp;</b></td><td><asp:TextBox ID="TextPredictMonth1" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth2" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth3" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth4" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth5" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth6" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth7" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth8" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth9" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth10" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth11" runat="server" /></td><td><asp:TextBox ID="TextPredictMonth12" runat="server" /></td><td><asp:Button ID="ButtonSavePredict" Text="Save Predictions" runat="server" OnClick="ButtonSavePredict_Click" /></td>
    </tr>
    <tr>
        <td><b>Actuals&nbsp;</b></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth1" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth2" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth3" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth4" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth5" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth6" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth7" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth8" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth9" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth10" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth11" runat="server" /></td><td><asp:TextBox ReadOnly="true" Enabled="false" ID="TextActualsMonth12" runat="server" /></td><td>&nbsp;</td>
    </tr>
    </table>
    </div>
    </div>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr valign="top"><td width="50%">
            <div class="DivGroupBox" style="z-index:50">
                <span class="DivGroupBoxTitle">Sub-budgets and allocations</span><br />
                <div class="DivGroupBoxContents">
                <div style="float:left; width:200px"><asp:Label ID="LabelThisAccountName" runat="server" Text="This account" /><asp:Repeater ID="RepeaterAccountNames" runat="server"><ItemTemplate><br /><%# Eval("AccountName") %></ItemTemplate></asp:Repeater></div>
                <div style="float:left; width:200px">&nbsp;<asp:TextBox ID="TextThisAccountBudget" runat="server" ReadOnly="true" Enabled="false" />&nbsp;<asp:Repeater ID="RepeaterBudgetTextBoxes" EnableViewState="true" OnItemDataBound="RepeaterTextBoxes_ItemDataBound" runat="server"><ItemTemplate><br />&nbsp;<asp:TextBox ID="TextChildBudget" EnableViewState="true" runat="server" /><asp:HiddenField ID="HiddenAccountId" runat="server" />&nbsp;</ItemTemplate></asp:Repeater><br />&nbsp;<asp:Button ID="ButtonReallocate" OnClick="ButtonReallocate_Click" Text="Save and Reallocate" runat="server" /></div>
                <asp:Label ID="LabelThisAccountOwner" Text="Owned by you" runat="server" /><asp:Repeater ID="RepeaterBudgetOwners" OnItemDataBound="RepeaterBudgetOwners_ItemDataBound" runat="server"><ItemTemplate><br /><span style="border-bottom: dashed 1px #808080;cursor:pointer"><asp:Label ID="LabelChildBudgetOwner" runat="server" /></span><asp:HiddenField ID="HiddenAccountId" runat="server" /><telerik:RadToolTip ID="ToolTip" runat="server"  AnimationDuration="150" AutoCloseDelay="200000" ShowDelay="0"
            EnableShadow="true" HideDelay="1" Width="288px" Height="96px" HideEvent="ManualClose" OffsetX="30" OffsetY="0"
            RelativeTo="Element" Animation="Slide" Position="TopCenter" ShowCallout="true" TargetControlID="LabelChildBudgetOwner" RenderInPageRoot="true" ShowEvent="OnClick"
            Skin="Telerik"><pw4:PersonDetailPopup runat="server" ID="PersonDetail" /></telerik:RadToolTip></ItemTemplate></asp:Repeater>
                <br clear="all" />
                </div>
            </div>
        </td>
        <td width="50%">
            <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Ledger</span><br />
                <div class="DivGroupBoxContents">
                    <asp:UpdatePanel ID="UpdateLedger" runat="server" >
                        <ContentTemplate>
                            <pw4:Ledger ID="Ledger" runat="server" SimplifiedView="true" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </td></tr>
    </table>
    
    <br clear="all" />
    </div>


</asp:Content>

