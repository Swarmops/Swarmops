using System.Collections.Generic;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Communications
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