using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicAddressValidationRun : IHasIdentity
    {
        public BasicAddressValidationRun (int validationRunId)
        {
            this.validationRunId = validationRunId;
        }

        private readonly int validationRunId;
        
        public int Identity
        {
            get { return this.validationRunId; }
        }
    }
}