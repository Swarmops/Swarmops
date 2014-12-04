using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicSupplier : IHasIdentity
    {
        public BasicSupplier(int supplierId, string name, string email, string passwordHash, string bankAccount,
            DateTime createdDateTime, int createdByPersonId)
        {
            SupplierId = supplierId;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            BankAccount = bankAccount;
            CreatedDateTime = createdDateTime;
            CreatedByPersonId = createdByPersonId;
        }

        public BasicSupplier(BasicSupplier original)
            : this(
                original.SupplierId, original.Name, original.Email, original.PasswordHash, original.BankAccount,
                original.CreatedDateTime, original.CreatedByPersonId)
        {
            // empty copy ctor
        }

        public int SupplierId { get; private set; }
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string PasswordHash { get; protected set; }
        public string BankAccount { get; protected set; }
        public DateTime CreatedDateTime { get; private set; }
        public int CreatedByPersonId { get; private set; }

        public int Identity
        {
            get { return SupplierId; }
        }
    }
}