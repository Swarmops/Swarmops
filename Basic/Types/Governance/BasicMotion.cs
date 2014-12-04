using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Governance
{
    public class BasicMotion : IHasIdentity
    {
        public BasicMotion(int motionId, int meetingId, int sequenceNumber, string designation, int submittedByPersonId,
            int createdByPersonId, DateTime createdDateTime, bool amended, int amendedByPersonId,
            DateTime amendedDateTime, string title, string text, string amendedText, string decisionPoints,
            string amendedDecisionPoints, string threadUrl, bool open, bool carried)
        {
            MotionId = motionId;
            MeetingId = meetingId;
            SequenceNumber = sequenceNumber;
            Designation = designation;
            SubmittedByPersonId = submittedByPersonId;
            CreatedByPersonId = createdByPersonId;
            CreatedDateTime = createdDateTime;
            Amended = amended;
            AmendedByPersonId = amendedByPersonId;
            AmendedDateTime = amendedDateTime;
            Title = title;
            Text = text;
            AmendedText = amendedText;
            DecisionPoints = decisionPoints;
            AmendedDecisionPoints = amendedDecisionPoints;
            ThreadUrl = threadUrl;
            Open = open;
            Carried = carried;
        }

        public BasicMotion(BasicMotion original) :
            this(
            original.MotionId, original.MeetingId, original.SequenceNumber, original.Designation,
            original.SubmittedByPersonId, original.CreatedByPersonId, original.CreatedDateTime, original.Amended,
            original.AmendedByPersonId, original.AmendedDateTime, original.Title, original.Text, original.AmendedText,
            original.DecisionPoints, original.AmendedDecisionPoints, original.ThreadUrl, original.Open, original.Carried
            )
        {
            // empty copy ctor
        }


        public int MotionId { get; private set; }
        public int MeetingId { get; private set; }
        public int SubmittedByPersonId { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public int AmendedByPersonId { get; protected set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime AmendedDateTime { get; protected set; }
        public string Designation { get; private set; }
        public string Title { get; private set; }
        public string Text { get; private set; }
        public string AmendedText { get; protected set; }
        public string DecisionPoints { get; private set; }
        public string AmendedDecisionPoints { get; protected set; }
        public string ThreadUrl { get; private set; }
        public bool Open { get; protected set; }
        public bool Carried { get; protected set; }
        public bool Amended { get; protected set; }
        public int SequenceNumber { get; private set; }


        public int Identity
        {
            get { return MotionId; }
        }
    }
}