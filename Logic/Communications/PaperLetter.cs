using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class PaperLetter : BasicPaperLetter
    {
        public static PaperLetter Create (Person creator, Organization organization, string fromName,
            string[] replyAddressLines, DateTime receivedDate, Person recipient, RoleType recipientRole,
            bool personal)
        {
            return Create (creator.Identity, organization.Identity, fromName, replyAddressLines, receivedDate,
                recipient.Identity, recipientRole, personal);
        }


        public static PaperLetter Create (int creatingPersonId, int organizationId, string fromName,
            string[] replyAddressLines, DateTime receivedDate, int toPersonId, RoleType toPersonInRole, bool personal)
        {
            return FromIdentity (SwarmDb.GetDatabaseForWriting().
                CreatePaperLetter (organizationId, fromName, String.Join ("|", replyAddressLines),
                    receivedDate, toPersonId, toPersonInRole, personal, creatingPersonId));
        }

        public static PaperLetter FromIdentity (int paperLetterId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetPaperLetter (paperLetterId));
        }

        public static PaperLetter FromBasic (BasicPaperLetter basic)
        {
            return new PaperLetter (basic);
        }

        private PaperLetter (BasicPaperLetter basic) : base (basic)
        {
            // empty private constructor
        }

        public Documents Documents
        {
            get { return Documents.ForObject (this); }
        }

#pragma warning disable 169
// ReSharper disable InconsistentNaming
        private new string ReplyAddress; // hides ReplyAddress in base, quite on purpose
// ReSharper restore InconsistentNaming
#pragma warning restore 169

        public string[] ReplyAddressLines
        {
            get { return base.ReplyAddress.Split ('|'); }
        }

        public Person Recipient
        {
            get
            {
                if (base.ToPersonId == 0)
                {
                    return null;
                }

                return Person.FromIdentity (base.ToPersonId);
            }
        }
    }
}