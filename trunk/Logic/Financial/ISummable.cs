using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Enums;

namespace Swarmops.Logic.Financial
{
    public interface ISummable
    {
        long SumCents { get; }
        OrganizationFinancialAccountType CounterAccountType { get; }
    }
}
