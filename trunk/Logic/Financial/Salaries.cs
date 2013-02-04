using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class Salaries: PluralBase<Salaries,Salary,BasicSalary>
    {
        static public Salaries ForOrganization(Organization organization)
        {
            return ForOrganization(organization, false);
        }

        static public Salaries ForOrganization(Organization organization, bool includeClosed)
        {
            return FromArray (PirateDb.GetDatabaseForReading().GetSalaries(organization, 
                includeClosed? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }


        public double TotalAmountNet
        {
            get
            {
                double result = 0.0;

                foreach (Salary salary in this)
                {
                    result += salary.NetSalaryCents / 100.0;
                }

                return result;
            }
        }

        public double TotalAmountTax
        {
            get
            {
                double result = 0.0;

                foreach (Salary salary in this)
                {
                    result += salary.TaxTotalCents / 100.0;
                }

                return result;
            }
        }

        public Int64 TotalAmountCentsNet
        {
            get
            {
                Int64 result = 0;

                foreach (Salary salary in this)
                {
                    result += salary.NetSalaryCents;
                }

                return result;
            }
        }

        public Int64 TotalAmountCentsTax
        {
            get
            {
                Int64 result = 0;

                foreach (Salary salary in this)
                {
                    result += salary.TaxTotalCents;
                }

                return result;
            }
        }

    }
}
