using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicExternalIdentity : IHasIdentity
    {

        public BasicExternalIdentity ( int ExternalIdentityIdentity,
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

        public BasicExternalIdentity (BasicExternalIdentity original)
            : this(original.Identity, original.TypeOfAccount, original.ExternalSystem, original.UserID, original.Password, original.AttachedToPersonID)
        {
        }

        private int externalIdentityIdentity;

        private string externalSystem;
        private string userID;
        private string password;
        private int attachedToPersonID;
        private ExternalIdentityType typeOfAccount;
        
        public int ExternalIdentityIdentity
        {
            get { return externalIdentityIdentity; }
        }

        public string ExternalSystem
        {
            get
            {
                return externalSystem;
            }
        }
        public string UserID
        {
            get
            {
                return userID;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
        }
        public int AttachedToPersonID
        {
            get
            {
                return attachedToPersonID;
            }
        }
        public ExternalIdentityType TypeOfAccount
        {
            get
            {
                return typeOfAccount;
            }
        }


        #region IHasIdentity Members

        public int Identity
        {
            get { return ExternalIdentityIdentity; }
        }

        #endregion
    }
}
