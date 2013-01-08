using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Communications;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Communications
{
    public class AutoMail : BasicAutoMail
    {
        private AutoMail() : base (null)
        {
        } // Private constructor

        private AutoMail (BasicAutoMail basic) : base (basic)
        {
        }

        public new string Body
        {
            get { return base.Body; }

            set
            {
                base.Body = value;
                PirateDb.GetDatabaseForWriting().SetAutoMail (this); // saves changes
            }
        }


        internal static AutoMail FromBasic (BasicAutoMail basic)
        {
            return new AutoMail (basic);
        }


        public static AutoMail FromTypeOrganizationAndGeography (AutoMailType type, Organization org, Geography geo)
        {
            BasicAutoMail basic = PirateDb.GetDatabaseForReading().GetAutoMail (type, org.Identity, geo.Identity);

            if (basic == null)
            {
                return null;
            }

            if (basic.Body.Trim().Length < 3)
            {
                return null; // If there is no body, there is no mail
            }

            return FromBasic (basic);
        }


        public static AutoMail Create (AutoMailType type, Organization org, Geography geo,
                                       Person author, string title, string body)
        {
            PirateDb.GetDatabaseForWriting().SetAutoMail (type, org.Identity, geo.Identity,
                                                author == null ? 0 : author.Identity, title, body);
            return FromTypeOrganizationAndGeography (type, org, geo);
        }


        // For the time being, only the body and intro have "Set" functions, as they're the only fields
        // saveable from
        // the interface. This is supposed to grow to include title and authorId too.

        public static BasicAutoMail[] GetAllForMigration()
        {
            return PirateDb.GetDatabaseForReading().GetAllAutoMailsForMigration();
        }
    }
}