using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Localization
{
    public static class LocalizedStrings
    {
        public static string Get (LocDomain domain, string stringResource)
        {
            switch (domain)
            {
                case LocDomain.Global:
                    return Global.ResourceManager.GetString(stringResource);

                case LocDomain.Menu5:
                    return Menu5.ResourceManager.GetString(stringResource);

                case LocDomain.PagesAdmin:
                    return Pages_Admin.ResourceManager.GetString(stringResource);

                case LocDomain.PagesComms:
                    return Pages_Comms.ResourceManager.GetString(stringResource);

                case LocDomain.PagesFinancial:
                    return Pages_Financial.ResourceManager.GetString(stringResource);

                case LocDomain.PagesGovernance:
                    return Pages_Governance.ResourceManager.GetString(stringResource);
                
                case LocDomain.PagesLedgers:
                    return Pages_Ledgers.ResourceManager.GetString(stringResource);

                case LocDomain.PagesPublic:
                    return Pages_Public.ResourceManager.GetString(stringResource);

                case LocDomain.PagesSecurity:
                    return Pages_Security.ResourceManager.GetString(stringResource);

                case LocDomain.PagesSwarm:
                    return Pages_Swarm.ResourceManager.GetString(stringResource);

                case LocDomain.ControlsFinancial:
                    return Controls_Financial.ResourceManager.GetString(stringResource);

                case LocDomain.ControlsSwarm:
                    return Controls_Swarm.ResourceManager.GetString(stringResource);

                default:
                    throw new NotImplementedException("Unimplemented localization domain: " + domain.ToString());
            }
        }
    }
}
