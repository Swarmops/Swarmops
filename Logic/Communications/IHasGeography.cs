using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Communications
{
    public interface IHasGeography
    {
        Geography Geography { get; }
    }
}
