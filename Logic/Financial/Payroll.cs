using System;
using System.Globalization;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;

namespace Swarmops.Logic.Financial
{
    public class Payroll : PluralBase<Payroll, PayrollItem, BasicPayrollItem>
    {
        /// <summary>
        ///     Returns all open payroll items for an organization.
        /// </summary>
        /// <param name="organization">The organization.</param>
        /// <returns>The payroll.</returns>
        public static Payroll ForOrganization(Organization organization)
        {
            return ForOrganization(organization, false);
        }


        /// <summary>
        ///     Returns all payroll items for an organization, optionally including closed ones.
        /// </summary>
        /// <param name="organization">The organization to filter for.</param>
        /// <param name="includeClosed">True to include closed records.</param>
        /// <returns>The payroll.</returns>
        public static Payroll ForOrganization(Organization organization, bool includeClosed)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPayroll(organization,
                includeClosed ? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }


        /// <summary>
        ///     Returns all open payroll for all organizations.
        /// </summary>
        /// <returns>The payroll.</returns>
        public static Payroll GetAll()
        {
            return GetAll(false);
        }


        /// <summary>
        ///     Returns all payroll for all organizations, optionally including closed ones.
        /// </summary>
        /// <param name="includeClosed">True to include closed records.</param>
        /// <returns>The payroll.</returns>
        public static Payroll GetAll(bool includeClosed)
        {
            return
                FromArray(
                    SwarmDb.GetDatabaseForReading()
                        .GetPayroll(includeClosed ? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }

        /// <summary>
        ///     Processes all payroll for all organizations system-wide. Should run on the 1st of every month.
        /// </summary>
        public static void ProcessMonthly()
        {
            DateTime today = DateTime.UtcNow;

            string lastRun = Persistence.Key["LastSalaryRun"];

            string expectedLastRun = today.ToString("yyyyMM", CultureInfo.InvariantCulture);

            if (lastRun != null &&
                String.Compare(lastRun, expectedLastRun, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) >= 0)
            {
                // nothing to do, return

                return;
            }

            Persistence.Key["LastSalaryRun"] = expectedLastRun;

            // Process the payroll for all organizations. Assume payday is 25th.
            // TODO: Different payday per organization?

            Payroll payroll = GetAll();
            DateTime payday = new DateTime(today.Year, today.Month, 25);

            foreach (PayrollItem payrollItem in payroll)
            {
                Salary salary = Salary.Create(payrollItem, payday);
                SwarmopsLog.CreateEntry(payrollItem.Person, new SalaryCreatedLogEntry(salary));

                // TODO: CREATE SALARY SPECIFICATION, SEND TO PERSON
            }
        }
    }
}