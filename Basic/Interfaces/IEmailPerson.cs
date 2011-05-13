using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Interfaces
{
    public interface IEmailPerson : IHasIdentity
    {
        string Email { get; }
        string Name { get; }
    }
}