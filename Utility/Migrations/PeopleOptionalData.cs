using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Pirates;
using Swarmops.Database;

namespace Swarmops.Utility.Migrations
{
    public class PeopleOptionalData
    {
        public static void Migrate()
        {
            PirateDb db = PirateDb.GetDatabaseForAdmin();

            using (DbConnection connection = db.GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                command.CommandText =
                    "SELECT PeopleOptionalData.PersonId, PersonOptionalDataTypes.Name AS PersonOptionalDataType, " +
                    "PeopleOptionalData.Data FROM PeopleOptionalData,PersonOptionalDataTypes " +
                    "WHERE PeopleOptionalData.PersonOptionalDataTypeId=PersonOptionalDataTypes.PersonOptionalDataTypeId ORDER BY PersonId";

                using (DbDataReader reader = command.ExecuteReader())
                {
                    int lastPersonId = 0;

                    while (reader.Read())
                    {
                        int personId = reader.GetInt32(0);
                        string personOptionalDataTypeString = reader.GetString(1);
                        string data = reader.GetString(2);

                        string displayData = data;
                        if (displayData.Length > 40)
                        {
                            displayData = displayData.Substring(0, 40);
                        }

                        displayData = displayData.Replace("\r\n", "#");

                        PersonOptionalDataKey key =
                            (PersonOptionalDataKey)
                            Enum.Parse(typeof (PersonOptionalDataKey), personOptionalDataTypeString);

                        // Display

                        Person person = null;

                        try
                        {
                            person = Person.FromIdentity(personId);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("PERSON #{0} IS NOT IN DATABASE", personId);
                        }

                        if (person != null)
                        {

                            if (personId != lastPersonId)
                            {
                                Console.WriteLine(person.Canonical + " -- ");
                                lastPersonId = personId;
                            }

                            ObjectOptionalDataType dataType =
                                (ObjectOptionalDataType)
                                Enum.Parse(typeof (ObjectOptionalDataType), personOptionalDataTypeString);

                            Console.WriteLine(" -- {0,-20} {1}", dataType.ToString(), displayData);

                            if (data.Trim().Length > 0)
                            {
                                db.SetObjectOptionalData(person, dataType, data);
                            }
                        }
                    }
                }
            }
        }
    }
}
