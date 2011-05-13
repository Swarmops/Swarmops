<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="RequestAdvance.aspx.cs" Inherits="Pages_v4_Accounting_RequestAdvance" Title="Request a Cash Advance - PirateWeb" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/ComboPerson.ascx" TagName="ComboPerson" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="coins_add.png" Title="Request Cash" Description="Request a cash advance for expenses you're about to have on behalf of the organization"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Request a cash advance</span><br />
            <div class="DivGroupBoxContents">
                <b>Step 1:</b> What <b>organization</b> are you requesting a cash advance from?<br />
                <div style="float:left;width:100px">Organization:</div><asp:DropDownList ID="DropOrganizations" 
                                    runat="server" onselectedindexchanged="DropOrganizations_SelectedIndexChanged"><asp:ListItem Selected="True" Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br />
                <asp:UpdatePanel ID="UpdateImpersonation" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="PanelImpersonation" Visible="false" runat="server">
                    <b>Step 1.5:</b> Are you, as an officer, filing this on behalf of <b>someone else</b>?<br />
                    
                    <asp:CheckBox ID="CheckImpersonate" AutoPostBack="true"
                        Text="Request advance on behalf of other person:" runat="server" 
                        oncheckedchanged="CheckImpersonate_CheckedChanged" /> <pw4:ComboPerson ID="ComboPersonImpersonated" OnSelectedPersonChanged="ComboPersonImpersonated_SelectedPersonChanged" runat="server" />
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DropOrganizations" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="UpdateBankDetails" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                <b>Step 2:</b> Verify <b><asp:Label ID="LabelBankDetails" Text="your bank details" runat="server" /></b>, so an expense can be paid out.<br />
                <div style="float:left;width:100px">Bank:<br />Clearing:</br>Account:</div>
                <asp:TextBox ID="TextBank" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextBank_Required" runat="server" 
                                    ErrorMessage="Please enter the bank's name." EnableClientScript="false"
                    ControlToValidate="TextBank" /><br />
                                
                    <asp:TextBox ID="TextBankClearing" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="RequiredFieldValidator1" runat="server" EnableClientScript="false" Display="Dynamic"  
                                    ErrorMessage="Please enter the clearing number (four digits)."
                    ControlToValidate="TextBankClearing" /><asp:CustomValidator ID="Validator_Clearing_NumbersOnly" OnServerValidate="ValidatorClearingNumbersOnly_ServerValidate" runat="server" Display="Dynamic" EnableClientScript="false" ErrorMessage="Please enter exactly four (4) digits." ControlToValidate="TextBankClearing" /><br />
                    
                    <asp:TextBox ID="TextAccount" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                    ID="Validator_TextAccount_Required" runat="server" EnableClientScript="false" Display="Dynamic" 
                    ErrorMessage="Please enter your bank account for reimbursement." 
                    ControlToValidate="TextAccount" />
                    <asp:CustomValidator ID="Validator_Account_NumbersOnly" runat="server" Display="Dynamic" OnServerValidate="ValidatorAccountNumbersOnly_ServerValidate" EnableClientScript="false" ErrorMessage="Please use digits only, maximum 11. " 
                    ControlToValidate="TextAccount" /><asp:CustomValidator ID="Validator_Account_NotStartClearing" runat="server" Display="Dynamic" OnServerValidate="ValidatorAccountNoClearing_ServerValidate" EnableClientScript="false" ErrorMessage="Do not include the clearing number." ControlToValidate="TextAccount" />
                                <br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="CheckImpersonate" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="ComboPersonImpersonated" EventName="SelectedPersonChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DropOrganizations" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>                
                <b>Step 3:</b> What <b>amount</b> of cash are you requesting, and <b>for what</b>? Describe in a few words.<br />
                <div style="float:left;width:100px">Amount:<br />Description:</div>
                <asp:TextBox ID="TextAmount" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextAmount_Required" runat="server" 
                                    ErrorMessage="Please type an amount." InitialValue="0,00" EnableClientScript="false" 
                    ControlToValidate="TextAmount" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="Validator_TextAmount_Custom" runat="server" 
                                    ErrorMessage="Please type a numeric amount." 
                    Display="Dynamic" 
                    onservervalidate="Validator_TextAmount_Custom_ServerValidate" />
                                <br /><asp:TextBox ID="TextDescription" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextDescription_Required" runat="server" EnableClientScript="false"
                                    ErrorMessage="Please describe your request." 
                    ControlToValidate="TextDescription" />
                                <br />
                <b>Step 4:</b> What <b>budget</b> should be charged within <asp:Label ID="LabelOrganizationCopy" runat="server">the organization</asp:Label>? The budget owner must attest.<br />
                <div style="float:left;width:100px">Budget:</div>
                <pw4:FinancialAccountTreeDropDown ID="DropBudgets" runat="server" OnSelectedNodeChanged="DropBudgets_SelectedNodeChanged" />
                                
                                &nbsp;<asp:Label ID="LabelBudgetOwner" runat="server" />
                                <asp:CustomValidator ID="Validator_DropBudgets_Custom" runat="server" 
                                    ErrorMessage="Please select a budget." 
                                    onservervalidate="Validator_DropBudgets_Custom_ServerValidate" /><br />
                <b>Step 5:</b> All done? Send off your cash advance request for attestation by the budget owner, after which the cash will be paid out.<br />
                <div style="float:left;width:100px">All done?</div>
                <asp:Button ID="ButtonSubmitClaim" runat="server" Text="All Done!" 
                    onclick="ButtonSubmitClaim_Click" />&nbsp;<br />
            </div>
        </div>
    </div>


</asp:Content>

