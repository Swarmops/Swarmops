using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Pirates;

namespace Swarmops.Logic.Financial
{
    public interface IAttestable
    {
        void Attest(Person attester);
        void Deattest(Person deattester);
    }
}
