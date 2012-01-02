using System;
using Activizr.Logic.Interfaces;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Basic.Enums;

namespace Activizr.Logic.Financial
{
    public class FinancialAccount : BasicFinancialAccount, ITreeNode
    {
        private FinancialAccount (BasicFinancialAccount basic)
            : base(basic)
        {
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use.")]
        public double BalanceTotal
        {
            get { return PirateDb.GetDatabase().GetFinancialAccountBalanceTotal(Identity); }
        }

        public Int64 BalanceTotalCents
        {
            get { return PirateDb.GetDatabase().GetFinancialAccountBalanceTotalCents(Identity); }
        }

        public static FinancialAccount FromIdentity (int identity)
        {
            return FromBasic(PirateDb.GetDatabase().GetFinancialAccount(identity));
        }

        public static FinancialAccount FromBasic (BasicFinancialAccount basic)
        {
            return new FinancialAccount(basic);
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use.")]
        public decimal GetDelta(DateTime start, DateTime end)
        {
            return PirateDb.GetDatabase().GetFinancialAccountBalanceDelta(Identity, start, end);
        }

        public Int64 GetDeltaCents(DateTime start, DateTime end)
        {
            return PirateDb.GetDatabase().GetFinancialAccountBalanceDeltaCents(Identity, start, end);
        }

        public FinancialAccountRows GetLastRows(int rowCount)
        {
            BasicFinancialAccountRow[] basicRows = PirateDb.GetDatabase().GetLastFinancialAccountRows(Identity, rowCount);
            return FinancialAccountRows.FromArray(basicRows);
        }

        public FinancialAccountRows GetRows(DateTime start, DateTime end)
        {
            BasicFinancialAccountRow[] basicRows = PirateDb.GetDatabase().GetFinancialAccountRows(Identity, start, end);
            return FinancialAccountRows.FromArray(basicRows);
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use.")]
        public double GetBudget(int year)
        {
            return PirateDb.GetDatabase().GetFinancialAccountBudget(this.Identity, year);
        }

        public Int64 GetBudgetCents (int year)
        {
            return (Int64) (GetBudget(year)*100);
        }

        public Int64[] GetBudgetMonthly (int year)
        {
            return PirateDb.GetDatabase().GetFinancialAccountBudgetMonthly(this.Identity, year);
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use.")]
        public void SetBudget(int year, double amount)
        {
            PirateDb.GetDatabase().SetFinancialAccountBudget(this.Identity, year, amount);
        }

        public void SetBudgetMontly (int year, int month, Int64 amountCents)
        {
            PirateDb.GetDatabase().SetFinancialAccountBudgetMonthly(this.Identity, year, month, amountCents);
        }

        public static FinancialAccount Create (int pOrganizationId, string pName, FinancialAccountType pAccountType, int pParentFinancialAccountId)
        {
            int accountId = PirateDb.GetDatabase().CreateFinancialAccount(pOrganizationId, pName, pAccountType, pParentFinancialAccountId);
            return FromIdentity(accountId);
        }

        public Organization Organization
        {
            get
            {
                return Organization.FromIdentity(this.OrganizationId);  // TODO: Cache
            }
        }

        public Person Owner
        {
            get
            {
                if (this.OwnerPersonId == 0)
                {
                    return null;
                }

                return Person.FromIdentity(this.OwnerPersonId);
            }
            set
            {
                PirateDb.GetDatabase().SetFinancialAccountOwner(this.Identity, value.Identity);
            }
        }


        public FinancialAccounts Children
        {
            get { return FinancialAccounts.FromArray(PirateDb.GetDatabase().GetFinancialAccountChildren(this.Identity)); }
        }

        public FinancialAccounts GetTree ()
        {
            return FinancialAccounts.GetTree(this);
        }

        public FinancialAccount Parent
        {
            get { return FromIdentity(this.ParentFinancialAccountId); }
        }


        #region ITreeNode Members


        public int ParentIdentity
        {
            get { return this.ParentFinancialAccountId; }
        }

        public int[] ChildrenIdentities
        {
            get { return Children.Identities; }
        }

        public Geography AssignedGeography
        {
            get
            {
                int geographyId = OptionalData.GetOptionalDataInt(ObjectOptionalDataType.GeographyId);

                if (geographyId != 0)
                {
                    return Geography.FromIdentity(geographyId);
                }

                return null;
            }
            set
            {
                if (value != null)
                    OptionalData.SetOptionalDataInt(ObjectOptionalDataType.GeographyId, value.Identity);
                else
                    OptionalData.SetOptionalDataInt(ObjectOptionalDataType.GeographyId, 0);
            }
        }

        public string BankTransactionTag
        {
            get
            {
                if (!String.IsNullOrEmpty(OptionalData.GetOptionalDataString(ObjectOptionalDataType.BankTransactionTag)))
                {
                    return OptionalData.GetOptionalDataString(ObjectOptionalDataType.BankTransactionTag);
                }

                return null;
            }
            set
            {
                OptionalData.SetOptionalDataString(ObjectOptionalDataType.BankTransactionTag, value.ToLower());
            }
        }

        private ObjectOptionalData OptionalData
        {
            get
            {
                if (_optionalData == null)
                {
                    FinancialAccount acc = (FinancialAccount)this;
                    _optionalData = ObjectOptionalData.ForObject(acc); //Added cast, otherwise it fails for subclasses
                }
                return _optionalData;
            }
        }

        private ObjectOptionalData _optionalData;

        #endregion
    }
}