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
        /// <summary>
        /// Different countries will require different data to execute a domestic transfer.
        /// </summary>
        DomesticBankTransfer,
        /// <summary>
        /// The "Giro" is a special meta-account in some countries that map to an underlying (changeable) account.
        /// </summary>
        DomesticBankGiro,
        /// <summary>
        /// The international bank transfer assumes the IBAN/BIC standard. This is used by all but the recalcitrant United States.
        /// </summary>
        InternationalBankTransfer
    }
}
