using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicCurrency: IHasIdentity
    {
        public BasicCurrency (int currencyId, string code, string description, string sign)
        {
            this.CurrencyId = currencyId;
            this.Code = code;
            this.Description = description;
            this.Sign = sign;
        }

        public BasicCurrency (BasicCurrency original)
            : this (original.CurrencyId, original.Code, original.Description, original.Sign)
        {
            // empty copy ctor
        }


        public int CurrencyId { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public string Sign { get; private set; }

        public int Identity
        {
            get { return this.CurrencyId; }
        }
    }
}
