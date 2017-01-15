<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Admin.OrganizationStaffing" CodeFile="OrgStaffing.aspx.cs" %>
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
        <div title="<img src='/Images/Icons/iconshock-general-128px.png' width='64' height='64' />">
            <h2><asp:Label ID="LabelHeaderStrategic" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositionsStrategic" Level="OrganizationStrategic" Cookie="Strategic" Height="600" runat="server" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-executive-128px.png' width='64' height='64' />">
            <h2><asp:Label ID="LabelHeaderExecutive" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositionsExecutive" Level="OrganizationExecutive" Cookie="Executive" Height="600" runat="server" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-foreman-128px.png' width='64' height='64' />">
            <h2><asp:Label ID="LabelHeaderGeographicDefault" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositionsGeographic" Level="GeographyDefault" Cookie="Local" Height="600" runat="server" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-redshirt-128px.png' width='64' height='64' />">
            <h2><asp:Label ID="LabelHeaderVolunteers" runat="server" /></h2>
            
        </div>
        <div title="<img src='/Images/Icons/iconshock-soldier-128px.png' width='64' height='64' />">
            <h2><asp:Label ID="LabelHeaderPayroll" runat="server" /></h2>
            
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

