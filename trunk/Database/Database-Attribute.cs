using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Database.Attributes
{
    /// <summary>
    /// DbRecordType is used to mark a class what its database record is named
    /// Needed if a Logic entity is using Database-condition and have to work with a subclass
    /// i.e.     [DbRecordType("Person")] on class Person
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class DbRecordType : System.Attribute
    {
        string typeName = "";

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }
        public DbRecordType (string recordname)
        {
            typeName = recordname;
        }
    }
}
