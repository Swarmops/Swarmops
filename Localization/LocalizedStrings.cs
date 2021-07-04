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
                case LocDomain.Menu5:
                    return Menu5.ResourceManager.GetString(stringResource);

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
