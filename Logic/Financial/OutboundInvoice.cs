using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class OutboundInvoice: BasicOutboundInvoice
    {
        private OutboundInvoice (BasicOutboundInvoice basic): base (basic)
        {
            // empty ctor
        }

        public static OutboundInvoice FromBasic (BasicOutboundInvoice basic)
        {
            return new OutboundInvoice(basic);
        }

        public static OutboundInvoice FromIdentity (int outboundInvoiceId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetOutboundInvoice(outboundInvoiceId));
        }

        public static OutboundInvoice FromReference (string reference)
        {
            // TODO: Verify Luhn



            // Use identity as drogue chute

            string identityString = reference.Substring(12, reference.Length - 13);

            OutboundInvoice result = FromIdentity(Int32.Parse(identityString));

            if (result.Reference != reference)
            {
                throw new ArgumentException("No such outbound invoice");
            }

            return result;
        }

        public static OutboundInvoice Create (Organization organization, Person createdByPerson, DateTime dueDate, FinancialAccount budget, string customerName, string invoiceAddressMail, string invoiceAddressPaper, Currency currency, bool domestic, string theirReference)
        {
            OutboundInvoice invoice = FromIdentity (SwarmDb.GetDatabaseForWriting().CreateOutboundInvoice(organization.Identity, createdByPerson != null? createdByPerson.Identity : 0, dueDate,
                                                         budget.Identity, customerName, invoiceAddressPaper,
                                                         invoiceAddressMail, currency.Identity, string.Empty, domestic, Authentication.CreateRandomPassword(6), theirReference));

            // Set reference

            invoice.Reference =
                Formatting.AddLuhnChecksum(Formatting.ReverseString(DateTime.Now.ToString("yyyyMMddHHmm")) +
                                           invoice.Identity.ToString());

            return invoice;
        }

        public OutboundInvoiceItems Items
        {
            get
            {
                return OutboundInvoiceItems.ForInvoice(this);
            }
        }


        public void AddItem(string description, double amount)
        {
            SwarmDb.GetDatabaseForWriting().CreateOutboundInvoiceItem(this.Identity, description, amount);
        }


        public void AddItem(string description, Int64 amountCents)
        {
            // NOTE: MUST NOT ADD FINANCIAL TRANSACTION LINES HERE; SURCHARGES ARE ADDED AS LINE ITEMS ON PAYMENT

            SwarmDb.GetDatabaseForWriting().CreateOutboundInvoiceItem(this.Identity, description, amountCents);
        }


        public decimal Amount
        {
            get
            {
                decimal result = 0.0m;

                OutboundInvoiceItems items = this.Items;

                foreach (OutboundInvoiceItem item in items)
                {
                    result += item.Amount;
                }

                return result;
            }
        }


        public Int64 AmountCents
        {
            get
            {
                Int64 result = 0;

                OutboundInvoiceItems items = this.Items;

                foreach (OutboundInvoiceItem item in items)
                {
                    result += item.AmountCents;
                }

                return result;
            }
        }


        public Organization Organization
        {
            get
            {
                return Structure.Organization.FromIdentity(this.OrganizationId);
            }
        }

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity(this.BudgetId); }
        }

        public new bool Sent
        {
            get
            {
                return base.Sent;
            }
            set
            {
                if (value != base.Sent)
                {
                    if (value == true)
                    {
                        SwarmDb.GetDatabaseForWriting().SetOutboundInvoiceSent(this.Identity, true);
                        base.Sent = true;
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot set a sent invoice to unsent status");
                    }
                }
            }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (value != base.Open)
                {
                    SwarmDb.GetDatabaseForWriting().SetOutboundInvoiceOpen(this.Identity, value);
                    base.Open = value;
                }
            }
        }

        public void SetPaid()
        {
            Open = false;
            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoicePaid, 0, this.OrganizationId,
                                 Geography.RootIdentity, 0, this.Identity, string.Empty);
        }

        public Payment Payment
        {
            get
            {
                Payment payment = null;

                try
                {
                    payment = Financial.Payment.ForOutboundInvoice(this);
                }
                catch (Exception)
                {
                }

                return payment;
            }
        }

        public new string Reference
        {
            get
            {
                return base.Reference;
            }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetOutboundInvoiceReference(this.Identity, value);
                base.Reference = value;
            }
        }

        public Currency Currency
        {
            get { return Currency.FromIdentity(base.CurrencyId); }
        }



        public OutboundInvoice Credit(Person creditingPerson)
        {
            OutboundInvoice credit = OutboundInvoice.Create(this.Organization, creditingPerson, DateTime.Now,
                                                            this.Budget, this.CustomerName,
                                                            this.InvoiceAddressMail,
                                                            this.InvoiceAddressPaper, this.Currency,
                                                            this.Domestic, this.TheirReference);
            if (this.Domestic)   // TODO: LANGUAGE
            {
                credit.AddItem("Kredit för faktura " + this.Reference, -this.AmountCents);
                credit.AddItem("DETTA ÄR EN KREDITFAKTURA OCH SKA EJ BETALAS", 0.00);

                this.AddItem(String.Format("KREDITERAD {0:yyyy-MM-dd} i kreditfaktura {1}", DateTime.Today, credit.Reference), 0.00);
            }
            else
            {
                credit.AddItem("Credit for invoice " + this.Reference, -this.AmountCents);
                credit.AddItem("THIS IS A CREDIT. DO NOT PAY.", 0.00);

                this.AddItem(String.Format("CREDITED {0:yyyy-MM-dd} in credit invoice {1}", DateTime.Today, credit.Reference), 0.00);
            }

            this.CreditInvoice = credit;
            credit.CreditsInvoice = this;


            credit.Open = false;

            // Create the financial transaction with rows

            FinancialTransaction transaction =
                FinancialTransaction.Create(credit.OrganizationId, DateTime.Now,
                "Credit Invoice #" + credit.Identity + " to " + credit.CustomerName);

            transaction.AddRow(Organization.FromIdentity(credit.OrganizationId).FinancialAccounts.AssetsOutboundInvoices, credit.AmountCents, creditingPerson);
            transaction.AddRow(credit.Budget, -credit.AmountCents, creditingPerson);

            // Make the transaction dependent on the credit

            transaction.Dependency = credit;

            // Create the event for PirateBot-Mono to send off mails

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoiceCreated, creditingPerson.Identity,
                                 this.OrganizationId, Geography.RootIdentity, 0, credit.Identity, string.Empty);

            // If this invoice was already closed, issue a credit. If not closed, close it.

            if (this.Open)
            {
                this.Open = false;
            }
            else
            {
                Payment payment = this.Payment;

                if (payment != null)
                {
                    payment.Refund(creditingPerson);
                }
            }

            return credit;
        }

        public Person Person
        {
            get
            {
                ObjectOptionalData data = ObjectOptionalData.ForObject(this);

                int personId = data.GetOptionalDataInt(ObjectOptionalDataType.OutboundInvoiceToPersonId);
                if (personId == 0)
                {
                    return null;
                }

                return Person.FromIdentity(personId);
            }
            set
            {
                ObjectOptionalData data = ObjectOptionalData.ForObject(this);
                data.SetOptionalDataInt(ObjectOptionalDataType.OutboundInvoiceToPersonId, value.Identity);
            }
        }

        public OutboundInvoice CreditInvoice
        {
            get
            {
                ObjectOptionalData data = ObjectOptionalData.ForObject(this);

                int invoiceId = data.GetOptionalDataInt(ObjectOptionalDataType.OutboundInvoiceCreditedWithInvoice);
                if (invoiceId == 0)
                {
                    return null;
                }

                return OutboundInvoice.FromIdentity(invoiceId);
            }
            set
            {
                ObjectOptionalData data = ObjectOptionalData.ForObject(this);
                data.SetOptionalDataInt(ObjectOptionalDataType.OutboundInvoiceCreditedWithInvoice, value.Identity);
            }
        }

        public OutboundInvoice CreditsInvoice
        {
            get
            {
                ObjectOptionalData data = ObjectOptionalData.ForObject(this);

                int invoiceId = data.GetOptionalDataInt(ObjectOptionalDataType.OutboundInvoiceCreditsInvoice);
                if (invoiceId == 0)
                {
                    return null;
                }

                return OutboundInvoice.FromIdentity(invoiceId);
            }
            set
            {
                ObjectOptionalData data = ObjectOptionalData.ForObject(this);
                data.SetOptionalDataInt(ObjectOptionalDataType.OutboundInvoiceCreditsInvoice, value.Identity);
            }
        }

    }
}
