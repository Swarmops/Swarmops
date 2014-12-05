using Swarmops.Logic.Financial;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskAttestationLate : TaskBase
    {
        public TaskAttestationLate (InboundInvoice invoice)
            : base (invoice.Identity, "Invoice #" + invoice.Identity, invoice.CreatedDateTime, invoice.DueDate)
        {
        }
    }
}