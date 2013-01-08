using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class Payment: BasicPayment
    {
        private Payment (BasicPayment payment)
            : base (payment)
        {
            // empty copy ctor
        }

        public static Payment FromBasic (BasicPayment basic)
        {
            return new Payment(basic);
        }

        public static Payment FromIdentity (int paymentId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetPayment(paymentId));
        }

        public static Payment ForOutboundInvoice (OutboundInvoice invoice)
        {
            BasicPayment[] array = PirateDb.GetDatabaseForReading().GetPayments(invoice);

            if (array.Length == 0)
            {
                return null; // or throw exception?
            }

            if (array.Length > 1)
            {
                throw new InvalidDataException("There can not be two payments for one invoice");
            }

            return FromBasic(array[0]);
        }

        public static Payment Create (PaymentGroup group, double amount, string reference, string fromAccount, string key, bool hasImage)
        {
            // Match against outbound invoice, too

            OutboundInvoice invoice = OutboundInvoice.FromReference(reference);

            // TODO: Verify that invoice is not already closed; if so, issue refund

            // TODO: Verify correct amount

            invoice.Open = false;

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoicePaid, 0, group.Organization.Identity,
                                 Geography.RootIdentity, 0, invoice.Identity, string.Empty);

            return FromIdentity (PirateDb.GetDatabaseForWriting().CreatePayment(group.Identity, amount, reference, fromAccount, key, hasImage,
                                                 invoice.Identity));
        }

        public static Payment CreateSingle (Organization organization, DateTime dateTime, Currency currency, Int64 amountCents, OutboundInvoice invoice, Person createdByPerson)
        {
            // TODO: Verify that invoice is not already closed; if so, issue refund

            // TODO: Verify correct amount

            invoice.SetPaid();

            PaymentGroup group = PaymentGroup.Create(organization, dateTime, currency, createdByPerson);
            Payment newPayment = FromIdentity(PirateDb.GetDatabaseForWriting().CreatePayment(group.Identity, amountCents, string.Empty, string.Empty, string.Empty, false,
                                                 invoice.Identity));
            group.AmountCents = amountCents;

            return newPayment;
        }

        private PaymentInformationList informationList;

        private void PopulateInformation()
        {
            if (informationList == null)
            {
                informationList = PaymentInformationList.ForPayment(this);  // caution: assumes the list is static over lifetime of the payment object
            }
        }

        private string GetSinglePaymentInformation (PaymentInformationType type)
        {
            PopulateInformation();

            foreach (PaymentInformation infoPiece in informationList)
            {
                if (infoPiece.Type == type)
                {
                    return infoPiece.Data;
                }
            }

            return null;
        }

        public string[] PaymentInformation
        {
            get
            {
                List<string> result = new List<string>();

                PopulateInformation();

                foreach (PaymentInformation infoPiece in informationList)
                {
                    if (infoPiece.Type == PaymentInformationType.Freeform)
                    {
                        result.Add(infoPiece.Data);
                    }
                }

                return result.ToArray();
            }
        }

        public string PayerName
        {
            get { return GetSinglePaymentInformation(PaymentInformationType.Name); }
        }

        public string PayerStreet
        {
            get { return GetSinglePaymentInformation(PaymentInformationType.Street); }
        }

        public string PayerPostalCode
        {
            get { return GetSinglePaymentInformation(PaymentInformationType.PostalCode); }
        }

        public string PayerCity
        {
            get { return GetSinglePaymentInformation(PaymentInformationType.City); }
        }

        public string PayerOrganizationNumber
        {
            get { return GetSinglePaymentInformation(PaymentInformationType.OrgNumber); }
        }

        public void AddInformation (PaymentInformationType type, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                PirateDb.GetDatabaseForWriting().CreatePaymentInformation(this.Identity, type, data);
            }
        }

        public PaymentGroup Group
        {
            get { return PaymentGroup.FromIdentity(this.PaymentGroupId); }
        }

        public void Refund (Person refundingPerson)
        {
            Financial.Refund.Create(this, refundingPerson);
        }

        public void Refund (Int64 amountCents, Person refundingPerson)
        {
            Financial.Refund.Create(this, refundingPerson, amountCents);
        }

        public OutboundInvoice OutboundInvoice
        {
            get
            {
                return Financial.OutboundInvoice.FromIdentity(this.OutboundInvoiceId);
            }
        }
    }
}
