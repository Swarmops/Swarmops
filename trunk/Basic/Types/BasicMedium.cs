using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicMedium : IHasIdentity
    {
        public BasicMedium (int mediumId, string name, PoliticalAffiliation politicalAffiliation)
        {
            this.mediumId = mediumId;
            this.name = name;
            this.politicalAffiliation = politicalAffiliation;
        }

        public BasicMedium (int mediumId, string name)
            : this(mediumId, name, Enums.PoliticalAffiliation.NotPolitical)
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

        private int mediumId;
        private string name;
        private PoliticalAffiliation politicalAffiliation;

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.MediumId; }
        }

        #endregion
    }
}