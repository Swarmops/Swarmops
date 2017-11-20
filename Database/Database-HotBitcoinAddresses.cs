using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Governance;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string hotBitcoinAddressFieldSequence =
            " HotBitcoinAddressId,OrganizationId,BitcoinChainId,DerivationPath,AddressString," + // 0-4
            " AddressStringFallback,BalanceSatoshis,ThroughputSatoshis" + // 5-7
            " FROM HotBitcoinAddresses ";

        private const string hotBitcoinAddressUnspentFieldSequence =
            " HotBitcoinAddressUnspentId,HotBitcoinAddressId,TransactionHash,TransactionOutputIndex,AmountSatoshis," + // 0-4
            " ConfirmationCount" + // 5
            " FROM HotBitcoinAddressUnspents ";

        private BasicHotBitcoinAddress ReadHotBitcoinAddressFromDataReader(IDataRecord reader)
        {
            int hotBitcoinAddressId = reader.GetInt32 (0);
            int organizationId = reader.GetInt32 (1);
            BitcoinChain chain = (BitcoinChain) reader.GetInt32(2);
            string derivationPath = reader.GetString (3);
            string addressString = reader.GetString(4);
            string addressStringFallback = reader.GetString(5);
            Int64 balanceSatoshis = reader.GetInt64 (6);
            Int64 throughputSatoshis = reader.GetInt64 (7);

            return new BasicHotBitcoinAddress (hotBitcoinAddressId, organizationId, chain, derivationPath, addressString, addressStringFallback, balanceSatoshis, throughputSatoshis);
        }

        private BasicHotBitcoinAddressUnspent ReadHotBitcoinAddressUnspentFromDataReader(IDataRecord reader)
        {
            int hotBitcoinAddressUnspentId = reader.GetInt32 (0);
            int hotBitcoinAddressId = reader.GetInt32 (1);
            string transactionHash = reader.GetString (2);
            int transactionOutputIndex = reader.GetInt32 (3);
            Int64 amountSatoshis = reader.GetInt64 (4);
            int confirmationCount = reader.GetInt32 (5);

            return new BasicHotBitcoinAddressUnspent (hotBitcoinAddressUnspentId, hotBitcoinAddressId, transactionHash, transactionOutputIndex, amountSatoshis, confirmationCount);
        }

        #endregion

        #region Record reading code

        public BasicHotBitcoinAddress GetHotBitcoinAddress(int hotBitcoinAddressId)
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

        public BasicHotBitcoinAddress GetHotBitcoinAddress(BitcoinChain chain, string address)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + hotBitcoinAddressFieldSequence + "WHERE BitcoinChainId=" + (Int32) chain + " AND AddressString='" + SqlSanitize (address) + "';", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadHotBitcoinAddressFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown HotBitcoinAddress: " + address);
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

        public BasicHotBitcoinAddressUnspent GetHotBitcoinAddressUnspent(int hotBitcoinAddressUnspentId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + hotBitcoinAddressUnspentFieldSequence + "WHERE HotBitcoinAddressUnspentId=" + hotBitcoinAddressUnspentId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadHotBitcoinAddressUnspentFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown HotBitcoinAddressUnspent Id: " + hotBitcoinAddressUnspentId);
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
                        "SELECT" + hotBitcoinAddressUnspentFieldSequence + ConstructWhereClause("HotBitcoinAddressUnspents", conditions) + " ORDER BY HotBitcoinAddressUnspentId ASC", connection);

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

        public int CreateHotBitcoinAddressConditional(int organizationId, BitcoinChain chain, string derivationPath, string address, string addressFallback = "")
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                using (DbCommand command = GetDbCommand("CreateHotBitcoinAddressConditional", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "organizationId", organizationId);
                    AddParameterWithName(command, "bitcoinChainId", (int) chain);
                    AddParameterWithName(command, "derivationPath", derivationPath);
                    AddParameterWithName(command, "addressString", address);
                    AddParameterWithName(command, "addressStringFallback", addressFallback);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int SetHotBitcoinAddressBalance (int hotBitcoinAddressId, Int64 balanceSatoshis)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                using (DbCommand command = GetDbCommand("SetHotBitcoinAddressBalance", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "hotBitcoinAddressId", hotBitcoinAddressId);
                    AddParameterWithName(command, "balanceSatoshis", balanceSatoshis);

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

        public int DeleteHotBitcoinAddressUnspent(int hotBitcoinAddressUnspentId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                using (DbCommand command = GetDbCommand("DeleteHotBitcoinAddressUnspent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "hotBitcoinAddressUnspentId", hotBitcoinAddressUnspentId);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        #endregion
    }
}
