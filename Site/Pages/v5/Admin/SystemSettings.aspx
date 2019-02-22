<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Admin.SystemSettingsPage" CodeFile="SystemSettings.aspx.cs" %>
<%@ Register tagPrefix="Swarmops5" tagName="AjaxTextBox" src="~/Controls/v5/Base/AjaxTextBox.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="TreePositions" src="~/Controls/v5/Swarm/TreePositions.ascx"  %>
<%@ Register tagPrefix="Swarmops5" tagName="DropDown" src="~/Controls/v5/Base/DropDown.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();

            setTimeout(function() {

                $('.EditCheck').switchbutton({
                    checkedLabel: SwarmopsJS.unescape('<%= this.Localized_SwitchLabelOn_Upper %>'),
                    uncheckedLabel: SwarmopsJS.unescape('<%= this.Localized_SwitchLabelOff_Upper %>'),
                }).change(function() {

                    if (suppressSwitchResponse) {
                        return;
                    }

                    var jsonData = {};
                    jsonData.switchName = $(this).attr("rel");
                    jsonData.switchValue = $(this).prop('checked');

                    $.ajax({
                        type: "POST",
                        url: "/Pages/v5/Admin/SystemSettings.aspx/SwitchToggled",
                        data: $.toJSON(jsonData),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function(msg) {
                            if (msg.d.Success) {
                                if (msg.d.DisplayMessage.length > 2) { // the 2 is rather arbitrary. Read as "if defined".
                                    alertify.log(msg.d.DisplayMessage);
                                }
                            } else {
                                alertify.error(msg.d.DisplayMessage);
                            }
                        },
                        error: function(msg) {
                            // TODO: Reset switch, protest server unreachable
                        }

                    });
                });

                // Ask for initial data

                $.ajax({
                    type: "POST",
                    url: "/Pages/v5/Admin/SystemSettings.aspx/GetInitialData",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg) {
                        initializeSettings(msg.d);
                    },
                    error: function(msg) {
                        // TODO: try again? bail out?
                        alert("The server did not respond with the organization's current settings.");
                    }

                });
            }, 100); // delay the init of switchbuttons slightly to ensure visibility

        });

        function initializeSettings(systemSettings) {
            suppressSwitchResponse = true;
            $("#CheckRequireSsl").prop("checked", systemSettings.RequireSsl).change();
            suppressSwitchResponse = false;
        }

        function onUrlChange(newText) {
            // Find domain and reset the admin sender name
            // TODO

            // Also, nag an admin who tries to create an unsecure website
            if (newText.length >= 7 && newText.substring(0,7) == 'http://') {
                alertify.log(SwarmopsJS.unescape('<%= this.Localized_Warning_Insecure %>'));
            }
        }

        var suppressSwitchResponse = false;

    </script>

    
    
    <style type="text/css">
        .IconEdit { cursor: pointer; }

        #IconCloseEdit { cursor: pointer; }

        .CheckboxContainer {
            float: right;
            padding-top: 6px;
            padding-right: 8px;
        }
    </style>


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-switch-red-64px.png' />">
            <h2>System Behavior</h2>
            <div class="data-entry-fields">
                <label for="CheckRequireSsl">Force SSL</label><div class="CheckboxContainer"><input type="checkbox" rel="RequireSsl" class="EditCheck" id="CheckRequireSsl"/></div><br/>
            </div>
            <div class="data-entry-labels">
                Require SSL connection?<br/>
            </div>
        </div>
        <div title="<img src='/Images/Icons/iconshock-group-diversified-64px.png' />">
            <h2><asp:Label ID="LabelHeaderSysops" runat="server" /></h2>
            <Swarmops5:TreePositions ID="TreePositions" Level="SystemWide" runat="server" Height="600" />
        </div>
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2><asp:Label ID="LabelHeaderCorrespondenceTransmission" runat="server" /></h2>
            <div class="data-entry-fields">
                <Swarmops5:AjaxTextBox runat="server" ID="TextSmtpServer" Cookie="Smtp" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="localhost:587" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextInstallationName" Cookie="InstallationName" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="localhost:587" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextExternalUrl" Cookie="ExtUrl" OnChange="onUrlChange" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="https://swarmops.example.com/" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextAdminSender" Cookie="AdminSender" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="Swarmops Admin" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextAdminAddress" Cookie="AdminAddress" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="swarmops-admin@example.com" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextWebsocketPortFrontend" Cookie="WebsocketFrontend" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="12172" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextWebsocketPortBackend" Cookie="WebsocketBackend" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" Placeholder="10944" />
                <Swarmops5:AjaxTextBox runat="server" ID="TextBackendHostnameOverride" Cookie="BackendHost" AjaxCallbackUrl="/Pages/v5/Admin/SystemSettings.aspx/StoreCallback" />
            </div>
            <div class="data-entry-labels">
                <asp:Label ID="LabelSmtpServer" runat="server" /><br/>
                <asp:Label ID="LabelInstallationName" runat="server" /><br/>
                <asp:Label ID="LabelExternalUrl" runat="server" /><br />
                <asp:Label ID="LabelAdminSender" runat="server" /><br />
                <asp:Label ID="LabelAdminAddress" runat="server" /><br />
                <asp:Label ID="LabelWebsocketPortFrontend" runat="server" /><br />
                <asp:Label ID="LabelWebsocketPortBackend" runat="server" /><br />
                <asp:Label ID="LabelBackendHostnameOverride" runat="server" /><br />
            </div>
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

