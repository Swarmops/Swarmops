using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
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
                return FromArray(SwarmDb.GetDatabaseForReading().GetPayouts(organization));
            }
            else
            {
                return FromArray(SwarmDb.GetDatabaseForReading().GetPayouts(organization, DatabaseCondition.OpenTrue));
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
            AddUnpaidCashAdvances(result, organizationId);

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
                                                                      claim.Claimer.BankClearing + " / " + claim.Claimer.BankAccount, string.Empty, 0,
                                                                      DateTime.MinValue, false, DateTime.Now, 0);
                            Payout payout = Payout.FromBasic(basicPayout);

                            payout.DependentExpenseClaims.Add(claim);

                            payoutLookup[claim.ClaimingPersonId] = payout;
                        }
                    }
                }
            }

            // At this point, all the expense claims have been added - but we need to add the open
            // cash advances and deduct them.

            CashAdvances cashAdvances = CashAdvances.ForOrganization(Organization.FromIdentity(organizationId));
            cashAdvances = cashAdvances.WherePaid;

            // At this point, only open and paid cash advances are in the list: they're debts to the org

            foreach (CashAdvance cashAdvance in cashAdvances)
            {
                if (payoutLookup.ContainsKey(cashAdvance.PersonId))
                {
                    // there's a payout prepared to this person - we need to deduct the cash advance from it.

                    payoutLookup[cashAdvance.PersonId].DependentCashAdvancesPayback.Add(cashAdvance);
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

                foreach (CashAdvance previousAdvance in payout.DependentCashAdvancesPayback)
                {
                    newAmountCents -= previousAdvance.AmountCents;
                }

                string lessAdvancesIndicator = payout.DependentCashAdvancesPayback.Count > 0
                                                   ? "LessAdvances"
                                                   : string.Empty;

                payout.AmountCents = newAmountCents;

                if (claimIds.Count == 1)
                {
                    payout.Reference = "[Loc]Financial_ExpenseClaimSpecification" + lessAdvancesIndicator + "|" + claimIds[0].ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    claimIds.Sort();
                    payout.Reference = "[Loc]Financial_ExpenseClaimsSpecification" + lessAdvancesIndicator + "|" + Formatting.GenerateRangeString(claimIds);
                }

                if (newAmountCents > 0)
                {
                    payoutList.Add(payout);
                }
            }

        }



        private static void AddUnpaidCashAdvances(Payouts payoutList, int organizationId)
        {
            CashAdvances advances = CashAdvances.ForOrganization(Organization.FromIdentity(organizationId));
            advances = advances.WhereAttested;
            advances = advances.WhereUnpaid;

            Dictionary<int, Payout> payoutLookup = new Dictionary<int, Payout>();

            foreach (CashAdvance advance in advances)
            {
                // If ready for payout, add to list.

                if (!advance.Open || !advance.Attested || advance.PaidOut)
                {
                    throw new InvalidOperationException("Got into loop with closed/unattested/paid-out cash advances - this is not a possible state");
                }

                if (payoutLookup.ContainsKey(advance.PersonId))
                {
                    // Yes. Add claim to list.

                    payoutLookup[advance.PersonId].DependentCashAdvancesPayout.Add(advance);
                }
                else
                {
                    // No. Create a new payout for this person.

                    BasicPayout basicPayout = new BasicPayout(0, organizationId, advance.Person.BankName,
                                                                advance.Person.BankClearing + " / " + advance.Person.BankAccount, string.Empty, 0,
                                                                DateTime.MinValue, false, DateTime.Now, 0);
                    Payout payout = Payout.FromBasic(basicPayout);

                    payout.DependentCashAdvancesPayout.Add(advance);

                    payoutLookup[advance.PersonId] = payout;
                }
            }

            // We now have the list of payouts and the associated claims, but the amounts aren't set on the
            // payouts. This will be the next step, as we assemble the list.

            foreach (Payout payout in payoutLookup.Values)
            {
                Int64 newAmountCents = 0;
                List<int> advanceIds = new List<int>();

                foreach (CashAdvance advance in payout.DependentCashAdvancesPayout)
                {
                    newAmountCents += advance.AmountCents;
                    advanceIds.Add(advance.Identity);
                }

                payout.AmountCents = newAmountCents;

                if (advanceIds.Count == 1)
                {
                    payout.Reference = "[Loc]Financial_CashAdvanceSpecification|" + advanceIds[0].ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    advanceIds.Sort();
                    payout.Reference = "[Loc]Financial_CashAdvancesSpecification|" + Formatting.GenerateRangeString(advanceIds);
                }

                if (newAmountCents > 0)
                {
                    payoutList.Add(payout);
                }
            }
        }



        private static void AddUnpaidInboundInvoices(Payouts payoutList, int organizationId)
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
                                                              employee.BankClearing+" / "+employee.BankAccount, "[Loc]Financial_SalarySpecification|[Date]" + salary.PayoutDate.ToString(CultureInfo.InvariantCulture), 
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
                    referenceString = "[Loc]Financial_TaxSpecification|" + identityList[0].ToString();
                }
                else
                {
                    identityList.Sort();
                    referenceString = "[Loc]Financial_TaxesSpecification|" + Formatting.GenerateRangeString(identityList);
                }


                BasicPayout basicPayout = new BasicPayout(0, organizationId, "[Loc]Financial_TheTaxMan",
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
                    if (possiblyCompetingTransaction.Rows.AmountCentsTotal == transaction.Rows.AmountCentsTotal &&
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
                    // Ugly hack to fix cash advance payouts

                    DateTime payoutLowerTimeLimit = timeLow;
                    DateTime payoutUpperTimeLimit = timeHigh;

                    if (payout.AmountCents == -transaction.Rows.AmountCentsTotal && (payout.DependentCashAdvancesPayout.Count > 0 || payout.DependentCashAdvancesPayback.Count > 0))
                    {
                        // HACK: While PW5 doesn't have a manual-debug interface, special case for cash advances

                        payoutLowerTimeLimit = transaction.DateTime.AddDays(-60);
                        payoutUpperTimeLimit = transaction.DateTime.AddDays(60);
                    }

                    if (payout.ExpectedTransactionDate >= payoutLowerTimeLimit && payout.ExpectedTransactionDate <= payoutUpperTimeLimit && payout.AmountCents == -transaction.Rows.AmountCentsTotal)
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
