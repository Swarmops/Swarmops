using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Logic.DashboardTasks;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class DashboardTodos: List<DashboardTodo>
    {
        public static DashboardTodos ForPerson (Person person, Organization organization)
        {
            DashboardTodos result = new DashboardTodos();

            result.AddExpenseClaimAttestations(person, organization);
            result.AddCashAdvanceAttestations(person, organization);
            result.AddReceiptValidations(person, organization);
            result.AddPayouts(person, organization);

            // TODO: Add any hooks

            return result;
        }



        private void AddReceiptValidations(Person person, Organization organization)
        {
            if (!person.HasAccess(new Access(organization, AccessAspect.Financials, AccessType.Write)))
            {
                return;
            }

            ExpenseClaims claims = ExpenseClaims.ForOrganization(organization);
            claims = claims.WhereUnvalidated;

            if (claims.Count == 0)
            {
                // nothing to add
                return;
            }

            DashboardTodo todo = new DashboardTodo();

            if (claims.Count > 1)
            {
                todo.Description = String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Validate_Receipts_Many, Formatting.GenerateRangeString(claims.Identities));
            }
            else
            {
                todo.Description = String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Validate_Receipts_One, claims[0].Identity);
            }

            todo.Icon = "/Images/PageIcons/iconshock-invoice-greentick-16px.png";
            todo.Url = "/Pages/v5/Financial/ValidateReceipts.aspx";

            this.Add(todo);
        }





        // TODO: Refactor the attest-X functions into one function with minimal differences


        private void AddExpenseClaimAttestations(Person person, Organization organization)
        {
            ExpenseClaims claims = ExpenseClaims.ForOrganization(organization);
            List<int> expenseClaimIds = new List<int>();

            bool isPersonOrgAdmin = false;

            if (person.Identity == 1)
            {
                isPersonOrgAdmin = true;  // TODO: Make more advanced, obviously
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
                else if (claim.Budget.OwnerPersonId == person.Identity)
                {
                    attestable = true;
                }

                if (attestable)
                {
                    expenseClaimIds.Add(claim.Identity);
                }
            }

            if (expenseClaimIds.Count > 0)
            {
                DashboardTodo todo = new DashboardTodo();
                
                if (expenseClaimIds.Count > 1)
                {
                    todo.Description = String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Attest_ExpenseClaim_Many, Formatting.GenerateRangeString(expenseClaimIds));
                }
                else
                {
                    todo.Description = String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Attest_ExpenseClaim_One, expenseClaimIds[0]);
                }

                todo.Icon = "/Images/PageIcons/iconshock-stamped-paper-16px.png";
                todo.Url = "/Pages/v5/Financial/AttestCosts.aspx";

                this.Add(todo);
            }
        }

        private void AddCashAdvanceAttestations(Person person, Organization organization)
        {
            CashAdvances advances = CashAdvances.ForOrganization(organization);
            List<int> cashAdvanceIds = new List<int>();

            bool isPersonOrgAdmin = false;

            if (person.Identity == 1)
            {
                isPersonOrgAdmin = true;  // TODO: Make more advanced, obviously
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
                else if (advance.Budget.OwnerPersonId == person.Identity)
                {
                    attestable = true;
                }


                if (attestable)
                {
                    cashAdvanceIds.Add(advance.Identity);
                }
            }

            if (cashAdvanceIds.Count > 0)
            {
                DashboardTodo todo = new DashboardTodo();

                if (cashAdvanceIds.Count > 1)
                {
                    todo.Description = String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Attest_CashAdvance_Many, Formatting.GenerateRangeString(cashAdvanceIds));
                }
                else
                {
                    todo.Description = String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Attest_CashAdvance_One, cashAdvanceIds[0]);
                }

                todo.Icon = "/Images/PageIcons/iconshock-stamped-paper-16px.png";
                todo.Url = "/Pages/v5/Financial/AttestCosts.aspx";

                this.Add(todo);
            }
        }

        private void AddPayouts(Person person, Organization organization)
        {
            if (!person.HasAccess(new Access(organization, AccessAspect.Financials, AccessType.Write)))
            {
                return; // do not add this if can't pay out
            }

            DashboardTodo todoNormal = new DashboardTodo();
            DashboardTodo todoUrgent = new DashboardTodo();
            DashboardTodo todoOverdue = new DashboardTodo();

            todoNormal.Url =
                todoUrgent.Url =
                todoOverdue.Url = "/Pages/v5/Financial/PayOutMoney.aspx";

            todoNormal.Icon =
                todoUrgent.Icon =
                todoOverdue.Icon = "/Images/PageIcons/iconshock-money-envelope-16px.png";

            int payoutCount = 0;
            int urgentPayoutCount = 0;
            int overduePayoutCount = 0;

            Payouts payouts = Payouts.Construct(organization);

            foreach (Payout payout in payouts)
            {
                payoutCount++;

                if (payout.DependentInvoices.Count > 0)
                {
                    if (payout.ExpectedTransactionDate < DateTime.Today)
                    {
                        overduePayoutCount++;
                    }
                    else if (payout.ExpectedTransactionDate < DateTime.Today.AddDays(7))
                    {
                        urgentPayoutCount++;
                    }
                }
            }

            if (payoutCount > 0)
            {
                todoNormal.Description = payoutCount > 1
                                             ? String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Payout_Many,
                                                             payoutCount)
                                             : App_GlobalResources.Logic_Swarm_DashboardTodos.Payout_One;
                this.Add(todoNormal);
            }

            if (overduePayoutCount > 0)
            {
                todoOverdue.Description = overduePayoutCount > 1
                                             ? String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Payout_Overdue_Many,
                                                             payoutCount)
                                             : App_GlobalResources.Logic_Swarm_DashboardTodos.Payout_Overdue_One;
                todoOverdue.Urgency = TodoUrgency.Red;
                this.Add(todoOverdue);
            }

            if (urgentPayoutCount > 0)
            {
                todoUrgent.Description = overduePayoutCount > 1
                                             ? String.Format(App_GlobalResources.Logic_Swarm_DashboardTodos.Payout_Urgent_Many,
                                                             payoutCount)
                                             : App_GlobalResources.Logic_Swarm_DashboardTodos.Payout_Urgent_One;
                todoUrgent.Urgency = TodoUrgency.Yellow;
                this.Add(todoUrgent);
            }
        }


    }
}
