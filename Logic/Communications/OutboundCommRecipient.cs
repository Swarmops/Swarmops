using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Communications;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class OutboundCommRecipient: BasicOutboundCommRecipient
    {
        private OutboundCommRecipient(BasicOutboundCommRecipient basic): base (basic)
        {
            // private ctor
        }

        public static OutboundCommRecipient FromBasic (BasicOutboundCommRecipient basic)
        {
            return new OutboundCommRecipient(basic);
        }

        public Person Person
        {
            get { return Person.FromIdentity(base.PersonId); }
        }

        // TODO: CloseSuccess, CloseFail
    }
}
