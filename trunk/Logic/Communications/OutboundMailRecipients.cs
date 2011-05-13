using System.Collections.Generic;

using Activizr.Basic.Types;

namespace Activizr.Logic.Communications
{
    public class OutboundMailRecipients : List<OutboundMailRecipient>
    {
        public static OutboundMailRecipients FromArray (BasicOutboundMailRecipient[] basicArray, OutboundMail mail)
        {
            var result = new OutboundMailRecipients();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicOutboundMailRecipient basic in basicArray)
            {
                result.Add (OutboundMailRecipient.FromBasic (basic, mail));
            }

            return result;
        }
    }
}