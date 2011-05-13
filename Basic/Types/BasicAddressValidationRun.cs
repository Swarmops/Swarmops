using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
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