<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Admin.CreateOrganization" CodeBehind="CreateOrganization.aspx.cs" CodeFile="CreateOrganization.aspx.cs" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript">

        $(document).ready(function() {
            $('#<%= TextOrganizationName.ClientID %>').focus();
        });

        function validateFields() {
            var newOrganizationName = $('#<%=this.TextOrganizationName.ClientID%>').val();
            if (newOrganizationName.trim().length < 1) {
                alertify.error(SwarmopsJS.unescape('<%=this.Localized_Error_OrganizationNameCannotBeEmpty%>'));
                $('#<%=this.TextOrganizationName.ClientID%>').focus();
                return false;
            }
            return true; 
        }

    </script>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="BoxTitle" runat="server" /></h2>
    <div class="data-entry-fields">
        <div class="stacked-input-control"><asp:TextBox runat="server" ID="TextOrganizationName" /></div>
        <Swarmops5:DropDown runat="server" ID="DropCreateChild" />
        <Swarmops5:DropDown runat="server" ID="DropCurrencies"/>
        <Swarmops5:DropDown runat="server" ID="DropApplicantLabel" />
        <Swarmops5:DropDown runat="server" ID="DropPersonLabel" />
        <Swarmops5:DropDown runat="server" ID="DropActivistLabel" />
        <Swarmops5:DropDown runat="server" ID="DropPositionLabel" />
        <asp:Button ID="ButtonCreate" runat="server" CssClass="button-accent-color suppress-input-focus" OnClientClick=" return validateFields(); " OnClick="ButtonCreate_Click" Text="CreateXYZ"/>
    </div>
    <div class="data-entry-labels">
        <asp:Label runat="server" ID="LabelOrganizationName" /><br/>
        <asp:Label runat="server" ID="LabelCreateAs" /><br/>
        <asp:Label runat="server" ID="LabelNativeCurrency" /><br/>
        <asp:Label runat="server" ID="LabelApplicantLabel" /><br/>
        <asp:Label runat="server" ID="LabelPersonLabel" /><br/>
        <asp:Label runat="server" ID="LabelActivistLabel" /><br/>
        <asp:Label runat="server" ID="LabelPositionLabel" />
    </div>
    <div style="clear: both"></div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

