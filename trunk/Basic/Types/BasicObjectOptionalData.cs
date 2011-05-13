using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;

namespace Activizr.Basic.Types
{
    [Serializable]
    public class BasicObjectOptionalData
    {
        public BasicObjectOptionalData(ObjectType objectType, int objectId, Dictionary<ObjectOptionalDataType, string> initialData)
        {
            this.ObjectType = objectType;
            this.ObjectId = objectId;
            this.OptionalData = initialData;
        }

        public BasicObjectOptionalData(BasicObjectOptionalData original)
            : this (original.ObjectType, original.ObjectId, original.OptionalData)
        {
            // empty ctor
        }

        public ObjectType ObjectType { get; private set; }
        public int ObjectId { get; private set; }

        protected Dictionary<ObjectOptionalDataType, string> OptionalData;
    }
}
