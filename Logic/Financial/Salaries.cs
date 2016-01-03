using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

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

        public static void CreateAnnualStatements (int year)
        {
            // This function is supposed to be running once a year, and summarize the past year of salaries.

            // Get all organizations.

            Organizations organizations = Organizations.GetAll();

            foreach (Organization organization in organizations)
            {
                // Get all salaries for this organization (filter year in logic - this function is allowed to be expensive, rather than
                // adding a custom database query for this particular operation)

                Salaries allSalaries = ForOrganization (organization, true);

                Salaries yearSalaries = new Salaries();
                yearSalaries.AddRange (allSalaries.Where (salary => salary.PayoutDate.Year == year));

                Dictionary<int, Dictionary<int, Salary>> personSalaryLookup = new Dictionary<int, Dictionary<int, Salary>>();

                // Go through the salaries and store them in the dictionary, so we can process the statement by person later

                foreach (Salary salary in yearSalaries)
                {
                    int personId = salary.PayrollItem.PersonId;

                    if (!personSalaryLookup.ContainsKey (personId))
                    {
                        personSalaryLookup[personId] = new Dictionary<int, Salary>();
                    }

                    personSalaryLookup[personId][salary.PayoutDate.Month] = salary;
                }

                // Once here, salaries are arranged by person and month. Iterate over people, create statements.

                foreach (int personId in personSalaryLookup.Keys)
                {
                    Person person = Person.FromIdentity (personId);
                    Int64 grossTotal = 0;
                    Int64 subTaxTotal = 0;
                    Int64 addTaxTotal = 0;
                    Int64 netTotal = 0;

                    string preFormattedStatement = string.Empty;

                    for (int monthNumber = 1; monthNumber <= 12; monthNumber++)
                    {
                        string monthName = new DateTime (year, monthNumber, 1).ToString ("MMM",
                            new CultureInfo (person.PreferredCulture));
                        string lineItem = "  " + monthName;

                        if (personSalaryLookup[personId].ContainsKey (monthNumber) && personSalaryLookup[personId][monthNumber].GrossSalaryCents > 0)
                        {
                            Salary monthSalary = personSalaryLookup[personId][monthNumber];

                            // TODO: replace "bitcoin" with actual payout method - this is just for show at the moment

                            lineItem += String.Format ("  {0,8:N2}  {1,9:N2}  {2,8:N2}  bitcoin",
                                monthSalary.GrossSalaryCents/100.0, -monthSalary.SubtractiveTaxCents/100.0,
                                monthSalary.NetSalaryCents/100.0);

                            grossTotal += monthSalary.GrossSalaryCents;
                            addTaxTotal += monthSalary.AdditiveTaxCents;
                            subTaxTotal += monthSalary.SubtractiveTaxCents;
                            netTotal += monthSalary.NetSalaryCents;
                        }

                        preFormattedStatement += lineItem + "\r\n";
                    }

                    NotificationStrings notificationStrings = new NotificationStrings();
                    NotificationCustomStrings customStrings = new NotificationCustomStrings();

                    notificationStrings[NotificationString.CurrencyCode] = organization.Currency.DisplayCode;
                    notificationStrings[NotificationString.EmbeddedPreformattedText] = preFormattedStatement;
                    customStrings["LastYear"] = year.ToString(CultureInfo.InvariantCulture);
                    customStrings["GrossSalaryTotal"] = String.Format("{0:N2,8}", grossTotal / 100.0);
                    customStrings["TaxDeductedTotal"] = String.Format("{0:N2,9}", -subTaxTotal / 100.0);
                    customStrings["NetSalaryTotal"] = String.Format("{0:N2,8}", netTotal / 100.0);
                    customStrings["TaxAdditiveTotalUnpadded"] = String.Format("{0:N2}", addTaxTotal / 100.0);

                    // Send notification

                    OutboundComm.CreateNotification (organization, NotificationResource.Salary_LastYearSummary,
                        notificationStrings, customStrings, People.FromSingle (Person.FromIdentity (1)));
                }
            }
        }
    }
}