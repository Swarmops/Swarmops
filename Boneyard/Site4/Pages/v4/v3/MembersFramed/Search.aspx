<%@ Page Language="C#" MasterPageFile="~/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="Search.aspx.cs" Inherits="Pages_Members_Search" Title="PirateWeb - Search for members"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/WSOrgTreeDropDown.ascx" TagName="WSOrgTreeDropDown"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/v4/WSGeographyTreeDropDown.ascx" TagName="WSGeographyTreeDropDown"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/v4/WSGeographyTree.ascx" TagName="WSGeographyTree" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/v4/WSOrgTree.ascx" TagName="WSOrgTree" TagPrefix="uc4" %>
<%@ Register Src="~/Controls/v4/v3/PersonList.ascx" TagName="PersonList" TagPrefix="uc1" %>
<%@ Register Src="~/jQuery/ServerControls/TreeDropdown.ascx" TagName="TreeDropdown"
    TagPrefix="uc5" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        #theList .odd
        {
            background-color: #EEEEEE;
        }
        #theList .even
        {
            background-color: white;
        }

        #theList .selected td
        {
            border-top:2px solid black !important;
            border-bottom:2px solid black !important;
        }
    </style>

    <script src="/jQuery/js/jquery.alternate.1.1.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(window).ready(function() {
            $('#theList tbody tr').not('tr:first,tr:last').alternate({}, function(evt) {
                var td = $(evt.target).parents('td').first().get()
                if ($(this).children('td:last').get()[0] === td[0]) {
                    $('#theList tbody tr').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
        });

        function LoadMember(personID) {
            if (typeof parent.LoadMember == 'function') {
                parent.LoadMember(personID);
                return false;
            }
            return true;
        } 
        
         </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" runat="Server">
    <div class="bodyContent">
        <h1 class="ContentHeader">
            <asp:Label ID="labelSearchHeader" runat="server" Font-Size="Large" Text="Sök medlemmar"
                meta:resourcekey="labelSearchHeaderResource1"></asp:Label></h1>
        <asp:Panel runat="server" DefaultButton="ButtonSearch" meta:resourcekey="PanelResource1">
            <table border="0" cellpadding="2">
                <tr>
                    <td>
                        <asp:Label ID="LabelOrganization" runat="server" Text="Organization:"></asp:Label>
                    </td>
                    <td>
                        <uc5:TreeDropdown ID="DropOrganizations" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged" />
                    </td>
                    <td>
                        <asp:Label ID="LabelGeography" runat="server" Text="Geography:"></asp:Label>
                    </td>
                    <td>
                        <uc5:TreeDropdown ID="DropGeographies" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropGeographies_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="labelNamePattern" runat="server" Text="Namn:" meta:resourcekey="labelNamePatternResource1"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextNamePattern" runat="server" meta:resourcekey="TextNamePatternResource1"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="LabelBirthDate" runat="server" Text="Födelsedatum:" meta:resourcekey="LabelBirthDateResource1"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="textPersonalNumber" runat="server" meta:resourcekey="textPersonalNumberResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="labelEmailPattern" runat="server" Text="e-post:" meta:resourcekey="labelEmailPatternResource1"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextEmailPattern" runat="server" meta:resourcekey="TextEmailPatternResource1"></asp:TextBox>
                    </td>
                    <td align="right">
                        &nbsp;
                        <asp:Label ID="LabelCity0" runat="server" Text="PostNummer:" meta:resourcekey="LabelCity0Resource1"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="TextPostalCodePattern" runat="server" meta:resourcekey="TextPostalCodePatternResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="labelMemberNumber" runat="server" Text="Medlems #:" meta:resourcekey="labelMemberNumberResource1"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextMemberNumber" runat="server" meta:resourcekey="TextMemberNumberResource1"></asp:TextBox>
                    </td>
                    <td align="right">
                        <asp:Label ID="LabelCity" runat="server" Text="Ort:" meta:resourcekey="LabelCityResource1"></asp:Label>
                        &nbsp;&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="TextCityPattern" runat="server" meta:resourcekey="textCityResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="ButtonSearch" runat="server" OnClick="ButtonSearch_Click" Text="Search" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;<asp:CheckBox ID="CheckBoxActivist" runat="server" meta:resourcekey="CheckBox1Resource1"
                            Text="Aktivist" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <hr />
        <asp:Label ID="LabelListHeader" runat="server" Font-Size="Large" meta:resourcekey="LabelListHeaderResource1"></asp:Label>
        <asp:Label ID="LabelExpiryHeader" runat="server" Font-Size="Large" meta:resourcekey="LabelExpiryHeaderResource1"
            Text="Expiring members" Visible="False"></asp:Label>
        <asp:Label ID="LabelDupeHeader" runat="server" Font-Size="Large" meta:resourcekey="LabelDupeHeaderResource1"
            Text="Possible Duplicates" Visible="False"></asp:Label>
        <div id="theList">
            <uc1:PersonList ID="PersonList" runat="server" />
        </div>
        <br />
        <asp:Literal ID="labelSearchHints" runat="server" Text="labelSearchHints" meta:resourcekey="labelSearchHintsResource1"></asp:Literal>
    </div>
</asp:Content>
