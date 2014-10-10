using System;

namespace Swarmops.Logic.Security
{
    public class InternalLoginTicket
    {
        public DateTime created = DateTime.MinValue;
        public int validatedUserID = -1;
    }

}
