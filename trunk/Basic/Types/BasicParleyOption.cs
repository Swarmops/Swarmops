using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicParleyOption: IHasIdentity
    {
        public BasicParleyOption (int parleyOptionId, int parleyId, string description, Int64 amountCents, bool active)
        {
            this.ParleyOptionId = parleyOptionId;
            this.ParleyId = parleyId;
            this.Description = description;
            this.AmountCents = amountCents;
            this.Active = active;
        }

        public BasicParleyOption (BasicParleyOption original)
            : this (original.ParleyOptionId, original.ParleyId, original.Description, original.AmountCents, original.Active)
        {
            // empty copy ctor
        }

        public int ParleyOptionId { get; private set; }
        public int ParleyId { get; private set; }
        public string Description { get; private set; }
        public Int64 AmountCents { get; private set; }
        public bool Active { get; protected set; }

        public int Identity
        {
            get { return this.ParleyOptionId; }
        }
    }
}
