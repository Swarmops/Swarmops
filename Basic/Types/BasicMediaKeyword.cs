using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    [Serializable]
    public class BasicMediaKeyword : IHasIdentity
    {
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
            : this(0, string.Empty)
        {
        }

        public int Id
        {
            get { return id; }
        }

        public string Keyword
        {
            get { return keyword; }
        }

        private int id;
        private string keyword;

        #region IHasIdentity Members

        public int Identity
        {
            get { return Id; }
        }

        #endregion
    }
}