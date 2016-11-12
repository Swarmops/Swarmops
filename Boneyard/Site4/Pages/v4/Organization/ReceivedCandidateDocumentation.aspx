<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ReceivedCandidateDocumentation.aspx.cs" Inherits="Pages_v4_Organization_ReceivedCandidateDocumentation" Title="HACK Add Ballot - PirateWeb" %>

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

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="script_add.png" Title="Received candidate documentation" Description="Check documentation  as received for a particular candidate"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Candidate</span><br />
            <div class="DivGroupBoxContents">
            <div style="float:left">Candidate:&nbsp;&nbsp;<br />Ballots:&nbsp;</div>
                <pw4:ComboPerson ID="ComboCandidate" runat="server" />&nbsp;<asp:Button ID="ButtonCandidateLookup" runat="server" OnClick="ButtonCandidateLookup_Click" Text="Lookup" /><br />
            <div><asp:Label ID="LabelBallots" runat="server" />&nbsp;</div>
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Verify details for <asp:Label ID="LabelCandidateName" runat="server" Text="---" /></span><br />
            <div class="DivGroupBoxContents">
            <div style="float:left">Personal #:&nbsp;<br />Mobile phone #:&nbsp;<br />Good to go:&nbsp;</div>
                <asp:TextBox ID="TextPersonalNumber" runat="server" /> (verify and correct if needed)<br />
                <asp:TextBox ID="TextPhone" runat="server" /> (verify and correct if needed)<br />
                <asp:Button ID="ButtonGo" runat="server" Text="Good to go!" 
                        onclick="ButtonGo_Click" /> This candidate has provided the needed documentation.
            </div>
        </div>
    </div>


</asp:Content>

