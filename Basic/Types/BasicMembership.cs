using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMembership : IHasIdentity
    {
        public BasicMembership (int personId, int organizationId)
        {
            PersonId = personId;
            OrganizationId = organizationId;
            Active = true;
            DateTerminated = DateTime.MinValue;
        }

        public BasicMembership (int personId, int organizationId, DateTime memberSince, DateTime expires)
            : this (personId, organizationId)
        {
            MemberSince = memberSince;
            Expires = expires;
        }

        public BasicMembership (int membershipId, int personId, int organizationId, DateTime memberSince,
            DateTime expires, bool active, DateTime dateTerminated)
        {
            MembershipId = membershipId;
            PersonId = personId;
            OrganizationId = organizationId;
            MemberSince = memberSince;
            Expires = expires;
            Active = active;
            DateTerminated = dateTerminated;
        }

        public BasicMembership (BasicMembership original)
            : this (
                original.MembershipId, original.PersonId, original.OrganizationId, original.MemberSince,
                original.Expires, original.Active, original.DateTerminated)
        {
            // Empty copy ctor
        }

        public int MembershipId { get; private set; }
        public int OrganizationId { get; private set; }
        public int PersonId { get; private set; }
        public bool Active { get; protected set; }
        public DateTime MemberSince { get; private set; }
        public DateTime DateTerminated { get; protected set; }
        public DateTime Expires { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return MembershipId; }
        }

        #endregion

        // RICK NOTES: I moved the MembershipPayments to a separate table, as they are NULL on most rows.
        // Not implemented as per 2010-01-11.

        // Also, note the renaming of "Valid" to "Active", consistently with other objects and analogous to "Open".
    }
}