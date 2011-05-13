using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class PaymentGroup: BasicPaymentGroup
    {
        private PaymentGroup(BasicPaymentGroup basic): base (basic)
        {
            // empty ctor
        }

        public static PaymentGroup FromIdentity (int paymentGroupId)
        {
            return FromBasic(PirateDb.GetDatabase().GetPaymentGroup(paymentGroupId));
        }

        public static PaymentGroup FromBasic (BasicPaymentGroup basic)
        {
            return new PaymentGroup(basic);
        }

        public static PaymentGroup Create (Organization organization, DateTime timestamp, Currency currency, Person createdByPerson)
        {
            return
                FromIdentity(PirateDb.GetDatabase().CreatePaymentGroup(organization.Identity, timestamp,
                                                                       currency.Identity,
                                                                       System.DateTime.Now, createdByPerson.Identity));
        }

        public static PaymentGroup FromTag (Organization organization, string tag)
        {
            BasicPaymentGroup basicGroup = PirateDb.GetDatabase().GetPaymentGroupByTag(organization.Identity, tag);

            if (basicGroup == null)
            {
                return null;
            }

            return FromBasic(basicGroup);
        }

        public Payment CreatePayment (double amount, string reference, string fromAccount, string key, bool hasImage)
        {
            return Payment.Create(this, amount, reference, fromAccount, key, hasImage);
        }

        public new string Tag
        {
            get { return base.Tag; }
            set 
            { 
                base.Tag = value;
                PirateDb.GetDatabase().SetPaymentGroupTag(this.Identity, value);
            }
        }

        public new Int64 AmountCents
        {
            get { return base.AmountCents; }
            set 
            {
                base.AmountCents = value;
                PirateDb.GetDatabase().SetPaymentGroupAmount(this.Identity, value);
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
                PirateDb.GetDatabase().SetPaymentGroupOpen(this.Identity, value);
            }
        }


        public Organization Organization
        {
            get { return Organization.FromIdentity(this.OrganizationId); }
        }


        public bool MapTransaction()
        {
            // This function attempts to find a transaction that matches the payment group, a transaction that is currently unbalanced.
            // If found, it balances the transaction against accounts receivable and ties it to the payment group.

            FinancialTransactions transactions = FinancialTransactions.GetUnbalanced(this.Organization);

            string lookfor = "bg 451-0061 " + this.Tag.Substring(7);  // TODO: Set string and tag start to org dependent
            
            foreach (FinancialTransaction transaction in transactions)
            {
                if (transaction.Description.ToLower() == lookfor && transaction.Rows.AmountCentsTotal == this.AmountCents)
                {
                    // match!
                    
                    transaction.AddRow(Organization.FinancialAccounts.AssetsOutboundInvoices, -AmountCents, null);
                    transaction.Dependency = this;
                    this.Open = false;
                    return true;
                }
            }

            return false;
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
    }
}
