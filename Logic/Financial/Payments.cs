using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class Payments: PluralBase<Payments,Payment,BasicPayment>
    {
        public static Payments ForPaymentGroup (PaymentGroup group)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPayments(group));
        }
    }
}
