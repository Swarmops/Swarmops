using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Communications;

namespace Swarmops.Logic.Communications
{
    public class OutboundComm: BasicOutboundComm
    {
        private OutboundComm (BasicOutboundComm basic): base (basic)
        {
            // private ctor
        }
    }
}
