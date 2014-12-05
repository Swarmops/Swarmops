using System;

namespace Swarmops.Database.Attributes
{
    /// <summary>
    ///     DbRecordType is used to mark a class what its database record is named
    ///     Needed if a Logic entity is using Database-condition and have to work with a subclass
    ///     i.e.     [DbRecordType("Person")] on class Person
    /// </summary>
    [AttributeUsage (AttributeTargets.All)]
    public class DbRecordType : Attribute
    {
        private string typeName = "";

        public DbRecordType (string recordname)
        {
            this.typeName = recordname;
        }

        public string TypeName
        {
            get { return this.typeName; }
            set { this.typeName = value; }
        }
    }
}