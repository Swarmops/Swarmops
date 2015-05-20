using System;
using System.Globalization;
using System.Net;
using Swarmops.Database;
using Swarmops.Logic.Resources;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications.Transmission
{
    [Serializable]
    public class NotificationPayload : PayloadBase<NotificationPayload>, ICommsRenderer
    {
        [Obsolete (
            "Do not use this direct constructor. It is intended for XML serialization only. Therefore, you're getting a compile-time error.",
            true)]
        public NotificationPayload()
        {
            // empty ctor, for XML reflection. Do not use.
        }

        public NotificationPayload (string notificationResource)
            : this (notificationResource, new NotificationStrings())
        {
            // redirect to main ctor
        }

        public NotificationPayload (string notificationResource, NotificationStrings strings)
        {
            SubjectResource = notificationResource + "_Subject";
            BodyResource = notificationResource + "_Body";
            Strings = strings;
        }

        public string SubjectResource { get; set; }
        public string BodyResource { get; set; }
        public NotificationStrings Strings { get; set; }

        public string GetSubject(string cultureId)
        {
            return
                ExpandMacros (
                    Logic_Communications_Transmission_NotificationPayload.ResourceManager.GetString (SubjectResource, CultureInfo.CreateSpecificCulture (cultureId)));
        }

        public string GetBody(string cultureId)
        {
            return
                ExpandMacros (
                    Logic_Communications_Transmission_NotificationPayload.ResourceManager.GetString (BodyResource, CultureInfo.CreateSpecificCulture (cultureId)));
        }

        public string ExpandMacros (string input)
        {
            // Replace a few technical items that would be found in system-level notifications

            input = input.Replace ("[HostName]", Dns.GetHostName());
            input = input.Replace ("[DbVersion]", SwarmDb.DbVersionExpected.ToString(CultureInfo.InvariantCulture));
            input = input.Replace ("[SwarmopsVersion]", Formatting.SwarmopsVersion);
            input = input.Replace ("[InstallationName]", SystemSettings.InstallationName);
            input = input.Replace ("[ExternalUrl]", SystemSettings.ExternalUrl);

            // Loop through supplied strings and replace them in the resource. Not very efficient but who cares

            foreach (NotificationString notificationString in Strings.Keys)
            {
                // TODO: Check if string ends in Float, and if so, parse and culturize it

                input = input.Replace ("[" + notificationString + "]", Strings[notificationString]);
            }

            return input;
        }

        #region Implementation of ICommsRenderer

        public RenderedComm RenderComm (Person person)
        {
            string culture = person.PreferredCulture;
            if (string.IsNullOrEmpty (culture))
            {
                culture = "en-US";
            }

            RenderedComm result = new RenderedComm();

            result[CommRenderPart.Subject] = GetSubject(culture);
            result[CommRenderPart.BodyText] = GetBody(culture);
            result[CommRenderPart.SenderName] = SystemSettings.AdminNotificationSender; // TODO: Make dependent on an enum
            result[CommRenderPart.SenderMail] = SystemSettings.AdminNotificationAddress;

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
        System_DatabaseUpgradeFailed,
        System_MailServerTest,
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
        Receipts_Filed,
        Password_ResetOnRequest,
        Password_ResetByCrew,
        Password_CannotReset2FA,
        Password_Changed,
        Participant_Signup,
        Participant_Volunteer,
        Participant_Renewed,
        Participant_Terminated,
        Participant_Churned
    }

// ReSharper restore InconsistentNaming

    public enum NotificationString
    {
        Unknown = 0,
        ActingPersonName,
        BudgetAmountFloat,
        BudgetName,
        ConcernedPersonName,
        CurrencyCode,
        DateTime,
        DateTimeExpiry,
        Description,
        EmbeddedPreformattedText,
        OrganizationName,
        MailAddress,
        RequestPurpose,
        TertiaryPersonName,
        TicketCode,
        Supplier,
        GeographyName,
        RegularTitle,
        ActivistTitle
    }

    [Serializable]
    public class NotificationStrings : Basic.Types.Common.SerializableDictionary<NotificationString, string>
    {
        // typeset
    }
}