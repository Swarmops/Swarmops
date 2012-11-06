using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
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
            return FromBasic(PirateDb.GetDatabaseForReading().GetParleyOption(parleyOptionId));
        }

        public static ParleyOption Create (Parley parley, string description, Int64 amountCents)
        {
            return FromIdentity(PirateDb.GetDatabaseForWriting().CreateParleyOption(parley.Identity, description, amountCents));
        }

        public decimal AmountDecimal
        {
            get { return AmountCents/100.0m; }
        }
    }
}
