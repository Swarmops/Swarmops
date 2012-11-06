using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class Election: BasicElection
    {
        private Election (BasicElection basic): base (basic)
        {
            // empty private ctor
        }

        public static Election FromBasic (BasicElection basic)
        {
            return new Election(basic);
        }

        public static Election FromIdentity (int electionId)
        {
            switch (electionId)
            {
                case 1:
                    return
                        FromBasic(new BasicElection(1, "SE General Elections", Country.FromCode("SE").GeographyId,
                                                    new DateTime(2006, 9, 17)));
                case 2:
                    return
                        FromBasic(new BasicElection(2, "SE European Elections", Country.FromCode("SE").GeographyId,
                                                    new DateTime(2009, 6, 7)));
                case 3:
                    return
                        FromBasic(new BasicElection(3, "SE General Elections", Country.FromCode("SE").GeographyId,
                                                    new DateTime(2010, 9, 19)));

                default:
                    throw new NotImplementedException("Elections are not in the db yet.");
            }
        }

        public People GetDocumentedCandidates (Organization organization)
        {
            return
                People.FromIdentities(PirateDb.GetDatabaseForReading().GetDocumentedCandidates(this.Identity,
                                                                                     organization.Identity));
        }

        public void SetCandidateDocumented (Organization organization, Person candidate)
        {
            PirateDb.GetDatabaseForWriting().SetCandidateDocumentationReceived(this.Identity, organization.Identity, candidate.Identity);
        }

        public static Election September2010 { get { return FromIdentity(3); } }
    }
}
