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
        [Obsolete("Do not use this direct constructor. It is intended for XML serialization only. Therefore, you're getting a compile-time error.", true)]
        public NotificationPayload()
        {
            // empty ctor, for XML reflection. Do not use.
        }

        public NotificationPayload(string notificationResource): this (notificationResource, new NotificationStrings())
        {
            // redirect to main ctor
        }

        public NotificationPayload (string notificationResource, NotificationStrings strings)
        {
            this.SubjectResource = "Notifications_" + notificationResource + "_Subject";
            this.BodyResource = "Notifications_" + notificationResource + "_Body";
            this.Strings = strings;
        }

        public string SubjectResource { get; set; }
        public string BodyResource { get; set; }
        public NotificationStrings Strings { get; set; }

        public string GetSubject()
        {
            ResourceManager resourceManager = new System.Resources.ResourceManager("Swarmops.Logic.Localizations",
                                                                                   System.Reflection.Assembly.
                                                                                       GetExecutingAssembly());
            // TODO: Pick culture
            
            return ExpandMacros(resourceManager.GetString(this.SubjectResource));
        }

        public string GetBody()
        {
            ResourceManager resourceManager = new System.Resources.ResourceManager("Swarmops.Logic.Localizations",
                                                                                   System.Reflection.Assembly.
                                                                                       GetExecutingAssembly());
            // TODO: Pick culture

            return ExpandMacros(resourceManager.GetString(this.BodyResource));
        }

        public string ExpandMacros (string input)
        {
            // TODO: Replace all, of course

            return input.Replace("[HostName]", System.Net.Dns.GetHostName());
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

    public enum NotificationString
    {
        Unknown=0,
        ConcernedPersonName,
        ActingPersonName,
        TertiaryPersonName,
        OrganizationName,
        DateTime,
        DateTimeExpiry,
        BudgetName,
        BudgetAmountFloat,
        EmbeddedPreformattedList,
        HostName,
        SwarmopsVersion
    }

    [Serializable]
    public class NotificationStrings: Dictionary<NotificationString, string>
    {
        // typeset
    }

}
