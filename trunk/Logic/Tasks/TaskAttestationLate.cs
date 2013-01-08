using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Financial;

namespace Swarmops.Logic.Tasks
{
    public class TaskAttestationLate: TaskBase
    {
        public TaskAttestationLate (InboundInvoice invoice): base (invoice.Identity, "Invoice #" + invoice.Identity.ToString(), invoice.CreatedDateTime, invoice.DueDate)
        {
            
        }
    }
}
