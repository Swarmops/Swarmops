using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;
using Activizr.Database;

namespace Activizr.Logic.Support
{
    public class TemporaryIdentity: IHasIdentity
    {
        public TemporaryIdentity (int temporaryId)
        {
            this.temporaryId = temporaryId;
        }

        static public TemporaryIdentity GetNew()
        {
            return new TemporaryIdentity (PirateDb.GetDatabase().GetTemporaryIdentity());
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
