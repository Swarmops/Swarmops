using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    /// <summary>
    /// Renders an OutboundComm for one specific type of comm and for the target person.
    /// </summary>
    public interface ICommsRenderer
    {
        RenderedComms RenderComm(OutboundComm comm, Person person);
    }

    public class RenderedComms: Dictionary<CommsRenderPart,string>
    {
        // typeset for readability
    }

    public enum CommsRenderPart
    {
        Unknown = 0,
        SenderName,
        SenderMail,
        Subject,
        Body
    }
}
