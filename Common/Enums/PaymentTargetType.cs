using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Enums
{
    public enum PaymentTargetType
    {
        Unknown = 0,
        BitcoinCashAccount,
        DomesticBankTransfer,
        DomesticBankGiro,
        InternationalBankTransfer
    }
}
