using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMediaCategory : IHasIdentity
    {
        private readonly int mediaCategoryId;
        private readonly string name;

        #region IHasIdentity Members

        public int Identity
        {
            get { return MediaCategoryId; }
        }

        #endregion

        public BasicMediaCategory (int mediaCategoryId, string name)
        {
            this.mediaCategoryId = mediaCategoryId;
            this.name = name;
        }

        public BasicMediaCategory (BasicMediaCategory original) :
            this (original.mediaCategoryId, original.name)
        {
        }

        public BasicMediaCategory()
        {
            this.mediaCategoryId = 0;
            this.name = string.Empty;
        }

        public string Name
        {
            get { return this.name; }
        }

        public int MediaCategoryId
        {
            get { return this.mediaCategoryId; }
        }
    }
}