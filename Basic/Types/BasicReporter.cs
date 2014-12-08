using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicReporter : IEmailPerson, IHasIdentity
    {
        private readonly string email;
        private readonly string name;
        private readonly int reporterId;
        private int[] mediaCategoryIds; // null if not loaded

        #region IHasIdentity Members

        public int Identity
        {
            get { return ReporterId; }
        }

        #endregion

        public BasicReporter (int reporterId, string name, string email, int[] mediaCategoryIds)
        {
            this.reporterId = reporterId;
            this.name = name;
            this.email = email;
            this.mediaCategoryIds = mediaCategoryIds;
        }

        public BasicReporter (BasicReporter original) :
            this (original.reporterId, original.name, original.email, original.mediaCategoryIds)
        {
        }

        public BasicReporter()
        {
            this.reporterId = 0;
            this.name = string.Empty;
            this.email = string.Empty;
            this.mediaCategoryIds = new int[0];
        }

        public int ReporterId
        {
            get { return this.reporterId; }
        }

        public int[] MediaCategoryIds
        {
            get { return this.mediaCategoryIds; }
            set { this.mediaCategoryIds = value; } // this is an exception to the norm
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Email
        {
            get { return this.email; }
        }
    }
}