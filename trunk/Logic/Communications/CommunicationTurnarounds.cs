using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Communications
{
    public class CommunicationTurnarounds: PluralBase<CommunicationTurnarounds,CommunicationTurnaround,BasicCommunicationTurnaround>
    {
        public static CommunicationTurnarounds ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);
        }

        public static CommunicationTurnarounds ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(SwarmDb.GetDatabaseForReading().GetCommunicationTurnarounds(organization));
            }
            else
            {
                return
                    FromArray(SwarmDb.GetDatabaseForReading().GetCommunicationTurnarounds(organization,
                                                                                 DatabaseCondition.OpenTrue));
            }
        }
    }
}
