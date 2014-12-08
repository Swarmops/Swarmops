using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMedium : IHasIdentity
    {
        private readonly int mediumId;
        private readonly string name;
        private readonly PoliticalAffiliation politicalAffiliation;

        #region IHasIdentity Members

        public int Identity
        {
            get { return MediumId; }
        }

        #endregion

        public BasicMedium (int mediumId, string name, PoliticalAffiliation politicalAffiliation)
        {
            this.mediumId = mediumId;
            this.name = name;
            this.politicalAffiliation = politicalAffiliation;
        }

        public BasicMedium (int mediumId, string name)
            : this (mediumId, name, PoliticalAffiliation.NotPolitical)
        {
        }

        public string Name
        {
            get { return this.name; }
        }

        public PoliticalAffiliation PoliticalAffiliation
        {
            get { return this.politicalAffiliation; }
        }

        public int MediumId
        {
            get { return this.mediumId; }
        }
    }
}