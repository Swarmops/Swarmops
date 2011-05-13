using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class Payments: PluralBase<Payments,Payment,BasicPayment>
    {
        public static Payments ForPaymentGroup (PaymentGroup group)
        {
            return FromArray(PirateDb.GetDatabase().GetPayments(group));
        }
    }
}
