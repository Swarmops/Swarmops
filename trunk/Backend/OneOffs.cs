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
                        person.SendPhoneMessage("PP: Hej! Gl�m inte att r�sta p� Piratpartiet idag, och ta g�rna med dig en kompis eller tv�. Vallokalerna st�nger kl 20.");
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
                                          "Du har tyv�rr f�tt en faktura fr�n Piratpartiet som hade ett ogiltigt OCR-nummer (" +
                                          invoice.Reference +
                                          "). Det fungerar inte. Anv�nd f�ljande OCR-nummer i st�llet:\r\n\r\n" +
                                          newReference + "\r\n\r\n" +
                                          "F�r att se en bild p� fakturan med nya OCR-numret, klicka p� denna l�nk:\r\n" +
                                          "http://data.piratpartiet.se/Forms/DisplayOutboundInvoice.aspx?Reference=" +
                                          newReference + "&Culture=svse\r\n";


                        MailMessage newMessage = new MailMessage("accounting@piratpartiet.se",
                                                                 invoice.InvoiceAddressMail,
                                                                 "Faktura fr�n Piratpartiet -- nytt OCR-nummer",
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

                "Nu �r det tv� m�nader sedan ditt medlemskap i Piratpartiet gick ut, och vi vill g�rna att du f�rnyar ditt medlemskap. Det �r naturligtvis gratis. Det g�r du genom att klicka p� den h�r l�nken:\r\n\r\n" +

                stdLink + "\r\n\r\n" +

                "Det som h�nt de senaste m�naderna �r alarmerande och starka sk�l att vara en del av den r�relse som vill bygga ett nytt och annorlunda samh�lle. Ett samh�lle d�r grundl�ggande r�ttigheter respekteras. H�r �r ett par exempel p� vad som ist�llet har h�nt nyligen:\r\n\r\n" +

                "- I EU s� drevs det alldeles nyligen igenom att alla v�ra s�kningar p� n�tet ska registreras och lagras i upp till tv� �r f�r att kunna anv�ndas mot oss.\r\n\r\n" +

                "- Samma EU-personer, fr�n Italien, arbetar aktivt f�r att eliminera all anonymitet p� n�tet -- att ingen ska kunna anv�nda Internet utan att identifiera sig f�rst. Men yttrandefrihet och �siktsfrihet kr�ver anonymitet, s� ledande politiker �r ute efter att eliminera riktigt grundl�ggande r�ttigheter.\r\n\r\n" +

                "- H�r i Sverige f�rs�kte Journalistf�rbundet nyligen olagligf�rklara l�nkning till nyhetsartiklar (och d�rigenom l�nkning som koncept).\r\n\r\n" +

                "- Samtliga riksdagspartier t�nker inf�ra datalagring i Sverige, som loggar alla kontakter vi tar med v�ra medm�nniskor i det uttryckliga syftet att kunna anv�nda det mot oss. Plus att logga hur v�ra mobiltelefoner r�r sig genom staden. (Vissa politiker f�rs�ker blanda bort korten med att prata om \"vad de vill\". G� inte p� det. Fr�ga efter \"hur de t�nker r�sta\". Det visar sig att det inte �r samma sak.)\r\n\r\n" +

                "- En sak som inte m�nga k�nner till �r att polisen redan idag f�r s�tta upp dolda kameror och mikrofoner i ditt hem, utan att du �r misst�nkt f�r n�got brott.\r\n\r\n" +

                "Men samtidigt har Piratpartiet gjort skillnad nere i EU-parlamentet, p� punkt efter punkt. Vi var drivande i att �ppna upp Acta-f�rhandlingarna, vi var drivande i att tvinga fram r�ttss�kerhet i Telekompaketet, vi var drivande i att stoppa Swift-avtalet som skulle givit USA tillg�ng till alla v�ra privata bankdata. Och mycket mer, hela tiden. D�rf�r beh�vs vi i svenska riksdagen ocks�.\r\n\r\n" +
                
                "Vi g�r in i en intensiv valr�relse nu -- valet �r om 56 dagar. Det h�r �r slutskedet. Om vi ska lyckas sl� vakt om grundl�ggande fundamenta f�r det �ppna samh�llet, s� �r det i det h�r valet som det m�ste ske.\r\n\r\n" +

                "D�rf�r ser vi g�rna att du �terv�nder till Piratpartiet som medlem. Vi sparar bara medlemsdata i tre m�nader efter medlemskapet har g�tt ut, s� det h�r kan vara sista p�minnelsen du f�r.\r\n\r\n" +

                "Medlemskapet �r, som alltid, gratis. G� med igen genom att klicka p� den h�r l�nken:\r\n\r\n" +

                stdLink + "\r\n\r\n" +

                "H�lsningar,\r\n" +
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
                FinancialTransaction resultTransaction = FinancialTransaction.Create(1, new DateTime(year, 12, 31, 23, 59, 00), "�rets resultat " + year.ToString());
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
                FinancialTransaction resultTransaction = FinancialTransaction.Create(1, new DateTime (2010,12,31,23,59,00), "�rets resultat 2010");
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
                "Piratpartiet kallar till �rsm�te.\r\n\r\n" +
                "Kallelsen har g�tt ut tidigare till medlemmar som prenumererar p� nyhetsutskick, vilket �r de allra flesta. Detta mail g�r till alla medlemmar, oavsett inst�llningar. Det �r enda s�ttet vi kan garantera " +
                "att alla f�r kallelsen.\r\n\r\n" +
                "S� de som sett kallelsen tidigare kan betrakta detta som en p�minnelse om att idag �r sista motions- och nomineringsdagen. Om du inte sett kallelsen tidigare, s� kallas du " +
                "med detta mail till �rsm�te den 12-25 april. �rsm�tet kommer att ske i h�r p� forumet:\r\n\r\n" +
                "http://forum.piratpartiet.se/Forum617-1.aspx\r\n\r\n" +
                "F�r mer detaljer, se den fullst�ndiga kallelsen, som finns h�r:\r\n\r\n" +
                "http://forum.piratpartiet.se/Topic199246-56-1.aspx\r\n\r\n" +
                "V�l m�tt p� �rsm�tet!\r\nRick\r\n";

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

                membership.Person.SendNotice("Kallelse till �rsm�te 12-25 april 2010", body, 1);
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
                                         "Riksmedia", "Lokalt/Syd/Sk�ne", "�mne/Medborgarr�tt", "�mne/Teknik",
                                         "�mne/Politik", "Bloggare"
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
                        // candidate.Person.SendPhoneMessage("Prim�rvalsresultatet �r nu klart. Tyv�rr kom du inte med p� Piratpartiets valsedlar, men vi hoppas att du kommer att vara med i valkampanjen!");
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
                fakeString = " EJ MEDLEM L�NGRE";
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
                    "Eftersom du ser det h�r mailet, s� kan du ta emot mail till din pp-adress. Glad p�sk, eller n�t.\r\n\r\n",
                    person, true);
                mail.Send();*/

                if (person.Email.EndsWith("piratpartiet.se") || person.Identity == 1)
                {
                    Console.WriteLine("\rFAULT: {0} (#{1})has mail address {2}", person.Name, person.Identity,
                                      person.Email);
                    Console.Write("Notifiying by SMS... ");
                    try
                    {
                        //person.SendPhoneMessage("PP: V�r mailserver har kraschat. Du har ingen privat mailadress i PirateWeb. Utan privat mailadress kan vi inte skicka nya inloggningsuppgifter.");
                        //person.SendPhoneMessage("PP: Kan du g� in i PirateWeb och byta din privata mailadress till en som inte ligger under piratpartiet.se, s� att du kan f� nytt login/l�sen?");
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
                         "TREDJE l�sen till din mail - jag klantade mig",
                         "Ok, jag �r extremt klantig. Sorry. N�r det f�rra brevet gick ut nyss, s� gl�mde jag ta bort koden som satte ett nytt l�sen p� allas mailkonton. L�senordet byttes allts� alldeles nyss utan att du fick reda p� det.\r\n\r\n" +
                         "Det �r mitt fel, jag �r superklantig och har broccoli i strumporna, etc.\r\n\r\n" +
                         "Ditt nya (tredje) l�sen till mailen �r: " + password + "\r\n\r\n" +
                         "Sorry f�r strul. /Rick\r\n",
                         /*
                        "H�r �r dina nya inloggningsuppgifter IGEN till din mail p� Piratpartiet (din adress " + person.PartyEmail + "). Du fick ett liknande mail nyss. Uppgifterna d�r fungerade inte. Sorry.\r\n\r\n " +
                        "Ny SERVER: mail.piratpartiet.se.\r\n" +
                        "Ditt LOGIN: " + person.PartyEmail + " (samma som mailadressen).\r\n" +
                        "Ditt L�SEN: " + password + "\r\n\r\n" +
                        "Det g�r �nnu inte att SKICKA mail till adresser utanf�r piratpartiet.se, och webmail ska upp och fungera ocks�. " +
                        "Nya mail kommer allteftersom de funktionerna kommer upp, men nu g�r det att st�lla in sin mailklient p� att " +
                        "h�mta posten som kommit.\r\n\r\n" +
                        "Om du sparade mail p� den gamla servern och vill ha den mailen extraherad och skickad till dig, " +
                        "s� g�r det att g�ra. Svara p� det mail som kommer om n�gon dag med nya inloggningsuppgifter. Om det �r " +
                        "br�ttom, s� skicka ett SMS till mig (Rick) p� 0708-303600, s� extraherar jag. Ring inte, " +
                        "�tminstone inte om det, idag �r det fortsatt tokigt mycket med Ipred.\r\n\r\n",*/

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
                                  "Inf�r valet kraftsamlar vi ordentligt och inf�rde nyligen en ny roll -- AKTIVIST. F�r dig som redan �r medlem �r det " +
                                  "en markering att du �r, eller kan vara, intresserad av de aktiviteter som p�g�r p� din ort f�r att " +
                                  "hj�lpa Piratpartiet att lyckas i valen 2009 och 2010.\r\n\r\n" +
                                  "F�r andra inneb�r det ett s�tt att erbjuda sig att hj�lpa Piratpartiet utan att bli medlem och l�mna ut sina " +
                                  "personuppgifter. Det s�nker allts� ribban f�r att aktivera sig ordentligt, �r tanken, samtidigt som piratfunktion�rerna " +
                                  "f�r m�jlighet att rikta kommunikation om aktioner specifikt till dem som �r intresserade av s�dant.\r\n\r\n" +
                                  "De som �r aktivister kommer allts� att f� kommunikation av typen \"samling p� plats X klockan Y f�r att g�ra Z\" till sin mobil och mail, " +
                                  "f�r att vi som r�relsen snabbt kunna kraftsamla och ta vara p� tillf�llen som �ppnas. Det inneb�r inga som helst f�rpliktelser att faktiskt g�ra n�got -- " +
                                  "bara att du f�r reda p� vilka m�jligheter som finns att aktivera sig. Till exempel gick det ut SMS s� inf�r demonstrationerna vid tingsr�tten d� " +
                                  "Pirate-Bay-r�tteg�ngen b�rjade.\r\n\r\n" +
                                  "F�r de aktivister som inte �r medlemmar -- alternativet \"Bli aktivist\" finns p� hemsidan under \"Bli medlem\" -- s� kommer " +
                                  "funktion�rerna inte ens att se vilka aktivisterna �r. Aktivistkommunikation kommer att fungera s� att en funktion�r skickar n�got till " +
                                  "alla aktivister i ett visst geografiskt omr�de, bara genom att skriva in ett meddelande, markera \"till aktivisterna i omr�det\" och " +
                                  "v�lja vilket omr�de. Det g�r allts� till och med att vara med och " +
                                  "hj�lpa piratr�relsen helt anonymt -- det enda vi d� kommer att fr�ga efter ut�ver postnummer och postort �r e-mail och mobilnummer, som b�da kan vara anonyma. " +
                                  "N�got att prata med dina v�nner om, de som varit skeptiska till att bli medlem?\r\n\r\n" +
                                  "Nu �r inte det h�r n�gon nedv�rdering av medlemsrollen. Tv�rtom. Att vara medlem skickar alltid ett starkare budskap �n att vara aktivist. " +
                                  "Men vi s�nker ing�ngstr�skeln f�r att engagera sig, och dessutom erbjuder vi med det h�r v�ra befintliga medlemmar att tala om tydligt att de vill vara " +
                                  "med och engagera sig inf�r valet.\r\n\r\n" +
                                  "Jag hoppas att du �r en av dem som funderat p� att engagera sig, men inte vetat riktigt hur eller med vad. Klicka p� den h�r l�nken f�r att " +
                                  "bli aktiv medlem och aktivist:\r\n\r\n" +
                                  "https://pirateweb.net/Pages/Public/SE/People/MemberNewActivist.aspx?MemberId=" +
                                  membership.PersonId.ToString() + "&SecHash=" +
                                  membership.Person.PasswordHash.Replace(" ", "").Substring(0, 8) + "\r\n\r\n" +
                                  "Nu g�r vi ett kanonresultat i valet 2009, och ser till att chocka etablissemanget ordentligt!\r\n\r\n" +
                                  "H�lsningar,\r\nRick\r\n\r\n";

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
                writer.WriteLine("F�rening,Namn (medlemsnr),k�n,f�delsedatum,bidragsgrund,email,telefon");

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

                            writer.WriteLine("{0},{1} (#{2}),{3},{4} ({5} �r),{6},{7},'{8}'",
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
                        person.SendNotice("Om din volont�rroll som funktion�r",
                            "Hej! Nyligen anm�lde du ett intresse f�r att vara funktion�r i Piratpartiet. " +
                            "Vi har f�tt massor av intresse, och det �r kanonroligt s� h�r inf�r EU-valet! " +
                            "Men i vanlig ordning s� var vi inte beredda p� att ta emot anstormningen.\r\n\r\n" +
                            "�ven om det �r ett k�rt problem att vi har fler som vill hj�lpa till �n vi kunde " +
                            "h�lla ordning p� med de primitiva mekanismer vi hade satt upp, s� f�rtj�nar du " +
                            "naturligtvis ett svar, om du inte redan f�tt det. D�rf�r vill vi tala om att " +
                            "vi precis h�ller p� att s�tta upp ett mer formellt system f�r att ta hand om " +
                            "alla v�ra k�ra blivande kollegor.\r\n\r\n" +
                            "S� signalen �r att vi absolut inte har gl�mt dig, vi fick bara fler nya kollegor " +
                            "�n vi var beredda p�, och r�knar med att ha b�ttre �versikt inom n�gra dagar.\r\n\r\n" +
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