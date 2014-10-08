using System;
using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.DashboardTasks
{
    public class Tasks: List<TaskGroup>
    {
        static public Tasks ForPerson (Person person)
        {
            Tasks tasks = new Tasks();

            tasks.AddVolunteers(person);
            tasks.AddExpenseClaims(person);
            tasks.AddSalaries(person);
            tasks.AddInboundInvoices(person);
            tasks.AddReceiptValidation(person);
            tasks.AddAdvanceDebts(person);
            tasks.AddPayouts(person);
            tasks.AddAttestationWarnings(person);

            return tasks;
        }

        private void AddVolunteers (Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.Volunteers);

            Volunteers volunteers = Volunteers.GetOpen();

            foreach (Volunteer volunteer in volunteers)
            {
                if (volunteer.Owner.Identity == person.Identity)
                {
                    group.Tasks.Add(new TaskVolunteer(volunteer));
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }
        }

        private void AddExpenseClaims (Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.AttestExpenseClaims);

            // TODO: Loop over roles, get all open claims for roles where person can attest

            ExpenseClaims claims = ExpenseClaims.ForOrganization(Organization.PPSE);

            foreach (ExpenseClaim claim in claims)
            {
                try
                {
                    if (claim.Budget.OwnerPersonId == person.Identity && !claim.Attested)
                    {
                        group.Tasks.Add(new TaskExpenseClaim(claim));
                    }
                }
                catch (Exception)
                {
                    // ignore fn
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }
        }

        private void AddSalaries (Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.AttestSalaries);

            // TODO: Loop over roles, get all open claims for roles where person can attest

            Salaries salaries = Salaries.ForOrganization(Organization.PPSE);

            foreach (Salary salary in salaries)
            {
                if (salary.AttestationExpectedBy.Identity == person.Identity && !salary.Attested)
                {
                    group.Tasks.Add(new TaskSalary(salary));
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }
        }

        private void AddInboundInvoices(Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.AttestInvoices);

            // TODO: Loop over roles, get all open claims for roles where person can attest

            InboundInvoices invoices = InboundInvoices.ForOrganization(Organization.PPSE);

            foreach (InboundInvoice invoice in invoices)
            {
                if (invoice.Budget.OwnerPersonId == person.Identity && !invoice.Attested)
                {
                    group.Tasks.Add(new TaskInboundInvoice(invoice));
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }
        }

        private void AddReceiptValidation(Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.ValidateExpenseClaims);

            // TODO: Loop over roles, get all open claims for roles where person can attest

            if (!person.GetAuthority().HasPermission(Permission.CanDoEconomyTransactions, Organization.PPSEid, Geography.RootIdentity, Authorization.Flag.AnyGeographyExactOrganization))
            {
                // no permission, no tasks
                return;
            }

            ExpenseClaims claims = ExpenseClaims.ForOrganization(Organization.PPSE);

            foreach (ExpenseClaim claim in claims)
            {
                if (!claim.Validated)
                {
                    group.Tasks.Add(new TaskReceiptValidation (claim));
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }
        }


        private void AddAdvanceDebts (Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.DeclareAdvanceDebts);
            
            // TODO: One task group for each organization, actually

            ExpenseClaims claims = ExpenseClaims.FromClaimingPersonAndOrganization(person, Organization.PPSE);

            decimal debt = 0.0m;

            foreach (ExpenseClaim claim in claims)
            {
                if (claim.Open && claim.Attested && claim.Validated && claim.Claimed)
                {
                    debt += -claim.Amount;
                }
            }

            if (debt > 0.0m)
            {
                group.Tasks.Add(new TaskAdvanceDebt(debt));
                this.Add(group);
            }
        }


        private void AddPayouts (Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.Payout);
            TaskGroup groupUrgent = new TaskGroup(TaskGroupType.PayoutUrgently);
            TaskGroup groupOverdue = new TaskGroup(TaskGroupType.PayoutOverdue);

            if (!person.GetAuthority().HasPermission(Permission.CanPayOutMoney, Organization.PPSEid, 1, Authorization.Flag.AnyGeographyExactOrganization))
            {
                return;
            }

            Payouts payouts = Payouts.Construct(Organization.PPSE);

            foreach (Payout payout in payouts)
            {
                group.Tasks.Add(new TaskPayout(payout));

                if (payout.DependentInvoices.Count > 0)
                {
                    if (payout.ExpectedTransactionDate < DateTime.Today)
                    {
                        groupOverdue.Tasks.Add(new TaskPayout(payout));
                    }
                    else if (payout.ExpectedTransactionDate < DateTime.Today.AddDays(7))
                    {
                        groupUrgent.Tasks.Add(new TaskPayout(payout));
                    }
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }

            if (groupUrgent.Tasks.Count > 0)
            {
                this.Add(groupUrgent);
            }

            if (groupOverdue.Tasks.Count > 0)
            {
                this.Add(groupOverdue);
            }
        }


        private void AddAttestationWarnings (Person person)
        {
            TaskGroup group = new TaskGroup(TaskGroupType.AttestationWarning);

            if (!person.GetAuthority().HasPermission(Permission.CanSeeEconomyTransactions, Organization.PPSEid, 1, Authorization.Flag.AnyGeographyExactOrganization))
            {
                return;
            }

            InboundInvoices invoices = InboundInvoices.ForOrganization(Organization.PPSE);

            DateTime threshold = DateTime.Today.AddDays(10);

            foreach (InboundInvoice invoice in invoices)
            {
                if (invoice.DueDate < threshold && !invoice.Attested)
                {
                    group.Tasks.Add(new TaskAttestationLate (invoice));
                }
            }

            if (group.Tasks.Count > 0)
            {
                this.Add(group);
            }
        }
    }
}
