<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="EditOrganization.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.EditOrganization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#divTabs').tabs();
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-switch-red-64px.png' />">
            <h2>Testing inline header: switches</h2>
            Work in progress
        </div>
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2>Testing inline header: members policy</h2>
            work in progress
        </div>
        <div title="<img src='/Images/Icons/iconshock-colorswatch-64px.png' />">
            <h2>Communications profile and branding</h2>
            work in progress
        </div>
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Transmitting mail</h2>
            Work in progress
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

