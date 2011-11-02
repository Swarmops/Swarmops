<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FinancialAccountTreeDropDown.ascx.cs" Inherits="Controls_v4_FinancialAccountTreeDropDown" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<%@ Register Src="~/Controls/v4/FinancialAccountTree.ascx" TagName="FinancialAccountTree" TagPrefix="pw4" %>

<script type="text/javascript">

function <%=this.ClientID %>_OnClientNodeClick(sender, args)
{
    var comboBox = $find('<%=this.ClientID %>_DropFinancialAccounts');
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

<telerik:RadComboBox ID="DropFinancialAccounts" runat="server" Height="200px" Width="300px"
    ShowToggleImage="True" Skin="WebBlue" style="vertical-align:middle;" Text="Select Account...">
    <ItemTemplate>
        <div id="<%=this.ClientID %>_divDropFinancialAccountsTree">
            <pw4:FinancialAccountTree ID="FinancialAccountTree" OnSelectedNodeChanged="Tree_SelectedNodeChanged" runat="server" Skin="WebBlue" OnClientNodeClicking="<%=this.ClientID %>_OnClientNodeClick" />
       </div>
    </ItemTemplate>
    <Items>
        <telerik:RadComboBoxItem Text="Select Account..." />
    </Items>
</telerik:RadComboBox>

<script type="text/javascript">
    var <%=this.ClientID %>_divDropFinancialAccountsTree = document.getElementById("<%=this.ClientID %>_divDropFinancialAccountsTree");
    <%=this.ClientID %>_divDropFinancialAccountsTree.onclick = <%=this.ClientID %>_StopPropagation;
</script>
