using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class PaymentTransferMethodInfo
    {
        public Currency Currency { get; set; }
        public PaymentTargetType TargetType { get; set; }
        public string LocalizedPaymentMethodName { get; set; }
        Dictionary<string,string> LocalizedPaymentInformation { get; set; }

        public static PaymentTransferMethodInfo FromObject(IHasIdentity financialObject)
        {
            if (financialObject is ExpenseClaim)
            {
                return FromObject((financialObject as ExpenseClaim).Claimer);
            }

            if (financialObject is CashAdvance)
            {
                return FromObject((financialObject as CashAdvance).Person);
            }

            if (financialObject is Salary)
            {
                return FromObject((financialObject as Salary).PayrollItem.Person);
            }

            if (financialObject is Person)
            {
                Person person = financialObject as Person;

                // For now, assume the bank - clearing - account triplet
                // TODO: Implement payment targets on teh Person object

                PaymentTransferMethodInfo result = new PaymentTransferMethodInfo();
                result.TargetType = PaymentTargetType.DomesticBankTransfer;
            }
        }

    }
}
