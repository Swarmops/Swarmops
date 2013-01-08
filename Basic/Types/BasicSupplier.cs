using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicSupplier: IHasIdentity
    {
        public BasicSupplier (int supplierId, string name, string email, string passwordHash, string bankAccount, DateTime createdDateTime, int createdByPersonId)
        {
            this.SupplierId = supplierId;
            this.Name = name;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.BankAccount = bankAccount;
            this.CreatedDateTime = createdDateTime;
            this.CreatedByPersonId = createdByPersonId;
        }

        public BasicSupplier(BasicSupplier original): this (original.SupplierId, original.Name, original.Email, original.PasswordHash, original.BankAccount, original.CreatedDateTime, original.CreatedByPersonId)
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
            get { return this.SupplierId; }
        }
    }
}
