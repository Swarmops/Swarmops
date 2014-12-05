using System;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicPWLog
    {
        private int actingPersonId;
        private string actionDescription;
        private string actionType;
        private int affectedItemId;
        private string affectedItemType;
        private string changedField;
        private string comment;
        private DateTime dateTimeUtc;
        private string ipAddress;
        private string valueAfter;
        private string valueBefore;

        public BasicPWLog (DateTime dateTimeUtc,
            int actingPersonId,
            string affectedItemType,
            int affectedItemId,
            string actionType,
            string actionDescription,
            string changedField,
            string valueBefore,
            string valueAfter,
            string comment,
            string ipAddress)
        {
            this.dateTimeUtc = dateTimeUtc;
            this.actingPersonId = actingPersonId;
            this.affectedItemType = affectedItemType;
            this.affectedItemId = affectedItemId;
            this.actionType = actionType;
            this.actionDescription = actionDescription;
            this.changedField = changedField;
            this.valueBefore = valueBefore;
            this.valueAfter = valueAfter;
            this.comment = comment;
            this.ipAddress = ipAddress;
        }


        public DateTime DateTimeUtc
        {
            get { return this.dateTimeUtc; }
            set { this.dateTimeUtc = value; }
        }

        public int ActingPersonId
        {
            get { return this.actingPersonId; }
            set { this.actingPersonId = value; }
        }

        public string AffectedItemType
        {
            get { return this.affectedItemType; }
            set { this.affectedItemType = value; }
        }

        public int AffectedItemId
        {
            get { return this.affectedItemId; }
            set { this.affectedItemId = value; }
        }

        public string ActionType
        {
            get { return this.actionType; }
            set { this.actionType = value; }
        }

        public string ActionDescription
        {
            get { return this.actionDescription; }
            set { this.actionDescription = value; }
        }

        public string ChangedField
        {
            get { return this.changedField; }
            set { this.changedField = value; }
        }

        public string ValueBefore
        {
            get { return this.valueBefore; }
            set { this.valueBefore = value; }
        }

        public string ValueAfter
        {
            get { return this.valueAfter; }
            set { this.valueAfter = value; }
        }

        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        public string IpAddress
        {
            get { return this.ipAddress; }
            set { this.ipAddress = value; }
        }
    }
}