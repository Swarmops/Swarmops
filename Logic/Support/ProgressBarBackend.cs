using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using Swarmops.Logic.Cache;

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

        }

        public string Guid { get; private set; }
    }
}
