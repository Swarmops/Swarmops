using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Logic.App_GlobalResources;

namespace Swarmops.Logic.Swarm
{
    public class Position
    {

        public static string Localized (PositionType title, bool plural = false)
        {
            string titleString = title.ToString();
            if (plural)
            {
                titleString += "_Plural";
            }

            return ParticipantTitles.ResourceManager.GetString ("Position_" + titleString);
        }
    }
}
