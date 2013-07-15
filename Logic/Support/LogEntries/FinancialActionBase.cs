using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Support.Log;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class FinancialActionBase: LogEntryBase<FinancialActionBase>
    {
        public FinancialActionBase()
        {
            // necessary for serialization
        }
        
        public int FinancialAccountId { get; set; }
        public string FinancialAccountName { get; set; }
        public int PersonId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public int OwnerPersonId { get; set; }
        public string OwnerPersonName { get; set; }
        public DateTime DateTime { get; set; }
    }
}
