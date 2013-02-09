using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class TemporaryIdentity: IHasIdentity
    {
        public TemporaryIdentity (int temporaryId)
        {
            this.temporaryId = temporaryId;
        }

        static public TemporaryIdentity GetNew()
        {
            return new TemporaryIdentity (SwarmDb.GetDatabaseForWriting().GetTemporaryIdentity());  // This is actually an Insert, not a Select
        }

        private int temporaryId;

        public int Identity
        {
            get
            {
                return temporaryId;
            }
        }
    }
}
