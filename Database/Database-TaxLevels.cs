using System;
using System.Data;
using System.Data.Common;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Record reading - SELECT statements

        public double GetSalaryTaxLevel (int countryId, int taxLevelId, int grossSalary)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                int thisYear = DateTime.Today.Year; // TODO: Fiscal years may be different in some weird countries

                // TODO: Adapt currency if the operating currency is different from the tax currency? Or should that be in the logic layer?

                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        String.Format (
                            "SELECT Tax,Year FROM SalaryTaxLevels WHERE CountryId={0} AND TaxLevelId={1} AND BracketLow<={2} AND Year<={3} ORDER BY BracketLow Desc LIMIT 1",
                            countryId, taxLevelId, grossSalary, thisYear), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int year = reader.GetInt32 (1);

                        if (thisYear != year)
                        {
                            // TODO: Send warning somewhere that non-current data was used for tax deduction, which is usually ok for
                            // TODO: a very limited time, but not in a sustained situation
                        }

                        int taxLevel = reader.GetInt32 (0);

                        if (taxLevel < 0)
                        {
                            return -taxLevel/10000.0; // -4500 means 45% and should be returned as .45
                        }

                        return taxLevel;
                    }

                    throw new ArgumentException ("Can't find tax rate for level/country: " + taxLevelId + "/" +
                                                 countryId);
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public void CreateSalaryTaxLevel (int countryId, int taxLevelId, int bracketLow, int year, double tax)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateSalaryTaxLevel", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "countryId", countryId);
                AddParameterWithName (command, "taxLevelId", taxLevelId);
                AddParameterWithName (command, "bracketLow", bracketLow);
                AddParameterWithName (command, "year", year);
                AddParameterWithName (command, "tax", tax < 1.0 ? (int) (-tax*10000) : (int) tax);

                command.ExecuteNonQuery();
            }
        }


        [Obsolete ("We're using a new Year column rather than deleting obsolete tax data", false)]
        public void DeleteTaxLevels (int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("DeleteTaxLevels", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "countryId", countryId);

                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}