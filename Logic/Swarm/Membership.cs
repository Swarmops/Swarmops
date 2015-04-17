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

namespace Swarmops.Logic.Swarm
{
    public class Membership : BasicMembership
    {
        public static readonly int GracePeriod = 28;
        private Organization organization;
        private BasicMembershipPaymentStatus paymentStatus;
        private Person person;

        private Membership (BasicMembership basic)
            : base (basic)
        {
        }

        public Person Person
        {
            get
            {
                if (this.person == null)
                {
                    this.person = Person.FromIdentity (base.PersonId);
                }

                return this.person;
            }
        }

        public Organization Organization
        {
            get
            {
                if (this.organization == null)
                {
                    this.organization = Organization.FromIdentity (base.OrganizationId);
                }

                return this.organization;
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
                    SwarmDb.GetDatabaseForWriting().SetMembershipExpires (Identity, value);
                    base.Expires = value;
                }
            }
        }

        public MembershipPaymentStatus PaymentStatus
        {
            get
            {
                LoadPaymentStatus();
                return this.paymentStatus.Status;
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
                return this.paymentStatus.StatusDateTime;
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
            this.paymentStatus.Status = status;
            this.paymentStatus.StatusDateTime = dateTime;
        }

        private void LoadPaymentStatus()
        {
            if (this.paymentStatus == null)
            {
                this.paymentStatus = SwarmDb.GetDatabaseForReading().GetMembershipPaymentStatus (Identity);
            }
        }


        //Optimization metod, loadMultiple in one call
        public static void LoadPaymentStatuses (Memberships mss)
        {
            Dictionary<int, BasicMembershipPaymentStatus> statuses =
                SwarmDb.GetDatabaseForReading().GetMembershipPaymentStatuses (mss.Identities);
            foreach (Membership ms in mss)
            {
                if (ms.paymentStatus == null && statuses.ContainsKey (ms.Identity))
                {
                    ms.paymentStatus = statuses[ms.Identity];
                }
            }
        }


        public static Membership FromBasic (BasicMembership basic)
        {
            return new Membership (basic);
        }

        public static Membership FromIdentity (int membershipId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetMembership (membershipId));
        }

        public static Membership FromIdentityAggressive (int membershipId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetMembership (membershipId));
        }

        public static Membership FromPersonAndOrganization (int personId, int organizationId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetActiveMembership (personId, organizationId));
        }

        public static Membership Create (int personId, int organizationId, DateTime expires)
        {
            int membershipId = SwarmDb.GetDatabaseForWriting().CreateMembership (personId, organizationId, expires);

            return FromIdentityAggressive (membershipId);
        }

        public static Membership Create (Person person, Organization organization, DateTime expires)
        {
            return Create (person.Identity, organization.Identity, expires);
        }

        public static Membership Import (Person person, Organization organization, DateTime memberSince,
            DateTime expires)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .ImportMembership (person.Identity, organization.Identity, memberSince,
                        expires));
        }


        //public void Terminate ()
        //{
        //    Terminate(EventSource.Unknown);
        //}


        public void Terminate (EventSource eventSource, Person actingPerson, string description)
        {
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
                SwarmDb.GetDatabaseForWriting().TerminateMembership (Identity);
                base.Active = false;
                base.DateTerminated = DateTime.Now;

                // Remove all newsletter subscriptions once the membership is terminated (to make sure default is now off and only turn off explicitly turned-on subscriptions)

                // HACK HACK HACK: uses feed IDs in an extremely ugly way to loop 1-9. Should use NewsletterFeeds.ForOrganization() once support for newsletters in different orgs are established.

                for (int newsletterFeedId = 1; newsletterFeedId < 10; newsletterFeedId++)
                {
                    try
                    {
                        if (this.person.IsSubscribing (newsletterFeedId))
                        {
                            this.person.SetSubscription (newsletterFeedId, false);
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