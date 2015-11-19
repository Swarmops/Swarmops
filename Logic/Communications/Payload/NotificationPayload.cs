using System;
using System.Globalization;
using System.Net;
using Swarmops.Database;
using Swarmops.Logic.Resources;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications.Payload
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

        public NotificationPayload(string notificationResource, NotificationStrings strings) :
            this(notificationResource, strings, new NotificationCustomStrings())
        {
            // also redirect to main ctor
        }

        public NotificationPayload(string notificationResource, NotificationCustomStrings customStrings) :
            this(notificationResource, new NotificationStrings(), customStrings)
        {
            // also redirect to main ctor
        }

        public NotificationPayload(string notificationResource, NotificationStrings strings, NotificationCustomStrings customStrings)
        {
            SubjectResource = notificationResource + "_Subject";
            BodyResource = notificationResource + "_Body";
            Strings = strings;
            CustomStrings = customStrings;
        }

        public string SubjectResource { get; set; }
        public string BodyResource { get; set; }
        public NotificationStrings Strings { get; set; }
        public NotificationCustomStrings CustomStrings { get; set; }

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

            if (Strings != null)
            {
                foreach (NotificationString notificationString in Strings.Keys)
                {
                    // TODO: Check if string ends in Float, and if so, parse and culturize it

                    input = input.Replace ("[" + notificationString /* this is an enum converted to string */+ "]",
                        Strings[notificationString]);
                }
            }

            if (CustomStrings != null)
            {
                foreach (string notificationCustomString in CustomStrings.Keys)
                {
                    input =
                        input.Replace (
                            "[" + notificationCustomString /* this is a System.String as opposed to above */+ "]",
                            CustomStrings[notificationCustomString]);
                }
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
        Participant_Churned,
        Bitcoin_PaidOut,
        Bitcoin_Shortage,
        Bitcoin_Shortage_Urgent,
        Bitcoin_Shortage_Critical,
        BitcoinPayoutAddress_Set,
        BitcoinPayoutAddress_Bad,
        BitcoinPayoutAddress_PleaseSet,
        BitcoinPayoutAddress_OfficerNotify,
        BitcoinPayoutAddress_Changed
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

    [Serializable]
    public class NotificationCustomStrings : SerializableDictionary<string, string>
    {
        // typeset
    }
}