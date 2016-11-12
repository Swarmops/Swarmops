<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="EditAccountPlan.aspx.cs" Inherits="Activizr.Site.Pages.v4.Financial.EditAccountPlan" Title="Edit Account Plan - PirateWeb" %>

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
<%@ Register Src="~/Controls/v4/FinancialAccountTree.ascx" TagName="FinancialAccountTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/DocumentList.ascx" TagName="DocumentList" TagPrefix="pw4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="balloon--plus.png" Title="Edit Account Plan" Description="Edits accounts for the organization, as well as their properties"
        runat="server" ID="PageTitle" />

        <div class="DivMainContent">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Which organization?</span><br />
                <div class="DivGroupBoxContents">
                    I want to edit the account plan for
                    <asp:DropDownList ID="DropOrganizations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged" />.
                </div>
            </div>
            
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr valign="top">
                    <td width="25%">
                        <div class="DivGroupBox">
                            <span class="DivGroupBoxTitle">Account Plan</span>
                            <div class="DivGroupBoxContents">
                                <pw4:FinancialAccountTree ID="TreeAccounts" runat="server" OnSelectedNodeChanged="TreeAccounts_SelectedNodeChanged" />
                            </div>
                        </div>
                    </td>
                    <td width="75%">
                        <div class="DivGroupBox">
                            <span class="DivGroupBoxTitle">Account Plan</span>
                            <div class="DivGroupBoxContents">
                                <asp:UpdatePanel ID="UpdateAccountDetails" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                <asp:Panel ID="PanelNoAccountSelected" runat="server">
                                    Please select an account.
                                </asp:Panel>
                                <asp:Panel ID="PanelAccount" runat="server" Visible="false">
                                    <span class="Continuous"><asp:Literal ID="LiteralSelectedAccount" runat="server" /></span><br />
                                    <b>Create new</b> account under current: <asp:TextBox ID="TextNameNew" runat="server" /> of type <asp:DropDownList ID="DropNewAccountTypes" runat="server"><asp:ListItem Text="Asset" /><asp:ListItem Text="Debt" /><asp:ListItem Text="Income" /><asp:ListItem Text="Cost" /></asp:DropDownList> <asp:Button ID="ButtonCreateAccount" runat="server" Text="Create" OnClick="ButtonCreateAccount_Click" /><br />
                                    
                                </asp:Panel>
                                </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
            
        </div>

</asp:Content>

