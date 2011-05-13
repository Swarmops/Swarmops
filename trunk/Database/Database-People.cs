using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string personFieldSequence =
            " PersonId, PasswordHash, Name, Email, Street, " + // 0-4
            " PostalCode, City, CountryId, PhoneNumber, GeographyId, " + // 5-9
            " Birthdate, GenderId " + // 10-11
            " FROM People ";

        private static BasicPerson ReadPersonFromDataReader (DbDataReader reader)
        {
            int personId = reader.GetInt32(0);
            string passwordHash = reader.GetString(1);
            string name = reader.GetString(2);
            string email = reader.GetString(3);
            string street = reader.GetString(4);
            string postalCode = reader.GetString(5);
            string cityName = reader.GetString(6);
            int countryId = reader.GetInt32(7);
            string phone = reader.GetString(8);
            int geographyId = reader.GetInt32(9);
            DateTime birthdate = reader.GetDateTime(10);
            int genderId = reader.GetInt32(11);


            // Fix broken names, emails

            email = email.ToLower().Trim();
            name = name.Trim();

            while (name.Contains("  "))
            {
                name = name.Replace("  ", " ");
            }


            return new BasicPerson(personId, passwordHash, name, email, street, postalCode, cityName, countryId, phone,
                                   geographyId, birthdate, (PersonGender)genderId);
        }

        #endregion



        #region Retrieving lists of people based on wildcards or partial data

        public BasicPerson[] GetPeopleFromNamePattern (string namePattern)
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE Name like '" + namePattern.Replace("'", "''") + "'",
                        connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromBirthdate (DateTime fromdate, DateTime todate)
        {
            // Is this really valid for MySQL? Yes .net connector makes @parameter substitutions

            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE BirthDate >= @fromdate AND BirthDate < @todate",
                        connection);

                AddParameterWithName(command, "fromdate", fromdate);
                AddParameterWithName(command, "todate", todate);

                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromEmailPattern (string emailPattern)
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE Email LIKE '" + emailPattern.Replace("'", "''") + "'",
                        connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromCityPattern (string cityPattern)
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE City like '" + cityPattern.Replace("'", "''") + "'",
                        connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromPostalCodePattern (string pcPattern)
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE PostalCode like '" + pcPattern.Replace("'", "''") + "'",
                        connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromPostalCodes (string[] postalCodes)
        {
            List<BasicPerson> result = new List<BasicPerson>();
            for (int i = 0; i < postalCodes.Length; ++i)
                postalCodes[i] = postalCodes[i].Replace("'", "''");

            string postalCodesString = ("'" + string.Join("','", postalCodes) + "'").Replace(" ", "");
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE TRIM(PostalCode) In (" + postalCodesString + ")",
                        connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromEmail (string email)
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + personFieldSequence + " WHERE Email='" + email.Replace("'", "''").Trim() + "'",
                                 connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }




        public BasicPerson[] GetPeopleFromPhoneNumber (int countryId, string phoneNumber)
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + personFieldSequence + " WHERE CountryId=" + countryId.ToString() + " AND PhoneNumber='" +
                        phoneNumber.Replace("'", "''") + "'", connection);
                command.CommandTimeout = 300;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicPerson[] GetPeopleFromPhoneNumber (string countryCode, string phoneNumber)
        {
            BasicCountry country = GetCountry(countryCode);

            return GetPeopleFromPhoneNumber(country.Identity, phoneNumber);
        }

        #endregion

        #region Retrieving specific people or lists of people

        public BasicPerson[] GetPeople (int[] personIds)
        {
            if (personIds == null || personIds.Length == 0)
            {
                return new BasicPerson[0];
            }


            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + personFieldSequence + " WHERE PersonId in (" + JoinIds(personIds) + ")", connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public BasicPerson[] GetAllPeople ()
        {
            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + personFieldSequence, connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                { 
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public BasicPerson GetPerson (int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + personFieldSequence + " WHERE PersonId=" + personId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Parse the person

                        BasicPerson person = ReadPersonFromDataReader(reader);

                        return person;
                    }
                    else
                    {
                        // A special case here for developers who don't have the registry of people, but lots and lots of links to people in financi

                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            return new BasicPerson(personId, string.Empty, "PERSON #" + personId.ToString() + " NOT IN DEV DB", "noreply@example.com", "Foobar 12", "12345", "Duckville", 1, "", 1, new DateTime (1972, 1, 21), PersonGender.Unknown);
                        }
                        throw new ArgumentException("No such PersonId: " + personId.ToString());
                    }
                }
            }
        }

        #endregion

        public Dictionary<int, int> GetPeopleGeographies (int[] personIds)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            if (personIds == null || personIds.Length == 0)
            {
                return result;
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId, GeographyId From People WHERE PersonId in (" + JoinIds(personIds) + ")",
                        connection);
                command.CommandTimeout = 120;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0), reader.GetInt32(1));
                    }
                }
            }

            return result;
        }


        public BasicPerson[] GetPeopleInGeographies (int[] geographyIds)
        {
            if (geographyIds.Length == 0)
            {
                return new BasicPerson[0];
            }

            List<BasicPerson> result = new List<BasicPerson>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                DbCommand command =
                    GetDbCommand("SELECT " + personFieldSequence + " WHERE GeographyId IN (" + JoinIds(geographyIds) + ")",
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }



        public void SetPersonName (int personId, string name)
        {
            this.SetPersonBasicStringValue(personId, "Name", name);
        }

        public void SetPersonPasswordHash (int personId, string passwordHash)
        {
            this.SetPersonBasicStringValue(personId, "PasswordHash", passwordHash);
        }

        public void SetPersonStreet (int personId, string street)
        {
            this.SetPersonBasicStringValue(personId, "Street", street);
        }

        public void SetPersonPostalCode (int personId, string postalCode)
        {
            this.SetPersonBasicStringValue(personId, "PostalCode", postalCode);
        }

        public void SetPersonCity (int personId, string city)
        {
            this.SetPersonBasicStringValue(personId, "City", city);
        }

        public void SetPersonPhone (int personId, string phone)
        {
            this.SetPersonBasicStringValue(personId, "PhoneNumber", phone);
        }

        public void SetPersonEmail (int personId, string email)
        {
            this.SetPersonBasicStringValue(personId, "Email", email);
        }

        public int CreatePerson (string name, string email)
        {
            return CreatePerson(name, email, string.Empty, string.Empty, string.Empty, string.Empty, 0,
                                DateTime.MinValue, PersonGender.Unknown);
        }

        protected void SetPersonBasicStringValue (int personId, string key, string newValue)
        {
            string parameterName = Char.ToLower(key[0]) + key.Substring(1);
            string storedProcedureName = "SetPerson" + key;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, parameterName, newValue);
                command.ExecuteNonQuery();
            }
        }

        public int CreatePerson (string name, string email, string phone, string street, string postalCode, string city,
                                 int countryId, DateTime birthDate, PersonGender gender)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePerson", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "name", name);
                AddParameterWithName(command, "email", email);
                AddParameterWithName(command, "passwordHash", string.Empty);
                AddParameterWithName(command, "phoneNumber", phone);
                AddParameterWithName(command, "street", street);
                AddParameterWithName(command, "postalCode", postalCode);
                AddParameterWithName(command, "city", city);
                AddParameterWithName(command, "countryId", countryId);
                AddParameterWithName(command, "birthdate", birthDate);
                AddParameterWithName(command, "genderId", (int)gender);

                int personId = Convert.ToInt32(command.ExecuteScalar());

                return personId;
            }
        }

        /*
        public int ResolvePersonGeography (int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("ResolvePersonGeography", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }*/

        public void SetPersonGeography (int personId, int geographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPersonGeography", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "geographyId", geographyId);
                command.ExecuteNonQuery();
            }
        }


        public void SetPersonCountry (int personId, int countryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPersonCountry", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "countryId", countryId);
                command.ExecuteNonQuery();
            }
        }

        public void SetPersonGender (int personId, PersonGender gender)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPersonGender", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "genderId", (int)gender);
                command.ExecuteNonQuery();
            }
        }

        public void SetPersonBirthdate (int personId, DateTime birthdate)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPersonBirthdate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "birthdate", birthdate);
                command.ExecuteNonQuery();
            }
        }
    }
}