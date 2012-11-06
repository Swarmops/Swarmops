using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Communications
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
                return FromArray(PirateDb.GetDatabaseForReading().GetCommunicationTurnarounds(organization));
            }
            else
            {
                return
                    FromArray(PirateDb.GetDatabaseForReading().GetCommunicationTurnarounds(organization,
                                                                                 DatabaseCondition.OpenTrue));
            }
        }
    }
}
