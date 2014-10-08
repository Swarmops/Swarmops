using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public interface IAttestable
    {
        void Attest(Person attester);
        void Deattest(Person deattester);

        FinancialAccount Budget { get; }
    }
}
