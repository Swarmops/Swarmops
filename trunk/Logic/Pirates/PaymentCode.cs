using System;
using Activizr.Logic.Security;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    public class PaymentCode : BasicPaymentCode
    {
        private PaymentCode (BasicPaymentCode basic)
            : base(basic)
        {
            // empty
        }

        public static PaymentCode CreateFromPhone (string phoneNumber)
        {
            return Create(phoneNumber, 0);
        }

        public static PaymentCode CreateFromPerson (Person person)
        {
            return CreateFromPerson(person.Identity);
        }

        public static PaymentCode CreateFromPerson (int personId)
        {
            return Create(string.Empty, personId);
        }

        public static PaymentCode FromBasic (BasicPaymentCode basic)
        {
            return new PaymentCode(basic);
        }

        public static PaymentCode FromCode (string paymentCode)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetPaymentCode(paymentCode));
        }


        private static PaymentCode Create (string phoneNumber, int personId)
        {
            bool success = false;

            while (!success)
            {
                string randomCode = CreateRandomCode(5);

                try
                {
                    int codeId = PirateDb.GetDatabaseForWriting().CreatePaymentCode(randomCode, phoneNumber, personId);
                    success = true;
                    return FromBasic(PirateDb.GetDatabaseForReading().GetPaymentCode(randomCode));
                }
                catch (Exception)
                {
                    // If the creation throws, we had a duplicate code. Re-randomize and try again.

                    // TODO: Set a maximum attempt count, like 1E5 or something like that.
                }
            }

            return null; // dummy code path - we will never get here, but suppresses warning
        }


        public void Claim (Person claimingPerson)
        {
            Claim(claimingPerson.Identity);
        }

        public void Claim (int claimingPersonId)
        {
            PirateDb.GetDatabaseForWriting().ClaimPaymentCode(Identity, claimingPersonId);
        }

        private static string CreateRandomCode (int length)
        {
            return Authentication.CreateRandomPassword(length);
        }
    }
}