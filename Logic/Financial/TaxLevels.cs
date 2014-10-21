using System;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class TaxLevels
    {
        static public double GetTax (Country country, int taxLevelIdentifier, double grossSalary)
        {
            if (grossSalary < 1.0)  // The lowest tax bracket is 1 currency unit
            {
                return 0.0;  // no tax
            }

            double taxLevel = SwarmDb.GetDatabaseForReading().GetSalaryTaxLevel(country.Identity, taxLevelIdentifier,
                                                      (int) Math.Floor(grossSalary));

            if (taxLevel < 1.0)
            {
                // Percentage

                return Math.Floor(grossSalary*taxLevel);
            }

            // otherwise, absolute number

            return taxLevel;
        }

        static public void ImportTaxLevels (Country country, string data)
        {
            // Lock out salary processing

            Persistence.Key["TaxDataImporting"] = "1";

            int year = DateTime.Today.Year;

            string[] lines = data.Split('\n');   // if \r\n is used, the \r will be harmlessly at the end of lines

            foreach (string line in lines)
            {
                if (line.Length > 5)
                {
                    // LAYOUT:
                    // 30B36  79801  80000381673984037340373403984039840
                    // 30%36  80001  84800   48   50   47   47   50   50
                    // +---------+---------+---------+
                    // 0123456789012345678901234567890
                    // 0         1         2         3

                    // First example line is a fixed tax amount, second is a percentage
                    // We're only interested in "column one" in positions 19-23

                    // This is for Sweden only, ffs!

                    bool isPercentage = line[2] == '%' ? true : false;

                    int lowerBracket = Int32.Parse(line.Substring(5, 7));
                    double tax = Double.Parse(line.Substring(19, 5));
                    int taxLevelId = Int32.Parse(line.Substring(3, 2));

                    if (isPercentage)
                    {
                        tax = tax / 100.0;
                    }

                    SwarmDb.GetDatabaseForWriting().CreateSalaryTaxLevel(country.Identity, taxLevelId, lowerBracket, year, tax);
                }
            }

            // Re-enable salary processing

            Persistence.Key["TaxDataImporting"] = "0";
        }
    }
}
