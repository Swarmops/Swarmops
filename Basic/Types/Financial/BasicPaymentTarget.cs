using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicPaymentTarget: IHasIdentity
    {
        public int PaymentTargetId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public int ForeignId { get; private set; }
        public ForeignObjectType ForeignType { get; private set; }
        public bool Active { get; protected set; }
        public bool Open { get; protected set; }
        public int CurrencyId { get; private set; }
        public string TargetType { get; private set; }
        public string Description { get; private set; }
        public Dictionary<PaymentTargetField, string> Fields { get; private set; }


        public int Identity { get { return PaymentTargetId; } }
    }
}
