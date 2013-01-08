using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicAddressValidationRun : IHasIdentity
    {
        public BasicAddressValidationRun (int validationRunId)
        {
            this.validationRunId = validationRunId;
        }

        private int validationRunId;
        private bool complete;
        private int addressCount;

        public int Identity
        {
            get { return this.validationRunId; }
        }
    }
}