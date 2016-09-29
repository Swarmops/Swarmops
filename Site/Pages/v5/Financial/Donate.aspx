<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.Donate" Codebehind="Donate.aspx.cs" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

            // Initialize socket to wait for transaction

            if (enable) {
                initializeSocket();
            }

            // TODO: Also poll regularly in case socket fails
        });

        function initializeSocket() {
            if (socket != null) {
                socket.close();
                socket = null;
            }

            socket = new WebSocket("wss://ws.blockchain.info/inv?api_code=<%= SystemSettings.BlockchainSwarmopsApiKey %>");

            socket.onopen = function(data) {
                console.log("socket opened");
                socket.send('{"op":"addr_sub","addr":"' + addressUsed + '"}');
            };
            socket.onclose = function (data) {
                if (!completed) {
                    console.log("socket lost");
                    initializeSocket();
                }
            };
            socket.onerror = function(data) { console.log("socket error"); };
            socket.onmessage = function(data) {

                console.log(data.data);

                var message = $.parseJSON(data.data);

                if (message.op == "utx") {
                    // transaction received at address
                    // call page to check if funds received server side

                    alertify.log(decodeURIComponent('<asp:Literal ID="LiteralTxDetected" runat="server" />'));
                    checkTransactionReceived(message.x.hash);
                }
            };
        }

        function checkTransactionReceived(hash) {

            if (hash == null) {
                hash = "";
            }

            SwarmopsJS.ajaxCall('/Pages/v5/Financial/Donate.aspx/CheckTransactionReceived',
                { guid: guid, txHash: hash },
                function (data) {
                    if (data.Success) {
                        $('#paraStatus').text(data.DisplayMessage);
                        $('#paraIntro').fadeOut().slideUp();
                        $('#divQr').fadeOut().slideUp();
                        completed = true;
                        socket.close();
                        socket = null;
                    }
                });
        }

        var addressUsed = '<asp:Literal runat="server" ID="LiteralBitcoinAddress" />';
        var guid = '<asp:Literal runat="server" ID="LiteralGuid" />';
        var socket = null;
        var completed = false;
        var enable = <asp:Literal ID="LiteralEnable" runat="server">true</asp:Literal>;

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <asp:Panel runat="server" ID="PanelEnabled">
        <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
        <p id="paraIntro"><asp:Label runat="server" ID="LabelExplainBitcoinDonation" /></p>
        <p style="font-size:200%" id="paraStatus"><asp:Label runat="server" ID="LabelStatus" /></p>
        <div align="center" id="divQr"><asp:Image ID="ImageBitcoinQr" runat="server"/></div>
    </asp:Panel>
    <asp:Panel runat="server" ID="PanelDisabled" Visible="false">
        <h2>Bitcoin hotwallet is not enabled</h2>
        <img alt="Fail symbol" src="/Images/Icons/iconshock-cross-96px.png" align="left" /><br/>
        <p>This organization has not enabled its bitcoin hotwallet in Swarmops. Therefore, direct donations are not available.</p>
        <p>Contact the administrator for <%=this.CurrentOrganization.Name %> and ask them to enable the bitcoin hotwallet.</p>
        <div style="clear:both"></div>
    </asp:Panel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

