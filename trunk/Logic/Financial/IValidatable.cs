using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public interface IValidatable
    {
        void Validate(Person validator);
        void Devalidate(Person devalidator);
    }
}
