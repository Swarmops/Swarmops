<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WSGeographyTree.ascx.cs"
    Inherits="Controls_v4_WSGeographyTree" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript">
        function GeoTree_NodeClicking(sender, args) {
            // ugly hack to get this to work in FF
            if (typeof (DropGeographies_OnClientNodeClicking) == 'function')
                DropGeographies_OnClientNodeClicking(sender, args);
        }
    </script>

</telerik:RadScriptBlock>
<telerik:RadTreeView ID="Tree" Skin="Web20"  runat="server" 
    onprerender="Tree_PreRender" OnClientNodeClicking="GeoTree_NodeClicking" >
    <WebServiceSettings Method="GetNodes" Path="~/WebServices/GeographyTreeService.asmx" />
</telerik:RadTreeView>
