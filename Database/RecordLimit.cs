using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Database
{
    public class RecordLimit
    {
        public RecordLimit() { }

        public RecordLimit(int limit)
        {
            this.Limit = limit;
        }

        public int Limit { get; set; }
    }
}
