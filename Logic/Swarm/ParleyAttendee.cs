using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class ParleyAttendee : BasicParleyAttendee
    {
        private ParleyAttendee(BasicParleyAttendee basic) : base(basic)
        {
            // empty pvt ctor
        }

        public ParleyOptions Options
        {
            get { return ParleyOptions.ForParleyAttendee(this); }
        }

        public Parley Parley
        {
            get { return Parley.FromIdentity(ParleyId); }
        }

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                if (value != base.Active)
                {
                    SwarmDb.GetDatabaseForWriting().SetParleyAttendeeActive(Identity, value);
                    base.Active = value;
                }
            }
        }

        public Person Person
        {
            get { return Person.FromIdentity(PersonId); }
        }

        public OutboundInvoice Invoice
        {
            get { return OutboundInvoice.FromIdentity(base.OutboundInvoiceId); }
        }

        public static ParleyAttendee FromBasic(BasicParleyAttendee basic)
        {
            return new ParleyAttendee(basic);
        }

        public static ParleyAttendee FromIdentity(int parleyAttendeeId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetParleyAttendee(parleyAttendeeId));
        }

        public static ParleyAttendee Create(Parley parley, Person person, bool asGuest)
        {
            return
                FromIdentity(SwarmDb.GetDatabaseForWriting()
                    .CreateParleyAttendee(parley.Identity, person.Identity, asGuest));
        }

        public void AddOption(ParleyOption option)
        {
            SwarmDb.GetDatabaseForWriting().AddParleyAttendeeOption(Identity, option.Identity);
        }

        public void SendInvoice()
        {
            DateTime invoiceDue = DateTime.Today.AddDays(14);
            DateTime maxInvoiceDue = Parley.StartDate.AddDays(-10);

            if (invoiceDue > maxInvoiceDue)
            {
                invoiceDue = maxInvoiceDue;
            }

            OutboundInvoice invoice = OutboundInvoice.Create(Parley.Organization, Parley.Person, invoiceDue,
                Parley.Budget, Person.Name, Person.Mail,
                string.Empty,
                Parley.Organization.DefaultCountry.Currency, true,
                string.Empty);

            invoice.AddItem("Deltagarkostnad " + Parley.Name, Parley.AttendanceFeeCents); // TODO: Localize

            ParleyOptions options = Options;

            foreach (ParleyOption option in options)
            {
                invoice.AddItem(option.Description, option.AmountCents);
            }


            // Create the financial transaction with rows

            FinancialTransaction transaction =
                FinancialTransaction.Create(Parley.OrganizationId, DateTime.Now,
                    "Outbound Invoice #" + invoice.Identity + " to " + Person.Name);

            transaction.AddRow(Parley.Organization.FinancialAccounts.AssetsOutboundInvoices, invoice.AmountCents, null);
            transaction.AddRow(Parley.Budget, -invoice.AmountCents, null);

            // Make the transaction dependent on the outbound invoice

            transaction.Dependency = invoice;

            // Create the event for PirateBot-Mono to send off mails

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoiceCreated,
                PersonId, Parley.OrganizationId, 1, PersonId,
                invoice.Identity, string.Empty);

            // Update the attendee as invoiced

            base.Invoiced = true;
            base.OutboundInvoiceId = invoice.Identity;

            SwarmDb.GetDatabaseForWriting().SetParleyAttendeeInvoiced(Identity, invoice.Identity);
        }
    }
}