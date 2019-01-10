using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 2018-Dec-22: This is a started refactoring of the Database namespace into separate derived classes
 *              from a generic that contains most of the gruntwork. It is not nearly complete and
 *              the code is still experimental, not taken into production yet.
 */


namespace Swarmops.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnName: Attribute
    {
        public DbColumnName()
        {
            // empty attr means the column name is the same as the property name that houses the attribute
            this.ColumnName = string.Empty;
        }
        public DbColumnName(string columnName)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName { get; private set; }
    }
}
