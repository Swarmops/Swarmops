using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Exceptions
{
    /// <summary>
    /// Thrown when another session captured the item being modified before this session could
    /// execute its transaction. This happens but must be handled gracefully.
    /// </summary>
    public class DatabaseConcurrencyException: ConcurrencyException
    {
    }
}
