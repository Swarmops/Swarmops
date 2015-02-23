using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Interfaces
{
    public interface IHasParentIdentity
    {
        int ParentIdentity { get; }
    }
}
