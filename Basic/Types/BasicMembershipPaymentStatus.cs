using System;
using Swarmops.Basic.Enums;

namespace Swarmops.Basic.Types
{
    public class BasicMembershipPaymentStatus
    {
        public BasicMembershipPaymentStatus (int membershipId, MembershipPaymentStatus status, DateTime statusDateTime)
        {
            MembershipId = membershipId;
            Status = status;
            StatusDateTime = statusDateTime;
        }

        public int MembershipId { get; protected set; }
        public MembershipPaymentStatus Status { get; set; }
        public DateTime StatusDateTime { get; set; }
    }
}