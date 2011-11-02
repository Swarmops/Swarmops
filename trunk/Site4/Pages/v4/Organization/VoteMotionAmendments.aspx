<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="VoteMotionAmendments.aspx.cs" Inherits="Pages_v4_Organization_VoteMotionAmendments" Title="Untitled Page" %>

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

    <pw4:PageTitle Icon="script.png" Title="Vote on Amendments" Description="Place your vote for all the separate motion parts and amendments"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Årsmötet 2010</span><br />
            <div class="DivGroupBoxContents"><div style="float:right">&nbsp;<asp:Button ID="ButtonSave" runat="server" Text="Spara mina val" /><br />&nbsp;<asp:Button ID="Button2" runat="server" Text="Styrelsens rekommendation" /><br />&nbsp;<asp:DropDownList ID="DropVotingLists" runat="server"><asp:ListItem Text="-- voteringslistor --" Selected="True" /><asp:ListItem Text="En medlem (#10001)" /><asp:ListItem Text="En annan medlem (#20002)" /><asp:ListItem Text="Ännu en medlem (#30003)" /><asp:ListItem Text="Ett år i Gulag (#40004)" /><asp:ListItem Text="Nej, nej! Traktor! (#50005)" /></asp:DropDownList>&nbsp;<asp:Button ID="ButtonSelectVotingList" runat="server" Text="OK" /></div>
            Du röstar på att-satser till individuella motioner. Spara din röst med knappen till höger.<br />Om du vill utgå från styrelsens rekommendationer när du röstar, tryck på "Styrelsens rekommendation" så markeras dessa.<br />Om du istället vill utgå från någon individuell medlems rekommendationer, välj vilken här.
        </div>
        </div>
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Rösta på att-satser till årsmötet 2010</span><br />
            <div class="DivGroupBoxContents">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
            <td>&nbsp;</td>
            <td width="100px">&nbsp;</td>
            <td width="40px" style="background-color:#80FF80;text-align:center">Ja</td>
            <td width="40px" style="background-color:#FFC0A0;text-align:center">Nej</td>
            <td width="40px" style="background-color:#E0E080;text-align:center">Avstår</td>
            <td width="100px">&nbsp;Styrelsens&nbsp;rek.</td>
            </tr>
            <asp:Repeater ID="RepeaterMotions" runat="server" 
                    onitemdatabound="RepeaterMotions_ItemDataBound">
            <ItemTemplate>
                <tr><td colspan="6"><b><%# Eval ("Designation") %>. <%# Eval ("Title") %></b> (<a href="<%# Eval("ThreadUrl") %>">diskussion</a>)</td></tr>
                <asp:Repeater ID="RepeaterAmendments" runat="server" onitemcreated="RepeaterAmendments_ItemCreated">
                <ItemTemplate>
                <tr style="line-height:150%">
                    <td><asp:Label ID="LabelText" runat="server" /></td>
                    <td style="text-align:right"><b><asp:Label ID="LabelDesignation" runat="server" /></b>&nbsp;&nbsp;</td>
                    <td style="background-color:#80FF80;text-align:center"><input type="radio" <asp:Literal runat="server" ID="LiteralCheckedYes" /> name=" <asp:Literal  runat="server" ID="LiteralRadioGroupYes" />" value="Y" /></td>
                    <td style="background-color:#FFC0A0;text-align:center"><input type="radio" <asp:Literal runat="server" ID="LiteralCheckedNo" /> name=" <asp:Literal  runat="server" ID="LiteralRadioGroupNo" />" value="N" /></td>
                    <td style="background-color:#E0E080;text-align:center"><input type="radio" <asp:Literal runat="server" ID="LiteralCheckedAbstain" /> name=" <asp:Literal  runat="server" ID="LiteralRadioGroupAbstain" />" value="A" /></td>
                    <td>&nbsp;&nbsp;<asp:Label ID="LabelRecommended" runat="server" /></td>
                </tr>
                </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
            </asp:Repeater>
          
            </table>
        </div>

    </div>
    
    
                    <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Spara din röst</span><br />
            <div class="DivGroupBoxContents"><div style="float:right">&nbsp;<asp:Button ID="Button1" runat="server" Text="Spara Röst" /></div>
            Glöm inte att spara dina röster när du är klar!
        </div>

    </div>


</asp:Content>


