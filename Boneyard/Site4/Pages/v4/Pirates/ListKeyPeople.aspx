<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="ListKeyPeople.aspx.cs" Inherits="Pages_v4_ListKeyPeople" Title="List Key People - PirateWeb"
    CodePage="65001" %>

<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/OrganizationTreeDropDown.ascx" TagName="OrganizationTreeDropDown"
    TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/PageTitle.ascx" TagName="PageTitle" TagPrefix="pw4" %>
<%@ Register Src="~/Controls/v4/SelectOrganizationLine.ascx" TagName="SelectOrganizationLine"
    TagPrefix="pw4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div style="position: relative; z-index: 2">
        <pw4:PageTitle Icon="group_key.png" Title="List Key People" Description="Lists officers and activists per organization and geography"
            runat="server" ID="PageTitle" />
        <pw4:SelectOrganizationLine ID="SelectOrganizationLine" OnSelectedNodeChanged="SelectOrganizationLine_SelectedNodeChanged"
            runat="server" />
        <table border="0" cellpadding="0" cellspacing="0" style="border-top: solid 1px #808080">
            <tr>
                <td class="CellGeographySelector" valign="top">
                    <asp:UpdatePanel ID="UpdateGeography" runat="server">
                        <ContentTemplate>
                            <div style="width: 299px; background-color: White; border: solid 1px #C0C0C0">
                                <pw4:GeographyTree ID="GeographyTree" OnSelectedNodeChanged="GeographyTree_SelectedNodeChanged"
                                    runat="server" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="SelectOrganizationLine" EventName="SelectedNodeChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td class="CellFacts" width="100%" valign="top">
                    <asp:UpdatePanel ID="UpdateFacts" runat="server">
                        <ContentTemplate>
                            <asp:Literal ID="LiteralFacts" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
