using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string countryFieldSequence = " CountryId,Name,Code,Culture,GeographyId " +  // 0-4
                                            " FROM Countries ";


        private static BasicCountry ReadCountryFromDataReader(DbDataReader reader)
        {
            int countryId = reader.GetInt32(0);
            string name = reader.GetString(1);
            string code = reader.GetString(2).ToUpperInvariant();
            string culture = reader.GetString(3);
            int geographyId = reader.GetInt32(4);

            return new BasicCountry(countryId, name, code, culture, geographyId);
        }

        #endregion


        #region Record reading code

        public BasicCountry GetCountry (string countryCode)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + countryFieldSequence + " WHERE Code='" + countryCode.ToUpperInvariant().Replace("'", "''") + "'",
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCountryFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Country Code");
                }
            }
        }

        public BasicCountry GetCountry (int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + countryFieldSequence + " WHERE CountryId=" + countryId.ToString(),
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCountryFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown CountryId");
                }
            }
        }

        public BasicCountry[] GetAllCountries ()
        {
            List<BasicCountry> result = new List<BasicCountry>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + countryFieldSequence + " ORDER BY Code", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCountryFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicCountry[] GetCountriesInUse ()
        {
            return GetAllCountries(); // Migrate this once Organizations has moved, too

            /*
            List<BasicCountry> result = new List<BasicCountry>();

            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT * From CountriesInUseView ORDER BY Code", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCountryFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }*/
        }

        #endregion



        public int CreateCountry (string name, string code, string culture, int geographyId, int postalCodeLength, string collation)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateCountry", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "name", name);
                AddParameterWithName(command, "code", code);
                AddParameterWithName(command, "culture", culture);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "postalCodeLength", postalCodeLength);
                AddParameterWithName(command, "collation", collation);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int SetCountryGeographyId (int countryId, int geographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetCountryGeographyId", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "countryId", countryId);
                AddParameterWithName(command, "geographyId", geographyId);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


    }
}