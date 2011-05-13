using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Financial;

namespace Activizr.Logic.Tasks
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
