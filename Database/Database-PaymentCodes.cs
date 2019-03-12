using System;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public BasicPaymentCode GetPaymentCode (string paymentCode)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "select PaymentCodes.*, PhoneNumbers.PhoneNumber AS IssuedToPhoneNumber FROM PaymentCodes, PhoneNumbers where PaymentCodes.IssuedToPhoneId=PhoneNumbers.PhoneNumberId AND PaymentCodes.PaymentCode=@paymentCode", connection);

                AddParameterWithName(command, "paymentCode", paymentCode);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadPaymentCodeFromDataReader (reader);
                    }

                    throw new ArgumentException ("No such payment code");
                }
            }
        }


        private BasicPaymentCode ReadPaymentCodeFromDataReader (DbDataReader reader)
        {
            int issuedToPersonId = (int) reader["IssuedToPersonId"];
            string issuedToPhoneNumber = (string) reader["IssuedToPhoneNumber"];
            int paymentCodeId = (int) reader["PaymentCodeId"];
            string paymentCode = (string) reader["PaymentCode"];
            bool claimed = (bool) reader["Claimed"];

            return new BasicPaymentCode (paymentCodeId, paymentCode, issuedToPhoneNumber, issuedToPersonId, claimed);
        }

        public int CreatePaymentCode (string paymentCode, string issuingPhoneNumber, int issuingPersonId)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreatePaymentCode", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "personId", issuingPersonId);
                AddParameterWithName (command, "phoneNumber", issuingPhoneNumber);
                AddParameterWithName (command, "paymentCode", paymentCode);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int ClaimPaymentCode (int paymentCodeId, int claimingPersonId)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("ClaimPaymentCode", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "paymentCodeId", paymentCodeId);
                AddParameterWithName (command, "personId", claimingPersonId);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }
    }
}