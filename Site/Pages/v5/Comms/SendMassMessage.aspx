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

            updateRecipientCount(selectedReceipientClass, selectedGeographyId);

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
            alertify.alert(SwarmopsJS.unescape('<%= this.Localized_SendMessageResult %>'));
            return false;
        }

        function onClickTest() {

            var jsonData = {
                recipientTypeId: selectedRecipientClass,
                geographyId: selectedGeographyId,
                mode: "Mail",
                subject: "Test Subject",
                body: "SHAKAZAM *IT WORKS*\r\n\r\nMaybe?",
                dummyMail: "rick@falkvinge.net",
                live: false
            };

            SwarmopsJS.ajaxCall
            ("/Pages/v5/Comms/SendMassMessage.aspx/ExecuteSend",
                jsonData,
                function (result) {
                    $('#spanRecipientCount').text(result);
                });

            alertify.alert(SwarmopsJS.unescape('<%= this.Localized_TestMessageResult %>'));
            return false;
        }

        function onRecipientChange(newRecipientClass) {
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

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <div class="entryFields" style="padding-top:4px">
        <Swarmops5:DropDown runat="server" ID="DropRecipientClasses" OnClientChange="onRecipientChange"/>&thinsp;<br/>
        <Swarmops5:ComboGeographies ID="ComboGeographies" runat="server" OnClientSelect="onGeographyChange" />&thinsp;<br/>
        <asp:TextBox ID="TextSubject" runat="server"/>&thinsp;
    </div>
    <div class="entryLabels" style="padding-top:10px">
        <asp:Label ID="LabelRecipientType" runat="server" /><br/>
        <asp:Label ID="LabelGeography" runat="server" /><br/>
        <asp:Label ID="LabelSubject" runat="server"/>
    </div>
    <h2 style="padding-top:15px"><asp:Label ID="LabelHeaderMessage" runat="server" /> (<span id="spanRecipientCount">...</span>)</h2>
    <asp:TextBox runat="server" TextMode="MultiLine" Rows="10" ID="TextMessage" />
    <asp:Button runat="server" CssClass="buttonAccentColor" Text="Foo" OnClientClick="onClickSend(); return false;" ID="ButtonSend" /><asp:Button runat="server" CssClass="buttonAccentColor" OnClientClick="onClickTest(); return false;" Text="Bar" ID="ButtonTest" />
    <div style="clear:both"></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

