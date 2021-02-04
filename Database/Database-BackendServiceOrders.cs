using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.System;
using Swarmops.Common.Exceptions;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string backendServiceOrderFieldSequence =
            " BackendServiceOrders.BackendServiceOrderId, BackendServiceOrders.CreatedDateTime, BackendServiceOrders.OrganizationId, BackendServiceOrders.PersonId, BackendServiceClasses.Name, " + // 0-4
            " BackendServiceOrders.OrderXml, BackendServiceOrders.Open, BackendServiceOrders.Active, BackendServiceOrders.StartedDateTime, BackendServiceOrders.ClosedDateTime, " + // 5-9
            " BackendServiceOrders.ExceptionText " + // 10
            " FROM BackendServiceOrders JOIN BackendServiceClasses ON (BackendServiceOrders.BackendServiceClassId=BackendServiceClasses.BackendServiceClassId) ";

        private static BasicBackendServiceOrder ReadBackendServiceOrderFromDataReader(IDataRecord reader)
        {
            int backendServiceOrderId = reader.GetInt32(0);
            DateTime createdDateTime = reader.GetDateTime(1);
            int organizationId = reader.GetInt32(2);
            int personId = reader.GetInt32(3);
            string backendServiceClassName = reader.GetString(4);

            string orderXml = reader.GetString(5);
            bool open = reader.GetBoolean(6);
            bool active = reader.GetBoolean(7);
            DateTime startedDateTime = reader.GetDateTime(8);
            DateTime closedDateTime = reader.GetDateTime(9);

            string exceptionText = reader.GetString(10);

            return new BasicBackendServiceOrder(backendServiceOrderId, organizationId, personId, backendServiceClassName,
                orderXml, open, active, createdDateTime, startedDateTime, closedDateTime, exceptionText);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicBackendServiceOrder GetBackendServiceOrder(int currencyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + backendServiceOrderFieldSequence + " WHERE BackendServiceOrderId=" + currencyId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadBackendServiceOrderFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Backend Service Order Id: " + currencyId);
                }
            }
        }


        public BasicBackendServiceOrder[] GetBackendServiceOrders(params object[] conditions)
        {
            return GetBackendServiceOrderBatch(10000, conditions);  // 10k is an arbitrary "everything"
        }

        public BasicBackendServiceOrder[] GetBackendServiceOrderBatch(int batchMaxSize, params object[] conditions)
        {
            List<BasicBackendServiceOrder> result = new List<BasicBackendServiceOrder>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("DEBUG: " + connection.ConnectionString);  // TODO/HACK: Temporary debug code
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Before Open");
                connection.Open();
                Console.WriteLine("Between Open and GetDbCommand");

                string sql = "SELECT " + backendServiceOrderFieldSequence +
                             ConstructWhereClause("BackendServiceOrders", conditions) +
                             " ORDER BY BackendServiceOrderId LIMIT " +
                             batchMaxSize.ToString(CultureInfo.InvariantCulture);

                // this ORDERBY + LIMIT gets the oldest first in a FIFO queue

                DbCommand command = GetDbCommand(sql, connection);
                Console.WriteLine("Between GetDbCommand and ExecuteReader");

                using (DbDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Between ExecuteReader and reader.Read");
                    while (reader.Read())
                    {
                        result.Add(ReadBackendServiceOrderFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #region Creation and manipulation - stored procedures

        public int CreateBackendServiceOrder(string backendServiceClassName, string orderXml, int organizationId = 0, int personId = 0)
        {
            DateTime now = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateBackendServiceOrder", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "nowUtc", now);
                AddParameterWithName(command, "backendServiceClassName", backendServiceClassName);
                AddParameterWithName(command, "orderXml", orderXml);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "personId", personId);


                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetBackendServiceOrderActive(int backendServiceOrderId)
        {
            DateTime now = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetBackendServiceOrderActive", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "backendServiceOrderId", backendServiceOrderId);
                AddParameterWithName(command, "nowUtc", now);

                if (Convert.ToInt32(command.ExecuteScalar()) != 1) // returns count of rows updated
                {
                    throw new DatabaseConcurrencyException();
                }
            }
        }

        public void SetBackendServiceOrderClosed(int backendServiceOrderId)
        {
            DateTime now = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetBackendServiceOrderClosed", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "backendServiceOrderId", backendServiceOrderId);
                AddParameterWithName(command, "nowUtc", now);

                if (Convert.ToInt32(command.ExecuteScalar()) != 1) // returns count of rows updated
                {
                    throw new DatabaseConcurrencyException();
                }
            }
        }


        public void SetBackendServiceOrderException (int backendServiceOrderId, Exception exception)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetBackendServiceOrderExceptionText", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "backendServiceOrderId", backendServiceOrderId);
                AddParameterWithName(command, "exceptionText", exception.ToString());

                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}