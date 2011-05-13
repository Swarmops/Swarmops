using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Support;

namespace Activizr.Logic.Financial
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
            BasicPaymentInformation[] basicInfoList = PirateDb.GetDatabase().GetPaymentInformation(payment.Identity);

            return FromArray(basicInfoList);
        }
    }
}
