using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Database.Attributes;
using Swarmops.Logic.Communications;
using Swarmops.Logic.DashboardTasks;
using Swarmops.Logic.Security;
using Swarmops.Logic.Special.Mail;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    [Serializable]
    [DbRecordType ("Person")]
    public class Person : BasicPerson, IComparable, IHasIdentity
    {
        private readonly IHandleProvider handleProvider;
        private ObjectOptionalData _optionalData;
        private Dictionary<int, bool> subscriptions;

        protected Person (BasicPerson basic)
            : base (basic)
        {
            // Today, we set the Swedish handle provider. This can be changed as desired.

            this.handleProvider = new SwedishForumHandleProvider();
        }

        private ObjectOptionalData OptionalData
        {
            get
            {
                if (this._optionalData == null)
                {
                    Person p = this;
                    this._optionalData = ObjectOptionalData.ForObject (p);
                    //Added cast, otherwise it fails for subclasses
                }
                return this._optionalData;
            }
        }

        public string PostalCodeAndCity
        {
            get { return Country.Code + "-" + PostalCode + " " + base.CityName; }
        }

        public bool IsActivist
        {
            get { return SwarmDb.GetDatabaseForReading().GetActivistStatus (Identity); }
        }

        public new virtual string Name
        {
            get { return base.Name.Replace ("|", " "); }
            set { SwarmDb.GetDatabaseForWriting().SetPersonName (Identity, value); }
        }


        public new virtual string Street
        {
            get { return base.Street; }
            set { SwarmDb.GetDatabaseForWriting().SetPersonStreet (Identity, value); }
        }


        public new string PostalCode
        {
            get { return base.PostalCode; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetPersonPostalCode (Identity, value);
                base.PostalCode = value;
                ResolveGeography();
            }
        }


        public new string CityName
        {
            get { return base.CityName; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetPersonCity (Identity, value);
                base.CityName = value;
                ResolveGeography();
            }
        }


        public Country Country
        {
            get { return Country.FromIdentity (base.CountryId); }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetPersonCountry (Identity, value.Identity);
                base.CountryId = value.Identity;
                ResolveGeography();
            }
        }


        public virtual Geography Geography
        {
            get
            {
                if (GeographyId == 0)
                {
                    ResolveGeography();
                }

                return Geography.FromIdentity (GeographyId);
            }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetPersonGeography (Identity, value.Identity);
                base.GeographyId = value.Identity;
            }
        }

        public new int GeographyId
        {
            get
            {
                if (base.GeographyId == 0)
                {
                    ResolveGeography();
                }

                return base.GeographyId;
            }
        }


        public virtual string Mail
        {
            get { return base.Email; }
            set { SwarmDb.GetDatabaseForWriting().SetPersonEmail (Identity, value); }
        }


        public new virtual string Phone
        {
            get { return base.Phone; }
            set { SwarmDb.GetDatabaseForWriting().SetPersonPhone (Identity, value); }
        }


        public new virtual DateTime Birthdate
        {
            get { return base.Birthdate; }
            set { SwarmDb.GetDatabaseForWriting().SetPersonBirthdate (Identity, value); }
        }

        public new virtual PersonGender Gender
        {
            get { return base.Gender; }
            set { SwarmDb.GetDatabaseForWriting().SetPersonGender (Identity, value); }
        }


        // Optional data properties

        public virtual string PartyEmail
        {
            get
            {
                string email = OptionalData.GetOptionalDataString (ObjectOptionalDataType.PartyEmail);

                return email;
            }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.PartyEmail, value); }
        }


        public virtual string BankName
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BankName); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BankName, value); }
        }


        public virtual string BankClearing
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BankClearing); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BankClearing, value); }
        }


        public virtual string BankAccount
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BankAccount); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BankAccount, value); }
        }


        public virtual string CryptoPublicKey
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.CryptoPublicKey); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.CryptoPublicKey, value); }
        }

        public virtual string CryptoSecretKey
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.CryptoSecretKey); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.CryptoSecretKey, value); }
        }

        public virtual string CryptoFingerprint
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.CryptoFingerprint); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.CryptoFingerprint, value); }
        }

        public virtual string CryptoRevocationCertificate
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.CryptoRevocation); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.CryptoRevocation, value); }
        }

        [Obsolete ("Use NationalIdNumber instead")]
        public virtual string PersonalNumber
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.PersonalNumber); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.PersonalNumber, value); }
        }

        public virtual string NationalIdNumber
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.NationalIdNumber); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.NationalIdNumber, value); }
        }

        public virtual string Longitude
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.Longitude); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.Longitude, value); }
        }

        public virtual string Latitude
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.Latitude); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.Latitude, value); }
        }

        public virtual string BitIdAddress
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BitIdLoginAddress); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BitIdLoginAddress, value); }
        }


        public virtual bool MailUnreachable
        {
            get { return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.MailUnreachable); }
            set { OptionalData.SetOptionalDataBool (ObjectOptionalDataType.MailUnreachable, value); }
        }

        public virtual bool LimitMailToLatin1
        {
            get { return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.LimitMailToLatin1); }
            set { OptionalData.SetOptionalDataBool (ObjectOptionalDataType.LimitMailToLatin1, value); }
        }

        public virtual bool LimitMailToText
        {
            get { return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.LimitMailToText); }
            set { OptionalData.SetOptionalDataBool (ObjectOptionalDataType.LimitMailToText, value); }
        }

        public virtual bool NeverMail
        {
            get { return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.NeverMail); }
            set { OptionalData.SetOptionalDataBool (ObjectOptionalDataType.NeverMail, value); }
        }


        public virtual string Handle
        {
            get
            {
                if (this.handleProvider == null)
                {
                    throw new NotImplementedException ("No handle provider");
                }

                return this.handleProvider.GetPersonHandle (Identity);
            }
            set
            {
                if (this.handleProvider == null)
                {
                    throw new NotImplementedException ("No handle provider");
                }

                this.handleProvider.SetPersonHandle (Identity, value);
            }
        }

        public virtual int SwedishForumAccountId
        {
            get { return OptionalData.GetOptionalDataInt (ObjectOptionalDataType.ForumAccountId); }
            set { OptionalData.SetOptionalDataInt (ObjectOptionalDataType.ForumAccountId, value); }
        }

        public virtual string SwedishForumAccountName
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.ForumAccountName); }
        }


        //
        //
        public bool HasFinancialAccess // Replaced by Authority class
        {
            get
            {
                throw (new Exception ("Person.HasFinancialAccess was called; it is replaced by Authority.HasAccess(...)"));
            }
        }

        public string ResetPasswordTicket
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.ResetPasswordTicket); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.ResetPasswordTicket, value); }
        }

        public virtual bool EMailIsInvalid
        {
            get { return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.EMailIsInvalid); }
            set { OptionalData.SetOptionalDataBool (ObjectOptionalDataType.EMailIsInvalid, value); }
        }

        public new string PasswordHash
        {
            get { return base.PasswordHash; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetPersonPasswordHash (Identity, value);
                base.PasswordHash = value;
            }
        }

        public virtual string TempPasswordHash
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.PersonTempPasswordStore); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.PersonTempPasswordStore, value); }
        }

        public string Initials
        {
            get
            {
                string[] nameWords = Name.Split (' ');

                string result = string.Empty;
                foreach (string nameWord in nameWords)
                {
                    if (nameWord.Length > 0)
                    {
                        result += Char.ToUpperInvariant (nameWord[0]);
                    }
                }

                return result;
            }
        }

        public string PreferredCulture
        {
            get
            {
                if (!string.IsNullOrEmpty (OptionalData.GetOptionalDataString (ObjectOptionalDataType.PreferredCulture)))
                {
                    return OptionalData.GetOptionalDataString (ObjectOptionalDataType.PreferredCulture);
                }

                // make best guesses:

                if (base.CountryId != 0 && Country != null && !string.IsNullOrEmpty (Country.Culture))
                {
                    return Country.Culture;
                }

                //not set, make a guess...

                foreach (Membership ms in GetMemberships (false))
                {
                    try
                    {
                        return ms.Organization.DefaultCountry.Culture;
                    }
                    catch
                    {
                    }
                }

                return CultureInfo.InvariantCulture.Name;
            }

            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.PreferredCulture, value); }
        }

        public string Canonical // On the canonical format "Rick Falkvinge (#1)"
        {
            get { return Name + " (#" + Identity + ")"; }
        }

        public Image Portrait
        {
            get
            {
                Documents docs = Documents.ForObject (this);

                if (docs.Count == 0)
                {
                    return null;
                }

                return Image.FromFile (@"C:\Data\Uploads\PirateWeb\" + docs[0].ServerFileName);
            }
        }

        public string PortraitPhotographer
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.PortraitPhotographer); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.PortraitPhotographer, value); }
        }

        public string TShirtSize
            // ReSharper restore InconsistentNaming
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.TShirtSize); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.TShirtSize, value); }
        }

        public string BlogName
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BlogName); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BlogName, value); }
        }

        public string BlogUrl
        {
            get { return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BlogUrl); }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BlogUrl, value); }
        }

        public string Formal
        {
            get { return FirstName[0] + " " + LastName; }
        }

        public string FirstName
        {
            get
            {
                int indexLastSpace = Name.LastIndexOf (' ');
                if (indexLastSpace == -1)
                {
                    return Name;
                }

                return Name.Substring (0, indexLastSpace);
            }
        }

        public string LastName
        {
            get
            {
                int indexLastSpace = Name.LastIndexOf (' ');
                if (indexLastSpace == -1)
                {
                    return Name;
                }

                return Name.Substring (indexLastSpace + 1);
            }
        }

        protected string MailMD5
        {
            get
            {
                string email = Mail.Trim().ToLowerInvariant();

                string md5 = MD5.Hash (email);

                return md5.Replace (" ", "").ToLowerInvariant();
            }
        }

        public static int NobodyId
        {
            get { return 0; }
        }

        #region IComparable Members

        // TODO: Is same name really a match for equality for Person?  /JL
        public int CompareTo (object obj)
        {
            Person otherPerson = (Person) obj;

            return String.Compare (Name, otherPerson.Name);
        }

        #endregion

        protected int ResolveGeography()
        {
            if (base.CountryId == 0)
            {
                base.GeographyId = 1;
                SwarmDb.GetDatabaseForWriting().SetPersonGeography (Identity, 1);
                return 1; // root geography, if no country set
            }

            Cities cities = Cities.FromPostalCode (PostalCode, base.CountryId);
            City city = null;

            if (cities.Count == 0)
            {
                try
                {
                    city = City.FromName (CityName, CountryId);
                }
                catch (ArgumentException)
                {
                    // ignore
                }
            }
            else
            {
                city = cities[0]; // This may be adjusted manually, but will work in 99.9% of cases
            }

            if (city == null)
            {
                base.GeographyId = Country.FromIdentity (base.CountryId).GeographyId;
                return base.GeographyId;
            }

            base.GeographyId = city.GeographyId;
            SwarmDb.GetDatabaseForWriting().SetPersonGeography (Identity, base.GeographyId);
            return city.GeographyId;
        }

        public static Person FromBitIdAddress (string address)
        {
            People candidates = People.FromOptionalData (ObjectOptionalDataType.BitIdLoginAddress, address);

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            throw new ArgumentException ("Zero or several candidates");
        }

        public static Person FromIdentity (int personId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetPerson (personId));
        }

        public static Person FromIdentityAggressive (int personId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetPerson (personId));
            // Note "for writing". Intentional. Queries master db and bypasses replication lag.
        }

        public Authority GetAuthority()
        {
            return Authorization.GetPersonAuthority (Identity);
        }

        public Memberships GetMemberships (bool includeTerminated)
        {
            if (!includeTerminated)
            {
                return GetMemberships();
            }

            return Memberships.FromArray (SwarmDb.GetDatabaseForReading().GetMemberships (this));
        }

        public Memberships GetMemberships()
        {
            return
                Memberships.FromArray (SwarmDb.GetDatabaseForReading()
                    .GetMemberships (this, DatabaseCondition.ActiveTrue));
        }


        /// <summary>
        ///     Returns the most recent membership for an org wich is active or terminated within the grace period
        /// </summary>
        /// <param name="gracePeriod">For exired, number of days to add to allow it to be returned</param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Membership GetRecentMembership (int gracePeriod, int orgId)
        {
            List<int> orgIdList = new List<int>();
            orgIdList.Add (orgId);
            Memberships mss = GetRecentMemberships (orgIdList, gracePeriod);
            foreach (Membership ms in mss)
            {
                if (ms.OrganizationId == orgId)
                    return ms;
            }
            return null;
        }

        /// <summary>
        ///     Returns most recent membership for each org wich is active or terminated within the grace period
        /// </summary>
        /// <param name="gracePeriod">For exired, number of days to add to allow it to be returned</param>
        /// <returns></returns>
        public Memberships GetRecentMemberships (int gracePeriod)
        {
            List<int> orgIdList = new List<int>();
            return GetRecentMemberships (orgIdList, gracePeriod);
        }

        /// <summary>
        ///     Returns most recent membership for each org wich is active or terminated within the grace period
        /// </summary>
        /// <param name="orgs">List of ids. If empty, all orgs</param>
        /// <param name="gracePeriod">For exired, number of days to add to allow it to be returned</param>
        /// <returns></returns>
        public Memberships GetRecentMemberships (Organizations orgs, int gracePeriod)
        {
            List<int> orgIdList = new List<int> (orgs.Identities);
            return GetRecentMemberships (orgIdList, gracePeriod);
        }

        /// <summary>
        ///     Returns most recent membership for each org wich is active or terminated within the grace period
        /// </summary>
        /// <param name="orgs">List of ids. If empty, all orgs</param>
        /// <param name="gracePeriod">For exired, number of days to add to allow it to be returned</param>
        /// <returns></returns>
        public Memberships GetRecentMemberships (List<int> orgs, int gracePeriod)
        {
            Memberships memberships = GetMemberships (true);
            Dictionary<int, Membership> collectMembers = new Dictionary<int, Membership>();

            memberships.Sort (
                delegate (Membership ms1, Membership ms2) { return ms2.DateTerminated.CompareTo (ms1.DateTerminated); });

            //Keep one for each org, the active one or the one with the highest Terminationdate
            foreach (Membership membership in memberships)
            {
                if (orgs.Count == 0 || orgs.Contains (membership.OrganizationId))
                {
                    if (membership.Active)
                    {
                        collectMembers[membership.OrganizationId] = membership;
                    }
                    else if (membership.DateTerminated.AddDays (gracePeriod) > DateTime.Today)
                    {
                        if (!collectMembers.ContainsKey (membership.OrganizationId)
                            || collectMembers[membership.OrganizationId].Active == false)
                        {
                            collectMembers[membership.OrganizationId] = membership;
                        }
                    }
                }
            }

            Memberships collectedMS = new Memberships();
            collectedMS.AddRange (collectMembers.Values);

            if (collectedMS.Count > 0)
            {
                //sort to get most recent first
                collectedMS.Sort (delegate (Membership ms1, Membership ms2)
                {
                    if (ms1.Active && ms1.Active != ms2.Active)
                        return -1; // active before terminated
                    if (ms1.Active != ms2.Active)
                        return 1; // active before terminated
                    if (ms1.Active)
                        return ms1.Expires.CompareTo (ms2.Expires); // active with lowest expiry
                    return ms2.DateTerminated.CompareTo (ms1.DateTerminated); // terminated with latest terminationdate
                });
            }

            return collectedMS;
        }

        internal static Person FromBasic (BasicPerson basic)
        {
            return new Person (basic);
        }

        public static Person Create (string name, string email, string password, string phone, string street,
            string postal, string city, string countryCode, DateTime dateOfBirth,
            PersonGender gender)
        {
            BasicCountry country = null;

            if (countryCode.Length > 0)
            {
                country = Country.FromCode (countryCode);
            }

            // Clean data

            while (name.Contains ("  "))
            {
                name = name.Replace ("  ", " ");
            }

            name = name.Trim();
            email = email.ToLower().Trim();
            phone = LogicServices.CleanNumber (phone);
            postal = postal.Replace (" ", "").Trim();

            int personId = SwarmDb.GetDatabaseForWriting().CreatePerson (name, email, phone, street, postal, city,
                country == null ? 0 : country.Identity, dateOfBirth, gender);


            // Resolve the geography

            Person newPerson = FromIdentityAggressive (personId);
            // aggressive bypasses replication lag, avoids race condition
            newPerson.ResolveGeography();

            // Generate the salted password hash and set it

            if (password.Length > 0)
            {
                newPerson.PasswordHash = Authentication.GenerateNewPasswordHash (personId, password);
            }
            else
            {
                newPerson.PasswordHash = string.Empty; // if no password, prevent matching to anything
            }

            // Return the finished Person object

            return newPerson;
        }

        public Membership AddMembership (Organization organization, DateTime expires)
        {
            return AddMembership (organization.Identity, expires);
        }

        public Membership AddMembership (int organizationId, DateTime expires)
        {
            return Membership.Create (Identity, organizationId, expires);
        }

        private bool? HasExplicitSubscription (int newsletterFeedId)
        {
            if (null == this.subscriptions)
            {
                this.subscriptions = SwarmDb.GetDatabaseForReading().GetNewsletterFeedsForSubscriber (PersonId);
            }

            if (this.subscriptions.ContainsKey (newsletterFeedId))
            {
                return this.subscriptions[newsletterFeedId];
            }
            return null;
        }

        public bool IsSubscribing (int newsletterFeedId)
        {
            bool? subscription = HasExplicitSubscription (newsletterFeedId);
            if (subscription != null)
            {
                return (bool) subscription;
            }
            try
            {
                //Added try catch, throws exception on nonexisting feed id
                NewsletterFeed feed = NewsletterFeed.FromIdentity (newsletterFeedId);
                return feed.DefaultSubscribed;
            }
            catch
            {
                return false;
            }
        }

        public virtual void SetSubscription (int newsletterFeedId, bool subscribe)
        {
            bool? subscription = HasExplicitSubscription (newsletterFeedId);
            if (subscription != null && subscription != subscribe)
            {
                SwarmDb.GetDatabaseForWriting().SetNewsletterSubscription (PersonId, newsletterFeedId, subscribe);
            }
            else
            {
                NewsletterFeed feed = NewsletterFeed.FromIdentity (newsletterFeedId);
                if (subscribe != feed.DefaultSubscribed)
                {
                    SwarmDb.GetDatabaseForWriting().SetNewsletterSubscription (PersonId, newsletterFeedId, subscribe);
                }
            }
            this.subscriptions[newsletterFeedId] = subscribe;
        }


        public void DeleteSubscriptionData()
        {
            SwarmDb.GetDatabaseForWriting().DeletePersonNewsletterSubscriptions (Identity);
        }


        /// <summary>
        ///     Sends a phone text message to the person's mobile phone.
        /// </summary>
        public void SendPhoneMessage (string message)
        {
            // HACK: This currently only works for Swedish people.
            try
            {
                PhoneMessageTransmitter.Send (Phone, message);
                // SwarmDb.GetDatabase().LogTransmittedPhoneMessage(Identity, Phone, message);
            }
            catch (Exception ex)
            {
                throw new Exception (
                    "Failed to send phone message:[" + message + "]\r\nto person [#" + Identity + "]:\r\n" + ex.Message,
                    ex);
            }
        }


        public virtual void SetPassword (string newPassword)
        {
            string hash = Authentication.GenerateNewPasswordHash (Identity, newPassword);
            SwarmDb.GetDatabaseForWriting().SetPersonPasswordHash (Identity, hash);
            base.PasswordHash = hash;
        }


        private void SendNotice (string subject, string body, int organizationId, bool asOfficer)
        {
            int mailId = SwarmDb.GetDatabaseForWriting().CreateOutboundMail (
                asOfficer ? MailAuthorType.PirateWeb : MailAuthorType.Service, 0, subject,
                body, OutboundMail.PriorityNormal, 0, GeographyId, organizationId, DateTime.Now);
            SwarmDb.GetDatabaseForWriting()
                .CreateOutboundMailRecipient (mailId, Identity, asOfficer,
                    (int) OutboundMailRecipient.RecipientType.Person);
            SwarmDb.GetDatabaseForWriting().SetOutboundMailRecipientCount (mailId, 1);
            SwarmDb.GetDatabaseForWriting().SetOutboundMailResolved (mailId);
            SwarmDb.GetDatabaseForWriting().SetOutboundMailReadyForPickup (mailId);
        }

        public void SendNotice (string subject, string body, int organizationId)
        {
            //TODO: Comes in here with organizationId hardcoded as 1 from a lot of places
            SendNotice (subject, body, organizationId, false);
        }

        public void SendOfficerNotice (string subject, string body, int organizationId)
        {
            SendNotice (subject, body, organizationId, true);
        }


        public void SendOfficerNotice (TypedMailTemplate mailtempl, int organizationId)
        {
            SendNotice (mailtempl, organizationId, true);
        }

        public void SendNotice (TypedMailTemplate mailtempl, int organizationId)
        {
            SendNotice (mailtempl, organizationId, false);
        }

        public void SendNotice (TypedMailTemplate mailtempl, int organizationId, bool asOfficer)
        {
            OutboundMail mail = mailtempl.CreateFunctionalOutboundMail (
                asOfficer ? MailAuthorType.PirateWeb : MailAuthorType.Service,
                OutboundMail.PriorityNormal, Organization.FromIdentity (organizationId),
                Geography.Root, DateTime.Now);
            mail.AddRecipient (this, asOfficer);
            mail.SetRecipientCount (1);
            mail.SetResolved();
            mail.SetReadyForPickup();
        }


        public void CreateActivist (bool isPublic, bool isConfirmed)
        {
            SwarmDb.GetDatabaseForWriting().CreateActivist (Identity, isPublic, isConfirmed);
        }

        public void TerminateActivist()
        {
            SwarmDb.GetDatabaseForWriting().TerminateActivist (Identity);
        }

        public PersonRole AddRole (RoleType roleType, Organization organization, Geography geography)
        {
            return AddRole (roleType, organization.Identity, geography.Identity);
        }

        public PersonRole AddRole (RoleType roleType, int organizationId, int geographyId)
        {
            return PersonRole.Create (Identity, roleType, organizationId, geographyId);
        }


        public string CreatePPMailAddress()
        {
            // TODO: Handle different mail domains based on organisation of person.

            string name = Name.Trim();
            name = RemoveDiacritics (name);
            name = name.ToLower();
            name = name.Replace ("æ", "ae");
            //change all non valid chars to '.'
            Regex re = new Regex (@"[^-a-z0-9\.]", RegexOptions.IgnoreCase);
            name = re.Replace (name, ".");

            while (name.Contains (".."))
                name = name.Replace ("..", ".");

            return name + "@piratpartiet.se";
        }

        private static string RemoveDiacritics (string stIn)
        {
            // Doesn't account for æ, but strips everything else conceivable

            string stFormD = stIn.Normalize (NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory (stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append (stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize (NormalizationForm.FormC));
        }


        public string CreateUniquePPMailAddress()
        {
            string newAddress = CreatePPMailAddress();
            newAddress = MailServerDatabase.FindFreeAccount (newAddress);
            return newAddress;
        }

        public bool ValidatePassword (string oldpassword)
        {
            return Authentication.ValidatePassword (this, oldpassword);
        }

        public string HexIdentifier()
        {
            string identifier = String.Format ("{0:X4}", Identity);
            char[] array = identifier.ToCharArray();
            Array.Reverse (array);
            return new string (array);
        }

        public bool MemberOf (Organization organization)
        {
            return MemberOf (organization.Identity);
        }

        public bool MemberOf (int orgId)
        {
            Memberships memberships = GetMemberships();

            foreach (Membership membership in memberships)
            {
                if (membership.OrganizationId == orgId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool MemberOfWithInherited (int orgId)
        {
            Memberships memberships = GetMemberships();
            foreach (Membership membership in memberships)
            {
                if (membership.Organization.IsOrInherits (orgId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MemberOfWithInherited (Organization org)
        {
            return MemberOfWithInherited (org.Identity);
        }

        /* --- not used, commented out for usage of hardcoded org ids
        public int NationalPartyOrg (bool onlyIfMember)
        {
            //TODO: Implement a flag in Organization
            int[] partyOrgs = new int[] { Organization.PPSEid, Organization.PPDKid, Organization.PPFIid };
            foreach (int orgId in partyOrgs)
            {
                if (this.MemberOf(orgId))
                {
                    return orgId;
                }
            }

            //no, wasnt a member, try by country
            if (!onlyIfMember)
            {
                foreach (int orgId in partyOrgs)
                {
                    try
                    {
                        Organization org = Organization.FromIdentity(orgId);
                        if (this.Country.Code == org.DefaultCountry.Code)
                        {
                            return orgId;
                        }
                    }
                    catch { }
                }
            }
            //didn't find any
            return 0;

        } */

        // ReSharper disable InconsistentNaming

        public Tasks GetTasksForOrganization (Organization organization)
        {
            return Tasks.ForPersonOrganization (this, organization);
        }

        public string GetAvatarLink (int pixelSize)
        {
            return string.Format ("http://www.gravatar.com/avatar/{0}.jpg?s={1}&d=mm", MailMD5, pixelSize);
        }

        public string GetSecureAvatarLink (int pixelSize)
        {
            return GetAvatarLink (pixelSize).Replace ("http://www", "https://secure");
        }

        public bool HasAccess (Access access)
        {
            if (access == null)
            {
                return true;
            }

            // TEMPORARY access control lists

            // TODO: Build access control list from org chart, compare against required access

            string ids = string.Empty;

            if (access.Type == AccessType.Write)
            {
                ids = access.Organization.Parameters.TemporaryAccessListWrite;
            }
            else if (access.Type == AccessType.Read)
            {
                ids = access.Organization.Parameters.TemporaryAccessListWrite + " " +
                      access.Organization.Parameters.TemporaryAccessListRead; // Write access implies read access
            }


            List<string> resultingPeople = new List<string>();
            string[] idStrings = ids.Trim().Replace ("  ", " ").Split (' ');

            foreach (string idString in idStrings)
            {
                if (Int32.Parse (idString) == Identity)
                {
                    return true;
                }
            }

            return false;
        }
    }
}