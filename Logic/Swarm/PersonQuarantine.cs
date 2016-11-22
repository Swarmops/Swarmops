using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class PersonQuarantine
    {
        internal PersonQuarantine(Person person, ObjectOptionalDataType quarantineType)  // internal: can only be constructed in assembly
        {
            this._person = person;
            this._quarantineType = quarantineType;
        }

        public DateTime UntilWhen
        {
            get
            {
                string quarantineString = ObjectOptionalData.ForObject(this._person).GetOptionalDataString(this._quarantineType);

                if (quarantineString.Length > 0)
                {
                    return DateTime.ParseExact(quarantineString, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                }
                else
                {
                    return DateTime.MinValue; // no quarantine active
                }
            }
        }

        public bool IsActive
        {
            get { return (this.UntilWhen > DateTime.UtcNow); }
        }

        public void QuarantineFor(TimeSpan span)
        {
            DateTime now = DateTime.UtcNow;
            DateTime quarantineUntil = now + span;

            ObjectOptionalData.ForObject(this._person).SetOptionalDataString(this._quarantineType, quarantineUntil.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
        }

        private Person _person;
        private ObjectOptionalDataType _quarantineType;
    }
}
