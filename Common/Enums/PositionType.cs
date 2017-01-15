using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Enums
{
    public enum PositionType
    {
        // ReSharper disable InconsistentNaming
        Unknown = 0,
        System_SysadminMain,
        System_SysadminReadWrite,
        System_SysadminAssistantReadOnly,
        Strategic_BoardChairperson,
        Strategic_BoardChairpersonDeputy,
        Strategic_BoardMember,
        Strategic_BoardDeputy,
        Strategic_BoardAssistant,
        Strategic_Auditor,
        Strategic_AuditorDeputy,
        Strategic_AuditorAssistant,
        Strategic_Observer,
        Executive_ChiefAdminOfficer,
        Executive_ChiefExecutiveOfficer,
        Executive_CeoAssistant,
        Executive_VicePresident,
        Executive_ChiefStaff,
        Executive_ChiefHrOfficer,
        Executive_HrAssistant,
        Executive_ChiefFinancialOfficer,
        Executive_CfoAssistant,
        Executive_ChiefMarketingOfficer,
        Executive_MarketingAssistant,
        Executive_ChiefTechnologyOfficer,
        Executive_ChiefInformationOfficer,
        Executive_ChiefOperationsOfficer,
        Geographic_Leader,
        Geographic_Deputy,
        Geographic_Assistant,
        Geographic_SwarmLeader,     /* dotreports to chief of staff */
        Geographic_SwarmLeaderAssistant,
        Geographic_InfoLeader,      /* dotreports to CIO */
        Geographic_InfoLeaderAssistant,
        Geographic_ActivismLeader,  /* dotreports to Operations */ 
        Geographic_ActivismLeaderAssistant,
        Geographic_OutreachLeader,  /* dotreports to Marketing */
        Geographic_OutreachLeaderAssistant,
        Geographic_ExpandingSubleaderNode  /* Just a UX element, not a position */
        // ReSharper restore InconsistentNaming
    }
}
