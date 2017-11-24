<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.Donate" Codebehind="Donate.aspx.cs" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script src="https://bitcoincash.blockexplorer.com/socket.io/socket.io.js"></script>

    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

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

                    if (outAddress == pageDonationAddress) {

                        $('#paraStatus').text(verifyingText);
                        var donatedSatoshis = data.vout[index][outAddress];
                        donatedFunds += Math.floor(donatedSatoshis * conversionRateSatoshisToCents) / 100.0;
                        odoDonatedCents.innerHTML = donatedFunds; // looks weird but $('#id') not used with odo

                        var json = {};
                        json.guid = guid;
                        json.txHash = data.txid;

                        SwarmopsJS.ajaxCall('/Pages/v5/Financial/Donate.aspx/ProcessTransactionReceived',
                            json,
                            function (data) {
                                if (data.Success) {
                                    $('#paraStatus').text(data.DisplayMessage);
                                    $('#paraIntro').fadeOut().slideUp();
                                    $('#divQr').fadeOut().slideUp();
                                    completed = true;
                                }
                            });

                    }
                }
            }
        });


        function pageBitcoinReceived(address, hash, satoshis, cents, currencyCode) {

            // This function is detected called by the Master Page; it is not
            // called from this page or anything visible here

            // console.log("address: " + address);
            // console.log("hash: " + hash);
            // console.log("satoshis: " + satoshis);
            // console.log("cents: " + cents);
            // console.log("currencyCode: " + currencyCode);

            if (address == pageDonationAddress) {
                // We have received a donation at this address

                donatedFunds += (cents / 100.0);
                odoDonatedCents.innerHTML = donatedFunds; // looks weird but $('#id') not used with odo

                var json = {};
                json.guid = guid;
                json.txHash = hash;

                SwarmopsJS.ajaxCall('/Pages/v5/Financial/Donate.aspx/ProcessTransactionReceived',
                    json,
                    function (data) {
                        if (data.Success) {
                            $('#paraStatus').text(data.DisplayMessage);
                            $('#paraIntro').fadeOut().slideUp();
                            $('#divQr').fadeOut().slideUp();
                            completed = true;
                        }
                    });

                return true;
            }

            return false; // not handled
        }

        var donatedFunds = 0.001; // the 0.1 cents are necessary for an odometer workaround

        var pageDonationAddress = SwarmopsJS.unescape('<%=this.BitcoinCashAddressUsed %>');
        var verifyingText = SwarmopsJS.unescape('<%=this.Localized_Verifying%>');
        var guid = SwarmopsJS.unescape('<%=this.TransactionGuid%>');
        var conversionRateSatoshisToCents = <%= this.ConversionRateSatoshisToCents %>;
        var completed = false;

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
     <div class="box" style="background-image: url(/Images/Other/coins-background-istockphoto.jpg); background-size: 700px">
        <div class="content">
            <div class="odometer-wrapper">
                <div class="elementFloatFar odometer odometer-currency" id="odoDonatedCents">0.001</div>
                <div class="odometer-label"><asp:Label runat="server" ID="LabelReceivedFunds"/></div>
            </div>
        </div>
    </div>
    
    <div class="box"><div class="content">
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
    </div></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderSide" Runat="Server">
</asp:Content>

