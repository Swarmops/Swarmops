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
            this.SubjectResource = notificationResource + "_Subject";
            this.BodyResource = notificationResource + "_Body";
            this.Strings = strings;
        }

        public string SubjectResource { get; set; }
        public string BodyResource { get; set; }
        public NotificationStrings Strings { get; set; }

        public string GetSubject()
        {
            // TODO: Pick culture
            
            return ExpandMacros(App_GlobalResources.Logic_Communications_Transmission_NotificationPayload.ResourceManager.GetString(this.SubjectResource));
        }

        public string GetBody()
        {
            // TODO: Pick culture

            return ExpandMacros(App_GlobalResources.Logic_Communications_Transmission_NotificationPayload.ResourceManager.GetString(this.BodyResource));
        }

        public string ExpandMacros (string input)
        {
            // TODO: Replace all, of course

            input = input.Replace("[HostName]", System.Net.Dns.GetHostName());

            // Loop through supplied strings and replace them in the resource. Not very efficient but who cares

            foreach (NotificationString notificationString in Strings.Keys)
            {
                // TODO: Check if string ends in Float, and if so, parse and culturize it

                input = input.Replace("[" + notificationString + "]", Strings[notificationString]);
            }

            return input;
        }

        #region Implementation of ICommsRenderer

        public RenderedComm RenderComm(Person person)
        {
            RenderedComm result = new RenderedComm();

            result[CommRenderPart.Subject] = GetSubject();
            result[CommRenderPart.BodyText] = GetBody();
            result[CommRenderPart.SenderName] = "Swarmops Administrative";
            result[CommRenderPart.SenderMail] = "admin@swarmops.com";

            return result;
        }

        #endregion
    }

// ReSharper disable InconsistentNaming

    public enum NotificationResource
    {
        Unknown = 0,
        System_Startup,
        System_Exception,
        System_DatabaseSchemaUpgraded,
        CashAdvance_Requested,
        CashAdvance_Attested,
        CashAdvance_Deattested,
        CashAdvance_Denied,
        CashAdvance_Adjusted,
        CashAdvance_Rebudgeted,
        CashAdvance_PaidOut,
        ExpenseClaim_Created,
        ExpenseClaim_Attested,
        ExpenseClaim_Deattested,
        ExpenseClaim_Validated,
        ExpenseClaim_Devalidated,
        ExpenseClaim_Denied,
        ExpenseClaim_Adjusted,
        ExpenseClaim_Rebudgeted,
        ExpenseClaim_PaidOut,
        Payout_Settled,
        InboundInvoice_Created,
        Receipts_Filed
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
        CurrencyCode,
        EmbeddedPreformattedText,
        RequestPurpose,
        HostName,
        SwarmopsVersion,
        Supplier,
        Description
    }

    [Serializable]
    public class NotificationStrings: SerializableDictionary<NotificationString, string>
    {
        // typeset
    }

}
