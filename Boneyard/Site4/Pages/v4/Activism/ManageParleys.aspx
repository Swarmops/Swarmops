<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="ManageParleys.aspx.cs" Inherits="Activizr.Site.Pages.v4.Activism.ManageParleys" Title="Manage Budget - PirateWeb" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/Financial/Ledger.ascx" TagName="Ledger" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

    <pw4:PageTitle Icon="balloons.png" Title="Manage Conferences" Description="Manage conferences" runat="server" ID="PageTitle" />

    <div class="DivMainContent">
    <div class="DivGroupBox">
    <span class="DivGroupBoxTitle">Select Conference</span><br />
    <div class="DivGroupBoxContents">
    Conference: <asp:DropDownList ID="DropParleys" runat="server" /> <asp:Button ID="ButtonSelectConference" runat="server" Text="Select" OnClick="ButtonSelectConference_Click" />
    </div>
    </div>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr valign="top"><td width="50%">
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Summary</span><br />
                <div class="DivGroupBoxContents">
                <asp:PlaceHolder ID="PlaceHolderSummary" runat="server" />
                </div>
            </div>
            <div class="DivGroupBox">
                <span class="DivGroupBoxTitle">Guest List</span><br />
                <div class="DivGroupBoxContents">
                <span style="line-height:120%"><asp:PlaceHolder ID="PlaceHolderGuests" runat="server" /></span>
                </div>
            </div>
        </td>
        <td width="50%">
            <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Ledger</span><br />
                <div class="DivGroupBoxContents">
                    <asp:UpdatePanel ID="UpdateLedger" runat="server" >
                        <ContentTemplate>
                            <pw4:Ledger ID="Ledger" runat="server" SimplifiedView="true" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </td></tr>
    </table>
    
    <br clear="all" />
    </div>


</asp:Content>

