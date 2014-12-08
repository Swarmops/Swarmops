using System;

namespace Swarmops.Logic.Communications
{
    /// <summary>
    ///     This exception is thrown if transmission of an OutboundComm fails (for a specific person). The description should
    ///     NOT contain personal data or anything that varies with person/time.
    /// </summary>
    public class OutboundCommTransmitException : Exception
    {
        public OutboundCommTransmitException (string description) : base (description)
        {
            Description = description;
        }

        public OutboundCommTransmitException (string description, Exception innerException)
            : base (description, innerException)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}