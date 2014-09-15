using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class AccountPlan : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-openbook";
            this.PageTitle = Resources.Pages.Ledgers.AccountPlan_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Ledgers.AccountPlan_Info;
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);
        }


        [WebMethod]
        public static JsonAccountData GetAccountData(int accountId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException("A million nopes");
            }

            FinancialAccounts accountTree = account.GetTree();
            int year = DateTime.Today.Year;

            JsonAccountData result = new JsonAccountData();

            result.AccountName = account.Name;
            result.ParentAccountId = account.ParentIdentity;
            result.ParentAccountName = account.ParentFinancialAccountId == 0
                                           ? Resources.Global.ResourceManager.GetString("Financial_" +
                                                                                        account.AccountType.ToString())
                                           : account.Parent.Name;
            result.Expensable = account.Expensable;
            result.Administrative = account.Administrative;
            result.Open = account.Open;
            result.AccountOwnerName = account.OwnerPersonId != 0 ? account.Owner.Name : Resources.Global.Global_NoOwner;
            result.AccountOwnerAvatarUrl = account.OwnerPersonId != 0
                                               ? account.Owner.GetSecureAvatarLink(24)
                                               : "/Images/Icons/iconshock-warning-24px.png";
            result.Budget = (accountTree.GetBudgetSumCents(year)/100L).ToString("N0", CultureInfo.CurrentCulture);

            if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
            {
                result.Balance = (accountTree.GetDeltaCents(new DateTime(1900, 1, 1), new DateTime(year + 1, 1, 1))/100L).ToString("N0");
            }
            else
            {
                result.Balance = (-accountTree.GetDeltaCents(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1)) / 100L).ToString("N0");
            }
            result.CurrencyCode = account.Organization.Currency.Code;

            return result;
        }


        private static bool PrepareAccountChange(FinancialAccount account, AuthenticationData authData, bool checkOpenedYear)
        {
            // TODO: Check permissions, too (may be read-only)

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException("A million nopes");
            }

            try
            {
                if (checkOpenedYear && account.OpenedYear <= authData.CurrentOrganization.Parameters.FiscalBooksClosedUntilYear)
                {
                    // This require breaking the account, which we can't do yet (in this sprint, will come next sprint).
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
        public static bool SetAccountName(int accountId, string name)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (!PrepareAccountChange(account, authData, true))
            {
                return false;
            }

            account.Name = HttpContext.Current.Server.UrlDecode(name);
            return true;
        }

        [WebMethod]
        public static bool SetAccountOwner(int accountId, int newOwnerId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (!PrepareAccountChange(account, authData, false))
            {
                return false;
            }

            // TODO: Verify that authdata.AuthenticatedPerson can see personId, or this can be exploited to enumerate all people

            account.Owner = Person.FromIdentity(newOwnerId);
            return true;
        }

        [WebMethod]
        public static ChangeAccountDataResult SetAccountBudget(int accountId, string budget)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (!PrepareAccountChange(account, authData, false))
            {
                return new ChangeAccountDataResult
                {
                    Result = ChangeAccountDataOperationsResult.NoPermission
                };
            }

            Int64 newTreeBudget;
            budget = budget.Replace("%A0", "%20"); // some very weird browser space-to-otherspace translation weirds out number parsing
            budget = HttpContext.Current.Server.UrlDecode(budget);

            if (budget.Trim().Length > 0 && Int64.TryParse(budget, NumberStyles.Currency, CultureInfo.CurrentCulture, out newTreeBudget))
            {
                newTreeBudget *= 100; // convert to cents

                int year = DateTime.Today.Year;
                FinancialAccounts accountTree = account.GetTree();
                Int64 currentTreeBudget = accountTree.GetBudgetSumCents(year);
                Int64 currentSingleBudget = account.GetBudgetCents(year);
                Int64 suballocatedBudget = currentTreeBudget - currentSingleBudget;

                Int64 newSingleBudget = newTreeBudget - suballocatedBudget;

                account.SetBudgetCents(DateTime.Today.Year, newSingleBudget);
                return new ChangeAccountDataResult
                {
                    Result = ChangeAccountDataOperationsResult.Changed,
                    NewData = (newTreeBudget/100).ToString("N0", CultureInfo.CurrentCulture)
                };
            }
            else
            {
                return new ChangeAccountDataResult
                {
                    Result = ChangeAccountDataOperationsResult.Invalid
                };
            }
        }


        public class ChangeAccountDataResult
        {
            public ChangeAccountDataOperationsResult Result { set; get; }
            public string NewData { set; get; }
        }


        public enum ChangeAccountDataOperationsResult
        {
            Unknown = 0,
            /// <summary>
            /// The account data was changed and nothing more.
            /// </summary>
            Changed = 1,
            /// <summary>
            /// The account has transactions in closed ledgers, so the account was broken in two -
            /// the closed ledgers were kept, and all open ledgers were moved into a new account
            /// with the new data.
            /// </summary>
            ChangedBroken = 2,
            /// <summary>
            /// The user doesn't have write permissions.
            /// </summary>
            NoPermission = 3,
            /// <summary>
            /// The data submitted was invalid (for example an unparsable number for budget).
            /// </summary>
            Invalid = 4
        }

        public struct JsonAccountData
        {
            public string AccountName;
            public int ParentAccountId;
            public string ParentAccountName;
            public string ClosedYear;
            public bool Administrative;
            public bool Expensable;
            public bool Open;
            public string AccountOwnerName;
            public string Budget;
            public string AccountOwnerAvatarUrl;
            public string Balance;
            public string CurrencyCode;
        }

    }
}