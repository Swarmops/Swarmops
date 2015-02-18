using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Security;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        private const string readFieldsSQL =
            @" ExternalIdentityIdentity, ExternalIdentityTypeName, ExternalSystem, UserID, `Password`, AttachedToPerson
        FROM ExternalIdentities inner join ExternalIdentityTypes on TypeOfAccount=ExternalIdentityTypeId";

        public BasicExternalIdentity GetExternalIdentity (int identity)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + readFieldsSQL + " WHERE ExternalIdentityIdentity=@id",
                        connection);
                AddParameterWithName (command, "id", identity);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExternalIdentityFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Identity");
                }
            }
        }

        public BasicExternalIdentity GetExternalIdentityFromPersonIdAndType (int persId, ExternalIdentityType type)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + readFieldsSQL +
                        " WHERE AttachedToPerson= @persId and ExternalIdentityTypeName = @typeName",
                        connection);
                AddParameterWithName (command, "persId", persId);
                AddParameterWithName (command, "typeName", type.ToString());

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExternalIdentityFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Identity");
                }
            }
        }

        public BasicExternalIdentity GetExternalIdentityFromUserIdAndType (string userid, ExternalIdentityType type)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + readFieldsSQL + " WHERE UserID = @userid and ExternalIdentityTypeName = @type",
                        connection);
                AddParameterWithName (command, "userid", userid);
                AddParameterWithName (command, "type", type.ToString());

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExternalIdentityFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Identity");
                }
            }
        }

        public List<BasicExternalIdentity> GetExternalIdentities (int persId)
        {
            List<BasicExternalIdentity> retval = new List<BasicExternalIdentity>();
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + readFieldsSQL + " WHERE AttachedToPerson= @persId",
                        connection);
                AddParameterWithName (command, "persId", persId);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retval.Add (ReadExternalIdentityFromDataReader (reader));
                    }
                    return retval;
                }
            }
        }

        /// <summary>
        ///     Update/Insert if id == 0 it is an insert
        /// </summary>
        /// <param name="externalIdentityIdentity"></param>
        /// <param name="externalSystem"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="attachedToPerson"></param>
        /// <param name="typeOfAccount"></param>
        /// <returns></returns>
        public BasicExternalIdentity SetExternalIdentity (
            int externalIdentityIdentity,
            ExternalIdentityType typeOfAccount,
            string externalSystem,
            string userID,
            string password,
            int attachedToPerson)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetExternalIdentity", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "pExternalIdentityIdentity", externalIdentityIdentity);
                AddParameterWithName (command, "pTypeOfAccount", typeOfAccount.ToString());
                AddParameterWithName (command, "pExternalSystem", externalSystem);
                AddParameterWithName (command, "pUserID", userID);
                AddParameterWithName (command, "pPassword", password);
                AddParameterWithName (command, "pAttachedToPerson", attachedToPerson);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExternalIdentityFromDataReader (reader);
                    }
                }
            }
            return null;
        }


        private BasicExternalIdentity ReadExternalIdentityFromDataReader (DbDataReader reader)
        {
            int externalIdentityId = (int) reader["ExternalIdentityIdentity"];
            ExternalIdentityType typeOfAccount =
                (ExternalIdentityType)
                    Enum.Parse (typeof (ExternalIdentityType), reader["ExternalIdentityTypeName"].ToString());
            string externalSystem = (string) reader["ExternalSystem"];
            string userID = (string) reader["UserID"];
            string password = (string) reader["Password"];
            int attachedToPerson = (int) reader["AttachedToPerson"];

            return new BasicExternalIdentity (externalIdentityId, typeOfAccount, externalSystem, userID, password,
                attachedToPerson);
        }
    }
}