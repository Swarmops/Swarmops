<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreePositions.ascx.cs" Inherits="Swarmops.Frontend.Controls.v5.Swarm.TreePositions" %>
<%@ Register TagPrefix="Swarmops5Workaround" TagName="ComboPeople" src="~/Controls/v5/Swarm/ComboPeople.ascx" %>

    <script language="javascript" type="text/javascript">

        $(document).ready(function() {

            $('#<%=this.ClientID%>_tablePositions').treegrid(
	        {

	            onLoadSuccess: function () {
	                $('.LocalAssignPerson.LocalPosition<%=this.Cookie%>').click(function () {
	                    currentPositionId = $(this).attr("positionId");
	                    $('#<%= this.ClientID %>_modalPositionName').text(decodeURIComponent($(this).attr("positionName")));
	                    <%= this.DropPerson.ClientID%>_clear();
	                    $('#<%= this.DropDuration.ClientID%>').val("12"); // reset to 12-month default
	                    <%= this.DialogAdd.ClientID %>_open();
	                    <%= this.DropPerson.ClientID%>_focus();
	                });
	            }

                // TODO: RowStyler
	        });

            $('#<%=this.ClientID%>_buttonAssign').click(function() {
                var personId = <%=this.DropPerson.ClientID%>_selectedPersonId();

                if (personId == 0) {
                    // abort
                    alert("No person selected, not assigning anybody");
                    return;
                }

                // TODO: Check existing position for person; can't have two positions

                SwarmopsJS.ajaxCall(
                    "/Automation/SwarmFunctions.aspx/AssignPosition",
                    { personId: personId, positionId: currentPositionId, durationMonths: $('#<%= this.DropDuration.ClientID%>').val(), organizationId: 0, geographyId: 0 },
                    function (result) {
                        // Close modal and reload grid regardless of success or not
                        // (the only failure here should be a concurrency error, in which case
                        // the user needs to examine the new state anyway)

                        <%= this.DialogAdd.ClientID %>_close();
                        $('#<%=this.ClientID%>_tablePositions').treegrid('reload');
                        if (result.Success) {
                            // success
                        } else {
                            alertify.error(result.DisplayMessage);
                        }
                    });
            });

        });

        var currentPositionId = '';

    </script>


    <table id="<%=this.ClientID %>_tablePositions" title="" class="easyui-treegrid" style="width:680px;height:600px"  
        url="/Automation/Json-PositionsTree.aspx?Cookie=<%=this.Cookie %>"
        rownumbers="false"
        animate="true"
        fitColumns="true"
        idField="id" treeField="positionTitle">
        <thead>  
            <tr>  
                <th field="positionTitle" width="200"><asp:Literal ID="LiteralHeaderPosition" Text="Position XYZ" runat="server"/></th>  
                <th field="assignedName" width="200"><asp:Literal ID="LiteralHeaderName" Text="AssignedName XYZ" runat="server" /></th>  
                <th field="expires" width="120"><asp:Literal ID="LiteralHeaderExpires" Text="ExpiresDate XYZ" runat="server" /></th>
                <th field="minMax" width="80" align="center"><asp:Literal ID="LiteralHeaderMinMax" Text="Min/Max" runat="server" /></th>
                <th field="actionIcon" width="50" align="center"><asp:Literal ID="LiteralHeaderAction" Text="XYZ" runat="server" /></th>
            </tr>  
        </thead>
    </table> 

    <Swarmops5:ModalDialog id="DialogAdd" runat="server">
        <DialogCode>
            <h2><asp:Label runat="server" ID="LabelModalHeader"/></h2>
            <div class="entryFields">
                <Swarmops5Workaround:ComboPeople ID="DropPerson" runat="server" />&#8203;<br/>
                <asp:DropDownList ID="DropDuration" runat="server" />&#8203;<br/>
                <input type="button" id="<%=this.ClientID %>_buttonAssign" class="buttonAccentColor" value='<asp:Literal ID="LiteralButtonAssign" runat="server"/>' />
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelAssignPersonTo" runat="server" /><br/>
                <asp:Label ID="LabelAssignmentDuration" runat="server" />
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>