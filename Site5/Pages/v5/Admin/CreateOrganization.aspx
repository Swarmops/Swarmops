<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="CreateOrganization.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.CreateOrganization" %>
<%@ Register TagPrefix="Swarmops5" TagName="ExternalScripts" Src="~/Controls/v5/UI/ExternalScripts.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <Swarmops5:ExternalScripts ID="ExternalScripts1" Package="easyui" Control="tree" runat="server" />
	<link rel="stylesheet" type="text/css" href="/Style/v5-easyui-elements.css">

    <script type="text/javascript">

        $(document).ready(function () {
            $('#<%=this.TextOrganizationName.ClientID %>').focus();

            $('#<%=this.DropCreateChild.ClientID %>').combobox({
                editable: false,
                height: 30,
                width: 300
            });

            $('#<%=this.DropCurrencies.ClientID%>').combobox({
                editable: false,
                height: 30,
                width: 300
            });

            $('#<%=this.DropPersonLabel.ClientID%>').combobox({
                editable: false,
                height: 30,
                width: 300
            });

            $('#<%=this.DropActivistLabel.ClientID%>').combobox({
                editable: false,
                height: 30,
                width: 300
            });
        });

        function validateFields() {
            return true; // TODO: validate the input fields
        }

    </script>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields">
        <asp:TextBox runat="server" ID="TextOrganizationName" />&#8203;<br/>
        <asp:DropDownList runat="server" ID="DropCreateChild" />&#8203;<br/>
        <asp:DropDownList runat="server" ID="DropCurrencies"/>&#8203;<br/>
        <asp:DropDownList runat="server" ID="DropPersonLabel" />&#8203;<br/>
        <asp:DropDownList runat="server" ID="DropActivistLabel" />&#8203;<br/>
        <asp:Button ID="ButtonCreate" runat="server" CssClass="buttonAccentColor NoInputFocus" OnClientClick="return validateFields();" OnClick="ButtonCreate_Click" Text="CreateXYZ"/>
    </div>
    <div class="entryLabels">
        <asp:Label runat="server" ID="LabelOrganizationName" /><br/>
        <asp:Label runat="server" ID="LabelCreateAs" /><br/>
        <asp:Label runat="server" ID="LabelNativeCurrency" /><br/>
        <asp:Label runat="server" ID="LabelPersonLabel" /><br/>
        <asp:Label runat="server" ID="LabelActivistLabel" />
    </div>
    <div style="clear:both"></div>
</asp:Content>



<asp:Content ID="Content6" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

