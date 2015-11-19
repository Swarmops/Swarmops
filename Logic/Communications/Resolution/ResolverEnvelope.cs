using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;


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
            ResolverClass = resolver.GetType().ToString();
            ResolverDataXml = resolver.ToXml();
        }

        public string ResolverClass { get; set; }
        public string ResolverDataXml { get; set; }
    }
}
