using System;
using System.Collections.Generic;
using Swarmops;
using Swarmops.Logic.Security.App_LocalResources;
using Swarmops.Logic.Structure;
using System.Globalization;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Security
{
    public class Authentication
    {
        private const int EmailVerificationTicketLength = 8;

        public class VerificationTicketTooOldException : Exception
        {
            internal VerificationTicketTooOldException (string msg) : base(msg)
            {
            }
        }

        public class VerificationTicketWrongException : Exception
        {
            internal VerificationTicketWrongException (string msg) : base(msg)
            {
            }
        }

        public class VerificationTicketLengthException : Exception
        {
            internal VerificationTicketLengthException (string msg) : base(msg)
            {
            }
        }

        /// <summary>
        /// Returns one authenticated user from login token and password.
        /// </summary>
        /// <param name="loginToken">The login token provided.</param>
        /// <param name="password">The password provided.</param>
        /// <returns>Exactly one authenticated user, if successful.</returns>
        /// <exception cref="UnauthorizedAccessException">This function will throw an UnauthorizedAccessException if the user cannot be authenticated using the supplied credentials.</exception>
        public static Person Authenticate (string loginToken, string password)
        {
            // Get the list of people that match the login token.

            People candidatePeople = GetPeopleByLoginToken(loginToken);
            Person authenticatedUser = null;

            // For every person in the list, test the supplied password against the current and legacy hash schemes.

            foreach (Person candidate in candidatePeople)
            {
                // Check that the candidate has at least one valid membership.

                bool hasActiveMemberships = false;

                Memberships memberships = candidate.GetMemberships();

                foreach (Membership membership in memberships)
                {
                    if (membership.Active)
                    {
                        hasActiveMemberships = true;
                        break;
                    }
                }

                // If no active memberships, do not authenticate against this candidate.

                if (!hasActiveMemberships)
                {
                    continue;
                }

                // Check the credentials.

                bool goodCredentials = false;

                if (candidate.PasswordHash == GeneratePasswordHash(candidate, password))
                {
                    goodCredentials = true;
                }
                else
                {
                    // If the most recent password hash mechanism fails, try legacy hashes

                    string[] legacyHashes = GenerateLegacyPasswordHashes(candidate, password);

                    foreach (string legacyHash in legacyHashes)
                    {
                        if (legacyHash == candidate.PasswordHash)
                        {
                            goodCredentials = true;
                        }
                    }
                }

                // Now we've iterated over the possible password hashes for the candidate. Were the credentials good?

                if (goodCredentials)
                {
                    // We have a set of good credentials. As a security mechanism, make sure that we haven't approved another
                    // user already using these credentials. In theory, the chances of this happening with SHA-1 hashes is less
                    // than astronomical, but anyway.

                    if (authenticatedUser != null)
                    {
                        // We have a double credentials hit. This becomes a serious security concern.
                        // TODO: Alert operator about this, it's a serious condition.

                        throw new UnauthorizedAccessException("Double credentials hit");
                    }

                    // The current candidate has good credentials:

                    authenticatedUser = candidate;
                }
            }

            // If a user came through as authenticated, return him/her. Otherwise, sod off.

            if (authenticatedUser != null)
            {
                return authenticatedUser;
            }

            throw new UnauthorizedAccessException();
        }


        public static string CreateRandomPassword (int length)
        {
            string allowedChars = "ABCDEFHJKMNPQRTUVWXYZ23456789";
            var chars = new char[length];
            var randomizer = new Random();

            for (int index = 0; index < length; index++)
            {
                chars[index] = allowedChars[randomizer.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Generates a SHA-1 password hash, salted with the person's unique id.
        /// </summary>
        /// <param name="person">The person to generate the hash for.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>A hash on the format "C8 5B A2 19 B5..." (29 characters).</returns>
        internal static string GeneratePasswordHash (Person person, string password)
        {
            if (person.PersonId == 0)
            {
                throw new ArgumentOutOfRangeException("PersonId on the supplied person cannot be 0");
            }

            return GeneratePasswordHash(person.PersonId, password);
        }

        internal static string GeneratePasswordHash (int personId, string password)
        {
            return BCrypt.HashPassword(password + personId.ToString(CultureInfo.InvariantCulture), BCrypt.GenerateSalt(12));
        }


        /// <summary>
        /// Generates password hashes for legacy passwords in system (that are no longer generated).
        /// </summary>
        /// <param name="person">The person to generate a hash for.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>A list of previously valid password hashes.</returns>
        private static string[] GenerateLegacyPasswordHashes (Person person, string password)
        {
            if (person.PersonalNumber.Length > 0)
            {
                return new[]
                {
                    SHA1.Hash(password + person.Identity + "Pirate"),
                    MD5.Hash(password + person.PersonalNumber + "Pirate")
                };
            }
            else
                return new[]
                {
                    SHA1.Hash(password + person.Identity + "Pirate")
                };
        }


        /// <summary>
        /// Get all person IDs that match a certain login token.
        /// </summary>
        /// <param name="loginToken">The login token to look for.</param>
        /// <returns>An array of person IDs.</returns>
        private static People GetPeopleByLoginToken (string loginToken)
        {
            People result = new People();

            SwarmDb database = SwarmDb.GetDatabaseForReading();

            // First, is the login token numeric? If so, add it as is and is a valid person.

            if (LogicServices.IsNumber(loginToken))
            {
                try
                {
                    int personId = Int32.Parse(loginToken);
                    Person person = Person.FromIdentity(personId);

                    // If we get here without exception, the login token is a valid person Id

                    result.Add(person);
                }
                catch (Exception)
                {
                    // Do nothing. In particular, do not add the person Id as a candidate.
                }
            }

            // Second, is the login token ten digits? If so, look for the person with this personal number.
            // This is specific to the Swedish PP.

            string cleanedNumber = LogicServices.CleanNumber(loginToken);

            if (cleanedNumber.Length == 10)
            {
                int[] personIds = database.GetObjectsByOptionalData(ObjectType.Person,
                                                                    ObjectOptionalDataType.PersonalNumber,
                                                                    cleanedNumber);

                foreach (int personId in personIds)
                {
                    result.Add(Person.FromIdentity(personId));
                }
            }

            // Third, look for a matching name. Expand the login token so that "R Falkv" will match "Rickard Falkvinge".
            // Only do this if the login token, excessive whitespace removed, is five characters or more.

            result = People.LogicalOr(result, People.FromNamePattern(loginToken));

            // Fourth, look for a matching email. Only do an exact match here.

            if (loginToken.Contains("@"))
            {
                result = People.LogicalOr(result, People.FromEmailPattern(loginToken));
            }

            return result;
        }

        public static Person RequestNewPasswordProcess (string eMail, string URL)
        {
            int personID = 0;
            int.TryParse(eMail, out personID);
            Person authenticatedUser = null;
            People candidatePeople = null;
            if (personID == 0)
            {
                BasicPerson[] people = SwarmDb.GetDatabaseForReading().GetPeopleFromEmailPattern(eMail.ToLower().Replace("%", "").Trim());
                candidatePeople = People.FromArray(people);

                // if multiple people share same e-mail, suppose the last one registered is the one to change.
                foreach (Person p in candidatePeople)
                {
                    if (authenticatedUser == null || authenticatedUser.PersonId < p.PersonId)
                    {
                        authenticatedUser = p;
                    }
                }
            }
            else
            {
                candidatePeople = People.FromIdentities(new int[] {personID});
                if (candidatePeople.Count > 0)
                {
                    authenticatedUser = candidatePeople[0];
                }
            }

            if (authenticatedUser == null)
            {
                return null;
            }

            string passwordTicket = CreateRandomPassword(EmailVerificationTicketLength);

            foreach (Person p in candidatePeople)
            {
                string encodedPasswordTicket = GeneratePasswordHash(p, passwordTicket);
                p.ResetPasswordTicket = encodedPasswordTicket + ";" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //TODO: Localize
            string mailbody = "";
            App_LocalResources.Authentication.Culture = CultureInfo.InvariantCulture;

            if (candidatePeople.Count == 1)
            {
                App_LocalResources.Authentication.Culture =
                    System.Globalization.CultureInfo.GetCultureInfo(authenticatedUser.PreferredCulture);

                mailbody = App_LocalResources.Authentication.RequestNewPassword_Mail_Preamble;
                mailbody += App_LocalResources.Authentication.RequestNewPassword_Mail_ClickOneLink;
                mailbody += "\r\n" + String.Format(URL, authenticatedUser.PersonId, passwordTicket);
            }
            else
            {
                string linksTot = "";
                foreach (Person p in candidatePeople)
                {
                    string links = "";
                    int membershipCount = 0;
                    if (App_LocalResources.Authentication.Culture == CultureInfo.InvariantCulture)
                    {
                        App_LocalResources.Authentication.Culture = CultureInfo.GetCultureInfo(p.PreferredCulture);
                    }

                    links += "\r\n\r\n";
                    links += "#" + p.PersonId.ToString();
                    links += " [Member of:";
                    Memberships msList = p.GetMemberships();
                    foreach (Membership ms in msList)
                    {
                        ++membershipCount;
                        links += " (" + ms.Organization.Name + "," + ms.MemberSince.ToString("yyyy-MM-dd") + ")";
                    }
                    links += "] ";
                    links += "\r\n" + String.Format(URL, p.PersonId, passwordTicket);
                    if (membershipCount > 0)
                    {
                        linksTot += links;
                    }
                }

                mailbody = App_LocalResources.Authentication.RequestNewPassword_Mail_Preamble;
                mailbody += App_LocalResources.Authentication.RequestNewPassword_Mail_ClickOneOfLinks;
                mailbody += "\r\n" + linksTot;
            }

            mailbody += App_LocalResources.Authentication.RequestNewPassword_Mail_Ending;

            authenticatedUser.SendNotice(App_LocalResources.Authentication.RequestNewPassword_Mail_Subject, mailbody, 1);
            return authenticatedUser;
        }


        public static void RequestMembershipConfirmation (Person p, string URL)
        {
            string passwordTicket = CreateRandomPassword(EmailVerificationTicketLength);

            string encodedPasswordTicket = GeneratePasswordHash(p.Identity, passwordTicket);
            p.ResetPasswordTicket = encodedPasswordTicket + ";" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string mailbody = "";

            App_LocalResources.Authentication.Culture =
                System.Globalization.CultureInfo.GetCultureInfo(p.PreferredCulture);

            mailbody = App_LocalResources.Authentication.MembershipConf_Mail_Preamble;
            mailbody += "\r\n" + String.Format(URL, p.PersonId, passwordTicket);

            mailbody += App_LocalResources.Authentication.MembershipConf_Mail_Ending;

            p.SendNotice(App_LocalResources.Authentication.MembershipConf_Mail_Subject, mailbody, 1);
        }

        public static void ValidateEmailVerificationTicket (Person pers, string token)
        {
            if (token.Length != EmailVerificationTicketLength)
            {
                throw new VerificationTicketLengthException("Wrong length of code, should be " +
                                                            EmailVerificationTicketLength.ToString() + " characters.");
            }
            string encodedPasswordTicket = GeneratePasswordHash(pers, token);
            string storedTicket = pers.ResetPasswordTicket;
            string[] storedParts = storedTicket.Split(new char[] {';'});
            if (storedParts.Length < 2)
            {
                throw new VerificationTicketWrongException("No such code exists.");
            }
            DateTime createdTime = DateTime.MinValue;
            DateTime.TryParseExact(storedParts[1].Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out createdTime);

            if (DateTime.Now.Subtract(createdTime).TotalHours > 10)
            {
                throw new VerificationTicketTooOldException(
                    "Verification code too old, it must be used within 10 hours.");
            }
            else if (encodedPasswordTicket.ToLower().Replace(" ", "") == storedParts[0].ToLower().Replace(" ", ""))
            {
                // Yes, proceed to next step
            }
            else
            {
                throw new VerificationTicketWrongException("Wrong verification code.");
            }
        }

        public static void SetPasswordByEmailVerificationTicket (Person pers, string token, string newPassword)
        {
            ValidateEmailVerificationTicket(pers, token);

            pers.SetPassword(newPassword);
            pers.ResetPasswordTicket = "";
        }

        internal static bool ValidatePassword (Person person, string oldpassword)
        {
            if (person.PasswordHash == GeneratePasswordHash(person, oldpassword))
            {
                return true;
            }
            else
            {
                // If the most recent password hash mechanism fails, try legacy hashes

                string[] legacyHashes = GenerateLegacyPasswordHashes(person, oldpassword);

                foreach (string legacyHash in legacyHashes)
                {
                    if (legacyHash == person.PasswordHash)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Person RequestActivistSignoffProcess (string eMail, string URL)
        {
            int personID = 0;
            int.TryParse(eMail, out personID);
            Person authenticatedUser = null;
            People candidatePeople = null;
            bool personIsActivist = false;
            if (personID == 0)
            {
                BasicPerson[] people = SwarmDb.GetDatabaseForReading().GetPeopleFromEmailPattern(eMail.ToLower().Replace("%", "").Trim());
                candidatePeople = People.FromArray(people);

                // if multiple people share same e-mail, suppose the last one registered is the one to change.
                foreach (Person p in candidatePeople)
                {
                    if (authenticatedUser == null || authenticatedUser.PersonId < p.PersonId && p.IsActivist)
                    {
                        authenticatedUser = p;
                    }
                }
            }
            else
            {
                candidatePeople = People.FromIdentities(new int[] {personID});
                if (candidatePeople.Count > 0)
                {
                    authenticatedUser = candidatePeople[0];
                }
            }

            if (authenticatedUser == null)
            {
                return null;
            }


            //TODO: Localize
            string mailbody = "";
            App_LocalResources.Authentication.Culture = System.Globalization.CultureInfo.InvariantCulture;


            if (candidatePeople.Count == 1 && candidatePeople[0].IsActivist)
            {
                personIsActivist = true;
                Person p = candidatePeople[0];
                if (App_LocalResources.Authentication.Culture == CultureInfo.InvariantCulture)
                {
                    App_LocalResources.Authentication.Culture = CultureInfo.GetCultureInfo(p.PreferredCulture);
                }


                string encodedPasswordTicket =
                    GeneratePasswordHash(p, p.Identity.ToString()).Replace(" ", "").Substring(0, 4) +
                    p.Identity.ToString();

                mailbody = App_LocalResources.Authentication.RequestActivistSignoff_Mail_Preamble;
                mailbody += App_LocalResources.Authentication.RequestActivistSignoff_Mail_ClickOneLink;


                mailbody += "\r\n" + String.Format(URL, encodedPasswordTicket);
            }
            else
            {
                string links = "";
                foreach (Person p in candidatePeople)
                {
                    Memberships msList = p.GetMemberships();
                    if (msList.Count == 0 && p.IsActivist)
                    {
                        personIsActivist = true;
                        if (App_LocalResources.Authentication.Culture == CultureInfo.InvariantCulture)
                        {
                            App_LocalResources.Authentication.Culture = CultureInfo.GetCultureInfo(p.PreferredCulture);
                        }


                        string encodedPasswordTicket =
                            GeneratePasswordHash(p, p.Identity.ToString()).Replace(" ", "").Substring(0, 4) +
                            p.Identity.ToString();
                        links += "\r\n\r\n";
                        links += "#" + p.PersonId.ToString();

                        links += "\r\n" + String.Format(URL, encodedPasswordTicket);
                    }
                }

                mailbody = App_LocalResources.Authentication.RequestActivistSignoff_Mail_Preamble;
                mailbody += App_LocalResources.Authentication.RequestActivistSignoff_Mail_ClickOneOfLinks;
                mailbody += links;
            }

            mailbody += App_LocalResources.Authentication.RequestActivistSignoff_Mail_Ending;

            if (personIsActivist)
            {
                authenticatedUser.SendNotice(App_LocalResources.Authentication.RequestActivistSignoff_Mail_Subject,
                                             mailbody, 1);
            }
            return authenticatedUser;
        }


        public static void ValidateRequestActivistSignoffProcess (Person p, string code)
        {
            string encodedPasswordTicket =
                GeneratePasswordHash(p, p.Identity.ToString()).Replace(" ", "").Substring(0, 4) + p.Identity.ToString();
            if (code != encodedPasswordTicket)
            {
                throw new VerificationTicketWrongException("No such code exists.");
            }
        }
    }
}