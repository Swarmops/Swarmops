using System;
using Swarmops.Common;

namespace Swarmops.Logic.Security
{
    public class InternalLoginTicket
    {
        public DateTime created = Constants.DateTimeLow;
        public int validatedUserID = -1;
    }
}