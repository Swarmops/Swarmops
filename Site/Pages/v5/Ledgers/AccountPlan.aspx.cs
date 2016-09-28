using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Services;
using Resources;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class AccountPlan : PageV5Base
    {
        public enum ChangeAccountDataOperationsResult
        {
            Unknown = 0,

            /// <summary>
            ///     The account data was changed and nothing more.
            /// </summary>
            Changed = 1,

            /// <summary>
            ///     The account has transactions in closed ledgers, so the account was split in two -
            ///     the closed ledgers were kept, and all open ledgers were moved into a new account
            ///     with the new data.
            /// </summary>
            ChangedWithDiscontinuity = 2,

            /// <summary>
            ///     The user doesn't have write permissions.
            /// </summary>
            NoPermission = 3,

            /// <summary>
            ///     The data submitted was invalid (for example an unparsable number for budget).
            /// </summary>
            Invalid = 4
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            if (!Page.IsPostBack)
            {
                // Localize

                PageIcon = "iconshock-openbook";
                this.BoxTitle.Text = PageTitle = Resources.Pages.Ledgers.AccountPlan_PageTitle;
                InfoBoxLiteral = Resources.Pages.Ledgers.AccountPlan_Info;
                this.LiteralExpensesBudgetsAreNegative.Text =
                    Resources.Pages.Ledgers.AccountPlan_ExpensesBudgetsAreNegative;
                this.LiteralDebtBalancesAreNegative.Text =
                    Resources.Pages.Ledgers.AccountPlan_DebtBalancesAreNegative;
                this.LiteralHeaderAccountName.Text = Resources.Pages.Ledgers.AccountPlan_Header_AccountName;
                this.LiteralHeaderBalance.Text = Resources.Pages.Ledgers.AccountPlan_Header_Balance;
                this.LiteralHeaderBudget.Text = Resources.Pages.Ledgers.AccountPlan_Header_Budget;
                this.LiteralHeaderEdit.Text = Resources.Pages.Ledgers.AccountPlan_Header_Edit;
                this.LiteralHeaderEditingAccount.Text = Resources.Pages.Ledgers.AccountPlan_Edit_Header;
                this.LiteralHeaderFlags.Text = Resources.Pages.Ledgers.AccountPlan_Header_Flags;
                this.LiteralHeaderOwner.Text = Global.Global_Owner;
                this.LiteralLabelAccountName.Text = Resources.Pages.Ledgers.AccountPlan_Edit_AccountName;
                this.LiteralLabelActiveLong.Text = Resources.Pages.Ledgers.AccountPlan_Edit_ActiveLong;
                this.LiteralLabelActiveShort.Text = Resources.Pages.Ledgers.AccountPlan_Edit_ActiveShort;
                this.LiteralLabelAdministrativeLong.Text = Resources.Pages.Ledgers.AccountPlan_Edit_AdministrativeLong;
                this.LiteralLabelAdministrativeShort.Text = Resources.Pages.Ledgers.AccountPlan_Edit_AdministrativeShort;
                this.LiteralLabelBudgetBalance.Text = Resources.Pages.Ledgers.AccountPlan_Edit_BudgetBalance;
                this.LiteralLabelExpensableLong.Text = Resources.Pages.Ledgers.AccountPlan_Edit_ExpensableLong;
                this.LiteralLabelExpensableShort.Text = Resources.Pages.Ledgers.AccountPlan_Edit_ExpensableShort;
                this.LiteralLabelFileUploadProfile.Text = Resources.Pages.Ledgers.AccountPlan_Edit_FileUploadProfile;
                this.LiteralLabelHeaderAutomation.Text = Resources.Pages.Ledgers.AccountPlan_Edit_HeaderAutomation;
                this.LiteralLabelHeaderConfiguration.Text = Resources.Pages.Ledgers.AccountPlan_Edit_HeaderConfiguration;
                this.LiteralLabelHeaderDailyOperations.Text =
                    Resources.Pages.Ledgers.AccountPlan_Edit_HeaderDailyOperations;
                this.LiteralLabelOwner.Text = Global.Global_Owner;
                this.LiteralLabelParent.Text = Resources.Pages.Ledgers.AccountPlan_Edit_Parent;

                this.LiteralLabelInitialAmount.Text =
                    String.Format (Resources.Pages.Ledgers.AccountPlan_Edit_InitialBalance,
                        CurrentOrganization.FirstFiscalYear, CurrentOrganization.Currency.DisplayCode);

                FinancialAccounts organizationAccounts = FinancialAccounts.ForOrganization(this.CurrentOrganization);
                int inactiveCount = organizationAccounts.Count(account => !account.Active);

                this.LabelSidebarOptions.Text = Resources.Global.Sidebar_Options;
                this.LabelOptionsShowInactive.Text = String.Format(Resources.Pages.Ledgers.AccountPlan_Options_ShowInactive, inactiveCount);
            }
            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);
            DbVersionRequired = 2; // Account reparenting

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);
            RegisterControl (IncludedControl.SwitchButton);
        }


        [WebMethod]
        public static JsonAccountData GetAccountData (int accountId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException ("A million nopes");
            }

            FinancialAccounts accountTree = account.ThisAndBelow();
            int year = DateTime.Today.Year;

            JsonAccountData result = new JsonAccountData();

            if (account.ParentIdentity == 0)
            {
                // if this is a root account, put it under the category root node, which has negative the type id
                result.ParentAccountId = -(int) account.AccountType;
            }
            else
            {
                result.ParentAccountId = account.ParentIdentity;
            }

            result.AccountName = account.Name;

            result.ParentAccountName = account.ParentFinancialAccountId == 0
                ? Global.ResourceManager.GetString ("Financial_" +
                                                    account.AccountType)
                : account.Parent.Name;
            result.Expensable = account.Expensable;
            result.Administrative = account.Administrative;
            result.Active = account.Active;
            result.Open = account.Open;
            result.AccountOwnerName = account.OwnerPersonId != 0 ? account.Owner.Name : Global.Global_NoOwner;
            result.AccountOwnerAvatarUrl = account.OwnerPersonId != 0
                ? account.Owner.GetSecureAvatarLink (24)
                : "/Images/Icons/iconshock-warning-24px.png";
            result.Budget = (accountTree.GetBudgetSumCents (year)/100L).ToString ("N0", CultureInfo.CurrentCulture);

            if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
            {
                result.Balance =
                    (accountTree.GetDeltaCents (new DateTime (1900, 1, 1), new DateTime (year + 1, 1, 1))/100L).ToString
                        (
                            "N0");
                result.InitialBalance =
                    ((accountTree.GetDeltaCents (new DateTime (1900, 1, 1),
                        new DateTime (authData.CurrentOrganization.FirstFiscalYear, 1, 1))/100.0).ToString ("N2"));
            }
            else
            {
                result.Balance =
                    (-accountTree.GetDeltaCents (new DateTime (year, 1, 1), new DateTime (year + 1, 1, 1))/100L)
                        .ToString (
                            "N0");
                result.InitialBalance = "N/A"; // unused
            }
            result.CurrencyCode = account.Organization.Currency.DisplayCode;

            return result;
        }


        private static bool PrepareAccountChange (FinancialAccount account, AuthenticationData authData,
            bool checkOpenedYear)
        {
            // TODO: Check permissions, too (may be read-only)

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException ("A million nopes");
            }

            try
            {
                int ledgersClosedUntilYear = authData.CurrentOrganization.Parameters.FiscalBooksClosedUntilYear;

                if (checkOpenedYear && ledgersClosedUntilYear > 0 && account.OpenedYear <= ledgersClosedUntilYear)
                {
                    // This requires breaking the account, which we can't do yet (in this sprint, will come next sprint).
                    return false;
                }
            }
            catch (Exception)
            {
                // OpenedYear throws because there isn't a transaction. That's fine.
            }

            return true;
        }


        [WebMethod]
        public static int CreateAccount (string accountType)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccountType accountTypeEnum =
                (FinancialAccountType) Enum.Parse (typeof (FinancialAccountType), accountType);

            FinancialAccount account = FinancialAccount.Create (authData.CurrentOrganization,
                Resources.Pages.Ledgers.AccountPlan_NewAccount, accountTypeEnum, null);

            return account.Identity;
        }


        [WebMethod]
        public static bool SetAccountName (int accountId, string name)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (account.Name == name)
            {
                // no change
                return true;
            }

            if (!PrepareAccountChange (account, authData, true))
            {
                return false;
            }

            account.Name = name;
            return true;
        }

        [WebMethod]
        public static bool SetAccountParent (int accountId, int parentAccountId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (account.ParentFinancialAccountId == parentAccountId)
            {
                // no change
                return true;
            }

            if (!PrepareAccountChange (account, authData, true))
            {
                return false;
            }

            FinancialAccount newParent = null; // to cover the root account case (reparenting to root)
            if (parentAccountId > 0) // the group parent IDs are -1 ... -4
            {
                newParent = FinancialAccount.FromIdentity (parentAccountId);
                if (newParent.OrganizationId != authData.CurrentOrganization.Identity)
                {
                    throw new ArgumentException ("Parent account mismatches with organization identity");
                }
            }

            account.Parent = newParent;
            return true;
        }

        [WebMethod]
        public static bool SetAccountOwner (int accountId, int newOwnerId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (!PrepareAccountChange (account, authData, false))
            {
                return false;
            }

            // TODO SECURITY: Verify that authdata.AuthenticatedPerson can see personId, or this can be exploited to enumerate all people

            account.Owner = Person.FromIdentity (newOwnerId);
            return true;
        }

        [WebMethod]
        public static ChangeAccountDataResult SetAccountInitialBalance (int accountId, string newInitialBalanceString)
        {
            try
            {
                AuthenticationData authData = GetAuthenticationDataAndCulture();
                FinancialAccount account = FinancialAccount.FromIdentity (accountId);

                if (!PrepareAccountChange (account, authData, false) || authData.CurrentOrganization.Parameters.FiscalBooksClosedUntilYear >= authData.CurrentOrganization.FirstFiscalYear)
                {
                    return new ChangeAccountDataResult
                    {
                        Result = ChangeAccountDataOperationsResult.NoPermission
                    };
                }

                Int64 desiredInitialBalanceCents =
                    (Int64)
                        (Double.Parse (newInitialBalanceString,
                            NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                            CultureInfo.CurrentCulture)*100.0);

                Int64 currentInitialBalanceCents = account.GetDeltaCents (new DateTime (1900, 1, 1),
                    new DateTime (authData.CurrentOrganization.FirstFiscalYear, 1, 1));

                Int64 deltaCents = desiredInitialBalanceCents - currentInitialBalanceCents;

                // Find or create "Initial Balances" transaction

                FinancialAccountRows testRows = FinancialAccountRows.ForOrganization (authData.CurrentOrganization,
                    new DateTime (1900, 1, 1), new DateTime (authData.CurrentOrganization.FirstFiscalYear, 1, 1));

                FinancialTransaction initialBalancesTransaction = null;

                foreach (FinancialAccountRow row in testRows)
                {
                    if (row.Transaction.Description == "Initial Balances")
                    {
                        initialBalancesTransaction = row.Transaction;
                        break;
                    }
                }

                if (initialBalancesTransaction == null)
                {
                    // create transaction

                    initialBalancesTransaction = FinancialTransaction.Create (authData.CurrentOrganization.Identity,
                        new DateTime (authData.CurrentOrganization.FirstFiscalYear - 1, 12, 31), "Initial Balances");
                }

                Dictionary<int, Int64> recalcBase = initialBalancesTransaction.GetRecalculationBase();
                int equityAccountId = authData.CurrentOrganization.FinancialAccounts.DebtsEquity.Identity;

                if (!recalcBase.ContainsKey (accountId))
                {
                    recalcBase[accountId] = 0;
                }
                if (!recalcBase.ContainsKey (equityAccountId))
                {
                    recalcBase[equityAccountId] = 0;
                }

                recalcBase[accountId] += deltaCents;
                recalcBase[equityAccountId] -= deltaCents;
                initialBalancesTransaction.RecalculateTransaction (recalcBase, authData.CurrentUser);
                return new ChangeAccountDataResult
                {
                    Result = ChangeAccountDataOperationsResult.Changed,
                    NewData = (desiredInitialBalanceCents / 100.0).ToString("N2", CultureInfo.CurrentCulture)
                };
            }
            catch (Exception weirdException)
            {
                SwarmDb.GetDatabaseForWriting()
                    .CreateExceptionLogEntry(DateTime.UtcNow, "AccountPlan-SetInitBalance", weirdException);

                throw;
            }
        }

        [WebMethod]
        public static ChangeAccountDataResult SetAccountBudget (int accountId, string budget)
        {
            try
            {
                AuthenticationData authData = GetAuthenticationDataAndCulture();
                FinancialAccount account = FinancialAccount.FromIdentity (accountId);

                if (!PrepareAccountChange (account, authData, false))
                {
                    return new ChangeAccountDataResult
                    {
                        Result = ChangeAccountDataOperationsResult.NoPermission
                    };
                }

                Int64 newTreeBudget;
                budget = budget.Replace ("%A0", "%20");
                // some very weird browser space-to-otherspace translation weirds out number parsing
                budget = HttpContext.Current.Server.UrlDecode (budget);

                if (budget.Trim().Length > 0 &&
                    Int64.TryParse (budget, NumberStyles.Currency, CultureInfo.CurrentCulture, out newTreeBudget))
                {
                    newTreeBudget *= 100; // convert to cents

                    int year = DateTime.Today.Year;
                    FinancialAccounts accountTree = account.ThisAndBelow();
                    Int64 currentTreeBudget = accountTree.GetBudgetSumCents (year);
                    Int64 currentSingleBudget = account.GetBudgetCents (year);
                    Int64 suballocatedBudget = currentTreeBudget - currentSingleBudget;

                    Int64 newSingleBudget = newTreeBudget - suballocatedBudget;

                    account.SetBudgetCents (DateTime.Today.Year, newSingleBudget);

                    // Once we've set the budget, also update the "yearly result" budget.
                    // The "yearly result" budget isn't shown in the account plan, but is
                    // abstracted to "projected loss" or "projected gain" pseudobudgets.

                    int thisYear = DateTime.UtcNow.Year;
                    FinancialAccounts allProfitLossAccounts = FinancialAccounts.ForOrganization(authData.CurrentOrganization);
                    Int64 newProfitLossProjection = allProfitLossAccounts.Where(queryAccount => queryAccount.Identity != authData.CurrentOrganization.FinancialAccounts.CostsYearlyResult.Identity).Sum(queryAccount => queryAccount.GetBudgetCents(thisYear));

                    authData.CurrentOrganization.FinancialAccounts.CostsYearlyResult.SetBudgetCents(thisYear, -newProfitLossProjection);

                    return new ChangeAccountDataResult
                    {
                        Result = ChangeAccountDataOperationsResult.Changed,
                        NewData = (newTreeBudget/100).ToString ("N0", CultureInfo.CurrentCulture)
                    };
                }

                return new ChangeAccountDataResult
                {
                    Result = ChangeAccountDataOperationsResult.Invalid
                };
            }
            catch (Exception weirdException)
            {
                // Exceptions are happening here in deployment ONLY. We're logging it to find which one and why.
                // TODO: This really needs to be in Logic. DO NOT DO NOT DO NOT call Database layer directly from Site layer.

                SwarmDb.GetDatabaseForWriting()
                    .CreateExceptionLogEntry (DateTime.UtcNow, "AccountPlan-SetBudget", weirdException);

                throw;
            }
        }

        [WebMethod]
        public static bool SetAccountSwitch (int accountId, string switchName, bool switchValue)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (!PrepareAccountChange (account, authData, false))
            {
                return false;
            }

            switch (switchName)
            {
                case "Active":
                    account.Active = switchValue;
                    break;
                case "Administrative":
                    account.Administrative = switchValue;
                    break;
                case "Expensable":
                    account.Expensable = switchValue;
                    break;
                default:
                    throw new NotImplementedException ("Unknown switchName parameter");
            }

            return true;
        }


        [WebMethod]
        public static string GetInactiveAccountCount()
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            int inactiveAccountCount = FinancialAccounts.ForOrganization(authData.CurrentOrganization).Count(account => !account.Active);
            return inactiveAccountCount.ToString("N0");
        }


        public class ChangeAccountDataResult
        {
            public ChangeAccountDataOperationsResult Result { set; get; }
            public string NewData { set; get; }
        }


        public struct JsonAccountData
        {
            public string AccountName;
            public string AccountOwnerAvatarUrl;
            public string AccountOwnerName;
            public bool Active;
            public bool Administrative;
            public string InitialBalance;
            public string Balance;
            public string Budget;
            public string ClosedYear;
            public string CurrencyCode;
            public bool Expensable;
            public bool Open;
            public int ParentAccountId;
            public string ParentAccountName;
        }
    }
}