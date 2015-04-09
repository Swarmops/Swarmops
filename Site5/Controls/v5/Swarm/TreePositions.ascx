<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreePositions.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Swarm.TreePositions" %>

    <script language="javascript" type="text/javascript">

        $(document).ready(function() {

            $('#<%=this.ClientID%>_tablePositions').treegrid(
	        {

	            onLoadSuccess: function () {
                    // Anything here
	            }
	        });

        });


    </script>


    <table id="<%=this.ClientID %>_tablePositions" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="/Automation/Json-PositionsTree.aspx"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="positionTitle">
        <thead>  
            <tr>  
                <th field="positionTitle" width="200"><asp:Literal ID="LiteralHeaderPosition" Text="Position XYZ" runat="server"/></th>  
                <th field="assignedName" width="200"><asp:Literal ID="LiteralHeaderName" Text="AssignedName XYZ" runat="server" /></th>  
                <th field="dateExpires" width="120"><asp:Literal ID="LiteralHeaderExpires" Text="ExpiresDate XYZ" runat="server" /></th>
                <th field="minMax" width="80" align="center"><asp:Literal ID="LiteralHeaderMinMax" Text="Min/Max" runat="server" /></th>
                <th field="actionIcon" width="50" align="center"><asp:Literal ID="LiteralHeaderAction" Text="XYZ" runat="server" /></th>
            </tr>  
        </thead>  
    </table> 
