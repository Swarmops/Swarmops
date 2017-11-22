using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Swarmops.Logic.Support
{
    public interface IBackendServiceOrderBase
    {
        void Run();
        void Close();
        bool HasWorkerThread { get; set; }
        Thread WorkerThread { get; set; }
    }
}
