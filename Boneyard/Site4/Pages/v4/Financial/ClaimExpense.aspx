<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ClaimExpense.aspx.cs" Inherits="Pages_v4_Accounting_ClaimExpense" Title="Claim an Expense - PirateWeb" %>

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

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="coins_add.png" Title="Claim Expense" Description="File a claim for an expense you've made on behalf of the organization"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Place a new expense claim</span><br />
            <div class="DivGroupBoxContents">
                
                <b>Step 1:</b> What <b>organization</b> are you filing an expense claim against?<br />
                <div style="float:left;width:100px">Organization:</div><asp:DropDownList ID="DropOrganizations" runat="server"><asp:ListItem Selected="True" Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br />
                <b>Step 2:</b> Verify your <b>bank details</b>, so an expense can be paid out.<br />
                <div style="float:left;width:100px">Bank:<br />Clearing:<br />Account:</div>
                <asp:TextBox ID="TextBank" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextBank_Required" runat="server" EnableClientScript="false" 
                                    ErrorMessage="Please enter your bank's name."
                    ControlToValidate="TextBank" /><br />
                                
                    <asp:TextBox ID="TextBankClearing" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="RequiredFieldValidator1" runat="server" EnableClientScript="false" Display="Dynamic"  
                                    ErrorMessage="Please enter the clearing number (four digits)."
                    ControlToValidate="TextBankClearing" /><asp:CustomValidator ID="Validator_Clearing_NumbersOnly" runat="server" Display="Dynamic" OnServerValidate="ValidatorClearingNumbersOnly_ServerValidate" EnableClientScript="false" ErrorMessage="Please enter exactly four (4) digits." ControlToValidate="TextBankClearing" /><br />
                    
                    <asp:TextBox ID="TextAccount" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                    ID="Validator_TextAccount_Required" runat="server" EnableClientScript="false" Display="Dynamic" 
                    ErrorMessage="Please enter your bank account for reimbursement." 
                    ControlToValidate="TextAccount" />
                    <asp:CustomValidator ID="Validator_Account_NumbersOnly" runat="server" Display="Dynamic" OnServerValidate="ValidatorAccountNumbersOnly_ServerValidate" EnableClientScript="false" ErrorMessage="Please use digits only, maximum 11. " 
                    ControlToValidate="TextAccount" /><asp:CustomValidator ID="Validator_Account_NotStartClearing" runat="server" Display="Dynamic" OnServerValidate="ValidatorAccountNoClearing_ServerValidate" EnableClientScript="false" ErrorMessage="Do not include the clearing number." ControlToValidate="TextAccount" />
                                <br />
                <asp:UpdatePanel ID="UpdatePreattestation" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <b>Step 3:</b> Has this expense been <b>previously approved</b> by a budget owner?<br />
                    <div style="float:left;width:100px">&nbsp;<br />&nbsp;</div>
                    <asp:RadioButton ID="RadioPreattested" GroupName="GroupPreattested" runat="server" Text="Yes, use this approval:" 
                                            AutoPostBack="True" oncheckedchanged="RadioPreattested_CheckedChanged" />&nbsp;<asp:DropDownList 
                                            ID="DropPreattested" runat="server" AutoPostBack="True" 
                                            onselectedindexchanged="DropPreattested_SelectedIndexChanged" />&nbsp;<asp:CustomValidator 
                                    ID="Validator_DropPreattested_SelectionRequired" runat="server" 
                                    ErrorMessage="Please select an approved expense." 
                                    onservervalidate="Validate_DropPreattested_SelectionRequired" /><br />
                    <asp:RadioButton ID="RadioNewClaim" GroupName="GroupPreattested" runat="server" Text="No, this was not previously approved." 
                                            AutoPostBack="True" oncheckedchanged="RadioNewClaim_CheckedChanged" />&nbsp;<br />                                    
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DropPreattested" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>

                <b>Step 4:</b> What <b>amount</b> is your expense claim, <b>when</b> did it happen, and what was it <b>for</b>? Describe in a few words.<br />
                <div style="float:left;width:100px">Amount:<br />Date:<br />Description:</div>
                    <asp:UpdatePanel ID="UpdateInputFields" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                                    <asp:TextBox ID="TextAmount" runat="server" AutoPostBack="True" 
                                        ontextchanged="TextAmount_TextChanged" />&nbsp;<asp:RequiredFieldValidator EnableClientScript="false" 
                                    ID="Validator_TextAmount_Required" runat="server" 
                                    ErrorMessage="Please type an amount." InitialValue="0,00" 
                    ControlToValidate="TextAmount" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="Validator_TextAmount_Custom" runat="server" 
                                    ErrorMessage="Please type a numeric amount." 
                    Display="Dynamic" 
                    onservervalidate="Validator_TextAmount_Custom_ServerValidate" />
<asp:Image ID="IconWarningExceedage" ImageUrl="~/Images/Public/Fugue/icons-shadowless/exclamation.png" runat="server" ImageAlign="TextTop" Visible="false" /><asp:Label ID="LabelWarningExceedage" runat="server" Text=" The amount exceeds what has been approved. A new attestation will be required." Visible="false" />
                                <br /><telerik:RadDatePicker ID="DatePicker" runat="server" />&nbsp;<asp:CustomValidator 
                                    ID="Validator_DatePicker_Custom" runat="server" 
                                    ErrorMessage="Please select a valid non-future date." 
                                    onservervalidate="Validator_DatePicker_Custom_ServerValidate" />
                                <br /><asp:TextBox ID="TextDescription" runat="server" /><asp:Label ID="LabelDescription" runat="server" Visible="false" />&nbsp;<asp:RequiredFieldValidator EnableClientScript="false" 
                                    ID="Validator_TextDescription_Required" runat="server" 
                                    ErrorMessage="Please describe your expense." 
                    ControlToValidate="TextDescription" /></ContentTemplate>
                    <Triggers><asp:AsyncPostBackTrigger ControlID="TextAmount" EventName="TextChanged" /><asp:AsyncPostBackTrigger ControlID="DropPreattested" EventName="SelectedIndexChanged" /></Triggers></asp:UpdatePanel>

                <asp:UpdatePanel ID="UpdatePanelBudget" runat="server" UpdateMode="Conditional"><ContentTemplate>
                    <b>Step 5:</b> What <b>budget</b> should be charged within <asp:Label ID="LabelOrganizationCopy" runat="server">the organization</asp:Label>? The budget owner must approve.<br />
                    <div style="float:left;width:100px">Budget:</div>
                    <pw4:FinancialAccountTreeDropDown ID="DropBudgets" runat="server" OnSelectedNodeChanged="DropBudgets_SelectedNodeChanged" />
                                    
                                    <asp:Label ID="LabelPreattestedBudget" runat="server" Visible="false" />&nbsp;<asp:Label ID="LabelBudgetOwner" runat="server" />
                                    <asp:CustomValidator ID="Validator_DropBudgets_Custom" runat="server" 
                                        ErrorMessage="Please select a budget." 
                                        onservervalidate="Validator_DropBudgets_Custom_ServerValidate" /><br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RadioNewClaim" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="RadioPreattested" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DropPreattested" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>
                <b>Step 6:</b> Upload <b>documentation</b> supporting the expense (a photo, scan, or screenshot of a receipt or similar).<br />
                <div style="float:left;width:100px">Documents:<br />Upload:<br />&nbsp;</div><asp:Label Visible="false" ID="TemporaryDocumentIdentity" Text="0" runat="server" />
                <pw4:DocumentList ID="DocumentList" runat="server" />
                                <asp:CustomValidator ID="Validator_DocumentList_Custom" runat="server" 
                                    ErrorMessage="Please upload documentation supporting the expense." 
                                    onservervalidate="Validator_DocumentList_Custom_ServerValidate"></asp:CustomValidator>
                                <br />
                <telerik:RadUpload ID="Upload" Runat="server" ControlObjectsVisibility="None" 
                    MaxFileInputsCount="1" EnableFileInputSkinning="false"
                    TargetPhysicalFolder="C:\Data\Uploads\PirateWeb" /><asp:Button ID="ButtonUpload" 
                                    runat="server" Text="Upload file" 
                    onclick="ButtonUpload_Click" CausesValidation="False" />&nbsp;<br clear="all"/>
                <b>Step 7:</b> All done? Send off your claim for attestation by the budget owner, validation by accounting, and payout to you, in that order.<br />
                <div style="float:left;width:100px">All done?</div>
                <asp:Button ID="ButtonSubmitClaim" runat="server" Text="All Done!" 
                    onclick="ButtonSubmitClaim_Click" />&nbsp;<br />
                
                
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Your recent expense claims</span><br />
            <div class="DivGroupBoxContents">
                <telerik:RadGrid ID="GridExpenseClaims" runat="server" GridLines="None" 
                    onitemdatabound="GridExpenseClaims_ItemDataBound" 
                    onitemcreated="GridExpenseClaims_ItemCreated" >
<MasterTableView autogeneratecolumns="False" DataKeyNames="Identity">
    <Columns>
        <telerik:GridBoundColumn HeaderText="Claim#" UniqueName="column1" DataField="Identity" />
        <telerik:GridBoundColumn HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" UniqueName="column2" DataField="ExpenseDate" />
        <telerik:GridBoundColumn HeaderText="Description" UniqueName="column3" DataField="Description" />
        <telerik:GridBoundColumn HeaderText="Amount" 
            UniqueName="column" DataFormatString="{0:N2}" DataField="Amount">
            <HeaderStyle HorizontalAlign="Right" />
            <ItemStyle HorizontalAlign="Right" />
        </telerik:GridBoundColumn>
        <telerik:GridTemplateColumn HeaderText="Claimed" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn3">
            <ItemTemplate>
                <asp:Image ID="ImageClaimed" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Attested" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn2">
            <ItemTemplate>
                <asp:Image ID="ImageAttested" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Validated" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" 
            UniqueName="TemplateColumn1">
            <ItemTemplate>
                <asp:Image ID="ImageValidated" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        <telerik:GridTemplateColumn HeaderText="Repaid" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="TemplateColumn">
            <ItemTemplate>
                <asp:Image ID="ImageRepaid" runat="server" />
            </ItemTemplate>
        </telerik:GridTemplateColumn>
    </Columns>
<RowIndicatorColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>
</MasterTableView>
                </telerik:RadGrid>
            </div>
        </div>
    </div>


</asp:Content>

