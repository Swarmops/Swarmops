using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Communications;
using Swarmops.Database;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class OutboundComm: BasicOutboundComm
    {
        private OutboundComm (BasicOutboundComm basic): base (basic)
        {
            // private ctor
        }

        public static OutboundComm FromBasic (BasicOutboundComm basic)
        {
            return new OutboundComm(basic);
        }

        public static OutboundComm FromIdentity(int outboundCommId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetOutboundComm(outboundCommId));
        }

        public static OutboundComm FromIdentityAggressive(int outboundCommId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetOutboundComm(outboundCommId));
        }

        public static OutboundComm Create(Person sender, Person from, Organization organization, CommResolverClass resolverClass, string recipientDataXml, CommTransmitterClass transmitterClass, string payloadXml, OutboundCommPriority priority)
        {
            string resolverClassString = string.Empty;
            if (resolverClass != CommResolverClass.Unknown)
            {
                resolverClassString = resolverClass.ToString();
            }

            string transmitterClassString = string.Empty;
            if (transmitterClass != CommTransmitterClass.Unknown)
            {
                transmitterClassString = "Swarmops.Utility.Communications." + transmitterClass.ToString();
            }

            return Create(sender, from, organization, resolverClassString, recipientDataXml, transmitterClassString,
                          payloadXml, priority);
        }

        public static OutboundComm Create(Person sender, Person from, Organization organization, string resolverClassString, string recipientDataXml, string transmitterClassString, string payloadXml, OutboundCommPriority priority)
        {
            int newId = SwarmDb.GetDatabaseForWriting().CreateOutboundComm(sender != null? sender.Identity: 0, from != null? from.Identity: 0,
                                                                           organization != null? organization.Identity: 0, resolverClassString,
                                                                           recipientDataXml ?? string.Empty, transmitterClassString,
                                                                           payloadXml, priority);

            return FromIdentityAggressive(newId);
        }


        public static OutboundComm CreateNotification (Organization organization, NotificationResource notification)
        {
            return CreateNotification(organization, notification.ToString());
        }


        public static OutboundComm CreateNotification (Organization organization, string notificationResourceString)
        {
            List<Person> recipients = People.FromSingle(Person.FromIdentity(1)); // Initial Admin recipient

            if (organization != null)
            {
                // TODO: Change to org admins
            }

            OutboundComm comm = OutboundComm.Create(null, null, organization, CommResolverClass.Unknown, null,
                                                    CommTransmitterClass.CommsTransmitterMail,
                                                    new PayloadEnvelope(new NotificationPayload(notificationResourceString)).ToXml(),
                                                    OutboundCommPriority.Low);

            foreach (Person person in recipients)
            {
                comm.AddRecipient(person);
            }

            comm.Resolved = true;

            return comm;
        }

        // public static OutboundComm CreateNotification (FinancialAccount budget, string notificationResource)


        public new bool Resolved
        {
            get { return base.Resolved; }
            set
            {
                if (!base.Resolved && value == true)
                {
                    SwarmDb.GetDatabaseForWriting().SetOutboundCommResolved(this.Identity);
                    base.Resolved = true;
                }
                else
                {
                    throw new InvalidOperationException("Can only set OutboundComms.Resolved from false to true");
                }
            }
        }


        public void AddRecipient (Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException("Recipient cannot be null.");
            }

            SwarmDb.GetDatabaseForWriting().CreateOutboundCommRecipient(this.Identity, person.Identity);
        }

        public OutboundCommRecipients Recipients
        {
            get { return OutboundCommRecipients.ForOutboundComm(this); }
        }

    }

    public enum CommResolverClass
    {
        Unknown = 0,
        Test_Foo_Bar

    }

    public enum CommTransmitterClass
    {
        Unknown = 0,
        CommsTransmitterMail
    }
}
