using System;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Communications.Resolution;
using Swarmops.Utility.Communications;
using System.Reflection;

namespace Swarmops.Backend
{
    internal class CommsTransmitter
    {
        internal static void Run()
        {
            OutboundComms comms = OutboundComms.GetOpen();

            foreach (OutboundComm comm in comms)
            {
                if (!comm.Resolved)
                {
                    // Resolve recipients

                    ResolverEnvelope resolverEnvelope = ResolverEnvelope.FromXml(comm.ResolverDataXml);

                    // Create the resolver via reflection of the static FromXml method

                    Assembly assembly = typeof(ResolverEnvelope).Assembly;

                    Type payloadType = assembly.GetType(resolverEnvelope.ResolverClass);
                    var methodInfo = payloadType.GetMethod("FromXml", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    ICommsResolver resolver = (ICommsResolver)(methodInfo.Invoke(null, new object[] { resolverEnvelope.ResolverDataXml }));
                    resolver.Resolve(comm);

                    continue; // continue is not strictly necessary; could continue processing the same OutboundComm after resolution
                }

                if (comm.TransmitterClass != "Swarmops.Utility.Communications.CommsTransmitterMail")
                {
                    throw new NotImplementedException();
                }

                ICommsTransmitter transmitter = new CommsTransmitterMail();

                OutboundCommRecipients recipients = comm.Recipients;
                PayloadEnvelope envelope = PayloadEnvelope.FromXml (comm.PayloadXml);

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
            }
        }
    }
}