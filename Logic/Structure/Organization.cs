using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Interfaces;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Structure
{
    public class Organization : BasicOrganization, ITreeNode, IComparable, ITreeNodeObject
    {
        #region Creation and Construction

        // ReSharper disable UnusedPrivateMember
        protected Organization()
            // ReSharper restore UnusedPrivateMember
        {
        } // disallow public direct construction

        private Organization (BasicOrganization basic)
            : base (basic)
        {
        }

        protected Organization (Organization org)
            : base (org)
        {
        }

        public static Organization FromIdentity (int identity)
        {
            return FromBasic (OrganizationCache.GetOrganization (identity));
            //return FromBasic(SwarmDb.GetDatabaseForReading().GetOrganization(identity));
        }

        public static Organization FromIdentityAggressive (int identity)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetOrganization (identity));
            // Note "for writing". Intentional. Queries master db; bypasses replication lag.
        }

        public static Organization FromBasic (BasicOrganization basic)
        {
            return new Organization (basic);
        }

        #endregion

        private ObjectOptionalData _optionalData;

        private Geography anchorGeography;
        private string mailPrefixInherited;

        #region class globals

        [Obsolete ("This should never be used anymore!", true)] public static readonly int RootIdentity = 5;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int PPSEid = 1;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int UPSEid = 2;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int PPFIid = 49;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int UPFIid = 95;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int PPNOid = 48;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int PPDKid = 82;
        [Obsolete ("This should never be used anymore!", true)] public static readonly int UPDKid = 83;

        public static Organization Sandbox
        {
            get { return FromIdentity (SandboxIdentity); }
        }

        public static int SandboxIdentity
        {
            get
            {
                if (FromIdentity (1).Name.StartsWith ("Sandbox"))
                {
                    return 1;
                }
                if (FromIdentity (3).Name.StartsWith ("Sandbox"))
                {
                    return 3;
                }

                throw new InvalidOperationException ("Cannot locate Sandbox organization's identity");
            }
        }


        [Obsolete ("This should never be used anymore!", true)]
        public static Organization PPSE
        {
            get { return FromIdentity (PPSEid); }
        }

        #endregion

        #region IComparable Members

        public int CompareTo (object obj)
        {
            Organization org2 = obj as Organization;

            if (org2 == null)
            {
                return 0; // undefined
            }

            if (org2.CatchAll && !CatchAll)
            {
                return -1;
            }

            if (!org2.CatchAll && CatchAll)
            {
                return 1;
            }

            // TODO: Read culture from organization's default country instead of hardcoding like this

            return String.Compare (Name, org2.Name, true, new CultureInfo ("sv-SE"));
        }

        #endregion

        #region ITreeNode Members

        public int ParentIdentity
        {
            get { return ParentOrganizationId; }
        }


        public int[] ChildrenIdentities
        {
            get { return Children.Identities; }
        }

        public ITreeNodeObject ParentObject
        {
            get { return Parent; }
        }

        public List<ITreeNodeObject> ChildObjects
        {
            get
            {
                List<ITreeNodeObject> retVal = new List<ITreeNodeObject>();
                foreach (Organization child in Children)
                    retVal.Add (child);
                return retVal;
            }
        }

        #endregion

        private ObjectOptionalData OptionalData
        {
            get
            {
                if (this._optionalData == null)
                {
                    Organization o = this;
                    this._optionalData = ObjectOptionalData.ForObject (o);
                    //Added cast, otherwise it fails for subclasses
                }
                return this._optionalData;
            }
        }

        public Organization Parent
        {
            get { return OrganizationCache.FromCache (ParentIdentity); }
        }

        public Organizations Children
        {
            get { return Organizations.FromArray (OrganizationCache.GetOrganizationChildren (Identity)); }
        }


        public OrganizationFinancialAccounts FinancialAccounts
        {
            get { return new OrganizationFinancialAccounts (Identity); }
        }

        public FinancialAccounts FinancialAccountsExternal
        {
            get
            {
                // HACK: MUST FETCH THIS FROM ACTUAL ACCOUNTS
                // HACK HACK HACK HACK HACK HACK

                if (PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE) && OrganizationId == 1)
                {
                    FinancialAccounts result = new FinancialAccounts();
                    result.Add (FinancialAccount.FromIdentity (1));
                    result.Add (FinancialAccount.FromIdentity (2));

                    return result;
                }
                if (PilotInstallationIds.IsPilot (PilotInstallationIds.SwarmopsLive) && OrganizationId == 7)
                {
                    FinancialAccounts result = new FinancialAccounts();
                    result.Add (FinancialAccount.FromIdentity (29));

                    return result;
                }

                throw new NotImplementedException();
            }
        }


        public OrganizationParameters Parameters
        {
            get { return new OrganizationParameters (this); }
        }


        public string IncomingPaymentTag
        {
            // HACK: REmove this property altogether - should be per account
            get { return "bg 451-0061 "; } // TODO: Per organization, of course
        }


        public People ValidatingPeople
        {
            get
            {
                People result = new People();

                // HACK: this gets the TEMPORARY list

                string peopleIdstring = Parameters.TemporaryAccessListWrite;
                string[] peopleIds = peopleIdstring.Split (' ');

                foreach (string peopleId in peopleIds)
                {
                    result.Add (Person.FromIdentity (Int32.Parse (peopleId)));
                }

                return result;
            }
        }

        public ParticipantTitle RegularLabel
        {
            get
            {
                string optionalData = OptionalData[ObjectOptionalDataType.OrgRegularLabel];
                if (string.IsNullOrEmpty (optionalData))
                {
                    RegularLabel = ParticipantTitle.Member;
                    return ParticipantTitle.Member; // Legacy
                }

                return (ParticipantTitle) (Enum.Parse (typeof (ParticipantTitle), optionalData));
            }
            set { OptionalData[ObjectOptionalDataType.OrgRegularLabel] = value.ToString(); }
        }

        public string ActivistLabel
        {
            get
            {
                string optionalData = OptionalData[ObjectOptionalDataType.OrgActivistLabel];
                if (string.IsNullOrEmpty (optionalData))
                {
                    ActivistLabel = "Activist";
                    return "Activist";
                }

                return optionalData;
            }
            set { OptionalData[ObjectOptionalDataType.OrgActivistLabel] = value; }
        }

        #region Public methods

        public bool IsEconomyEnabled
        {
            get { return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgEconomyEnabled); }
        }

        public int FirstFiscalYear
        {
            get
            {
                if (!IsEconomyEnabled)
                {
                    throw new InvalidOperationException (
                        "Cannot request first fiscal year; organization is not economy-enabled");
                }

                int year = Parameters.FirstFiscalYear;

                if (year < 2000)
                {
                    if (Identity == 1 && Name.StartsWith ("Piratpartiet"))
                    {
                        OptionalData.SetOptionalDataInt (ObjectOptionalDataType.OrgFirstFiscalYear, 2009);
                        year = 2009;
                    }
                    else if (Identity == 7 && Name.StartsWith ("Europirate Acad"))
                    {
                        OptionalData.SetOptionalDataInt (ObjectOptionalDataType.OrgFirstFiscalYear, 2012);
                        year = 2012;
                    }
                    else if (Identity == 1 && Name.StartsWith ("Sandbox"))
                    {
                        OptionalData.SetOptionalDataInt (ObjectOptionalDataType.OrgFirstFiscalYear, 2012);
                        year = 2012;
                    }
                }

                return year;
            }
        }

        public Currency Currency
        {
            get
            {
                if (!IsEconomyEnabled)
                {
                    throw new InvalidOperationException ("Cannot request currency; organization is not economy-enabled");
                }

                string currencyCode = OptionalData.GetOptionalDataString (ObjectOptionalDataType.OrgCurrency);

                if (string.IsNullOrEmpty (currencyCode))
                {
                    if (Identity == 1 && (Name.StartsWith ("Piratpartiet") || Name.StartsWith ("Sandbox")))
                    {
                        // This is a one-off to fix the v4 installation. Currency is SEK.
                        OptionalData.SetOptionalDataString (ObjectOptionalDataType.OrgCurrency, "SEK");
                        currencyCode = "SEK";
                    }
                    if (Identity == 2 && (Name.StartsWith ("European")))
                    {
                        OptionalData.SetOptionalDataString (ObjectOptionalDataType.OrgCurrency, "SEK");
                        currencyCode = "SEK";
                    }
                }

                return Currency.FromCode (currencyCode);
            }
        }

        public bool IsOrInherits (Organization prospectiveParent)
        {
            if (Identity == prospectiveParent.Identity)
                return true;
            return Inherits (prospectiveParent.Identity);
        }

        public bool Inherits (Organization prospectiveParent)
        {
            return Inherits (prospectiveParent.Identity);
        }

        public bool IsOrInherits (int prospectiveParentOrganizationId)
        {
            if (Identity == prospectiveParentOrganizationId)
                return true;
            return Inherits (prospectiveParentOrganizationId);
        }

        public bool Inherits (int prospectiveParentOrganizationId)
        {
            // Returns true if prospectiveParent is a parent of ours.

            Organizations line = GetLine();

            for (int index = 0; index < line.Count - 1; index++)
            {
                if (line[index].Identity == prospectiveParentOrganizationId)
                {
                    return true;
                }
            }

            return false;
        }

        public Organizations GetLine()
        {
            return Organizations.FromArray (OrganizationCache.GetOrganizationLine (Identity));

            //return Organizations.FromArray(SwarmDb.GetDatabaseForReading().GetOrganizationLine(Identity));
        }

        public Organizations GetTree()
        {
            return Organizations.FromArray (OrganizationCache.GetOrganizationTree (Identity));
            //return Organizations.FromArray(SwarmDb.GetDatabaseForReading().GetOrganizationTree(Identity));
        }

        public int GetMemberCount()
        {
            return Memberships.GetMemberCountForOrganization (this);
        }

        public NewsletterFeeds GetNewsletterFeeds()
        {
            return
                NewsletterFeeds.FromArray (SwarmDb.GetDatabaseForReading().GetNewsletterFeedsForOrganization (Identity));
        }


        public void EnableEconomy (Currency currency)
        {
            if (IsEconomyEnabled)
            {
                throw new InvalidOperationException ("Economy data already enabled");
            }

            // First, set hardwired accounts


            // TODO: Set names according to org default culture

            FinancialAccounts[OrganizationFinancialAccountType.AssetsBankAccountMain] =
                FinancialAccount.Create (this, "Bank Account", FinancialAccountType.Asset, null);
            FinancialAccounts[OrganizationFinancialAccountType.AssetsOutboundInvoices] =
                FinancialAccount.Create (this, "Outbound Invoices", FinancialAccountType.Asset, null);
            FinancialAccounts[OrganizationFinancialAccountType.AssetsOutstandingCashAdvances] =
                FinancialAccount.Create (this, "Cash Advances", FinancialAccountType.Asset, null);
            //FinancialAccounts[OrganizationFinancialAccountType.AssetsPaypal] =
            //    FinancialAccount.Create(this, "Paypal Account", FinancialAccountType.Asset, null);
            //FinancialAccounts[OrganizationFinancialAccountType.AssetsBitcoinHot] =
            //    FinancialAccount.Create(this, "Bitcoin Holdings (Hot)", FinancialAccountType.Asset, null);
            //FinancialAccounts[OrganizationFinancialAccountType.AssetsBitcoinCold] =
            //    FinancialAccount.Create(this, "Bitcoin Cold Storage", FinancialAccountType.Asset, null);
            //FinancialAccounts[OrganizationFinancialAccountType.AssetsVatInbound] =
            //    FinancialAccount.Create(this, "Inbound Value Added Tax", FinancialAccountType.Asset, null);
            FinancialAccounts[OrganizationFinancialAccountType.CostsAllocatedFunds] =
                FinancialAccount.Create (this, "Allocated funds", FinancialAccountType.Cost, null);
            FinancialAccounts[OrganizationFinancialAccountType.CostsBankFees] =
                FinancialAccount.Create (this, "Bank Fees", FinancialAccountType.Cost, null);
            FinancialAccounts[OrganizationFinancialAccountType.CostsInfrastructure] =
                FinancialAccount.Create (this, "ICT and Infrastructure", FinancialAccountType.Cost, null);
            FinancialAccounts[OrganizationFinancialAccountType.CostsYearlyResult] =
                FinancialAccount.Create (this, "Yearly result", FinancialAccountType.Cost, null);
            FinancialAccounts[OrganizationFinancialAccountType.DebtsEquity] =
                FinancialAccount.Create (this, "Equity", FinancialAccountType.Debt, null);
            FinancialAccounts[OrganizationFinancialAccountType.DebtsExpenseClaims] =
                FinancialAccount.Create (this, "Expense Claims", FinancialAccountType.Debt, null);
            FinancialAccounts[OrganizationFinancialAccountType.DebtsInboundInvoices] =
                FinancialAccount.Create (this, "Inbound Invoices", FinancialAccountType.Debt, null);
            FinancialAccounts[OrganizationFinancialAccountType.DebtsOther] =
                FinancialAccount.Create (this, "General Debt", FinancialAccountType.Debt, null);
            FinancialAccounts[OrganizationFinancialAccountType.DebtsSalary] =
                FinancialAccount.Create (this, "Salaries Due", FinancialAccountType.Debt, null);
            FinancialAccounts[OrganizationFinancialAccountType.DebtsTax] =
                FinancialAccount.Create (this, "Taxes Due", FinancialAccountType.Debt, null);
            //FinancialAccounts[OrganizationFinancialAccountType.DebtsVatOutbound] =
            //    FinancialAccount.Create(this, "Outbound Value Added Tax", FinancialAccountType.Debt, null);

            FinancialAccounts[OrganizationFinancialAccountType.IncomeDonations] =
                FinancialAccount.Create (this, "Donations", FinancialAccountType.Income, null);
            FinancialAccounts[OrganizationFinancialAccountType.IncomeSales] =
                FinancialAccount.Create (this, "Sales", FinancialAccountType.Income, null);


            // Then, create various cost accounts that are probably needed, or that could be used as a starting point

            FinancialAccount.Create (this, "Offices", FinancialAccountType.Cost, null);
            FinancialAccount.Create (this, "Unforeseen", FinancialAccountType.Cost, null);
            FinancialAccount.Create (this, "Staff", FinancialAccountType.Cost, null);
            FinancialAccount.Create (this, "Marketing and Campaigns", FinancialAccountType.Cost, null);
            FinancialAccount.Create (this, "Research and Development", FinancialAccountType.Cost, null);

            // Finally, create the first conference parent

            FinancialAccount conferenceBase = FinancialAccount.Create (this, "Conferences",
                FinancialAccountType.Cost, null);
            conferenceBase.IsConferenceParent = true;

            // Set the currency

            OptionalData.SetOptionalDataString (ObjectOptionalDataType.OrgCurrency, currency.Code);

            // Set current year to first fiscal year

            OptionalData.SetOptionalDataInt (ObjectOptionalDataType.OrgFirstFiscalYear, DateTime.Today.Year);

            // Finally, flag the org as enabled

            OptionalData.SetOptionalDataBool (ObjectOptionalDataType.OrgEconomyEnabled, true);
        }

        #endregion

        #region Public properties

        //protected new int DefaultCountryId
        //{
        //    get { return base.DefaultCountryId; }
        //}
        /// <summary>
        ///     The purpose of this property is to hide the base property from the Organization object.
        ///     DefaultCountry should be used instead.
        ///     Wich has been disabled because we need to be able to update it... /JL
        /// </summary>
        /// <summary>
        ///     Gets the default country for this organization.
        /// </summary>
        public Country DefaultCountry
        {
            get
            {
                if (DefaultCountryId == 0)
                {
                    return null;
                }
                return Country.FromIdentity (DefaultCountryId);
            }
        }

        /// <summary>
        ///     Gets true if this is a fallback organization allocation (like UP Piratnätet).
        /// </summary>
        public bool CatchAll
        {
            get
            {
                // Hardcoded for now, move to db when need arises

                if (NameShort == "UP Piratnätet")
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        ///     Gets the mail prefix for this organization. If there is none,
        ///     traverses the hierarchy upward until one is found.
        /// </summary>
        public string MailPrefixInherited // Cached
        {
            get
            {
                if (MailPrefix.Length > 0)
                {
                    return MailPrefix;
                }

                if (string.IsNullOrEmpty (this.mailPrefixInherited))
                {
                    Organizations orgLine = GetLine();

                    orgLine.Reverse();

                    foreach (Organization org in orgLine)
                    {
                        if (org.MailPrefix.Length > 0)
                        {
                            this.mailPrefixInherited = org.MailPrefix;
                            return this.mailPrefixInherited;
                        }
                    }
                }

                return this.mailPrefixInherited;
            }
        }


        public Geography PrimaryGeography
        {
            get
            {
                if (this.anchorGeography == null)
                {
                    this.anchorGeography = Geography.FromIdentity (AnchorGeographyId);
                }

                return this.anchorGeography;
            }
        }

        #endregion

        #region UptakeGeographies

        private List<Geography> uptakeGeographies;

        public ReadOnlyCollection<Geography> UptakeGeographies
        {
            get
            {
                if (this.uptakeGeographies == null)
                {
                    this.uptakeGeographies =
                        Geographies.FromIdentities (
                            SwarmDb.GetDatabaseForReading().GetOrganizationUptakeGeographyIds (Identity));
                }

                return this.uptakeGeographies.AsReadOnly();
            }
        }

        public void AddUptakeGeography (Geography geo)
        {
            AddUptakeGeography (geo.Identity);
        }

        public void DeleteUptakeGeography (Geography geo)
        {
            DeleteUptakeGeography (geo.Identity);
        }

        public void AddUptakeGeography (int geoId)
        {
            OrganizationCache.AddOrgUptakeGeography (Identity, geoId);
        }

        public void DeleteUptakeGeography (int geoId)
        {
            OrganizationCache.DeleteOrgUptakeGeography (Identity, geoId);
        }

        public UptakeGeography[] GetUptakeGeographies (bool others)
        {
            BasicUptakeGeography[] basics = SwarmDb.GetDatabaseForReading()
                .GetOrganizationUptakeGeographies (Identity, others);
            List<UptakeGeography> retVal = new List<UptakeGeography>();
            foreach (BasicUptakeGeography b in basics)
            {
                try
                {
                    UptakeGeography ug = UptakeGeography.FromBasic (b);
                    if (ug.Organization != null && ug.Geography != null && ug.Organization.ParentIdentity != -1)
                        retVal.Add (UptakeGeography.FromBasic (b));
                }
                catch
                {
                    //catch bad references
                }
            }

            if (others)
            {
                throw new NotImplementedException ("Apollo 13. Contact the devs with the Apollo 13.");

                // When is this code used anyway? One organization setting its geography uptake structure should NEVER interfere with
                // other organizations' uptake trees ("Organization.RootIdentity"). There is no reason whatsoever to get all
                // organizations at any time when determining uptake. We set an exception here to catch this logic in action.

                /*
                Dictionary<int, BasicOrganization> allOrgs = OrganizationCache.GetOrganizationHashtable(Organization.RootIdentity);
                foreach (BasicOrganization bo in allOrgs.Values)
                {
                    if (bo.Identity > 0)
                    {
                        BasicUptakeGeography myAnchor = new BasicUptakeGeography(bo.Identity, bo.AnchorGeographyId);
                        retVal.Add(UptakeGeography.FromBasic(myAnchor));
                    }
                }*/
            }
            BasicUptakeGeography myAnchor = new BasicUptakeGeography (Identity, AnchorGeographyId);
            if (retVal.Find (delegate (UptakeGeography ug) { return ug.GeoId == AnchorGeographyId; }) == null)
                retVal.Add (UptakeGeography.FromBasic (myAnchor));

            return retVal.ToArray();
        }

        #endregion

        #region FunctionalMailAddresses

        private Dictionary<MailAuthorType, FunctionalMail.AddressItem> functionalMailDict;

        public FunctionalMail.AddressItem GetFunctionalMailAddressInh (MailAuthorType authorType)
        {
            LoadFunctionalMailAddress();

            if (this.functionalMailDict.ContainsKey (authorType))
                return this.functionalMailDict[authorType];
            if (ParentIdentity != 0)
                return Parent.GetFunctionalMailAddressInh (authorType);
            if (FunctionalMail.Address.ContainsKey (authorType))
                return FunctionalMail.Address[authorType]; // Default.
            return null;
        }

        public FunctionalMail.AddressItem GetFunctionalMailAddress (MailAuthorType authorType)
        {
            LoadFunctionalMailAddress();

            if (this.functionalMailDict.ContainsKey (authorType))
                return this.functionalMailDict[authorType];
            return null;
        }

        private void LoadFunctionalMailAddress()
        {
            if (this.functionalMailDict == null)
            {
                this.functionalMailDict = new Dictionary<MailAuthorType, FunctionalMail.AddressItem>();
                string funcMails = OptionalData.GetOptionalDataString (ObjectOptionalDataType.OrgFunctionalMail);
                string[] rows = funcMails.Replace ("\r", "\n")
                    .Replace ("\n\n", "\n")
                    .Split (new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

                Regex reSplitAddress = new Regex (@"^(?<Type>.+?):(?<Name>.+)[,;:]\s*(?<Address>\S+?@\S+?)$",
                    RegexOptions.IgnoreCase);
                foreach (string row in rows)
                {
                    Match match = reSplitAddress.Match (row);
                    try
                    {
                        MailAuthorType maType =
                            (MailAuthorType) Enum.Parse (typeof (MailAuthorType), match.Groups["Type"].Value);
                        string name = match.Groups["Name"].Value;
                        string address = match.Groups["Address"].Value;
                        this.functionalMailDict[maType] = new FunctionalMail.AddressItem (address, name);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void SetFunctionalMailAddress (MailAuthorType authorType, string name, string email)
        {
            if (string.IsNullOrEmpty (email.Trim()))
            {
                this.functionalMailDict.Remove (authorType);
            }
            else
            {
                this.functionalMailDict[authorType] = new FunctionalMail.AddressItem (email, name);
            }
            SaveFunctionalMailDict();
        }


        private void SaveFunctionalMailDict()
        {
            StringBuilder sb = new StringBuilder();

            foreach (MailAuthorType ma in this.functionalMailDict.Keys)
            {
                sb.AppendFormat ("{0}:{1};{2}\r\n", ma, this.functionalMailDict[ma].Name,
                    this.functionalMailDict[ma].Email);
            }
            OptionalData.SetOptionalDataString (ObjectOptionalDataType.OrgFunctionalMail, sb.ToString());
        }

        #endregion

        #region ShowNamesInNotifications

        public bool ShowNamesInNotificationsInh
        {
            get
            {
                if (OptionalData.HasData (ObjectOptionalDataType.OrgShowNamesInNotifications))
                    return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgShowNamesInNotifications);
                if (ParentIdentity != 0)
                    return Parent.ShowNamesInNotificationsInh;
                return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgShowNamesInNotifications);
            }
        }

        public bool? ShowNamesInNotifications
        {
            get
            {
                if (OptionalData.HasData (ObjectOptionalDataType.OrgShowNamesInNotifications))
                    return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgShowNamesInNotifications);
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    OptionalData.SetOptionalDataBool (ObjectOptionalDataType.OrgShowNamesInNotifications, value.Value);
                }
                else
                {
                    OptionalData.SetOptionalData (ObjectOptionalDataType.OrgShowNamesInNotifications, null);
                }
            }
        }

        #endregion

        #region UsePaymentStatus

        public bool UsePaymentStatusInh
        {
            get
            {
                if (OptionalData.HasData (ObjectOptionalDataType.OrgUsePaymentStatus))
                    return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgUsePaymentStatus);
                if (ParentIdentity != 0)
                    return Parent.UsePaymentStatusInh;
                return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgUsePaymentStatus);
            }
        }

        public bool? UsePaymentStatus
        {
            get
            {
                if (OptionalData.HasData (ObjectOptionalDataType.OrgUsePaymentStatus))
                    return OptionalData.GetOptionalDataBool (ObjectOptionalDataType.OrgUsePaymentStatus);
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    OptionalData.SetOptionalDataBool (ObjectOptionalDataType.OrgUsePaymentStatus, value.Value);
                }
                else
                {
                    OptionalData.SetOptionalData (ObjectOptionalDataType.OrgUsePaymentStatus, null);
                }
            }
        }

        #endregion

        public Person GetTreasurer()
        {
            Organizations line = GetLine();
            BasicPersonRole[] treasurers
                = SwarmDb.GetDatabaseForReading()
                    .GetPeopleWithRoleType (RoleType.OrganizationTreasurer, line.Identities, new int[] {});

            if (treasurers.Length == 0)
                throw new Exception ("No Treasurer Found");

            if (treasurers.Length == 1)
                return Person.FromIdentity (treasurers[0].PersonId);

            for (int i = line.Count - 1; i >= 0; --i)
            {
                //check orgline backwards, giving the most local role
                Organization org = line[i];
                foreach (BasicPersonRole br in treasurers)
                {
                    if (org.Identity == br.OrganizationId)
                        return Person.FromIdentity (br.PersonId);
                }
            }

            throw new Exception ("No Treasurer Found");
        }


        public Memberships GetMemberships (bool includeTerminated)
        {
            return Memberships.ForOrganization (this, includeTerminated);
        }

        public Memberships GetMemberships()
        {
            return GetMemberships (false);
        }

        public static Organization Create (int parentOrganizationId, string nameInternational, string name,
            string nameShort, string domain, string mailPrefix, int anchorGeographyId, bool acceptsMembers,
            bool autoAssignNewMembers, int defaultCountryId)
        {
            return FromIdentityAggressive (OrganizationCache.CreateOrganization (parentOrganizationId,
                nameInternational,
                name,
                nameShort,
                domain,
                mailPrefix,
                anchorGeographyId,
                acceptsMembers,
                autoAssignNewMembers,
                defaultCountryId));
        }

        [Obsolete ("Never use this function. Mark the organization as unused. Records are needed for history.", true)]
        public void Delete()
        {
            string problems = "";
            int ChildrenCount = Children.Count;
            int ActiveMembershipsCount = GetMemberships (false).Count;
            int HistoricalMembershipsCount = GetMemberships (true).Count - ActiveMembershipsCount;

            if (ChildrenCount > 0)
                problems += ChildrenCount + " child organisations\n\r";

            if (ActiveMembershipsCount > 0)
                problems += ActiveMembershipsCount + " active memberships\n\r";

            if (HistoricalMembershipsCount > 0)
                problems += HistoricalMembershipsCount + " historical memberships\n\r";

            if (problems != "")
                throw new Exception ("Can not delete because:\n\r" + problems);

            // OrganizationCache.DeleteOrganization(this.Identity);  -- commented out because OrganizationCache.Delete is also marked obsolete
        }

        public static void UpdateOrganization (int ParentOrganizationId, string NameInternational, string Name,
            string NameShort, string Domain, string MailPrefix, int AnchorGeographyId, bool AcceptsMembers,
            bool AutoAssignNewMembers, int DefaultCountryId, int OrganizationId)
        {
            OrganizationCache.UpdateOrganization (ParentOrganizationId, NameInternational, Name, NameShort, Domain,
                MailPrefix, AnchorGeographyId, AcceptsMembers, AutoAssignNewMembers, DefaultCountryId,
                OrganizationId);
        }

        public DateTime GetEndOfFiscalYear (int year)
        {
            // Returns the last millisecond of the fiscal year. For now, this is limited to calendar years; the function
            // is placed here for future flexibility.

            return new DateTime (year, 12, 31, 23, 59, 59, 999);
        }
    }
}