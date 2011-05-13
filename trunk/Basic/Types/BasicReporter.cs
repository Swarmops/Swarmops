using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicReporter : IEmailPerson, IHasIdentity
    {
        public BasicReporter (int reporterId, string name, string email, int[] mediaCategoryIds)
        {
            this.reporterId = reporterId;
            this.name = name;
            this.email = email;
            this.mediaCategoryIds = mediaCategoryIds;
        }

        public BasicReporter (BasicReporter original) :
            this(original.reporterId, original.name, original.email, original.mediaCategoryIds)
        {
        }

        public BasicReporter()
        {
            reporterId = 0;
            name = string.Empty;
            email = string.Empty;
            mediaCategoryIds = new int[0];
        }

        public int ReporterId
        {
            get { return this.reporterId; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Email
        {
            get { return this.email; }
        }

        public int[] MediaCategoryIds
        {
            get { return this.mediaCategoryIds; }
            set { this.mediaCategoryIds = value; } // this is an exception to the norm
        }


        private int reporterId;
        private string name;
        private string email;
        private int[] mediaCategoryIds; // null if not loaded

        #region IHasIdentity Members

        public int Identity
        {
            get { return ReporterId; }
        }

        #endregion
    }
}