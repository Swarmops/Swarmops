using System;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.System;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    [Serializable]
    public class ObjectOptionalData : BasicObjectOptionalData
    {
        private IHasIdentity forObject;

        private ObjectOptionalData (BasicObjectOptionalData basic)
            : base (basic)
        {
            // empty ctor
        }

        public string this [ObjectOptionalDataType dataType]
        {
            get
            {
                if (OptionalData.ContainsKey (dataType))
                {
                    return OptionalData[dataType];
                }

                return null;
            }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetObjectOptionalData (this.forObject, dataType, value);
                OptionalData[dataType] = value;
            }
        }

        public static ObjectOptionalData FromBasic (BasicObjectOptionalData basic)
        {
            return new ObjectOptionalData (basic);
        }

        public static ObjectOptionalData ForObject (IHasIdentity identifiableObject)
        {
            ObjectOptionalData thisObject =
                FromBasic (SwarmDb.GetDatabaseForReading().GetObjectOptionalData (identifiableObject));
            thisObject.forObject = identifiableObject;
            return thisObject;
        }

        public bool HasData (ObjectOptionalDataType dataType)
        {
            if (OptionalData.ContainsKey (dataType))
            {
                return true;
            }
            return false;
        }

        //static private IHasIdentity ConstructObject (ObjectType objectType, int objectId)
        //{
        //    switch (objectType)
        //    {
        //        case ObjectType.Person:
        //            return Person.FromIdentity(objectId);
        //        case ObjectType.FinancialAccount:
        //            return FinancialAccount.FromIdentity(objectId);
        //        default:
        //            throw new NotImplementedException(
        //                "Unimplemented ObjectType in ObjectOptionalData.ConstructObject: " + objectType.ToString());
        //    }
        //}


        // Optional data support

        public string GetOptionalDataString (ObjectOptionalDataType key)
        {
            if (HasData (key))
            {
                return this[key];
            }

            return string.Empty;
        }

        public void SetOptionalDataString (ObjectOptionalDataType key, string value)
        {
            SetOptionalData (key, value);
        }


        // All ints default to 0
        public int GetOptionalDataInt (ObjectOptionalDataType key)
        {
            if (HasData (key))
            {
                return Int32.Parse (this[key]);
            }

            return 0;
        }

        public void SetOptionalDataInt (ObjectOptionalDataType key, int value)
        {
            SetOptionalData (key, value.ToString());
        }


        // All bools default to false
        public bool GetOptionalDataBool (ObjectOptionalDataType key)
        {
            if (HasData (key))
            {
                return (this[key] == "1" ? true : false);
            }

            return false;
        }

        public void SetOptionalDataBool (ObjectOptionalDataType key, bool value)
        {
            SetOptionalData (key, value ? "1" : "0");
        }


        public void SetOptionalData (ObjectOptionalDataType dataType, string data)
        {
            SwarmDb.GetDatabaseForWriting().SetObjectOptionalData (this.forObject, dataType, data);

            if (data == null && OptionalData.ContainsKey (dataType))
                OptionalData.Remove (dataType);
            else
                this[dataType] = data;
        }
    }
}