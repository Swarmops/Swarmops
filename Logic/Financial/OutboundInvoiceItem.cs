using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class OutboundInvoiceItem : BasicOutboundInvoiceItem
    {
        private OutboundInvoiceItem (BasicOutboundInvoiceItem basic) : base (basic)
        {
            // empty ctor
        }

        public decimal Amount
        {
            get { return AmountCents/100.0m; }
        }

        public static OutboundInvoiceItem FromBasic (BasicOutboundInvoiceItem basic)
        {
            return new OutboundInvoiceItem (basic);
        }

        public static OutboundInvoiceItem FromIdentity (int outboundInvoiceItemId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetOutboundInvoiceItem (outboundInvoiceItemId));
        }
    }
}