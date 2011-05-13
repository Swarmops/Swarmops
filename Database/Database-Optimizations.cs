using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;


// This file contains odd functions that are optimizations of very specific cases, that would otherwise
// require remodeling of the data model and/or very expensive database operations.

namespace Activizr.Database
{
    public partial class PirateDb
    {
        /// <summary>
        /// Optimization function.
        /// </summary>
        public Dictionary<int, bool> GetPeopleWhoDeclineLocalMail (int[] personIds)
        {
            Array.Sort(personIds);
            Dictionary<int, bool> result = new Dictionary<int, bool>();

            // Step 0 - build a lookup table of the personIds

            Dictionary<int, bool> lookup = new Dictionary<int, bool>();

            foreach (int personId in personIds)
            {
                lookup[personId] = true;
            }

            if (personIds.Length == 0)
            {
                return result; // if input empty, output is empty too
            }

            // Step 1 - find people who have declined just local mail. Get all, match against lookup.

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId FROM NewsletterSubscriptions WHERE NewsletterFeedId=2 AND Subscribed=0",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int personId = reader.GetInt32(0);

                        if (lookup.ContainsKey(personId))
                        {
                            result[personId] = false;
                        }
                    }
                }
            }

            // Step 2 - find people who have "NeverMail" set

            int[] neverMailPersonIds = this.GetObjectsByOptionalData(ObjectType.Person, ObjectOptionalDataType.NeverMail, "1");
            // BasicPerson[] neverMailPeople = this.GetPeopleFromOptionalData(PersonOptionalDataKey.NeverMail, "1");

            // For each of these people (they're not that many), if they are present in the personIds array, add them to the dictionary

            foreach (int neverMailPersonId in neverMailPersonIds)
            {
                if (lookup.ContainsKey(neverMailPersonId))
                {
                    result[neverMailPersonId] = false;
                }
            }

            return result;
        }



        public Dictionary<int,int> GetPeopleGeographies()
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId,GeographyId FROM People",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int personId = reader.GetInt32(0);
                        int geographyId = reader.GetInt32(1);

                        result[personId] = geographyId;
                    }
                }
            }

            return result;
        }


        public Dictionary<int,int> GetInternalPollVoteCountsPerGeography(int pollId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select count(*) AS VoteCount,VoteGeographyId from InternalPollVotes where InternalPollId=" + pollId.ToString() + " GROUP BY VoteGeographyId",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int voteCount = reader.GetInt32(0);
                        int geographyId = reader.GetInt32(1);

                        result[geographyId] = voteCount;
                    }
                }
            }

            return result;
        }

    }
}