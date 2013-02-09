using System;
using System.Data;
using System.Data.Common;


namespace Swarmops.Database
{
    public partial class SwarmDb
    {


        #region Record reading - SELECT statements

        public double GetTaxLevel(int countryId, int taxLevelId, int grossSalary)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        String.Format("SELECT Tax FROM SalaryTaxLevels WHERE CountryId={0} AND TaxLevelId={1} AND BracketLow<={2} ORDER BY BracketLow Desc LIMIT 1", countryId, taxLevelId, grossSalary), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int taxLevel = reader.GetInt32(0);

                        if (taxLevel < 0)
                        {
                            return -taxLevel/100.0; // -45 means 45% and should be returned as .45
                        }

                        return taxLevel;
                    }

                    throw new ArgumentException("Can't find tax rate for level/country: " + taxLevelId + "/" + countryId);
                }
            }
        }



        #endregion



        #region Creation and manipulation - stored procedures

        public void CreateTaxLevel (int countryId, int taxLevelId, int bracketLow, double tax)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateTaxLevel", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "countryId", countryId);
                AddParameterWithName(command, "taxLevelId", taxLevelId);
                AddParameterWithName(command, "bracketLow", bracketLow);
                AddParameterWithName(command, "tax", tax < 1.0 ? (int) (-tax*100) : (int) tax);

                command.ExecuteNonQuery();
            }
        }


        public void DeleteTaxLevels (int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("DeleteTaxLevels", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "countryId", countryId);

                command.ExecuteNonQuery();
            }
        }



        #endregion

    }

}