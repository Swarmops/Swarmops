using System;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicPWLog
    {
        public BasicPWLog ( DateTime dateTimeUtc,
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
            get { return dateTimeUtc; }
            set { dateTimeUtc = value; }
        }

        public int ActingPersonId
        {
            get { return actingPersonId; }
            set { actingPersonId = value; }
        }

        public string AffectedItemType
        {
            get { return affectedItemType; }
            set { affectedItemType = value; }
        }

        public int AffectedItemId
        {
            get { return affectedItemId; }
            set { affectedItemId = value; }
        }

        public string ActionType
        {
            get { return actionType; }
            set { actionType = value; }
        }

        public string ActionDescription
        {
            get { return actionDescription; }
            set { actionDescription = value; }
        }

        public string ChangedField
        {
            get { return changedField; }
            set { changedField = value; }
        }

        public string ValueBefore
        {
            get { return valueBefore; }
            set { valueBefore = value; }
        }

        public string ValueAfter
        {
            get { return valueAfter; }
            set { valueAfter = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        private DateTime dateTimeUtc;
        private int actingPersonId;
        private string affectedItemType;
        private int affectedItemId;
        private string actionType;
        private string actionDescription;
        private string changedField;
        private string valueBefore;
        private string valueAfter;
        private string comment;
        private string ipAddress;

    }
}
