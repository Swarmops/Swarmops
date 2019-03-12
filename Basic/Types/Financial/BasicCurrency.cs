using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicCurrency : IHasIdentity
    {
        public BasicCurrency (int currencyId, string code, string name, string sign, bool isCrypto)
        {
            this.CurrencyId = currencyId;
            this.Code = code;
            this.Name = name;
            this.Sign = sign;
            this.IsCrypto = isCrypto;
        }

        public BasicCurrency (BasicCurrency original)
            : this (original.CurrencyId, original.Code, original.Name, original.Sign, original.IsCrypto)
        {
            // empty copy ctor
        }


        public int CurrencyId { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Sign { get; private set; }
        public bool IsCrypto { get; private set; }

        public int Identity
        {
            get { return this.CurrencyId; }
        }
    }
}