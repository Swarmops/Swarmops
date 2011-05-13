using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicVolunteer : IHasIdentity
    {
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
            : this(original.volunteerId, original.personId, original.ownerPersonId, original.openedDateTime,
                   original.open, original.closedDateTime, original.closingComments)
        {
            // copy ctor
        }


        private int volunteerId;
        private int personId;
        private int ownerPersonId;
        private DateTime openedDateTime;
        private bool open;
        private DateTime closedDateTime;
        private string closingComments;

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