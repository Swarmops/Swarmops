using System;
using System.Collections.Generic;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Common.Generics;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class FinancialAccount : BasicFinancialAccount, IOwnerSettable
    {
        private FinancialAccount (BasicFinancialAccount basic)
            : base (basic)
        {
        }

        [Obsolete (
            "This method uses floating point for financials. Deprecated. Do not use; use BalanceTotalCents instead.",
            true)]
        public double BalanceTotal
        {
            get { return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceTotal (Identity); }
        }

        public Int64 BalanceTotalCents
        {
            get { return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceTotalCents (Identity); }
        }

        public Organization Organization
        {
            get
            {
                return Organization.FromIdentity (OrganizationId); // TODO: Cache
            }
        }

        public Person Owner
        {
            get
            {
                if (OwnerPersonId == 0)
                {
                    return null;
                }

                return Person.FromIdentity (OwnerPersonId);
            }
            set { SwarmDb.GetDatabaseForWriting().SetFinancialAccountOwner (Identity, value.Identity); }
        }


        public FinancialAccounts Children
        {
            get
            {
                return
                    FinancialAccounts.FromArray (SwarmDb.GetDatabaseForReading().GetFinancialAccountChildren (Identity));
            }
        }

        public FinancialAccount Parent
        {
            get { return FromIdentity (ParentFinancialAccountId); }
            set
            {
                // We do not change the parent lightly. It cannot be changed if this account has transactions
                // in closedledger space.

                try
                {
                    int yearClosed = Organization.Parameters.FiscalBooksClosedUntilYear;
                    if (yearClosed > 0 && OpenedYear <= yearClosed)
                    {
                        throw new InvalidOperationException (
                            "Can't reparent an account with transactions in closedledger space");
                    }
                }
                catch (ArgumentException)
                {
                    // This is ok. OpenedYear can throw an ArgumentException if there aren't any transactions yet.
                }

                int newParentId = value != null ? value.Identity : 0;

                base.ParentFinancialAccountId = newParentId;
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountParent (Identity, newParentId);
            }
        }

        public Tree<FinancialAccount> Tree
        {
            get
            {
                FinancialAccounts accounts = this.ThisAndBelow();
                return Tree<FinancialAccount>.FromCollection (accounts);
            }
        }



        public bool IsConferenceParent
        {
            get
            {
                return
                    OptionalData.GetOptionalDataBool (ObjectOptionalDataType.FinancialAccountEnabledForConferences);
            }
            set
            {
                OptionalData.SetOptionalDataBool (ObjectOptionalDataType.FinancialAccountEnabledForConferences, value);
            }
        }


        public Currency ForeignCurrency
        {
            get
            {
                int currencyId = OptionalData.GetOptionalDataInt (ObjectOptionalDataType.FinancialAccountNativeCurrency); // "Native" word is legacy
                return (currencyId != 0 ? Currency.FromIdentity (currencyId) : null);
            }
            set
            {
                OptionalData.SetOptionalDataInt (ObjectOptionalDataType.FinancialAccountNativeCurrency, value.Identity);
            }
        }


        public Money ForeignBalanceTotalCents
        {
            get
            {
                Int64 foreignCents =
                    SwarmDb.GetDatabaseForReading().GetFinancialAccountForeignCentsBalance (this.Identity);
                return new Money (foreignCents, ForeignCurrency); // default to now
            }
        }


        public string BitcoinAddress
        {
            get
            {
                return OptionalData.GetOptionalDataString (ObjectOptionalDataType.FinancialAccountBitcoinPublicAddress);
            }
            set
            {
                OptionalData.SetOptionalDataString (ObjectOptionalDataType.FinancialAccountBitcoinPublicAddress, value);
            }
        }


        public void CheckForexProfitLoss()
        {
            // Compare current balance in native currency with the current balance in foreign currency. If off by more than 100 cents,
            // log a corrective transaction to account for exchange rate fluctuations.

            DateTime compareDateTime = DateTime.UtcNow;

            Int64 nativeCents = BalanceTotalCents;
            Money foreignCents = new Money(ForeignBalanceTotalCents.Cents, ForeignCurrency, compareDateTime);

            Int64 currentNativeValueOfForeignCents = foreignCents.ToCurrency (Organization.Currency).Cents;

            if (nativeCents - 100 > currentNativeValueOfForeignCents)
            {
                // log a loss

                Int64 lossCents = nativeCents - currentNativeValueOfForeignCents;
                FinancialTransaction lossTx = FinancialTransaction.Create (Organization, DateTime.UtcNow, "Forex Loss");
                lossTx.AddRow (this, -lossCents, null);
                lossTx.AddRow (Organization.FinancialAccounts.CostsCurrencyFluctuations, lossCents, null);
            }
            else if (nativeCents + 100 < currentNativeValueOfForeignCents)
            {
                // log a profit

                Int64 profitCents = currentNativeValueOfForeignCents - nativeCents;
                FinancialTransaction profitTx = FinancialTransaction.Create(Organization, DateTime.UtcNow, "Forex Gains");
                profitTx.AddRow(this, profitCents, null);
                profitTx.AddRow(Organization.FinancialAccounts.IncomeCurrencyFluctuations, -profitCents, null);
            }
        }


        public ExternalBankDataProfile ExternalBankDataProfile
        {
            get
            {
                // HACK HACK HACK HACK HACK for our pilots; this will be softcoded later TODO

                if (OrganizationId == 1 && PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE))
                {
                    switch (FinancialAccountId)
                    {
                        case 1:
                            return ExternalBankDataProfile.FromIdentity (ExternalBankDataProfile.SESebId);
                        case 2:
                            return ExternalBankDataProfile.FromIdentity (ExternalBankDataProfile.PaypalId);
                        default:
                            return null;
                    }
                }
                if (OrganizationId == 7 && PilotInstallationIds.IsPilot (PilotInstallationIds.SwarmopsLive) &&
                    FinancialAccountId == 29)
                {
                    return ExternalBankDataProfile.FromIdentity (ExternalBankDataProfile.SESebId);
                }

                return null;
            }
        }

        public new int OpenedYear
        {
            get
            {
                int testYear = base.OpenedYear;
                if (testYear < 1900) // not initialized
                {
                    DateTime firstTransactionDate = Constants.DateTimeLow;
                    try
                    {
                        firstTransactionDate =
                            SwarmDb.GetDatabaseForReading().GetFinancialAccountFirstTransactionDate (Identity);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }

                    if (firstTransactionDate.Year > 1900)
                    {
                        SwarmDb.GetDatabaseForWriting()
                            .SetFinancialAccountOpenedYear (Identity, firstTransactionDate.Year);
                        base.OpenedYear = firstTransactionDate.Year;
                        // TODO: Adapt for non-calendar fiscal years as future expansion?
                    }
                }

                if (base.OpenedYear > 1900)
                {
                    return base.OpenedYear;
                }
                throw new ArgumentException ("FinancialAccount is not opened yet (no transactions)");
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
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountActive (Identity, value);
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
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountExpensable (Identity, value);
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
                SwarmDb.GetDatabaseForWriting().SetFinancialAccountAdministrative (Identity, value);
            }
        }

        private ObjectOptionalData _optionalData;

        public Geography AssignedGeography
        {
            get
            {
                int geographyId = OptionalData.GetOptionalDataInt (ObjectOptionalDataType.GeographyId);

                if (geographyId != 0)
                {
                    return Geography.FromIdentity (geographyId);
                }

                return null;
            }
            set {
                this.OptionalData.SetOptionalDataInt (ObjectOptionalDataType.GeographyId,
                    value != null ? value.Identity : 0);
            }
        }

        public string BankTransactionTag
        {
            get
            {
                if (
                    !String.IsNullOrEmpty (OptionalData.GetOptionalDataString (ObjectOptionalDataType.BankTransactionTag)))
                {
                    return OptionalData.GetOptionalDataString (ObjectOptionalDataType.BankTransactionTag);
                }

                return null;
            }
            set { OptionalData.SetOptionalDataString (ObjectOptionalDataType.BankTransactionTag, value.ToLower()); }
        }

        private ObjectOptionalData OptionalData
        {
            get
            {
                if (this._optionalData == null)
                {
                    FinancialAccount acc = this;
                    this._optionalData = ObjectOptionalData.ForObject (acc);
                    //Added cast, otherwise it fails for subclasses
                }
                return this._optionalData;
            }
        }

        public int[] ChildrenIdentities
        {
            get { return Children.Identities; }
        }

        #region IOwnerSettable members

        public void SetOwner (Person newOwner)
        {
            Owner = newOwner;
        }

        #endregion

        public new string Name
        {
            get
            {
                if (base.Name.StartsWith ("[LOC]"))
                {
                    string localized = Resources.Logic_Financial_FinancialAccount.ResourceManager.GetString (base.Name.Substring (5));

                    if (!string.IsNullOrEmpty (localized))
                    {
                        return localized;  // We return the localized string only if the lookup succeeds. Otherwise, the raw "[LOC]whatever" will be returned.
                    }
                }
                
                return base.Name;
            }
            set
            {
                try
                {
                    int yearClosed = Organization.Parameters.FiscalBooksClosedUntilYear;
                    if (yearClosed > 0 && OpenedYear <= yearClosed)
                    {
                        throw new InvalidOperationException (
                            "Can't rename an account with transactions in closedledger space");
                    }
                }
                catch (ArgumentException)
                {
                    // This is ok and can be thrown if OpenedYear throws because there are no transactions yet.
                }

                if (value != Name) // check that we're not setting value to the localized version, thereby destroying loc for other users
                {
                    SwarmDb.GetDatabaseForWriting().SetFinancialAccountName (Identity, value);
                    base.Name = value;
                }
            }
        }

        public static FinancialAccount FromIdentity (int identity)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetFinancialAccount (identity));
        }

        public static FinancialAccount FromIdentityAggressive (int identity)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetFinancialAccount (identity));
            // Note "for writing". Intentional; this bypasses the replication lag from master to slave.
        }

        public static FinancialAccount FromBasic (BasicFinancialAccount basic)
        {
            return new FinancialAccount (basic);
        }

        [Obsolete ("This method uses floating point for financials. Deprecated. Do not use; use GetDeltaCents instead.",
            true)]
        public decimal GetDelta (DateTime start, DateTime end)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceDelta (Identity, start, end);
        }

        public Int64 GetDeltaCents (DateTime start, DateTime end)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceDeltaCents (Identity, start, end);
        }

        public FinancialAccountRows GetLastRows (int rowCount)
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading()
                .GetLastFinancialAccountRows (Identity, rowCount);
            return FinancialAccountRows.FromArray (basicRows);
        }

        public FinancialAccountRows GetRows (DateTime start, DateTime end)
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading()
                .GetFinancialAccountRows (Identity, start, end, false);
            return FinancialAccountRows.FromArray (basicRows);
        }

        public FinancialAccountRows GetRowsFar (DateTime start, DateTime end) // selects lowerbound < x <= upperbound
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading()
                .GetFinancialAccountRows (Identity, start, end, true);
            return FinancialAccountRows.FromArray (basicRows);
        }

        [Obsolete ("This method uses floating point for financials. Deprecated. Do not use; use GetBudgetCents instead."
            )
        ]
        public double GetBudget (int year)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBudget (Identity, year);
        }

        public Int64 GetBudgetCents (int year = -1)
        {
            if (year == -1)
            {
                year = DateTime.UtcNow.Year;
            }

            return (Int64) (SwarmDb.GetDatabaseForReading().GetFinancialAccountBudget (Identity, year)*100);
        }

        public Int64[] GetBudgetMonthly (int year)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBudgetMonthly (Identity, year);
        }

        [Obsolete ("This method uses floating point for financials. Deprecated. Do not use.")]
        public void SetBudget (int year, double amount)
        {
            SwarmDb.GetDatabaseForWriting().SetFinancialAccountBudget (Identity, year, amount);
        }

        public void SetBudgetCents (int year, Int64 amount)
        {
            SwarmDb.GetDatabaseForWriting().SetFinancialAccountBudget (Identity, year, amount/100.0);
            // TODO: Change db structure to use cents
        }

        public void SetBudgetMontly (int year, int month, Int64 amountCents)
        {
            SwarmDb.GetDatabaseForWriting().SetFinancialAccountBudgetMonthly (Identity, year, month, amountCents);
        }


        public static FinancialAccount Create (Organization organization, string newAccountName,
            FinancialAccountType accountType, FinancialAccount parentAccount)
        {
            return Create (organization.Identity, newAccountName, accountType,
                parentAccount == null ? 0 : parentAccount.Identity);
        }

        protected static FinancialAccount Create (int organizationId, string name, FinancialAccountType accountType,
            int parentFinancialAccountId)
        {
            int accountId = SwarmDb.GetDatabaseForWriting()
                .CreateFinancialAccount (organizationId, name, accountType, parentFinancialAccountId);
            return FromIdentityAggressive (accountId);
        }

        public FinancialAccounts ThisAndBelow()
        {
            return FinancialAccounts.ThisAndBelow (this);
        }


        static private Dictionary<int, Dictionary<int, Int64>> _organizationBudgetAttestationSpaceLookup =
            new Dictionary<int, Dictionary<int, long>>();

        public static Dictionary<int, Int64> GetBudgetAttestationSpaceAdjustments(Organization organization)
        {
            // This function returns a dictionary for the cents that are either accounted for but not attested,
            // or attested but accounted for, to be used to understand how much is really left in budget

            // Positive adjustment means more [cost] budget available, negative less [cost] budget available

            if (_organizationBudgetAttestationSpaceLookup.ContainsKey (organization.Identity))
            {
                return _organizationBudgetAttestationSpaceLookup [organization.Identity];
            }

            // TODO: This is expensive research, we should cache this result and clear cache on any attestation or create op

            Dictionary<int, Int64> result = new Dictionary<int, long>();

            // Cash advances are accounted for when paid out. Make sure they count toward the budget when attested.

            CashAdvances advances = CashAdvances.ForOrganization(organization);
            foreach (CashAdvance advance in advances)
            {
                if (!result.ContainsKey(advance.BudgetId))
                {
                    result[advance.BudgetId] = 0;
                }

                if (advance.Attested)
                {
                    result[advance.BudgetId] -= advance.AmountCents;
                }
            }

            // Expense claims, Inbound invoices, and Salaries are accounted for when filed. Make sure they DON'T
            // count toward the budget while they are NOT attested.

            ExpenseClaims claims = ExpenseClaims.ForOrganization(organization); // gets all open claims
            foreach (ExpenseClaim claim in claims)
            {
                if (!result.ContainsKey(claim.BudgetId))
                {
                    result[claim.BudgetId] = 0;
                }

                if (!claim.Attested)
                {
                    result[claim.BudgetId] += claim.AmountCents;
                }
            }

            InboundInvoices invoices = InboundInvoices.ForOrganization(organization);
            foreach (InboundInvoice invoice in invoices)
            {
                if (!result.ContainsKey(invoice.BudgetId))
                {
                    result[invoice.BudgetId] = 0;
                }

                if (!invoice.Attested)
                {
                    result[invoice.BudgetId] += invoice.AmountCents;
                }
            }

            Salaries salaries = Salaries.ForOrganization(organization);
            foreach (Salary salary in salaries)
            {
                if (!result.ContainsKey(salary.PayrollItem.BudgetId))
                {
                    result[salary.PayrollItem.BudgetId] = 0;
                }

                if (!salary.Attested)
                {
                    result[salary.PayrollItem.BudgetId] += (salary.GrossSalaryCents + salary.AdditiveTaxCents);
                }
            }

            _organizationBudgetAttestationSpaceLookup[organization.Identity] = result;

            return result;
        }

        public static void ClearAttestationAdjustmentsCache (Organization organization)
        {
            if (_organizationBudgetAttestationSpaceLookup.ContainsKey (organization.Identity))
            {
                _organizationBudgetAttestationSpaceLookup.Remove (organization.Identity);
            }
        }



        public Int64 GetBudgetCentsRemaining(int year = -1)
        {
            if (year == -1)
            {
                year = DateTime.UtcNow.Year;
            }

            if (this.AccountType != FinancialAccountType.Cost)
            {
                throw new NotImplementedException("The behavior of this function is undefined for revenue accounts");
            }

            Int64 deltaCentsYear = this.GetDeltaCents(new DateTime(year, 1, 1),
                new DateTime(year + 1, 1, 1));
            Int64 budgetYear = this.GetBudgetCents(year);

            Dictionary<int, Int64> adjustments = GetBudgetAttestationSpaceAdjustments (this.Organization);

            Int64 result = budgetYear; // start with the initial (negative) budget for a cost account
            result += deltaCentsYear; // if money spent, that's a positive number in bookkeeping, and so should reduce neg number
                                      // (bring it closer to zero). If result goes positive here, budget is overdrafted.

            if (adjustments.ContainsKey (this.Identity))
            {
                // A positive adjustment means more available in the budget. As the (cost) budget is negative, we should reduce
                // by this number.

                result -= adjustments[this.Identity];
            }

            return result;
        }
    }
}