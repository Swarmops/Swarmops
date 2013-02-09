using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {

        #region Database field reading

        private const string parleyAttendeeFieldSequence =
            " ParleyAttendeeId, ParleyId, PersonId, SignupDateTime, Active, " + // 0-4
            "CancelDateTime, Invoiced, OutboundInvoiceId, IsGuest" +            // 5-8
            " FROM ParleyAttendees ";

        private static BasicParleyAttendee ReadParleyAttendeeFromDataReader(IDataRecord reader)
        {
            int parleyAttendeeId = reader.GetInt32(0);
            int parleyId = reader.GetInt32(1);
            int personId = reader.GetInt32(2);
            DateTime signupDateTime = reader.GetDateTime(3);
            bool active = reader.GetBoolean(4);

            DateTime cancelDateTime = reader.GetDateTime(5);
            bool invoiced = reader.GetBoolean(6);
            int outboundInvoiceId = reader.GetInt32(7);
            bool isGuest = reader.GetBoolean(8);

            return new BasicParleyAttendee (parleyAttendeeId, parleyId, personId, signupDateTime, active, cancelDateTime, invoiced, outboundInvoiceId, isGuest);
        }


        #endregion


        #region Database record reading -- SELECT clauses


        public BasicParleyAttendee GetParleyAttendee(int parleyAttendeeId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + parleyAttendeeFieldSequence +
                    "WHERE ParleyAttendeeId=" + parleyAttendeeId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadParleyAttendeeFromDataReader(reader);
                    }

                    throw new ArgumentException("No such ParleyAttendeeId:" + parleyAttendeeId.ToString());
                }
            }

        }


        public BasicParleyAttendee[] GetParleyAttendees(params object[] conditions)
        {
            List<BasicParleyAttendee> result = new List<BasicParleyAttendee>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + parleyAttendeeFieldSequence + ConstructWhereClause("ParleyAttendees", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadParleyAttendeeFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public int[] GetParleyAttendeeOptions(int parleyAttendeeId)
        {
            List<int> result = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT ParleyOptionId FROM ParleyAttendeeOptions WHERE ParleyAttendeeId=" + parleyAttendeeId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion


        #region Creation and manipulation -- stored procedures


        public int CreateParleyAttendee(int parleyId, int personId, bool isGuest)  // ok
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateParleyAttendee", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "parleyId", parleyId);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "signupDateTime", DateTime.Now);
                AddParameterWithName(command, "isGuest", isGuest);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int SetParleyAttendeeActive(int parleyAttendeeId, bool active)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetParleyAttendeeActive", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "parleyAttendeeId", parleyAttendeeId);
                AddParameterWithName(command, "active", active);
                AddParameterWithName(command, "cancelDateTime", DateTime.Now);  // if active=false, set to cancellation time. if active=true, ignored

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int SetParleyAttendeeInvoiced(int parleyAttendeeId, int outboundInvoiceId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetParleyAttendeeInvoiced", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "parleyAttendeeId", parleyAttendeeId);
                AddParameterWithName(command, "invoiced", (outboundInvoiceId > 0 ? true : false));
                AddParameterWithName(command, "outboundInvoiceId", outboundInvoiceId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int AddParleyAttendeeOption(int parleyAttendeeId, int parleyOptionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("AddParleyAttendeeOption", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "parleyAttendeeId", parleyAttendeeId);
                AddParameterWithName(command, "parleyOptionId", parleyOptionId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        #endregion


    }
}