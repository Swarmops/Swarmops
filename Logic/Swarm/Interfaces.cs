using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swarmops.Logic.Pirates
{
    public interface IOwnerSettable
    {
        void SetOwner(Person newOwner);
    }
}
