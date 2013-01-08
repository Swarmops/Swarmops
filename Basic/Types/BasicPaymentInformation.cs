using System;
using System.Collections.Generic;
using System.Text;


namespace Swarmops.Basic.Types
{
    public class BasicPaymentInformation
    {
        public BasicPaymentInformation (PaymentInformationType type, string data)
        {
            this.Type = type;
            this.Data = data;
        }

        public BasicPaymentInformation (BasicPaymentInformation original)
            : this (original.Type, original.Data)
        {
            // empty copy ctor
        }

        public PaymentInformationType Type { get; private set; }
        public string Data { get; private set; }
    }


    public enum PaymentInformationType
    {
        Unknown = 0,
        /// <summary>
        /// The payer's name as supplied by the bank.
        /// </summary>
        Name,
        /// <summary>
        /// The payer's street as and if supplied by the bank.
        /// </summary>
        Street,
        /// <summary>
        /// The payer's postal code as and if supplied by the bank.
        /// </summary>
        PostalCode,
        /// <summary>
        /// The payer's city as and if supplied by the bank.
        /// </summary>
        City,
        /// <summary>
        /// The payer's org# or person# as and if supplied by the bank.
        /// </summary>
        OrgNumber,
        /// <summary>
        /// Freeform information supplied by payer.
        /// </summary>
        Freeform,
        /// <summary>
        /// The payer's country as and if supplied by the bank.
        /// </summary>
        Country,
        /// <summary>
        /// The payer's country code as and if supplied by the bank.
        /// </summary>
        CountryCode,
        /// <summary>
        /// Payer's email as and if supplied by the bank.
        /// </summary>
        Email,
        /// <summary>
        /// Information needed to perform a future refund.
        /// </summary>
        RefundInformation
    }
}
