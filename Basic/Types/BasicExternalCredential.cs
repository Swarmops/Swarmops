using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicExternalCredential : IHasIdentity
    {
        public BasicExternalCredential (int externalCredentialId, string serviceName, string login, string password)
        {
            this.ExternalCredentialId = externalCredentialId;
            this.ServiceName = serviceName;
            this.Login = login;
            this.Password = password;
        }

        public readonly int ExternalCredentialId;
        public readonly string ServiceName;
        public readonly string Login;
        public readonly string Password;


        public int Identity
        {
            get { return this.ExternalCredentialId; }
        }
    }
}