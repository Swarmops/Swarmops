using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Management;
using System.Web.Services;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Basic.Types.Structure;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
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
                this.LabelFileUploadProfile.Text = Resources.Pages.Ledgers.AccountPlan_Edit_FileUploadProfile;
                this.LabelHeaderAutomation.Text = Resources.Pages.Ledgers.AccountPlan_Edit_HeaderAutomation;
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

                this.DropAccountAutomationProfile.Items.Add(new ListItem(Resources.Global.Global_SelectOne, "0"));

                // Hardcoded automation profiles for the time being

                this.DropAccountAutomationProfile.Items.Add(new ListItem("[BTC] Bitcoin Core - Armory", ((int)(FinancialAccountAutomationProfileHardIds.BitcoinCoreArmory)).ToString()));
                this.DropAccountAutomationProfile.Items.Add(new ListItem("[BCH] Bitcoin Cash - Armory", ((int)(FinancialAccountAutomationProfileHardIds.BitcoinCoreArmory)).ToString()));
                this.DropAccountAutomationProfile.Items.Add(new ListItem("[CZ CZK] Fio CSV", ((int)(FinancialAccountAutomationProfileHardIds.BankCzechFio)).ToString()));
                this.DropAccountAutomationProfile.Items.Add(new ListItem("[DE EUR] Postbank CSV", ((int)(FinancialAccountAutomationProfileHardIds.BankGermanyPostbank)).ToString()));
                this.DropAccountAutomationProfile.Items.Add(new ListItem("[SE SEK] SEB CSV", ((int)(FinancialAccountAutomationProfileHardIds.BankSwedenSeb)).ToString()));

            }
            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);
            DbVersionRequired = 2; // Account reparenting

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);
            RegisterControl (IncludedControl.SwitchButton);
        }


        [WebMethod]
        public static AjaxCallAccountData GetAccountData (int accountId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException ("A million nopes");
            }

            FinancialAccounts accountTree = account.ThisAndBelow();
            int year = DateTime.Today.Year;

            AjaxCallAccountData result = new AjaxCallAccountData();

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

            result.AutomationData = GetAccountAutomationData(account.AutomationProfileId);

            if (result.AutomationData.AutomationEnabled && result.AutomationData.NonPresentationCurrency)
            {
                Currency foreignCurrency = Currency.FromCode(result.AutomationData.AutomationCurrencyCode);

                result.AutomationData.AutomationCurrencyCode = foreignCurrency.DisplayCode;
                account.ForeignCurrency = foreignCurrency;
            }
            else
            {
                result.CurrencyCode = authData.CurrentOrganization.Currency.DisplayCode;
            }

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

            Person newOwner = Person.FromIdentity(newOwnerId);

            // Verify that authdata.AuthenticatedPerson can see personId, or this can be exploited to enumerate all people

            if (!authData.Authority.CanSeePerson(newOwner))
            {
                throw new ArgumentException("No such person identity");
            }

            account.Owner = newOwner;
            return true;
        }

        [WebMethod]
        public static ChangeAccountDataResult SetAccountInitialBalance (int accountId, string newInitialBalanceString)
        {
            try
            {
                const string initialBalanceTransactionTitle = "Initial Balances";

                AuthenticationData authData = GetAuthenticationDataAndCulture();
                FinancialAccount account = FinancialAccount.FromIdentity (accountId);

                if (!PrepareAccountChange (account, authData, false) || authData.CurrentOrganization.Parameters.FiscalBooksClosedUntilYear >= authData.CurrentOrganization.FirstFiscalYear)
                {
                    return new ChangeAccountDataResult
                    {
                        Result = ChangeAccountDataOperationsResult.NoPermission
                    };
                }

                Int64 desiredInitialBalanceCents = Formatting.ParseDoubleStringAsCents(newInitialBalanceString);

                Int64 currentInitialBalanceCents = account.GetDeltaCents (new DateTime (1900, 1, 1),
                    new DateTime (authData.CurrentOrganization.FirstFiscalYear, 1, 1));

                Int64 deltaCents = desiredInitialBalanceCents - currentInitialBalanceCents;

                // Find or create "Initial Balances" transaction

                FinancialAccountRows testRows = FinancialAccountRows.ForOrganization (authData.CurrentOrganization,
                    new DateTime (1900, 1, 1), new DateTime (authData.CurrentOrganization.FirstFiscalYear, 1, 1));

                FinancialTransaction initialBalancesTransaction = null;

                foreach (FinancialAccountRow row in testRows)
                {
                    if (row.Transaction.Description == initialBalanceTransactionTitle)
                    {
                        initialBalancesTransaction = row.Transaction;
                        break;
                    }
                }

                if (initialBalancesTransaction == null)
                {
                    // create transaction

                    initialBalancesTransaction = FinancialTransaction.Create (authData.CurrentOrganization.Identity,
                        new DateTime (authData.CurrentOrganization.FirstFiscalYear - 1, 12, 31), initialBalanceTransactionTitle);
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
                SupportFunctions.LogException ("AccountPlan-SetInitBalance", weirdException);

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
                SupportFunctions.LogException ("AccountPlan-SetBudget", weirdException);

                throw;
            }
        }

        [WebMethod]
        public static bool ToggleSwitch ()
        {
            return false;  // TODO
        }


        [WebMethod]
        public static AjaxCallAutomationDataResult SetAccountAutomationProfile(int accountId, int profileId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity(accountId);


            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException("A million nopes");
            }

            if (
                !authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.BookkeepingDetails,
                    AccessType.Write)))
            {
                throw new UnauthorizedAccessException("No");
            }

            account.AutomationProfileId = profileId;

            AjaxCallAutomationDataResult result = new AjaxCallAutomationDataResult();
            result.Success = true;
            result.Data = GetAccountAutomationData(profileId);
            result.Data.NonPresentationCurrency =
                (result.Data.Profile.CurrencyId != authData.CurrentOrganization.Currency.Identity);

            if (result.Data.NonPresentationCurrency)
            {
                account.ForeignCurrency = Currency.FromCode(result.Data.AutomationCurrencyCode);
            }
            else
            {
                account.ForeignCurrency = null;
            }

            return result;
        }


        private static AutomationData GetAccountAutomationData(int profileId)
        {
            AutomationData result = new AutomationData();

            result.AutomationProfileCustomXml = string.Empty;
            result.AutomaticRetrievalPossible = false;
            result.AutomationProfileId = profileId;
            result.AutomationEnabled = (profileId != 0);

            if (result.AutomationEnabled)
            {
                result.Profile = FinancialAccountAutomationProfile.FromIdentity(profileId);
                result.AutomationCurrencyCode = Currency.FromIdentity(result.Profile.CurrencyId).Code;
                // the "NonpresentationCurrency" field can't be set here, because we don't know the presentation currency
            }

            return result;
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


        [Serializable]
        public class ChangeAccountDataResult
        {
            public ChangeAccountDataOperationsResult Result { set; get; }
            public string NewData { set; get; }
        }


        [Serializable]
        public class AjaxCallAccountData: AjaxCallResult
        {
            public string AccountName { get; set; }
            public string AccountOwnerAvatarUrl { get; set; }
            public string AccountOwnerName { get; set; }
            public bool Active { get; set; }
            public bool Administrative { get; set; }
            public int AutomationProfileId { get; set; }
            public string AutomationProfileCustomXml { get; set; }
            public string InitialBalance { get; set; }
            public string Balance { get; set; }
            public string Budget { get; set; }
            public string ClosedYear { get; set; }
            public string CurrencyCode { get; set; }
            public bool Expensable { get; set; }
            public bool Open { get; set; }
            public int ParentAccountId { get; set; }
            public string ParentAccountName { get; set; }
            public AutomationData AutomationData { get; set; }
        }


        [Serializable]
        public class AutomationData
        {
            public bool AutomationEnabled { get; set; }
            public int AutomationProfileId { get; set; }
            public string AutomationCurrencyCode { get; set; }
            public string AutomationCountryCode { get; set; }
            public bool NonPresentationCurrency { get; set; }
            public bool AutomaticRetrievalPossible { get; set; }  // always false for now
            public string AutomationProfileCustomXml { get; set; }  // always empty for now
            public FinancialAccountAutomationProfile Profile { get; set; }
        }

        [Serializable]
        public class AjaxCallAutomationDataResult : AjaxCallResult
        {
            public AutomationData Data { get; set; }
        }
    }
}