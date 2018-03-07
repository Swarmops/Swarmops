using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class Applicant: BasicApplicant
    {
        private Applicant(BasicApplicant basic) : base(basic)
        {
            // private ctor
        }

        static public Applicant FromBasic(BasicApplicant basic)
        {
            return new Applicant(basic);
        }

        static public Applicant FromIdentity(int applicantId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetApplicant(applicantId));
        }

        static private Applicant FromIdentityAggressive(int applicantId)
        {
            // "ForWriting" intentional - bypass lag to read instances in multi dbserver setup
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetApplicant(applicantId));
        }

        static public Applicant Create(Person person, Organization organization)
        {
            return
                FromIdentityAggressive(SwarmDb.GetDatabaseForWriting()
                    .CreateApplicant(person.Identity, organization.Identity));
        }
    }
}
