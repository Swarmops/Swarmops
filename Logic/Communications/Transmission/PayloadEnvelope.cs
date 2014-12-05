using System;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Communications.Transmission
{
    [Serializable]
    public class PayloadEnvelope : PayloadBase<PayloadEnvelope>
    {
        public PayloadEnvelope()
        {
            // default public ctor for serializability
        }

        public PayloadEnvelope (IXmlPayload payload)
        {
            PayloadClass = payload.GetType().ToString();
            PayloadXml = payload.ToXml();
        }

        public string PayloadClass { get; set; }
        public string PayloadXml { get; set; }
    }
}