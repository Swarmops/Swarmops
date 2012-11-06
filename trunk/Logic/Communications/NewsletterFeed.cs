using Activizr.Basic.Types;
using Activizr.Database;
using System;

namespace Activizr.Logic.Communications
{
    public class NewsletterFeed : BasicNewsletterFeed
    {
        //HACK: sensitive to database ids changing.
        //NOTE: Added meaning to organizationId == 0 => Returned for all organisations (To handle finland and notifications)
        //HACK: Should better be implemented as an extra field to set on the feed to indicate type, so it would be possible to subscribe to notifications for only some orgs.
        public static class TypeID
        {
            public static readonly int MemberMail = 1;
            public static readonly int ChairmanBlog = 2;
            public static readonly int SMSNewsAlert = 4;
            public static readonly int LocalMail = 5;
            public static readonly int OfficerUpwardCopies = 7;
            public static readonly int OfficerNewMembers = 8;
        }

        #region Creation and Construction

        private NewsletterFeed (BasicNewsletterFeed basic)
            : base(basic)
        {
        }

        public static NewsletterFeed FromIdentity (int identity)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetNewsletterFeed((int)identity));
        }

        public static NewsletterFeed FromBasic (BasicNewsletterFeed basic)
        {
            return new NewsletterFeed(basic);
        }

        #endregion
    }
}