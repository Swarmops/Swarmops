using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string financialValidationFieldSequence =
            " FinancialValidationId,FinancialValidationTypes.Name AS ValidationType,FinancialDependencyTypes.Name AS DependencyType,ForeignId,ValidatedDateTime," +  // 0-4
            " PersonId,Amount " +  // 5-6
            " FROM FinancialValidations JOIN FinancialValidationTypes ON (FinancialValidations.TypeId=FinancialValidationTypes.FinancialValidationTypeId) " +
            " JOIN FinancialDependencyTypes USING (FinancialDependencyTypeId)";

        private const string financialValidationOrder = " ORDER BY FinancialValidationId ";

        private static BasicFinancialValidation ReadFinancialValidationFromDataReader(IDataRecord reader)
        {
            int financialValidationId = reader.GetInt32(0);
            FinancialValidationType validationType =
                (FinancialValidationType)(Enum.Parse(typeof(FinancialValidationType), reader.GetString(1)));
            FinancialDependencyType dependencyType =
                (FinancialDependencyType)(Enum.Parse(typeof(FinancialDependencyType), reader.GetString(2)));
            int foreignId = reader.GetInt32(3);
            DateTime dateTime = reader.GetDateTime(4);
            int personId = reader.GetInt32(5);
            double amount = reader.GetDouble(6); // not yet used

            return new BasicFinancialValidation(financialValidationId, validationType, personId, dateTime, dependencyType, foreignId);
        }

        #endregion



        #region Record reading - SELECT statements

        public BasicFinancialValidation[] GetFinancialValidations(FinancialDependencyType dependencyType, int foreignId)
        {
            List<BasicFinancialValidation> result = new List<BasicFinancialValidation>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + financialValidationFieldSequence + " WHERE FinancialDependencyTypes.Name='" + dependencyType.ToString() + "' AND ForeignId=" + foreignId.ToString() + " " + financialValidationOrder, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialValidationFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion


    }
}