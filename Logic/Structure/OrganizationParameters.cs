using Swarmops.Basic.Enums;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Structure
{
    public class OrganizationParameters
    {
        public OrganizationParameters (Organization organization)
        {
            this.organization = organization;
            data = ObjectOptionalData.ForObject(organization);
        }

        public bool EconomyEnabled
        {
            get
            {
                if (data.HasData(ObjectOptionalDataType.OrgEconomyEnabled))
                {
                    return data.GetOptionalDataBool(ObjectOptionalDataType.OrgEconomyEnabled);
                }

                if (organization.Identity == 1)  // temp
                {
                    data.SetOptionalDataBool(ObjectOptionalDataType.OrgEconomyEnabled, true);
                }

                return false;
            }
        }


        public int FirstFiscalYear
        {
            get 
            { 
                int year = data.GetOptionalDataInt(ObjectOptionalDataType.OrgFirstFiscalYear);
                return year;
            }
        }


        public int FiscalBooksClosedUntilYear
        {
            get
            {
                int year = data.GetOptionalDataInt(ObjectOptionalDataType.OrgBooksClosedForYear);

                return year;
            }
            set
            {
                data.SetOptionalDataInt(ObjectOptionalDataType.OrgBooksClosedForYear, value);
            }
        }


        public int FiscalBooksAuditedUntilYear
        {
            get
            {
                int year = data.GetOptionalDataInt(ObjectOptionalDataType.OrgBooksClosedForYear);

                if (year == 0 && organization.Identity == 1)
                {
                    data.SetOptionalDataInt(ObjectOptionalDataType.OrgBooksClosedForYear, 2009);
                    return 2009;
                }

                return year;
            }
        }

        public string TaxAccount
        {
            get 
            { 
                string taxAccount = data.GetOptionalDataString(ObjectOptionalDataType.OrgTaxAccount);

                if (string.IsNullOrEmpty(taxAccount) && organization.Identity == 1)
                {
                    taxAccount = "Bg 5050-1055";
                    data.SetOptionalDataString(ObjectOptionalDataType.OrgTaxAccount, taxAccount);
                }

                return taxAccount;
            }
        }

        public string TaxOcr
        {
            get 
            { 
                string taxOcr = data.GetOptionalDataString(ObjectOptionalDataType.OrgTaxOcr);

                if (string.IsNullOrEmpty(taxOcr) && organization.Identity == 1)
                {
                    taxOcr = "1680243045141";
                    data.SetOptionalDataString(ObjectOptionalDataType.OrgTaxOcr, taxOcr);
                }

                return taxOcr;
            }
        }

        public string TemporaryAccessListRead
        {
            get
            {
                string accessList = data.GetOptionalDataString(ObjectOptionalDataType.OrgTemporaryAccessListRead);

                if (string.IsNullOrEmpty(accessList))
                {
                    accessList = string.Empty;
                }

                return accessList;
            }
            set
            {
                data.SetOptionalDataString(ObjectOptionalDataType.OrgTemporaryAccessListRead, value);
            }
        }

        public string TemporaryAccessListWrite
        {
            get
            {
                string accessList = data.GetOptionalDataString(ObjectOptionalDataType.OrgTemporaryAccessListWrite);

                if (string.IsNullOrEmpty(accessList))
                {
                    accessList = "1"; // Priming new organizations with the sysadmin account
                }

                return accessList;
            }
            set
            {
                data.SetOptionalDataString(ObjectOptionalDataType.OrgTemporaryAccessListWrite, value);
            }
        }

        private ObjectOptionalData data;
        private Organization organization;
    }
}
