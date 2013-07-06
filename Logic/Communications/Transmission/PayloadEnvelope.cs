using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swarmops.Logic.Communications.Transmission
{
    [Serializable]
    public class PayloadEnvelope: PayloadBase<PayloadEnvelope>
    {
        public PayloadEnvelope(IPayload payload)
        {
            PayloadClass = payload.GetType().ToString();
            PayloadXml = payload.ToXml();
        }

        public string PayloadClass { get; set; }
        public string PayloadXml { get; set; }
    }
}
