<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="AllocateFunds.aspx.cs" Inherits="Pages_v4_Financial_AllocateFunds" Title="Allocate Funds - PirateWeb" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/ComboPerson.ascx" TagName="ComboPerson" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="money-coin.png" Title="Allocate Funds" Description="Reserve money in a budget for an expense or invoice, or subdivide the budget" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Organization</span><br />
    <div class="DivGroupBoxContents">
    Only <b>Piratpartiet SE</b> for now.
    </div>
    </div>
    <asp:Panel runat="server" ID="PanelExpenseClaims">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Allocate funds from a budget</span><br />
    <div class="DivGroupBoxContents">
                <b>Step 1:</b> What <b>budget</b> are you allocating funds from?<br />
                <div style="float:left;width:100px">Budget:</div>
                <asp:DropDownList ID="DropBudgets" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="DropBudgets_SelectedIndexChanged" ><asp:ListItem Selected="True" Text="-- Select One --" Value="0" /></asp:DropDownList>&nbsp;<asp:CompareValidator
                    ID="Validator_DropBudgets_Selected" ValueToCompare="0" Operator="NotEqual" Text="Please select a budget." runat="server"
                    ControlToValidate="DropBudgets" EnableClientScript="false" Display="Dynamic" /><br />

                <b>Step 2:</b> What <b>amount</b> are you allocating?<br />
                <div style="float:left;width:100px">Amount:</div>
                <asp:TextBox ID="TextAmount" runat="server" Text="0,00" />&nbsp;<asp:RequiredFieldValidator EnableClientScript="false" 
                                    ID="RequiredFieldValidator1" runat="server" 
                                    ErrorMessage="Please type an amount." InitialValue="0,00" 
                    ControlToValidate="TextAmount" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="CustomValidator1" runat="server" 
                                    ErrorMessage="Please type a numeric amount." 
                    Display="Dynamic" 
                    onservervalidate="Validator_TextAmount_Custom_ServerValidate" />
                                <br />
                
                <b>Step 3:</b> What <b>mechanism</b> are you using to allocate funds from this budget?<br />
                <div style="float:left;width:100px">Mechanism:<br /></div>
                <asp:UpdatePanel ID="UpdateMethod" runat="server" UpdateMode="Conditional"><ContentTemplate>
                    <asp:DropDownList ID="DropMethod" runat="server" AutoPostBack="True" 
                        onselectedindexchanged="DropMethod_SelectedIndexChanged"><asp:ListItem Selected="True" Text="-- Select one --" Value="0" />
                    <asp:ListItem Text="Pre-attest an expense claim by" Value="ExpenseClaim" />
                    <asp:ListItem Text="Create a Purchase Order for a purchase from" Value="PurchaseOrder" Enabled="false" />
                    <asp:ListItem Text="Create a new sub-budget owned by" Value="CreateSubBudget" />
                    <asp:ListItem Text="Transfer funds to the sub-budget" Value="TransferSubBudget" />
                    </asp:DropDownList><asp:CompareValidator
                    ID="Validator_Mechanism_Required" ValueToCompare="0" Operator="NotEqual" EnableClientScript="false" Text=" Please select a mechanism." runat="server"
                    ControlToValidate="DropMethod" Display="Dynamic" />
                    
                    &nbsp;<pw4:ComboPerson ID="ComboClaimPerson" runat="server" Visible="false" /><asp:CustomValidator ID="Validator_ClaimPerson_Required" runat="server" Display="Dynamic" Text=" Please select a claiming person." OnServerValidate="Validate_ClaimPerson_Required" />
                    <asp:TextBox ID="TextPurchaseOrderCompany" Visible="false" runat="server" /><asp:CustomValidator ID="Validator_CompanyName_Required" runat="server" Display="Dynamic" Text=" Please type a company name." OnServerValidate="Validate_CompanyName_Required" />
                    <pw4:ComboPerson ID="ComboBudgetPerson" runat="server" Visible="false" /><asp:CustomValidator ID="Validator_BudgetPerson_Required" runat="server" Display="Dynamic" Text=" Please select an owner." OnServerValidate="Validate_BudgetPerson_Required" />
                    <pw4:FinancialAccountTreeDropDown ID="DropSubBudget" Visible="false" runat="server" /><asp:CustomValidator ID="Validator_Budget_Required" runat="server" Display="Dynamic" Text=" Please select a sub-budget." OnServerValidate="Validate_SubBudget_Required" />
                    <br />
            
                    <b>Step 4:</b> Enter a <b>description</b> <asp:Label ID="LabelForTheItem" runat="server" />.<br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DropMethod" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DropBudgets" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>

                
                <div style="float:left;width:100px">Description:</div>
                <asp:TextBox ID="TextDescription" runat="server" />&nbsp;<asp:RequiredFieldValidator EnableClientScript="false" 
                                    ID="Validator_TextDescription_Required" runat="server" 
                                    ErrorMessage="Please type a description."
                    ControlToValidate="TextDescription" Display="Dynamic"></asp:RequiredFieldValidator>
                    
                 <br />
                <b>Step 5:</b> All done? Then go ahead and allocate.<br />
                <div style="float:left;width:100px">&nbsp;</div><asp:Button ID="ButtonSubmit" runat="server" OnClick="Submit_Click" Text="Allocate!" />&nbsp;<br />
                 
                                
    </div>
    </div>
    </asp:Panel>
    </div>
</asp:Content>

