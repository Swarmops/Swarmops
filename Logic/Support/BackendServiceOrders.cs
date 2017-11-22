using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.System;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class BackendServiceOrders:PluralBase<BackendServiceOrders, BackendServiceOrder, BasicBackendServiceOrder>
    {
        public static BackendServiceOrders GetNextBatch(int batchSize)
        {
            return
                FromArray(SwarmDb.GetDatabaseForReading()
                    .GetBackendServiceOrderBatch(batchSize, DatabaseCondition.ActiveFalse, DatabaseCondition.OpenTrue));
        }

        public void Execute()
        {
            foreach (BackendServiceOrder order in this)
            {
                order.Execute();
            }
        }
    }
}
