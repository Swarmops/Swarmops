using System;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class Election : BasicElection
    {
        private Election (BasicElection basic) : base (basic)
        {
            // empty private ctor
        }

        public static Election September2010
        {
            get { return FromIdentity (3); }
        }

        public static Election FromBasic (BasicElection basic)
        {
            return new Election (basic);
        }

        public static Election FromIdentity (int electionId)
        {
            switch (electionId)
            {
                case 1:
                    return
                        FromBasic (new BasicElection (1, "SE General Elections", Country.FromCode ("SE").GeographyId,
                            new DateTime (2006, 9, 17)));
                case 2:
                    return
                        FromBasic (new BasicElection (2, "SE European Elections", Country.FromCode ("SE").GeographyId,
                            new DateTime (2009, 6, 7)));
                case 3:
                    return
                        FromBasic (new BasicElection (3, "SE General Elections", Country.FromCode ("SE").GeographyId,
                            new DateTime (2010, 9, 19)));

                default:
                    throw new NotImplementedException ("Elections are not in the db yet.");
            }
        }

        public People GetDocumentedCandidates (Organization organization)
        {
            return
                People.FromIdentities (SwarmDb.GetDatabaseForReading().GetDocumentedCandidates (Identity,
                    organization.Identity));
        }

        public void SetCandidateDocumented (Organization organization, Person candidate)
        {
            SwarmDb.GetDatabaseForWriting()
                .SetCandidateDocumentationReceived (Identity, organization.Identity, candidate.Identity);
        }
    }
}