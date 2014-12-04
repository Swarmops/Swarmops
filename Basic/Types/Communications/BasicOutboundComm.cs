using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicOutboundComm : IHasIdentity
    {
        public BasicOutboundComm(int outboundCommId, int senderPersonId, int fromPersonId, int organizationId,
            DateTime createdDateTime, string resolverClass, string recipientDataXml, bool resolved,
            DateTime resolvedDateTime, OutboundCommPriority priority, string transmitterClass, string payloadXml,
            bool open, DateTime startTransmitDateTime, DateTime closedDateTime, int recipientCount,
            int recipientsSuccess, int recipientsFail)
        {
            OutboundCommId = outboundCommId;
            SenderPersonId = senderPersonId;
            FromPersonId = fromPersonId;
            OrganizationId = organizationId;
            CreatedDateTime = createdDateTime;
            ResolverClass = resolverClass;
            RecipientDataXml = recipientDataXml;
            Resolved = resolved;
            ResolvedDateTime = resolvedDateTime;
            Priority = priority;
            TransmitterClass = transmitterClass;
            PayloadXml = payloadXml;
            Open = open;
            StartTransmitDateTime = startTransmitDateTime;
            ClosedDateTime = closedDateTime;
            RecipientCount = recipientCount;
            RecipientsSuccess = recipientsSuccess;
            RecipeintsFail = recipientsFail;
        }


        public BasicOutboundComm(BasicOutboundComm original) :
            this(
            original.OutboundCommId, original.SenderPersonId, original.FromPersonId, original.OrganizationId,
            original.CreatedDateTime, original.ResolverClass, original.RecipientDataXml, original.Resolved,
            original.ResolvedDateTime, original.Priority, original.TransmitterClass, original.PayloadXml,
            original.Open, original.StartTransmitDateTime, original.ClosedDateTime, original.RecipientCount,
            original.RecipientsSuccess, original.RecipeintsFail)
        {
            // copy ctor    
        }


        public int OutboundCommId { get; private set; }
        public int SenderPersonId { get; private set; }
        public int FromPersonId { get; private set; }
        public int OrganizationId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public string ResolverClass { get; private set; }
        public string RecipientDataXml { get; private set; }
        public bool Resolved { get; protected set; }
        public DateTime ResolvedDateTime { get; protected set; }
        public OutboundCommPriority Priority { get; private set; }
        public string TransmitterClass { get; private set; }
        public string PayloadXml { get; private set; }
        public bool Open { get; protected set; }
        public DateTime StartTransmitDateTime { get; protected set; }
        public DateTime ClosedDateTime { get; protected set; }
        public int RecipientCount { get; protected set; }
        public int RecipientsSuccess { get; protected set; }
        public int RecipeintsFail { get; protected set; }

        #region Implementation of IHasIdentity

        public int Identity
        {
            get { return OutboundCommId; }
        }

        #endregion
    }
}