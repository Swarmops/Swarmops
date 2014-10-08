using Swarmops.Logic.Financial;

namespace Swarmops.Logic.DashboardTasks
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
