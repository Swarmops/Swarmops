using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    class CashAdvanceAttestedLogEntry: CashAdvanceRequestedLogEntry
    {
        // Identical class
        
        CashAdvanceAttestedLogEntry(Person attestingPerson, double amount, FinancialAccount budget, string reason)
            :base (attestingPerson, amount, budget, reason)
        {
            
        }
    }
}
