<%@ Page Title="" Language="C#" MasterPageFile="~/Master-v5.master" AutoEventWireup="true" Inherits="Swarmops.Frontend.Pages.v5.Financial.Donate" Codebehind="Donate.aspx.cs" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderHead" Runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function() {

        });

        function pageBitcoinReceived(address, hash, satoshis, cents, currencyCode) {

            console.log("address: " + address);
            console.log("hash: " + hash);
            console.log("satoshis: " + satoshis);
            console.log("cents: " + cents);
            console.log("currencyCode: " + currencyCode);

            if (address == addressUsed) {
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

        var addressUsed = '<asp:Literal runat="server" ID="LiteralBitcoinAddress" />';
        var guid = '<asp:Literal runat="server" ID="LiteralGuid" />';
        var completed = false;

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" Runat="Server">
     <div class="box" style="background-image: url(/Images/Other/coins-background-istockphoto.jpg); background-size: 700px">
        <div class="content">
            <div class="odometer-wrapper">
                <div class="elementFloatFar odometer odometer-currency" id="odoDonatedCents">0.001</div><div class="odometer-label">Received funds (CUR)</div>
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

