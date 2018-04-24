using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class FinancialAccountAutomationProfile: IHasIdentity
    {
        public int FinancialAccountAutomationProfileId { get; set; }
        public string Name { get; set; }
        public bool CanRetrieve { get; set; }

        [XmlIgnore]
        public Country Country
        {
            get { return CountryId != 0? Country.FromIdentity(CountryId): null; }
            set { this.CountryId = (value == null? 0: value.Identity); }
        }

        [XmlIgnore]
        public Currency Currency
        {
            get { return CurrencyId != 0? Currency.FromIdentity(CurrencyId): null; }
            set { this.CurrencyId = (value == null? 0: value.Identity); }
        }

        [XmlIgnore]
        public ExternalBankDataProfile BankDataProfile
        {
            get { return BankDataProfileId != 0 ? ExternalBankDataProfile.FromIdentity(BankDataProfileId): null; }
            set { this.BankDataProfileId = (value == null ? 0 : value.Identity); }
        }

        protected int BankDataProfileId { get; set; }
        protected int CountryId { get; set; }
        protected int CurrencyId { get; set; }

        [XmlIgnore]
        public int Identity { get { return FinancialAccountAutomationProfileId; } }
    }



    public enum FinancialAccountAutomationProfileHardIds
    {
        Unknown = 0,
        Custom = 1,
        BitcoinHotwallet = 2,
        BitcoinCashArmory = 3,
        BitcoinCoreArmory = 4,
        BankPaypal = 5,
        BankSwedenSeb = 6,
        BankGermanyPostbank = 7,
        BankCzechFio = 8
    }
}
