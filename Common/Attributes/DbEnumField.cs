using System;

namespace Swarmops.Common.Attributes
{
    /// <summary>
    ///     DbRecordType is used to mark a class what its database record is named
    ///     Needed if a Logic entity is using Database-condition and have to work with a subclass
    ///     i.e.     [DbRecordType("Person")] on class Person
    /// </summary>
    [AttributeUsage (AttributeTargets.All)]
    public class DbEnumField : Attribute
    {
        public DbEnumField (string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }
}