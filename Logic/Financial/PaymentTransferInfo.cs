using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Resources;

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
        public string CurrencyAmount { get; set; }
        public PaymentTargetType TargetType { get; set; }
        public string LocalizedPaymentMethodName { get; set; }
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
                result.Country = person.Country;
                result.Currency = amountToPay.Currency; // TODO: The currency will need to come from the payment method instead
                
                result.CurrencyAmount = result.Currency.Code + " " + (amountToPay.ToCurrency(result.Currency).Cents / 100.0).ToString("N2");

                result.LocalizedPaymentMethodName =
                    Logic_Financial_PaymentTransferInfo.ResourceManager.GetString("PaymentTargetType_" +
                                                                                        result.TargetType.ToString());

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
