using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class VatReports: PluralBase<VatReports, VatReport, BasicVatReport>
    {
        static public void CreateNewReports()
        {
            // This goes through all organizations and creates VAT reports where organizations are registered for VAT
            // and it's time for a new report to be created.

            DateTime nowUtc = DateTime.UtcNow;

            // HACK: Only for organization #8 at the moment while we're building this function.

            // foreach organization, if organization is VAT enabled

            Organization organization = Organization.FromIdentity(8); // HACK TEMPORARY HACK
            int reportMonthInterval = 1; // TODO: READ FROM ORG PARAMETERS

            // Get the list of previous VAT reports

            VatReports reports = VatReports.ForOrganization(organization);

            if (reports.Count == 0)
            {
                DateTime firstReportGenerationTime =
                    new DateTime(organization.FirstFiscalYear, 1, 1).AddMonths(reportMonthInterval).AddDays(4);

                if (nowUtc > firstReportGenerationTime)
                {
                    VatReport.Create(organization, organization.FirstFiscalYear, 1, reportMonthInterval);
                }
            }
            else
            {
                reports.Sort(VatReportSorterByDate);

                DateTime lastReport = new DateTime(reports.Last().YearMonthStart/100, reports.Last().YearMonthStart%100,
                    1);
                DateTime nextReport = lastReport.AddMonths(reportMonthInterval);

                DateTime nextReportGenerationTime = nextReport.AddMonths(reportMonthInterval).AddDays(4); // Make the report on the 4th after the period has ended
                if (nowUtc > nextReportGenerationTime)
                {
                    // Create a new report

                    VatReport.Create(organization, nextReport.Year, nextReport.Month, reportMonthInterval);
                }
            }
        }

        private static int VatReportSorterByDate (VatReport a, VatReport b)
        {
            return a.YearMonthStart - b.YearMonthStart;
        }

        public static VatReports ForOrganization(Organization organization)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetVatReports(organization));
        }
    }
}
