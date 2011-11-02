<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true" CodeFile="CreateInternalPoll.aspx.cs" Inherits="Pages_v4_Organization_CreateInternalPoll" Title="Create Internal Poll - PirateWeb" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTreeDropDown.ascx" TagName="GeographyTreeDropDown" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTree.ascx" TagName="FinancialAccountTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/FinancialAccountTreeDropDown.ascx" TagName="FinancialAccountTreeDropDown" TagPrefix="pw4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" Runat="Server">

<pw4:PageTitle Icon="chart_add.png" Title="Create Internal Poll" Description="Create a new Condorcet-Schulze or Accept-winner multiple choice poll" runat="server" ID="PageTitle" />


    <div class="DivMainContent">
        <div class="DivGroupBox">
            <span class="DivGroupBoxTitle">Create new poll</span><br />
            <div class="DivGroupBoxContents">
                <div style="float: left">
                    Organization&nbsp;<br />
                    Geography&nbsp;<br />
                    Poll Name&nbsp;<br />
                    Voting 1st day&nbsp;<br />
                    Voting last day&nbsp;<br />
                    Results Type&nbsp;<br />
                    &nbsp;
                </div>
                <asp:DropDownList ID="DropDownList1" runat="server"><asp:ListItem Text="Piratpartiet SE" Value="1" Selected="True" /></asp:DropDownList>&nbsp;<br />
                <pw4:GeographyTreeDropDown ID="DropGeographies" runat="server" />&nbsp;<br />
                <asp:TextBox runat="server" ID="TextPollName" />&nbsp;<br />
                <telerik:RadDatePicker ID="DateFirst" runat="server" Skin="Web20" />&nbsp;<br />
                <telerik:RadDatePicker ID="DateLast" runat="server" Skin="Web20" />&nbsp;<br />
                <asp:DropDownList ID="DropResultsType" runat="server"><asp:ListItem Text="Condorcet-Schulze" Value="1" Selected="True" /></asp:DropDownList>&nbsp;<br />
                <asp:Button ID="ButtonCreate" runat="server" Text="Create" OnClick="ButtonCreate_Click" />
            </div>
        </div>
    </div>


</asp:Content>

