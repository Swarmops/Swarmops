using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Communications;
using Swarmops.Database;

namespace Swarmops.Logic.Communications
{
    public class OutboundMails : List<OutboundMail>
    {
        public static OutboundMails FromArray (BasicOutboundMail[] basicArray)
        {
            OutboundMails result = new OutboundMails();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicOutboundMail basic in basicArray)
            {
                result.Add (OutboundMail.FromBasic (basic));
            }

            return result;
        }


        public static OutboundMails GetTopUnprocessed (int count)
        {
            BasicOutboundMail[] basicMails = SwarmDb.GetDatabaseForReading().GetTopUnprocessedOutboundMail (count);

            return FromArray (basicMails);
        }
    }
}