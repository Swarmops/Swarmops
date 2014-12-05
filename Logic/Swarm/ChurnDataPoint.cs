using Swarmops.Basic.Types;

namespace Swarmops.Logic.Swarm
{
    public class ChurnDataPoint : BasicChurnDataPoint
    {
        private ChurnDataPoint() : base (null)
        {
        }

        private ChurnDataPoint (BasicChurnDataPoint basic) : base (basic)
        {
        }

        internal static ChurnDataPoint FromBasic (BasicChurnDataPoint basic)
        {
            return new ChurnDataPoint (basic);
        }
    }
}