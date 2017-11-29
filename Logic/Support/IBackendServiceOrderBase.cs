using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public interface IBackendServiceOrderBase
    {
        void Run();
        void Close();


        bool HasWorkerThread { get; set; }
        Thread WorkerThread { get; set; }
        void Terminate();
        bool HasTerminated { get; set; }
        void ThrewException(Exception exception);


        int ServiceOrderIdentity { get; set; }
        Organization Organization { get; set; }
        Person Person { get; set; }
    }
}
