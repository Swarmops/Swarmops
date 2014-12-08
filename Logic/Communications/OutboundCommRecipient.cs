using Swarmops.Basic.Types.Communications;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class OutboundCommRecipient : BasicOutboundCommRecipient
    {
        private OutboundCommRecipient (BasicOutboundCommRecipient basic) : base (basic)
        {
            // private ctor
        }

        public Person Person
        {
            get { return Person.FromIdentity (base.PersonId); }
        }

        public static OutboundCommRecipient FromBasic (BasicOutboundCommRecipient basic)
        {
            return new OutboundCommRecipient (basic);
        }

        public void CloseSuccess()
        {
            SwarmDb.GetDatabaseForWriting().SetOutboundCommRecipientClosed (Identity);

            base.Success = true;
            base.Open = false;
        }

        public void CloseFailed (string failReason)
        {
            SwarmDb.GetDatabaseForWriting().SetOutboundCommRecipientFailed (Identity, failReason);

            base.Success = false;
            base.Open = false;
        }

        // TODO: CloseSuccess, CloseFail
    }
}