using System;
using Swarmops.Basic.Enums;

namespace Swarmops.Basic.Types
{
    public class BasicPWEvent
    {
        public BasicPWEvent(int eventId, DateTime dateTime, bool open, DateTime processedDateTime, EventType eventType,
            EventSource eventSource, int actingPersonId, int affectedPersonId, int organizationId,
            int geographyId, int parameterInt, string parameterText)
        {
            EventId = eventId;
            DateTime = dateTime;
            Open = open;
            ProcessedDateTime = processedDateTime;
            EventType = eventType;
            EventSource = eventSource;
            ActingPersonId = actingPersonId;
            AffectedPersonId = affectedPersonId;
            OrganizationId = organizationId;
            GeographyId = geographyId;
            ParameterInt = parameterInt;
            ParameterText = parameterText;
        }


        public DateTime DateTime { get; private set; }
        public DateTime ProcessedDateTime { get; protected set; }
        public int EventId { get; private set; }
        public bool Open { get; protected set; }
        public EventType EventType { get; private set; }
        public EventSource EventSource { get; private set; }
        public int ActingPersonId { get; private set; }
        public int AffectedPersonId { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public int ParameterInt { get; private set; }
        public string ParameterText { get; private set; }
    }
}