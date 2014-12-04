using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicExternalIdentity : IHasIdentity
    {
        private readonly int attachedToPersonID;
        private readonly int externalIdentityIdentity;

        private readonly string externalSystem;
        private readonly string password;
        private readonly ExternalIdentityType typeOfAccount;
        private readonly string userID;

        public BasicExternalIdentity(int ExternalIdentityIdentity,
            ExternalIdentityType TypeOfAccount,
            string ExternalSystem,
            string UserID,
            string Password,
            int AttachedToPersonID)
        {
            this.externalIdentityIdentity = ExternalIdentityIdentity;
            this.externalSystem = ExternalSystem;
            this.userID = UserID;
            this.password = Password;
            this.attachedToPersonID = AttachedToPersonID;
            this.typeOfAccount = TypeOfAccount;
        }

        public BasicExternalIdentity(BasicExternalIdentity original)
            : this(
                original.Identity, original.TypeOfAccount, original.ExternalSystem, original.UserID, original.Password,
                original.AttachedToPersonID)
        {
        }

        public int ExternalIdentityIdentity
        {
            get { return this.externalIdentityIdentity; }
        }

        public string ExternalSystem
        {
            get { return this.externalSystem; }
        }

        public string UserID
        {
            get { return this.userID; }
        }

        public string Password
        {
            get { return this.password; }
        }

        public int AttachedToPersonID
        {
            get { return this.attachedToPersonID; }
        }

        public ExternalIdentityType TypeOfAccount
        {
            get { return this.typeOfAccount; }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return ExternalIdentityIdentity; }
        }

        #endregion
    }
}