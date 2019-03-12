<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.Comms.SendMassMessage" CodeFile="SendMassMessage.aspx.cs" %>
<%@ Register src="~/Controls/v5/UI/ExternalScripts.ascx" tagname="ExternalScripts" tagprefix="Swarmops5" %>
<%@ Register src="~/Controls/v5/Base/ComboGeographies.ascx" tagname="ComboGeographies" tagprefix="Swarmops5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    
    <script type="text/javascript">

        $(document).ready(function () {
            // Document.Ready() goes here

            $('#<%=this.DropRecipientClasses.ClientID%>').change(function() {
                onGeographyChange(selectedGeographyId); // does the same thing anyway
            });

            updateRecipientCount(selectedRecipientClass, selectedGeographyId);

            $('#<%=this.TextSubject.ClientID%>').focus();
});

        var selectedRecipientClass = 1;
        var selectedGeographyId = 1;  // TODO: SET ROOT GEOGRAPHY BY AUTHORITY/ACCESS

        function onGeographyChange(newGeographyId) {
            var recipientClass = <%=this.DropRecipientClasses.ClientID %>_val();

            updateRecipientCount(recipientClass, newGeographyId);
            selectedGeographyId = newGeographyId;
        }

        function onClickSend() {
            onExecute(true);
            return false;
        }

        function onClickTest() {
            onExecute(false);
            return false;
        }

        function onExecute(live) {
            var body = $('#<%=this.TextMessage.ClientID%>').val().trim();
            var subject = $('#<%=this.TextSubject.ClientID%>').val().trim();

            if (body.length < 2) // interpreted as empty
            {
                alertify.alert(SwarmopsJS.unescape('<%=this.Localized_Error_EmptyBody%>'));
                return false;
            }

            if (subject.length < 2) // interpreted as empty
            {
                // TODO: ADD TRANSMISSION MODUS / ONLY TRIGGER ON MODUS WITH SUBJECTS
                alertify.alert(SwarmopsJS.unescape('<%=this.Localized_Error_EmptySubject%>'));
                return false;
            }

            if (<%=this.RunningOnSandbox%> && !live && testMessageSandboxAddress == '') {
                // Prompt for an address to send to
                // This should preferably be in a mod on just Sandbox and not in code everywhere

                alertify.prompt(SwarmopsJS.unescape('<%=this.Localized_Sandbox_EnterAddress%>'),
                    function(evt, value) {
                        testMessageSandboxAddress = value; // may need sanitation
                        onClickTest(); // rerun with value set
                    });

                return null; // do not process at this time, wait for async response
            }

            var jsonData = {
                recipientTypeId: selectedRecipientClass,
                geographyId: selectedGeographyId,
                mode: "Mail",
                subject: subject,
                body: body,
                dummyMail: testMessageSandboxAddress,
                live: live
            };

            SwarmopsJS.ajaxCall
            ("/Pages/v5/Comms/SendMassMessage.aspx/ExecuteSend",
                jsonData,
                function(result) {
                    if (result.Success) {
                        if (live) { // race condition but won't probably ever trigger
                            alertify.alert(SwarmopsJS.unescape('<%= this.Localized_SendMessageResult %>'));
                        } else {
                            alertify.alert(SwarmopsJS.unescape('<%= this.Localized_TestMessageResult %>'));
                        }
                    }
                });

            return null;
        }

        function onRecipientChange(newRecipientClass, oldClassIgnored) {
            selectedRecipientClass = newRecipientClass;
            updateRecipientCount(newRecipientClass, selectedGeographyId);
        }

        function updateRecipientCount(recipientClass, geography) {
            var jsonData = {
                recipientTypeId: recipientClass,
                geographyId: geography
            };

            SwarmopsJS.ajaxCall
            ("/Pages/v5/Comms/SendMassMessage.aspx/GetRecipientCount",
                jsonData,
                function(result) {
                    $('#spanRecipientCount').text(result);
                });
            
        }

        var testMessageSandboxAddress = '';

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="data-entry-fields">
        <Swarmops5:DropDown runat="server" ID="DropRecipientClasses" OnClientChange="onRecipientChange"/>
        <Swarmops5:ComboGeographies ID="ComboGeographies" runat="server" OnClientSelect="onGeographyChange" />
        <asp:TextBox ID="TextSubject" runat="server"/>
    </div>
    <div class="data-entry-labels">
        <asp:Label ID="LabelRecipientType" runat="server" /><br/>
        <asp:Label ID="LabelGeography" runat="server" /><br/>
        <asp:Label ID="LabelSubject" runat="server"/>
    </div>
    <h2 style="padding-top:15px"><asp:Label ID="LabelHeaderMessage" runat="server" /> (<span id="spanRecipientCount">...</span>)</h2>
    <asp:TextBox runat="server" TextMode="MultiLine" Rows="10" ID="TextMessage" />
    <asp:Button runat="server" CssClass="button-accent-color" Text="Foo" OnClientClick="onClickSend(); return false;" ID="ButtonSend" /><asp:Button runat="server" CssClass="button-accent-color" OnClientClick="onClickTest(); return false;" Text="Bar" ID="ButtonTest" />
    <div style="clear:both"></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

