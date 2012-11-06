using System;
using System.Collections.Generic;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    public class ChurnData : List<ChurnDataPoint>
    {
        public int RetentionCount
        {
            get { return Count - ChurnCount; }
        }

        public int ChurnCount
        {
            get
            {
                int count = 0;

                foreach (ChurnDataPoint dataPoint in this)
                {
                    if (dataPoint.DataType == ChurnDataType.Churn)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        internal static ChurnData FromArray (BasicChurnDataPoint[] basicArray)
        {
            var result = new ChurnData();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicChurnDataPoint basic in basicArray)
            {
                result.Add (ChurnDataPoint.FromBasic (basic));
            }

            return result;
        }


        /// <summary>
        /// Call this when one or more memberships have expired for a person.
        /// </summary>
        /// <param name="personId">The person churning.</param>
        /// <param name="organizationId">The organization churned from.</param>
        public static void LogChurn (int personId, int organizationId)
        {
            PirateDb.GetDatabaseForWriting().LogChurnData (personId, organizationId, true, DateTime.Now);
        }

        public static void LogRetention (int personId, int organizationId, DateTime expiry)
        {
            PirateDb.GetDatabaseForWriting().LogChurnData (personId, organizationId, false, expiry);
        }

        public static void LogRetention (int personId, int organizationId, DateTime expiry, DateTime decisionDateTime)
        {
            PirateDb.GetDatabaseForWriting().LogChurnData (personId, organizationId, false, expiry, decisionDateTime);
        }


        public static ChurnData ForOrganization (Organization organization)
        {
            return FromArray (PirateDb.GetDatabaseForReading().GetChurnData (organization));
        }

        public static ChurnData ForPerson (Person person)
        {
            return FromArray (PirateDb.GetDatabaseForReading().GetChurnData (person));
        }


        public static ChurnData GetByDate (Organization organization, DateTime date)
        {
            return GetByDate (organization, date, date);
        }

        public static ChurnData GetByDate (Organization organization, DateTime dateLower, DateTime dateUpper)
        {
            return FromArray (PirateDb.GetDatabaseForReading().GetChurnDataForOrganization (organization, dateLower, dateUpper));
        }


        public int GetRetentionDecisionRangeCount (int minDays, int maxDays)
        {
            int count = 0;

            if (minDays > maxDays)
            {
                // If misplaced, swap parameters, using count as temporary

                count = minDays;
                minDays = maxDays;
                maxDays = count;
                count = 0;
            }

            foreach (ChurnDataPoint dataPoint in this)
            {
                int dayCount = (dataPoint.ExpiryDate - dataPoint.DecisionDateTime).Days;

                if (dayCount >= minDays && dayCount <= maxDays)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetChurnCount()
        {
            return ChurnCount;
        }
    }
}