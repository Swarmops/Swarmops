<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GeographyTreeDropDown.ascx.cs"
    Inherits="Controls_v4_GeographyTreeDropDown" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register Src="~/Controls/v4/GeographyTree.ascx" TagName="GeographyTree" TagPrefix="pw4" %>

<script type="text/javascript">

function <%=this.ClientID %>_OnClientNodeClick(sender, args)
{
    var comboBox = $find('<%=this.ClientID %>_DropGeographies');
    var node = args.get_node()
    
    comboBox.set_text(node.get_text());
    comboBox.hideDropDown();
}


function <%=this.ClientID %>_StopPropagation(e)
{
     if(!e)
     {
        e = window.event;
     }
    
     e.cancelBubble = true;
}

</script>

<telerik:RadComboBox ID="DropGeographies" runat="server" Height="400px" Width="300px"
    ShowToggleImage="True" Skin="WebBlue" Style="vertical-align: middle;" Text="Select Geography...">
    <ItemTemplate>
        <div id="<%= this.ClientID %>_divDropGeographiesTree">
            <pw4:GeographyTree ID="GeographyTree" OnSelectedNodeChanged="Tree_SelectedNodeChanged"
                runat="server" Skin="WebBlue" OnClientNodeClicking="<%=this.ClientID %>_OnClientNodeClick" />
            <%-- NOTE: 
                    OnClientNodeClicking="<%=this.ClientID %>_OnClientNodeClick" does not work, replacements can't be done like that in a server control. 
                    Fixed it in page load instead /JL
            --%>
        </div>
    </ItemTemplate>
    <Items>
        <telerik:RadComboBoxItem Text="Select Geography..." />
    </Items>
</telerik:RadComboBox>

<script type="text/javascript">
    var <%=this.ClientID %>_divDropGeographiesTree = document.getElementById("<%=this.ClientID %>_divDropGeographiesTree");
    <%=this.ClientID %>_divDropGeographiesTree.onclick = <%=this.ClientID %>_StopPropagation;
</script>

