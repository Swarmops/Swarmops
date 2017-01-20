using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Security
{
    [Serializable]
    public class Impersonation
    {
        public int ImpersonatedByPersonId;
        public DateTime ImpersonationStarted;
    }
}
