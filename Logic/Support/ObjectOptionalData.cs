using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Pirates;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{

    [Serializable]
    public class ObjectOptionalData : BasicObjectOptionalData
    {
        private IHasIdentity forObject = null;
        private ObjectOptionalData (BasicObjectOptionalData basic)
            : base(basic)
        {
            // empty ctor
        }

        public static ObjectOptionalData FromBasic (BasicObjectOptionalData basic)
        {
            return new ObjectOptionalData(basic);
        }

        public static ObjectOptionalData ForObject (IHasIdentity identifiableObject)
        {
            ObjectOptionalData thisObject = FromBasic(PirateDb.GetDatabaseForReading().GetObjectOptionalData((IHasIdentity)identifiableObject));
            thisObject.forObject = identifiableObject;
            return thisObject;
        }

        public bool HasData (ObjectOptionalDataType dataType)
        {
            if (this.OptionalData.ContainsKey(dataType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string this[ObjectOptionalDataType dataType]
        {
            get
            {
                if (this.OptionalData.ContainsKey(dataType))
                {
                    return this.OptionalData[dataType];
                }

                return null;
            }
            set
            {
                PirateDb.GetDatabaseForWriting().SetObjectOptionalData((IHasIdentity)forObject, dataType, value);
                this.OptionalData[dataType] = value;
            }
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

            if (this.HasData(key))
            {
                return this[key];
            }

            return string.Empty;
        }

        public void SetOptionalDataString (ObjectOptionalDataType key, string value)
        {
            this.SetOptionalData(key, value);
        }


        // All ints default to 0
        public int GetOptionalDataInt (ObjectOptionalDataType key)
        {

            if (this.HasData(key))
            {
                return Int32.Parse(this[key]);
            }

            return 0;
        }

        public void SetOptionalDataInt (ObjectOptionalDataType key, int value)
        {
            SetOptionalData(key, value.ToString());
        }


        // All bools default to false
        public bool GetOptionalDataBool (ObjectOptionalDataType key)
        {

            if (this.HasData(key))
            {
                return (this[key] == "1" ? true : false);
            }

            return false;
        }

        public void SetOptionalDataBool (ObjectOptionalDataType key, bool value)
        {
            SetOptionalData(key, value ? "1" : "0");
        }


        public void SetOptionalData (ObjectOptionalDataType dataType, string data)
        {
            PirateDb.GetDatabaseForWriting().SetObjectOptionalData((IHasIdentity)forObject, dataType, data);

            if (data == null && this.OptionalData.ContainsKey(dataType))
                this.OptionalData.Remove(dataType);
            else
                this[dataType] = data;
        }

    }
}
