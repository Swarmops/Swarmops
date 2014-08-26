using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swarmops.Logic.Support
{
    public class PilotInstallationIds
    {
        static public string PiratePartySE
        { get { return "d7588903-5fd0-40cf-a5b1-9af7a722cb6e"; } }

        static public bool IsPilot(string installationId)
        {
            string thisInstallationId = Persistence.Key["SwarmopsInstallationId"];
            if (installationId == thisInstallationId)
            {
                return true;
            }

            return false;
        }
    }


}
