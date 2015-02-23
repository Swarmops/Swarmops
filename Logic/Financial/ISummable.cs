using Swarmops.Common.Enums;

namespace Swarmops.Logic.Financial
{
    public interface ISummable
    {
        long SumCents { get; }
        OrganizationFinancialAccountType CounterAccountType { get; }
    }
}