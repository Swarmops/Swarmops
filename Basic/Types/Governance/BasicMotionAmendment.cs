using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Governance
{
    public class BasicMotionAmendment : IHasIdentity
    {
        public BasicMotionAmendment(int motionAmendmentId, int motionId, int sequenceNumber, int submittedByPersonId,
            int createdByPersonId, DateTime createdDateTime, string title, string text, string decisionPoint, bool open,
            bool carried)
        {
            MotionAmendmentId = motionAmendmentId;
            MotionId = motionId;
            SequenceNumber = sequenceNumber;
            SubmittedByPersonId = submittedByPersonId;
            CreatedByPersonId = createdByPersonId;
            CreatedDateTime = createdDateTime;
            Title = title;
            Text = text;
            DecisionPoint = decisionPoint;
            Open = open;
            Carried = carried;
        }

        public BasicMotionAmendment(BasicMotionAmendment original) :
            this(
            original.MotionAmendmentId, original.MotionId, original.SequenceNumber, original.SubmittedByPersonId,
            original.CreatedByPersonId, original.CreatedDateTime, original.Title, original.Text, original.DecisionPoint,
            original.Open, original.Carried)
        {
            // empty copy ctor
        }

        public int MotionAmendmentId { get; private set; }
        public int MotionId { get; private set; }
        public int SequenceNumber { get; private set; }
        public int SubmittedByPersonId { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public string Title { get; private set; }
        public string Text { get; private set; }
        public string DecisionPoint { get; private set; }
        public bool Open { get; protected set; }
        public bool Carried { get; protected set; }

        public int Identity
        {
            get { return MotionAmendmentId; }
        }
    }
}