using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using NBitcoin;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Satoshis = NBitcoin.Money;

namespace Swarmops.Logic.Financial
{
    public class Payouts : PluralBase<Payouts, Payout, BasicPayout>
    {
        [Obsolete("Use TotalAmountCents", true)]
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

        public static Payouts ForOrganization (Organization organization)
        {
            return ForOrganization (organization, false);
        }

        public static Payouts ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetPayouts (organization));
            }
            return FromArray (SwarmDb.GetDatabaseForReading().GetPayouts (organization, DatabaseCondition.OpenTrue));
        }

        public static Payouts Construct (Organization organization)
        {
            // Construct a list of not-yet-created payouts. Basically, these are the things that
            // Economy hasn't yet filed with the bank.

            Payouts result = new Payouts();

            AddUnpaidExpenseClaims (result, organization);
            AddUnpaidInboundInvoices (result, organization);
            AddUnpaidSalaries (result, organization);
            AddUnpaidCashAdvances (result, organization);

            return result;
        }

        private static void AddUnpaidExpenseClaims (Payouts payoutList, Organization organization)
        {
            ExpenseClaims claims = ExpenseClaims.FromOrganization (organization);

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

                        if (payoutLookup.ContainsKey (claim.ClaimingPersonId))
                        {
                            // Yes. Add claim to list.

                            payoutLookup[claim.ClaimingPersonId].DependentExpenseClaims.Add (claim);
                        }
                        else
                        {
                            // No. Create a new payout for this person.

                            BasicPayout basicPayout = new BasicPayout (0, organization.Identity, claim.Claimer.BankName,
                                claim.Claimer.BankClearing + " / " + claim.Claimer.BankAccount, string.Empty, 0,
                                Constants.DateTimeLow, false, DateTime.Now, 0);
                            Payout payout = Payout.FromBasic (basicPayout);
                            payout.RecipientPerson = claim.Claimer;

                            payout.DependentExpenseClaims.Add (claim);

                            payoutLookup[claim.ClaimingPersonId] = payout;
                        }
                    }
                }
            }

            // At this point, all the expense claims have been added - but we need to add the open
            // cash advances and deduct them.

            CashAdvances cashAdvances = CashAdvances.ForOrganization (organization);
            cashAdvances = cashAdvances.WherePaid;

            // At this point, only open and paid cash advances are in the list: they're debts to the org

            foreach (CashAdvance cashAdvance in cashAdvances)
            {
                if (payoutLookup.ContainsKey (cashAdvance.PersonId))
                {
                    // there's a payout prepared to this person - we need to deduct the cash advance from it.

                    payoutLookup[cashAdvance.PersonId].DependentCashAdvancesPayback.Add (cashAdvance);
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
                    claimIds.Add (claim.Identity);
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
                    payout.Reference = "[Loc]Financial_ExpenseClaimSpecification" + lessAdvancesIndicator + "|" +
                                       claimIds[0].ToString (CultureInfo.InvariantCulture);
                }
                else
                {
                    claimIds.Sort();
                    payout.Reference = "[Loc]Financial_ExpenseClaimsSpecification" + lessAdvancesIndicator + "|" +
                                       Formatting.GenerateRangeString (claimIds);
                }

                if (newAmountCents > 0)
                {
                    payoutList.Add (payout);
                }
            }
        }


        private static void AddUnpaidCashAdvances (Payouts payoutList, Organization organization)
        {
            CashAdvances advances = CashAdvances.ForOrganization (organization);
            advances = advances.WhereAttested;
            advances = advances.WhereUnpaid;

            Dictionary<int, Payout> payoutLookup = new Dictionary<int, Payout>();

            foreach (CashAdvance advance in advances)
            {
                // If ready for payout, add to list.

                if (!advance.Open || !advance.Attested || advance.PaidOut)
                {
                    throw new InvalidOperationException (
                        "Got into loop with closed/unattested/paid-out cash advances - this is not a possible state");
                }

                if (payoutLookup.ContainsKey (advance.PersonId))
                {
                    // Yes. Add claim to list.

                    payoutLookup[advance.PersonId].DependentCashAdvancesPayout.Add (advance);
                }
                else
                {
                    // No. Create a new payout for this person.

                    BasicPayout basicPayout = new BasicPayout (0, organization.Identity, advance.Person.BankName,
                        advance.Person.BankClearing + " / " + advance.Person.BankAccount, string.Empty, 0,
                        Constants.DateTimeLow, false, DateTime.Now, 0);
                    Payout payout = Payout.FromBasic (basicPayout);
                    payout.RecipientPerson = advance.Person;

                    payout.DependentCashAdvancesPayout.Add (advance);

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
                    advanceIds.Add (advance.Identity);
                }

                payout.AmountCents = newAmountCents;

                if (advanceIds.Count == 1)
                {
                    payout.Reference = "[Loc]Financial_CashAdvanceSpecification|" +
                                       advanceIds[0].ToString (CultureInfo.InvariantCulture);
                }
                else
                {
                    advanceIds.Sort();
                    payout.Reference = "[Loc]Financial_CashAdvancesSpecification|" +
                                       Formatting.GenerateRangeString (advanceIds);
                }

                if (newAmountCents > 0)
                {
                    payoutList.Add (payout);
                }
            }
        }


        private static void AddUnpaidInboundInvoices (Payouts payoutList, Organization organization)
        {
            InboundInvoices invoices = InboundInvoices.ForOrganization (organization);

            foreach (InboundInvoice invoice in invoices)
            {
                if (invoice.Attested)
                {
                    BasicPayout basicPayout = new BasicPayout (0, organization.Identity, string.Empty,
                        invoice.PayToAccount,
                        invoice.Ocr.Length > 0 ? invoice.Ocr : invoice.InvoiceReference, // prefer OCR (automatic) over Reference (manual)
                        (Int64) (invoice.Amount*100),
                        invoice.DueDate, false, DateTime.Now, 0);
                    Payout payout = Payout.FromBasic (basicPayout);

                    payout.DependentInvoices.Add (invoice);

                    payoutList.Add (payout);
                }
            }
        }

        private static void AddUnpaidSalaries (Payouts payoutList, Organization organization)
        {
            Int64 taxTotalCents = 0;

            Salaries salaries = Salaries.ForOrganization (organization);
            List<int> identityList = new List<int>();
            DateTime payDay = Constants.DateTimeHigh;

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

                    BasicPayout basicPayout = new BasicPayout (0, organization.Identity, employee.BankName,
                        employee.BankClearing + " / " + employee.BankAccount,
                        "[Loc]Financial_SalarySpecification|[Date]" +
                        salary.PayoutDate.ToString (CultureInfo.InvariantCulture),
                        salary.NetSalaryCents, salary.PayoutDate, false, DateTime.Now, 0);
                    Payout payout = Payout.FromBasic (basicPayout);
                    payout.RecipientPerson = employee;

                    payout.DependentSalariesNet.Add (salary);

                    payoutList.Add (payout);

                    if (payDay > salary.PayoutDate)
                    {
                        payDay = salary.PayoutDate;
                    }
                }

                if (!salary.TaxPaid)
                {
                    taxTotalCents += salary.TaxTotalCents;
                    identityList.Add (salary.Identity);

                    if (payDay > salary.PayoutDate)
                    {
                        payDay = salary.PayoutDate;
                    }
                }
            }

            if (taxTotalCents > 0)
            {
                // Add the summarized tax line, too

                BasicPayout basicPayout = new BasicPayout (0, organization.Identity, "[Loc]Financial_TheTaxMan", "SEBG 5050-1055", // HACK: Get tax account from something
                    organization.TaxPaymentOcr, taxTotalCents, payDay, false, DateTime.Now, 0);
                Payout payout = Payout.FromBasic (basicPayout);

                foreach (int salaryId in identityList)
                {
                    payout.DependentSalariesTax.Add (Salary.FromIdentity (salaryId));
                }

                payoutList.Add (payout);
            }
        }


        public static void AutomatchAgainstUnbalancedTransactions (Organization organization)
        {
            // Matches unbalanced financial transactions against unclosed payouts

            // Should this be in bot?

            Payouts payouts = ForOrganization (organization);

            FinancialTransactions transactions = FinancialTransactions.GetUnbalanced (organization);

            foreach (FinancialTransaction transaction in transactions)
            {
                // Console.WriteLine("Looking at transaction #{0} ({1:yyyy-MM-dd}, {2:N2}).", transaction.Identity, transaction.DateTime, transaction.Rows.AmountTotal);

                // First, establish that there are no similar transactions within 7 days. N^2 search.

                DateTime timeLow = transaction.DateTime.AddDays (-7);
                DateTime timeHigh = transaction.DateTime.AddDays (7);

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

                    if (payout.AmountCents == -transaction.Rows.AmountCentsTotal &&
                        (payout.DependentCashAdvancesPayout.Count > 0 || payout.DependentCashAdvancesPayback.Count > 0))
                    {
                        // HACK: While PW5 doesn't have a manual-debug interface, special case for cash advances

                        payoutLowerTimeLimit = transaction.DateTime.AddDays (-60);
                        payoutUpperTimeLimit = transaction.DateTime.AddDays (60);
                    }

                    // HACK: Allow for up to 20 days beyond scheduled payment to catch tax payments

                    if (payout.DependentSalariesTax.Count > 0)
                    {
                        payoutLowerTimeLimit = transaction.DateTime.AddDays (-25);
                        payoutUpperTimeLimit = transaction.DateTime.AddDays (3); // nobody pays taxes early...
                    }

                    if (payout.ExpectedTransactionDate >= payoutLowerTimeLimit &&
                        payout.ExpectedTransactionDate <= payoutUpperTimeLimit &&
                        payout.AmountCents == -transaction.Rows.AmountCentsTotal)
                    {
                        // Console.WriteLine(" - - payout #{0} matches ({1}, {2:yyyy-MM-dd})", payout.Identity, payout.Recipient, payout.ExpectedTransactionDate);

                        try
                        {
                            FinancialTransaction tiedTransaction = FinancialTransaction.FromDependency (payout);

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
                    Payout payout = Payout.FromIdentity (payoutIdFound);
                    payout.BindToTransactionAndClose (transaction, null);
                }
            }
        }

        public static void PerformAutomated()
        {
            // Perform all waiting hot payouts for all orgs in the installation

            DateTime utcNow = DateTime.UtcNow;

            foreach (Organization organization in Organizations.GetAll())
            {
                // If this org doesn't do hotwallet, continue
                if (organization.FinancialAccounts.AssetsBitcoinHot == null)
                {
                    continue;
                }

                Payouts orgPayouts = Payouts.Construct (organization);
                Payouts bitcoinPayouts = new Payouts();
                Dictionary<string, Int64> satoshiPayoutLookup = new Dictionary<string, long>();
                Dictionary<string, Int64> nativeCentsPayoutLookup = new Dictionary<string, long>();
                Dictionary<int, Int64> satoshiPersonLookup = new Dictionary<int, long>();
                Dictionary <int, Int64> nativeCentsPersonLookup = new Dictionary<int, long>();
                Int64 satoshisTotal = 0;

                string currencyCode = organization.Currency.Code;

                // For each ready payout that can automate, add an output to a constructed transaction

                TransactionBuilder txBuilder = new TransactionBuilder();
                foreach (Payout payout in orgPayouts)
                {
                    if (payout.ExpectedTransactionDate > utcNow)
                    {
                        continue; // payout is not due yet
                    }

                    if (payout.RecipientPerson != null && payout.RecipientPerson.BitcoinPayoutAddress.Length > 2 &&
                        payout.Account.Length < 4)
                    {
                        // If the payout address is still in quarantine, don't pay out yet

                        string addressSetTime = payout.RecipientPerson.BitcoinPayoutAddressTimeSet;
                        if (addressSetTime.Length > 4 && DateTime.Parse (addressSetTime, CultureInfo.InvariantCulture).AddHours (48) > utcNow)
                        {
                            continue; // still in quarantine
                        }

                        // Test the payout address - is it valid and can we handle it?

                        if (!BitcoinUtility.IsValidBitcoinAddress(payout.RecipientPerson.BitcoinPayoutAddress))
                        {
                            // Notify person that address is invalid, then clear it

                            NotificationStrings primaryStrings = new NotificationStrings();
                            NotificationCustomStrings secondaryStrings = new NotificationCustomStrings();
                            primaryStrings[NotificationString.OrganizationName] = organization.Name;
                            secondaryStrings["BitcoinAddress"] = payout.RecipientPerson.BitcoinPayoutAddress;

                            OutboundComm.CreateNotification(organization, NotificationResource.BitcoinPayoutAddress_Bad,
                                primaryStrings, secondaryStrings,
                                People.FromSingle(payout.RecipientPerson));

                            payout.RecipientPerson.BitcoinPayoutAddress = string.Empty;

                            continue; // do not add this payout
                        }

                        // Ok, so it seems we're making this payout at this time.

                        bitcoinPayouts.Add (payout);

                        int recipientPersonId = payout.RecipientPerson.Identity;

                        if (!satoshiPersonLookup.ContainsKey (recipientPersonId))
                        {
                            satoshiPersonLookup[recipientPersonId] = 0;
                            nativeCentsPersonLookup[recipientPersonId] = 0;
                        }

                        nativeCentsPersonLookup[recipientPersonId] += payout.AmountCents;

                        // Find the amount of satoshis for this payout

                        if (organization.Currency.IsBitcoin)
                        {
                            satoshiPayoutLookup[payout.ProtoIdentity] = payout.AmountCents;
                            nativeCentsPayoutLookup[payout.ProtoIdentity] = payout.AmountCents;
                            satoshisTotal += payout.AmountCents;
                            satoshiPersonLookup[recipientPersonId] += payout.AmountCents;
                        }
                        else
                        {
                            // Convert currency
                            Money payoutAmount = new Money(payout.AmountCents, organization.Currency);
                            Int64 satoshis = payoutAmount.ToCurrency(Currency.Bitcoin).Cents;
                            satoshiPayoutLookup[payout.ProtoIdentity] = satoshis;
                            nativeCentsPayoutLookup[payout.ProtoIdentity] = payout.AmountCents;
                            satoshisTotal += satoshis;
                            satoshiPersonLookup[recipientPersonId] += satoshis;
                        }
                    }
                    else if (payout.RecipientPerson != null && payout.RecipientPerson.BitcoinPayoutAddress.Length < 3 && payout.Account.Length < 4)
                    {
                        // There is a payout for this person, but they don't have a bitcoin payout address set. Send notification to this effect once a day.

                        if (utcNow.Minute != 0)
                        {
                            continue;
                        }
                        if (utcNow.Hour != 12)
                        {
                            continue;
                        }

                        NotificationStrings primaryStrings = new NotificationStrings();
                        primaryStrings[NotificationString.OrganizationName] = organization.Name;
                        OutboundComm.CreateNotification(organization, NotificationResource.BitcoinPayoutAddress_PleaseSet, primaryStrings, People.FromSingle(payout.RecipientPerson));
                    }
                    else if (payout.Account.StartsWith ("bitcoin:"))
                    {
                        
                    }
                }

                if (bitcoinPayouts.Count == 0)
                {
                    // no automated payments pending for this organization, nothing more to do
                    continue;
                }

                // We now have our desired payments. The next step is to find enough inputs to reach the required amount (plus fees; we're working a little blind here still).

                BitcoinTransactionInputs inputs = null;
                Int64 satoshisMaximumAnticipatedFees = BitcoinUtility.GetRecommendedFeePerThousandBytesSatoshis() * 20; // assume max 20k transaction size

                try
                {
                    inputs = BitcoinUtility.GetInputsForAmount(organization, satoshisTotal + satoshisMaximumAnticipatedFees); 
                }
                catch (NotEnoughFundsException)
                {
                    // If we're at the whole hour, send a notification to people responsible for refilling the hotwallet

                    if (utcNow.Minute != 0)
                    {
                        continue; // we're not at the whole hour, so continue with next org instead
                    }

                    // Send urgent notification to top up the damn wallet so we can make payments

                    NotificationStrings primaryStrings = new NotificationStrings();
                    primaryStrings[NotificationString.CurrencyCode] = organization.Currency.Code;
                    primaryStrings[NotificationString.OrganizationName] = organization.Name;
                    NotificationCustomStrings secondaryStrings = new NotificationCustomStrings();
                    Int64 satoshisAvailable = HotBitcoinAddresses.ForOrganization (organization).BalanceSatoshisTotal;

                    secondaryStrings["AmountMissingMicrocoinsFloat"] =
                        ((satoshisTotal - satoshisAvailable + satoshisMaximumAnticipatedFees) / 100.0).ToString("N2");

                    if (organization.Currency.IsBitcoin)
                    {
                        secondaryStrings["AmountNeededFloat"] = ((satoshisTotal + satoshisMaximumAnticipatedFees) / 100.0).ToString("N2");
                        secondaryStrings["AmountWalletFloat"] = (satoshisAvailable/100.0).ToString ("N2");
                    }
                    else
                    {
                        // convert to org native currency

                        secondaryStrings["AmountNeededFloat"] =
                            (new Money (satoshisTotal, Currency.Bitcoin).ToCurrency (organization.Currency).Cents/100.0).ToString ("N2");
                        secondaryStrings["AmountWalletFloat"] =
                            (new Money (satoshisAvailable, Currency.Bitcoin).ToCurrency (organization.Currency).Cents/100.0).ToString ("N2");
                    }

                    OutboundComm.CreateNotification (organization,
                        NotificationResource.Bitcoin_Shortage_Critical, primaryStrings, secondaryStrings, People.FromSingle (Person.FromIdentity (1)));

                    continue; // with next organization
                }

                // If we arrive at this point, the previous function didn't throw, and we have enough money. 
                // Ensure the existence of a cost account for bitcoin miner fees.

                organization.EnsureMinerFeeAccountExists();

                // Add the inputs to the transaction.

                txBuilder = txBuilder.AddCoins (inputs.Coins);
                txBuilder = txBuilder.AddKeys (inputs.PrivateKeys);
                Int64 satoshisInput = inputs.AmountSatoshisTotal;

                // Add outputs and prepare notifications

                Int64 satoshisUsed = 0;
                Dictionary<int, List<string>> notificationSpecLookup = new Dictionary<int, List<string>>();
                Dictionary<int, List<Int64>> notificationAmountLookup = new Dictionary<int, List<Int64>>();
                Payout masterPayoutPrototype = Payout.Empty;
                HotBitcoinAddress changeAddress = HotBitcoinAddress.OrganizationWalletZero (organization);

                foreach (Payout payout in bitcoinPayouts)
                {
                    int recipientPersonId = payout.RecipientPerson.Identity;
                    if (!notificationSpecLookup.ContainsKey (recipientPersonId))
                    {
                        notificationSpecLookup[recipientPersonId] = new List<string>();
                        notificationAmountLookup[recipientPersonId] = new List<Int64>();
                    }
                    notificationSpecLookup[recipientPersonId].Add (payout.Specification);
                    notificationAmountLookup[recipientPersonId].Add (payout.AmountCents);

                    if (payout.RecipientPerson.BitcoinPayoutAddress.StartsWith ("1")) // regular address
                    {
                        txBuilder = txBuilder.Send (new BitcoinPubKeyAddress (payout.RecipientPerson.BitcoinPayoutAddress),
                            new Satoshis (satoshiPayoutLookup[payout.ProtoIdentity]));
                    }
                    else if (payout.RecipientPerson.BitcoinPayoutAddress.StartsWith ("3")) // multisig
                    {
                        txBuilder = txBuilder.Send(new BitcoinScriptAddress(payout.RecipientPerson.BitcoinPayoutAddress, Network.Main),
                            new Satoshis(satoshiPayoutLookup[payout.ProtoIdentity]));
                    }
                    else
                    {
                        throw new InvalidOperationException("Unhandled bitcoin address type in Payouts.PerformAutomated(): " + payout.RecipientPerson.BitcoinPayoutAddress);
                    }

                    satoshisUsed += satoshiPayoutLookup[payout.ProtoIdentity];

                    payout.MigrateDependenciesTo (masterPayoutPrototype);
                }

                // Set change address to wallet slush

                txBuilder.SetChange (new BitcoinPubKeyAddress (changeAddress.Address));

                // Add fee

                int transactionSizeBytes = txBuilder.EstimateSize (txBuilder.BuildTransaction (false)) + inputs.Count; 
                // +inputs.Count for size variability

                Int64 feeSatoshis = (transactionSizeBytes/1000 + 1) * BitcoinUtility.GetRecommendedFeePerThousandBytesSatoshis();
                
                txBuilder = txBuilder.SendFees (new Satoshis (feeSatoshis));
                satoshisUsed += feeSatoshis;

                // Sign transaction - ready to execute

                Transaction txReady = txBuilder.BuildTransaction (true);

                // Verify that transaction is ready

                if (!txBuilder.Verify (txReady))
                {
                    // Transaction was not signed with the correct keys. This is a serious condition.

                    NotificationStrings primaryStrings = new NotificationStrings();
                    primaryStrings[NotificationString.OrganizationName] = organization.Name;

                    OutboundComm.CreateNotification (organization, NotificationResource.Bitcoin_PrivateKeyError,
                        primaryStrings);

                    throw new InvalidOperationException("Transaction is not signed enough");
                }

                // Broadcast transaction

                BitcoinUtility.BroadcastTransaction (txReady);

                // Note the transaction hash

                string transactionHash = txReady.GetHash().ToString();

                // Delete all old inputs, adjust balance for addresses (re-register unused inputs)

                inputs.AsUnspents.DeleteAll();

                // Log the new unspent created by change (if there is any)

                if (satoshisInput - satoshisUsed > 0)
                {
                    SwarmDb.GetDatabaseForWriting()
                        .CreateHotBitcoinAddressUnspentConditional(changeAddress.Identity, transactionHash,
                            + /* the change address seems to always get index 0? is this a safe assumption? */ 0, satoshisInput - satoshisUsed, /* confirmation count*/ 0);
                }

                // Register new balance of change address, should have increased by (satoshisInput-satoshisUsed)

                // TODO

                // Send notifications

                foreach (int personId in notificationSpecLookup.Keys)
                {
                    Person person = Person.FromIdentity (personId);

                    string spec = string.Empty;
                    for (int index = 0; index < notificationSpecLookup[personId].Count; index++)
                    {
                        spec += String.Format(" * {0,-40} {1,14:N2} {2,-4}\r\n", notificationSpecLookup[personId][index], notificationAmountLookup[personId][index]/100.0, currencyCode);
                    }

                    spec = spec.TrimEnd();

                    NotificationStrings primaryStrings = new NotificationStrings();
                    NotificationCustomStrings secondaryStrings = new NotificationCustomStrings();

                    primaryStrings[NotificationString.OrganizationName] = organization.Name;
                    primaryStrings[NotificationString.CurrencyCode] = organization.Currency.DisplayCode;
                    primaryStrings[NotificationString.EmbeddedPreformattedText] = spec;
                    secondaryStrings["AmountFloat"] = (nativeCentsPersonLookup[personId]/100.0).ToString ("N2");
                    secondaryStrings["BitcoinAmountFloat"] = (satoshiPersonLookup[personId]/100.0).ToString ("N2");
                    secondaryStrings["BitcoinAddress"] = person.BitcoinPayoutAddress; // warn: potential rare race condition here

                    OutboundComm.CreateNotification (organization, NotificationResource.Bitcoin_PaidOut, primaryStrings,
                        secondaryStrings, People.FromSingle(person));
                }

                // Create the master payout from its prototype

                Payout masterPayout = Payout.CreateBitcoinPayoutFromPrototype (organization, masterPayoutPrototype, txReady.GetHash().ToString());

                // Finally, create ledger entries and notify

                NotificationStrings masterPrimaryStrings = new NotificationStrings();
                NotificationCustomStrings masterSecondaryStrings = new NotificationCustomStrings();

                masterPrimaryStrings[NotificationString.OrganizationName] = organization.Name;
                masterPrimaryStrings[NotificationString.CurrencyCode] = organization.Currency.DisplayCode;
                masterSecondaryStrings["AmountFloat"] =
                    (new Swarmops.Logic.Financial.Money (satoshisUsed, Currency.Bitcoin).ToCurrency (
                        organization.Currency).Cents/100.0).ToString ("N2", CultureInfo.InvariantCulture);
                masterSecondaryStrings["BitcoinAmountFloat"] = (satoshisUsed/100.0).ToString ("N2", CultureInfo.InvariantCulture);
                masterSecondaryStrings["PaymentCount"] = bitcoinPayouts.Count.ToString("N0", CultureInfo.InvariantCulture);

                OutboundComm.CreateNotification (organization, NotificationResource.Bitcoin_Hotwallet_Outflow,
                    masterPrimaryStrings, masterSecondaryStrings);

                // TODO: special case for native-bitcoin organizations vs. fiat-currency organizations

                FinancialTransaction ledgerTransaction = FinancialTransaction.Create (organization, utcNow,
                    "Bitcoin automated payout");

                if (organization.Currency.IsBitcoin)
                {
                    ledgerTransaction.AddRow(organization.FinancialAccounts.AssetsBitcoinHot, -(masterPayoutPrototype.AmountCents + feeSatoshis), null);
                    ledgerTransaction.AddRow (organization.FinancialAccounts.CostsBitcoinFees, feeSatoshis, null);
                }
                else
                {
                    // If the ledger isn't using bitcoin natively, we need to translate the miner fee paid to ledger cents before entering it into ledger

                    Int64 feeCentsLedger = new Money (feeSatoshis, Currency.Bitcoin).ToCurrency (organization.Currency).Cents;
                    ledgerTransaction.AddRow(organization.FinancialAccounts.AssetsBitcoinHot, -(masterPayoutPrototype.AmountCents + feeCentsLedger), null).AmountForeignCents = new Money(satoshisUsed, Currency.Bitcoin);
                    ledgerTransaction.AddRow (organization.FinancialAccounts.CostsBitcoinFees, feeCentsLedger, null);
                }

                ledgerTransaction.BlockchainHash = transactionHash;

                masterPayout.BindToTransactionAndClose (ledgerTransaction, null);
            }
        }
    }
}