<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreePersonRoles.ascx.cs" Inherits="Controls_v5_Swarm_TreePersonRoles" %>

    <script language="javascript" type="text/javascript">

        $(document).ready(function() {

            // initialize table

        });


    </script>


    <table id="<%=this.ClientID %>_tablePositions" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="/Automation/Json-Positions.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="positionTitle">
        <thead>  
            <tr>  
                <th field="positionTitle" width="160"><asp:Literal ID="LiteralHeaderPosition" Text="Position XYZ" runat="server"/></th>  
                <th field="assignedName" width="160"><asp:Literal ID="LiteralHeaderQ1" Text="AssignedName XYZ" runat="server" /></th>  
                <th field="dateExpires" width="80"><asp:Literal ID="LiteralHeaderExpires" Text="ExpiresDate XYZ" runat="server" /></th>
                <th field="reportsToName" width="120"><asp:Literal ID="LiteralHeaderReportsTo" Text="ReportsTo XYZ" runat="server" /></th>
                <th field="flags" width="50" align="center"><asp:Literal ID="Literal2" Text="FLAGS" runat="server" /></th>
                <th field="vol" width="30" align="center"><asp:Literal ID="Literal3" Text="VOL" runat="server" /></th>
                <th field="editIcon" width="30" align="center"><asp:Literal ID="Literal1" Text="EDIT" runat="server" /></th>
            </tr>  
        </thead>  
    </table> 
