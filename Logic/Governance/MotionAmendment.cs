using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class MotionAmendment : BasicMotionAmendment
    {
        private MotionAmendment(BasicMotionAmendment basic) :
            base(basic)
        {
            // empty ctor
        }

        public Motion Motion
        {
            get { return Motion.FromIdentity(MotionId); }
        }

        public string Designation
        {
            get { return Motion.SequenceNumber + "-" + ((char) ('A' + SequenceNumber - 1)); }
            // HACK: only works till Z, need something that works beyond
        }

        public Person Submitter
        {
            get { return Person.FromIdentity(SubmittedByPersonId); }
        }

        public static MotionAmendment FromBasic(BasicMotionAmendment basic)
        {
            return new MotionAmendment(basic);
        }

        public static MotionAmendment FromIdentity(int motionAmendmentId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetMotionAmendment(motionAmendmentId));
        }

        public static MotionAmendment Create(Motion motion, string title, string text, string decisionPoint,
            Person submittingPerson, Person creatingPerson)
        {
            return
                FromIdentity(SwarmDb.GetDatabaseForWriting()
                    .CreateMotionAmendment(motion.Identity, submittingPerson.Identity, creatingPerson.Identity, title,
                        text, decisionPoint));
        }
    }
}