<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="ChangeOrganizations.aspx.cs" Inherits="Swarmops.Frontend.Pages.Security.ChangeOrganizations" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
    
    <script type="text/javascript">
        $(document).ready(function () {
            $('#OrganizationId').combotree({
                animate: true,
                height: 30
            });
        });
     </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields">
        <asp:Label runat="server" ID="LabelCurrentOrganizationName" />&#8203;<br/>
        <select class="easyui-combotree" url="Json-AccessibleOrganizationsTree.aspx" name="OrganizationId" id="OrganizationId" animate="true" style="width:300px"></select>

        <asp:Button ID="ButtonSwitch" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClick="ButtonSwitch_Click" Text="Switch"/>
    </div>
    <div class="entryLabels">
        <asp:Label runat="server" ID="LabelCurrentOrganization" /><br/>
        <asp:Label runat="server" ID="LabelNewOrganization" /><br/>
    </div>
    <div style="clear:both"></div></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

