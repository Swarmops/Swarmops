using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Support.SocketMessages;

namespace Swarmops.Logic.Support
{
    public class ProgressBarBackend
    {
        public ProgressBarBackend(string guid)
        {
            this.Guid = guid;
        }

        public void Set (int progress)
        {
            GuidCache.Set(this.Guid + "-Progress", progress);
            SocketMessage message = new SocketMessage
            {
                MessageType = "BroadcastProgress",
                Guid = this.Guid,
                Progress = progress
            };
            message.SendUpstream();
        }

        public string Guid { get; private set; }
    }
}
