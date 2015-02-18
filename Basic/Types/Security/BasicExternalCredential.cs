using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Security
{
    public class BasicExternalCredential : IHasIdentity
    {
        public readonly int ExternalCredentialId;
        public readonly string Login;
        public readonly string Password;
        public readonly string ServiceName;

        public BasicExternalCredential (int externalCredentialId, string serviceName, string login, string password)
        {
            this.ExternalCredentialId = externalCredentialId;
            this.ServiceName = serviceName;
            this.Login = login;
            this.Password = password;
        }


        public int Identity
        {
            get { return this.ExternalCredentialId; }
        }
    }
}