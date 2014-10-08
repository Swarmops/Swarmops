using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class Motion: BasicMotion
    {
        private Motion (BasicMotion basic): base (basic)
        {
            // empty ctor
        }

        public static Motion FromBasic (BasicMotion basic)
        {
            return new Motion(basic);
        }

        public static Motion FromIdentity (int motionId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetMotion(motionId));
        }

        public static Motion Create (Meeting meeting, Person submittingPerson, Person creatingPerson, string title, string text, string decisionPoints)
        {
            return
                Motion.FromIdentity(SwarmDb.GetDatabaseForWriting().CreateMotion(meeting.Identity, submittingPerson.Identity, creatingPerson.Identity, title, text, decisionPoints));
        }

        public MotionAmendments Amendments
        {
            get
            {
                return MotionAmendments.ForMotion(this);
            }
        }

        public MotionAmendment AddAmendment (string title, string text, string decisionPoint, Person submittingPerson, Person createdByPerson)
        {
            return MotionAmendment.Create(this, title, text, decisionPoint, submittingPerson, createdByPerson);
        }

        public Meeting Meeting
        {
            get { return Meeting.FromIdentity(this.MeetingId); }
        }

        public Person Submitter
        {
            get { return Person.FromIdentity(this.SubmittedByPersonId); }
        }
    }
}