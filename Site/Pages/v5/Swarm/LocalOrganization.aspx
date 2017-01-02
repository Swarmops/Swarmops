<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Swarm.LocalOrganization" CodeFile="LocalOrganization.aspx.cs" %>
<%@ Register tagPrefix="Swarmops5" tagName="AjaxTextBox" src="~/Controls/v5/Base/AjaxTextBox.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="TreePositions" src="~/Controls/v5/Swarm/TreePositions.ascx"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            // docready goes here
        });

    </script>


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="LabelHeaderLocal" runat="server" /></h2>
    <Swarmops5:TreePositions ID="TreePositions" Level="Geography" runat="server" />
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

