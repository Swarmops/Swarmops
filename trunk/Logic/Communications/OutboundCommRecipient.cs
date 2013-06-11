using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Communications;

namespace Swarmops.Logic.Communications
{
    public class OutboundCommRecipient: BasicOutboundCommRecipient
    {
        private OutboundCommRecipient(BasicOutboundCommRecipient basic): base (basic)
        {
            // private ctor
        }
    }
}
