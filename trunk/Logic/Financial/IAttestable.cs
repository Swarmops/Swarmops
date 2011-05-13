using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;

namespace Activizr.Logic.Financial
{
    public interface IAttestable
    {
        void Attest(Person attester);
        void Deattest(Person deattester);
    }
}
