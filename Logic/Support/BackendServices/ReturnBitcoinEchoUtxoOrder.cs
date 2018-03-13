using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Satoshis = NBitcoin.Money;

namespace Swarmops.Logic.Support.BackendServices
{
    [Serializable]
    public class ReturnBitcoinEchoUtxoOrder: BackendServiceOrderBase<ReturnBitcoinEchoUtxoOrder>
    {
        [Obsolete("Never call the parameterless ctor from code; intended for use by serialization only.", true)]
        public ReturnBitcoinEchoUtxoOrder()
        {
            // Never call -- intended for use by serialization only
        }

        public ReturnBitcoinEchoUtxoOrder(BasicHotBitcoinAddressUnspent utxo)
        {
            this.UtxoIdentity = utxo == null? 0: utxo.Identity;
        }

        public override void Run()
        {
            HotBitcoinAddressUnspent utxoToReturn = HotBitcoinAddressUnspent.FromIdentity(UtxoIdentity);
            HotBitcoinAddress utxoAddress = utxoToReturn.Address;
            BitcoinSecret secretKey = utxoAddress.PrivateKey;

            // TODO: Verify that the utxoAddress is an EchoTest address, i.e. has second path component == BitcoinUtility.BitcoinEchoTestIndex

            string returnAddress = BitcoinUtility.GetInputAddressesForTransaction(BitcoinChain.Cash, utxoToReturn.TransactionHash)[0]; // assumes at least one input address -- not coinbase

            // Return the money

            BitcoinTransactionInputs inputs = utxoToReturn.AsInputs;
            Int64 satoshisToReturn = utxoToReturn.AmountSatoshis;

            Coin[] coins = inputs.Coins;
            ICoin[] iCoins = coins;
            ISecret[] privateKeys = utxoToReturn.AsInputs.PrivateKeys;

            TransactionBuilder txBuilder = new TransactionBuilder();
            txBuilder = txBuilder.SendFees(new Satoshis(BitcoinUtility.EchoFeeSatoshis));
            txBuilder = txBuilder.AddCoins(iCoins);
            txBuilder = txBuilder.AddKeys(privateKeys);

            if (returnAddress.StartsWith("1"))
            {
                txBuilder = txBuilder.Send(new BitcoinPubKeyAddress(returnAddress),
                    new Satoshis(utxoToReturn.AmountSatoshis - BitcoinUtility.EchoFeeSatoshis));
            }
            else if (returnAddress.StartsWith("3"))
            {
                txBuilder = txBuilder.Send(new BitcoinScriptAddress(returnAddress, Network.Main), 
                    new Satoshis(utxoToReturn.AmountSatoshis - BitcoinUtility.EchoFeeSatoshis));
            }
            else
            {
                throw new ArgumentException("Unrecognized address format");
            }

            Transaction tx = txBuilder.BuildTransaction(true, SigHash.ForkId | SigHash.All);

            BitcoinUtility.BroadcastTransaction(tx, BitcoinChain.Cash);
            utxoToReturn.Delete();
            utxoAddress.UpdateTotal();

            // Update the ledger

            string tx2Description = "Bitcoin echo test repayment";
            FinancialTransaction ledgerTx2 = FinancialTransaction.Create(this.Organization,
                DateTime.UtcNow, tx2Description);

            if (this.Organization.Currency.IsBitcoinCash)
            {
                ledgerTx2.AddRow(this.Organization.FinancialAccounts.DebtsOther, satoshisToReturn, this.Person);
                ledgerTx2.AddRow(this.Organization.FinancialAccounts.AssetsBitcoinHot, -satoshisToReturn, this.Person);
            }
            else
            {
                Int64 centsPresentation =
                    new Swarmops.Logic.Financial.Money(satoshisToReturn, Currency.BitcoinCash).ToCurrency(this.Organization.Currency).Cents;
                ledgerTx2.AddRow(this.Organization.FinancialAccounts.DebtsOther, centsPresentation, this.Person);
                ledgerTx2.AddRow(this.Organization.FinancialAccounts.AssetsBitcoinHot, -centsPresentation, this.Person);
            }
            ledgerTx2.BlockchainHash = tx.GetHash().ToString();
        }

        public int UtxoIdentity { get; set; }
    }
}
