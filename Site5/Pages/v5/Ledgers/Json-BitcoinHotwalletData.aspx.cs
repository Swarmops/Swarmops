using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Resources;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_BitcoinHotwalletData : DataV5Base
    {
        private AuthenticationData _authenticationData;

        protected void Page_Load(object sender, EventArgs e)
        {
            PageAccessRequired = new Access(CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read);
            this._authenticationData = GetAuthenticationDataAndCulture();

            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization (_authenticationData.CurrentOrganization);

            Response.ContentType = "application/json";
            Response.Output.WriteLine(FormatJson(addresses));
            Response.End();
        }



        private string FormatJson(HotBitcoinAddresses addresses)
        {
            StringBuilder result = new StringBuilder(16384);

            double conversionRate = 1.0;
            if (!this._authenticationData.CurrentOrganization.Currency.IsBitcoin)
            {
                long fiatCentsPerCoin = new Money(100000000, Currency.Bitcoin).ToCurrency (_authenticationData.CurrentOrganization.Currency).Cents;
                conversionRate = fiatCentsPerCoin/100000000.0; // on satoshi level
            }

            result.Append("{\"rows\":[");

            Int64 satoshisTotal = 0;
            int addressesWithFunds = 0;

            foreach (HotBitcoinAddress address in addresses)
            {
                HotBitcoinAddressUnspents unspents = HotBitcoinAddressUnspents.ForAddress (address);
                Int64 satoshisUnspentAddress = 0;

                StringBuilder childResult = new StringBuilder (16384);
                foreach (HotBitcoinAddressUnspent unspent in unspents)
                {
                    childResult.Append ("{");
                    childResult.AppendFormat (
                        "\"id\":\"UTXO{0}\"," +
                        "\"derivePath\":\"{1}\"," +
                        "\"address\":\"{2}\"," +
                        "\"balanceMicrocoins\":\"{3}\"," +
                        "\"balanceFiat\":\"{4}\"",
                        unspent.Identity,
                        Resources.Pages.Ledgers.BitcoinHotWallet_UnspentTransaction,
                        unspent.TransactionHash,
                        (unspent.AmountSatoshis/100.0).ToString ("N2"),
                        (unspent.AmountSatoshis/100.0*conversionRate).ToString ("N2")
                        );
                    satoshisUnspentAddress += unspent.AmountSatoshis;
                    childResult.Append ("},");
                }
                if (unspents.Count > 0)
                {
                    childResult.Remove (childResult.Length - 1, 1); // remove last comma
                }

                if (satoshisUnspentAddress > 0)
                {
                    result.Append ("{");
                    result.AppendFormat (
                        "\"id\":\"{0}\"," +
                        "\"derivePath\":\"{1}\"," +
                        "\"address\":\"{2}\"," +
                        "\"balanceMicrocoins\":\"{3}\"," +
                        "\"balanceFiat\":\"{4}\",",
                        address.Identity,
                        address.DerivationPath,
                        address.Address,
                        JsonExpandingString (address.Identity, satoshisUnspentAddress),
                        JsonExpandingString (address.Identity, (Int64) (satoshisUnspentAddress*conversionRate))
                        );
                    result.Append ("\"state\":\"closed\",\"children\":[" + childResult.ToString() + "]");
                    result.Append ("},");
                    satoshisTotal += satoshisUnspentAddress;
                    addressesWithFunds++;
                }
            }

            if (addressesWithFunds > 0)
            {
                result.Remove(result.Length - 1, 1); // remove last comma
            }
            else // no funds at all in hotwallet
            {
                result.Append("{");
                result.AppendFormat(
                    "\"id\":\"0\"," +
                    "\"derivePath\":\"0\"," +
                    "\"address\":\"{0}\"," +
                    "\"balanceMicrocoins\":\"{1}\"," +
                    "\"balanceFiat\":\"{2}\"",
                    Resources.Pages.Ledgers.BitcoinHotWallet_Empty,
                    0.0.ToString("N2"),
                    0.0.ToString("N2")
                    );
                result.Append("}");
            }

            result.Append("],\"footer\":[");

            result.Append("{");

            result.AppendFormat("\"derivePath\":\"" + Resources.Global.Global_Total.ToUpperInvariant() +  "\",\"balanceMicrocoins\":\"{0}\",\"balanceFiat\":\"{1}\"",
                (satoshisTotal / 100.0).ToString("N2"), (satoshisTotal / 100.0 * conversionRate).ToString("N2"));

            result.Append("}]}"); // on separate line to suppress warning

            return result.ToString();
        }


        private string JsonExpandingString(int addressId, Int64 currencyValue)
        {
            return string.Format(CultureInfo.CurrentCulture,
                "<span class=\\\"bitcoinhotwalletdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N2}</span><span class=\\\"bitcoinhotwalletdata-expanded-{0}\\\" style=\\\"display:none\\\">&nbsp;</span>",
                addressId, currencyValue / 100.00);
        }


    }
}