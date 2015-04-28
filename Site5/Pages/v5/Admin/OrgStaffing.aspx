<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="OrgStaffing.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.OrganizationStaffing" %>
<%@ Register tagPrefix="Swarmops5" tagName="AjaxTextBox" src="~/Controls/v5/Base/AjaxTextBox.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="TreePositions" src="~/Controls/v5/Swarm/TreePositions.ascx"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();
        });

    </script>


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2><asp:Label ID="LabelHeaderStrategic" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositionsStrategic" Level="OrganizationStrategic" runat="server" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2><asp:Label ID="LabelHeaderExecutive" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositionsExecutive" Level="OrganizationExecutive" runat="server" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2><asp:Label ID="LabelHeaderGeographicDefault" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositionsGeographic" Level="GeographyDefault" runat="server" />
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

