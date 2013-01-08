using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Communications
{
    public class OutboundMails : List<OutboundMail>
    {
        public static OutboundMails FromArray (BasicOutboundMail[] basicArray)
        {
            var result = new OutboundMails();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicOutboundMail basic in basicArray)
            {
                result.Add (OutboundMail.FromBasic (basic));
            }

            return result;
        }


        public static OutboundMails GetTopUnprocessed (int count)
        {
            BasicOutboundMail[] basicMails = PirateDb.GetDatabaseForReading().GetTopUnprocessedOutboundMail (count);

            return FromArray (basicMails);
        }
    }
}