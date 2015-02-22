using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Interfaces
{
    public interface IHasSingularPluralTypes
    {
        Type SingularType { get; }
        Type PluralType { get; }
    }
}
