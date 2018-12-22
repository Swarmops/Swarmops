using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 2018-Dec-22: This is a started refactoring of the Database namespace into separate derived classes
 *              from a generic that contains most of the gruntwork. It is not nearly complete and
 *              the code is still experimental, not taken into production yet.
 */


namespace Swarmops.Common.Interfaces
{
    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    public interface IDatabaseColumnAware<TBasic>
    {
        string DbTableName { get; }
        string[] DbFieldSequence { get; }
        string DbIdentityField { get; }
        TBasic FromDataReader (IDataReader reader);  // this could be static but C# doesn't allow static methods in interfaces
    }
}
