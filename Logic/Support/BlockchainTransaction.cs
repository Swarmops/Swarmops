using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Swarmops.Logic.Support
{
    public class BlockchainTransaction
    {
        static public BlockchainTransaction FromBlockchainInfoJson (JObject json)
        {
            BlockchainTransaction thisTx = new BlockchainTransaction();

            System.DateTime transactionDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            thisTx.TransactionDateTimeUtc = transactionDateTimeUtc.AddSeconds(Int64.Parse((string)json["time"]));
            thisTx.TransactionHash = (string)json["hash"];

            Int64 satoshiCount = 0;
            thisTx.Inputs = new List<BlockchainTransactionRow>();
            thisTx.Outputs = new List<BlockchainTransactionRow>();
            foreach (JObject inputRow in json["inputs"])
            {
                BlockchainTransactionRow newRow =
                    BlockchainTransactionRow.FromBlockchainInfoJson((JObject)inputRow["prev_out"]);
                thisTx.Inputs.Add(newRow);
                satoshiCount += newRow.ValueSatoshis;
            }
            foreach (JObject outputRow in json["out"])
            {
                BlockchainTransactionRow newRow =
                    BlockchainTransactionRow.FromBlockchainInfoJson(outputRow);
                thisTx.Outputs.Add(newRow);
                satoshiCount -= newRow.ValueSatoshis;
            }

            thisTx.FeeSatoshis = satoshiCount; // inputs minus outputs equals fee

            return thisTx;
        }


        static public BlockchainTransaction FromInsightInfoJson(JObject json)
        {
            BlockchainTransaction thisTx = new BlockchainTransaction();

            System.DateTime transactionDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            thisTx.TransactionDateTimeUtc = transactionDateTimeUtc.AddSeconds(Int64.Parse((string)json["time"]));

            thisTx.TransactionHash = (string)json["txid"];

            Int64 satoshiCount = 0;
            thisTx.Inputs = new List<BlockchainTransactionRow>();
            thisTx.Outputs = new List<BlockchainTransactionRow>();
            foreach (JObject inputRow in json["vin"])
            {
                BlockchainTransactionRow newRow =
                    BlockchainTransactionRow.FromInsightInfoJson(inputRow, true);
                thisTx.Inputs.Add(newRow);
                satoshiCount += newRow.ValueSatoshis;
            }
            foreach (JObject outputRow in json["vout"])
            {
                BlockchainTransactionRow newRow =
                    BlockchainTransactionRow.FromInsightInfoJson(outputRow);
                thisTx.Outputs.Add(newRow);
                satoshiCount -= newRow.ValueSatoshis;
            }

            thisTx.FeeSatoshis = satoshiCount; // inputs minus outputs equals fee

            return thisTx;
        }
    


        public DateTime TransactionDateTimeUtc { get; private set; }
        public Int64 FeeSatoshis { get; private set; }
        public string TransactionHash { get; private set; }

        public List<BlockchainTransactionRow> Inputs { get; private set; }
        public List<BlockchainTransactionRow> Outputs { get; private set; }

    }

    public class BlockchainTransactionRow
    {
        static public BlockchainTransactionRow FromBlockchainInfoJson (JObject json)
        {
            BlockchainTransactionRow newRow = new BlockchainTransactionRow();
            newRow.Address = (string) json["addr"];
            newRow.ValueSatoshis = Int64.Parse ((string) json["value"]);
            newRow.Index = Int32.Parse ((string) json["n"]);
            newRow.Spent = Boolean.Parse ((string) json["spent"]);

            return newRow;
        }

        static public BlockchainTransactionRow FromInsightInfoJson(JObject json, bool inRow = false)
        {
            BlockchainTransactionRow newRow = new BlockchainTransactionRow();
            newRow.Address = (string)json["addr"];
            newRow.ValueSatoshis = Int64.Parse((string)json["valueSat"]);
            newRow.Index = Int32.Parse((string)json["n"]);
            newRow.Spent = inRow && (bool)(json["spentTxId"] == null? false: true);

            return newRow;
        }

        public string Address { get; private set; }
        public bool Spent { get; private set; }
        public Int64 ValueSatoshis { get; private set; }
        public int Index { get; private set; }
    }
}
