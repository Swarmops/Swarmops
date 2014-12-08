using Swarmops.Basic.Interfaces;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class TemporaryIdentity : IHasIdentity
    {
        private readonly int temporaryId;

        public TemporaryIdentity (int temporaryId)
        {
            this.temporaryId = temporaryId;
        }

        public int Identity
        {
            get { return this.temporaryId; }
        }

        public static TemporaryIdentity GetNew()
        {
            return new TemporaryIdentity (SwarmDb.GetDatabaseForWriting().GetTemporaryIdentity());
            // This is actually an Insert, not a Select
        }
    }
}