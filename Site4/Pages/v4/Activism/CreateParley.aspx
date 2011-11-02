<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="CreateParley.aspx.cs" Inherits="Pages_v4_Activism_CreateParley" Title="Create Conference - PirateWeb" %>

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

    <pw4:PageTitle Icon="balloon--plus.png" Title="Create Conference" Description="Create a new conference"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Create a new conference</span><br />
            <div class="DivGroupBoxContents">
                <asp:Wizard ID="WizardCreateConference" runat="server" DisplaySideBar="false" Width="100%" OnFinishButtonClick="WizardCreateConference_Finish">
                    <WizardSteps>
                        <asp:WizardStep ID="WizardStepWelcome" runat="server" StepType="Start">
                            <h2>So you want to create a conference?</h2>
                            <span style="line-height:120%">
                            <p>Creating a conference with PirateWeb is easy. You get self-signup, budgets, options and guest lists taken care of. What <b>you</b> need to provide, before we start, are these items:</p>
                            <p><ul><li>A budget estimate: how much money do you <b>plan</b> to spend (lose) on the conference? Different organizations have different policies for this. For PPSE, this must be zero or negative.</li>
                            <li>A guarantee: in the worst possible scenario, how much do you need to be <b>allowed</b> to lose on the conference?</li>
                            <li>An attendance fee. (Self-signups will be invoiced this amount, plus options below.)</li>
                            <li>A list of options and their fee. Things such as lunch, dinner, etc, that are optional.</li>
                            <li>A choice of budget. Usually, this comes from "conference initiatives" or a similar budget. The budget owner will need to attest your guarantee, after which you will get your budget.</li></ul></p>
                            <p>Once you have these items, let's go ahead and create the conference!</p></span>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStepNameDate" runat="server" StepType="Step" >
                            <h2>Conference name, date, organization, and budget</h2>
                            <asp:UpdatePanel ID="UpdateOrgBudget" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div style="float:left" >
                                        What is the <b>name</b> of the conference?&nbsp;&nbsp;<br />
                                        What <b>dates</b> will it be held?<br />
                                        What is the <b>webpage</b> with more info?&nbsp;&nbsp;<br />
                                        What <b>organization</b> is hosting?<br />
                                        What <b>budget</b> should be charged?<br />
                                        <b>Where</b> is it going to be held?&nbsp;&nbsp;
                                    </div>
                                    <asp:TextBox ID="TextConferenceName" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_ConferenceName" runat="server" EnableClientScript="false" Display="Dynamic" Text="You must assign a conference name." ControlToValidate="TextConferenceName" /> <br />
                                    <telerik:RadDatePicker ID="DateStart" runat="server" /> for <asp:TextBox ID="TextDateCount" Text="1" runat="server" /> day(s) <asp:CustomValidator ID="Validator_DatesOk" runat="server" Display="Dynamic" Text="Please select valid dates for the conference." />&nbsp;<br />
                                    <asp:TextBox ID="TextConferenceUrl" runat="server" />&nbsp;<asp:RequiredFieldValidator ID="Validator_ConferenceUrl" runat="server" Display="Dynamic" EnableClientScript="false" Text="You must provide a page with more info." ControlToValidate="TextConferenceUrl" /> <br />
                                    <asp:DropDownList ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged" AutoPostBack="true"><asp:ListItem Text="-- Select organization --" Value="0" /></asp:DropDownList>&nbsp;<asp:CompareValidator ID="Validator_Organization" runat="server" Display="Dynamic" Text="Select an organization." Operator="NotEqual" ValueToCompare="0" ControlToValidate="DropOrganizations" EnableClientScript="false" /> <br />
                                    <pw4:FinancialAccountTreeDropDown ID="DropBudgets" runat="server" /> <asp:CustomValidator ID="Validate_DropBudgets" runat="server" Text="Please select a budget." OnServerValidate="Validate_DropBudgets_ServerValidate" Display="Dynamic" />&nbsp;<br />
                                    <pw4:GeographyTreeDropDown ID="DropGeographies" runat="server" Enabled="true" />&nbsp;
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="DropOrganizations" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStepDescription">
                            <h2>Description</h2>
                            <span style="line-height:120%">
                            <p>Describe the conference in a few sentences. This will be listed under the conference name.</p>
                            <asp:TextBox ID="TextDescription" runat="server" TextMode="MultiLine" Rows="10" Columns="80" /></span>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStepBasicFinancials" runat="server" StepType="Step">
                            <h2>Financial Basics</h2>
                            <p style="line-height:120%">Ok, this is it. This is where you need to have your budget estimates pulled out and actually have a decent guesstimate at how many you expect to come to your conference based on a certain price point, and have some sort of clue as to how much that arrangement is going to cost. In the next page, we'll add the attendee options. These are the basics. Note that if your conference is cancelled, the attendance fees will be refunded. You should factor this into your worst case.</p>
                            <div style="float:left">
                                What <b>budget amount</b> are you requesting?&nbsp;&nbsp;<br />
                                What <b>guarantee</b> do you need, worst-case?&nbsp;&nbsp;<br />
                                What is the basic <b>attendance fee</b>?
                            </div>
                            <asp:TextBox ID="TextBudgetRequested" runat="server" />&nbsp;<asp:CustomValidator ID="Validator_BudgetRequested" runat="server" Display="Dynamic" Text="Please request a budget of at most 0." ControlToValidate="TextBudgetRequested" /><br />
                            <asp:TextBox ID="TextGuaranteeRequested" runat="server" />&nbsp;<asp:CustomValidator ID="Validator_GuaranteeRequested" runat="server" Display="Dynamic" Text="Please request a guarantee of at most 10000." ControlToValidate="TextGuaranteeRequested" /><br />
                            <asp:TextBox ID="TextAttendanceFee" runat="server" />&nbsp;<asp:CustomValidator ID="Validator_AttendanceFee" runat="server" Display="Dynamic" Text="Please choose an attendance fee." ControlToValidate="TextAttendanceFee" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStepOptions" runat="server" StepType="Step">
                            <h2>Options for the attendees</h2>
                            <p style="line-height:120%">This is the final step. Here, you create the extras available to conference attendees, along with their cost. (This list can be modified later, though already-ordered options at that point will stand.) Typical options may include lunch options, wine with dinner, dietary requirements, etc.</p>
                            <asp:UpdatePanel ID="UpdateOptions" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:Literal ID="LiteralItems" runat="server" />
                                    <asp:TextBox ID="TextNewOptionDescription" runat="server" />&nbsp;<asp:TextBox ID="TextNewOptionAmount" runat="server" />&nbsp;<asp:Button ID="ButtonAddOption" runat="server" Text="Add" onclick="ButtonAddOption_Click" /><br />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStepFinish" runat="server" StepType="Finish">
                            <h2>All done!</h2>
                            <p style="line-height:120%">We now have all the data we need. When you hit Finish, the conference will be created, subject to attestation by the budget owner of the budget you selected.</p>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStepComplete" runat="server" StepType="Complete">
                            <h2>Conference created!</h2>
                            <p style="line-height:120%">The conference has been created. Once your budget and guarantee have been attested, you will receive web page code for self-signup and the ability to add guests.</p>
                        </asp:WizardStep>
                    </WizardSteps>
                </asp:Wizard>
            </div>
        </div>
    </div>


</asp:Content>

