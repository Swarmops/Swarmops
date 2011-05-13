using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Activizr.Basic.Types;
using Activizr.Basic.Enums;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        public int[] GetActivistPersonIds (int[] geographyIds)
        {
            List<int> result = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT Activists.PersonId FROM Activists,People WHERE Activists.Active=1 AND Activists.PersonId=People.PersonId AND People.GeographyId IN (" +
                        JoinIds(geographyIds) + ")", connection);
                DbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }

            return result.ToArray();
        }

        public void CreateActivist (int personId, bool isPublic, bool isConfirmed)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                DbCommand command = GetDbCommand("CreateActivist", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "public", isPublic);
                AddParameterWithName(command, "confirmed", isConfirmed);
                AddParameterWithName(command, "dateTimeCreated", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }

        public int GetActivistCountForGeographies (int[] geographyIds)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT Count(Activists.PersonId) FROM Activists,People WHERE Activists.Active=1 AND Activists.PersonId=People.PersonId AND People.GeographyId IN (" +
                        JoinIds(geographyIds) + ")", connection);
                DbDataReader reader = command.ExecuteReader();

                reader.Read();
                return reader.GetInt32(0);
            }
        }


        public bool GetActivistStatus (int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT * From Activists WHERE Active=1 AND PersonId=" + personId.ToString(),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }

                    return false;
                }
            }
        }


        public void TerminateActivist (int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                DbCommand command = GetDbCommand("TerminateActivist", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);

                command.ExecuteNonQuery();

            }
        }




    }
}