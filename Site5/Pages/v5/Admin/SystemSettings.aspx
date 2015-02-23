<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="SystemSettings.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.SystemSettingsPage" %>
<%@ Register tagPrefix="Swarmops5" tagName="AjaxTextBox" src="~/Controls/v5/Base/AjaxTextBox.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="TreePositions" src="~/Controls/v5/Swarm/TreePositions.ascx"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();
        });

        function onUrlChange(newText) {
            // Find domain and reset the admin sender name
            // TODO

            // Also, nag an admin who tries to create an unsecure website
            if (newText.length >= 7 && newText.substring(0,7) == 'http://') {
                alertify.log("<%=Resources.Pages.Admin.SystemSettings_Warning_Insecure %>");
            }
        }

    </script>


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2>System Administrators</h2>
            <Swarmops5:TreePositions ID="TreePositions" runat="server" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Correspondence transmission settings</h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox runat="server" ID="TextSmtpServer" Cookie="Smtp" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="localhost:587" />&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextInstallationName" Cookie="InstallationName" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="localhost:587" />&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextExternalUrl" Cookie="ExtUrl" OnChange="onUrlChange" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="https://swarmops.example.com/" />&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextAdminSender" Cookie="AdminSender" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="Swarmops Admin" />&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextAdminAddress" Cookie="AdminAddress" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="swarmops-admin@example.com" />&#8203;<br/>
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelSmtpServer" runat="server" /><br/>
                <asp:Label ID="LabelInstallationName" runat="server" /><br/>
                <asp:Label ID="LabelExternalUrl" runat="server" /><br />
                <asp:Label ID="LabelAdminSender" runat="server" /><br />
                <asp:Label ID="LabelAdminAddress" runat="server" /><br />
            </div>
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

