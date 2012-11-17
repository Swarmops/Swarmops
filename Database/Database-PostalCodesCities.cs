using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Activizr.Basic.Types;
using Activizr.Basic.Enums;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        private const string cityFieldSequence =
            " CityId, CountryId, CityName, GeographyId " +
            " FROM Cities ";


        private static BasicCity ReadCityFromDataReader (DbDataReader reader)
        {
            int cityId = reader.GetInt32(0);
            int countryId = reader.GetInt32(1);
            string cityName = reader.GetString(2);
            int geographyId = reader.GetInt32(3);

            return new BasicCity(cityId, cityName, countryId, geographyId);
        }


        public BasicCity[] GetCitiesByCountryAndPostalCode (int countryId, string postalCode)
        {
            List<int> cityIdList = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT CityId FROM PostalCodes WHERE PostalCode='" + postalCode.Replace("'", "''") +
                        "' AND CountryId=" + countryId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cityIdList.Add(reader.GetInt32(0));
                    }
                }
            }

            return GetCities(cityIdList.ToArray());
        }
        
        public Dictionary<string,BasicCity> GetCitiesPerPostalCode (int countryId)
        {
            Dictionary<string, int> postalList = new Dictionary<string, int>();
            Dictionary<int, bool> cityIdList = new Dictionary<int,bool>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT CityId, PostalCode FROM PostalCodes WHERE CountryId=" + countryId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                    postalList[ reader.GetString(1)]=reader.GetInt32(0);
                    cityIdList[ reader.GetInt32(0)]=true;
                    }
                }
            }
            
            int[] cityIds = new int[cityIdList.Count];
            cityIdList.Keys.CopyTo(cityIds,0);
            BasicCity[] cityArr = GetCities(cityIds);
            Dictionary<int,BasicCity> cities= new Dictionary<int,BasicCity>();
            foreach(BasicCity bc in cityArr)
                cities[bc.Identity]=bc;
            Dictionary<string,BasicCity> result=new Dictionary<string,BasicCity>();
            foreach(string pcode in postalList.Keys)
            {
                result[pcode]=cities[postalList[pcode]];
            }
            return result;
        }


        public BasicCity[] GetCities (int[] cityIds)
        {
            if (cityIds.Length == 0)
            {
                return new BasicCity[0];
            }

            List<BasicCity> result = new List<BasicCity>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + cityFieldSequence + " WHERE CityId in (" + JoinIds(cityIds) + ")",
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCityFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicCity GetCity (int cityId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + cityFieldSequence + " WHERE CityId=" + cityId.ToString(),
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCityFromDataReader(reader);
                    }

                    throw new ArgumentException("No such CityId: " + cityId.ToString());
                }
            }
        }


        public BasicCity GetCityByName(string cityName, int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + cityFieldSequence + " WHERE CityName='" + cityName.Replace("'", "''") + "' AND CountryId=" + countryId.ToString(),
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCityFromDataReader(reader);
                    }

                    throw new ArgumentException("No such CityName: " + cityName);
                }
            }
        }

        public BasicCity GetCityByName(string cityName, string countryCode)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + cityFieldSequence + " WHERE CityName='" + cityName.Replace("'", "''") + "' AND CountryCode='" + countryCode.ToUpperInvariant().Replace("'", "''") + "'",
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCityFromDataReader(reader);
                    }

                    throw new ArgumentException("No such CityName: " + cityName);
                }
            }
        }

        public BasicCity[] GetCitiesByName(string cityName, int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + cityFieldSequence + " WHERE CityName='" + cityName.Replace("'", "''") + "' AND CountryId=" + countryId.ToString(),
                                                 connection);
                List<BasicCity> resList = new List<BasicCity>();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resList.Add(ReadCityFromDataReader(reader));
                        while (reader.Read())
                        {
                            resList.Add(ReadCityFromDataReader(reader));
                        }
                        return resList.ToArray();
                    }
                    throw new ArgumentException("No such CityName: " + cityName);
                }
            }
        }

        public BasicCity[] GetCitiesByCountry(string countryCode)
        {
            BasicCountry country = GetCountry(countryCode);

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + cityFieldSequence + " WHERE CountryId=" + country.CountryId.ToString(),
                                                 connection);
                List<BasicCity> result = new List<BasicCity>();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.Add(ReadCityFromDataReader(reader));
                        while (reader.Read())
                        {
                            result.Add(ReadCityFromDataReader(reader));
                        }
                        return result.ToArray();
                    }
                    throw new ArgumentException("Cities not in place yet for country: " + countryCode);
                }
            }
        }

        public int CreateCity(string cityName, int countryId, int geographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateCity", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "cityName", cityName);
                AddParameterWithName(command, "countryId", countryId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "comment", string.Empty);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int CreatePostalCode(string postalCode, int cityId, int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePostalCode", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "postalCode", postalCode);
                AddParameterWithName(command, "cityId", cityId);
                AddParameterWithName(command, "countryId", countryId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


    }
}