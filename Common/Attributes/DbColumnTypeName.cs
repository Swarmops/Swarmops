using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnForeignTypeName: Attribute
    {
        public DbColumnForeignTypeName(string foreignTableName, string commonIdColumnName, string foreignTypeColumnName)
        {
            this.ForeignTableName = foreignTableName;
            this.CommonIdColumnName = commonIdColumnName;
            this.ForeignTypeColumnName = foreignTypeColumnName;
        }

        public string ForeignTableName { get; private set; }
        public string CommonIdColumnName { get; private set; }
        public string ForeignTypeColumnName { get; private set; }
    }
}
