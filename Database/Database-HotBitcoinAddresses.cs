using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Governance;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string hotBitcoinAddressFieldSequence =
            " HotBitcoinAddressId,OrganizationId,DerivationPath,AddressString,BalanceSatoshis," + // 0-4
            " ThroughputSatoshis " + // 5
            " FROM HotBitcoinAddresses ";

        private const string hotBitcoinAddressUnspentFieldSequence =
            " HotBitcoinAddressUnspentId,TransactionHash,TransactionOutputIndex,AmountSatoshis,ConfirmationCount" + // 0-4
            " FROM HotBitcoinAddressUnspents ";

        private BasicHotBitcoinAddress ReadHotBitcoinAddressFromDataReader(IDataRecord reader)
        {
            int hotBitcoinAddressId = reader.GetInt32 (0);
            int organizationId = reader.GetInt32 (1);
            string derivationPath = reader.GetString (2);
            string addressString = reader.GetString (3);
            Int64 balanceSatoshis = reader.GetInt64 (4);
            Int64 throughputSatoshis = reader.GetInt64 (5);

            return new BasicHotBitcoinAddress (hotBitcoinAddressId, organizationId, derivationPath, addressString, balanceSatoshis, throughputSatoshis);
        }

        private BasicHotBitcoinAddressUnspent ReadHotBitcoinAddressUnspentFromDataReader(IDataRecord reader)
        {
            int hotBitcoinAddressUnspentId = reader.GetInt32 (0);
            string transactionHash = reader.GetString (1);
            int transactionOutputIndex = reader.GetInt32 (2);
            Int64 amountSatoshis = reader.GetInt64 (3);
            int confirmationCount = reader.GetInt32 (4);

            return new BasicHotBitcoinAddressUnspent (hotBitcoinAddressUnspentId, transactionHash, transactionOutputIndex, amountSatoshis, confirmationCount);
        }

        #endregion

        #region Record reading code

        public BasicHotBitcoinAddress GetHotBitcoinAddress (int hotBitcoinAddressId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + hotBitcoinAddressFieldSequence + "WHERE HotBitcoinAddressId=" + hotBitcoinAddressId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadHotBitcoinAddressFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown HotBitcoinAddress Id: " + hotBitcoinAddressId);
                }
            }
        }

        public BasicHotBitcoinAddress[] GetHotBitcoinAddresses(params object[] conditions) // the typical condition would be an Organization, here
        {
            List<BasicHotBitcoinAddress> result = new List<BasicHotBitcoinAddress>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + hotBitcoinAddressFieldSequence + ConstructWhereClause("HotBitcoinAddresses", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadHotBitcoinAddressFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicHotBitcoinAddressUnspent[] GetHotBitcoinAddressUnspents(params object[] conditions) // the typical condition would be a HotBitcoinAddress, here
        {
            List<BasicHotBitcoinAddressUnspent> result = new List<BasicHotBitcoinAddressUnspent>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + hotBitcoinAddressUnspentFieldSequence + ConstructWhereClause("HotBitcoinAddressUnspents", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadHotBitcoinAddressUnspentFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }



        #endregion

        #region Creation and manipulation

        public int CreateHotBitcoinAddressConditional(int organizationId, string derivationPath, string address)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                using (DbCommand command = GetDbCommand("CreateHotBitcoinAddressConditional", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "organizationId", organizationId);
                    AddParameterWithName(command, "derivationPath", derivationPath);
                    AddParameterWithName(command, "addressString", address);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int CreateHotBitcoinAddressUnspentConditional(int hotBitcoinAddressId, string transactionHash, int transactionOutputIndex, Int64 amountSatoshis, int confirmationCount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                using (DbCommand command = GetDbCommand("CreateHotBitcoinAddressUnspentConditional", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "hotBitcoinAddressId", hotBitcoinAddressId);
                    AddParameterWithName(command, "transactionHash", transactionHash);
                    AddParameterWithName(command, "transactionOutputIndex", transactionOutputIndex);
                    AddParameterWithName(command, "amountSatoshis", amountSatoshis);
                    AddParameterWithName(command, "confirmationCount", confirmationCount);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        #endregion
    }
}
