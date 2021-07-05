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

                case LocDomain.PagesFinancial:
                    return Pages_Financial.ResourceManager.GetString(stringResource);

                case LocDomain.PagesLedgers:
                    return Pages_Ledgers.ResourceManager.GetString(stringResource);

                case LocDomain.ControlsFinancial:
                    return Controls_Financial.ResourceManager.GetString(stringResource);

                default:
                    throw new NotImplementedException("Unimplemented localization domain: " + domain.ToString());
            }
        }
    }

    public enum LocalizedString
    {
        Unknown = 0
    };
}
