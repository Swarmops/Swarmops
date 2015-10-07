<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" CodeFile="Donate.aspx.cs" Inherits="Swarmops.Frontend.Pages.v5.Financial.Donate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

            // Initialize socket to wait for transaction

            initializeSocket();
            
            // TODO: Also poll regularly in case socket fails

            checkTransactionReceived(); // test ajax call

        });

        function initializeSocket() {
            if (socket != null) {
                socket.close();
                socket = null;
            }

            socket = new WebSocket("wss://ws.blockchain.info/inv");

            socket.onopen = function(data) {
                console.log("socket opened");
                socket.send('{"op":"addr_sub","addr":"' + addressUsed + '"}');
            };
            socket.onclose = function(data) {
                console.log("socket lost");
                initializeSocket();
            };
            socket.onerror = function(data) { console.log("socket error"); };
            socket.onmessage = function(data) {

                console.log(data.data);

                var message = $.parseJSON(data.data);

                if (message.op == "utx") {
                    // transaction received at address
                    // call page to check if funds received server side

                    alertify.log("Tx received on socket, checking...");
                    checkTransactionReceived(message.x.hash);
                }
            };
        }

        function checkTransactionReceived (hash) {
            SwarmopsJS.ajaxCall('/Pages/v5/Financial/Donate.aspx/CheckTransactionReceived',
                { guid: guid, txHash: hash },
                function (data) {
                    if (data.Success) {
                        $('#paraStatus').text(data.DisplayMessage);
                        $('#divQr').fadeOut().slideUp();
                        socket.close();
                        socket = null;
                    }
                });
        }

        var addressUsed = '<asp:Literal runat="server" ID="LiteralBitcoinAddress" />';
        var guid = '<asp:Literal runat="server" ID="LiteralGuid" />';
        var socket = null;

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
    <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
    <p><asp:Label runat="server" ID="LabelExplainBitcoinDonation" /></p>
    <p style="font-size:200%" id="paraStatus"><asp:Label runat="server" ID="LabelStatus" /></p>
    <div align="center" id="divQr"><asp:Image ID="ImageBitcoinQr" runat="server"/></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

