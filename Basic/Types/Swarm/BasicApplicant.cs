using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicApplicant: IHasIdentity
    {
        // public ctor - for use by database class

        BasicApplicant(int applicantId, int personId, int organizationId, DateTime createdDateTime, bool open,
            DateTime grantedDateTime, int grantedByPersonId, int score1, int score2, int score3)
        {
            this.ApplicantId = applicantId;
            this.PersonId = personId;
            this.OrganizationId = organizationId;
            this.Open = open;
            this.CreatedDateTime = createdDateTime;
            this.GrantedDateTime = grantedDateTime;
            this.GrantedByPersonId = grantedByPersonId;
            this.Score1 = score1;
            this.Score2 = score2;
            this.Score3 = score3;
        }


        private BasicApplicant(BasicApplicant original)
            : this(
                original.Identity, original.PersonId, original.OrganizationId, original.CreatedDateTime, original.Open,
                original.GrantedDateTime, original.GrantedByPersonId, original.Score1, original.Score2, original.Score3)
        {
            // copy ctor
        }

        public int ApplicantId { get; private set; }
        public int PersonId { get; private set; }
        public int OrganizationId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public bool Open { get; protected set; }
        public DateTime GrantedDateTime { get; protected set; }
        public int GrantedByPersonId { get; protected set; }
        public int Score1 { get; protected set; }
        public int Score2 { get; protected set; }
        public int Score3 { get; protected set; }

        public int Identity { get { return ApplicantId; } }
    }
}
