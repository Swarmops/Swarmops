using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Resources;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class PaymentTransferInfo
    {
        /// <summary>
        /// This is the currency to be used for the transfer. Note that the amount will be converted into this currency!
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// The Country field is needed whenever a transfer is marked as "domestic"
        /// </summary>
        public Country Country { get; set; }
        public string Recipient { get; set; }
        public string Reference { get; set; }
        public string CurrencyAmount { get; set; }
        public PaymentTargetType TargetType { get; set; }
        public string LocalizedPaymentMethodName { get; set; }
        public bool OcrAvailable { get; set; } // defaults to false which is fine
        public string[] OcrData { get; set; }
        public Dictionary<string,string> LocalizedPaymentInformation { get; set; }

        public static PaymentTransferInfo FromObject(IHasIdentity financialObject, Money amountToPay = null)
        {
            if (financialObject is ExpenseClaim)
            {
                ExpenseClaim claim = financialObject as ExpenseClaim;

                if (amountToPay == null)
                {
                    return FromObject(claim.Claimer, new Money(claim.AmountCents, claim.Organization.Currency));
                }
                else
                {
                    return FromObject(claim.Claimer, amountToPay);
                }
            }

            if (financialObject is CashAdvance)
            {
                CashAdvance advance = financialObject as CashAdvance;

                if (amountToPay == null)
                {
                    return FromObject(advance.Person, new Money(advance.AmountCents, advance.Organization.Currency));
                }
                else
                {
                    return FromObject(advance.Person, amountToPay);
                }
            }

            if (financialObject is Salary)
            {
                Salary salary = financialObject as Salary;
                return FromObject(salary.PayrollItem.Person, new Money(salary.NetSalaryCents, salary.PayrollItem.Organization.Currency));
            }

            if (financialObject is InboundInvoice)
            {
                // TODO: Create Supplier object, read from that instead

                InboundInvoice invoice = financialObject as InboundInvoice;
                PaymentTransferInfo result = new PaymentTransferInfo();

                // For now, do a switch on the pay-to-account string, before the payment targets are implemented
                // on invoices and expense claims:

                result.LocalizedPaymentInformation = new Dictionary<string, string>();

                if (invoice.PayToAccount.StartsWith("IBAN"))
                {
                    result.TargetType = PaymentTargetType.InternationalBankTransfer;

                    result.LocalizedPaymentInformation[
                        Logic_Financial_PaymentTransferInfo.PaymentTargetField_Iban] =
                            invoice.PayToAccount.Substring(5);

                    // this should have bic too but we don't have that information before payment targets
                    // are properly implemented
                }
                else if (invoice.PayToAccount.StartsWith("SEBG"))
                {
                    result.TargetType = PaymentTargetType.DomesticBankGiro; // for now
                    result.LocalizedPaymentInformation[
                        Logic_Financial_PaymentTransferInfo.PaymentTargetField_GiroNumber] =
                        invoice.PayToAccount.Substring(5);

                    // Check for SEBG OCR availability

                    if (invoice.InvoiceReference.Length > 2 && Formatting.CheckLuhnChecksum(invoice.InvoiceReference) &&
                        Formatting.CheckLuhnChecksum(invoice.PayToAccount))
                    {
                        result.OcrAvailable = true;
                        result.OcrData = new string[3];
                        result.OcrData[0] = invoice.InvoiceReference + " #";
                        result.OcrData[1] = string.Format("{0} {1:00}   {2} >", // three spaces between the cents and the checksum
                            invoice.AmountCents / 100, invoice.AmountCents % 100,
                            Formatting.GetLuhnChecksum(invoice.AmountCents.ToString(CultureInfo.InvariantCulture)));
                        result.OcrData[2] = Formatting.CleanNumber(invoice.PayToAccount) + "#41#";
                    }
                }
                else
                {
                    // Assume domestic giro in lack of other information

                    result.TargetType = PaymentTargetType.DomesticBankGiro;
                    result.LocalizedPaymentInformation[
                        Logic_Financial_PaymentTransferInfo.PaymentTargetField_GiroNumber] =
                        invoice.PayToAccount; // the whole string, as opposed to case above
                }

                result.LocalizedPaymentMethodName =
                    Logic_Financial_PaymentTransferInfo.ResourceManager.GetString("PaymentTargetType_" +
                                                                        result.TargetType.ToString());
                if (invoice.HasNativeCurrency)
                {
                    result.Currency = invoice.NativeCurrencyAmount.Currency;
                    result.CurrencyAmount = result.Currency.Code + " " +
                                            (invoice.NativeCurrencyAmount.Cents/100.0).ToString("N2");
                }
                else
                {
                    result.Currency = invoice.Organization.Currency;
                    result.CurrencyAmount = result.Currency.Code + " " + (invoice.AmountCents/100.0).ToString("N2");
                }

                result.Country = null; // TODO: Set to Supplier's PaymentTarget country

                result.Recipient = invoice.Supplier;

                return result;
            }

            if (financialObject is Person)
            {
                Person person = financialObject as Person;

                // For now, assume the bank/clearing/account triplet
                // TODO: Implement payment targets on teh Person object

                PaymentTransferInfo result = new PaymentTransferInfo();
                result.TargetType = PaymentTargetType.DomesticBankTransfer;

                // We must know the amount at this point

                if (amountToPay == null)
                {
                    throw new ArgumentNullException("amountToPay", @"Cannot determine payment transfer information without knowing origin currency");
                }

                if (string.IsNullOrEmpty(person.BankName) || string.IsNullOrEmpty(person.BankClearing) ||
                    string.IsNullOrEmpty(person.BankAccount))
                {
                    throw new NotImplementedException("Cannot provide payment transfer information for Person #" + person.Identity.ToString());
                }

                result.TargetType = PaymentTargetType.DomesticBankTransfer;
                result.Country = person.CountryId == 0? null: person.Country;
                result.Currency = amountToPay.Currency; // TODO: The currency will need to come from the payment method instead
                
                result.CurrencyAmount = result.Currency.Code + " " + (amountToPay.ToCurrency(result.Currency).Cents / 100.0).ToString("N2");

                result.LocalizedPaymentMethodName =
                    Logic_Financial_PaymentTransferInfo.ResourceManager.GetString("PaymentTargetType_" +
                                                                                        result.TargetType.ToString());
                result.Recipient = person.Canonical;

                result.LocalizedPaymentInformation = new Dictionary<string, string>();
                result.LocalizedPaymentInformation[
                    Logic_Financial_PaymentTransferInfo.PaymentTargetField_ClearingCode] =
                    person.BankName + " " + person.BankClearing;
                result.LocalizedPaymentInformation[
                    Logic_Financial_PaymentTransferInfo.PaymentTargetField_AccountNumber] = person.BankAccount;

                return result;
            }

            throw new NotImplementedException("Unknown payment information");
        }
    }
}
