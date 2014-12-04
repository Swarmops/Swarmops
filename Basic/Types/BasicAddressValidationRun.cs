using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicAddressValidationRun : IHasIdentity
    {
        private readonly int validationRunId;

        public BasicAddressValidationRun(int validationRunId)
        {
            this.validationRunId = validationRunId;
        }

        public int Identity
        {
            get { return this.validationRunId; }
        }
    }
}