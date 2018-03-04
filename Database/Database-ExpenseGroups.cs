using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Security;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string expenseClaimGroupFieldSequence =
            " ExpenseClaimGroups.ExpenseClaimGroupId, ExpenseClaimGroups.CreatedDateTime, ExpenseClaimGroups.CreatedByPersonId, ExpenseClaimGroups.Open, ExpenseClaimGroupTypes.Name, " + // 0-4
            " ExpenseClaimGroups.ExpenseClaimGroupData " +  // 5
            " FROM ExpenseClaimGroups JOIN ExpenseClaimGroupTypes ON (ExpenseGlaimGroupTypes.ExpenseClaimGroupTypeId=ExpenseClaimGroups.ExpenseClaimGroupTypeId) ";

        private static BasicExpenseClaimGroup ReadExpenseClaimGroupFromDataReader(DbDataReader reader)
        {
            int expenseClaimGroupId = reader.GetInt32(0);
            DateTime createdDateTime = reader.GetDateTime(1);
            int createdByPersonId = reader.GetInt32(2);
            bool open = reader.GetInt32(3) != 0 ? true : false;
            string groupTypeName = reader.GetString(4);
            string groupData = reader.GetString(5);

            ExpenseClaimGroupType groupType = (ExpenseClaimGroupType)Enum.Parse(typeof(ExpenseClaimGroupType), groupTypeName);

            return new BasicExpenseClaimGroup(expenseClaimGroupId, createdDateTime, createdByPersonId, open, groupType, groupData);
        }

        #endregion

        public BasicExpenseClaimGroup GetExpenseClaimGroup(int expenseClaimGroupId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + expenseClaimGroupFieldSequence + " WHERE ExpenseClaimGroups.ExpenseClaimGroupId= " + expenseClaimGroupId.ToString(CultureInfo.InvariantCulture),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExpenseClaimGroupFromDataReader(reader);
                    }

                    throw new ArgumentException("No such ExpenseClaimGroupId");
                }
            }
        }

        public BasicExpenseClaimGroup[] GetExpenseClaimGroups(params object[] conditions)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                List<BasicExpenseClaimGroup> result = new List<BasicExpenseClaimGroup>();
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + expenseClaimGroupFieldSequence + ConstructWhereClause("ExpenseClaimGroups", conditions),
                    connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadExpenseClaimGroupFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }



        /*
         *   IN organizationId INT,
  IN dateTime DATETIME,
  IN createdByPersonId INT,
  IN expenseClaimGroupType VARCHAR(64),
  IN expenseClaimGroupData TEXT
         */
        public int CreateExpenseClaimGroup(int organizationId, int personId, ExpenseClaimGroupType groupType, string groupData)
        {
            DateTime utcNow = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateExpenseClaimGroup", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "createdByPersonId", personId);
                AddParameterWithName(command, "dateTime", utcNow);
                AddParameterWithName(command, "expenseClaimGroupType", groupType.ToString());
                AddParameterWithName(command, "expenseClaimGroupData", groupData);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetExpenseClaimGroupClosed(int expenseClaimGroupId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimGroupClosed", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimGroupId", expenseClaimGroupId);
                command.ExecuteNonQuery();
            }
        }
    }
}
 