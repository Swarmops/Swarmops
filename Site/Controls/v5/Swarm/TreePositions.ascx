<%@ Control Language="C#" AutoEventWireup="true" Inherits="Swarmops.Frontend.Controls.Swarm.TreePositions" CodeFile="TreePositions.ascx.cs" %>
<%@ Register TagPrefix="Swarmops5Workaround" TagName="ComboPeople" src="~/Controls/v5/Swarm/ComboPeople.ascx" %>
<%@ Register TagPrefix="Swarmops5" TagName="ModalDialog" src="~/Controls/v5/Base/ModalDialog.ascx" %>

    <script language="javascript" type="text/javascript">
        function preload(arrayOfImages) {
            $(arrayOfImages).each(function () {
                (new Image()).src = this;
            });
        }

        preload([
            '/Images/Abstract/ajaxloader-48x36px.gif'
        ]);
        $(document).ready(function() {

            $('#<%=this.ClientID%>_buttonAssign').val("<asp:Literal ID='LiteralButtonAssign' runat='server' />");

            $('#<%=this.ClientID%>_tablePositions').treegrid(
	        {
	            rowStyler: function (rowData) {
	                if (rowData.iconType != null) {
	                    return { class: "iconType" + rowData.iconType };
	                }

	                return '';
	            },

	            onLoadSuccess: function () {
	                $('.LocalAssignPerson.LocalPosition<%=this.Cookie%>').click(function () {
	                    current<%=this.ClientID%>PositionId = $(this).attr("positionId");
	                    $('#<%= this.ClientID %>_modalPositionName').text(decodeURIComponent($(this).attr("positionName")));
	                    $('#<%= this.ClientID %>_modalGeographyName').text('');
	                    var thisGeographyId = $(this).attr("geographyId");
	                    current<%=this.ClientID%>GeographyId = '0';
	                    if (thisGeographyId != '0' && thisGeographyId != '1') {
	                        current<%=this.ClientID%>GeographyId = thisGeographyId;
	                        SwarmopsJS.ajaxCall("/Automation/SwarmFunctions.aspx/GetGeographyName",
	                            { geographyId: thisGeographyId },
	                            function(result) {
	                                if (result.Success) {
	                                    $('#<%= this.ClientID %>_modalGeographyName').text(result.DisplayMessage);
	                                }
	                            });
	                    }
	                    <%= this.DropPerson.ClientID%>_clear();
	                    $('#<%= this.DropDuration.ClientID%>').val("12"); // reset to 12-month default
	                    <%= this.DialogAdd.ClientID %>_open();
	                    <%= this.DropPerson.ClientID%>_focus();
	                });

	                $('.LocalIconTerminate.LocalPosition<%=this.Cookie%>').attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");

	                $('.LocalIconTerminate.LocalPosition<%=this.Cookie%>').click(function() {
	                    if ($(this).attr("rel") != "loading") {
	                        $(this).attr("rel", "loading");

	                        $(this).attr("src", "/Images/Abstract/ajaxloader-48x36px.gif");

	                        var okLabel = decodeURIComponent('<asp:Literal ID="LiteralTerminateYes" runat="server" />');
	                        var cancelLabel = decodeURIComponent('<asp:Literal ID="LiteralTerminateNo" runat="server" />');
	                        var confirmQuestion = decodeURIComponent('<asp:Literal ID="LiteralConfirmTermination" runat="server" />');

	                        if ($(this).attr("self") == "true") {
	                            okLabel = decodeURIComponent('<asp:Literal ID="LiteralTerminateSelfYes" runat="server" />');
	                            cancelLabel = decodeURIComponent('<asp:Literal ID="LiteralTerminateSelfNo" runat="server" />');
	                            confirmQuestion = decodeURIComponent('<asp:Literal ID="LiteralConfirmSelfTermination" runat="server" />');
	                        }

	                        alertify.set({
	                            labels: {
	                                ok: okLabel,
	                                cancel: cancelLabel
	                            },
	                            buttonFocus: 'cancel'
	                        });

	                        $('#spanTerminatePersonName').text('[...]');

	                        alertify.confirm(confirmQuestion,
	                            $.proxy(function(response) {
	                                if (response) {
	                                    // user clicked the GREEN button, which is "confirm termination"
	                                    onConfirmTermination<%=this.ClientID%>($(this).attr("assignmentId"));
	                                } else {
                                        // if cancel termination, restore icon from loader to action icon again
	                                    $(this).attr("src", "/Images/Icons/iconshock-balloon-no-128x96px.png");
	                                }
	                                $(this).attr("rel", ""); // clear state
	                            }, this));

	                        // Fill in the name and position that are now on-screen in a question

	                        SwarmopsJS.ajaxCall("/Automation/SwarmFunctions.aspx/GetAssignmentData",
                                {assignmentId: $(this).attr("assignmentId") },
	                            function (result) {
	                                if (result.Success) {
	                                    $('#spanTerminatePersonName').text(result.AssignedPersonCanonical);
	                                    $('#spanTerminatePositionName').text(result.PositionLocalized);
	                                } else {
	                                    $('#spanTerminatePersonName,#spanTerminatePositionName').text('[-ERROR-]');
	                                }
	                            });

	                        return; // Do not process here - must wait for confirm dialog to return

	                    }
	                });

	            }
	        });

            $('#<%=this.ClientID%>_buttonAssign').click(function() {
                var personId = <%=this.DropPerson.ClientID%>_selectedPersonId();

                if (personId == 0) {
                    // abort
                    alertify.error("Please select a person."); // LOC
                    return;
                }

                // TODO: Check existing position for person; can't have two positions, at least not within same org

                var jsonData = {
                    personId: personId,
                    positionId: current<%=this.ClientID%>PositionId,
                    durationMonths: <%= this.DropDuration.ClientID%>_val(),
                    geographyId: current<%=this.ClientID%>GeographyId
                };

                console.log(jsonData);

                SwarmopsJS.ajaxCall(
                    "/Automation/SwarmFunctions.aspx/AssignPosition",
                    jsonData,
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

        function onConfirmTermination<%=this.ClientID%> (assignmentId) {
            SwarmopsJS.ajaxCall("/Automation/SwarmFunctions.aspx/TerminatePositionAssignment",
                { assignmentId: assignmentId },
                function (result) {
                    if (result.Success) {
                        <%= this.DialogAdd.ClientID %>_close();
                        $('#<%=this.ClientID%>_tablePositions').treegrid('reload');
                    } else {
                        alertify.error(result.DisplayMessage);
                    }
                });
        }

        var current<%=this.ClientID%>PositionId = '';
        var current<%=this.ClientID%>GeographyId = '';
        var displayed<%=this.ClientID%>GeographyId = '<%=this.GeographyId%>';

    </script>

    <table id="<%=this.ClientID %>_tablePositions" title="" class="easyui-treegrid" style="width:680px<%=this.HeightField%>"  
        url="/Automation/Json-PositionsTree.aspx?Level=<%=this.Level%>&GeographyId=<%=this.GeographyId %>&Cookie=<%=this.Cookie %>"
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
                <th field="actions" width="50" align="center"><asp:Literal ID="LiteralHeaderAction" Text="XYZ" runat="server" /></th>
            </tr>  
        </thead>
    </table> 

    <Swarmops5:ModalDialog id="DialogAdd" runat="server">
        <DialogCode>
            <h2><asp:Label runat="server" ID="LabelModalHeader"/></h2>
            <div class="entryFields">
                <Swarmops5Workaround:ComboPeople ID="DropPerson" runat="server" />
                <Swarmops5:DropDown ID="DropDuration" runat="server" />
                <input type="button" id="<%=this.ClientID %>_buttonAssign" class="buttonAccentColor" value="#Assign#" />
            </div>
            <div class="entryLabels">
                <asp:Label ID="LabelAssignPersonTo" runat="server" /><br/>
                <asp:Label ID="LabelAssignmentDuration" runat="server" />
            </div>
        </DialogCode>
    </Swarmops5:ModalDialog>