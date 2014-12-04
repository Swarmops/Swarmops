using System;
using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class PaymentGroup : BasicPaymentGroup, ISummable
    {
        private PaymentGroup(BasicPaymentGroup basic) : base(basic)
        {
            // empty ctor
        }

        public new string Tag
        {
            get { return base.Tag; }
            set
            {
                base.Tag = value;
                SwarmDb.GetDatabaseForWriting().SetPaymentGroupTag(Identity, value);
            }
        }

        public new Int64 AmountCents
        {
            get { return base.AmountCents; }
            set
            {
                base.AmountCents = value;
                SwarmDb.GetDatabaseForWriting().SetPaymentGroupAmount(Identity, value);
            }
        }

        public decimal AmountDecimal
        {
            get { return AmountCents/100.0m; }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                base.Open = value;
                SwarmDb.GetDatabaseForWriting().SetPaymentGroupOpen(Identity, value);
            }
        }


        public Organization Organization
        {
            get { return Organization.FromIdentity(OrganizationId); }
        }

        public Payments Payments
        {
            get { return Payments.ForPaymentGroup(this); }
        }

        public string Payers
        {
            get
            {
                List<string> payerList = new List<string>();
                foreach (Payment payment in Payments)
                {
                    string payer = payment.PayerName;

                    if (!string.IsNullOrEmpty(payer))
                    {
                        payerList.Add(payer);
                    }
                }

                return String.Join(", ", payerList.ToArray());
            }
        }

        #region Implementation of ISummable

        public long SumCents
        {
            get { return AmountCents; }
        }

        public OrganizationFinancialAccountType CounterAccountType
        {
            get { return OrganizationFinancialAccountType.AssetsOutboundInvoices; }
        }

        #endregion

        public static PaymentGroup FromIdentity(int paymentGroupId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetPaymentGroup(paymentGroupId));
        }

        private static PaymentGroup FromIdentityAggressive(int paymentGroupId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetPaymentGroup(paymentGroupId));
                // "For writing" intentional - solves race conditions on create
        }

        public static PaymentGroup FromBasic(BasicPaymentGroup basic)
        {
            return new PaymentGroup(basic);
        }

        public static PaymentGroup Create(Organization organization, DateTime timestamp, Currency currency,
            Person createdByPerson)
        {
            return
                FromIdentityAggressive(SwarmDb.GetDatabaseForWriting()
                    .CreatePaymentGroup(organization.Identity, timestamp,
                        currency.Identity,
                        DateTime.Now, createdByPerson == null ? 0 : createdByPerson.Identity));
        }

        public static PaymentGroup FromTag(Organization organization, string tag)
        {
            BasicPaymentGroup basicGroup = SwarmDb.GetDatabaseForReading()
                .GetPaymentGroupByTag(organization.Identity, tag);

            if (basicGroup == null)
            {
                return null;
            }

            return FromBasic(basicGroup);
        }

        public Payment CreatePayment(double amount, string reference, string fromAccount, string key, bool hasImage)
        {
            return Payment.Create(this, amount, reference, fromAccount, key, hasImage);
        }


        public bool MapTransaction()
        {
            // This function attempts to find a transaction that matches the payment group, a transaction that is currently unbalanced.
            // If found, it balances the transaction against accounts receivable and ties it to the payment group.

            FinancialTransactions transactions = FinancialTransactions.GetUnbalanced(Organization);

            return MapTransaction(transactions);
        }


        public bool MapTransaction(FinancialTransactions transactions)
        {
            int namespaceLength = 4;
                // typically, transactions are tagged to be unique per year, so the year is added to tag to make unique

            switch (Char.ToLowerInvariant(Tag[5]))
            {
                case 'm':
                    namespaceLength = 6; // unique per year and month
                    break;
                case 'd':
                    namespaceLength = 8; // unique per year, month, day
                    break;
                default:
                    // do nothing -- assume year, namespace length 4 as declared on init
                    break;
            }

            string lookfor = Tag.Substring(5 + namespaceLength).ToLowerInvariant().Trim();
            int year = Int32.Parse(Tag.Substring(5, 4));

            if (Char.IsDigit(Tag[0]))
            {
                lookfor = Tag.Substring(namespaceLength); // temp - remove after PPSE books closed for 2011
                year = Int32.Parse(Tag.Substring(0, 4));
            }

            foreach (FinancialTransaction transaction in transactions)
            {
                if (transaction.Description.Length >= Organization.IncomingPaymentTag.Length + lookfor.Length &&
                    transaction.Description.ToLowerInvariant().StartsWith(Organization.IncomingPaymentTag) &&
                    transaction.Description.Trim().ToLowerInvariant().EndsWith(lookfor))
                {
                    if (transaction.Rows.AmountCentsTotal == AmountCents)
                    {
                        if (transaction.DateTime.Year == year)
                        {
                            // TODO: Prep for matching month/date too, when those namespaces appear

                            // match!

                            transaction.AddRow(Organization.FinancialAccounts.AssetsOutboundInvoices, -AmountCents, null);
                            transaction.Dependency = this;
                            Open = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}