<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="EditBallot.aspx.cs" Inherits="Pages_v4_Organization_EditBallot" Title="HACK Add Ballot - PirateWeb" %>

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

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="script_add.png" Title="Add ballot" Description="Add ballot for an upcoming election"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Väj valsedel</span><br />
            <div class="DivGroupBoxContents">
            <div style="float:left">Valsedel:&nbsp;</div>
                <asp:DropDownList ID="DropBallots" runat="server" />&nbsp;<asp:Button ID="ButtonLookup" runat="server" Text="Lookup" OnClick="ButtonLookup_Click" />
            </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Valsedeln <asp:Label ID="LabelBallotName" runat="server" /></span><br />
            <div class="DivGroupBoxContents">
            <div style="float:left">Geografi:<br />Antal:&nbsp;<br />Leveransadress:&nbsp;</div>
                <asp:Label ID="LabelGeography" runat="server" />&nbsp;<br />
                <asp:TextBox ID="TextBallotCount" runat="server" />&nbsp;<br />
                <asp:TextBox ID="TextDeliveryAddress" TextMode="MultiLine" runat="server" />&nbsp;
                
                <p>Kandidater, i ordning (en per rad):</p>
                <asp:TextBox ID="TextCandidates" TextMode="MultiLine" Rows="10" Wrap="false" runat="server" />
                <p><asp:Button ID="ButtonSave" runat="server" Text="Spara" 
                        onclick="ButtonSave_Click" />
            </div>
        </div>
    </div>


</asp:Content>

