using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;
using Swarmops.Logic.Communications.Transmission;

namespace Swarmops.Logic.Communications.Resolution
{
    [Serializable]
    public class ResolverEnvelope : PayloadBase<ResolverEnvelope>
    {
        public ResolverEnvelope()
        {
            // default public ctor for serializability
        }

        public ResolverEnvelope (ICommsResolver resolver)
        {
            PayloadClass = resolver.GetType().ToString();
            PayloadXml = resolver.ToXml();
        }

        public string PayloadClass { get; set; }
        public string PayloadXml { get; set; }
    }
}
