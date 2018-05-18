<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Admin.BitcoinEchoTest" Codebehind="BitcoinEchoTest.aspx.cs" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://bitcoincash.blockexplorer.com/socket.io/socket.io.js"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

            // Initialize socket to wait for transaction

            // TODO: Also poll regularly in case socket fails
        });

        // Socket code to listen to Cash Block Explorer

        eventToListenTo = 'tx';
        room = 'inv';

        var socket = io("https://bitcoincash.blockexplorer.com/");
        socket.on('connect', function() {
            // Join the room.
            socket.emit('subscribe', room);
        });

        socket.on(eventToListenTo, function(data) {
            var utxoCount = data.vout.length;
            for (var index = 0; index < utxoCount; index++)
            {
                for (var outAddress in data.vout[index]) {

                    if (outAddress == addressUsedLegacy || outAddress == addressUsedCash) {

                        var donatedSatoshis = data.vout[index][outAddress];

                        if (donatedSatoshis > minerFeeSatoshis) {

                            $('#paraStatus').text(verifyingText);
                            console.log(donatedSatoshis);
                            sentFunds += Math.floor(donatedSatoshis * conversionRateSatoshisToCents) / 100.0;
                            odoSentCents.innerHTML = sentFunds; // looks weird but $('#id') not used with odo

                            var json = {};
                            json.guid = guid;
                            json.txHash = data.txid;


                            SwarmopsJS.ajaxCall('/Pages/v5/Admin/BitcoinEchoTest.aspx/ProcessTransactionReceived',
                                json,
                                function(data) {
                                    if (data.Success) {
                                        var minerFeeCents = (Math.floor(minerFeeSatoshis * conversionRateSatoshisToCents) / 100.0);
                                        minerFees -= minerFeeCents;
                                        console.log(donatedSatoshis);
                                        console.log("Miner fee is " + minerFeeSatoshis + " satoshis, translating to " + minerFeeCents + " cents");
                                        returnedFunds += Math.floor((donatedSatoshis - minerFeeSatoshis) * conversionRateSatoshisToCents) / 100.0;

                                        odoMinerFeeCents.innerHTML = minerFees;
                                        odoReturnedCents.innerHTML = returnedFunds;

                                        $('#paraStatus').text(data.DisplayMessage);
                                        $('#paraIntro').fadeOut().slideUp();
                                        $('#divQr').fadeOut().slideUp();
                                        completed = true;
                                    }
                                });
                        } else {
                            // Dust collected

                            alertify.dialog(SwarmopsJS.unescape('<%=Localized_DustCollected%>'));
                        }


                    }
                }
            }
        });

        var sentFunds = 0.001; // the 0.1 cents are necessary for an odometer workaround
        var minerFees = -0.001; // negative, or the .1 cents will go wrong
        var returnedFunds = 0.001; // as above

        var addressUsedLegacy = SwarmopsJS.unescape('<%=this.BitcoinCashAddressLegacy %>');
        var addressUsedCash = SwarmopsJS.unescape('<%=this.BitcoinCashAddressCash %>');
        var verifyingText = SwarmopsJS.unescape('<%=this.Localized_Verifying%>');
        var guid = SwarmopsJS.unescape('<%=this.TransactionGuid%>');
        var conversionRateSatoshisToCents = <%= this.ConversionRateSatoshisToCents %>;
        var completed = false;
        var minerFeeSatoshis = <%= EchoFeeSatoshis %>;

        var enable = <%=CurrentOrganization.FinancialAccounts.AssetsBitcoinHot == null? "false": "true"%>;

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
     <div class="box" style="background-image: url(/Images/Other/coins-background-istockphoto.jpg); background-size: 700px">
        <div class="content">
            <div class="odometer-wrapper">
                <div class="elementFloatFar odometer odometer-currency" id="odoSentCents">0.001</div>
                <div class="odometer-label">Received in echo test (<%=CurrentOrganization.Currency.DisplayCode %>)</div>
            </div>
            <div class="odometer-wrapper">
                <div class="elementFloatFar odometer odometer-currency" id="odoMinerFeeCents">-0.001</div>
                <div class="odometer-label">Miner fees paid</div>
            </div>
            <hr/>
            <div class="odometer-wrapper">
                <div class="elementFloatFar odometer odometer-currency" id="odoReturnedCents">0.001</div>
                <div class="odometer-label">Successfully returned</div>
            </div>
        </div>
    </div>
        

    <div class="box"><div class="content">
    <asp:Panel runat="server" ID="PanelEnabled">
        <h2><asp:Label runat="server" ID="BoxTitle" /></h2>
        <p id="paraIntro"><asp:Label runat="server" ID="LabelExplainBitcoinEchoTest" /></p>
        <p style="font-size:200%" id="paraStatus"><asp:Label runat="server" ID="LabelStatus" /></p>
        <div align="center" id="divQr"><asp:Image ID="ImageBitcoinQr" runat="server"/></div>
    </asp:Panel>
    <asp:Panel runat="server" ID="PanelDisabled" Visible="false">
        <h2>Bitcoin hotwallet is not enabled</h2>
        <img alt="Fail symbol" src="/Images/Icons/iconshock-cross-96px.png" align="left" /><br/>
        <p>This organization has not enabled its bitcoin hotwallet in Swarmops. Therefore, echo tests are not available.</p>
        <p>Contact the administrator for <%=this.CurrentOrganization.Name %> and ask them to enable the bitcoin hotwallet.</p>
        <div style="clear:both"></div>
    </asp:Panel>
    </div></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

