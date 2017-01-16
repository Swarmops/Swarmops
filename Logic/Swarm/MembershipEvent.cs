using System;
using Swarmops.Common;
using Swarmops.Common.Enums;

namespace Swarmops.Logic.Swarm
{
    [Serializable]
    public class MembershipEvent
    {
        public int BirthYear;
        public DateTime DateTime;
        public int DeltaCount;
        public PersonGender Gender;
        public int GeographyId;
        public int OrganizationId;
        public int PersonId;

        public MembershipEvent()
        {
            this.DateTime = Constants.DateTimeLow;
            this.DeltaCount = 0;
            this.PersonId = 0;
            this.OrganizationId = 0;
            this.GeographyId = 0;
            this.BirthYear = 0;
            this.Gender = PersonGender.Unknown;
        }

        public MembershipEvent (DateTime dateTime, int personId, int organizationId, int geographyId, int birthYear,
            PersonGender gender, int deltaCount)
        {
            this.DateTime = dateTime;
            this.PersonId = personId;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.BirthYear = birthYear;
            this.Gender = gender;
            this.DeltaCount = deltaCount;
        }
    }
}