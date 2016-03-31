using System;
using System.Collections.Generic;
using NBitcoin.BouncyCastle.Crypto.Digests;
using Swarmops.Logic.Resources;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class DashboardTodos : List<DashboardTodo>
    {
        public static DashboardTodos ForAuthority (Authority authority)
        {
            DashboardTodos result = new DashboardTodos();

            if (authority.IsIdentified) // exclude OpenLedgers and other anon identities
            {

                result.AddBitcoinChecks (authority);

                result.AddExpenseClaimAttestations (authority);
                result.AddCashAdvanceAttestations (authority);
                //result.AddSalaryAttestations(person, organization);   TODO!
                result.AddReceiptValidations (authority);
                result.AddPayouts (authority);

                // TODO: Add any hooks
            }

            return result;
        }


        private void AddBitcoinChecks (Authority authority)
        {
            // Does this person have a bitcoin address set in an org with bitcoin hotwallets?

            if (string.IsNullOrEmpty (authority.Person.BitcoinPayoutAddress))
            {
                if (authority.Organization.FinancialAccounts.AssetsBitcoinHot != null)
                {
                    DashboardTodo todo = new DashboardTodo();
                    todo.Description = Logic_Swarm_DashboardTodos.Bitcoin_SetPayoutAddress;

                    todo.Icon = "/Images/Icons/bitcoin-icon-16px.png";
                    todo.JavaScript = "alertify.prompt(decodeURIComponent('" +
                                      Uri.EscapeDataString (
                                          Logic_Swarm_DashboardTodos.Bitcoin_SetPayoutAddress_Prompt.Replace (
                                              "[InstallationName]", SystemSettings.InstallationName)) +
                                      "' + '<br/><br/>'), function(okPressed, enteredData) { " +
                                      " if (okPressed) { " +
                                        "SwarmopsJS.ajaxCall('/Automation/FinancialFunctions.aspx/SetBitcoinPayoutAddress', { bitcoinAddress: enteredData }, function (result) { " +
                                          "if (result.Success) { alertify.log('Payout address set.'); $('div#divDashboardTodo').fadeOut(); } else { alertify.alert('Could not set payout address: ' + result.DisplayMessage); } " +
                                          "} ); " +
                                      " }}); return false;";

                    Add (todo);
                }
            }
        }


        private void AddReceiptValidations (Authority authority)
        {
            if (!authority.HasAccess (new Access (authority.Organization, AccessAspect.Financials, AccessType.Write)))
            {
                return;
            }

            ExpenseClaims claims = ExpenseClaims.ForOrganization (authority.Organization);
            claims = claims.WhereUnvalidated;

            if (claims.Count == 0)
            {
                // nothing to add
                return;
            }

            DashboardTodo todo = new DashboardTodo();

            if (claims.Count > 1)
            {
                todo.Description = String.Format (Logic_Swarm_DashboardTodos.Validate_Receipts_Many,
                    Formatting.GenerateRangeString (claims.Identities));
            }
            else
            {
                todo.Description = String.Format (Logic_Swarm_DashboardTodos.Validate_Receipts_One, claims[0].Identity);
            }

            todo.Icon = "/Images/PageIcons/iconshock-invoice-greentick-16px.png";
            todo.Url = "/Financial/ValidateReceipts";

            Add (todo);
        }


        // TODO: Refactor the attest-X functions into one function with minimal differences


        private void AddExpenseClaimAttestations (Authority authority)
        {
            ExpenseClaims claims = ExpenseClaims.ForOrganization (authority.Organization);
            List<int> expenseClaimIds = new List<int>();

            bool isPersonOrgAdmin = false;

            if (authority.Person.Identity == 1)
            {
                isPersonOrgAdmin = true; // TODO: Make more advanced, obviously
            }

            foreach (ExpenseClaim claim in claims)
            {
                if (claim.Attested)
                {
                    continue;
                }

                bool attestable = false;

                if (claim.Budget.OwnerPersonId == 0 && isPersonOrgAdmin)
                {
                    attestable = true;
                }
                else if (claim.Budget.OwnerPersonId == authority.Person.Identity)
                {
                    attestable = true;
                }

                if (attestable)
                {
                    expenseClaimIds.Add (claim.Identity);
                }
            }

            if (expenseClaimIds.Count > 0)
            {
                DashboardTodo todo = new DashboardTodo();

                if (expenseClaimIds.Count > 1)
                {
                    todo.Description = String.Format (Logic_Swarm_DashboardTodos.Attest_ExpenseClaim_Many,
                        Formatting.GenerateRangeString (expenseClaimIds));
                }
                else
                {
                    todo.Description = String.Format (Logic_Swarm_DashboardTodos.Attest_ExpenseClaim_One,
                        expenseClaimIds[0]);
                }

                todo.Icon = "/Images/PageIcons/iconshock-stamped-paper-16px.png";
                todo.Url = "/Financial/AttestCosts";

                Add (todo);
            }
        }

        private void AddCashAdvanceAttestations (Authority authority)
        {
            CashAdvances advances = CashAdvances.ForOrganization (authority.Organization);
            List<int> cashAdvanceIds = new List<int>();

            bool isPersonOrgAdmin = false;

            if (authority.Person.Identity == 1)
            {
                isPersonOrgAdmin = true; // TODO: Make more advanced, obviously
            }

            foreach (CashAdvance advance in advances)
            {
                if (advance.Attested)
                {
                    continue;
                }

                bool attestable = false;

                if (advance.Budget.OwnerPersonId == 0 && isPersonOrgAdmin)
                {
                    attestable = true;
                }
                else if (advance.Budget.OwnerPersonId == authority.Person.Identity)
                {
                    attestable = true;
                }


                if (attestable)
                {
                    cashAdvanceIds.Add (advance.Identity);
                }
            }

            if (cashAdvanceIds.Count > 0)
            {
                DashboardTodo todo = new DashboardTodo();

                if (cashAdvanceIds.Count > 1)
                {
                    todo.Description = String.Format (Logic_Swarm_DashboardTodos.Attest_CashAdvance_Many,
                        Formatting.GenerateRangeString (cashAdvanceIds));
                }
                else
                {
                    todo.Description = String.Format (Logic_Swarm_DashboardTodos.Attest_CashAdvance_One,
                        cashAdvanceIds[0]);
                }

                todo.Icon = "/Images/PageIcons/iconshock-stamped-paper-16px.png";
                todo.Url = "/Financial/AttestCosts";

                Add (todo);
            }
        }

        private void AddPayouts (Authority authority)
        {
            if (!authority.HasAccess (new Access (authority.Organization, AccessAspect.Financials, AccessType.Write)))
            {
                return; // do not add this if can't pay out
            }

            DashboardTodo todoNormal = new DashboardTodo();
            DashboardTodo todoUrgent = new DashboardTodo();
            DashboardTodo todoOverdue = new DashboardTodo();

            todoNormal.Url =
                todoUrgent.Url =
                    todoOverdue.Url = "/Financial/PayOutMoney";

            todoNormal.Icon =
                todoUrgent.Icon =
                    todoOverdue.Icon = "/Images/PageIcons/iconshock-money-envelope-16px.png";

            int payoutCount = 0;
            int urgentPayoutCount = 0;
            int overduePayoutCount = 0;

            Payouts payouts = Payouts.Construct (authority.Organization);

            foreach (Payout payout in payouts)
            {
                payoutCount++;

                if (payout.DependentInvoices.Count > 0)
                {
                    if (payout.ExpectedTransactionDate < DateTime.Today)
                    {
                        overduePayoutCount++;
                    }
                    else if (payout.ExpectedTransactionDate < DateTime.Today.AddDays (7))
                    {
                        urgentPayoutCount++;
                    }
                }
            }

            if (payoutCount > 0)
            {
                todoNormal.Description = payoutCount > 1
                    ? String.Format (Logic_Swarm_DashboardTodos.Payout_Many,
                        payoutCount)
                    : Logic_Swarm_DashboardTodos.Payout_One;
                Add (todoNormal);
            }

            if (overduePayoutCount > 0)
            {
                todoOverdue.Description = overduePayoutCount > 1
                    ? String.Format (Logic_Swarm_DashboardTodos.Payout_Overdue_Many,
                        payoutCount)
                    : Logic_Swarm_DashboardTodos.Payout_Overdue_One;
                todoOverdue.Urgency = TodoUrgency.Red;
                Add (todoOverdue);
            }

            if (urgentPayoutCount > 0)
            {
                todoUrgent.Description = overduePayoutCount > 1
                    ? String.Format (Logic_Swarm_DashboardTodos.Payout_Urgent_Many,
                        payoutCount)
                    : Logic_Swarm_DashboardTodos.Payout_Urgent_One;
                todoUrgent.Urgency = TodoUrgency.Yellow;
                Add (todoUrgent);
            }
        }
    }
}