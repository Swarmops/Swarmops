using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Exceptions
{
    public class DatabaseExecuteException: Exception
    {
        public DatabaseExecuteException(string attemptedCommand, Exception innerException = null)
        {
            if (String.IsNullOrEmpty(attemptedCommand))
            {
                throw new ArgumentNullException(attemptedCommand, "Parameter cannot be null or empty");
            }

            this.AttemptedCommand = attemptedCommand;
            this.InnerException = innerException;
        }

        public string AttemptedCommand { get; }
        public new Exception InnerException { get; }
    }
}
