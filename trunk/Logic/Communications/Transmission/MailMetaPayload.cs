using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swarmops.Logic.Communications.Transmission
{
    [Serializable]
    public class MailMetaPayload: PayloadBase<MailMetaPayload>
    {
        public string PayloadClass { get; set; }
        public string PayloadXml { get; set; }
    }
}
