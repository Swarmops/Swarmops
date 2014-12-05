using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Media;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class OutboundMailRecipient : BasicOutboundMailRecipient
    {
        public enum RecipientType
        {
            Person,
            Reporter
        }

        private readonly OutboundMail outboundMail;
        private IEmailPerson person;

        private OutboundMailRecipient()
            : base (null)
        {
            // Never construct this way
        }

        private OutboundMailRecipient (BasicOutboundMailRecipient basic, OutboundMail outboundMail)
            : base (basic)
        {
            this.outboundMail = outboundMail;
        }


        private new int OutboundMailId
        {
            get { return 0; } // Hide base.OutboundMailId
        }

        private new int PersonId
        {
            get { return 0; } // Hide base.PersonId
        }

        private new int PersonType
        {
            get { return 0; } // Hide base.PersonId
        }

        public OutboundMail OutboundMail
        {
            get { return this.outboundMail; }
        }

        public IEmailPerson EmailPerson // Cache the Person object
        {
            get
            {
                CachePerson();
                return this.person;
            }
        }

        public Person Person // Cache the Person object
        {
            get
            {
                CachePerson();
                if (this.person is Person)
                    return (Person) this.person;
                return null;
            }
        }

        public Reporter Reporter // Cache the Person object
        {
            get
            {
                CachePerson();
                if (this.person is Reporter)
                    return (Reporter) this.person;
                return null;
            }
        }

        private void CachePerson()
        {
            if (this.person == null)
            {
                RecipientType type = (RecipientType) base.PersonType;
                if (type == RecipientType.Person)
                {
                    this.person = Person.FromIdentity (base.PersonId);
                }
                else if (type == RecipientType.Reporter)
                {
                    this.person = Reporter.FromIdentity (base.PersonId);
                }
            }
        }

        internal static OutboundMailRecipient FromBasic (BasicOutboundMailRecipient basic, OutboundMail outboundMail)
        {
            return new OutboundMailRecipient (basic, outboundMail);
        }

        public static void Create (OutboundMail outboundMail, Person person, bool asOfficer)
        {
            Create (outboundMail.Identity, person.Identity, asOfficer, (int) RecipientType.Person);
        }

        public static void Create (OutboundMail outboundMail, Reporter person, bool asOfficer)
        {
            Create (outboundMail.Identity, person.Identity, asOfficer, (int) RecipientType.Reporter);
        }

        public static void Create (int outboundMailId, int personId, bool asOfficer, int personType)
        {
            SwarmDb.GetDatabaseForWriting()
                .CreateOutboundMailRecipient (outboundMailId, personId, asOfficer, personType);
        }

        public void Delete()
        {
            SwarmDb.GetDatabaseForWriting().DeleteOutboundMailRecipient (Identity);
        }
    }
}