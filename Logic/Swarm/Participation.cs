using System;
using System.Collections.Generic;
using System.Web;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using System.IO;

namespace Swarmops.Logic.Swarm
{
    public class Participation : BasicParticipation
    {
        public static readonly int GracePeriod = 28;
        private Organization _organization;
        private BasicMembershipPaymentStatus _paymentStatus;
        private Person _person;

        private Participation (BasicParticipation basic)
            : base (basic)
        {
        }


        public Participation FromPersonAndOrganization(Person person, Organization organization)
        {
            Participations participations = Participations.FromArray(SwarmDb.GetDatabaseForReading().GetParticipations(person, organization));

            if (participations.Count > 1)
            {
                throw new InvalidDataException("More than one participation record item for a person/organization combination is not allowed");
            }
            if (participations.Count == 0)
            {
                return null; // no combo. Throw instead? This has not been consistently coded
            }

            return participations[0];
        }


        public Person Person
        {
            get
            {
                if (this._person == null)
                {
                    this._person = Person.FromIdentity (base.PersonId);
                }

                return this._person;
            }
        }

        public Organization Organization
        {
            get
            {
                if (this._organization == null)
                {
                    this._organization = Organization.FromIdentity (base.OrganizationId);
                }

                return this._organization;
            }
        }

        public new DateTime Expires
        {
            get { return base.Expires; }
            set
            {
                if (base.Expires != value)
                {
                    if (PersonId > 0 && OrganizationId > 0 && base.Expires != new DateTime (1900, 1, 1))
                    {
                        ChurnData.LogRetention (PersonId, OrganizationId, base.Expires);
                    }
                    SwarmDb.GetDatabaseForWriting().SetParticipationExpires (Identity, value);
                    base.Expires = value;
                }
            }
        }

        public MembershipPaymentStatus PaymentStatus
        {
            get
            {
                LoadPaymentStatus();
                return this._paymentStatus.Status;
            }
            private set
            {
                throw new InvalidOperationException (
                    "It is not allowed to set PaymentStatus separately. Use SetPaymentStatus() method.");

                // Rick: Isn't it better to not have a setter at all and get the error compile-time rather than run-time?
            }
        }


        public DateTime PaymentStatusDate
        {
            get
            {
                LoadPaymentStatus();
                return this._paymentStatus.StatusDateTime;
            }
            private set
            {
                throw new InvalidOperationException (
                    "It is not allowed to set PaymentStatusDate separately. Use SetPaymentStatus() method");

                // Rick: Isn't it better to not have a setter at all and get the error compile-time rather than run-time?
            }
        }

        public void SetPaymentStatus (MembershipPaymentStatus status, DateTime dateTime)
        {
            SwarmDb.GetDatabaseForWriting().SetMembershipPaymentStatus (Identity, status, dateTime);
            LoadPaymentStatus();
            this._paymentStatus.Status = status;
            this._paymentStatus.StatusDateTime = dateTime;
        }

        private void LoadPaymentStatus()
        {
            if (this._paymentStatus == null)
            {
                this._paymentStatus = SwarmDb.GetDatabaseForReading().GetMembershipPaymentStatus (Identity);
            }
        }


        //Optimization metod, loadMultiple in one call
        public static void LoadPaymentStatuses (Participations mss)
        {
            Dictionary<int, BasicMembershipPaymentStatus> statuses =
                SwarmDb.GetDatabaseForReading().GetMembershipPaymentStatuses (mss.Identities);
            foreach (Participation ms in mss)
            {
                if (ms._paymentStatus == null && statuses.ContainsKey (ms.Identity))
                {
                    ms._paymentStatus = statuses[ms.Identity];
                }
            }
        }


        public static Participation FromBasic (BasicParticipation basic)
        {
            return new Participation (basic);
        }

        public static Participation FromIdentity (int membershipId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetParticipation (membershipId));
        }

        public static Participation FromIdentityAggressive (int membershipId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetParticipation (membershipId));
        }

        public static Participation FromPersonAndOrganization (int personId, int organizationId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetActiveParticipation (personId, organizationId));
        }

        public static Participation Create (int personId, int organizationId, DateTime expires)
        {
            int membershipId = SwarmDb.GetDatabaseForWriting().CreateParticipation (personId, organizationId, expires);

            return FromIdentityAggressive (membershipId);
        }

        public static Participation Create (Person person, Organization organization, DateTime expires)
        {
            return Create (person.Identity, organization.Identity, expires);
        }

        public static Participation Import (Person person, Organization organization, DateTime memberSince,
            DateTime expires)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .ImportParticipation (person.Identity, organization.Identity, memberSince,
                        expires));
        }


        //public void Terminate ()
        //{
        //    Terminate(EventSource.Unknown);
        //}


        public void Terminate (EventSource eventSource, Person actingPerson, string description)
        {
            // TODO: This needs a new overload for v5, which adds the Position and possibly removes the EventSource

            if (base.Active)
            {

                // REMOVED: everything Roles related; being phased out


                PWLog.Write (actingPerson.Identity, PWLogItem.Person, Person.Identity, PWLogAction.MemberLost,
                    eventSource + ":" + description, string.Empty);
                PWEvents.CreateEvent (EventSource.PirateWeb, EventType.LostMember, actingPerson.Identity, OrganizationId, Person.GeographyId,
                    Person.Identity, 0, OrganizationId.ToString());


                //Added LogChurn here to make SURE they always are logged with the membership.
                if (PersonId > 0 && OrganizationId > 0 && base.Expires != new DateTime (1900, 1, 1))
                {
                    ChurnData.LogChurn (PersonId, OrganizationId);
                }
                SwarmDb.GetDatabaseForWriting().TerminateParticipation (Identity);
                base.Active = false;
                base.DateTerminated = DateTime.Now;

                // Remove all newsletter subscriptions once the membership is terminated (to make sure default is now off and only turn off explicitly turned-on subscriptions)

                // HACK HACK HACK: uses feed IDs in an extremely ugly way to loop 1-9. Should use NewsletterFeeds.ForOrganization() once support for newsletters in different orgs are established.

                for (int newsletterFeedId = 1; newsletterFeedId < 10; newsletterFeedId++)
                {
                    try
                    {
                        if (this._person.IsSubscribing (newsletterFeedId))
                        {
                            this._person.SetSubscription (newsletterFeedId, false);
                        }
                    }
                    catch (Exception)
                    {
                        // ignore nonexisting newsletter feeds -- this is a hack anyway
                    }
                }
            }
        }
    }
}