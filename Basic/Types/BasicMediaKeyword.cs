using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicMediaKeyword : IHasIdentity
    {
        private readonly int id;
        private readonly string keyword;

        #region IHasIdentity Members

        public int Identity
        {
            get { return Id; }
        }

        #endregion

        public BasicMediaKeyword (int id, string keyword)
        {
            this.id = id;
            this.keyword = keyword;
        }

        public BasicMediaKeyword (BasicMediaKeyword original)
        {
            this.id = original.id;
            this.keyword = original.keyword;
        }

        public BasicMediaKeyword()
            : this (0, string.Empty)
        {
        }

        public int Id
        {
            get { return this.id; }
        }

        public string Keyword
        {
            get { return this.keyword; }
        }
    }
}