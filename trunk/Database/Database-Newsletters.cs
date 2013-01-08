using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class PirateDb
    {
        /// <summary>
        /// Gets all subscribed people for a certain newsletter.
        /// </summary>
        /// <param name="ReportId">The Report Id.</param>
        /// <returns>An int array of PersonId.</returns>
        public int[] GetSubscribersForNewsletterFeed (int newsletterFeedId)
        {
            System.Collections.Generic.Dictionary<int, bool> hash = new Dictionary<int, bool>();

            // First, we get the newsletter feed we're working with.

            BasicNewsletterFeed newsletterFeed = this.GetNewsletterFeed(newsletterFeedId);

            if (newsletterFeed.DefaultSubscribed)
            {
                // Since the default is to subscribe to this report for this organization, we
                // must first get all the PersonIds that are members of the associated org, and
                // set their subscription to true. We will later follow up with the individual
                // overrides to get the final list.

                // Note that the individual preferences may have people not in the organization,
                // and so the final list may include subscribers that are nonmembers.

                BasicOrganization[] orgTree = this.GetOrganizationTree(newsletterFeed.OrganizationId);

                List<int> organizationIds = new List<int>();

                foreach (BasicOrganization org in orgTree)
                {
                    organizationIds.Add(org.Identity);
                }

                int[] personIds = this.GetMembersForOrganizations(organizationIds.ToArray());

                foreach (int personId in personIds)
                {
                    hash[personId] = true;
                }
            }

            // Now, go over each individual preference for this newsletter.

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId,Subscribed FROM NewsletterSubscriptions WHERE NewsletterFeedId=" +
                        newsletterFeedId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int personId = reader.GetInt32(0);
                        bool subscribed = reader.GetBoolean(1);

                        if (personId > 0)
                        {
                            hash[personId] = subscribed;
                        }
                    }
                }
            }

            // Finally, assemble the result. We have a hash table where all the ints that are true should
            // go into the final result.

            List<int> result = new List<int>();

            foreach (int key in hash.Keys)
            {
                if (hash[key])
                {
                    result.Add(key);
                }
            }

            return result.ToArray();
        }


        /// <summary>
        /// Gets data about a specific newsletter.
        /// </summary>
        /// <param name="newsletterId">The newsletter id.</param>
        /// <returns>A Newsletter instance.</returns>
        public BasicNewsletterFeed GetNewsletterFeed (int newsletterFeedId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT OrganizationId,DefaultSubscribed,Name FROM NewsletterFeeds WHERE NewsletterFeedId=" + newsletterFeedId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int organizationId = reader.GetInt32(0);
                        bool defaultSubscribed = reader.GetBoolean(1);
                        string reportName = reader.GetString(2);

                        return new BasicNewsletterFeed(newsletterFeedId, organizationId, defaultSubscribed, reportName);
                    }
                    else
                    {
                        throw new ArgumentException("No such newsletter feed id: " + newsletterFeedId.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Returns all newsletterfeeds for organisation + those with org =0
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public BasicNewsletterFeed[] GetNewsletterFeedsForOrganization (int organizationId)
        {
            List<BasicNewsletterFeed> feeds = new List<BasicNewsletterFeed>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT NewsletterFeedId, Name, DefaultSubscribed FROM NewsletterFeeds WHERE OrganizationId =" +
                        organizationId.ToString() , connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int newsletterFeedId = reader.GetInt32(0);
                        string reportName = reader.GetString(1);
                        bool defaultSubscribed = reader.GetBoolean(2);
                        feeds.Add(new BasicNewsletterFeed(newsletterFeedId, organizationId, defaultSubscribed,
                                                          reportName));
                    }
                }
            }

            return feeds.ToArray();
        }

        public System.Collections.Generic.Dictionary<int, bool> GetNewsletterFeedsForSubscriber (int personId)
        {
            System.Collections.Generic.Dictionary<int, bool> subs =
                new System.Collections.Generic.Dictionary<int, bool>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT NewsletterFeedId, Subscribed FROM NewsletterSubscriptions WHERE PersonId=" +
                        personId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int newsletterFeedId = reader.GetInt32(0);
                        bool subscribed = reader.GetBoolean(1);
                        subs[newsletterFeedId] = subscribed;
                    }
                }
            }

            return subs;
        }

        public void SetNewsletterSubscription (int personId, int newsletterFeedId, bool subscribed)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPersonNewsletterSubscription", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "newsletterFeedId", newsletterFeedId);
                AddParameterWithName(command, "subscribed", subscribed);

                command.ExecuteNonQuery();
            }
        }

        public void DeletePersonNewsletterSubscriptions (int personId) // reverts to default
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("DeletePersonNewsletterSubscriptions", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);

                command.ExecuteNonQuery();
            }
        }
    }
}