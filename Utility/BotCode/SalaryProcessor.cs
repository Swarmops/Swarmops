using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.BotCode
{
    public class SalaryProcessor
    {
        public static void Run()
        {
            DateTime today = DateTime.Today;

            string lastRun = Persistence.Key["LastSalaryRun"];

            string expectedLastRun = today.ToString ("yyyyMM", CultureInfo.InvariantCulture);

            if (lastRun != null && String.Compare (lastRun, expectedLastRun) >= 0)
            {
                // nothing to do, return

                return;
            }

            Persistence.Key["LastSalaryRun"] = expectedLastRun;

            // Process the payroll for all organizations. Assume payday is 25th.

            Payroll payroll = Payroll.GetAll();
            DateTime payday = new DateTime (today.Year, today.Month, 25);

            Dictionary<int, double> salariesTotalPerBudget = new Dictionary<int, double>();

            foreach (PayrollItem payrollItem in payroll)
            {
                Salary salary = Salary.Create (payrollItem, payday);
                PWEvents.CreateEvent (EventSource.PirateBot, EventType.SalaryCreated, 0, payrollItem.OrganizationId,
                    0, payrollItem.Person.Identity, salary.Identity, string.Empty);
            }
        }

        /*
        public static void RecreateFirstTransaction()
        {
            Salary salary = Salary.FromIdentity(1);
            PayrollItem payrollItem = salary.PayrollItem;
            DateTime payday = salary.PayoutDate;
            PWEvents.CreateEvent(EventSource.PirateBot, EventType.SalaryCreated, 0, payrollItem.OrganizationId,
                               0, payrollItem.PersonId, salary.Identity, string.Empty);

            double cost = salary.GrossSalary + salary.AdditiveTax;
            double tax = salary.SubtractiveTax + salary.AdditiveTax;
            double net = salary.GrossSalary - salary.SubtractiveTax;

            FinancialTransaction transaction =
                FinancialTransaction.Create(payrollItem.OrganizationId, DateTime.Now,
                                            "Salary #" + salary.Identity + ": " + payrollItem.PersonCanonical + " " +
                                            payday.ToString("yyyy-MMM", CultureInfo.InvariantCulture));
            transaction.AddRow(payrollItem.BudgetId, cost, 0);
            transaction.AddRow(payrollItem.Organization.FinancialAccounts.DebtsSalary, -net, null);
            transaction.AddRow(payrollItem.Organization.FinancialAccounts.DebtsTax, -tax, null);
            transaction.Dependency = salary;
        }*/
    }
}