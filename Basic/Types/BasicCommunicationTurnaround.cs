using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicCommunicationTurnaround : IHasIdentity
    {
        public BasicCommunicationTurnaround (int organizationId, int communicationTypeId, int communicationId,
            DateTime dateTimeOpened, DateTime dateTimeFirstResponse, int personIdFirstResponse, DateTime dateTimeClosed,
            int personIdClosed, bool open, bool responded)
        {
            OrganizationId = organizationId;
            CommunicationTypeId = communicationTypeId;
            CommunicationId = communicationId;
            DateTimeOpened = dateTimeOpened;
            DateTimeFirstResponse = dateTimeFirstResponse;
            PersonIdFirstResponse = personIdFirstResponse;
            DateTimeClosed = dateTimeClosed;
            PersonIdClosed = personIdClosed;
            Open = open;
            Responded = responded;
        }

        public BasicCommunicationTurnaround (BasicCommunicationTurnaround original) :
            this (
            original.OrganizationId, original.CommunicationTypeId, original.CommunicationId, original.DateTimeOpened,
            original.DateTimeFirstResponse, original.PersonIdFirstResponse, original.DateTimeClosed,
            original.PersonIdClosed, original.Open, original.Responded)
        {
            // empty copy ctor
        }


        public int OrganizationId { get; private set; }
        public int CommunicationTypeId { get; private set; }
        public int CommunicationId { get; private set; }
        public DateTime DateTimeOpened { get; private set; }
        public DateTime DateTimeFirstResponse { get; protected set; }
        public int PersonIdFirstResponse { get; protected set; }
        public DateTime DateTimeClosed { get; protected set; }
        public int PersonIdClosed { get; protected set; }
        public bool Open { get; protected set; }
        public bool Responded { get; protected set; }

        public int Identity
        {
            get { return CommunicationId; } // ok, this isn't great, but wtf
        }
    }
}