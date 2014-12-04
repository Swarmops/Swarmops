using System.Collections.Generic;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    /// <summary>
    ///     Renders an OutboundComm for one specific type of comm and for the target person.
    /// </summary>
    public interface ICommsRenderer
    {
        RenderedComm RenderComm(Person person);
    }

    public class RenderedComm : Dictionary<CommRenderPart, string>
    {
        // typeset for readability
    }

    public enum CommRenderPart
    {
        Unknown = 0,
        SenderName,
        SenderMail,
        Subject,
        BodyText,
        BodyHtml,
        BodyImages
    }
}