using Swarmops.Basic.Enums;

namespace Swarmops.Basic.Types.Financial
{
    /// <summary>
    ///     This class encapsulates what a financial transaction was constructed from.
    /// </summary>
    public class BasicFinancialDependency
    {
        public BasicFinancialDependency (int objectId, FinancialDependencyType dependencyType, int foreignId)
        {
            ObjectId = objectId;
            DependencyType = dependencyType;
            ForeignId = foreignId;
        }

        public int ObjectId { get; private set; }
        public FinancialDependencyType DependencyType { get; private set; }
        public int ForeignId { get; private set; }
    }
}