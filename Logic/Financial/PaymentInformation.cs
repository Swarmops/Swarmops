using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class PaymentInformation: BasicPaymentInformation
    {
        private PaymentInformation (BasicPaymentInformation basic): base (basic)
        {
            // empty
        }

        public static PaymentInformation FromBasic (BasicPaymentInformation basic)
        {
            return new PaymentInformation(basic);
        }
    }

    public class PaymentInformationList: List<PaymentInformation>
    {
        static public PaymentInformationList FromArray(BasicPaymentInformation[] basicArray)
        {
            var result = new PaymentInformationList() { Capacity = basicArray.Length * 11 / 10 };

            foreach (BasicPaymentInformation basic in basicArray)
            {
                result.Add((PaymentInformation)PaymentInformation.FromBasic(basic));
            }

            return result;
        }
        
        
        public static PaymentInformationList ForPayment (Payment payment)
        {
            BasicPaymentInformation[] basicInfoList = SwarmDb.GetDatabaseForReading().GetPaymentInformation(payment.Identity);

            return FromArray(basicInfoList);
        }
    }
}
