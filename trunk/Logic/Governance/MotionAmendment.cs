using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Pirates;
using Swarmops.Basic.Types.Governance;
using Swarmops.Database;

namespace Swarmops.Logic.Governance
{
    public class MotionAmendment: BasicMotionAmendment
    {
        private MotionAmendment (BasicMotionAmendment basic):
            base (basic)
        {
            // empty ctor
        }

        public static MotionAmendment FromBasic (BasicMotionAmendment basic)
        {
            return new MotionAmendment(basic);
        }

        public static MotionAmendment FromIdentity (int motionAmendmentId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetMotionAmendment(motionAmendmentId));
        }

        public static MotionAmendment Create (Motion motion, string title, string text, string decisionPoint, Person submittingPerson, Person creatingPerson)
        {
            return FromIdentity(PirateDb.GetDatabaseForWriting().CreateMotionAmendment (motion.Identity, submittingPerson.Identity, creatingPerson.Identity, title, text, decisionPoint));
        }

        public Motion Motion
        {
            get { return Motion.FromIdentity(this.MotionId); }
        }

        public string Designation
        {
            get { return this.Motion.SequenceNumber + "-" + ((char) ((int) ('A') + this.SequenceNumber - 1)); }  // HACK: only works till Z, need something that works beyond
        }

        public Person Submitter
        {
            get { return Person.FromIdentity(this.SubmittedByPersonId); }
        }
    }
}