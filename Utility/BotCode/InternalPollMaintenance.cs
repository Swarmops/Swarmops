using System;
using System.Collections.Generic;
using Swarmops.Logic.Governance;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.BotCode
{
    public class InternalPollMaintenance
    {
        public static void Run()
        {
            MeetingElections polls = MeetingElections.GetAll();

            foreach (MeetingElection poll in polls)
            {
                Console.Write("Looking at poll #" + poll.Identity + "...");

                if (DateTime.Now > poll.VotingCloses)
                {
                    Console.Write(" after closedatetime " + poll.VotingCloses.ToString("yyyy-MM-dd HH:mm") + "...");

                    // Poll should be CLOSED, definitely CLOSED.

                    if (poll.VotingOpen)
                    {
                        Console.WriteLine(" CLOSING.");

                        poll.VotingOpen = false; // Closes poll
                        poll.Creator.SendNotice("Poll Closed: " + poll.Name, string.Empty, poll.OrganizationId);
                        Person.FromIdentity(1)
                            .SendNotice("Poll Closed: " + poll.Name, string.Empty, poll.OrganizationId);

                        ReportPollResults(poll, false);
                    }
                    else
                    {
                        Console.WriteLine(" no action.");
                    }
                }
                else if (DateTime.Now > poll.VotingOpens)
                {
                    try
                    {
                        // Poll should be OPEN.

                        Console.Write("after opendatetime " + poll.VotingOpens.ToString("yyyy-MM-dd HH:mm") + "...");

                        if (!poll.VotingOpen)
                        {
                            Console.WriteLine("OPENING for " + poll.Organization.Name + ":");

                            // Populate voters, then open for voting

                            Organizations orgTree = poll.Organization.GetTree();

                            Memberships memberships = Memberships.ForOrganizations(orgTree);

                            Dictionary<int, bool> dupeCheck = new Dictionary<int, bool>();

                            int count = 0;

                            foreach (Membership membership in memberships)
                            {
                                Console.Write("\r - voters: {0:D6}/{1:D6} (person #{2:D6})...", ++count,
                                    memberships.Count, membership.PersonId);

                                if (!membership.Active)
                                {
                                    continue; // paranoid programming
                                }

                                if (!membership.Organization.IsOrInherits(poll.Organization))
                                {
                                    continue; // more paranoid programming
                                }

                                if (!dupeCheck.ContainsKey(membership.PersonId))
                                {
                                    if (membership.Person.GeographyId == 0)
                                    {
                                        // It's a trap!

                                        Console.Write(" GEOGRAPHY ZERO, ignoring\r\n");
                                    }
                                    else
                                    {
                                        Geography geo = membership.Person.Geography;

                                        if (geo.Inherits(poll.GeographyId) || geo.Identity == poll.GeographyId)
                                        {
                                            poll.AddVoter(membership.Person);
                                            dupeCheck[membership.PersonId] = true;
                                        }
                                    }
                                }
                            }

                            Console.WriteLine(" done.");
                            Console.Write(" - opening poll...");

                            poll.VotingOpen = true;

                            Console.WriteLine(" done.");
                            Console.Write("Sending notices... ");

                            string body = "Electoral roll for poll \"" + poll.Name + "\" primed. " +
                                          dupeCheck.Count + " people can vote. The vote is now OPEN.";

                            string subject = "Poll Open: " + poll.Name;

                            poll.Creator.SendNotice(subject, body, poll.OrganizationId);
                            Person.FromIdentity(1).SendNotice(subject, body, poll.OrganizationId);

                            Console.WriteLine(" done.");
                        }
                        else
                        {
                            Console.WriteLine(" already open, no action.");

                            // Report intermediate results to owner

                            ReportPollResults(poll, true);
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());

                        throw exception;
                    }
                }
            }
        }


        private static void ReportPollResults(MeetingElection poll, bool stillOpen)
        {
            SchulzeProcessor processor = new SchulzeProcessor();

            processor.Candidates = poll.Candidates;
            processor.Votes = MeetingElectionVotes.ForInternalPoll(poll);

            processor.Process();

            MailRawElectionResults(poll, processor, stillOpen);
        }


        private static void MailRawElectionResults(MeetingElection poll, SchulzeProcessor processor, bool stillOpen)
        {
            MeetingElectionCandidates candidates = processor.FinalOrder;

            Dictionary<int, bool> dropoutLookup = new Dictionary<int, bool>();

            // TODO SOMETIME: implement and populate dropouts here

            string body = "These are " + (stillOpen ? "intermediate" : "THE FINAL") + " results for the poll \"" +
                          poll.Name +
                          "\". Note that the Dist field is purely informative and not part of the Schulze ranking.\r\n\r\n";

            body += "Rank Dist Candidate\r\n";

            for (int candidateIndex = 0; candidateIndex < candidates.Count; candidateIndex++)
            {
                int distance = processor.GetCandidateDistance(candidateIndex);
                body += String.Format("{0:D3}. {1} {2}{3}\r\n", candidateIndex + 1,
                    (distance > 0 ? distance.ToString("D4") : "----"), candidates[candidateIndex].Person.Canonical,
                    dropoutLookup.ContainsKey(candidates[candidateIndex].PersonId) ? " [hoppat av]" : string.Empty);
            }

            int[] recipients = {1, poll.CreatedByPersonId};
                // Send to Rick too while stabilizing functionality -- remove as soon as Rick is running in any poll

            string subject = (stillOpen ? "Intermediate" : "FINAL") + " results for poll - " + poll.Name;

            foreach (int personId in recipients)
            {
                Person.FromIdentity(personId).SendNotice(
                    subject, body, poll.OrganizationId);
            }
        }
    }
}