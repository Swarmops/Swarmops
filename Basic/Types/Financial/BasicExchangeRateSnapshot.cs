using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    [Serializable]
    public class BasicExchangeRateSnapshot: IHasIdentity
    {
        [Obsolete("Do not call the default constructor directly. It is reserved for serialization.", true)]
        public BasicExchangeRateSnapshot()
        {
            // do not call
        }

        public BasicExchangeRateSnapshot (int identity, DateTime dateTime)
        {
            this.ExchangeRateSnapshotId = identity;
            this.DateTime = dateTime;
        }

        public BasicExchangeRateSnapshot (BasicExchangeRateSnapshot original) :
            this (original.Identity, original.DateTime)
        {
            // empty copy ctor
        }

        public int ExchangeRateSnapshotId { get; private set; }
        public DateTime DateTime { get; private set; }
        public List<BasicExchangeRateSnapshotDatapoint> Datapoints { get; protected set; }

        public int Identity { get { return ExchangeRateSnapshotId; }}
    }

    [Serializable]
    public class BasicExchangeRateSnapshotDatapoint
    {
        public string CurrencyCodeA { get; set; }
        public string CurrencyCodeB { get; set; }
        public double APerB { get; set; }
    }
}
