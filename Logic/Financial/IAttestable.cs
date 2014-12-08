using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public interface IAttestable
    {
        FinancialAccount Budget { get; }
        void Attest (Person attester);
        void Deattest (Person deattester);
    }
}