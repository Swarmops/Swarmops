using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;

namespace Activizr.Logic.Financial
{
    public interface IValidatable
    {
        void Validate(Person validator);
        void Devalidate(Person devalidator);
    }
}
