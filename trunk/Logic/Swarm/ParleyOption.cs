using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Swarm
{
    public class ParleyOption: BasicParleyOption
    {
        private ParleyOption (BasicParleyOption basic): base (basic)
        {
            // empty pvt ctor
        }

        public static ParleyOption FromBasic (BasicParleyOption basic)
        {
            return new ParleyOption(basic);
        }

        public static ParleyOption FromIdentity (int parleyOptionId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetParleyOption(parleyOptionId));
        }

        public static ParleyOption Create (Parley parley, string description, Int64 amountCents)
        {
            return FromIdentity(SwarmDb.GetDatabaseForWriting().CreateParleyOption(parley.Identity, description, amountCents));
        }

        public decimal AmountDecimal
        {
            get { return AmountCents/100.0m; }
        }
    }
}
