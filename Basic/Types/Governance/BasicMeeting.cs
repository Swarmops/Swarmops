using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Governance
{
    public class BasicMeeting : IHasIdentity
    {
        public BasicMeeting (int meetingId, int organizationId, string name, DateTime motionSubmissionEnds,
            DateTime amendmentSubmissionEnds, DateTime amendmentVotingStarts, DateTime amendmentVotingEnds,
            DateTime motionVotingStarts, DateTime motionVotingEnds)
        {
            MeetingId = meetingId;
            OrganizationId = organizationId;
            Name = name;
            MotionSubmissionEnds = motionSubmissionEnds;
            AmendmentSubmissionEnds = amendmentSubmissionEnds;
            AmendmentVotingStarts = amendmentVotingStarts;
            AmendmentVotingEnds = amendmentVotingEnds;
            MotionVotingStarts = motionVotingStarts;
            MotionVotingEnds = motionVotingEnds;
        }

        public BasicMeeting (BasicMeeting original) :
            this (
            original.MeetingId, original.OrganizationId, original.Name, original.MotionSubmissionEnds,
            original.AmendmentSubmissionEnds, original.AmendmentVotingStarts, original.AmendmentVotingEnds,
            original.MotionVotingStarts, original.MotionVotingEnds)
        {
            // empty copy ctor
        }

        public int MeetingId { get; private set; }
        public int OrganizationId { get; private set; }
        public string Name { get; private set; }
        public DateTime MotionSubmissionEnds { get; protected set; }
        public DateTime AmendmentSubmissionEnds { get; protected set; }
        public DateTime AmendmentVotingStarts { get; private set; }
        public DateTime AmendmentVotingEnds { get; private set; }
        public DateTime MotionVotingStarts { get; private set; }
        public DateTime MotionVotingEnds { get; private set; }

        public int Identity
        {
            get { return MeetingId; }
        }
    }
}