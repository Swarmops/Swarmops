using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Web.Mail;
using Swarmops.Basic.Enums;
using Swarmops.Basic;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Utility;
using Swarmops.Utility.Server;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Governance;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Support;
using Swarmops.Utility.BotCode;
using Swarmops.Utility.Mail;
using Swarmops.Utility.Special.Sweden;

/*
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
*/

namespace Swarmops
{
    /// <summary>
    /// This class is solely used for one-off operations, like tests or one-time imports, that run
	/// in console mode.
    /// </summary>
    public class BotConsole
    {
        public BotConsole ()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [STAThread]
        static void Main (string[] args)
        {
            // PWEvents.CreateEvent(EventSource.PirateBot, EventType.ParleyCancelled, 1, 1, 1, 1, 1, string.Empty);
            // PWEvents.CreateEvent(EventSource.PirateBot, EventType.ParleyCancelled, 1, 1, 1, 1, 4, string.Empty);
            // PWEvents.CreateEvent(EventSource.PirateBot, EventType.ParleyCancelled, 1, 1, 1, 1, 5, string.Empty);

            // Utility.BotCode.EventProcessor.Run();

            OutboundComm.CreateNotification(null, NotificationResource.System_Startup);

			Console.WriteLine(string.Empty);
			Console.Write("Waiting for mail queue to flush... ");

			Console.WriteLine("done. Press Enter.");

			Console.ReadLine();
		}

        static private void PopulatePrimariesSimulationVoters()
        {
            MeetingElection poll = MeetingElection.Primaries2010Simulation;

            Roles allRoles = Roles.GetAll();
            Dictionary<int, bool> dupeCheck = new Dictionary<int, bool>();

            // Add all leads and vice leads under PPSE, and all females in an org role

            dupeCheck[2380] = true; // prevent this from adding

            foreach (PersonRole role in allRoles)
            {
                bool add = false;

                if (!dupeCheck.ContainsKey(role.PersonId))
                {
                    add = true;
                }

                if (add)
                {
                    dupeCheck[role.PersonId] = true;

                    poll.AddVoter(role.Person);
                    Console.WriteLine(role.Person);
                }
            }
        }


        static private void PopulatePrimariesSimulation()
        {
            MeetingElection poll = MeetingElection.Primaries2010Simulation;

            Roles allRoles = Roles.GetAll();
            Dictionary<int, bool> dupeCheck = new Dictionary<int, bool>();

            int maleCount = 0;
            int femaleCount = 0;

            // Add all leads and vice leads under PPSE, and all females in an org role

            dupeCheck[2380] = true; // prevent this from adding

            foreach (PersonRole role in allRoles)
            {
                bool add = false;

                if (!dupeCheck.ContainsKey(role.PersonId))
                {
                    if (RoleTypes.ClassOfRole(role.Type) == RoleClass.Organization && role.Type != RoleType.OrganizationAdmin)
                    {
                        if (role.Person.IsFemale)
                        {
                            add = true;
                        }
                    }

                    if (role.Type == RoleType.LocalLead || role.Type == RoleType.LocalDeputy)
                    {
                        add = true;
                    }

                    if (!(role.Organization.Inherits(1) || role.OrganizationId == 1))
                    {
                        add = false;
                    }

                    if (role.Type == RoleType.LocalAdmin)
                    {
                        add = false;
                    }
                }

                if (add)
                {
                    dupeCheck[role.PersonId] = true;

                    poll.AddCandidate(role.Person, "Candidacy Statement #" + role.PersonId);
                    Console.WriteLine("Adding " + role.Person.Canonical);

                    if (role.Person.IsMale)
                    {
                        maleCount++;
                    }
                    else
                    {
                        femaleCount++;
                    }
                }
            }

            // Add (fake) females

            random = new Random();

            while (femaleCount < maleCount)
            {
                string name = GetRandomFemaleName() + " " + GetRandomLastName();
                DateTime birthDate = RandomPerson().Birthdate;

                while (birthDate.Year > 1992 | birthDate.Year < 1920)
                {
                    birthDate = RandomPerson().Birthdate;
                }

                Person geographyTemplate = RandomPerson();

                Person newPerson = Person.Create(name, string.Empty, string.Empty, string.Empty, string.Empty,
                                                 geographyTemplate.PostalCode, geographyTemplate.CityName,
                                                 geographyTemplate.Country.Code, birthDate, PersonGender.Female);

                newPerson.PostalCode = geographyTemplate.PostalCode; // force geography resolution

                poll.AddCandidate(newPerson, "Candidacy Statement #" + newPerson.Identity);
                Console.WriteLine("Adding FAKE " + newPerson.Canonical);
                PWLog.Write(PWLogItem.Person, newPerson.Identity, PWLogAction.PersonCreated, "Created for primaries simulation", "This is a fake person.");

                femaleCount++;
            }
        }

        private static Random random;

        static private Person RandomPerson()
        {
            Person result = null;

            while (result == null)
            {
                try
                {
                    result = Person.FromIdentity(random.Next(1, 30000));
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        static private Person RandomFemale()
        {
            Person female = Person.FromIdentity(1);

            while (female.IsMale)
            {
                female = RandomPerson();
            }

            return female;
        }

        static private string GetRandomFemaleName()
        {
            string[] names = RandomFemale().Name.Split(' ');

            while (names.Length != 2 || names[0].Length < 3)
            {
                names = RandomFemale().Name.Split(' ');
            }

            return Capitalize(names[0]);
        }

        static private string GetRandomLastName()
        {
            string[] names = RandomPerson().Name.Split(' ');

            while (names.Length != 2 || names[1].Length < 3)
            {
                names = RandomPerson().Name.Split(' ');
            }

            return Capitalize(names[1]);
        }

        static private string Capitalize(string input)
        {
            return Char.ToUpperInvariant(input[0]) + input.Substring(1).ToLower();
        }


        static private void TestUptakeGeographyAllocation()
        {
            Console.WriteLine("Checking auto-assign routines for cities:\r\n");
            Console.WriteLine("CityName -- assigned to org");

            List<int> list = new List<int>();
            list.Add(11);

            for (int geoId = 71; geoId <= 102; geoId++)
            {
                list.Add(geoId);
            }

            foreach (int geoId in list)
            {
                Geography geo = Geography.FromIdentity(geoId);
                Organization org = Organizations.GetMostLocalOrganization(geoId, 2);

                Console.WriteLine(geo.Name + " -- " + org.NameShort);
            }
        }


        static private void InviteUPNovember2008()
        {
            // Invite all PP members under 26 who are not yet also UP members.

            string bodyTemplate = string.Empty;

            using (StreamReader reader = new StreamReader("up-kampanj-2008-nov-11.txt", Encoding.Default))
            {
                bodyTemplate = reader.ReadToEnd();
            }


            People people = People.GetAll();

            foreach (Person person in people)
            {
                Console.Write(person.Name + "...");

                if (person.Birthdate.Year > 1983)
                {
                    Console.Write(" in range...");

                    bool upMember = false;
                    bool ppMember = false;

                    Memberships memberships = person.GetMemberships();

                    foreach (Membership membership in memberships)
                    {
                        if (membership.Organization.Inherits(2))
                        {
                            upMember = true;
                        }

                        if (membership.OrganizationId == 1)
                        {
                            ppMember = true;
                        }
                    }

                    if (!ppMember)
                    {
                        Console.WriteLine(" not a PP member anymore");
                        continue;
                    }

                    if (upMember)
                    {
                        Console.WriteLine(" already member");
                        continue;
                    }

                    Organization localYouthOrg = Organizations.GetMostLocalOrganization(person.GeographyId, 2);

                    Console.Write(" inviting to {0}... ", localYouthOrg.NameShort);

                    string link = String.Format("https://pirateweb.net/Pages/Public/SE/Member/JoinUPFromPP.aspx?PersonId={0}&OrgId={1}&Check={2}", person.Identity, localYouthOrg.Identity, person.PasswordHash.Substring(0, 5).ToLower().Replace(" ", "") + localYouthOrg.Identity.ToString());

                    string body = bodyTemplate.Replace("%link%", link);

                    person.SendNotice("Vad sägs om att gå med i Ung Pirat också?", body, 1);
                    Console.WriteLine (" done.");
                }
                else
                {
                    Console.WriteLine(" out of age range");
                }
            }

        }


        private static void GetUPMembershipsByDate(DateTime date)
        {
            using (TextWriter writer = new StreamWriter ("UP-2008-12-31.csv", false, Encoding.Default))
            {

            writer.WriteLine("Förening,Namn (medlemsnr),kön,födelsedatum,bidragsgrund,email,telefon");

            Organizations orgTree = Organization.FromIdentity(2).GetTree();

            foreach (Organization org in orgTree)
            {
                Memberships memberships = Memberships.ForOrganization (org, true);

                foreach (Membership membership in memberships)
                {
                    if (membership.MemberSince < date && (membership.Active || membership.DateTerminated > date))
                    {
                        // If we get here, the membership is valid for this organization

                        int age = date.Year - membership.Person.Birthdate.Year;

                        writer.WriteLine("{0},{1} (#{2}),{3},{4} ({5} år),{6},{7},'{8}'",
                            org.Name, membership.Person.Name, membership.Person.Identity, membership.Person.IsMale ? "Man" : "Kvinna",
                            membership.Person.Birthdate.ToString("yyyy-MM-dd"), age, age > 6 && age < 26? "Ja":"Nej", membership.Person.Email, membership.Person.Phone);
                    }
                }
            }
            }
        }


        private static void CheckNewsletterSubscribers()
        {
            People subscribers = People.FromNewsletterFeed(2);

            int total = subscribers.Count;
            int count = 0;

            foreach (Person person in subscribers)
            {
                count++;

                if (count % 100 == 0)
                {
                    Console.Write("\x0d{0}/{1}...", count, total);
                }

                bool stillMember = false;

                if (person.Identity == 1591)
                {
                    System.Diagnostics.Debugger.Break();
                }

                Memberships memberships = person.GetMemberships();

                foreach (Membership membership in memberships)
                {
                    if (membership.OrganizationId == 1 && membership.Active)
                    {
                        stillMember = true;
                    }
                }

                if (!stillMember)
                {
                    Console.WriteLine("\x0d{0} (#{1}) is not a member. Deleting.", person.Name, person.Identity);

                    /* person.SendNotice("Fel i databasen har gjort att du fått nyhetsbrev",
                        "Genom ett fel i vår databas har du fortsatt att få nyhetsbrev, trots att du inte längre är medlem. " +
                        "Vi ber om ursäkt för detta. Felet är nu åtgärdat. Du kommer inte att få fler nyhetsbrev.\r\n\r\n" +
                        "Hälsningar,\r\nRick", 1);

                     person.DeleteSubscriptionData(); */
                }
            }
        }


        private static void SendPhoneMessages()
        {
            /*
            Organizations orgs = Organizations.FromIdentities(new int[] { 1, 7 });
            Dictionary <int, bool> officerHash = new Dictionary<int,bool>();
            Geographies geos = Geography.FromIdentity(39).GetTree();
            RoleType[] roleTypes = new RoleType[] { RoleType.LocalLead, RoleType.LocalDeputy, RoleType.LocalAdmin, RoleType.LocalActive };

            foreach (Geography geo in geos)
            {
                foreach (Organization org in orgs)
                {
                    RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, org.Identity);

                    foreach (RoleType roleType in roleTypes)
                    {
                        foreach (Person person in officers[roleType])
                        {
                            officerHash[person.Identity] = true;

                            Console.WriteLine (person.Name + ", " + roleType.ToString() + ", " + geo.Name + ", " + org.Name);
                        }
                    }
                }
            }*/

            //Person.FromIdentity (1).SendPhoneMessage("Piratpartiet: Dagens flygbladsutdelning Stockholm kl 12 tyvärr INSTÄLLD. Vår flygbladsskrivare havererade. Kontakta andra du vet tänkt vara med och berätta.");


            //officer.SendPhoneMessage("Dagens flygbladsutdelning Stockholm kl 12 INSTÄLLD. Flygbladstryckeriet havererade. Kontakta andra du vet tänkt vara med och berätta.");


            int[] personIds = Roles.GetAllDownwardRoles(1, 39);

            /*
            People officers = People.FromIdentities(personIds);

            foreach (Person officer in officers)
            {
                Console.Write (officer.Name + ", " + officer.Phone + "... ");

                try
                {
                    officer.SendPhoneMessage("Piratpartiet: Dagens flygbladsutdelning Stockholm kl 12 tyvärr INSTÄLLD. Vår flygbladsskrivare havererade. Kontakta andra du vet tänkt vara med och berätta.");
                    Console.WriteLine ("ok");
                }
                catch (Exception e)
                {
                    Console.WriteLine (" FAIL");
                }
            }*/


            
        }

		private static void RunSingleTest()
		{
			//FindInvalidAddresses();

			//ValidatePoll (163, Geography.FromIdentity (32), new DateTime (2007, 10, 17));

			//PirateWeb.Utility.BotCode.MailProcessor.Run();

			//PirateWeb.Utility.BotCode.RosterHousekeeping.RemindAllExpiries();
			// SetBirthdates();

			//PirateWeb.Utility.BotCode.RosterHousekeeping.RemindAllExpiries();
			//PirateWeb.Utility.BotCode.RosterHousekeeping.SendReminderMail (Person.FromIdentity (1), new DateTime (2007, 4, 15));

			///PirateWeb.Utility.BotCode.RosterHousekeeping.ChurnExpiredMembers();

			//Person.FromIdentity (5).SendPhoneMessage ("Piratpartiet: Ditt medlemskap går strax ut. Svara på detta SMS med texten \"PP IGEN\" för att förnya (5 kr).");

			// C:\Internet Publishing\Webs\PirateWeb\Data\MembersGeographically.xml

			//SynchronizeMemberships();

			// PirateWeb.Utility.BotCode.RosterHousekeeping.SendReminderMail (Person.FromIdentity (437));

			//GeneratePdfEnvelopes();

			//SendNewsletter();

			// CheckUPMembers();

			/*
			GeographyStatistics stats = GeographyStatistics.GeneratePresent(new int[] { 1, 2 });

			using (TextWriter writer = new StreamWriter(@"\\sparrow\c$\Internet Publishing\Webs\PirateWeb\Data\MembersGeographically.xml", false, Encoding.Default))
			{
				writer.WriteLine(stats.ToXml());
				writer.Close();
			}*/



			// PirateWeb.Logic.Special.Mail.AddressValidator.Validate ("proust123ff@pirateöåäöad2");

			// MarkUnreachable();

			//PirateWeb.Utility.Special.Sweden.SwedishForumMemberCheck.Run();

			//CreateRickOnBlood();

			//CreateNewMailAccounts();

			//GenerateMembershipStats();

			//DotNetChartingCrack.EncodeFile4x("C:\\Devkeyfulldomain.lic", "dev.hq.glife.se");

			//Console.WriteLine(Deobfuscate ("\u0d72\u5a74\u1576\u1078\u157a\u527c", 12));

			//CleanPhoneNumbers();

            // GetSupportEmails();

            // PirateWeb.Logic.Support.PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.ReceivedMembershipPayment, 3907, 1, 0, 3907, 0, string.Empty);

            //PirateWeb.Utility.Special.Sweden.SupportDatabase.CloseDelayWarnings();

            NewsletterChecker.Run();
		}



        private static void CheckDuplicatePeople()
        {
            Console.Write("Loading all people...");
            People allPeople = People.GetAll();
            Console.WriteLine(" done.");

            int dupeTotals = 0;

            Dictionary<string, People> lookup = new Dictionary<string, People>();

            // First, build the lookup table

            Console.Write("Building lookup...");

            foreach (Person person in allPeople)
            {
                if (!person.Name.StartsWith("Deleted"))
                {
                    string key = person.Name.ToUpper() + person.Birthdate.ToString("yyyyMMdd");

                    if (!lookup.ContainsKey(key))
                    {
                        lookup[key] = People.FromSingle(person);
                    }
                    else
                    {
                        lookup[key].Add(person);
                    }
                }
            }

            Console.WriteLine(" done.");

            // Second, for each entry in the lookup table, identify those that are potential dupes

            foreach (string key in lookup.Keys)
            {
                People people = lookup[key];

                if (people.Count > 1)
                {
                    // This is a key collision and therefore a potential dupe. For now, just print its contents.

                    bool foundAtLeastOneMembership = false;
                    foreach (Person person in people)
                    {
                        bool activeMemberships = false;

                        Memberships memberships = person.GetMemberships();
                        foreach (Membership membership in memberships)
                        {
                            if (membership.Active)
                            {
                                activeMemberships = true;
                                foundAtLeastOneMembership = true;
                            }
                        }

                        if (activeMemberships)
                        {
                            dupeTotals++;
                        }
                    }

                    if (foundAtLeastOneMembership)
                    {
                        dupeTotals--;

                        // THERE ARE ACTIVE MEMBERSHIPS

                        string baseComparison = people[0].Email.ToLower() + people[0].Street.ToLower() + people [0].Phone;

                        for (int index = 1; index < people.Count; index++)
                        {
                            string comparison = people[index].Email.ToLower() + people[index].Street.ToLower() + people[index].Phone;


                        }

                        // VERIFY ADDRESS, MAIL, PHONE ETC IDENTICAL
                        // MOVE ALL TO FIRST
                        // MOVE ALL RESPONSIBILITIES TO FIRST
                        // DELETE ALL BUT LAST
                        // NOTIFY OWNER
                    }
                    else
                    {
                        // NO ACTIVE MEMBERSHIPS
                        // TODO: DELETE ALL BUT FIRST


                    }

                    Console.Write(people[0].Name + ": #" + people[0].Identity.ToString());

                    for (int index = 1; index < people.Count; index++)
                    {
                        Console.Write(", #" + people[index].Identity.ToString());
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Total probable dupes: " + dupeTotals.ToString());
        }


        private static void ManualReminders()
        {
            RosterHousekeeping.RemindAllExpiries();
        }

        private static void GetSupportEmails()
        {
            SupportMailReview.Run();
        }

        /*
		private static void GeneratePdfEnvelopes()
		{
			Organizations orgs = Organization.FromIdentity (2).GetTree();
			Country sweden = Country.FromCode ("SE");

			foreach (Organization org in orgs)
			{
				Memberships memberships = org.GetActiveMemberships();

				if (memberships.Count > 0)
				{
					Document document = new Document();
					Console.Write ("Generating for " + org.Name + " (" + memberships.Count + " members)...");

					foreach (Membership membership in memberships)
					{
						Person member = membership.Person;

						Page page = new Page(PageSize.EnvelopeC5, PageOrientation.Landscape);
						page.Elements.Add(new Label(member.Name, 300, 150, 300, 18, Font.HelveticaBold, 18));
						page.Elements.Add(new Label(member.Street, 300, 175, 300, 18, Font.Helvetica, 18));
						page.Elements.Add(new Label(member.PostalCodeAndCity, 300, 200, 300, 18, Font.Helvetica, 18));

						if (member.Country.Identity != sweden.Identity)
						{
							page.Elements.Add(new Label(member.Country.Name, 300, 225, 300, 18, Font.HelveticaBold, 18));
						}
						document.Pages.Add(page);
					}

					document.Draw("C:\\Temp\\" + org.Name + ".pdf");
					Console.WriteLine ("");
				}
			}


		}*/


		private static void SynchronizeMemberships()
		{
			People people = People.GetAll();

			//int[] membershipIds = new int[] {9686, 9687, 9688, 9691, 9692, 9693, 9694, 9695, 10825};

			foreach (Person person in people)
			{
				bool foundActive = false;
				bool foundMismatch = false;
				DateTime expiry = DateTime.MinValue;

				Memberships memberships = person.GetMemberships();

				foreach (Membership membership in memberships)
				{
					if (membership.Active)
					{
						if (!foundActive)
						{
							expiry = membership.Expires.Date;
							foundActive = true;
						}
						else
						{
							if (expiry.Date != membership.Expires.Date)
							{
								foundMismatch = true;
								if (expiry.Date < membership.Expires.Date)
								{
									expiry = membership.Expires.Date;
								}
							}
						}
					}
				}

				if (foundMismatch)
				{
					Console.WriteLine ("");
					Console.WriteLine ("Found mismatch for " + person.Name + ":");

					foreach (Membership membership in memberships)
					{
						Console.WriteLine (" - " + membership.Organization.Name + ": expires " + membership.Expires.ToShortDateString());
						if (expiry != membership.Expires)
						{
							membership.Expires = expiry;
							Console.WriteLine(" -- extending to " + expiry.ToShortDateString());
						}
					}

				}


			}
		}


		private static void ValidatePoll (int pollId, Geography geography, DateTime cutoffMembershipDate)
		{
			Dictionary<string, People> result = SwedishForumDatabase.GetDatabase().GetPollVotes (pollId);

			foreach (string pollAlternative in result.Keys)
			{
				People voters = result[pollAlternative];

				Console.WriteLine ("");
				Console.WriteLine ("Votes for " + pollAlternative + ":");

				foreach (Person voter in voters)
				{
					// Console.Write (voter.Name + "... ");
					Console.Write (voter.Geography.Name);

					if (voter.Geography.Inherits (geography))
					{
						Console.Write (" (ok)");
					}
					else
					{
						Console.Write (" (INVALID)");
					}

					Console.Write ("... member since " + voter.GetMemberships() [0].MemberSince.ToString("yyyy-MMM"));

					if (voter.GetMemberships() [0].MemberSince > cutoffMembershipDate)
					{
						Console.WriteLine (" (INVALID)");
					}
					else
					{
						Console.WriteLine (" (ok)");
					}
				}
			}
		}

        /*
		private static void FindInvalidAddresses()
		{
			People people = People.FromOrganizationAndGeography (1, 33);

			Logic.Special.Mail.AddressValidator validator = new AddressValidator();
			validationResults = new Dictionary<Person, AddressValidationResult>();
			ongoingValidations = new Dictionary<Person, string>();

			validator.AddressValidationComplete += ReceiveResult;

			foreach (Person person in people)
			{
				if (person.Email.Length > 1)
				{
					ongoingValidations[person] = person.Email;
					validator.BeginValidate (person.Email, person);
				}
			}

			int lastCompletion = 0;
			DateTime lastTime = DateTime.Now;
			int timeoutSeconds = 60;
			bool timedOut = false;


			while (!validator.IsActive)
			{
				Console.Write ("\rStarting Validator Engine...");
				System.Threading.Thread.Sleep(500);
			}

			while (validator.IsActive && !timedOut)
			{
				int currentCompletion = validationResults.Count;

				if (currentCompletion != lastCompletion)
				{
					lastCompletion = currentCompletion;
					lastTime = DateTime.Now;
				}
				else
				{
					if (lastTime.AddSeconds (timeoutSeconds) < DateTime.Now)
					{
						timedOut = true;
					}
				}

				Console.Write("\rCompleted: {0} / Active Sessions: {1}...   ", currentCompletion, validator.ActiveSessions);
				System.Threading.Thread.Sleep(500);
			}

			if (timedOut)
			{
				validator.Stop();

				while (validator.IsActive)
				{
					Console.Write("\rTimed out, killing validator. Active Sessions: {0}...   ", validator.ActiveSessions);
					System.Threading.Thread.Sleep(500);
				}
			}

			Console.WriteLine ("");

			using (TextWriter writer = new StreamWriter("C:\\Emailadresser-Norr.txt", false))
			{
				lock (validationResults)
				{
					foreach (Person person in validationResults.Keys)
					{
						if (validationResults[person] != AddressValidationResult.Valid && validationResults[person] != AddressValidationResult.Unknown)
						{
							string printLine =
								String.Format("{0}, {1}: {2} - {3}", person.Identity, person.Name, person.Email,
											   validationResults[person].ToString());

							writer.WriteLine(printLine);
							Console.WriteLine(printLine);
						}
					}
				}
			}
		}*/


        /*

    	static private Dictionary<Person, AddressValidationResult> validationResults;
    	private static Dictionary<Person, string> ongoingValidations;

        
		private static void ReceiveResult(object sender, Logic.Special.Mail.AddressValidationEventArgs args)
		{
         * THIS VALIDATOR HAS BEEN DECOMMISSIONED
         * 
         * 
			lock (validationResults)
			{
				validationResults[(Person)args.Cookie] = args.Result;
				ongoingValidations.Remove((Person)args.Cookie);
			}
		}*/


		private static void CheckUPMembers()
		{
			// For each suborg to UP,

			Organizations orgs = Organization.FromIdentity (2).GetTree();

			foreach (Organization org in orgs)
			{
				// For each membership in this suborg,

				Memberships memberships = org.GetMemberships();

				Console.WriteLine();
				Console.WriteLine(org.Name + ": " + memberships.Count + " members");
				Console.WriteLine ("----------------------------------------------------------");

				foreach (Membership membership in memberships)
				{
					// Get the person for this membership and verify that he/she is also a member of PP

					Person member = membership.Person;

					Memberships personMemberships = member.GetMemberships();

					bool ppMember = false;

					foreach (Membership testMembership in personMemberships)
					{
						if (testMembership.OrganizationId == 1)
						{
							ppMember = true;
						}
					}

					if (!ppMember)
					{
						Console.WriteLine ("[!!] " + member.Name);

						member.AddMembership (1, membership.Expires);
						Console.WriteLine ("      - added membership in PP");

						PWEvents.CreateEvent(EventSource.PirateBot, EventType.AddedMembership,
						member.Identity, 1, member.GeographyId, member.Identity, 0, string.Empty);

						Console.WriteLine("      - created event for PirateBot");
					}
					else
					{
						Console.WriteLine ("[ok] " + member.Name);
					}
				}
			}
		}


    	private static void GenerateMembershipStats()
		{
			string xml = MembershipEvents.LoadAll().ToXml();

			TextWriter writer = new StreamWriter("MembershipEvents.xml");
			writer.WriteLine(xml);
			writer.Close();
		}

		private static void CreateNewMailAccounts()
		{
			int[] officerIds = new int[] { 1323 }; // Roles.GetAllDownwardRoles(1, 1);

			foreach (int officerId in officerIds)
			{
				if (officerId == 1)
				{
					continue;
				}

				PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.EmailAccountRequested, 1, 1, Person.FromIdentity(officerId).GeographyId, officerId, 0, string.Empty);
			}
		}

		private static void CreateRickOnBlood()
		{
			PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.EmailAccountRequested, 1, 1, Person.FromIdentity(1).GeographyId, 1, 0, string.Empty);

			/*
			Person rick = Person.FromIdentity (1);
			Organization ppse = Organization.FromIdentity (1);

			string password = Logic.Formatting.GeneratePassword(8);

			Console.WriteLine("Creating AD account");
			PirateWeb.Utility.Server.ActiveDirectoryCode.AddUser(rick, ppse, password);
			Console.WriteLine("Adding to security group");
			ActiveDirectoryCode.AddUserToPirateSecurityGroup(rick, ppse);

			Console.WriteLine("Mail enabling");

			ExchangeCode.CreateMailbox(rick);

			Console.WriteLine("Adding mail addresses - secondary");

			ActiveDirectoryCode.AddEmailAddress(rick, "rf-1@internal.pirateweb.net", false);
			ActiveDirectoryCode.AddEmailAddress(rick, "rf-1@pirateweb.net", false);
			Console.WriteLine("Adding mail address - primary");

			ActiveDirectoryCode.AddEmailAddress(rick, "rick.falkvinge@piratpartiet.se", true);*/

		}


		private static void AddChurnData()
		{
			/*
			Memberships memberships = Memberships.FromOrganizations(Organization.FromIdentity(2).GetTree());
			Dictionary<int, bool> lookup = new Dictionary<int, bool>();

			foreach (Membership membership in memberships)
			{
				if (membership.Active)
				{
					// Find the main-party membership of this individual

					try
					{
						Membership mainMembership = Membership.FromPersonAndOrganization(membership.PersonId, 1);

						if (mainMembership.Active)
						{
							if (mainMembership.MemberSince >= new DateTime(2006, 04, 16))
							{
								if (membership.MemberSince.Date != mainMembership.MemberSince.Date)
								{
									if (!lookup.ContainsKey(membership.PersonId))
									{
										lookup[membership.PersonId] = true;

										ChurnData.LogRetention(membership.PersonId, 1, mainMembership.MemberSince.AddYears (1), membership.MemberSince);
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}*/
		}


		/* private scope */
		static string Deobfuscate (string A_0, int A_1)
		{
			char[] chArray = A_0.ToCharArray();
			int num = 0xe769766 + A_1;
			int index = 0;
			do
			{
				char ch1 = chArray[index];
				byte num4 = (byte)(((ch1 & '\x00ff') ^ num++) & 0xFF);
				byte num3 = (byte)(((ch1 >> 8) ^ num++) & 0xFF);
				chArray[index] = (char)((num4 << 8) | num3);
				index++;
			}
			while (index < chArray.Length);
			return string.Intern(new string(chArray));
		}

 


		private static void CleanPhoneNumbers()
		{
			int[] personIds = { 14, 230, 260, 311, 683, 704, 942, 1070, 1402, 1431, 1538, 1714, 1938, 2324, 2710, 3999, 4088, 4772, 5075, 5858, 6040, 6057, 6765, 8907, 8920, 9262, 9280, 9366, 9538, 9588, 9602, 9648, 9757, 9961 };

			foreach (int personId in personIds)
			{
				Person person = Person.FromIdentity(personId);

				person.Phone = Formatting.CleanNumber(person.Phone);
			}
		}


		private static void SaveTextFile (string contents, string fileName)
		{
			StreamWriter writer = new StreamWriter(fileName, false, Encoding.Default);
			writer.Write(contents);
			writer.Close();
		}

		private static void SetBirthdates()
		{
			/*
			Memberships memberships = Organization.FromIdentity(1).GetAllMemberships();

			int count = 0;
			foreach (Membership membership in memberships)
			{
				count++;
				Console.Write("{0:D4}/{1:D4}: ", count, memberships.Count);

				Person person = membership.Person;
				Console.Write(person.Name + "... ");

				if (person.Birthdate != DateTime.MinValue)
				{
					Console.WriteLine(person.Birthdate.ToString("yyyy-MMM-dd") + "/" + person.Gender.ToString() + " ok");
				}
				else
				{
					// If birthdate invalid, set birthdate and gender from personal number, if possible

					string personalNumber = person.PersonalNumber;

					try
					{
						Console.Write(personalNumber.Substring(0, 6) + "-" + personalNumber.Substring(6) + ": ");

						int genderDigit = Int32.Parse (personalNumber.Substring(8, 1));
						PersonGender gender = PersonGender.Unknown;

						if (genderDigit % 2 == 1)
						{
							gender = PersonGender.Male;
						}
						else
						{
							gender = PersonGender.Female;
						}

						DateTime birthdate = new DateTime(
							1900 + Int32.Parse(personalNumber.Substring(0, 2)),
							Int32.Parse(personalNumber.Substring(2, 2)),
							Int32.Parse(personalNumber.Substring(4, 2)));

						Console.WriteLine(birthdate.ToString("yyyy-MMM-dd") + " " + gender.ToString());

						person.Gender = gender;
						person.Birthdate = birthdate;
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL");
						//person.Birthdate = DateTime.MinValue;
						//person.Gender = PersonGender.Unknown;
					}

				}

			}*/
		}


		private static void RunForumCheck()
		{
			SwedishForumMemberCheck.Run();
		}

		private static void RunRepaidTest()
		{
			// PirateWeb.Basic.Types.BasicPWEvent basicEvent = new BasicPWEvent(0, EventType.ExpensesRepaidClosed, EventSource.PirateWeb, 1, 1, 1, 1, 0, "3,4,5,6,7,8,55,56,61");
			// PirateWeb.Utility.BotCode.EventProcessor.ProcessExpensesRepaid(basicEvent);
		}


		static void PopulateCountries()
		{
            /*
			SwarmDb database = SwarmDb.GetDatabase();

			using (StreamReader reader = new StreamReader("C:\\Countries.txt"))
			{

				string line = reader.ReadLine();

				while (line != null && line.Length > 3)
				{
					string code = line.Substring(1, 2).ToUpper();
					string name = line.Substring(7);

					database.CreateCountry(name, code);

					line = reader.ReadLine();
				}
			}*/
		}

		static void InviteManyNodesToUP(int[] nodeIds, int organizationId)
		{
			foreach (int nodeId in nodeIds)
			{
				// InviteOneNodeToUP(geographyId, organizationId);
			}
		}


		static void SendUPInvitations()
		{
			//SendOneUPInvitation ( (1), 7);
		}

        /*
		static void SendOneUPInvitation(Person person, int organizationId)
		{
			string body =
				"GÅ MED I UNG PIRAT!\r\n\r\n" +

				"Det kostar inget, och du behöver bara klicka på den här länken.\r\n\r\n" +

				"https://pirateweb.net/Pages/Public/SE/UP2006/Invitation.aspx?PersonId=" + person.PersonId.ToString() + "&OrgId=" + organizationId.ToString() + "&Check=" + person.PasswordHash.Substring(0, 5).Replace(" ", "") + organizationId.ToString() + "\r\n\r\n" +

				"Ung Pirat är Piratpartiets ungdomsförbund, som bildades formellt nu i helgen. Genom att bli medlem, så hjälper du piratrörelsen att växa och bli större.\r\n\r\n" +

				"Ja, mer än så, faktiskt. Bara genom att bli medlem, så gör du att piratrörelsen får mellan 500 och 1000 kronor i ungdomsstöd. Det är vad vi får per medlem och år. Utan att det kostar dig en krona. Det enda du behöver göra är att klicka på länken ovan.\r\n\r\n" +

				"Det är bland annat därför det är så viktigt att du går med. De pengarna räcker till 1000 affischer, 3000 foldrar eller 33000 valsedlar. Även om du inte vill engagera dig aktivt i Ung Pirat, så är det ett enkelt sätt att bidra väldigt mycket till rörelsen med ett enkelt klick.\r\n\r\n" +

				"Men det finns naturligtvis fler anledningar att gå med i Ung Pirat än att det ger bidrag. Under året som har gått så har vi sett hur både piratrörelsen och gårdagens industriintressen kraftigt har flyttat fram sina positioner, och den här konflikten kommer bara att bli intensivare i upphovsrättsindustrins försök att ta bort vår rätt till ett privatliv för att de ska kunna behålla gårdagens privilegier.\r\n\r\n" +

				"Tyvärr förstår politikerna fortfarande inte vad frågan handlar om. Det är därför piratrörelsen behövs. Mer än någonsin.\r\n\r\n" +

				"Ung Pirat är en viktig komponent i detta. Som Piratpartiets ungdomsförbund, så kommer Ung Pirat att rikta sig framför allt till yngre och utbilda tidigt i piratideologi: att stå upp för sina rättigheter, att kräva rätten till ett privatliv, och att kultur- och kunskapsspridning är något väldigt positivt. Det här är människor som känner instinktivt att de har rätt. Vi behöver bara tala om varför.\r\n\r\n" +

				"Piratpartiet har lyckats bra med att nå till människor ner till en viss ålder. Nu är det dags för Ung Pirat att ta vid och gå ner i åldrarna, ut på skolorna. Både högstadier, gymnasier och universitet. Där har vi en väldigt viktig uppgift att fylla.\r\n\r\n" +

				"Så jag hoppas att du väljer att gå med genom att klicka på länken ovan. Även som symbolhandling är det viktigt. Plus att även en sådan symbolhandling ger oss resurser att arbeta med.\r\n\r\n" +

				"Som tack för att du väljer att gå med, så kommer Ung Pirat att betala ditt medlemskap i Piratpartiet för 2007, och dessutom får du en snygg metallpin med Piratpartiets vackra svarta segel.\r\n\r\n" +

				"Jag hoppas att du väljer att gå med i Ung Pirat. Klicka bara på länken högst upp i det här mailet, som är personlig och bara för dig. Gör det nu.\r\n\r\n\r\n" +

				"God jul och gott nytt år!\r\n\r\n" +

				"Rick Falkvinge\r\n" +

				"Partiledare för Piratpartiet\r\n\r\n";


			new PirateWeb.Utility.Mail.MailTransmitter("Rick Falkvinge (Piratpartiet)", "rick.falkvinge@piratpartiet.se", "Piratpartiet: Inbjudan till Ung Pirat", body, new Person[] { person }, false).Send();

		}
        */
        /*
        static void TestNewReminder()
        {
            Person person = Person.FromIdentity(28662);
            Utility.BotCode.RosterHousekeeping.SendReminderMail(person);
        }*/

        /*
		static void TestMail()
		{
			string welcomeBody =
				"Hej, och tack för att du gått med i Piratpartiets ungdomsförbund - Ung Pirat!\r\n\r\n" +

				"Ung Pirat kommer att vara en bred organisation som anordnar evenemang och visar hur fri information och ett skyddat privatliv måste vara viktiga grundpelare i ett samhälle. Vi ska se till att våra frågor alltid finns på agendan!\r\n\r\n" +

				"Varje dag ser vi våra rättigheter tryckas bakåt. Under valrörelsen lyckades vi väcka debatt, och kapporna vändes friskt efter vinden när de etablerade partierna lovade att \"fildelarna\" skulle sluta jagas. Bara en månad efter valet konstaterar den nya justitieministern att lagen mot fildelning ska finnas kvar. När vi inte längre är väljare, väljer de att vända oss ryggen.\r\n\r\n" +

				"Därför behövs piratrörelsen, och Ung Pirat i synnerhet. Vi vet att vi behövs när svenska internetleverantörer tillämpar censur för att stoppa fildelning, och när studenter stängs ute från universitetsnät för att de delat på kultur. Vi vet att vi behövs när respekterade organisationer som Läkare Utan Gränser talar för döva öron när de kritiserar läkemedelspatenten. Vi vet att vi behövs när undersökningar visar att Sverige är näst sämst i EU på personlig integritet. Vi behövs, då de etablerade partierna inte förstår frågorna, och vi gör det.\r\n\r\n" +

				"Vår rörelse har på ett år gått från att vara ett rop på förändring med fokus på Sverige, till att vara en stark internationell rörelse med fokus på världen. Idag finns mer än 10 piratpartier i världen, med ännu fler under uppstart. Ung Pirat kommer att vara en organisation som jobbar över gränserna, tillsammans med likasinnade i resten av världen.\r\n\r\n" +

				"Du är en av de första medlemmarna i piratrörelsens nya ungdomsförening, och många fler skall vi bli innan nyår. Vi har satt målet att vara minst 1100 medlemmar innan året är slut.\r\n\r\n" +

				"Som vanligt i piratrörelsen är det DU som bestämmer vad vi ska göra i Ung Pirat. Rörelsen står och faller med ditt engagemang, så sök upp andra pirater och finn ett sätt att lansera Ung Pirat!\r\n\r\n" +

				"Som tack för att du gått med i Ung Pirat kommer vi skicka en snygg metallpin med Piratpartiets välkända svarta segel. Dessutom betalar vi ditt medlemskap i Piratpartiet för året 2007.\r\n\r\n" +

				"Hugi Ásgeirsson\r\n" +
				"Ordförande för Ung Pirat\r\n";

			new PirateWeb.Utility.Mail.MailTransmitter("Hugi Ásgeirsson (Ung Pirat)", "hugi.asgeirsson@piratpartiet.se", "Ung Pirat: Välkommen som medlem!", welcomeBody, new Person[] { new Person("Rick Falkvinge", "falkvingetest@hotmail.com") }, false).Send();

		}*/


		static void SendNewsletter()
		{
            /*
			SwarmDb database = SwarmDb.GetDatabase();

			int[] subscriberIds = database.GetSubscribersForNewsletterFeed(2);
			People subscribers = People.FromIdentities(subscriberIds);

			new NewsletterTransmitter2 (
				"Nyhetsbrev 25 april 2008",
				@"H:\Data\Piratpartiet\MailTemplates\PP-Newsletter\Template.html",
				@"H:\Data\Piratpartiet\MailTemplates\PP-Newsletter\Template.txt",
				subscribers).
					Send();
            return;*/
        }

        static void ImportReporters()
        {/*
            string fileName = @"C:\Documents and Settings\rick\Desktop\reportrar.txt";
            SwarmDb database = SwarmDb.GetDatabase();

            using (StreamReader reader = new StreamReader(fileName, Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (line.Length > 5)
                    {
                        string[] parts = line.Split(',');

                        string email = parts[0].Trim();
                        string name = parts[1].Trim();
                        string[] categories = parts[2].Trim().Split(' ');

                        int reporterId = database.CreateReporter(name, email);

                        foreach (string category in categories)
                        {
                            if (category.Trim().Length > 2)
                            {
                                database.CreateReporterMediaCategory(reporterId, category.Trim());
                            }
                        }
                    }
                }
            }
            */
        }

        /*
		static void InviteOneNodeToUP (int geographyId, int organizationId)
		{
			Database.LiveDatabase database = new Database.LiveDatabase((string)new System.Configuration.AppSettingsReader().GetValue("PirateWebConnectString", typeof(string)));

			Geography node = database.GetGeography (geographyId);
			Organization organization = database.GetOrganization(organizationId);

			Console.WriteLine(string.Empty);
			Console.WriteLine("------------------ NEW NODE: " + node.Name);
			Console.WriteLine("------------------ ORGANIZN: " + organization.Name);
			Console.WriteLine(string.Empty);
			Console.WriteLine("Press Enter");
			Console.ReadLine();
			Console.WriteLine(string.Empty);

			int[] subscriberIds = database.GetSubscribersForNewsletter(5);

			Dictionary<int, Geography> nodes = database.GetGeographyHashtable(geographyId);
			Person[] people = database.GetPeople(subscriberIds);

			foreach (Person person in people)
			{
				if (nodes.ContainsKey(person.NodeId))
				{
					Console.Write(person.Name + ": ");
					database.LoadPersonExtraData(person);

					int birthYear = 1900 + Int32.Parse(person.ExtraData[PersonExtraDataKey.PersonalNumber].Substring(0, 2));

					Console.Write(person.ExtraData[PersonExtraDataKey.PersonalNumber] + " - ");

					if (birthYear < 1907)
					{
						birthYear += 100;
					}

					int age = 2006 - birthYear;

					if (age < 26)
					{
						Console.Write(age.ToString() + " yrs, sending... ");
						SendOneUPInvitation(person, organizationId);
						Console.WriteLine("done");
					}
					else
					{
						Console.WriteLine(age.ToString() + " yrs, not inviting");
					}
				}
			}

		}*/

        private static void AuditUPMembers()
        {
            int[] personIds = new int[] { 10161, 10744, 5454, 936, 9865, 1341, 5069, 10717, 9909, 10339, 1961, 9775, 2234,
                2097, 9975, 598, 9904, 10109, 10475, 10375, 10253, 2013, 6445, 1131, 10581, 10505, 10631, 10692, 10458, 
                10652, 10399, 10174, 10681, 4024, 7827 };

            using (StreamWriter writer = new StreamWriter(@"C:\Ungpirat-revision.txt", false, Encoding.Default))
            {

                foreach (int personId in personIds)
                {
                    Person person = Person.FromIdentity(personId);
                    writer.WriteLine(person.Name + " (#" + person.Identity.ToString() + ")");

                    Memberships memberships = person.GetMemberships();

                    foreach (Membership membership in memberships)
                    {
                        if (membership.OrganizationId == 1)
                        {
                            continue;
                        }

                        writer.Write("- membership: " + membership.Organization.Name + ", from " + membership.MemberSince.ToString("yyyy-MM-dd") + " ");

                        if (membership.Active)
                        {
                            writer.WriteLine("(still active)");
                        }
                        else
                        {
                            writer.WriteLine("until " + membership.DateTerminated.ToString("yyyy-MM-dd"));
                        }
                    }

                    /*
                    ChurnData churnData = ChurnData.GetByPerson(personId);

                    foreach (ChurnDataPoint dataPoint in churnData)
                    {
                        if (dataPoint.OrganizationId == 1)
                        {
                            continue;
                        }

                        Console.Write("- churn data: " + dataPoint.DecisionDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ", " + Organization.FromIdentity(dataPoint.OrganizationId).Name + " ");

                        if (dataPoint.DataType == ChurnDataType.Churn)
                        {
                            Console.WriteLine("(churned)");
                        }
                        else
                        {
                            Console.WriteLine("(renewal)");
                        }
                    }*/

                    BasicPWEvent[] events = PWEvents.ForPerson(personId);

                    foreach (BasicPWEvent basicEvent in events)
                    {
                        if (basicEvent.OrganizationId == 1)
                        {
                            continue;
                        }

                        writer.WriteLine("- event: " + basicEvent.EventType.ToString() + " at " + basicEvent.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + " (" + basicEvent.EventSource.ToString() + ")");
                        if (basicEvent.EventSource == EventSource.PirateWeb)
                        {
                            writer.WriteLine("-- added manually in PirateWeb by " + Person.FromIdentity(basicEvent.ActingPersonId).Name + " (#" + basicEvent.ActingPersonId.ToString() + ")");
                        }

                        if (basicEvent.EventSource == EventSource.SignupPage && basicEvent.EventType == EventType.AddedMembership)
                        {
                            writer.WriteLine("-- initial seeding through offers from PP's org");
                        }

                        if (basicEvent.EventSource == EventSource.SignupPage && basicEvent.EventType == EventType.AddedMember)
                        {
                            string ip = basicEvent.ParameterText;

                            if (string.IsNullOrEmpty(ip))
                            {
                                DateTime utcTime = basicEvent.DateTime.ToUniversalTime();
                                writer.WriteLine("-- searching logs at UTC timestamp " + utcTime.ToString("yyyy-MM-dd HH:mm:ss") + " +/- 30 seconds:");
                                ip = WriteAuditServerLogLines(utcTime, writer);
                            }
                            else
                            {
                                writer.WriteLine("-- joined from IP " + ip);
                            }
                        }
                    }

                    writer.WriteLine();
                }
            }
        }

        private static string WriteAuditServerLogLines (DateTime utcTime, StreamWriter writer)
        {
            bool foundAny = false;

            string fileName = "c:\\temp\\logfiles\\W3SVC1\\ex" + utcTime.ToString("yyMMdd") + ".log";

            if (!File.Exists(fileName))
            {
                writer.WriteLine("-- no log file for requested date");
                return string.Empty;
            }

            string timeStart = utcTime.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss");
            string timeStop = utcTime.AddSeconds(30).ToString("yyyy-MM-dd HH:mm:ss");

            string result = string.Empty;

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = reader.ReadLine();

                while (line.StartsWith("#"))
                {
                    line = reader.ReadLine();
                }

                string timeStamp = line.Substring(0, 19);

                while (String.Compare (timeStamp, timeStart) < 1)
                {
                    line = reader.ReadLine();
                    timeStamp = line.Substring(0, 19);
                }

                while (String.Compare(timeStamp, timeStop) < 1)
                {
                    if (line.Contains("POST /Pages/Public/SE/NewMember/New.aspx"))
                    {
                        string ip = line.Split (' ')[2];

                        writer.WriteLine("-- signup page accessed at " + timeStamp + " by IP " + line.Split (' ')[2]);
                        if (result.Length < 2)
                        {
                            result = ip;
                        }
                        else if (result != ip)
                        {
                            result = "invalid";
                        }

                        foundAny = true;
                    }

                    line = reader.ReadLine();
                    timeStamp = line.Substring(0, 19);
                }
            }

            if (!foundAny)
            {
                writer.WriteLine("-- nothing found in log");
            }

            if (result == "invalid")
            {
                result = string.Empty;
            }

            return result;
        }


        private static void AddMembersToSystemLog()
        {
            People people = People.GetAll();

            foreach (Person person in people)
            {
                Console.WriteLine(person.Name + " (#" + person.Identity.ToString() + ")");

                Memberships memberships = person.GetMemberships();

                foreach (Membership membership in memberships)
                {
                    if (membership.OrganizationId == 1)
                    {
                        continue;
                    }

                    Console.Write("- membership: " + membership.Organization.Name + ", from " + membership.MemberSince.ToString("yyyy-MM-dd") + " ");

                    if (membership.Active)
                    {
                        Console.WriteLine("(still active)");
                    }
                    else
                    {
                        Console.WriteLine("until " + membership.DateTerminated.ToString("yyyy-MM-dd"));
                    }
                }

                /*
                ChurnData churnData = ChurnData.GetByPerson(personId);

                foreach (ChurnDataPoint dataPoint in churnData)
                {
                    if (dataPoint.OrganizationId == 1)
                    {
                        continue;
                    }

                    Console.Write("- churn data: " + dataPoint.DecisionDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ", " + Organization.FromIdentity(dataPoint.OrganizationId).Name + " ");

                    if (dataPoint.DataType == ChurnDataType.Churn)
                    {
                        Console.WriteLine("(churned)");
                    }
                    else
                    {
                        Console.WriteLine("(renewal)");
                    }
                }*/

                BasicPWEvent[] events = PWEvents.ForPerson(person.Identity);

                foreach (BasicPWEvent basicEvent in events)
                {
                    if (basicEvent.OrganizationId == 1)
                    {
                        continue;
                    }

                    Console.WriteLine("- event: " + basicEvent.EventType.ToString() + " at " + basicEvent.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + " (" + basicEvent.EventSource.ToString() + ")");
                    if (basicEvent.EventSource == EventSource.PirateWeb)
                    {
                        Console.WriteLine("-- added manually in PirateWeb by " + Person.FromIdentity(basicEvent.ActingPersonId).Name + " (#" + basicEvent.ActingPersonId.ToString() + ")");
                    }

                    if (basicEvent.EventSource == EventSource.SignupPage && basicEvent.EventType == EventType.AddedMembership)
                    {
                        Console.WriteLine("-- initial seeding through offers from PP's org");
                    }

                    if (basicEvent.EventSource == EventSource.SignupPage && basicEvent.EventType == EventType.AddedMember)
                    {
                        string ip = basicEvent.ParameterText;

                        if (string.IsNullOrEmpty(ip))
                        {
                            DateTime utcTime = basicEvent.DateTime.ToUniversalTime();
                            Console.WriteLine("-- searching logs at UTC timestamp " + utcTime.ToString("yyyy-MM-dd HH:mm:ss") + " +/- 30 seconds:");
                            ip = AnalyzeAuditServerLogLines(utcTime);
                        }
                        else
                        {
                            Console.WriteLine("-- joined from IP " + ip);
                        }
                    }
                }

                Console.WriteLine();
            }
            
        }

        private static string AnalyzeAuditServerLogLines(DateTime utcTime)
        {
            bool foundAny = false;

            string fileName = "c:\\temp\\logfiles\\W3SVC1\\ex" + utcTime.ToString("yyMMdd") + ".log";

            if (!File.Exists(fileName))
            {
                Console.WriteLine("-- no log file for requested date");
                return string.Empty;
            }

            string timeStart = utcTime.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss");
            string timeStop = utcTime.AddSeconds(30).ToString("yyyy-MM-dd HH:mm:ss");

            string result = string.Empty;

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = reader.ReadLine();

                while (line.StartsWith("#"))
                {
                    line = reader.ReadLine();
                }

                string timeStamp = line.Substring(0, 19);

                while (String.Compare(timeStamp, timeStart) < 1)
                {
                    line = reader.ReadLine();
                    timeStamp = line.Substring(0, 19);
                }


                while (String.Compare(timeStamp, timeStop) < 1)
                {
                    if (line.Contains("POST /Pages/Public/SE/NewMember/New.aspx"))
                    {
                        string ip = line.Split(' ')[2];

                        Console.WriteLine("-- signup page accessed at " + timeStamp + " by IP " + line.Split(' ')[2]);
                        if (result.Length < 2)
                        {
                            result = ip;
                        }
                        else if (result != ip)
                        {
                            result = "invalid";
                        }

                        foundAny = true;
                    }

                    line = reader.ReadLine();
                    timeStamp = line.Substring(0, 19);
                }
            }

            if (!foundAny)
            {
                Console.WriteLine("-- nothing found in log");
            }

            if (result == "invalid")
            {
                result = string.Empty;
            }

            return result;
        }


        private static void ImportOfficialCityDesignations()
        {
            using (StreamReader reader = new StreamReader("GeographyId_to_Kommunkod.txt"))
            {
                string line = reader.ReadLine();

                while (line != null && line.Length > 3)
                {
                    string[] parts = line.Split('\t');
                    int geoId = Int32.Parse(parts[0]);
                    string designation = parts[1].Trim();

                    SwarmDb.GetDatabaseForWriting().CreateGeographyOfficialDesignation(geoId, GeographyLevel.Municipality, 1, designation);
                    line = reader.ReadLine();
                }
            }
        }
    }
}
