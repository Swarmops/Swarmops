using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Frontend.Socket
{
    public enum FrontendMalfunctions
    {
        Unknown = 0,
        BackendConnectionFault,
        BackendHeartbeatLost
    }
}
