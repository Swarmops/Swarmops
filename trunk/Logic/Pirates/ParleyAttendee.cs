using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Structure;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Pirates
{
    public class ParleyAttendee: BasicParleyAttendee
    {
        private ParleyAttendee (BasicParleyAttendee basic): base(basic)
        {
            // empty pvt ctor
        }

        public static ParleyAttendee FromBasic (BasicParleyAttendee basic)
        {
            return new ParleyAttendee(basic);
        }

        public static ParleyAttendee FromIdentity (int parleyAttendeeId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetParleyAttendee(parleyAttendeeId));
        }

        public static ParleyAttendee Create (Parley parley, Person person, bool asGuest)
        {
            return FromIdentity(PirateDb.GetDatabaseForWriting().CreateParleyAttendee(parley.Identity, person.Identity, asGuest));
        }

        public void AddOption (ParleyOption option)
        {
            PirateDb.GetDatabaseForWriting().AddParleyAttendeeOption(this.Identity, option.Identity);            
        }

        public ParleyOptions Options
        {
            get { return ParleyOptions.ForParleyAttendee(this); }
        }

        public Parley Parley
        {
            get { return Parley.FromIdentity(this.ParleyId); }
        }

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                if (value != base.Active)
                {
                    PirateDb.GetDatabaseForWriting().SetParleyAttendeeActive(this.Identity, value);
                    base.Active = value;
                }
            }
        }

        public Person Person
        {
            get { return Person.FromIdentity(this.PersonId); }
        }

        public OutboundInvoice Invoice
        {
            get { return OutboundInvoice.FromIdentity(base.OutboundInvoiceId); }
        }

        public void SendInvoice()
        {
            DateTime invoiceDue = DateTime.Today.AddDays(14);
            DateTime maxInvoiceDue = this.Parley.StartDate.AddDays(-10);

            if (invoiceDue > maxInvoiceDue)
            {
                invoiceDue = maxInvoiceDue;
            }

            OutboundInvoice invoice = OutboundInvoice.Create(this.Parley.Organization, this.Parley.Person, invoiceDue,
                                                             this.Parley.Budget, this.Person.Name, this.Person.Email,
                                                             string.Empty,
                                                             this.Parley.Organization.DefaultCountry.Currency, true,
                                                             string.Empty);

            invoice.AddItem("Deltagarkostnad " + this.Parley.Name, this.Parley.AttendanceFeeCents);  // TODO: Localize

            ParleyOptions options = this.Options;

            foreach (ParleyOption option in options)
            {
                invoice.AddItem(option.Description, option.AmountCents);
            }


            // Create the financial transaction with rows

            FinancialTransaction transaction =
                FinancialTransaction.Create(this.Parley.OrganizationId, DateTime.Now,
                "Outbound Invoice #" + invoice.Identity + " to " + this.Person.Name);

            transaction.AddRow(this.Parley.Organization.FinancialAccounts.AssetsOutboundInvoices, invoice.AmountCents, null);
            transaction.AddRow(this.Parley.Budget, -invoice.AmountCents, null);

            // Make the transaction dependent on the outbound invoice

            transaction.Dependency = invoice;

            // Create the event for PirateBot-Mono to send off mails

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoiceCreated,
                                                       this.PersonId, this.Parley.OrganizationId, 1, this.PersonId,
                                                       invoice.Identity, string.Empty);

            // Update the attendee as invoiced

            base.Invoiced = true;
            base.OutboundInvoiceId = invoice.Identity;

            PirateDb.GetDatabaseForWriting().SetParleyAttendeeInvoiced(this.Identity, invoice.Identity);
        }
    }
}
