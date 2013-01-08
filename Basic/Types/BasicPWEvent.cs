using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Basic.Types
{
    public class BasicPWEvent
    {
        public BasicPWEvent (int eventId, DateTime dateTime, bool open, DateTime processedDateTime, Enums.EventType eventType,
                           Enums.EventSource eventSource, int actingPersonId, int affectedPersonId, int organizationId,
                           int geographyId, int parameterInt, string parameterText)
        {
            this.EventId = eventId;
            this.DateTime = dateTime;
            this.Open = open;
            this.ProcessedDateTime = processedDateTime;
            this.EventType = eventType;
            this.EventSource = eventSource;
            this.ActingPersonId = actingPersonId;
            this.AffectedPersonId = affectedPersonId;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.ParameterInt = parameterInt;
            this.ParameterText = parameterText;
        }


        public DateTime DateTime { get; private set; }
        public DateTime ProcessedDateTime { get; protected set; }
        public int EventId { get; private set; }
        public bool Open { get; protected set; }
        public Enums.EventType EventType { get; private set; }
        public Enums.EventSource EventSource { get; private set; }
        public int ActingPersonId { get; private set; }
        public int AffectedPersonId { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public int ParameterInt { get; private set; }
        public string ParameterText { get; private set; }
    }
}