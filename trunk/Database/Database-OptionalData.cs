using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        private const string objectOptionalDataFieldSequence =
            " ObjectTypes.Name AS ObjectType,ObjectOptionalData.ObjectId,ObjectOptionalDataTypes.Name AS ObjectOptionalDataType,ObjectOptionalData.Data FROM ObjectOptionalData " +
            "JOIN ObjectTypes ON (ObjectTypes.ObjectTypeId=ObjectOptionalData.ObjectTypeId) " +
            "JOIN ObjectOptionalDataTypes ON (ObjectOptionalData.ObjectOptionalDataTypeId=ObjectOptionalDataTypes.ObjectOptionalDataTypeId) ";


        #region Select statements - reading data

        public BasicObjectOptionalData GetObjectOptionalData (IHasIdentity forObject)
        {
            Dictionary<ObjectOptionalDataType, string> initialData = new Dictionary<ObjectOptionalDataType, string>();
            ObjectType objectType = GetObjectTypeForObject(forObject);
            int objectId = forObject.Identity;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + objectOptionalDataFieldSequence + "WHERE ObjectTypes.Name='" + objectType.ToString() + "' " +
                        "AND ObjectOptionalData.ObjectId=" + objectId.ToString() + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string objectOptionalDataTypeString = reader.GetString(2);
                        string data = reader.GetString(3);
                        ObjectOptionalDataType objectOptionalDataType =
                            (ObjectOptionalDataType)
                            Enum.Parse(typeof(ObjectOptionalDataType), objectOptionalDataTypeString);

                        initialData[objectOptionalDataType] = data;
                    }

                    return new BasicObjectOptionalData(objectType, objectId, initialData);
                }
            }
        }


        public int[] GetObjectsByOptionalData (ObjectType objectType, ObjectOptionalDataType dataType, string data)
        {
            List<int> objectIds = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + objectOptionalDataFieldSequence + "WHERE ObjectTypes.Name='" + objectType.ToString() + "' " +
                        "AND ObjectOptionalDataTypes.Name='" + dataType.ToString() + "' " +
                        "AND ObjectOptionalData.Data='" + data.Replace("'", "''") + "';", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        objectIds.Add(reader.GetInt32(1));
                    }

                    return objectIds.ToArray();
                }
            }
        }


        static private ObjectType GetObjectTypeForObject (IHasIdentity objectToIdentify)
        {
            //string objectTypeString = objectToIdentify.GetType().ToString();

            //// Strip namespace

            //int lastPeriodIndex = objectTypeString.LastIndexOf('.');
            //objectTypeString = objectTypeString.Substring(lastPeriodIndex + 1);

            string objectTypeString = objectToIdentify.GetType().Name;

            // At this point, we should be able to cast the string to an ObjectType enum
            try
            {
                return (ObjectType)Enum.Parse(typeof(ObjectType), objectTypeString);
            }
            catch
            {
                //That failed. This could be a subclass, try upwards.
                Type parentType = objectToIdentify.GetType().BaseType; 
                while (parentType != typeof(Object))
                {
                    try
                    {
                        return (ObjectType)Enum.Parse(typeof(ObjectType), parentType.Name);
                    }
                    catch
                    {
                        parentType = parentType.BaseType;
                    }
                }
            }
            throw new InvalidCastException("GetObjectTypeForObject could not identify object of type " + objectToIdentify.GetType().Name);
        }

        #endregion


        #region Stored procedures for modifying data

        public void SetObjectOptionalData (IHasIdentity thisObject, ObjectOptionalDataType dataType, string data)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetObjectOptionalData", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "objectType", GetObjectTypeForObject(thisObject).ToString());
                AddParameterWithName(command, "objectId", thisObject.Identity);
                AddParameterWithName(command, "objectOptionalDataType", dataType.ToString());
                AddParameterWithName(command, "data", data);

                command.ExecuteNonQuery();
            }
        }


        #endregion
    }
}
