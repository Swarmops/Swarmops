using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Resources;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Money = Swarmops.Logic.Financial.Money;

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

            foreach (HotBitcoinAddress address in addresses)
            {
                if (address.Chain == BitcoinChain.Core)
                {
                    // These shouldn't exist much, so make sure that we have the equivalent Cash address registered

                    try
                    {
                        HotBitcoinAddress.FromAddress(BitcoinChain.Cash, address.Address);
                    }
                    catch (ArgumentException)
                    {
                        // We didn't have it, so create it
                        BitcoinUtility.TestUnspents(BitcoinChain.Cash, address.Address);
                    }
                }

            }

            Response.ContentType = "application/json";
            Response.Output.WriteLine(FormatJson(addresses));
            Response.End();
        }



        private string FormatJson(HotBitcoinAddresses addresses)
        {
            StringBuilder result = new StringBuilder(16384);

            Dictionary<BitcoinChain, double> conversionRateLookup = new Dictionary<BitcoinChain, double>();
            Dictionary<BitcoinChain, Int64> satoshisTotalLookup = new Dictionary<BitcoinChain, long>();
            
            long fiatCentsPerCoreCoin = new Money (BitcoinUtility.SatoshisPerBitcoin, Currency.BitcoinCore).ToCurrency(_authenticationData.CurrentOrganization.Currency).Cents;
            long fiatCentsPerCashCoin = new Money(BitcoinUtility.SatoshisPerBitcoin, Currency.BitcoinCash).ToCurrency(_authenticationData.CurrentOrganization.Currency).Cents;

            conversionRateLookup[BitcoinChain.Cash] = fiatCentsPerCashCoin/1.0/BitcoinUtility.SatoshisPerBitcoin;   // the "/1.0" converts to double implicitly
            conversionRateLookup[BitcoinChain.Core] = fiatCentsPerCoreCoin/1.0/BitcoinUtility.SatoshisPerBitcoin;

            result.Append("{\"rows\":[");

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
                        (unspent.AmountSatoshis/100.0*conversionRateLookup[unspent.Address.Chain]).ToString ("N2")
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
                        address.Chain.ToString() + " " + address.DerivationPath,
                        address.Address,
                        JsonExpandingString (address.Identity, satoshisUnspentAddress),
                        JsonExpandingString (address.Identity, (Int64) (satoshisUnspentAddress / 100.0 * conversionRateLookup[address.Chain]))
                        );
                    result.Append ("\"state\":\"closed\",\"children\":[" + childResult.ToString() + "]");
                    result.Append ("},");
                    addressesWithFunds++;

                    if (!satoshisTotalLookup.ContainsKey(address.Chain))
                    {
                        satoshisTotalLookup[address.Chain] = 0;
                    }

                    satoshisTotalLookup[address.Chain] += satoshisUnspentAddress;

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

            result.Append("]");

            if (satoshisTotalLookup.Count > 0)
            {
                // We should also have a footer, because we need a total

                result.Append(",\"footer\":[");

                bool previousFooterRow = false;

                foreach (BitcoinChain chain in satoshisTotalLookup.Keys)
                {
                    if (previousFooterRow)
                    {
                        result.Append(",");
                    }

                    result.Append("{");

                    result.AppendFormat(
                        "\"derivePath\":\"" + Resources.Global.Global_Total.ToUpperInvariant() + " " +
                        (string) chain.ToString().ToUpperInvariant() + "\",\"balanceMicrocoins\":\"{0}\",\"balanceFiat\":\"{1}\"",
                        (satoshisTotalLookup[chain]/100.0).ToString("N2"), (satoshisTotalLookup[chain]/100.0*conversionRateLookup[chain]).ToString("N2"));

                    result.Append("}"); // on separate line to suppress warning*/

                    previousFooterRow = true;
                }

                result.Append("]");
            }

            result.Append("}");

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