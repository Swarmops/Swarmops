using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;

namespace Activizr.Basic.Enums
{
    public enum ChurnDataType
    {
        Unknown = 0,
        /// <summary>
        /// The member churned (dropped out, left).
        /// </summary>
        Churn,
        /// <summary>
        /// The member remained in the organization (renewed).
        /// </summary>
        Retention
    }
}

namespace Activizr.Basic.Types
{
    public class BasicChurnDataPoint
    {
        public BasicChurnDataPoint (ChurnDataType dataType, DateTime decisionDateTime, DateTime expiryDate,
                                    int personId, int organizationId)
        {
            this.DataType = dataType;
            this.DecisionDateTime = decisionDateTime;
            this.ExpiryDate = expiryDate;
            this.PersonId = personId;
            this.OrganizationId = organizationId;
        }

        public BasicChurnDataPoint (BasicChurnDataPoint original) :
            this(
            original.DataType, original.DecisionDateTime, original.ExpiryDate, original.PersonId,
            original.OrganizationId)
        {
        }

        public readonly ChurnDataType DataType;
        public readonly DateTime DecisionDateTime;
        public readonly DateTime ExpiryDate;
        public readonly int OrganizationId;
        public readonly int PersonId;
    }
}