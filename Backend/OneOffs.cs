using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Utility.Special.Sweden;
using Swarmops.Basic;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Governance;
using Swarmops.Logic.Media;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Support;
using Swarmops.Utility.BotCode;
using Swarmops.Utility.Mail;

namespace Swarmops.Backend
{

    public class OneOffs
    {
        public static void RepairFinancials()
        {
            /*
            foreach (int claimId in new int[] { 940, 941})
            {
                ExpenseClaim claim = ExpenseClaim.FromIdentity(claimId);
                claim.Recalibrate();
            }*/

            OutboundInvoices invoices = OutboundInvoices.ForOrganization(Organization.PPSE, true);

            foreach (OutboundInvoice invoice in invoices)
            {
                if (invoice.Reference != Formatting.AddLuhnChecksum(invoice.Reference.Substring(0, invoice.Reference.Length - 1)))
                {
                    Console.WriteLine("Invoice #{0} ({1}) has a broken reference", invoice.Identity,
                                      invoice.Open ? "open" : "closed");

                    if (invoice.Open)
                    {
                        string newReference =
                            Formatting.AddLuhnChecksum(invoice.Reference.Substring(0, invoice.Reference.Length - 1));

                        Console.WriteLine(" - changing");
                        invoice.Reference = newReference;
                    }
                }
            }
        }

        /* -- commented out - this is 1) fixed and 2) calls obsolete double-precision financial code. 

        public static void DebugExpensesAgain()
        {
            ExpenseClaims claims = ExpenseClaims.ForOrganization(Organization.PPSE);
            Payouts payouts = Payouts.ForOrganization(Organization.PPSE);

            decimal accumulated = 0.0m;
            decimal accumulatedRecords = 0.0m;

            foreach (ExpenseClaim claim in claims)
            {
                decimal rowsExpenses = 0.0m;

                FinancialTransaction transaction = claim.FinancialTransaction;
                if (transaction == null) continue;

                foreach (FinancialTransactionRow row in transaction.Rows)
                {
                    if (row.FinancialAccountId == 3)
                    {
                        rowsExpenses -= row.Amount;
                    }
                }

                accumulated += rowsExpenses;
                accumulatedRecords += claim.Amount;

                Console.WriteLine("Claim #{0,-4}  {1,12:N2} {2,12:N2} {3,12:N2} {4,12:N2} {5,12:N2}", claim.Identity, claim.Amount, rowsExpenses, accumulated, accumulatedRecords, accumulatedRecords - accumulated);
            }

            foreach (Payout payout in payouts)
            {
                foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
                {
                    decimal rowsExpenses = 0.0m;

                    FinancialTransaction transaction = claim.FinancialTransaction;
                    if (transaction == null) continue;

                    foreach (FinancialTransactionRow row in transaction.Rows)
                    {
                        if (row.FinancialAccountId == 3)
                        {
                            rowsExpenses -= row.Amount;
                        }
                    }

                    accumulated += rowsExpenses;
                    accumulatedRecords += claim.Amount;

                    Console.WriteLine("Claim #{0,-4}  {1,12:N2} {2,12:N2} {3,12:N2} {4,12:N2} {5,12:N2}", claim.Identity,
                                      claim.Amount, rowsExpenses, accumulated, accumulatedRecords,
                                      accumulatedRecords - accumulated);
                }
            }


            
            foreach (ExpenseClaim claim in claims)
            {
                Console.WriteLine("#{0,-5} {1,-30} {2,8:N2} {3}", claim.Identity, claim.Description, claim.Amount, claim.AmountCents);

                FinancialTransaction transaction = claim.FinancialTransaction;

                if (transaction == null)
                {
                    Console.WriteLine(" - null transaction");
                }

                FinancialTransactionRows rows = transaction.Rows;

                Console.WriteLine(" - Tx#{0} {1:yyyy-MM-dd}", transaction.Identity, transaction.DateTime);

                foreach (FinancialTransactionRow row in rows)
                {
                    Console.WriteLine(" - {1,-30} {2,8:N2} {3}", row.Identity, row.AccountName, row.Amount,
                                      row.AmountCents);
                }
            }
        } */

        public static void SmsMembersOnElectionDay2010()
        {
            SupportCase[] cases = SupportDatabase.GetOpenCases();
            Dictionary<string, bool> mailLookup = new Dictionary<string, bool>();

            Console.WriteLine("{0} cases.", cases.Length);

            // Do not SMS people with open I-want-out mails. These are the only ones open.

            foreach (SupportCase supportCase in cases)
            {
                mailLookup[supportCase.Email.ToLower()] = true;
            }

            Memberships memberships = Organization.PPSE.GetMemberships();

            int total = memberships.Count;
            int count = 0;

            Console.WriteLine("{0} memberships for PPSE.", total);
            foreach (Membership membership in memberships)
            {
                Person person = membership.Person;

                if (mailLookup.ContainsKey(person.Email.ToLower()) && person.Identity != 1 && person.Identity != 437)
                {
                    Console.WriteLine("\rSkipping " + person.Canonical + "; has open support case");
                }
                else
                {
                    count++;

                    Console.Write("\r{0,05}/{1,05}", count, total);

                    try
                    {
                        person.SendPhoneMessage("PP: Hej! Glöm inte att rösta på Piratpartiet idag, och ta gärna med dig en kompis eller två. Vallokalerna stänger kl 20.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("\r" + person.Canonical + " failed.");
                    }
                }

            }
        }

        public static void FixOutboundInvoices()
        {
            OutboundInvoices invoices = OutboundInvoices.ForOrganization(Organization.PPSE);

            foreach (OutboundInvoice invoice in invoices)
            {
                if (invoice.DueDate > DateTime.Today && invoice.Identity == 18)
                {
                    string newReference = Formatting.AddLuhnChecksum(invoice.Reference.Substring(0, invoice.Reference.Length - 2));

                    if (newReference != invoice.Reference)
                    {
                        string mailBody = "Hej!\r\n\r\n" +
                                          "Du har tyvärr fått en faktura från Piratpartiet som hade ett ogiltigt OCR-nummer (" +
                                          invoice.Reference +
                                          "). Det fungerar inte. Använd följande OCR-nummer i stället:\r\n\r\n" +
                                          newReference + "\r\n\r\n" +
                                          "För att se en bild på fakturan med nya OCR-numret, klicka på denna länk:\r\n" +
                                          "http://data.piratpartiet.se/Forms/DisplayOutboundInvoice.aspx?Reference=" +
                                          newReference + "&Culture=svse\r\n";


                        MailMessage newMessage = new MailMessage("accounting@piratpartiet.se",
                                                                 invoice.InvoiceAddressMail,
                                                                 "Faktura från Piratpartiet -- nytt OCR-nummer",
                                                                 mailBody);
                        newMessage.BodyEncoding = Encoding.UTF8;
                        newMessage.SubjectEncoding = Encoding.UTF8;

                        SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
                        client.Credentials = null;
                        client.Send(newMessage);

                        Console.WriteLine("#" + invoice.Identity.ToString() + ": " + invoice.Reference + " => " + newReference);

                        invoice.Reference = newReference;

                    }
                }
            }
        }

        public static void InviteBack()
        {
            Memberships memberships = Memberships.ForOrganization(Organization.PPSE, true);
            DateTime earlyCutoff = new DateTime(2010, 5, 1);
            DateTime lateCutoff = new DateTime(2010,6,1);
            int mailedTotal = 0;

            foreach (Membership membership in memberships)
            {
                Console.Write(membership.Person.Canonical + "...");
                if (membership.Person.MemberOf(Organization.PPSE))  // this checks other memberships, too
                {
                    Console.WriteLine(" active");
                }
                else
                {
                    if (membership.DateTerminated < earlyCutoff)
                    {
                        Console.WriteLine(" early (" + membership.DateTerminated.ToString("yyyy-MM-dd") + ")");
                    }
                    else if (membership.DateTerminated > lateCutoff)
                    {
                        Console.WriteLine(" late (" + membership.DateTerminated.ToString("yyyy-MM-dd") + ")");
                    }
                    else
                    {
                        Console.Write(" IN RANGE (" + membership.DateTerminated.ToString("yyyy-MM-dd") + ")...");

                        InviteOneBack(membership.Person);

                        Console.WriteLine(" mailed");
                        mailedTotal++;
                    }
                }
            }

            Console.WriteLine("\r\n\r\nMailed a total of {0} people out of {1} memberships.", mailedTotal, memberships.Count);
        }

        private static void InviteOneBack(Person person)
        {
            string tokenBase = person.Name.Replace(" ", "-") + person.PasswordHash + "-" + DateTime.Today.Year.ToString();

            string stdLink = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?MemberId=" +
                 person.Identity.ToString() +
                 "&SecHash=" + SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8);

            string body = 
                
                "Vi saknar dig! Saknar du oss?\r\n\r\n" +

                "Nu är det två månader sedan ditt medlemskap i Piratpartiet gick ut, och vi vill gärna att du förnyar ditt medlemskap. Det är naturligtvis gratis. Det gör du genom att klicka på den här länken:\r\n\r\n" +

                stdLink + "\r\n\r\n" +

                "Det som hänt de senaste månaderna är alarmerande och starka skäl att vara en del av den rörelse som vill bygga ett nytt och annorlunda samhälle. Ett samhälle där grundläggande rättigheter respekteras. Här är ett par exempel på vad som istället har hänt nyligen:\r\n\r\n" +

                "- I EU så drevs det alldeles nyligen igenom att alla våra sökningar på nätet ska registreras och lagras i upp till två år för att kunna användas mot oss.\r\n\r\n" +

                "- Samma EU-personer, från Italien, arbetar aktivt för att eliminera all anonymitet på nätet -- att ingen ska kunna använda Internet utan att identifiera sig först. Men yttrandefrihet och åsiktsfrihet kräver anonymitet, så ledande politiker är ute efter att eliminera riktigt grundläggande rättigheter.\r\n\r\n" +

                "- Här i Sverige försökte Journalistförbundet nyligen olagligförklara länkning till nyhetsartiklar (och därigenom länkning som koncept).\r\n\r\n" +

                "- Samtliga riksdagspartier tänker införa datalagring i Sverige, som loggar alla kontakter vi tar med våra medmänniskor i det uttryckliga syftet att kunna använda det mot oss. Plus att logga hur våra mobiltelefoner rör sig genom staden. (Vissa politiker försöker blanda bort korten med att prata om \"vad de vill\". Gå inte på det. Fråga efter \"hur de tänker rösta\". Det visar sig att det inte är samma sak.)\r\n\r\n" +

                "- En sak som inte många känner till är att polisen redan idag får sätta upp dolda kameror och mikrofoner i ditt hem, utan att du är misstänkt för något brott.\r\n\r\n" +

                "Men samtidigt har Piratpartiet gjort skillnad nere i EU-parlamentet, på punkt efter punkt. Vi var drivande i att öppna upp Acta-förhandlingarna, vi var drivande i att tvinga fram rättssäkerhet i Telekompaketet, vi var drivande i att stoppa Swift-avtalet som skulle givit USA tillgång till alla våra privata bankdata. Och mycket mer, hela tiden. Därför behövs vi i svenska riksdagen också.\r\n\r\n" +
                
                "Vi går in i en intensiv valrörelse nu -- valet är om 56 dagar. Det här är slutskedet. Om vi ska lyckas slå vakt om grundläggande fundamenta för det öppna samhället, så är det i det här valet som det måste ske.\r\n\r\n" +

                "Därför ser vi gärna att du återvänder till Piratpartiet som medlem. Vi sparar bara medlemsdata i tre månader efter medlemskapet har gått ut, så det här kan vara sista påminnelsen du får.\r\n\r\n" +

                "Medlemskapet är, som alltid, gratis. Gå med igen genom att klicka på den här länken:\r\n\r\n" +

                stdLink + "\r\n\r\n" +

                "Hälsningar,\r\n" +
                "Rick Falkvinge\r\n";


            OutboundMail mail = OutboundMail.Create(Person.FromIdentity(1), "Vi saknar dig!", body, 10, 0, Organization.PPSE, Geography.Root);
            mail.AddRecipient(person, false);
            mail.SetRecipientCount(1);
            mail.SetResolved();
            mail.SetReadyForPickup();
        }


        public static void MigrateAutoMails()
        {
            /*
            BasicAutoMail[] allAutoMails = AutoMail.GetAllForMigration();

            foreach (BasicAutoMail autoMail in allAutoMails)
            {
                int id = PirateDb.GetDatabaseForWriting().SetAutoMail(autoMail);

                Organization org = null;

                try
                {
                    org = Organization.FromIdentity(autoMail.OrganizationId);
                }
                catch (Exception)
                {
                }

                Console.WriteLine("{3,3} {0} {1} {2}", org == null? "NULL": org.Name, autoMail.Type.ToString(), Geography.FromIdentity(autoMail.GeographyId).Name, id);
            }*/
        }


        public static void MigrateFlatData()
        {
            /*
            Dictionary<string, string> data = Persistence.All;

            foreach (string key in data.Keys)
            {
                if (data[key].Length > 30)
                {
                    Console.WriteLine("{0}: [Length {1}]", key, data[key].Length);
                }
                else
                {
                    Console.WriteLine("{0}: {1}", key, data[key]);
                }

                Persistence.Key[key] = data[key];
            }*/
        }


        public static void CloseBooksForYear (int year)
        {
            /*
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.PPSE);
            decimal balanceDelta = 0.0m;
            decimal resultsDelta = 0.0m;

            foreach (FinancialAccount account in accounts)
            {
                decimal accountBalance;
                string output;

                if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
                {
                    accountBalance = account.GetDelta(new DateTime(2006, 1, 1), new DateTime(year+1,1,1));
                    balanceDelta += accountBalance;
                    output = string.Format("{0,40} {1,14:N2}", account.Name, accountBalance);
                }
                else
                {
                    accountBalance = account.GetDelta(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));
                    resultsDelta += accountBalance;
                    output = string.Format("{0,40} {1,14} {2,14:N2}", account.Name, string.Empty, accountBalance);
                }

                Console.WriteLine(output);
            }

            Console.WriteLine("{0,40} {1,14:N2} {2,14:N2}", "DELTA TOTAL (balance, result)", balanceDelta, resultsDelta);

            if (balanceDelta == -resultsDelta && year < DateTime.Today.Year)
            {
                Console.WriteLine("Everything matches. Creating year-end transaction and closing books.");
                FinancialTransaction resultTransaction = FinancialTransaction.Create(1, new DateTime(year, 12, 31, 23, 59, 00), "Årets resultat " + year.ToString());
                resultTransaction.AddRow(FinancialAccount.FromIdentity(97), (double)-resultsDelta, null);
                resultTransaction.AddRow(FinancialAccount.FromIdentity(96), (double)-balanceDelta, null);

                Organization.PPSE.Parameters.FiscalBooksClosedUntilYear = year;
            }
            else
            {
                Console.WriteLine("NOT creating transaction.");
                Console.WriteLine("balanceDelta > 0.0: {0}", balanceDelta > 0.0m);
                Console.WriteLine("balanceDelta == -resultsDelta: {0}", balanceDelta == -resultsDelta);
                Console.WriteLine("{0} {1}", balanceDelta, resultsDelta);
            }*/

        }

        
        public static void Calculate2010Results()
        {
            DateTime iterator = new DateTime(2010, 11, 1);

            Console.WriteLine("Checking until " + iterator.ToString("yyyy-MMM-dd") + ".\r\n");

            while (!EconomyHasMismatches(iterator) && iterator < new DateTime(2011,1,2))
            {
                iterator = iterator.AddDays(1);

                Console.WriteLine("\r\nChecking until " + iterator.ToString("yyyy-MMM-dd") + ".\r\n");
            }
        }

        public static bool EconomyHasMismatches (DateTime untilDate)
        {
            return false;

            /*
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.PPSE);
            decimal balanceDelta = 0.0m;
            decimal resultsDelta = 0.0m;

            foreach (FinancialAccount account in accounts)
            {
                decimal accountBalance;
                string output;

                if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
                {
                    accountBalance = account.GetDelta(new DateTime(2006,1,1), untilDate);
                    balanceDelta += accountBalance;
                    output = string.Format("{0,40} {1,14:N2}", account.Name, accountBalance);
                }
                else
                {
                    accountBalance = account.GetDelta(new DateTime(untilDate.AddDays(-1).Year, 1, 1), untilDate);
                    resultsDelta += accountBalance;
                    output = string.Format("{0,40} {1,14} {2,14:N2}", account.Name, string.Empty, accountBalance);
                }

                Console.WriteLine(output);
            }

            Console.WriteLine("{0,40} {1,14:N2} {2,14:N2}", "DELTA TOTAL (balance, result)", balanceDelta, resultsDelta);

            return (balanceDelta != -resultsDelta);

            if (false) // balanceDelta > 1.0m)
            {
                Console.WriteLine("Creating year-end transaction and closing books.");
                FinancialTransaction resultTransaction = FinancialTransaction.Create(1, new DateTime (2010,12,31,23,59,00), "Årets resultat 2010");
                resultTransaction.AddRow(FinancialAccount.FromIdentity(97), (double) -resultsDelta, null);
                resultTransaction.AddRow(FinancialAccount.FromIdentity(96), (double) -balanceDelta, null);

                Organization.PPSE.Parameters.FiscalBooksClosedUntilYear = 2010;
            }
            else
            {
                Console.WriteLine("NOT creating transaction.");
                Console.WriteLine("balanceDelta > 0.0: {0}", balanceDelta > 0.0m);
                Console.WriteLine("balanceDelta == -resultsDelta: {0}", balanceDelta == -resultsDelta);
                Console.WriteLine("{0} {1}", balanceDelta, resultsDelta);
            }
            */
        }

        public static void CarryOver2010LocalBudgets()
        {
            /*
            FinancialAccount rootAccount = FinancialAccount.FromIdentity(29);
            FinancialAccount tempAccount = FinancialAccount.FromIdentity(98);

            FinancialAccounts localAccounts = rootAccount.GetTree();

            foreach (FinancialAccount account in localAccounts)
            {
                decimal currentBalance = account.GetDelta(new DateTime(2010,1,1), new DateTime(2011,1,1));
                decimal budget = (decimal) -account.GetBudget(2010);
                decimal carryOver = budget - currentBalance;

                string output = string.Format("{0,-40} {1,10:N2} {2,10:N2} {3,10:N2}", account.Name, currentBalance,
                                              budget, carryOver);
                Console.WriteLine(output);

                FinancialTransaction transaction2010 = FinancialTransaction.Create(1,
                                                                                   new DateTime(2010, 12, 31, 23, 50,
                                                                                                00),
                                                                                   "Budgetrest " + account.Name);

                transaction2010.AddRow(account, (double) carryOver, null);
                transaction2010.AddRow(tempAccount, (double) -carryOver, null);

                FinancialTransaction transaction2011 = FinancialTransaction.Create(1,
                                                                                   new DateTime(2011, 1, 1, 0, 10, 0),
                                                                                   "Budgetrest 2010 " + account.Name);

                transaction2011.AddRow(account, (double) -carryOver, null);
                transaction2011.AddRow(tempAccount, (double) carryOver, null);
            }*/
        }
        


        public static void ForciblyInviteToAnnualMeting()
        {
            string body =
                "Piratpartiet kallar till årsmöte.\r\n\r\n" +
                "Kallelsen har gått ut tidigare till medlemmar som prenumererar på nyhetsutskick, vilket är de allra flesta. Detta mail går till alla medlemmar, oavsett inställningar. Det är enda sättet vi kan garantera " +
                "att alla får kallelsen.\r\n\r\n" +
                "Så de som sett kallelsen tidigare kan betrakta detta som en påminnelse om att idag är sista motions- och nomineringsdagen. Om du inte sett kallelsen tidigare, så kallas du " +
                "med detta mail till årsmöte den 12-25 april. Årsmötet kommer att ske i här på forumet:\r\n\r\n" +
                "http://forum.piratpartiet.se/Forum617-1.aspx\r\n\r\n" +
                "För mer detaljer, se den fullständiga kallelsen, som finns här:\r\n\r\n" +
                "http://forum.piratpartiet.se/Topic199246-56-1.aspx\r\n\r\n" +
                "Väl mött på årsmötet!\r\nRick\r\n";

            Memberships memberships = Memberships.ForOrganization(Organization.PPSE);

            int total = memberships.Count;
            Console.WriteLine("Mailing to {0} members...", total);
            int count = 0;

            foreach (Membership membership in memberships)
            {
                count++;

                Console.Write("\r{0:D5}/{1:D5}...", count, total);
                if (!membership.Active)
                {
                    continue;
                }

                if (membership.OrganizationId != 1)
                {
                    continue;
                }

                membership.Person.SendNotice("Kallelse till årsmöte 12-25 april 2010", body, 1);
            }

            Console.WriteLine("done.");

        }


        public static void TestMembershipsMigration()
        {
            Console.Write("Member count PPSE, count method... ");
            Console.WriteLine(Organization.PPSE.GetMemberCount());
            Console.Write("Member count PPSE, Memberships method... ");
            Console.WriteLine(Organization.PPSE.GetMemberships().Count);
            Console.Write("Member count PPSE, direct person count... ");
            Console.WriteLine(PirateDb.GetDatabaseForReading().GetMembersForOrganizations(new int[] {1}).Length);

            Console.Write("Newsletter recipient count... ");
            Console.WriteLine(People.FromNewsletterFeed(NewsletterFeed.TypeID.ChairmanBlog).Count);

            Console.Write("Member count UPSE, count method... ");
            Console.WriteLine(Organization.FromIdentity(Organization.UPSEid).GetTree().GetMemberCount());
            Console.Write("Membership count UPSE, count method... ");
            Console.WriteLine(Organization.FromIdentity(Organization.UPSEid).GetTree().GetMembershipCount());
            Console.Write("Member count UPSE, direct person count... ");
            Console.WriteLine(PirateDb.GetDatabaseForReading().GetMembersForOrganizations(Organization.FromIdentity(Organization.UPSEid).GetTree().Identities).Length);

            Console.Write("Members expiring in next ten days...");

            DateTime date = DateTime.Today;

            for (int day = 0; day < 10; day++)
            {
                Console.Write(" " + Memberships.GetExpiring(Organization.PPSE, date).Count.ToString());
                date = date.AddDays(1);
            }

            Console.WriteLine();

        }


        /*
        public static void ScanExpenseRecordsForUnbalancedTransactions()
        {
            ExpenseClaims claims = ExpenseClaims.FromOrganization(Organization.PPSE);

            foreach (ExpenseClaim claim in claims)
            {
                if (claim.CreatedDateTime.Year < 2009)
                {
                    continue;
                }

                FinancialTransaction transaction = claim.FinancialTransaction;
                double amount = -claim.Amount;
                int expenseDebtAccountId = Organization.PPSE.FinancialAccounts.DebtsExpenseClaims.Identity;

                if (claim.Open == false && claim.Validated == false)
                {
                    amount = 0.0;
                }

                if (transaction == null)
                {
                    Console.Write("Expense #{0} has no transaction.", claim.Identity);
                    if (amount == 0.0)
                    {
                        Console.WriteLine(" Ignoring as amount is zero.");
                    }
                    else
                    {
                        Console.WriteLine (" Creating:");
                        int budgetId = claim.BudgetId;

                        string description = "Expense #" + claim.Identity + ": " + claim.Description;

                        if (description.Length > 64)
                        {
                            description = description.Substring(0, 61) + "...";
                        }

                        FinancialTransaction newTransaction = FinancialTransaction.Create(1, claim.CreatedDateTime, description);

                        newTransaction.Dependency = claim;

                        Console.WriteLine(" -- Transaction #{0}: {1}", newTransaction.Identity, description);

                        newTransaction.AddRow(budgetId, -amount, 1);
                        Console.WriteLine(" -- {0}, +{1:N2}", FinancialAccount.FromIdentity(budgetId), -amount);

                        newTransaction.AddRow(expenseDebtAccountId, amount, 1);
                        Console.WriteLine(" -- Expenses, {0:N2}", amount);
                    }

                    continue;
                }


                double transactionAmount = 0.0;

                FinancialTransactionRows rows = transaction.Rows;

                foreach (FinancialTransactionRow row in rows)
                {
                    if (row.FinancialAccountId == expenseDebtAccountId)
                    {
                        transactionAmount += row.Amount;
                    }
                }

                if (transactionAmount != amount)
                {
                    Console.WriteLine("Expense #{0}: Expected {1:N2}, but is {2:N2} (xact #{3})", claim.Identity, amount,
                                      transactionAmount, transaction.Identity);
                    Console.WriteLine(" -- expense status: {0} {1} {2} {3} {4}",
                        claim.Open? "Closed": "Open",
                        claim.Claimed? "Claimed": "Unclaimed",
                        claim.Attested? "Attested": "",
                        claim.Validated? "Validated": "",
                        claim.Repaid? "Repaid": "NOT-repaid");

                    if (transaction.Identity >= 9163 && transaction.Identity <= 9168)
                    {
                        Console.WriteLine("-- Fixing stupidity");
                        
                        // transaction.AddRow(expenseDebtAccountId, amount, 1);
                        // transaction.AddRow(expenseDebtAccountId, amount, 1);
                        // transaction.AddRow(claim.BudgetId, -amount, 1);
                        // transaction.AddRow(claim.BudgetId, -amount, 1);
                    }
                }
            }
        }*/


        public static void PrimeCryptoKeys()
        {
            Roles roles = Roles.GetAll();

            foreach (PersonRole role in roles)
            {
                if (role.OrganizationId == 1)
                {

                    Person person = role.Person;

                    if (person.MemberOf(Organization.PPSE) && person.PartyEmail.Length > 0 &&
                        person.CryptoFingerprint.Length < 4)
                    {
                        Console.Write("Generating for " + person.Canonical + "...");
                        PgpKey.Generate(person, Organization.PPSE);
                        Console.WriteLine();
                    }
                }
            }
        }


        public static void TestReporters()
        {
            string[] categoryNames = {
                                         "Riksmedia", "Lokalt/Syd/Skåne", "Ämne/Medborgarrätt", "Ämne/Teknik",
                                         "Ämne/Politik", "Bloggare"
                                     };

            MediaCategories categories = new MediaCategories();

            foreach (string categoryName in categoryNames)
            {
                MediaCategory mediaCategory = MediaCategory.FromName(categoryName);
                categories.Add(mediaCategory);
            }

            Reporters reporters = Reporters.FromMediaCategories(categories);

            foreach (Reporter reporter in reporters)
            {
                Console.WriteLine(reporter.Name + " -- " + reporter.Email);
            }
        }


        public static void ReportIntermediatePrimaryResult()
        {
            SchulzeProcessor processor = new SchulzeProcessor();

            processor.Candidates = MeetingElection.Primaries2010.Candidates;
            processor.Votes = MeetingElectionVotes.ForInternalPoll(MeetingElection.Primaries2010);

            processor.Process();

            MailRawElectionResults(processor);

            BallotComposer composer = new BallotComposer();
            composer.BallotGeographies = Geographies.FromIdentities(new int[] { 32, 940, 941, 34, 35, 39 });
            composer.RawElectionResult = processor.FinalOrder;

            CreateMasterListLookup(processor.FinalOrder);

            GenerateElectionResults(composer, 0.3, BallotCompositionMethod.InterleavedTenNational);
       }


        public static void TestSchulzeMethod()
        {
            Console.Write("Initializing and loading candidates, votes...");

            SchulzeProcessor processor = new SchulzeProcessor();

            processor.Candidates = MeetingElection.Primaries2010Simulation.Candidates;
            processor.Votes = MeetingElectionVotes.ForInternalPoll(MeetingElection.Primaries2010Simulation);
            Console.WriteLine(" done.");

            processor.Process();

            Console.Write("Composing ballots and mailing results...");


            BallotComposer composer = new BallotComposer();
            composer.DefectedPersonIds = new int[] { 9951, 26576, 42694, 33860, 21204 };
            composer.BallotGeographies = Geographies.FromIdentities(new int[] { 32, 33, 34, 35, 39 });
            composer.RawElectionResult = processor.FinalOrder;

            CreateMasterListLookup(processor.FinalOrder);

            GenerateElectionResults(composer, 0.3, BallotCompositionMethod.InterleavedTenNational);


            Console.WriteLine(" done.");
        }

        private static void GenerateElectionResults(BallotComposer composer, double genderQuota,
                                                    BallotCompositionMethod method)
        {
            composer.DefectedPersonIds = new int[] { 9951, 26576, 42694, 33860, 21204 };
            composer.GenderQuota = genderQuota;
            composer.ComposeBallots(method);

            MailElectionResults("Nationwide Ranking", method.ToString(), composer.GenderQuota, composer.QuotedMasterList);

            foreach (Geography geography in composer.BallotGeographies)
            {
                MailElectionResults(geography.Name, method.ToString(), composer.GenderQuota, composer.FinalBallots[geography.Identity]);
            }

            // SMS to candidates who didn't make it

            Dictionary<int, bool> listLookup = new Dictionary<int, bool>();

            foreach (MeetingElectionCandidates list in composer.FinalBallots.Values)
            {
                foreach (MeetingElectionCandidate candidate in list)
                {
                    listLookup[candidate.PersonId] = true;
                }
            }

            Console.WriteLine("SMSing candidates:\r\n");

            foreach (MeetingElectionCandidate candidate in composer.QuotedMasterList)
            {
                Console.Write("{0}... ", candidate.Person.Canonical);

                if (listLookup.ContainsKey(candidate.PersonId))
                {
                    Console.WriteLine("on list, not texting");
                }
                else
                {
                    try
                    {
                        // candidate.Person.SendPhoneMessage("Primärvalsresultatet är nu klart. Tyvärr kom du inte med på Piratpartiets valsedlar, men vi hoppas att du kommer att vara med i valkampanjen!");
                        Console.WriteLine("sent to " + candidate.Person.Phone);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("FAILED sending to " + candidate.Person.Phone);
                    }
                }
            }
        }

        private static void CreateMasterListLookup (MeetingElectionCandidates order)
        {
            masterListPositions = new Dictionary<int, int>();

            for (int index = 0; index < order.Count; index++)
            {
                masterListPositions[order[index].InternalPollCandidateId] = index + 1; 
                // the "index +1" is a list position, not an index
            }
        }

        private static Dictionary<int, int> masterListPositions;

        private static void MailRawElectionResults (SchulzeProcessor processor)
        {
            MeetingElectionCandidates candidates = processor.FinalOrder;
            int[] dropouts = new int[] {9951, 26576, 42694, 33860, 21204};

            Dictionary<int, bool> dropoutLookup = new Dictionary<int, bool>();
            foreach (int dropoutPersonId in dropouts)
            {
                dropoutLookup[dropoutPersonId] = true;
            }

            string body = "Rank Dist Candidate\r\n";

            for (int candidateIndex = 0; candidateIndex < candidates.Count; candidateIndex++)
            {
                int distance = processor.GetCandidateDistance(candidateIndex);
                body += String.Format("{0:D3}. {1} {2}{3}\r\n", candidateIndex+1, (distance > 0? distance.ToString("D4") : "----"), candidates [candidateIndex].Person.Canonical, dropoutLookup.ContainsKey(candidates[candidateIndex].PersonId)? " [hoppat av]": string.Empty);
            }

            int[] recipients = new int[] { 1 }; //, 11443 };

            foreach (int personId in recipients)
            {
                Person.FromIdentity(personId).SendNotice(
                    String.Format("Primaries To Date [Raw Unquoted]"),
                    body, 1);
            }

        }

        private static void MailElectionResults (string geographyLabel, string methodLabel, double quota, MeetingElectionCandidates results)
        {
            string body =
                String.Format(
                    "Geography: {0}\r\n" +
                    "Method:    {1}\r\n" +
                    "Gender Q:  {2:N0}%\r\n\r\n", geographyLabel, methodLabel, quota *100);

            int position = 1;

            foreach (MeetingElectionCandidate candidate in results)
            {
                body += GetSingleResultLine(position, candidate) + "\r\n";
                position++;
            }

            int[] recipients = new int[] { 1, 11443 };

            foreach (int personId in recipients)
            {
                Person.FromIdentity(personId).SendNotice(
                    String.Format("Primaries To Date [{0}]", geographyLabel),
                    body, 1);
            }
        }

        private static string GetSingleResultLine (int position, MeetingElectionCandidate candidate)
        {
            string fakeString = string.Empty;

            Memberships memberships = candidate.Person.GetMemberships();
            if (memberships.Count == 0)
            {
                fakeString = " EJ MEDLEM LÄNGRE";
            }

            if (candidate.Person.BlogUrl.Length > 7)
            {
                return String.Format("<li><!-- {0} --><a href=\"{1}\">{2}</a>, {3}</li>",
                                     position, candidate.Person.BlogUrl, candidate.PersonName,
                                     candidate.PersonSubtitle);
            }
            else
            {
                return String.Format("<li><!-- {0} -->{2}, {3}</li>",
                                     position, candidate.Person.BlogUrl, candidate.PersonName,
                                     candidate.PersonSubtitle);
            }

            /*
            return String.Format("#{0:D2}. {1} ({2} raw #{3:D2}) {4}",
                                 position, candidate.PersonName,
                                 candidate.Person.Gender == PersonGender.Male ? "M" : "F",
                                 masterListPositions[candidate.Identity],
                                 candidate.PersonSubtitle);
            */
        }



        public static void NotifyOfficerAddresses()
        {
            // Find & identify all party still-members who hold party addresses
            /*
            Console.Write("Adding accounts... ");
            MailServerDatabase.AddAccount("press@piratpartiet.se", "P0ppencorken", 1024);
            MailServerDatabase.AddAccount("info@piratpartiet.se", "P0ppencorken", 1024);
            MailServerDatabase.AddAccount("medlemsservice@piratpartiet.se", "P0ppencorken", 1024);
            Console.WriteLine("done.");*/

            Console.Write("Loading people... ");

            Memberships memberships = Memberships.ForOrganization(Organization.PPSE);
            People officersWithMail = new People();

            Console.WriteLine("done.");

            Console.Write("Finding people with party addresses... 000");

            foreach (Membership membership in memberships)
            {
                if (membership.Person.PartyEmail.Length > 1)
                {
                    Console.Write("\b\b\b" + officersWithMail.Count.ToString("000"));
                    officersWithMail.Add(membership.Person);
                }
            }

            Console.WriteLine("\b\b\bdone ({0}).", officersWithMail.Count);

            int count = 0;

            foreach (Person person in officersWithMail)
            {
                count++;
                Console.Write("\rCreating accounts... {0}", count.ToString("000"));

                // MailServerDatabase.AddAccount(person.PartyEmail, PirateWeb.Logic.Security.Authentication.CreateRandomPassword(20), 1024);
                /*
                PirateWeb.Utility.Mail.MailTransmitter mail = new PirateWeb.Utility.Mail.MailTransmitter
                    ("PirateWeb-L", "noreply@pirateweb.net", "Testmail - du kan ta emot mail",
                    "Eftersom du ser det här mailet, så kan du ta emot mail till din pp-adress. Glad påsk, eller nåt.\r\n\r\n",
                    person, true);
                mail.Send();*/

                if (person.Email.EndsWith("piratpartiet.se") || person.Identity == 1)
                {
                    Console.WriteLine("\rFAULT: {0} (#{1})has mail address {2}", person.Name, person.Identity,
                                      person.Email);
                    Console.Write("Notifiying by SMS... ");
                    try
                    {
                        //person.SendPhoneMessage("PP: Vår mailserver har kraschat. Du har ingen privat mailadress i PirateWeb. Utan privat mailadress kan vi inte skicka nya inloggningsuppgifter.");
                        //person.SendPhoneMessage("PP: Kan du gå in i PirateWeb och byta din privata mailadress till en som inte ligger under piratpartiet.se, så att du kan få nytt login/lösen?");
                        Console.WriteLine("done.\r\n");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("failed!\r\n");
                    }
                }
                else if (person.Email.Length < 3)
                {
                    Console.WriteLine("\rFAULT: {0} (#{1})has no mail address");
                }
                else
                {
                    string password = Logic.Security.Authentication.CreateRandomPassword(16);
                    //MailServerDatabase.SetNewPassword(person.PartyEmail, password);

                    MailTransmitter mail = new MailTransmitter
                        ("Rick Falkvinge (Piratpartiet)", "rick@piratpartiet.se",
                         "TREDJE lösen till din mail - jag klantade mig",
                         "Ok, jag är extremt klantig. Sorry. När det förra brevet gick ut nyss, så glömde jag ta bort koden som satte ett nytt lösen på allas mailkonton. Lösenordet byttes alltså alldeles nyss utan att du fick reda på det.\r\n\r\n" +
                         "Det är mitt fel, jag är superklantig och har broccoli i strumporna, etc.\r\n\r\n" +
                         "Ditt nya (tredje) lösen till mailen är: " + password + "\r\n\r\n" +
                         "Sorry för strul. /Rick\r\n",
                         /*
                        "Här är dina nya inloggningsuppgifter IGEN till din mail på Piratpartiet (din adress " + person.PartyEmail + "). Du fick ett liknande mail nyss. Uppgifterna där fungerade inte. Sorry.\r\n\r\n " +
                        "Ny SERVER: mail.piratpartiet.se.\r\n" +
                        "Ditt LOGIN: " + person.PartyEmail + " (samma som mailadressen).\r\n" +
                        "Ditt LÖSEN: " + password + "\r\n\r\n" +
                        "Det går ännu inte att SKICKA mail till adresser utanför piratpartiet.se, och webmail ska upp och fungera också. " +
                        "Nya mail kommer allteftersom de funktionerna kommer upp, men nu går det att ställa in sin mailklient på att " +
                        "hämta posten som kommit.\r\n\r\n" +
                        "Om du sparade mail på den gamla servern och vill ha den mailen extraherad och skickad till dig, " +
                        "så går det att göra. Svara på det mail som kommer om någon dag med nya inloggningsuppgifter. Om det är " +
                        "bråttom, så skicka ett SMS till mig (Rick) på 0708-303600, så extraherar jag. Ring inte, " +
                        "åtminstone inte om det, idag är det fortsatt tokigt mycket med Ipred.\r\n\r\n",*/

                         person);
                    mail.Send();
                }
            }
        }


        public static void FindInvalidUPMembers()
        {
            Console.Write("Loading memberships...");

            Organizations orgs = Organization.FromIdentity(2).GetTree();
            Memberships memberships = Memberships.ForOrganizations(orgs);

            using (StreamWriter writer = new StreamWriter("invalid-up-members.txt", false, Encoding.Default))
            {
                int count = 0;

                foreach (Membership membership in memberships)
                {
                    Console.Write("\rSearching members... {0:D04}", ++count);

                    bool valid = true;
                    string reason = string.Empty;

                    Person person = membership.Person;

                    if (person.Name.Length < 3 || person.Name.IndexOf(' ') == -1)
                    {
                        valid = false;
                        reason += " namn";
                    }

                    if (person.Email.Length < 5 || person.Email.IndexOf('@') == -1)
                    {
                        valid = false;
                        reason += " mail";
                    }

                    if (person.Phone.Length < 9)
                    {
                        valid = false;
                        reason += " telefon";
                    }

                    if (!valid)
                    {
                        string output = string.Format("#{0}, {1}, {2} {3}:{4}", person.Identity, person.Name,
                                                      person.Email, person.Phone, reason);
                        Console.WriteLine(output);
                        writer.WriteLine(output);
                    }
                }
            }
        }

        public static void InviteMembersToActivists()
        {
            Memberships memberships = Organization.PPSE.GetMemberships();

            int count = 0;

            Console.WriteLine("Inviting members to activismship...");

            foreach (Membership membership in memberships)
            {
                Console.Write("\rInvited {0:D04}/{1:D04}", ++count, memberships.Count);
            
                if (membership.PersonId != 1)
                {
                    // continue;
                }

                if (membership.Person.Email.Length < 3)
                {
                    continue;
                }

                if (membership.Person.NeverMail)
                {
                    continue;
                }

                if (membership.Person.IsActivist)
                {
                    continue;
                }

                if (membership.MemberSince > new DateTime(2009,4,15))
                {
                    continue;
                }

                if (membership.MemberSince < new DateTime(2009,3,1))
                {
                    continue;
                }

                string mailBody = "Bli pirataktivist!\r\n\r\n" +
                                  "Inför valet kraftsamlar vi ordentligt och införde nyligen en ny roll -- AKTIVIST. För dig som redan är medlem är det " +
                                  "en markering att du är, eller kan vara, intresserad av de aktiviteter som pågår på din ort för att " +
                                  "hjälpa Piratpartiet att lyckas i valen 2009 och 2010.\r\n\r\n" +
                                  "För andra innebär det ett sätt att erbjuda sig att hjälpa Piratpartiet utan att bli medlem och lämna ut sina " +
                                  "personuppgifter. Det sänker alltså ribban för att aktivera sig ordentligt, är tanken, samtidigt som piratfunktionärerna " +
                                  "får möjlighet att rikta kommunikation om aktioner specifikt till dem som är intresserade av sådant.\r\n\r\n" +
                                  "De som är aktivister kommer alltså att få kommunikation av typen \"samling på plats X klockan Y för att göra Z\" till sin mobil och mail, " +
                                  "för att vi som rörelsen snabbt kunna kraftsamla och ta vara på tillfällen som öppnas. Det innebär inga som helst förpliktelser att faktiskt göra något -- " +
                                  "bara att du får reda på vilka möjligheter som finns att aktivera sig. Till exempel gick det ut SMS så inför demonstrationerna vid tingsrätten då " +
                                  "Pirate-Bay-rättegången började.\r\n\r\n" +
                                  "För de aktivister som inte är medlemmar -- alternativet \"Bli aktivist\" finns på hemsidan under \"Bli medlem\" -- så kommer " +
                                  "funktionärerna inte ens att se vilka aktivisterna är. Aktivistkommunikation kommer att fungera så att en funktionär skickar något till " +
                                  "alla aktivister i ett visst geografiskt område, bara genom att skriva in ett meddelande, markera \"till aktivisterna i området\" och " +
                                  "välja vilket område. Det går alltså till och med att vara med och " +
                                  "hjälpa piratrörelsen helt anonymt -- det enda vi då kommer att fråga efter utöver postnummer och postort är e-mail och mobilnummer, som båda kan vara anonyma. " +
                                  "Något att prata med dina vänner om, de som varit skeptiska till att bli medlem?\r\n\r\n" +
                                  "Nu är inte det här någon nedvärdering av medlemsrollen. Tvärtom. Att vara medlem skickar alltid ett starkare budskap än att vara aktivist. " +
                                  "Men vi sänker ingångströskeln för att engagera sig, och dessutom erbjuder vi med det här våra befintliga medlemmar att tala om tydligt att de vill vara " +
                                  "med och engagera sig inför valet.\r\n\r\n" +
                                  "Jag hoppas att du är en av dem som funderat på att engagera sig, men inte vetat riktigt hur eller med vad. Klicka på den här länken för att " +
                                  "bli aktiv medlem och aktivist:\r\n\r\n" +
                                  "https://pirateweb.net/Pages/Public/SE/People/MemberNewActivist.aspx?MemberId=" +
                                  membership.PersonId.ToString() + "&SecHash=" +
                                  membership.Person.PasswordHash.Replace(" ", "").Substring(0, 8) + "\r\n\r\n" +
                                  "Nu gör vi ett kanonresultat i valet 2009, och ser till att chocka etablissemanget ordentligt!\r\n\r\n" +
                                  "Hälsningar,\r\nRick\r\n\r\n";

                MailTransmitter mail =
                    new MailTransmitter("Rick Falkvinge (Piratpartiet)",
                                                               "rick.falkvinge@piratpartiet.se",
                                                               "Vill du vara aktiv medlem (\"aktivist\")?", mailBody,
                                                               membership.Person);
                mail.Send();
            }
        }


        public static void GetUPMembershipsByDate (DateTime date)
        {
            using (TextWriter writer = new StreamWriter("UP-2008-12-31.csv", false, Encoding.Default))
            {
                writer.WriteLine("Förening,Namn (medlemsnr),kön,födelsedatum,bidragsgrund,email,telefon");

                Organizations orgTree = Organization.FromIdentity(2).GetTree();

                foreach (Organization org in orgTree)
                {
                    Memberships memberships = Memberships.ForOrganization(org, true);

                    foreach (Membership membership in memberships)
                    {
                        if (membership.MemberSince < date && (membership.Active || membership.DateTerminated > date))
                        {
                            // If we get here, the membership is valid for this organization

                            int age = date.Year - membership.Person.Birthdate.Year;

                            writer.WriteLine("{0},{1} (#{2}),{3},{4} ({5} år),{6},{7},'{8}'",
                                             org.Name, membership.Person.Name, membership.Person.Identity,
                                             membership.Person.IsMale ? "Man" : "Kvinna",
                                             membership.Person.Birthdate.ToString("yyyy-MM-dd"), age,
                                             age > 6 && age < 26 ? "Ja" : "Nej", membership.Person.Email,
                                             membership.Person.Phone);
                        }
                    }
                }
            }
        }

        public static void WriteFirstVolunteersToDb()
        {
            BasicPWEvent[] events = PWEvents.ByType(EventType.NewVolunteer);

            foreach (BasicPWEvent newEvent in events)
            {
                Geography geography = Geography.FromIdentity(newEvent.GeographyId);
                string[] parts = newEvent.ParameterText.Split('|');

                string name = parts[0];
                string phone = Formatting.CleanNumber(parts[1]);
                string roles = parts[3];

                Console.WriteLine(parts[0] + ", " + geography.Name + ", " + phone + ", " + roles + ":");

                // Find: is this an existing person?

                Person person = null;

                Console.Write(" - looking up phone " + phone + "...");
                People people = People.FromPhoneNumber(1, phone);

                if (people.Count == 1)
                {
                    Console.Write(" one person");

                    if (people[0].Name.ToLower() == name.ToLower())
                    {
                        person = people[0];
                        Console.WriteLine(" and match (#{0}).", person.Identity);

                        // SEND MAIL
                        /*
                        person.SendNotice("Om din volontärroll som funktionär",
                            "Hej! Nyligen anmälde du ett intresse för att vara funktionär i Piratpartiet. " +
                            "Vi har fått massor av intresse, och det är kanonroligt så här inför EU-valet! " +
                            "Men i vanlig ordning så var vi inte beredda på att ta emot anstormningen.\r\n\r\n" +
                            "Även om det är ett kärt problem att vi har fler som vill hjälpa till än vi kunde " +
                            "hålla ordning på med de primitiva mekanismer vi hade satt upp, så förtjänar du " +
                            "naturligtvis ett svar, om du inte redan fått det. Därför vill vi tala om att " +
                            "vi precis håller på att sätta upp ett mer formellt system för att ta hand om " +
                            "alla våra kära blivande kollegor.\r\n\r\n" +
                            "Så signalen är att vi absolut inte har glömt dig, vi fick bara fler nya kollegor " +
                            "än vi var beredda på, och räknar med att ha bättre översikt inom några dagar.\r\n\r\n" +
                            "Rick\r\n", 1);*/
                    }
                    else
                    {
                        Console.WriteLine(" but no match.");
                    }
                }
                else if (people.Count == 0)
                {
                    Console.WriteLine(" no match.");
                }
                else
                {
                    Console.WriteLine(" several matches, so not trying.");
                }

                // If not, create one for this purpose

                if (person == null)
                {
                    person = Person.Create(name, string.Empty, string.Empty, phone, string.Empty, string.Empty,
                                           string.Empty, "SE", DateTime.Now, PersonGender.Unknown);
                    person.Geography = geography;
                    Console.WriteLine(" - creating new person");
                }

                Volunteer volunteer = Volunteer.Create(person, Person.FromIdentity(1)); // RF owns new volunteers

                if (roles.Contains("KL1"))
                {
                    Console.WriteLine(" - role: Lead in " + geography.Name);
                    volunteer.AddRole(1, geography.Identity, RoleType.LocalLead);
                }

                if (roles.Contains("KL2"))
                {
                    Console.WriteLine(" - role: Vice in " + geography.Name);
                    volunteer.AddRole(1, geography.Identity, RoleType.LocalDeputy);
                }

                // Move "geography" up to electoral circuit level

                while (!geography.AtLevel(GeographyLevel.ElectoralCircuit))
                {
                    geography = geography.Parent;
                }

                if (roles.Contains("VL1"))
                {
                    Console.WriteLine(" - role: Lead in " + geography.Name);
                    volunteer.AddRole(1, geography.Identity, RoleType.LocalLead);
                }

                if (roles.Contains("VL2"))
                {
                    Console.WriteLine(" - role: Vice in " + geography.Name);
                    volunteer.AddRole(1, geography.Identity, RoleType.LocalDeputy);
                }
            }
        }


        static public void PopulatePrimariesSimulation()
        {
            MeetingElection poll = MeetingElection.Primaries2010Simulation;

            Roles allRoles = Roles.GetAll();
            Dictionary<int, bool> dupeCheck = new Dictionary<int, bool>();

            int maleCount = 0;
            int femaleCount = 0;

            // Add all leads and vice leads under PPSE, and all females in an org role

            dupeCheck[2380] = true; // prevent this from adding

            foreach (PersonRole role in allRoles)
            {
                bool add = false;

                if (!dupeCheck.ContainsKey(role.PersonId))
                {
                    if (RoleTypes.ClassOfRole(role.Type) == RoleClass.Organization && role.Type != RoleType.OrganizationAdmin)
                    {
                        if (role.Person.IsFemale)
                        {
                            add = true;
                        }
                    }

                    if (role.Type == RoleType.LocalLead || role.Type == RoleType.LocalDeputy)
                    {
                        add = true;
                    }

                    if (!(role.Organization.Inherits(1) || role.OrganizationId == 1))
                    {
                        add = false;
                    }

                    if (role.Type == RoleType.LocalAdmin)
                    {
                        add = false;
                    }
                }

                if (add)
                {
                    dupeCheck[role.PersonId] = true;

                    poll.AddCandidate(role.Person, "Candidacy Statement #" + role.PersonId);
                    Console.WriteLine("Adding " + role.Person.Canonical);

                    if (role.Person.IsMale)
                    {
                        maleCount++;
                    }
                    else
                    {
                        femaleCount++;
                    }
                }
            }

            // Add (fake) females

            random = new Random();

            while (femaleCount < maleCount)
            {
                string name = GetRandomFemaleName() + " " + GetRandomLastName();
                DateTime birthDate = RandomPerson().Birthdate;

                while (birthDate.Year > 1992 | birthDate.Year < 1920)
                {
                    birthDate = RandomPerson().Birthdate;
                }

                Person geographyTemplate = RandomPerson();

                Person newPerson = Person.Create(name, string.Empty, string.Empty, string.Empty, string.Empty,
                                                 geographyTemplate.PostalCode, geographyTemplate.CityName,
                                                 geographyTemplate.Country.Code, birthDate, PersonGender.Female);

                newPerson.PostalCode = geographyTemplate.PostalCode; // force geography resolution

                poll.AddCandidate(newPerson, "Candidacy Statement #" + newPerson.Identity);
                Console.WriteLine("Adding FAKE " + newPerson.Canonical);
                PWLog.Write(PWLogItem.Person, newPerson.Identity, PWLogAction.PersonCreated, "Created for primaries simulation", "This is a fake person.");

                femaleCount++;
            }
        }

        private static Random random;

        static private Person RandomPerson()
        {
            Person result = null;

            while (result == null)
            {
                try
                {
                    result = Person.FromIdentity(random.Next(1, 30000));
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        static private Person RandomFemale()
        {
            Person female = Person.FromIdentity(1);

            while (female.IsMale)
            {
                female = RandomPerson();
            }

            return female;
        }

        static private string GetRandomFemaleName()
        {
            string[] names = RandomFemale().Name.Split(' ');

            while (names.Length != 2 || names[0].Length < 3)
            {
                names = RandomFemale().Name.Split(' ');
            }

            return Capitalize(names[0]);
        }

        static private string GetRandomLastName()
        {
            string[] names = RandomPerson().Name.Split(' ');

            while (names.Length != 2 || names[1].Length < 3)
            {
                names = RandomPerson().Name.Split(' ');
            }

            return Capitalize(names[1]);
        }

        static private string Capitalize(string input)
        {
            return Char.ToUpperInvariant(input[0]) + input.Substring(1).ToLower();
        }

        static public void PopulatePrimariesVoters()
        {
            MeetingElection poll = MeetingElection.Primaries2010;

            Memberships memberships = Memberships.ForOrganization(Organization.PPSE);

            int count = 0;
            Console.WriteLine("Adding voter list...");
            foreach (Membership membership in memberships)
            {
                poll.AddVoter(membership.Person);
                count++;

                if (count % 100 == 0)
                {
                    Console.Write("\r{0:D5}/{1:D5}", count, memberships.Count);
                }
            }

            Console.WriteLine("Done.      ");
        }


        public static void PopulateUpDelegateVotingList()
        {
            MeetingElection poll = MeetingElection.UpDelegates2010;

            Memberships memberships = Memberships.ForOrganizations(Organization.FromIdentity(Organization.UPSEid).GetTree());
            int dupeCount = 0;
            Dictionary<int,bool> dupeLookup = new Dictionary<int, bool>();

            int count = 0;
            Console.WriteLine("Adding voter list for UP DELEGATES ({0})...", poll.Identity);
            foreach (Membership membership in memberships)
            {
                int personId = membership.PersonId;

                if (!dupeLookup.ContainsKey(personId))
                {
                    dupeLookup[personId] = true;
                    poll.AddVoter(membership.Person);
                    count++;
                }
                else
                {
                    dupeCount++;    
                }


                if (count % 100 == 0)
                {
                    Console.Write("\r{0:D5}/{1:D5}", count, memberships.Count - dupeCount);
                }
            }

            poll.AddVoter(Person.FromIdentity(1));  // so that Rick can test the page - he won't cast a vote, verifiably

            Console.Write("\r{0:D5}/{1:D5}", count, memberships.Count - dupeCount);
            Console.WriteLine("Done.      ");
        }



        static public void PopulatePrimariesSimulationVoters()
        {
            MeetingElection poll = MeetingElection.Primaries2010Simulation;

            Roles allRoles = Roles.GetAll();
            Dictionary<int, bool> dupeCheck = new Dictionary<int, bool>();

            // Add all leads and vice leads under PPSE, and all females in an org role

            dupeCheck[2380] = true; // prevent this from adding

            foreach (PersonRole role in allRoles)
            {
                bool add = false;

                if (!dupeCheck.ContainsKey(role.PersonId))
                {
                    add = true;
                }

                if (add)
                {
                    dupeCheck[role.PersonId] = true;

                    poll.AddVoter(role.Person);
                    Console.WriteLine(role.Person);
                }
            }
        }


        static public void RestoreBlogs()
        {
            Console.WriteLine("Opening file");
            using (TextReader reader = new StreamReader("/home/rick/staging/optionaldata-restorebase.txt", true))
            {
                string line = reader.ReadLine();
                int lineCount = 1;

                while (!String.IsNullOrEmpty(line))
                {
                    Console.Write("\r{0:D4}", lineCount++);

                    if (line.StartsWith("INSERT"))
                    {
                        line = reader.ReadLine();
                        continue;
                    }

                    string[] parts = line.Split(',');
                    int personId = 0;
                    int dataTypeId = 0;

                    try
                    {
                        personId = Int32.Parse(parts[1]);
                        dataTypeId = Int32.Parse(parts[2]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("DEBUG: {0}", line);
                        Console.WriteLine(" - [{0}] [{1}]", parts[1], parts[2]);
                        line = reader.ReadLine();
                        continue;
                    }

                    string data = parts[3].Substring(1);

                    int lastIndexOfSingleQuote = data.LastIndexOf('\'');

                    if (lastIndexOfSingleQuote == -1)
                    {
                        if (parts.Length > 4)
                        {
                            data += "," + parts[4];
                            lastIndexOfSingleQuote = data.LastIndexOf('\'');
                        }

                        if (lastIndexOfSingleQuote == -1)
                        {
                            Console.WriteLine("DEBUG: {0}", line);
                            Console.WriteLine(" - [{0}]", parts[3]);
                            line = reader.ReadLine();
                            continue;
                        }
                    }
                    data = data.Substring(0, lastIndexOfSingleQuote).Replace("\\'", "'");


                    if (dataTypeId == 18)
                    {
                        Person person = Person.FromIdentity(personId);

                        string oldData = person.BlogName;

                        if (oldData != data && oldData.Length < 3)
                        {
                            Console.WriteLine(person.Canonical + " BlogName: '{0}'\r\n - restoring to '{1}'", oldData,
                                              data);
                            person.BlogName = data;
                        }
                    }
                    if (dataTypeId == 19)
                    {
                        Person person = Person.FromIdentity(personId);

                        string oldData = person.BlogUrl;

                        if (oldData != data && oldData.Length < 3)
                        {
                            Console.WriteLine(person.Canonical + " BlogUrl: '{0}'\r\n - restoring to '{1}'", oldData,
                                              data);
                            person.BlogUrl = data;
                        }
                    }

                    line = reader.ReadLine();
                }
            }
        }
    }


    /*
    public class ExpenseClaimDebugger
    {
        // We need to track all expense claims, when they were opened and closed, and look at the balance of the expense debt at that point.

        public ExpenseClaimDebugger()
        {
            expenseOpenLookup = new Dictionary<int, bool>();
            expenseClosedDateLookup = new Dictionary<int, DateTime>();
            expenseClaimEvents = new List<ExpenseClaimEvent>();
            output = string.Empty;
        }

        public void Run()
        {
            LoadExpenseClaimEvents();
            AddHacks();
            SortEvents();
            PrintEvents();
            MailOutput();
        }


        private void LoadExpenseClaimEvents()
        {

            Console.Write("Loading events...");

            BasicPWEvent[] expenseCreateEvents = PWEvents.ByType(EventType.ExpenseCreated);
            BasicPWEvent[] oldCloseEvents = PWEvents.ByType(EventType.ExpensesRepaidClosed);
            BasicPWEvent[] newCloseEvents = PWEvents.ByType(EventType.PayoutCreated);

            foreach (BasicPWEvent foo in expenseCreateEvents)
            {
                if (foo.OrganizationId != 1)
                {
                    continue;
                }

                int expenseClaimId = foo.ParameterInt;
                ExpenseClaim claim = ExpenseClaim.FromIdentity(expenseClaimId);

                ExpenseClaimEvent bar = new ExpenseClaimEvent();
                bar.ExpenseClaimIds = new int[] { claim.Identity };
                bar.Amount = claim.Amount;
                bar.WasOpened = true;
                bar.EventDateTime = foo.DateTime;

                expenseClaimEvents.Add(bar);
            }

            foreach (BasicPWEvent foo in oldCloseEvents)
            {
                if (foo.OrganizationId != 1)
                {
                    continue;
                }

                ExpenseClaims claims = new ExpenseClaims();

                string[] expenseClaimIdStrings = foo.ParameterText.Split(',');
                double amount = 0.0;

                foreach (string expenseClaimIdString in expenseClaimIdStrings)
                {
                    ExpenseClaim claim = ExpenseClaim.FromIdentity(Int32.Parse(expenseClaimIdString));
                    claims.Add(claim);
                    amount += claim.Amount;
                }

                ExpenseClaimEvent bar = new ExpenseClaimEvent();
                bar.Amount = amount;
                bar.ExpenseClaimIds = claims.Identities;
                bar.EventDateTime = foo.DateTime;
                bar.WasOpened = false;

                expenseClaimEvents.Add(bar);
            }

            foreach (BasicPWEvent foo in newCloseEvents)
            {
                if (foo.OrganizationId != 1)
                {
                    continue;
                }

                ExpenseClaims claims = new ExpenseClaims();

                string[] expenseClaimIdStrings = foo.ParameterText.Split('|');
                double amount = 0.0;

                foreach (string expenseClaimIdString in expenseClaimIdStrings)
                {
                    if (expenseClaimIdString[0] == 'C')
                    {
                        ExpenseClaim claim = ExpenseClaim.FromIdentity(Int32.Parse(expenseClaimIdString.Substring(1)));
                        claims.Add(claim);
                        amount += claim.Amount;
                    }
                }

                ExpenseClaimEvent bar = new ExpenseClaimEvent();
                bar.Amount = amount;
                bar.ExpenseClaimIds = claims.Identities;
                bar.EventDateTime = foo.DateTime;
                bar.WasOpened = false;

                expenseClaimEvents.Add(bar);
            }

            Console.WriteLine(" loaded.");
        }

        private void AddHacks()
        {
            AddOpenHack(new DateTime(2007, 3, 13), 425.0, 63);
            AddCloseHack(new DateTime(2007, 6, 30), new int[] { 31, 50, 52, 53, 54 });
        }

        private void AddOpenHack(DateTime dateTime, double amount, int expenseId)
        {
            ExpenseClaimEvent bar = new ExpenseClaimEvent();
            bar.Amount = amount;
            bar.ExpenseClaimIds = new int[1] { expenseId };
            bar.EventDateTime = dateTime;
            bar.WasOpened = true;

            expenseClaimEvents.Add(bar);
        }

        private void AddCloseHack(DateTime dateTime, int[] expenseIds)
        {
            double amount = 0.0;

            foreach (int expenseId in expenseIds)
            {
                amount += ExpenseClaim.FromIdentity(expenseId).Amount;
            }

            ExpenseClaimEvent bar = new ExpenseClaimEvent();
            bar.Amount = amount;
            bar.ExpenseClaimIds = expenseIds;
            bar.EventDateTime = dateTime;
            bar.WasOpened = false;

            expenseClaimEvents.Add(bar);
        }

        private void SortEvents()
        {
            Console.Write("Sorting...");
            expenseClaimEvents.Sort(DateTimeComparer);
            Console.WriteLine("done.\r\n");
        }

        private void PrintEvents()
        {
            // DateTime    Event                      Amount  Expected bal   Actual bal  Currently open
            // ==========  ===================    ==========  ============  ===========  =============================
            // 2001-02-21  Open #1                 14.341,10    141.415,23   123.456,78  #41,15,135,153-51
            // 2002-03-31  Close #45-46, 49

            OutputLine("DateTime    Event                      Amount  Expected bal    Actual bal  Currently open");
            OutputLine("==========  ===================    ==========  ============  ============  =============================");

            double currentAmount = 0.0;

            foreach (ExpenseClaimEvent claimEvent in expenseClaimEvents)
            {
                string eventDescription = string.Empty;

                if (claimEvent.WasOpened)
                {
                    eventDescription = "Opened #" + claimEvent.ExpenseClaimIds[0].ToString();
                    currentAmount += claimEvent.Amount;
                    expenseOpenLookup[claimEvent.ExpenseClaimIds[0]] = true;
                }
                else
                {
                    eventDescription = "Closed " + Formatting.GenerateRangeString(claimEvent.ExpenseClaimIds);
                    currentAmount -= claimEvent.Amount;
                    foreach (int claimId in claimEvent.ExpenseClaimIds)
                    {
                        expenseOpenLookup[claimId] = false;
                    }
                }

                List<int> currentlyOpenList = new List<int>();
                foreach (int testId in expenseOpenLookup.Keys)
                {
                    if (expenseOpenLookup[testId])
                    {
                        currentlyOpenList.Add(testId);
                    }
                }
                string currentlyOpenString = Formatting.GenerateRangeString(currentlyOpenList);

                double actualBalance = FinancialAccount.FromIdentity(3).GetDelta(new DateTime(2006, 1, 1),
                                                                                 claimEvent.EventDateTime.Date.AddDays(1));

                OutputLine(String.Format("{0:yyyy-MM-dd}  {1,-21}  {2,10:N2}  {3,12:N2}  {4,12:N2}  {5}", claimEvent.EventDateTime,
                                         eventDescription, claimEvent.Amount, currentAmount, actualBalance, currentlyOpenString));
            }
        }

        private void OutputLine(string line)
        {
            output += line + "\r\n";
            Console.WriteLine(line);
        }

        private void MailOutput()
        {
            Person.FromIdentity(1).SendNotice("Expenses debugging", output, 1);
        }

        public static int DateTimeComparer(object foo1, object foo2)
        {
            ExpenseClaimEvent event1 = (ExpenseClaimEvent)foo1;
            ExpenseClaimEvent event2 = (ExpenseClaimEvent)foo2;

            return DateTime.Compare(event1.EventDateTime, event2.EventDateTime);
        }

        private Dictionary<int, bool> expenseOpenLookup;
        private Dictionary<int, DateTime> expenseClosedDateLookup;
        private List<ExpenseClaimEvent> expenseClaimEvents;
        private string output;


        private class ExpenseClaimEvent
        {
            public DateTime EventDateTime;
            public bool WasOpened;
            public int[] ExpenseClaimIds;
            public double Amount;
        }
    }
    */

}