using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialAccount : IHasIdentity, IHasParentIdentity
    {
        public BasicFinancialAccount (int financialAccountId, string name, FinancialAccountType accountType,
            int organizationId, int parentFinancialAccountId, int ownerPersonId,
            bool open, int openedYear, int closedYear, bool active, bool expensable,
            bool administrative, int linkBackward, int linkForward)
        {
            FinancialAccountId = financialAccountId;
            Name = name;
            AccountType = accountType;
            OrganizationId = organizationId;
            OwnerPersonId = ownerPersonId;
            ParentFinancialAccountId = parentFinancialAccountId;
            Open = open;
            OpenedYear = openedYear;
            ClosedYear = closedYear;
            Active = active;
            Expensable = expensable;
            Administrative = administrative;
            LinkBackward = linkBackward;
            LinkForward = linkForward;
        }

        public BasicFinancialAccount (BasicFinancialAccount original) :
            this (
            original.Identity, original.Name, original.AccountType, original.OrganizationId,
            original.ParentFinancialAccountId, original.OwnerPersonId, original.Open, original.OpenedYear,
            original.ClosedYear, original.Active, original.Expensable, original.Administrative, original.LinkBackward,
            original.LinkForward)
        {
            // Empty copy constructor
        }


        public int FinancialAccountId { get; private set; }
        public string Name { get; protected set; }
        public int OrganizationId { get; private set; }
        public FinancialAccountType AccountType { get; private set; }
        public int OwnerPersonId { get; protected set; }
        public int ParentFinancialAccountId { get; protected set; }
        public bool Open { get; protected set; }
        public int OpenedYear { get; protected set; }
        public int ClosedYear { get; protected set; }
        public bool Active { get; protected set; }
        public bool Expensable { get; protected set; }
        public bool Administrative { get; protected set; }
        public int LinkBackward { protected get; set; }
        public int LinkForward { protected get; set; }

        #region Interface Members

        public int Identity { get { return FinancialAccountId; } }
        public int ParentIdentity { get { return ParentFinancialAccountId; } }

        #endregion
    }
}