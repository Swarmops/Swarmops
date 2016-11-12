<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WSOrgTree.ascx.cs" Inherits="Controls_v4_WSOrgTree" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript">
        function OrgTree_NodeClicking(sender, args) {
            // ugly hack to get this to work in FF
            if (typeof (DropOrganizations_OnClientNodeClicking) == "function")
                DropOrganizations_OnClientNodeClicking(sender, args);
        }
    </script>

</telerik:RadScriptBlock>
<telerik:RadTreeView ID="Tree" Skin="Web20" runat="server" OnClientNodeClicking="OrgTree_NodeClicking">
    <WebServiceSettings Method="GetNodes" Path="~/WebServices/OrgTreeService.asmx" />
</telerik:RadTreeView>
