using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public interface ICommsTransmitter
    {
        /// <summary>
        /// This function transmits an outbound comm. If it fails, it must throw an OutboundCommTransmitException. It is expected to block until complete.
        /// </summary>
        /// <param name="comm">The piece of communications to send.</param>
        /// <param name="person">The person to send to.</param>
        void Transmit(OutboundComm comm, Person person);
    }
}
