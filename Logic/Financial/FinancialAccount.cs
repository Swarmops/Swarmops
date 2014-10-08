using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Interfaces;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class FinancialAccount : BasicFinancialAccount, ITreeNode, IOwnerSettable
    {
        private FinancialAccount (BasicFinancialAccount basic)
            : base(basic)
        {
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use; use BalanceTotalCents instead.", true)]
        public double BalanceTotal
        {
            get { return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceTotal(Identity); }
        }

        public Int64 BalanceTotalCents
        {
            get { return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceTotalCents(Identity); }
        }

        public static FinancialAccount FromIdentity(int identity)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetFinancialAccount(identity));
        }

        public static FinancialAccount FromIdentityAggressive(int identity)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetFinancialAccount(identity));  // Note "for writing". Intentional; this bypasses the replication lag from master to slave.
        }

        public static FinancialAccount FromBasic(BasicFinancialAccount basic)
        {
            return new FinancialAccount(basic);
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use; use GetDeltaCents instead.", true)]
        public decimal GetDelta(DateTime start, DateTime end)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceDelta(Identity, start, end);
        }

        public Int64 GetDeltaCents(DateTime start, DateTime end)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceDeltaCents(Identity, start, end);
        }

        public FinancialAccountRows GetLastRows(int rowCount)
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading().GetLastFinancialAccountRows(Identity, rowCount);
            return FinancialAccountRows.FromArray(basicRows);
        }

        public FinancialAccountRows GetRows(DateTime start, DateTime end)
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading().GetFinancialAccountRows(Identity, start, end, false);
            return FinancialAccountRows.FromArray(basicRows);
        }

        public FinancialAccountRows GetRowsFar(DateTime start, DateTime end) // selects lowerbound < x <= upperbound
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading().GetFinancialAccountRows(Identity, start, end, true);
            return FinancialAccountRows.FromArray(basicRows);
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use; use GetBudgetCents instead.")]
        public double GetBudget(int year)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBudget(this.Identity, year);
        }

        public Int64 GetBudgetCents (int year)
        {
            return (Int64) (SwarmDb.GetDatabaseForReading().GetFinancialAccountBudget(this.Identity, year)*100);
        }

        public Int64[] GetBudgetMonthly (int year)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBudgetMonthly(this.Identity, year);
        }

        [Obsolete("This method uses floating point for financials. Deprecated. Do not use.")]
        public void SetBudget(int year, double amount)
        {
            SwarmDb.GetDatabaseForWriting().SetFinancialAccountBudget(this.Identity, year, amount);
        }

        public void SetBudgetCents (int year, Int64 amount)
        {
            SwarmDb.GetDatabaseForWriting().SetFinancialAccountBudget(this.Identity, year, amount / 100.0);  // TODO: Change db structure to use cents
        }

        public void SetBudgetMontly (int year, int month, Int64 amountCents)
        {
            SwarmDb.GetDatabaseForWriting().SetFinancialAccountBudgetMonthly(this.Identity, year, month, amountCents);
        }

        public static FinancialAccount Create (int organizationId, string name, FinancialAccountType accountType, int parentFinancialAccountId)
        {
            int accountId = SwarmDb.GetDatabaseForWriting().CreateFinancialAccount(organizationId, name, accountType, parentFinancialAccountId);
            return FromIdentityAggressive(accountId);
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
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountOwner(this.Identity, value.Identity);
            }
        }


        public FinancialAccounts Children
        {
            get { return FinancialAccounts.FromArray(SwarmDb.GetDatabaseForReading().GetFinancialAccountChildren(this.Identity)); }
        }

        public FinancialAccounts GetTree ()
        {
            return FinancialAccounts.GetTree(this);
        }

        public FinancialAccount Parent
        {
            get
            {
                return FromIdentity(this.ParentFinancialAccountId);
            }
            set
            {
                // We do not change the parent lightly. It cannot be changed if this account has transactions
                // in closedledger space.

                try
                {
                    int yearClosed = this.Organization.Parameters.FiscalBooksClosedUntilYear;
                    if (yearClosed > 0 && this.OpenedYear <= yearClosed)
                    {
                        throw new InvalidOperationException("Can't reparent an account with transactions in closedledger space");
                    }
                }
                catch (ArgumentException)
                {
                    // This is ok. OpenedYear can throw an ArgumentException if there aren't any transactions yet.
                }

                int newParentId = value != null ? value.Identity : 0;

                base.ParentFinancialAccountId = newParentId;
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountParent(this.Identity, newParentId);
            }
        }


        public bool IsConferenceParent
        {
            get
            {
                return
                    this.OptionalData.GetOptionalDataBool(ObjectOptionalDataType.FinancialAccountEnabledForConferences);
            }
            set
            {
                this.OptionalData.SetOptionalDataBool(ObjectOptionalDataType.FinancialAccountEnabledForConferences, value);
            }
        }

        public ExternalBankDataProfile ExternalBankDataProfile
        {
            get
            {
                // HACK HACK HACK HACK HACK for our pilots; this will be softcoded later TODO

                if (this.OrganizationId != 1 || !PilotInstallationIds.IsPilot(PilotInstallationIds.PiratePartySE))
                {
                    return null;
                }

                switch (this.FinancialAccountId)
                {
                    case 1:
                        return ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.SESebId);
                    case 2:
                        return ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.PaypalId);
                    default:
                        return null;
                }
            }
        }


        public new string Name
        {
            get { return base.Name; }
            set
            {
                try
                {
                    int yearClosed = this.Organization.Parameters.FiscalBooksClosedUntilYear;
                    if (yearClosed > 0 && this.OpenedYear <= yearClosed)
                    {
                        throw new InvalidOperationException("Can't rename an account with transactions in closedledger space");
                    }
                }
                catch (ArgumentException)
                {
                    // This is ok and can be thrown if OpenedYear throws because there are no transactions yet.
                }

                SwarmDb.GetDatabaseForWriting().SetFinancialAccountName(this.Identity, value);
                base.Name = value;
            }
        }


        public new int OpenedYear
        {
            get
            {
                int testYear = base.OpenedYear;
                if (testYear < 1900) // not initialized
                {
                    DateTime firstTransactionDate = DateTime.MinValue;
                    try
                    {
                        firstTransactionDate =
                            SwarmDb.GetDatabaseForReading().GetFinancialAccountFirstTransactionDate(this.Identity);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }

                    if (firstTransactionDate.Year > 1900)
                    {
                        SwarmDb.GetDatabaseForWriting().SetFinancialAccountOpenedYear(this.Identity, firstTransactionDate.Year);
                        base.OpenedYear = firstTransactionDate.Year; // TODO: Adapt for non-calendar fiscal years as future expansion?
                    }
                }

                if (base.OpenedYear > 1900)
                {
                    return base.OpenedYear;
                }
                else
                {
                    throw new ArgumentException("FinancialAccount is not opened yet (no transactions)");
                }
            }
        }

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                if (base.Active == value)
                {
                    return;
                }

                base.Active = value;
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountActive(this.Identity, value);
            }
        }


        public new bool Expensable
        {
            get { return base.Expensable; }
            set
            {
                if (base.Expensable == value)
                {
                    return;
                }

                base.Expensable = value;
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountExpensable(this.Identity, value);
            }
        }


        public new bool Administrative
        {
            get { return base.Administrative; }
            set
            {
                if (base.Administrative == value)
                {
                    return;
                }

                base.Administrative = value;
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountAdministrative(this.Identity, value);
            }
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

        #region IOwnerSettable members

        public void SetOwner(Person newOwner)
        {
            this.Owner = newOwner;
        }

        #endregion
    }
}