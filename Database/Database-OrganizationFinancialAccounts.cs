using System.Data;
using System.Data.Common;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        private const string organizationFinancialAccountsDataFieldSequence =
            " OrganizationFinancialAccountTypes.Name AS OrganizationFinancialAccountType,OrganizationFinancialAccounts.OrganizationId,OrganizationFinancialAccounts.FinancialAccountId FROM OrganizationFinancialAccounts " +
            "JOIN OrganizationFinancialAccountTypes ON (OrganizationFinancialAccounts.OrganizationFinancialAccountTypeId=OrganizationFinancialAccountTypes.OrganizationFinancialAccountTypeId) ";

        #region Select statements - reading data

        public int GetOrganizationFinancialAccountId (int organizationId,
            OrganizationFinancialAccountType organizationFinancialAccountType)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + organizationFinancialAccountsDataFieldSequence +
                        "WHERE OrganizationFinancialAccountTypes.Name='" + organizationFinancialAccountType + "' " +
                        "AND OrganizationFinancialAccounts.Organizationid=" + organizationId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // account Id is third in select list, just return it

                        return reader.GetInt32 (2);
                    }

                    // TODO: Throw instead if requested account not found?

                    return 0;
                }
            }
        }

        /*

        public int[] GetObjectsByOptionalData(ObjectType objectType, ObjectOptionalDataType dataType, string data)
        {
            List<int> objectIds = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + objectOptionalDataFieldSequence + "WHERE ObjectTypes.Name='" + objectType.ToString() + "' " +
                        "AND ObjectOptionalDataTypes.Name='" + dataType.ToString() + "' " +
                        "AND ObjectOptionalData.Data='" + data.Replace("'", "''") + "';", connection);

                // NOTE: THIS STRING QUERY IS NOT PARAMETERIZED, EVEN THOUGH IT'S IN COMMENTED OUT CODE

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        objectIds.Add(reader.GetInt32(1));
                    }

                    return objectIds.ToArray();
                }
            }
        }


        static private ObjectType GetObjectTypeForObject(IHasIdentity objectToIdentify)
        {
            //string objectTypeString = objectToIdentify.GetType().ToString();

            //// Strip namespace

            //int lastPeriodIndex = objectTypeString.LastIndexOf('.');
            //objectTypeString = objectTypeString.Substring(lastPeriodIndex + 1);

            string objectTypeString = objectToIdentify.GetType().Name;

            // At this point, we should be able to cast the string to an ObjectType enum
            try
            {
                return (ObjectType)Enum.Parse(typeof(ObjectType), objectTypeString);
            }
            catch
            {
                //That failed. This could be a subclass, try upwards.
                Type parentType = objectToIdentify.GetType().BaseType;
                while (parentType != typeof(Object))
                {
                    try
                    {
                        return (ObjectType)Enum.Parse(typeof(ObjectType), parentType.Name);
                    }
                    catch
                    {
                        parentType = parentType.BaseType;
                    }
                }
            }
            throw new InvalidCastException("GetObjectTypeForObject could not identify object of type " + objectToIdentify.GetType().Name);
        }*/

        #endregion

        #region Stored procedures for modifying data

        public void SetOrganizationFinancialAccountId (int organizationId, OrganizationFinancialAccountType accountType,
            int financialAccountId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOrganizationFinancialAccount", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "organizationFinancialAccountTypeName", accountType.ToString());
                AddParameterWithName (command, "financialAccountId", financialAccountId);

                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}