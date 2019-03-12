using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public interface IApprovable
    {
        FinancialAccount Budget { get; }
        void Approve (Person approvingPerson);
        void RetractApproval (Person retractingPerson);

        void DenyApproval(Person denyingPerson, string reason);
    }
}