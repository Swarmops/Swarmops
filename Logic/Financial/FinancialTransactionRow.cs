using System;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionRow : BasicFinancialTransactionRow
    {
        private FinancialAccount account;
        private FinancialTransaction transaction;

        private FinancialTransactionRow (BasicFinancialTransactionRow basic)
            : base (basic)
        {
        }

        public FinancialTransaction Transaction
        {
            get
            {
                if (this.transaction == null)
                {
                    this.transaction = FinancialTransaction.FromIdentity (base.FinancialTransactionId);
                }

                return this.transaction;
            }
        }

        public FinancialAccount Account
        {
            get
            {
                if (this.account == null)
                {
                    this.account = FinancialAccount.FromIdentity (base.FinancialAccountId);
                }

                return this.account;
            }
        }

        public string AccountName
        {
            get { return Account.Name; }
        }

        public Person CreatedByPerson
        {
            get
            {
                if (base.CreatedByPersonId > 0)
                {
                    return Person.FromIdentity (base.CreatedByPersonId);
                }

                return null;
            }
        }

        public Money NativeAmountCents
        {
            get
            {
                Currency currency = this.Account.NativeCurrency;
                int currencyId = currency.Identity;
                Int64 nativeCents = SwarmDb.GetDatabaseForReading().GetFinancialTransactionRowNativeAmountCents (this.Identity, currencyId);
                return new Money(nativeCents, currency, this.CreatedDateTime); // includes the valuation datetime, as it's a non-org currency
            }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetFinancialTransactionRowNativeAmountCents (this.Identity, value.Currency.Identity, value.Cents);
            }
        }

        [Obsolete ("Do not use. Use Int64 AmountCents.", true)]
        public decimal Amount
        {
            get { return AmountCents/100.0m; }
        }

        public static FinancialTransactionRow FromBasic (BasicFinancialTransactionRow basic)
        {
            return new FinancialTransactionRow (basic);
        }

        public static FinancialTransactionRow FromIdentity(int identity)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetFinancialTransactionRow(identity));
        }

        public static FinancialTransactionRow FromIdentityAggressive(int identity)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetFinancialTransactionRow(identity)); // "For writing" is intentional
        }
    }
}