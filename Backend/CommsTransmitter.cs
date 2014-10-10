using System;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;

namespace Swarmops.Backend
{
    internal class CommsTransmitter
    {
        static internal void Run()
        {
            OutboundComms comms = OutboundComms.GetOpen();

            foreach (OutboundComm comm in comms)
            {
                // TODO: RESOLVE RECIPIENTS

                if (!comm.Resolved)
                {
                    continue;
                }

                if (comm.TransmitterClass != "Swarmops.Utility.Communications.CommsTransmitterMail")
                {
                    throw new NotImplementedException();
                }

                ICommsTransmitter transmitter = new Swarmops.Utility.Communications.CommsTransmitterMail();

                OutboundCommRecipients recipients = comm.Recipients;
                PayloadEnvelope envelope = PayloadEnvelope.FromXml(comm.PayloadXml);

                comm.StartTransmission();

                foreach (OutboundCommRecipient recipient in recipients)
                {
                    try
                    {
                        transmitter.Transmit(envelope, recipient.Person);
                        recipient.CloseSuccess();
                    }
                    catch (OutboundCommTransmitException e)
                    {
                        recipient.CloseFailed(e.Description);
                    }
                }

                comm.Open = false;
            }
        }
    }
}
