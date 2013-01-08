using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Financial;

namespace Swarmops.Logic.Tasks
{
    public class TaskInboundInvoice: TaskBase
    {
        public TaskInboundInvoice(InboundInvoice invoice)
            : base(
                invoice.Identity, "Inbound Invoice #" + invoice.Identity.ToString(), invoice.CreatedDateTime,
                invoice.DueDate.AddDays(-7))
        {
            // empty ctor
        }
    }
}
