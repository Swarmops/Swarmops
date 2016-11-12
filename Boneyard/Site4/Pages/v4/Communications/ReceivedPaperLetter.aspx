<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ReceivedPaperLetter.aspx.cs" Inherits="Pages_v4_Communications_ReceivedPaperLetter" Title="Received Paper Letter - PirateWeb" %>

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

    <pw4:PageTitle Icon="table_add.png" Title="Received Paper Letter" Description="Upload a paper letter that was received for the organization"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
            <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Paper letter received</span><br />
            <div class="DivGroupBoxContents">
                
                <b>Step 1:</b> What <b>organization</b> was the letter addressed to?<br />
                <div style="float:left;width:100px">Organization:</div><asp:DropDownList ID="DropOrganizations" runat="server"><asp:ListItem Selected="True" Text="Piratpartiet SE" Value="1" /></asp:DropDownList>&nbsp;<br />
                <b>Step 2:</b> Who <b>sent</b> this letter, and what is their <b>reply address</b>, if visible?<br />
                <div style="float:left;width:100px">Sender:<br />Reply Address:</div>
                <asp:TextBox ID="TextFrom" runat="server" />&nbsp;<asp:RequiredFieldValidator 
                                    ID="Validator_TextFrom_Required" runat="server" 
                                    ErrorMessage="Please enter the sender's name." 
                    ControlToValidate="TextFrom" />
                                
                                <br /><asp:TextBox ID="TextAddress" TextMode="MultiLine" Rows="4" runat="server" />
                                <br />
                <b>Step 3:</b> Who is this letter <b>for</b> (leave blank if addressed to the organization), and in what <b>role</b>? <b>When</b> was it received? Is it <b>personal</b> (sensitive)?<br />
                <div style="float:left;width:100px">Personal?<br />Recipient:<br />Received:</div>
                <asp:UpdatePanel ID="UpdateWhoWhenWhat" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:DropDownList ID="DropPersonal" runat="server"><asp:ListItem Selected="True" Value="0">-- Select one--</asp:ListItem><asp:ListItem Value="Personal">Personal - has sensitive information</asp:ListItem><asp:ListItem Value="NotPersonal">Not personal, like rally permits</asp:ListItem></asp:DropDownList>&nbsp;
                    <asp:CompareValidator ID="CompareValidator1" ControlToValidate="DropPersonal" Operator="NotEqual" ValueToCompare="0" runat="server"
                       ErrorMessage="Please select if the letter is personal." />&nbsp;
                    <br /><pw4:ComboPerson CausesValidation="False" ID="ComboRecipient" runat="server" OnSelectedPersonChanged="ComboRecipient_SelectedPersonChanged" />&nbsp;<asp:Label ID="LabelRoleAs" runat="server" Visible="false" Text="" />&nbsp;<asp:DropDownList ID="DropRoles" runat="server" Visible="false" />&nbsp;<asp:CompareValidator ID="Validator_DropRoles_Compare" runat="server" Display="Dynamic" ControlToValidate="DropRoles" ValueToCompare="0" Operator="NotEqual" ErrorMessage="Select a role." />
                    <br /><telerik:RadDatePicker ID="DatePicker" runat="server" />&nbsp;<asp:CustomValidator 
                        ID="Validator_DatePicker_Custom" runat="server" 
                        ErrorMessage="Please select a valid non-future date."
                        onservervalidate="Validator_DatePicker_Custom_ServerValidate" /><br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ComboRecipient" EventName="SelectedPersonChanged" />
                </Triggers>
                </asp:UpdatePanel>

                <b>Step 4:</b> Upload <b>a scan or high-res photo</b> of the paper letter and any accompanying information on paper.<br />
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
                <b>Step 5:</b> All done? Store the letter and, if applicable, have the recipient notified.<br />
                <div style="float:left;width:100px">All done?</div>
                <asp:Button ID="ButtonStoreLetter" runat="server" Text="All Done!" 
                    onclick="ButtonStoreLetter_Click" />&nbsp;<br />
                
                
            </div>
        </div>
    </div>


</asp:Content>

