using Swarmops.Common.Enums;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Structure
{
    public class OrganizationParameters
    {
        private readonly ObjectOptionalData data;
        private readonly Organization organization;

        public OrganizationParameters (Organization organization)
        {
            this.organization = organization;
            this.data = ObjectOptionalData.ForObject (organization);
        }

        public bool EconomyEnabled
        {
            get
            {
                if (this.data.HasData (ObjectOptionalDataType.OrgEconomyEnabled))
                {
                    return this.data.GetOptionalDataBool (ObjectOptionalDataType.OrgEconomyEnabled);
                }

                if (this.organization.Identity == 1) // temp
                {
                    this.data.SetOptionalDataBool (ObjectOptionalDataType.OrgEconomyEnabled, true);
                }

                return false;
            }
        }


        public int FirstFiscalYear
        {
            get
            {
                int year = this.data.GetOptionalDataInt (ObjectOptionalDataType.OrgFirstFiscalYear);
                return year;
            }
        }


        public int FiscalBooksClosedUntilYear
        {
            get
            {
                int year = this.data.GetOptionalDataInt (ObjectOptionalDataType.OrgBooksClosedForYear);

                return year;
            }
            set { this.data.SetOptionalDataInt (ObjectOptionalDataType.OrgBooksClosedForYear, value); }
        }


        public int FiscalBooksAuditedUntilYear
        {
            get
            {
                int year = this.data.GetOptionalDataInt (ObjectOptionalDataType.OrgBooksClosedForYear);

                if (year == 0 && this.organization.Identity == 1)
                {
                    this.data.SetOptionalDataInt (ObjectOptionalDataType.OrgBooksClosedForYear, 2009);
                    return 2009;
                }

                return year;
            }
        }

        public string TaxAccount
        {
            get
            {
                string taxAccount = this.data.GetOptionalDataString (ObjectOptionalDataType.OrgTaxAccount);

                if (string.IsNullOrEmpty (taxAccount) && this.organization.Identity == 1)
                {
                    taxAccount = "Bg 5050-1055";
                    this.data.SetOptionalDataString (ObjectOptionalDataType.OrgTaxAccount, taxAccount);
                }

                return taxAccount;
            }
        }

        public string TaxOcr
        {
            get
            {
                string taxOcr = this.data.GetOptionalDataString (ObjectOptionalDataType.OrgTaxOcr);

                if (string.IsNullOrEmpty (taxOcr) && this.organization.Identity == 1)
                {
                    taxOcr = "1680243045141";
                    this.data.SetOptionalDataString (ObjectOptionalDataType.OrgTaxOcr, taxOcr);
                }

                return taxOcr;
            }
        }

        public string TemporaryAccessListRead
        {
            get
            {
                string accessList = this.data.GetOptionalDataString (ObjectOptionalDataType.OrgTemporaryAccessListRead);

                if (string.IsNullOrEmpty (accessList))
                {
                    accessList = string.Empty;
                }

                return accessList;
            }
            set { this.data.SetOptionalDataString (ObjectOptionalDataType.OrgTemporaryAccessListRead, value); }
        }

        public string TemporaryAccessListWrite
        {
            get
            {
                string accessList = this.data.GetOptionalDataString (ObjectOptionalDataType.OrgTemporaryAccessListWrite);

                if (string.IsNullOrEmpty (accessList))
                {
                    accessList = "1"; // Priming new organizations with the sysadmin account
                }

                return accessList;
            }
            set { this.data.SetOptionalDataString (ObjectOptionalDataType.OrgTemporaryAccessListWrite, value); }
        }

        public bool AskParticipantStreet
        {
            get { return this.data.GetOptionalDataBool(ObjectOptionalDataType.OrgAskParticipantStreet); }
            set { this.data.SetOptionalDataBool(ObjectOptionalDataType.OrgAskParticipantStreet, value); }
        }

        public string ParticipationEntry
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgParticipationEntry); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgParticipationEntry, value); }
        }

        public string ParticipationDuration
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgParticipationDuration); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgParticipationDuration, value); }
        }
        public string ParticipationAcceptedMail
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgParticipationAcceptedMail); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgParticipationAcceptedMail, value); }
        }
        public string SidebarOrgInfo
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgSidebarShortInfo); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgSidebarShortInfo, value); }
        }
        public string SignupFirstPage
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgSignupFirstPage); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgSignupFirstPage, value); }
        }
        public string SignupLastPage
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgSignupLastPage); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgSignupLastPage, value); }
        }
        public string ApplicationCompleteMail
        {
            get { return this.data.GetOptionalDataString(ObjectOptionalDataType.OrgApplicationCompleteMail); }
            set { this.data.SetOptionalDataString(ObjectOptionalDataType.OrgApplicationCompleteMail, value); }
        }
        public int ApplicationQualifyingScore
        {
            get { return this.data.GetOptionalDataInt(ObjectOptionalDataType.OrgApplicationQualifyingScore); }
            set { this.data.SetOptionalDataInt(ObjectOptionalDataType.OrgApplicationQualifyingScore, value); }
        }
    }
}