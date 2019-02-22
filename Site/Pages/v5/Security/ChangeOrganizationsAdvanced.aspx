<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Security.ChangeOrganizationsAdvanced" Codebehind="ChangeOrganizationsAdvanced.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <script type="text/javascript">
        $(document).ready(function () {
            $('#DropOrganizations').combotree({
                animate: true,
                height: 30
            });
        });
     </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="data-entry-fields">
    <span style="margin-left:12px"><asp:Label runat="server" ID="LabelCurrentOrganizationName" /></span>&nbsp;<br/>
        <select class="easyui-combotree" url="Json-AccessibleOrganizationsTree.aspx" name="DropOrganizations" id="DropOrganizations" animate="true" style="width:300px" height="30"></select>

        <asp:Button ID="ButtonSwitch" runat="server" CssClass="button-accent-color suppress-input-focus" OnClick="ButtonSwitch_Click" Text="Switch"/>
    </div>
    <div class="data-entry-labels">
        <asp:Label runat="server" ID="LabelCurrentOrganization" /><br/>
        <asp:Label runat="server" ID="LabelNewOrganization" />
    </div>
    <div style="clear:both"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

