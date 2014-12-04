using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    // ReSharper disable InconsistentNaming
    public class PWLog
        // ReSharper restore InconsistentNaming
    {
        private static string hostIp = ""; //Used in batch programs

        public static void Write(DateTime dateTimeUtc, int actingPersonId, PWLogItem affectedItem,
            int affectedItemId,
            PWLogAction actionType, string actionDescription, string comment,
            string changedField, string valueBefore, string valueAfter)
        {
            SwarmDb.GetDatabaseForWriting().CreatePWLogEntry(dateTimeUtc, actingPersonId, affectedItem.ToString(),
                affectedItemId, actionType.ToString(), actionDescription,
                changedField, valueBefore, valueAfter, comment, GetCurrentIp());
        }

        public static void Write(DateTime dateTimeUtc, Person actingPerson, PWLogItem affectedItem,
            int affectedItemId,
            PWLogAction actionType, string actionDescription, string comment,
            string changedField, string valueBefore, string valueAfter)
        {
            Write(dateTimeUtc, actingPerson.Identity, affectedItem, affectedItemId, actionType,
                actionDescription, comment, changedField, valueBefore, valueAfter);
        }

        public static void Write(Person actingPerson, PWLogItem affectedItem, int affectedItemId,
            PWLogAction actionType,
            string actionDescription, string comment, string changedField, string valueBefore,
            string valueAfter)
        {
            Write(DateTime.UtcNow, actingPerson.Identity, affectedItem, affectedItemId, actionType,
                actionDescription, comment, changedField, valueBefore, valueAfter);
        }

        public static void Write(Person actingPerson, PWLogItem affectedItem, int affectedItemId,
            PWLogAction actionType,
            string actionDescription, string comment)
        {
            Write(DateTime.UtcNow, actingPerson.Identity, affectedItem, affectedItemId, actionType,
                actionDescription, comment, string.Empty, string.Empty, string.Empty);
        }

        public static void Write(int actingPersonId, PWLogItem affectedItem, int affectedItemId,
            PWLogAction actionType,
            string actionDescription, string comment)
        {
            if (actingPersonId > 0 && comment == "")
            {
                try
                {
                    comment = "Action done by " + Person.FromIdentity(actingPersonId).Name;
                }
                catch
                {
                }
            }
            Write(DateTime.UtcNow, actingPersonId, affectedItem, affectedItemId, actionType,
                actionDescription, comment, string.Empty, string.Empty, string.Empty);
        }

        public static void Write(PWLogItem affectedItem, int affectedItemId, PWLogAction actionType,
            string actionDescription, string comment)
        {
            Write(DateTime.UtcNow, 0, affectedItem, affectedItemId, actionType,
                actionDescription, comment, string.Empty, string.Empty, string.Empty);
        }

        private static string GetCurrentIp()
        {
            try
            {
                //Get IP from current web request, if available
                return HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                //Probably in a batch program,
                if (hostIp == "")
                {
                    try
                    {
                        hostIp = "n/a";
                        string myHostName = Dns.GetHostName();
                        hostIp = myHostName;
                        IPHostEntry ipEntry = Dns.GetHostEntry(myHostName);
                        IPAddress[] addr = ipEntry.AddressList;
                        hostIp = addr[0].ToString();
                    }
                    catch
                    {
                    }
                }
                return hostIp;
            }
        }

        public static DateTime CheckLatest(PWLogItem affectedItemType, int affectedItemId, PWLogAction actionType)
        {
            return SwarmDb.GetDatabaseForReading()
                .CheckLogEntry(affectedItemType.ToString(), affectedItemId, actionType.ToString());
        }

        public static BasicPWLog[] GetLatestEvents(PWLogItem affectedItemType, DateTime beforeDate, int[] affectedIds,
            PWLogAction[] actionTypes)
        {
            List<string> actionTypeList = new List<string>();
            foreach (PWLogAction at in actionTypes)
                actionTypeList.Add(at.ToString());

            return SwarmDb.GetDatabaseForReading()
                .GetLatestEvents(affectedItemType.ToString(), beforeDate, affectedIds, actionTypeList.ToArray());
        }
    }

    public enum PWLogAction
    {
        /// <summary>
        ///     Undefined. Do not use.
        /// </summary>
        Undefined = 0,

        /// <summary>
        ///     A new member has joined on his/her own accord.
        /// </summary>
        MemberJoin,

        /// <summary>
        ///     A new member was added by an officer.
        /// </summary>
        MemberAdd,

        /// <summary>
        ///     A new membership was added to an existing member.
        /// </summary>
        MembershipAdd,

        /// <summary>
        ///     A membership was removed.
        /// </summary>
        MembershipRemove,

        /// <summary>
        ///     A membership was transferred to another org.
        /// </summary>
        MembershipTransfer,

        /// <summary>
        ///     A member has been removed from the roster.
        /// </summary>
        MemberLost,

        /// <summary>
        ///     An activist has joined.
        /// </summary>
        ActivistJoin,

        /// <summary>
        ///     An activist was lost.
        /// </summary>
        ActivistLost,

        /// <summary>
        ///     Personal data was changed.
        /// </summary>
        PersonFieldChange,

        /// <summary>
        ///     Part of systems tests. Do not use.
        /// </summary>
        SystemTest,

        /// <summary>
        ///     A member was reminded to renew.
        /// </summary>
        MembershipRenewReminder,

        /// <summary>
        ///     A membership was renewed.
        /// </summary>
        MembershipRenewed,

        /// <summary>
        ///     A role (lead, admin, etc.) was assigned.
        /// </summary>
        RoleAssigned,

        /// <summary>
        ///     A role was removed.
        /// </summary>
        RoleDeleted,

        /// <summary>
        ///     Person created (usually for debug purposes).
        /// </summary>
        PersonCreated,

        /// <summary>
        ///     Mail account changes
        /// </summary>
        MailAccountChanged,

        /// <summary>
        ///     Changes in External Identities
        /// </summary>
        ExtAccountChanged,

        /// <summary>
        ///     A membership is requested to be renewed.
        /// </summary>
        MembershipRenewalRequest,

        /// <summary>
        ///     Logged info about a failure
        /// </summary>
        Failure,

        /// <summary>
        ///     SMS message Handled
        /// </summary>
        SMSHandled,

        /// <summary>
        ///     Starting Impersonation of other user
        /// </summary>
        StartImpersonation
    }

    public enum PWLogItem
    {
        None = 0,
        Person,
        MailAccount,
        ExtAccount
    }
}