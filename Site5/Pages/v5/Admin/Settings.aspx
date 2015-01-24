<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Admin.Settings" %>
<%@ Register tagPrefix="Swarmops5" tagName="AjaxTextBox" src="~/Controls/v5/Base/AjaxTextBox.ascx"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('#divTabs').tabs();
        });
    </script>


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div id="divTabs" class="easyui-tabs" data-options="tabWidth:70,tabHeight:70">
        <div title="<img src='/Images/Icons/iconshock-mail-open-64px.png' />">
            <h2>Correspondence transmission settings</h2>
            <div class="entryFields">
                <Swarmops5:AjaxTextBox runat="server" ID="TextSmtpServer" Cookie="Smtp" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="localhost:587" />&#8203;<br/>
                <Swarmops5:AjaxTextBox runat="server" ID="TextExternalUrl" Cookie="ExtUrl" AjaxCallbackUrl="/Pages/v5/Admin/Settings.aspx/StoreCallback" Placeholder="https://swarmops.example.com/" />&#8203;<br/>
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelSmtpServer" runat="server" /><br/>
                <asp:Label ID="LabelExternalUrl" runat="server" /><br />
            </div>
        </div>
    </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

