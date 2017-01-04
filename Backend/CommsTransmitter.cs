using System;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Communications.Resolution;
using Swarmops.Utility.Communications;
using System.Reflection;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;
using Swarmops.Utility.BotCode;

namespace Swarmops.Backend
{
    internal class CommsTransmitter
    {
        internal static void Run()
        {
            OutboundComms comms = OutboundComms.GetOpen();

            foreach (OutboundComm comm in comms)
            {
                BotLog.Write(1, "CommsTx", "OutboundComm #" + comm.Identity.ToString("N0"));

                if (!comm.Resolved)
                {
                    BotLog.Write(2, "CommsTx", "--resolving");

                    ICommsResolver resolver = FindResolver(comm);
                    resolver.Resolve(comm);
                    comm.Resolved = true;

                    int recipientCount = comm.Recipients.Count;
                    BotLog.Write(2, "CommsTx", "--resolved to " + recipientCount.ToString("N0") + " recipients");

                    if (recipientCount > 1)
                    {
                        // "Your message has been queued for delivery and the recipients have been resolved. 
                        // Your mail will be sent to, or be attempted to sent to, [RecipientCount] people in [Geography] in [OrganizationName]."

                        NotificationStrings notifyStrings = new NotificationStrings();
                        NotificationCustomStrings customStrings = new NotificationCustomStrings();
                        notifyStrings[NotificationString.OrganizationName] = Organization.FromIdentity(comm.OrganizationId).Name;
                        customStrings["RecipientCount"] = comm.Recipients.Count.ToString("N0");
                        if (resolver is IHasGeography)
                        {
                            customStrings["GeographyName"] = ((IHasGeography) resolver).Geography.Name;
                        }
                            OutboundComm.CreateNotification(Organization.FromIdentity(comm.OrganizationId),
                                NotificationResource.OutboundComm_Resolved, notifyStrings, customStrings,
                                People.FromSingle(Person.FromIdentity(comm.SenderPersonId)));
                    }


                    continue; // continue is not strictly necessary; could continue processing the same OutboundComm after resolution
                }

                if (comm.TransmitterClass != "Swarmops.Utility.Communications.CommsTransmitterMail")
                {
                    throw new NotImplementedException();
                }

                ICommsTransmitter transmitter = new CommsTransmitterMail();

                OutboundCommRecipients recipients = comm.Recipients;
                PayloadEnvelope envelope = PayloadEnvelope.FromXml (comm.PayloadXml);

                BotLog.Write(2, "CommsTx", "--transmitting to " + recipients.Count.ToString("N0") + " recipients");

                comm.StartTransmission();

                foreach (OutboundCommRecipient recipient in recipients)
                {
                    try
                    {
                        transmitter.Transmit (envelope, recipient.Person);
                        recipient.CloseSuccess();
                    }
                    catch (OutboundCommTransmitException e)
                    {
                        recipient.CloseFailed (e.Description);
                    }
                }

                comm.Open = false;

                BotLog.Write(2, "CommsTx", "--closing");

                if (comm.RecipientsFail + comm.RecipientsSuccess > 1)
                {
                    BotLog.Write(2, "CommsTx", "--notifying");

                    ICommsResolver resolver = FindResolver(comm);

                    // "Your message to [GeographyName] has been sent to all scheduled recipients. Of the [RecipientCount] planned recipients,
                    // [RecipientsSuccess] succeeded from Swarmops' horizon. (These can fail later for a number of reasons, from broken
                    // computers to hospitalized recipients.) Time spent transmitting: [TransmissionTime]."

                    NotificationStrings notifyStrings = new NotificationStrings();
                    NotificationCustomStrings customStrings = new NotificationCustomStrings();
                    notifyStrings[NotificationString.OrganizationName] = Organization.FromIdentity(comm.OrganizationId).Name;
                    customStrings["RecipientCount"] = comm.Recipients.Count.ToString("N0");
                    customStrings["RecipientsSuccess"] = comm.RecipientsSuccess.ToString("N0");
                    customStrings["TransmissionTime"] =
                        (comm.ClosedDateTime - comm.StartTransmitDateTime).ToString("m min ss.ff sec");
                    if (resolver is IHasGeography)
                    {
                        customStrings["GeographyName"] = ((IHasGeography)resolver).Geography.Name;
                    }
                    OutboundComm.CreateNotification(Organization.FromIdentity(comm.OrganizationId),
                        NotificationResource.OutboundComm_Sent, notifyStrings, customStrings,
                        People.FromSingle(Person.FromIdentity(comm.SenderPersonId)));
                }
            }
        }

        internal static ICommsResolver FindResolver(OutboundComm comm)
        {
            // Resolve recipients

            ResolverEnvelope resolverEnvelope = ResolverEnvelope.FromXml(comm.ResolverDataXml);

            // Create the resolver via reflection of the static FromXml method

            Assembly assembly = typeof(ResolverEnvelope).Assembly;

            Type payloadType = assembly.GetType(resolverEnvelope.ResolverClass);
            var methodInfo = payloadType.GetMethod("FromXml", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return (ICommsResolver)(methodInfo.Invoke(null, new object[] { resolverEnvelope.ResolverDataXml }));
        }
    }
}