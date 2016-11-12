<%@ Page Language="C#" MasterPageFile="/PirateWeb-v4.master" AutoEventWireup="true"
    CodeFile="List.aspx.cs" Inherits="Pages_Members_List" Title="PirateWeb - List Members"
    meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/v4/v3/PersonList.ascx" TagName="PersonList" TagPrefix="uc1" %>
<%@ Register Src="../../../../jQuery/ServerControls/TreeDropdown.ascx" TagName="TreeDropdown"
    TagPrefix="uc2" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .floatcontainer:after
        {
            content: ".";
            display: block;
            height: 0;
            font-size: 0;
            clear: both;
            visibility: hidden;
            zoom: 1.001;
        }
        .floatcontainer
        {
            zoom: 1.001;
            display: block;
        }
        /* Hides from IE Mac */* html .floatcontainer
        {
            height: 1%;
        }
        .floatcontainer
        {
            display: block;
        }
        /* End Hack */</style>
    <style type="text/css">
        #theList .odd
        {
            background-color: #EEEEEE;
        }
        #theList .even
        {
            background-color: white;
        }
        #theList .hover
        {
            color: red;
        }
        #theList .selected
        {
            border-top: 2px solid black !important;
            border-bottom: 2px solid black !important;
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
            <asp:Label ID="LabelHeader" runat="server" Text="List Members" Font-Size="Large"
                meta:resourcekey="LabelHeaderResource1"></asp:Label>
        </h1>
        <table style="max-width: 100%;">
            <tr>
                <td>
                    <div class="floatcontainer">
                        <div style="position: relative;">
                            <div style="position: relative; z-index: 2" class="floatcontainer">
                                <uc2:TreeDropdown ID="DropOrganizations" runat="server" OnSelectedIndexChanged="DropOrganizations_SelectedIndexChanged"
                                    AutoPostBack="True" />
                            </div>
                            <br />
                            <div style="position: relative; z-index: 1" class="floatcontainer">
                                <uc2:TreeDropdown ID="DropGeographies" runat="server" OnSelectedIndexChanged="DropGeographies_SelectedIndexChanged"
                                    AutoPostBack="True" />
                            </div>
                        </div>
                    </div>
                </td>
                <td>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
                </td>
                <td colspan="3">
                    <asp:Label ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Duplicate and expiry finder limited to max 2500 people at a time because of performance."></asp:Label>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                    <asp:Button ID="ButtonList" runat="server" OnClick="ButtonList_Click" Text="List"
                        meta:resourcekey="ButtonListResource1" />
                </td>
                <td>
                    &nbsp;
                </td>
                <td colspan="3">
                    <asp:Button ID="ButtonDupeList" runat="server" OnClick="ButtonDupeList_Click" Text="Find Duplicates (by email, phone, name)"
                        meta:resourcekey="ButtonDupeListResource1" />
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Button ID="ButtonExpiringList" runat="server" OnClick="ButtonExpiringList_Click"
                        Text="Find expiring memberships" meta:resourcekey="ButtonExpiringListResource1" />
                </td>
                <td>
                    <asp:Panel ID="PanelForwards" runat="server" Wrap="False">
                        <span class="normalText">
                            <asp:Literal ID="Literal2" runat="server" meta:resourcekey="Literal2Resource1" Text="within "></asp:Literal>
                            <asp:TextBox ID="ExpiryDays" runat="server" Width="40px"></asp:TextBox>
                            <asp:Literal ID="Literal3" runat="server" meta:resourcekey="Literal3Resource1" Text="days"></asp:Literal>
                        </span>
                    </asp:Panel>
                </td>
                <td>
                    <asp:Panel ID="PanelBackwards" runat="server">
                        <span class="normalText">
                            <asp:Literal ID="Literal4" runat="server" Text="since"></asp:Literal>
                            <asp:TextBox ID="SinceDays" runat="server" Width="40px"></asp:TextBox>
                            <asp:Literal ID="Literal5" runat="server" Text="days"></asp:Literal>
                        </span>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <hr />
        <asp:Label ID="LabelListHeader" Font-Size="Large" runat="server" meta:resourcekey="LabelListHeaderResource1"></asp:Label>
        <asp:Label ID="LabelExpiryHeader" Font-Size="Large" runat="server" Visible="False"
            Text="Expiring members" meta:resourcekey="LabelExpiryHeaderResource1"></asp:Label>
        <asp:Label ID="LabelDupeHeader" Font-Size="Large" runat="server" Visible="False"
            Text="Possible Duplicates" meta:resourcekey="LabelDupeHeaderResource1"></asp:Label>
        <div id="theList">
            <uc1:PersonList ID="PersonList" runat="server" />
        </div>
    </div>
</asp:Content>
