using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class Payroll: PluralBase<Payroll,PayrollItem,BasicPayrollItem>
    {
        /// <summary>
        /// Returns all open payroll items for an organization.
        /// </summary>
        /// <param name="organization">The organization.</param>
        /// <returns>The payroll.</returns>
        public static Payroll ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);           
        }


        /// <summary>
        /// Returns all payroll items for an organization, optionally including closed ones.
        /// </summary>
        /// <param name="organization">The organization to filter for.</param>
        /// <param name="includeClosed">True to include closed records.</param>
        /// <returns>The payroll.</returns>
        public static Payroll ForOrganization (Organization organization, bool includeClosed)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPayroll(organization, 
                includeClosed? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }


        /// <summary>
        /// Returns all open payroll for all organizations.
        /// </summary>
        /// <returns>The payroll.</returns>
        public static Payroll GetAll()
        {
            return GetAll(false);
        }


        /// <summary>
        /// Returns all payroll for all organizations, optionally including closed ones.
        /// </summary>
        /// <param name="includeClosed">True to include closed records.</param>
        /// <returns>The payroll.</returns>
        public static Payroll GetAll(bool includeClosed)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPayroll(includeClosed? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }
    }
}
