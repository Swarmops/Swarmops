using System;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    [Serializable]
    public class Volunteer : BasicVolunteer
    {
        #region Creation and Construction

        private Volunteer ()
            : base(0, 0, 0, DateTime.MinValue, false, DateTime.MinValue, string.Empty)
        {
        }

        private Volunteer (BasicVolunteer basic)
            : base(basic)
        {
            // empty ctor
        }

        public static Volunteer FromIdentity (int volunteerId)
        {
            return FromBasic(PirateDb.GetDatabase().GetVolunteer(volunteerId));
        }

        public static Volunteer FromBasic (BasicVolunteer basic)
        {
            return new Volunteer(basic);
        }

        public static Volunteer Create (Person person, Person owner)
        {
            return Create(person.Identity, owner.Identity);
        }

        public static Volunteer Create (int personId, int ownerPersonId)
        {
            return FromIdentity(PirateDb.GetDatabase().CreateVolunteer(personId, ownerPersonId));
        }

        #endregion

        private Geography geography;
        private Person ownerPerson;
        private Person person;

        public string Name
        {
            get
            {
                PopulateCache();
                return this.person.Name;
            }
        }

        public Person Owner
        {
            get
            {
                PopulateCache();
                return this.ownerPerson;
            }
            set
            {
                this.ownerPerson = value;
                PirateDb.GetDatabase().SetVolunteerOwnerPersonId(Identity, value.Identity);
            }
        }


        public Geography Geography
        {
            get
            {
                PopulateCache();
                return this.geography;
            }
        }


        public string OwnerName
        {
            get
            {
                PopulateCache();
                return this.ownerPerson.Name;
            }
        }

        public string GeographyName
        {
            get
            {
                PopulateCache();
                return this.geography.Name;
            }
        }

        public string Phone
        {
            get
            {
                PopulateCache();
                return this.person.Phone;
            }
        }

        public Person Person
        {
            get
            {
                PopulateCache();
                return this.person;
            }
        }

        public VolunteerRoles Roles
        {
            get { return VolunteerRoles.FromArray(PirateDb.GetDatabase().GetVolunteerRolesByVolunteer(Identity)); }
        }

        public void AddRole (Organization organization, Geography geography, RoleType roleType)
        {
            AddRole(organization.Identity, geography.Identity, roleType);
        }

        public void AddRole (int organizationId, int geographyId, RoleType roleType)
        {
            PirateDb.GetDatabase().CreateVolunteerRole(Identity, organizationId, geographyId, roleType);
        }

        public void Close (string comments)
        {
            PirateDb.GetDatabase().CloseVolunteer(Identity, comments);
        }

        private void PopulateCache ()
        {
            if (this.person == null)
            {
                this.person = Person.FromIdentity(PersonId);
            }

            if (this.ownerPerson == null)
            {
                this.ownerPerson = Person.FromIdentity(OwnerPersonId);
            }

            if (this.geography == null)
            {
                if (this.person.GeographyId != 0)
                {
                    this.geography = this.person.Geography;
                }
                else
                {
                    this.geography = Geography.Root;
                }
            }
        }



        /// <summary>
        ///          Assign the volunteer to a suitable owner will try to assign to ElectoralCircuit lead
        ///          If that is not possible it will go to district lead
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="withinOrg"></param>
        /// <param name="defaultOwner"></param>
        /// <param name="stopGeography">The parent of top geographies that could/should recieve volunteer (country) </param>
        public void AutoAssign (Geography geo, int withinOrg, Person defaultOwner, int stopGeography)
        {

            //Note: stopGeography is only needed because Districts ar not properly defined. Districts are the ones below country.
            try
            {
                this.Owner = defaultOwner;
                Geography volonteerGeography = geo;


                //Move up to target geography level
                //GeographyLevel targetLevel = GeographyLevel.ElectoralCircuit;
                GeographyLevel targetLevel = GeographyLevel.District; //This will never hit bcse Districts ar not properly defined

                while (!(volonteerGeography.AtLevel(targetLevel))
                    && (volonteerGeography.ParentGeographyId != 0)
                        && (volonteerGeography.ParentGeographyId != stopGeography)
                        )
                {
                    volonteerGeography = volonteerGeography.Parent;
                }

                Person localLead = Pirates.Roles.GetLocalLead(withinOrg, volonteerGeography.Identity);

                if (localLead == null && volonteerGeography.Parent != null)
                {
                    // Move "geography" up to district, wich are next below country, if no lead was found at the target level
                    while ((volonteerGeography.ParentGeographyId != 0)
                        && (volonteerGeography.ParentGeographyId != stopGeography)
                        )
                    {
                        volonteerGeography = volonteerGeography.Parent;
                    }
                    localLead = Pirates.Roles.GetLocalLead(withinOrg, volonteerGeography.Parent.Parent.Identity);
                }

                //Found anyone? otherwise leave default Owner.
                if (localLead != null)
                {
                    this.Owner = localLead;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}