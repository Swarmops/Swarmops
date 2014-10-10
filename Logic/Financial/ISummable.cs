using Swarmops.Basic.Enums;

namespace Swarmops.Logic.Financial
{
    public interface ISummable
    {
        long SumCents { get; }
        OrganizationFinancialAccountType CounterAccountType { get; }
    }
}
