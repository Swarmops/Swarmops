using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicVolunteer : IHasIdentity
    {
        private readonly DateTime closedDateTime;
        private readonly string closingComments;
        private readonly bool open;
        private readonly DateTime openedDateTime;
        private readonly int ownerPersonId;
        private readonly int personId;
        private readonly int volunteerId;

        public BasicVolunteer (int volunteerId, int personId, int ownerPersonId, DateTime openedDateTime,
            bool open, DateTime closedDateTime, string closingComments)
        {
            this.volunteerId = volunteerId;
            this.personId = personId;
            this.ownerPersonId = ownerPersonId;
            this.openedDateTime = openedDateTime;
            this.open = open;
            this.closedDateTime = closedDateTime;
            this.closingComments = closingComments;
        }

        public BasicVolunteer (BasicVolunteer original)
            : this (original.volunteerId, original.personId, original.ownerPersonId, original.openedDateTime,
                original.open, original.closedDateTime, original.closingComments)
        {
            // copy ctor
        }

        public int VolunteerId
        {
            get { return this.volunteerId; }
        }

        public int PersonId
        {
            get { return this.personId; }
        }

        public int OwnerPersonId
        {
            get { return this.ownerPersonId; }
        }

        public DateTime OpenedDateTime
        {
            get { return this.openedDateTime; }
        }

        public bool Open
        {
            get { return this.open; }
        }

        public DateTime ClosedDateTime
        {
            get { return this.closedDateTime; }
        }

        public string ClosingComments
        {
            get { return this.closingComments; }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.volunteerId; }
        }

        #endregion
    }
}