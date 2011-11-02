<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrganizationTreeDropDown.ascx.cs" Inherits="Controls_v4_OrganizationTreeDropDown" %>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/OrganizationTree.ascx" TagName="OrganizationTree" TagPrefix="pw4" %>



<script type="text/javascript">

function DropOrganizations_OnNodeClick(sender, args)
{
    var comboBox = $find('<%=this.ClientID %>_DropOrganizations');
    var node = args.get_node()
    
    comboBox.set_text(node.get_text());
    
    comboBox.hideDropDown();
    
    var divSelectionLine = $get('UglyHackRemoveThis');
    divSelectionLine.className = 'SelectionLineUnfocused';
}


function StopPropagation(e)
{
     if(!e)
     {
        e = window.event;
     }
    
     e.cancelBubble = true;
}

</script>



            <telerik:RadComboBox ID="DropOrganizations" runat="server" Height="400px" Width="300px"
                ShowToggleImage="True" Skin="WebBlue" style="vertical-align:middle;" Text="Select Organization...">
                <ItemTemplate>
                    <div id="divDropOrganizationsTree">
                        <pw4:OrganizationTree ID="OrganizationTree" OnSelectedNodeChanged="Tree_SelectedNodeChanged" runat="server" Skin="WebBlue" OnClientNodeClicking="DropOrganizations_OnNodeClick" />
                   </div>
                </ItemTemplate>
                <Items>
                    <telerik:RadComboBoxItem Text="Select Organization..." />
                </Items>
            </telerik:RadComboBox>
            <script type="text/javascript">
                var divDropOrganizationsTree = document.getElementById("divDropOrganizationsTree");
                divDropOrganizationsTree.onclick = StopPropagation;
            </script>