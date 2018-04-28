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
        public static FinancialAccountAutomationProfile FromIdentity(int financialAccountAutomationProfileId)
        {
            if (financialAccountAutomationProfileId >= 100)
            {
                // Database ID, not hard ID

                throw new NotImplementedException();
            }

            FinancialAccountAutomationProfileHardIds hardId =
                (FinancialAccountAutomationProfileHardIds) financialAccountAutomationProfileId;

            switch (hardId)
            {
                case FinancialAccountAutomationProfileHardIds.Unknown:
                    throw new ArgumentException("Null Profile");

                case FinancialAccountAutomationProfileHardIds.Custom:
                    throw new NotImplementedException("Custom profiles not implemented");

                case FinancialAccountAutomationProfileHardIds.BankPaypal:
                    return new FinancialAccountAutomationProfile
                    {
                        CanManualUpload = true,
                        CanAutoRetrieve = false,
                        Name = "Paypal CSV",
                        Country = null, // global
                        Currency = null, // whatever the presentation currency is
                        BankDataProfile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.PaypalId)
                    };

                case FinancialAccountAutomationProfileHardIds.BankSwedenSeb:
                    return new FinancialAccountAutomationProfile
                    {
                        CanManualUpload = true,
                        CanAutoRetrieve = false,
                        Name = "Swedish SEB CSV",
                        Country = Structure.Country.FromCode("SE"),
                        Currency = Financial.Currency.FromCode("SEK"),
                        BankDataProfile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.SESebId)
                    };

                case FinancialAccountAutomationProfileHardIds.BankGermanyPostbank:
                    return new FinancialAccountAutomationProfile
                    {
                        CanManualUpload = true,
                        CanAutoRetrieve = false,
                        Name = "Deutsche Postbank CSV",
                        Country = Structure.Country.FromCode("DE"),
                        Currency = Financial.Currency.FromCode("EUR"),
                        BankDataProfile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.DEPostbankId)
                    };

                case FinancialAccountAutomationProfileHardIds.BankCzechFio:
                    return new FinancialAccountAutomationProfile
                    {
                        CanManualUpload = true,
                        CanAutoRetrieve = false,
                        Name = "Ceská Fio CSV",
                        Country = Structure.Country.FromCode("CZ"),
                        Currency = Financial.Currency.FromCode("CZK"),
                        BankDataProfile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.CZFioId)
                    };

                default:
                    return new FinancialAccountAutomationProfile
                    {
                        // Some other, non-upload automation

                        CanManualUpload = false,
                        CanAutoRetrieve = false,
                        Name = "Unspecified"
                    };
            }
        }



        public int FinancialAccountAutomationProfileId { get; set; }
        public string Name { get; set; }
        public bool CanAutoRetrieve { get; set; }
        public bool CanManualUpload { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Country Country
        {
            get { return CountryId != 0? Country.FromIdentity(CountryId): null; }
            set { this.CountryId = (value == null? 0: value.Identity); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public Currency Currency
        {
            get { return CurrencyId != 0? Currency.FromIdentity(CurrencyId): null; }
            set { this.CurrencyId = (value == null? 0: value.Identity); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public ExternalBankDataProfile BankDataProfile
        {
            get { return BankDataProfileId != 0 ? ExternalBankDataProfile.FromIdentity(BankDataProfileId): null; }
            set { this.BankDataProfileId = (value == null ? 0 : value.Identity); }
        }

        protected int BankDataProfileId { get; set; }
        protected int CountryId { get; set; }
        protected int CurrencyId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
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
