<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Admin.CreateOrganization" Codebehind="CreateOrganization.aspx.cs" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript">

        $(document).ready(function() {
            $('#<%= TextOrganizationName.ClientID %>').focus();
        });

        function validateFields() {
            return true; // TODO: validate the input fields
        }

    </script>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label ID="BoxTitle" runat="server" /></h2>
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextOrganizationName" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropCreateChild" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropCurrencies"/>&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropPersonLabel" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropActivistLabel" />&#8203;<br/>
        <Swarmops5:DropDown runat="server" ID="DropPositionLabel" />&#8203;<br/>
        <asp:Button ID="ButtonCreate" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick=" return validateFields(); " OnClick="ButtonCreate_Click" Text="CreateXYZ"/>
    </div>
    <div class="entryLabels">
        <asp:Label runat="server" ID="LabelOrganizationName" /><br/>
        <asp:Label runat="server" ID="LabelCreateAs" /><br/>
        <asp:Label runat="server" ID="LabelNativeCurrency" /><br/>
        <asp:Label runat="server" ID="LabelPersonLabel" /><br/>
        <asp:Label runat="server" ID="LabelActivistLabel" /><br/>
        <asp:Label runat="server" ID="LabelPositionLabel" />
    </div>
    <div style="clear: both"></div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

