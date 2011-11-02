<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="AddMotion.aspx.cs" Inherits="Pages_v4_Organization_AddMotion" Title="Untitled Page" %>

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

    <pw4:PageTitle Icon="script_add.png" Title="Add motion" Description="Add motion for an upcoming meeting"
        runat="server" ID="PageTitle" />

    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Ny årsmötesmotion</span><br />
            <div class="DivGroupBoxContents">
            <div style="float:left">Beteckning:<br />Rubrik:<br />Diskussionstrådslänk:&nbsp;</div>
                <asp:TextBox ID="TextDesignation" runat="server" /><asp:RequiredFieldValidator ID="Validator_Designation_Required" runat="server" ControlToValidate="TextDesignation" Text="*" ErrorMessage="*" Display="Dynamic" />&nbsp;<br />
                <asp:TextBox ID="TextTitle" runat="server" /><asp:RequiredFieldValidator ID="Validator_Title_Required" runat="server" ControlToValidate="TextTitle" Text="*" ErrorMessage="*" Display="Dynamic" />&nbsp;<br />
                <asp:TextBox ID="TextThreadUrl" runat="server" /><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextThreadUrl" Text="*" ErrorMessage="*" Display="Dynamic" />&nbsp;<br />
                <p>Bakgrundstext:<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextPreamble" Text="*" ErrorMessage="*" Display="Dynamic" /></p>
                <asp:TextBox ID="TextPreamble" TextMode="MultiLine" Rows="10" Wrap="true" runat="server" />
                
                <p>Att-satser (en per rad):<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextAmendments" Text="*" ErrorMessage="*" Display="Dynamic" /></p>
                <asp:TextBox ID="TextAmendments" TextMode="MultiLine" Rows="10" Wrap="false" runat="server" />
                <p><asp:Button ID="ButtonAdd" runat="server" Text="Spara" 
                        onclick="ButtonAdd_Click" />
            </div>
        </div>
    </div>


</asp:Content>

