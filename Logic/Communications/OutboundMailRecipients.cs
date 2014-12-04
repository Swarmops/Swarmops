using System.Collections.Generic;
using Swarmops.Basic.Types;

namespace Swarmops.Logic.Communications
{
    public class OutboundMailRecipients : List<OutboundMailRecipient>
    {
        public static OutboundMailRecipients FromArray(BasicOutboundMailRecipient[] basicArray, OutboundMail mail)
        {
            OutboundMailRecipients result = new OutboundMailRecipients();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicOutboundMailRecipient basic in basicArray)
            {
                result.Add(OutboundMailRecipient.FromBasic(basic, mail));
            }

            return result;
        }
    }
}