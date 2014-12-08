using System;
using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class Salaries : PluralBase<Salaries, Salary, BasicSalary>
    {
        public Salaries WhereAttested
        {
            get
            {
                Salaries result = new Salaries();
                result.AddRange (this.Where (salary => salary.Attested));

                return result;
            }
        }


        public Salaries WhereUnattested
        {
            get
            {
                Salaries result = new Salaries();
                result.AddRange (this.Where (salary => !salary.Attested));

                return result;
            }
        }


        public double TotalAmountNet
        {
            get { return this.Sum (salary => salary.NetSalaryCents/100.0); }
        }

        public double TotalAmountTax
        {
            get { return this.Sum (salary => salary.TaxTotalCents/100.0); }
        }

        public Int64 TotalAmountCentsNet
        {
            get { return this.Sum (salary => salary.NetSalaryCents); }
        }

        public Int64 TotalAmountCentsTax
        {
            get { return this.Sum (salary => salary.TaxTotalCents); }
        }

        public static Salaries ForOrganization (Organization organization)
        {
            return ForOrganization (organization, false);
        }

        public static Salaries ForOrganization (Organization organization, bool includeClosed)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetSalaries (organization,
                includeClosed ? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }
    }
}