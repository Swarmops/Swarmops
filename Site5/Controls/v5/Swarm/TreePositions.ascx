<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreePositions.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Swarm.TreePositions" %>
<%@ Register TagPrefix="Swarmops5Workaround" TagName="ComboPeople" src="~/Controls/v5/Swarm/ComboPeople.ascx" %>

    <script language="javascript" type="text/javascript">

        $(document).ready(function() {

            $('#<%=this.ClientID%>_tablePositions').treegrid(
	        {

	            onLoadSuccess: function () {
	                $('.LocalAssignPerson').click(function () {
	                    <%= this.DropPerson.ClientID%>_val('');
	                    <%= this.DialogAdd.ClientID %>_open();
	                });
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

    <Swarmops5:ModalDialog id="DialogAdd" runat="server">
        <DialogCode>
            <h2>Assigning to ASDSADASDASDASD</h2>
            <div class="entryFields">
                <Swarmops5Workaround:ComboPeople ID="DropPerson" runat="server" />&#8203;<br/>
                <asp:DropDownList ID="DropDuration" runat="server" />&#8203;<br/>
                <input type="button" class="buttonAccentColor" value="Assign"/>
            </div>
            <div class="entryLabels">
                Assign this person to the position:<br/>
                Assignment duration:
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>