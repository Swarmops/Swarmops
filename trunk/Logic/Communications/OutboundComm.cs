using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Communications;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class OutboundComm: BasicOutboundComm
    {
        private OutboundComm (BasicOutboundComm basic): base (basic)
        {
            // private ctor
        }

        public static OutboundComm FromBasic (BasicOutboundComm basic)
        {
            return new OutboundComm(basic);
        }

        public static OutboundComm FromIdentity (int outboundCommId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetOutboundComm(outboundCommId));
        }

        public static OutboundComm Create(Person sender, Person from, Organization organization)
        {
            return null;
        }





    }

    public enum CommResolverClasses
    {
        Unknown = 0,
        Test_Foo_Bar

    }

    public enum CommTransmitterClasses
    {
        Unknown = 0,

    }
}
