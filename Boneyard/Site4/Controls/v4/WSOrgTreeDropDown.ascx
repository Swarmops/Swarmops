<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WSOrgTreeDropDown.ascx.cs"
    Inherits="Controls_v4_WSOrgTreeDropDown" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/WSOrgTree.ascx" TagName="WSOrgTree" TagPrefix="pw4" %>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript">
        function DropOrganizations_OnClientNodeClicking(sender, args) {
            var node = args.get_node()
            var tree = node.get_treeView();
            var elem = tree.get_element();
            while (elem != null && elem.id.substring(elem.id.length  - ('_DropDown').length, elem.id.length) != '_DropDown') {
                elem = elem.parentNode;
            }
            var id = elem.id
            id = id.substr(0, id.length - ('_DropDown').length);
            var comboBox = $find(id);
            comboBox.set_text(node.get_text());
            comboBox.set_value(node.get_value());
            comboBox.hideDropDown();
        }

        function StopPropagation(e) {
            if (typeof e == "undefined") {
                e = window.event;
            }
            e.cancelBubble = true;
            return false;
        }

    </script>

</telerik:RadScriptBlock>
<telerik:RadComboBox ID="DropOrganizations" runat="server" Height="400px" Width="300px"
    Skin="Web20" Style="vertical-align: middle;" Text="Select Organization..."
    EnableTextSelection="False" meta:resourcekey="DropOrganizationsResource1">
    <ItemTemplate>
        <div id="divDropOrganizationsTree" onclick='StopPropagation' style="z-index:7001;" >
            <pw4:WSOrgTree ID="OrgTree" runat="server" Skin="Web20" />
        </div>
    </ItemTemplate>
    <Items>
        <telerik:RadComboBoxItem Text="Select Organization..." meta:resourcekey="RadComboBoxItemResource1" />
    </Items>
</telerik:RadComboBox>
