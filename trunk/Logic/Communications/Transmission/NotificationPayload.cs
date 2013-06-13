using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Globalization;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications.Transmission
{
    [Serializable]
    public class NotificationPayload: PayloadBase<NotificationPayload>, ICommsRenderer
    {
        public NotificationPayload()
        {
            // empty ctor, for XML reflection
        }

        public NotificationPayload (string notificationResource)
        {
            this.SubjectResource = "Notifications_" + notificationResource + "_Subject";
            this.BodyResource = "Notifications_" + notificationResource + "_Body";
        }

        public string SubjectResource { get; set; }
        public string BodyResource { get; set; }

        public string GetSubject()
        {
            ResourceManager resourceManager = new System.Resources.ResourceManager("Swarmops.Logic.Localizations",
                                                                                   System.Reflection.Assembly.
                                                                                       GetExecutingAssembly());
            // TODO: Pick culture
            // TODO: Expand macros
            
            return resourceManager.GetString(this.SubjectResource);
        }

        public string GetBody()
        {
            ResourceManager resourceManager = new System.Resources.ResourceManager("Swarmops.Logic.Localizations",
                                                                                   System.Reflection.Assembly.
                                                                                       GetExecutingAssembly());
            // TODO: Pick culture
            // TODO: Expand macros

            return resourceManager.GetString(this.BodyResource);
        }

        #region Implementation of ICommsRenderer

        public RenderedComms RenderComm(OutboundComm comm, Person person)
        {
            RenderedComms result = new RenderedComms();

            result[CommsRenderPart.Subject] = GetSubject();
            result[CommsRenderPart.BodyText] = GetBody();
            result[CommsRenderPart.SenderName] = "Swarmops Administrative";
            result[CommsRenderPart.SenderMail] = "admin@swarmops.com";

            return result;
        }

        #endregion
    }

// ReSharper disable InconsistentNaming

    public enum NotificationResource
    {
        Unknown = 0,
        System_Startup
    }

// ReSharper restore InconsistentNaming
}
