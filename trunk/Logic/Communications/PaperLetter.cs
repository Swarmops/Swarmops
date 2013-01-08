using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Communications
{
    public class PaperLetter: BasicPaperLetter
    {
        public static PaperLetter Create (Person creator, Organization organization, string fromName,
            string[] replyAddressLines, DateTime receivedDate, Person recipient, RoleType recipientRole, 
            bool personal)
        {
            return Create(creator.Identity, organization.Identity, fromName, replyAddressLines, receivedDate,
                          recipient.Identity, recipientRole, personal);
        }


        public static PaperLetter Create(int creatingPersonId, int organizationId, string fromName,
            string[] replyAddressLines, DateTime receivedDate, int toPersonId, RoleType toPersonInRole, bool personal)
        {
            return FromIdentity(PirateDb.GetDatabaseForWriting().
                CreatePaperLetter(organizationId, fromName, String.Join("|", replyAddressLines),
                                  receivedDate, toPersonId, toPersonInRole, personal, creatingPersonId));
        }

        public static PaperLetter FromIdentity (int paperLetterId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetPaperLetter(paperLetterId));
        }

        public static PaperLetter FromBasic (BasicPaperLetter basic)
        {
            return new PaperLetter(basic);
        }

        private PaperLetter (BasicPaperLetter basic): base (basic)
        {
            // empty private constructor
        }

        public Documents Documents
        {
            get { return Support.Documents.ForObject(this); }
        }

#pragma warning disable 169
// ReSharper disable InconsistentNaming
        private new string ReplyAddress;  // hides ReplyAddress in base, quite on purpose
// ReSharper restore InconsistentNaming
#pragma warning restore 169

        public string[] ReplyAddressLines
        {
            get { return base.ReplyAddress.Split('|'); }
        }

        public Person Recipient
        {
            get
            {
                if (base.ToPersonId == 0)
                {
                    return null;
                }

                return Person.FromIdentity(base.ToPersonId);
            }
        }

    }
}
