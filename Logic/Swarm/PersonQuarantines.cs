using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;

namespace Swarmops.Logic.Swarm
{
    public class PersonQuarantines
    {
        internal PersonQuarantines(Person person)  // internal: can only be constructed in assembly
        {
            this._person = person;
        }

        public PersonQuarantine Withdrawal
        {
            get { return new PersonQuarantine(this._person, ObjectOptionalDataType.QuarantineWithdrawalsUntil); }
        }

        public PersonQuarantine Login
        {
            get {  return new PersonQuarantine(this._person, ObjectOptionalDataType.QuarantineLoginsUntil); }
        }

        public PersonQuarantine Sessions
        {
            get { return new PersonQuarantine(this._person, ObjectOptionalDataType.QuarantineSessionsUntil); }
        }

        private Person _person;
    }
}
