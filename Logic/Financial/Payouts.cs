using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class Payouts: PluralBase<Payouts,Payout,BasicPayout>
    {
        public static Payouts ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);
        }

        public static Payouts ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabase().GetPayouts(organization));
            }
            else
            {
                return FromArray(PirateDb.GetDatabase().GetPayouts(organization, DatabaseCondition.OpenTrue));
            }
        }

        public static Payouts Construct (Organization organization)
        {
            return Construct(organization.Identity);
        }

        public static Payouts Construct (int organizationId)
        {
            // Construct a list of not-yet-created payouts. Basically, these are the things that
            // Economy hasn't yet filed with the bank.

            Payouts result = new Payouts();

            AddUnpaidExpenseClaims(result, organizationId);
            AddUnpaidInboundInvoices(result, organizationId);
            AddUnpaidSalaries(result, organizationId);

            return result;
        }

        private static void AddUnpaidExpenseClaims(Payouts payoutList, int organizationId)
        {
            ExpenseClaims claims = ExpenseClaims.FromOrganization(Organization.FromIdentity(organizationId));

            Dictionary<int, Payout> payoutLookup = new Dictionary<int, Payout>();

            foreach (ExpenseClaim claim in claims)
            {
                // If ready for payout, add to list.

                if (claim.Open)
                {
                    if (claim.Attested && claim.Validated && !claim.Repaid && !claim.KeepSeparate)
                    {
                        // this should be added to the list. Check if we already have pending payouts
                        // for this person:

                        if (payoutLookup.ContainsKey(claim.ClaimingPersonId))
                        {
                            // Yes. Add claim to list.

                            payoutLookup[claim.ClaimingPersonId].DependentExpenseClaims.Add(claim);
                        }
                        else
                        {
                            // No. Create a new payout for this person.

                            BasicPayout basicPayout = new BasicPayout(0, organizationId, claim.Claimer.BankName,
                                                                      claim.Claimer.BankClearing+" / "+claim.Claimer.BankAccount, string.Empty, 0,
                                                                      DateTime.MinValue, false, DateTime.Now, 0);
                            Payout payout = Payout.FromBasic(basicPayout);

                            payout.DependentExpenseClaims.Add(claim);

                            payoutLookup[claim.ClaimingPersonId] = payout;
                        }
                    }
                }
            }

            // We now have the list of payouts and the associated claims, but the amounts aren't set on the
            // payouts. This will be the next step, as we assemble the list.

            foreach (Payout payout in payoutLookup.Values)
            {
                Int64 newAmountCents = 0;
                List<int> claimIds = new List<int>();

                foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
                {
                    newAmountCents += claim.AmountCents;
                    claimIds.Add(claim.Identity);
                }

                payout.AmountCents = newAmountCents;

                if (claimIds.Count == 1)
                {
                    payout.Reference = String.Format("Expense #{0}", claimIds[0]);
                }
                else
                {
                    claimIds.Sort();
                    payout.Reference = String.Format("Expenses {0}", Formatting.GenerateRangeString(claimIds));
                }

                if (newAmountCents > 0)
                {
                    payoutList.Add(payout);
                }
            }

            // Lastly, add the keep-separate claims.

            foreach (ExpenseClaim claim in claims)
            {
                if (claim.Open)
                {
                    if (claim.Attested && claim.Validated && !claim.Repaid && claim.KeepSeparate)
                    {
                        BasicPayout basicPayout = new BasicPayout(0, organizationId, claim.Claimer.BankName,
                                                                  claim.Claimer.BankClearing + " / " + claim.Claimer.BankAccount, string.Empty, claim.AmountCents,
                                                                  DateTime.MinValue, false, DateTime.Now, 0);
                        Payout payout = Payout.FromBasic(basicPayout);
                        payout.Reference = "Cash Advance #" + claim.Identity;
                        payout.DependentExpenseClaims.Add(claim);

                        payoutList.Add(payout);
                    }
                }
            }
        }



        private static void AddUnpaidInboundInvoices (Payouts payoutList, int organizationId)
        {
            InboundInvoices invoices = InboundInvoices.ForOrganization(Organization.PPSE);

            foreach (InboundInvoice invoice in invoices)
            {
                if (invoice.Attested)
                {
                    BasicPayout basicPayout = new BasicPayout(0, organizationId, string.Empty,
                                                              invoice.PayToAccount, invoice.Ocr.Length > 0? "OCR " + invoice.Ocr : "Ref# " + invoice.InvoiceReference, (Int64) (invoice.Amount * 100),
                                                              invoice.DueDate, false, DateTime.Now, 0);
                    Payout payout = Payout.FromBasic(basicPayout);

                    payout.DependentInvoices.Add(invoice);

                    payoutList.Add(payout);
                }
            }
        }

        private static void AddUnpaidSalaries (Payouts payoutList, int organizationId)
        {
            Int64 taxTotalCents = 0;

            Salaries salaries = Salaries.ForOrganization(Organization.FromIdentity(organizationId));
            List<int> identityList = new List<int>();
            DateTime payDay = DateTime.MaxValue;

            foreach (Salary salary in salaries)
            {
                if (!salary.Attested)
                {
                    continue;
                }

                if (!salary.NetPaid)
                {
                    PayrollItem payrollItem = salary.PayrollItem;
                    Person employee = payrollItem.Person;

                    BasicPayout basicPayout = new BasicPayout(0, organizationId, employee.BankName,
                                                              employee.BankAccount, "Salary " + salary.PayoutDate.ToString("yyyy-MMM"), 
                                                              salary.NetSalaryCents, salary.PayoutDate, false, DateTime.Now, 0);
                    Payout payout = Payout.FromBasic(basicPayout);

                    payout.DependentSalariesNet.Add(salary);

                    payoutList.Add(payout);

                    if (payDay > salary.PayoutDate)
                    {
                        payDay = salary.PayoutDate;
                    }
                }

                if (!salary.TaxPaid)
                {
                    taxTotalCents += salary.TaxTotalCents;
                    identityList.Add(salary.Identity);

                    if (payDay > salary.PayoutDate)
                    {
                        payDay = salary.PayoutDate;
                    }
                }
            }

            if (taxTotalCents > 0)
            {
                // Add the summarized tax line, too

                string referenceString = string.Empty;

                if (identityList.Count == 1)
                {
                    referenceString = "Tax for salary #" + identityList[0].ToString();
                }
                else
                {
                    identityList.Sort();
                    referenceString = "Tax for salaries " + Formatting.GenerateRangeString(identityList);
                }


                BasicPayout basicPayout = new BasicPayout(0, organizationId, "The Tax Man",
                                                          string.Empty, referenceString,
                                                          taxTotalCents, payDay, false, DateTime.Now, 0);
                Payout payout = Payout.FromBasic(basicPayout);

                foreach (int salaryId in identityList)
                {
                    payout.DependentSalariesTax.Add(Salary.FromIdentity(salaryId));
                }

                payoutList.Add(payout);
            }
        }


        public static void AutomatchAgainstUnbalancedTransactions(Organization organization)
        {
            // Matches unbalanced financial transactions against unclosed payouts

            // Should this be in bot?

            Payouts payouts = Payouts.ForOrganization(organization);

            FinancialTransactions transactions = FinancialTransactions.GetUnbalanced(organization);

            foreach (FinancialTransaction transaction in transactions)
            {
                // Console.WriteLine("Looking at transaction #{0} ({1:yyyy-MM-dd}, {2:N2}).", transaction.Identity, transaction.DateTime, transaction.Rows.AmountTotal);

                // First, establish that there are no similar transactions within 7 days. N^2 search.

                DateTime timeLow = transaction.DateTime.AddDays(-7);
                DateTime timeHigh = transaction.DateTime.AddDays(7);

                bool foundCompeting = false;

                foreach (FinancialTransaction possiblyCompetingTransaction in transactions)
                {
                    if (possiblyCompetingTransaction.Rows.AmountTotal == transaction.Rows.AmountTotal &&
                        possiblyCompetingTransaction.DateTime >= timeLow &&
                        possiblyCompetingTransaction.DateTime <= timeHigh &&
                        possiblyCompetingTransaction.Identity != transaction.Identity)
                    {
                        foundCompeting = true;
                        // Console.WriteLine(" - Transaction #{0} ({1:yyyy-MM-dd} is competing, aborting", possiblyCompetingTransaction.Identity, possiblyCompetingTransaction.DateTime);
                    }
                }

                if (foundCompeting)
                {
                    continue;
                }

                // Console.WriteLine(" - no competing transactions...\r\n - transaction description is \"{0}\".", transaction.Description);

                // Console.WriteLine(" - looking for matching payouts");

                int foundCount = 0;
                int payoutIdFound = 0;

                // As the amount of payouts grow, this becomes less efficient exponentially.

                foreach (Payout payout in payouts)
                {
                    if (payout.ExpectedTransactionDate >= timeLow && payout.ExpectedTransactionDate <= timeHigh && payout.AmountCents == -transaction.Rows.AmountCentsTotal)
                    {
                        // Console.WriteLine(" - - payout #{0} matches ({1}, {2:yyyy-MM-dd})", payout.Identity, payout.Recipient, payout.ExpectedTransactionDate);

                        try
                        {
                            FinancialTransaction tiedTransaction = FinancialTransaction.FromDependency(payout);

                            // Console.WriteLine(" - - - but is tied to transaction #{0} already", tiedTransaction.Identity);
                            break;
                        }
                        catch (Exception)
                        {
                            // There isn't such a transaction, which is what we want
                        }

                        foundCount++;
                        payoutIdFound = payout.Identity;
                    }
                }

                if (foundCount == 0)
                {
                    // Console.WriteLine(" - none found");
                }
                else if (foundCount > 1)
                {
                    // Console.WriteLine(" - multiple found, not autoprocessing");
                }
                else
                {
                    Payout payout = Payout.FromIdentity(payoutIdFound);
                    payout.BindToTransactionAndClose(transaction, null);
                  
                }
            }
        }

        public decimal TotalAmount
        {
            get
            {
                decimal result = 0.0m;

                foreach (Payout payout in this)
                {
                    result += payout.Amount;
                }

                return result;
            }
        }

        public Int64 TotalAmountCents
        {
            get 
            { 
                Int64 result = 0;
            
                foreach (Payout payout in this)
                {
                    result += payout.AmountCents;
                }

                return result;
            }
        }
    }
}
