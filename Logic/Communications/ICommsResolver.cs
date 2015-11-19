namespace Swarmops.Logic.Communications
{
    public interface ICommsResolver
    {
        string ToXml(); // implemented by PayloadBase

        /// <summary>
        /// Resolves the recipients for a certain piece of outbound communications.
        /// </summary>
        /// <param name="comm">The comm to resolve</param>
        void Resolve(OutboundComm comm);
    }
}