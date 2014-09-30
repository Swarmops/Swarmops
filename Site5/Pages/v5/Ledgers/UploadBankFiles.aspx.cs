using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Swarm;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;


// TODO: There are significant remnants in this file from the old Telerik-based upload code. When Account Automation is complete and the user can select a bank profile
// for an account, uploading payment files as well as bank statements, remove that dead code as its templates won't be needed after that.


// ReSharper disable CheckNamespace
namespace Swarmops.Site.Pages.Ledgers
// ReSharper restore CheckNamespace
{
    public partial class UploadBankFiles : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = Resources.Pages.Ledgers.UploadBankFiles_PageTitle;
            this.PageIcon = "iconshock-bank";
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

            if (!IsPostBack)
            {
                // Localize

                this.InfoBoxLiteral = Resources.Pages.Ledgers.UploadBankFiles_Info;
                this.LabelBankAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_BankAccount;
                this.LabelFileType.Text = Resources.Pages.Ledgers.UploadBankFiles_FileType;
                this.LabelInstructions.Text = Resources.Pages.Ledgers.UploadBankFiles_Instructions;
                this.LabelProcessing.Text = Resources.Pages.Ledgers.UploadBankFiles_Processing;
                this.LabelProcessingComplete.Text = Resources.Pages.Ledgers.UploadBankFiles_ProcessingComplete;
                this.LabelUploadBankFile.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadBankFile;
                this.LabelUploadMore.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadAnother;
                this.LabelUploadBankFileHeader.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadBankFile;

                // Populate the asset account dropdown, if needed for file type

                PopulateAccountDropDown();
            }

            if (!IsPostBack)
            {
                // there used to be some Telerik junk here
            }
        }

        /*
        protected void ButtonSebAccountFile_Click(object sender, ImageClickEventArgs e)
        {
            OnSelectedFileType();

            this.HiddenFileType.Value = "Seb";

            this.ButtonSebFile.CssClass = "FileTypeImage FileTypeImageSelected";

            this.ImageDownloadInstructions.ImageUrl = "~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-small.png";

            this.ImageDownloadInstructionsFull.ImageUrl =
                "~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-full.png";

            this.LiteralDownloadInstructions.Text =
                this.LiteralDownloadInstructionsModal.Text =
                Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions_SebAccountFile;

        }


        protected void ButtonBankgiroSEFile_Click(object sender, ImageClickEventArgs e)
        {
            OnSelectedFileType();

            this.HiddenFileType.Value = "BankgiroSE";

            this.ButtonBankgiroSEFile.CssClass = "FileTypeImage FileTypeImageSelected";

            this.ImageDownloadInstructions.ImageUrl = "~/Images/Ledgers/uploadbankfiles-bankgirose-small.png";

            this.ImageDownloadInstructionsFull.ImageUrl =
                "~/Images/Ledgers/uploadbankfiles-bankgirose-full.png";

            this.LiteralDownloadInstructions.Text =
                this.LiteralDownloadInstructionsModal.Text =
                Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions_BankgiroSEFile;

            this.LiteralLastAccountRecord.Visible = false;
        }


        protected void ButtonPaypalFile_Click(object sender, ImageClickEventArgs e)
        {
            OnSelectedFileType();

            this.HiddenFileType.Value = "PayPal";

            this.ButtonPaypalFile.CssClass = "FileTypeImage FileTypeImageSelected";

            this.ImageDownloadInstructions.ImageUrl = "~/Images/Ledgers/uploadbankfiles-paypal-small.png";

            this.ImageDownloadInstructionsFull.ImageUrl =
                "~/Images/Ledgers/uploadbankfiles-paypal-full.png";

            this.LiteralDownloadInstructions.Text =
                this.LiteralDownloadInstructionsModal.Text =
                Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions_PaypalFile;

            this.LiteralLastAccountRecord.Visible = false;
        }


        protected void ButtonPaysonFile_Click(object sender, ImageClickEventArgs e)
        {
            OnSelectedFileType();

            this.HiddenFileType.Value = "Payson";

            this.ButtonPaysonFile.CssClass = "FileTypeImage FileTypeImageSelected";

            this.ImageDownloadInstructions.ImageUrl = "~/Images/Ledgers/uploadbankfiles-payson-small.png";

            this.ImageDownloadInstructionsFull.ImageUrl =
                "~/Images/Ledgers/uploadbankfiles-payson-full.png";

            this.LiteralDownloadInstructions.Text =
                this.LiteralDownloadInstructionsModal.Text =
                Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions_PaysonFile;

            this.LiteralLastAccountRecord.Visible = true;
        }


        private void OnSelectedFileType()
        {
            this.ButtonPaypalFile.CssClass = 
            this.ButtonSebFile.CssClass = 
            this.ButtonBankgiroSEFile.CssClass =
            this.ButtonPaysonFile.CssClass = "FileTypeImage UnselectedType";

            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeType", "$(\".UnselectedType\").fadeTo('fast',0.2);", true);
            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeAccount1",
                                                    "$(\"#DivSelectAccount\").fadeTo('slow', 1.0);", true);
            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeAccount2",
                                                       "$(\"#DivSelectAccount\").css('display','inline');", true);

            this.LiteralLastAccountRecord.Visible = true;
        }*/
 

        private void PopulateAccountDropDown()
        {
            FinancialAccounts accounts = this.CurrentOrganization.FinancialAccountsExternal;

            this.DropAccounts.Items.Clear();
            this.DropAccounts.Items.Add(new ListItem(Resources.Global.Global_DropInits_SelectFinancialAccount, "0"));

            foreach (FinancialAccount account in accounts)
            {
                if (account.AccountType == FinancialAccountType.Asset)
                {
                    this.DropAccounts.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
                }
            }

        }


        [WebMethod(true)]
        public static void InitializeProcessing(string guid, string accountIdString)
        {
            // Start an async thread that does all the work, then return

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            int accountId = Int32.Parse(accountIdString);
            BankFileType fileType = BankFileType.AccountStatement;

            if (accountId < 0)
            {
                accountId = -accountId;
                fileType = BankFileType.PaymentDetails;
            }

            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (account.Organization.Identity != authData.CurrentOrganization.Identity ||
                !authData.CurrentUser.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write)))
            {
                throw new UnauthorizedAccessException();
            }

            Thread initThread = new Thread(ProcessUploadThread);

            ProcessThreadArguments args = new ProcessThreadArguments { Guid = guid, Organization = authData.CurrentOrganization, Account = account, CurrentUser = authData.CurrentUser, FileType = fileType };

            initThread.Start(args);
        }

        private class ProcessThreadArguments
        {
            public string Guid { get; set; }
            public Organization Organization { get; set; }
            public FinancialAccount Account { get; set; }
            public Person CurrentUser { get; set; }
            public BankFileType FileType { get; set; }
        }

        private enum BankFileType
        {
            Unknown = 0,
            /// <summary>
            /// A regular bank statement
            /// </summary>
            AccountStatement,
            /// <summary>
            /// A payments file with breakdowns of aggregate transactions
            /// </summary>
            PaymentDetails
        }

        [WebMethod]
        static public string GetAccountUploadInstructions(string guid, string accountIdString)
        {
            int accountId = Int32.Parse(accountIdString);

            // HACK HACK RELENTLESS HACK TODO

            switch (accountId)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "(Instruct Stock-SE-SEB)";
                case 2:
                    return "(Instruct Stock-Global-Paypal)";
                default:
                    throw new NotImplementedException();
            }
        }

        [WebMethod]
        static public ReportedImportResults GetReportedImportResults(string guid)
        {
            ReportedImportResults results = new ReportedImportResults();
            ImportResultsCategory category = (ImportResultsCategory) GuidCache.Get(guid + "-Result");
            ImportResults resultDetail = (ImportResults) GuidCache.Get(guid + "-ResultDetails");
            string html = string.Empty;

            switch (category)
            {
                case ImportResultsCategory.Good:
                    html = String.Format(Resources.Pages.Ledgers.UploadBankFiles_ResultsGood,
                        resultDetail.TransactionsImported, resultDetail.DuplicateTransactions,
                        resultDetail.EarliestTransaction, resultDetail.LatestTransaction);
                    break;
                case ImportResultsCategory.Questionable:
                    html = String.Format(Resources.Pages.Ledgers.UploadBankFiles_ResultsQuestionable,
                        resultDetail.TransactionsImported, resultDetail.DuplicateTransactions,
                        Math.Abs(resultDetail.BalanceMismatchCents / 100.0), resultDetail.CurrencyCode);
                    break;
                case ImportResultsCategory.Bad:
                    html = Resources.Pages.Ledgers.UploadBankFiles_ResultsBad;
                    break;
                default:
                    throw new NotImplementedException("Unhandled ImportResultCategory");
            }

            results.Html = html;
            results.Category = category.ToString();
            return results;
        }

        public class ReportedImportResults
        {
            public string Category { get; set; }
            public string Html { get; set; }
        }

        private static void ProcessUploadThread(object args)
        {
            string guid = ((ProcessThreadArguments) args).Guid;
            BankFileType fileType = ((ProcessThreadArguments) args).FileType;

            Documents documents = Documents.RecentFromDescription(guid);
            GuidCache.Set(guid + "-Result", ImportResultsCategory.Bad); // default - this is what happens if exception

            if (documents.Count != 1)
            {
                return; // abort
            }

            Document uploadedDoc = documents[0];

            try
            {
                FinancialAccount account = ((ProcessThreadArguments) args).Account;

                ExternalBankData externalData = new ExternalBankData();
                externalData.Profile = account.ExternalBankDataProfile;

                if (fileType == BankFileType.AccountStatement)
                {
                    using (StreamReader reader = uploadedDoc.GetReader(1252))
                    {
                        externalData.LoadData(reader, ((ProcessThreadArguments) args).Organization);
                            // catch here and set result to BAD
                        ImportResults results = ProcessImportedData(externalData, (ProcessThreadArguments) args);

                        GuidCache.Set(guid + "-ResultDetails", results);
                        if (results.AccountBalanceMatchesBank)
                        {
                            GuidCache.Set(guid + "-Result", ImportResultsCategory.Good);
                        }
                        else
                        {
                            GuidCache.Set(guid + "-Result", ImportResultsCategory.Questionable);
                        }
                    }
                }
                else if (fileType == BankFileType.PaymentDetails)
                {
                    // Get reader factory from ExternalBankData

                    throw new NotImplementedException("Need to implement new flexible payment reader structure");

                    // IBankDataPaymentsReader paymentsReader = externalData.GetPaymentsReader();
                    // then read
                }
            }
            catch (Exception e)
            {
                GuidCache.Set(guid + "-Exception", e.ToString());
            }
            finally
            {
                GuidCache.Set(guid + "-Progress", 100); // only here may the caller fetch the results
                uploadedDoc.Delete(); // document no longer needed after processing, no matter the result
            }
        }



        /*
        private void Submit_Click(object sender, EventArgs e)
        {
            bool fileWasUploaded = false;

            foreach (string fileInputID in Request.Files)
            {
                UploadedFile file = UploadedFile.FromHttpPostedFile(Request.Files[fileInputID]);
                if (file.ContentLength > 0)
                {

                    // TODO: PROCESS
                    // file.SaveAs("c:\\temp\\" + file.GetName());

                    this.PanelFileTypeAccount.Visible = false;
                    this.PanelResults.Visible = true;
                    fileWasUploaded = true;

                    // Interpret and then import the data

                    ExternalBankData bankData = new ExternalBankData();

                    try
                    {
                        // Set initial progress to light up progress box.

                        UpdateImportProgressBar(1);

                        switch(this.HiddenFileType.Value)
                        {
                            case "Seb":
                                bankData.Profile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.SESebId);
                                bankData.LoadData(new StreamReader(file.InputStream, Encoding.GetEncoding(1252)), this.CurrentOrganization);
                                break;
                            case "PayPal":
                                bankData.Profile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.PaypalId);
                                bankData.LoadData(new StreamReader(file.InputStream, Encoding.GetEncoding(1252)), this.CurrentOrganization);
                                break;
                            case "Payson":
                                throw new NotImplementedException("Won't reimplement");
                                break;
                            case "BankgiroSE": // Payment file, not transaction file
                                ImportedPaymentData paymentData = ImportBankgiroSE(file.InputStream);
                                PresentPaymentFileResults(paymentData);
                                return; // Exit function here -- shortcut for when there's payment data and no transaction data
                            default:
                                throw new InvalidOperationException("File type value not set to a valid filter name");
                        }

                        this.LabelProcessing.Text = bankData.Records.ToString();
                        ImportResults results = ProcessImportedData(bankData);

                        if (results.AccountBalanceMatchesBank)
                        {
                            this.LiteralImportResults.Text =
                                String.Format(Resources.Pages.Ledgers.UploadBankFiles_ImportResults,
                                              results.DuplicateTransactions + results.TransactionsImported,
                                              results.TransactionsImported,
                                              FinancialAccount.FromIdentity(Int32.Parse(this.DropAccounts.SelectedValue))
                                                  .Name,
                                              results.DuplicateTransactions);
                            this.LabelImportResultsHeader.Text = Resources.Pages.Ledgers.UploadBankFiles_FileImportedHeader;
                        }
                        else
                        {
                            this.LiteralImportResults.Text =
                                String.Format(Resources.Pages.Ledgers.UploadBankFiles_ErrorBalance,
                                              results.DuplicateTransactions + results.TransactionsImported,
                                              results.TransactionsImported,
                                              FinancialAccount.FromIdentity(Int32.Parse(this.DropAccounts.SelectedValue))
                                                  .Name,
                                              results.DuplicateTransactions);
                            this.LabelImportResultsHeader.Text = Resources.Pages.Ledgers.UploadBankFiles_FileImportedHeader_ErrorBalance;
                        }
                    }
                    catch (InvalidOperationException exception)
                    {
                        this.LiteralDownloadInstructionsModal.Text = exception.ToString(); // For debug purposes -- will not be shown to user
                        this.PanelErrorImage.Visible = true;
                        this.LabelImportResultsHeader.Text = Resources.Pages.Ledgers.UploadBankFiles_ImportError;
                        this.LiteralImportResults.Text = Resources.Pages.Ledgers.UploadBankFiles_ErrorInterpretation;
                    }
                }

            }

            if (!fileWasUploaded)
            {
                // If no file was uploaded, re-show the instructions div

                this.LabelNoFileUploaded.Text = Resources.Global.Global_Upload_ErrorSelectFile;
                this.LiteralDivInstructionsStyle.Text = @"style='display:inline'";
            }
            else
            {
                this.LabelNoFileUploaded.Text = string.Empty;
                this.LiteralDivInstructionsStyle.Text = @"style='display:none'";
            }
        }*/

        /*
        private void PresentPaymentFileResults (ImportedPaymentData data)
        {
            string literalResult = String.Format(Resources.Pages.Ledgers.UploadBankFiles_PaymentSummary1,
                   LocalizeCount(Resources.Pages.Ledgers.UploadBankFiles_PaymentCount, data.DuplicatePaymentCount + data.PaymentCount)) + " ";

            if (data.PaymentCount == 0 && data.DuplicatePaymentCount > 0)
            {
                literalResult += Resources.Pages.Ledgers.UploadBankFiles_PaymentAllDuplicates;
            }
            else if (data.PaymentCount > 0)
            {
                literalResult += String.Format(Resources.Pages.Ledgers.UploadBankFiles_PaymentSummary2,
                                               data.Currency.Code,
                                               data.PaymentCentsTotal/100.0,
                                               LocalizeCount(Resources.Pages.Ledgers.UploadBankFiles_PaymentCount,
                                                             data.PaymentCount),
                                               LocalizeCount(Resources.Pages.Ledgers.UploadBankFiles_PaymentGroupCount,
                                                             data.PaymentGroupCount));

                if (data.DuplicatePaymentCount > 0)
                {
                    literalResult += " " +
                                     String.Format(Resources.Pages.Ledgers.UploadBankFiles_PaymentSummaryDuplicates,
                                                   LocalizeCount(Resources.Pages.Ledgers.UploadBankFiles_PaymentCount,
                                                                 data.DuplicatePaymentCount),
                                                   LocalizeCount(
                                                       Resources.Pages.Ledgers.UploadBankFiles_PaymentGroupCount,
                                                       data.DuplicatePaymentGroupCount));
                }
            }

            this.LiteralImportResults.Text = @"<p>" + literalResult + @"</p>";
            this.LabelImportResultsHeader.Text = Resources.Pages.Ledgers.UploadBankFiles_PaymentFileUploadedHeader;
        }*/

        [Serializable]
        public class ImportResults
        {
            public ImportResults()
            {
                this.EarliestTransaction = DateTime.MaxValue;
                this.LatestTransaction = DateTime.MinValue;
            }

            public DateTime EarliestTransaction;
            public DateTime LatestTransaction;
            public int TransactionsImported;
            public int DuplicateTransactions;
            public bool AccountBalanceMatchesBank;
            public long BalanceMismatchCents;
            public string CurrencyCode;
        }


        [WebMethod]
        static public string GetImportResults(string guid)
        {
            return string.Empty;
        }

        static private ImportResults ProcessImportedData(ExternalBankData import, ProcessThreadArguments args)
        {
            FinancialAccount assetAccount = args.Account;
            FinancialAccount autoDepositAccount = args.Organization.FinancialAccounts.IncomeDonations;
            int autoDepositLimit = 1000; // TODO: this.CurrentOrganization.Parameters.AutoDonationLimit;

            ImportResults result = new ImportResults();
            int count = 0;
            int progressUpdateInterval = import.Records.Length/40;

            if (progressUpdateInterval > 100)
            {
                progressUpdateInterval = 100;
            }

            foreach (ExternalBankDataRecord row in import.Records)  
            {
                // Update progress.

                count++;
                if (progressUpdateInterval < 2 || count % progressUpdateInterval == 0)
                {
                    int percent = (count*99)/import.Records.Length;

                    GuidCache.Set(args.Guid + "-Progress", percent);
                }

                // Update high- and low-water marks.

                if (row.DateTime < result.EarliestTransaction)
                {
                    result.EarliestTransaction = row.DateTime;
                }

                if (row.DateTime > result.LatestTransaction)
                {
                    result.LatestTransaction = row.DateTime;
                }


                string importKey = row.ImportHash;

                Int64 amountCents = row.TransactionNetCents;

                if (amountCents == 0) // defensive programming - these _should_ be duplicated in the interpreter if no "fee" field
                {
                    amountCents = row.TransactionGrossCents;
                }

                if (args.Organization.Identity == 1 && assetAccount.Identity == 1 && PilotInstallationIds.IsPilot(PilotInstallationIds.PiratePartySE))
                {
                    // This is an ugly-as-fuck hack that sorts under the category "just bring our pilots the fuck back to operational
                    // status right fucking now".

                    // This code can and should be safely removed once the pilot's books are closed for 2014, which should be some time mid-2015.

                    if (row.DateTime < new DateTime(2014,03,22))
                    {
                        result.DuplicateTransactions++;
                        continue;
                    }
                }

                FinancialTransaction transaction = FinancialTransaction.ImportWithStub(args.Organization.Identity, row.DateTime,
                                                                                       assetAccount.Identity, amountCents,
                                                                                       row.Description, importKey,
                                                                                       args.CurrentUser.Identity);

                if (transaction != null)
                {
                    // The transaction was created. Examine if the autobook criteria are true.

                    result.TransactionsImported++;

                    FinancialAccounts accounts = FinancialAccounts.FromBankTransactionTag(row.Description);

                    if (accounts.Count == 1)
                    {
                        // This is a labelled local donation.

                        Geography geography = accounts[0].AssignedGeography;
                        FinancialAccount localAccount = accounts[0];

                        transaction.AddRow(args.Organization.FinancialAccounts.IncomeDonations, -amountCents, args.CurrentUser);
                        transaction.AddRow(args.Organization.FinancialAccounts.CostsLocalDonationTransfers,
                                           amountCents, args.CurrentUser);
                        transaction.AddRow(localAccount, -amountCents, args.CurrentUser);

                        PWEvents.CreateEvent(EventSource.PirateWeb, EventType.LocalDonationReceived,
                                                                     args.CurrentUser.Identity, args.Organization.Identity,
                                                                     geography.Identity, 0,
                                                                     transaction.Identity, localAccount.Identity.ToString());
                    }
                    else if (row.Description.ToLowerInvariant().StartsWith(args.Organization.IncomingPaymentTag))
                    {
                        // Check for previously imported payment group

                        // TODO: MAKE FLEXIBLE - CALL PAYMENTREADERINTERFACE!
                        // HACK HACK HACK HACK

                        PaymentGroup group = PaymentGroup.FromTag(args.Organization,
                                                                  "SEBGM" + DateTime.Today.Year.ToString() +   // TODO: Get tags from org
                                                                  row.Description.Substring(args.Organization.IncomingPaymentTag.Length).Trim());

                        if (group != null && group.Open)
                        {
                            // There was a previously imported and not yet closed payment group matching this transaction
                            // Close the payment group and match the transaction against accounts receivable

                            transaction.Dependency = group;
                            group.Open = false;
                            transaction.AddRow(args.Organization.FinancialAccounts.AssetsOutboundInvoices, -amountCents, args.CurrentUser);
                        }
                    }
                    else if (amountCents < 0)
                    {
                        // Autowithdrawal mechanisms removed, condition kept because of downstream else-if conditions
                    }
                    else if (amountCents > 0)
                    {
                        if (row.FeeCents < 0)
                        {
                            // This is always an autodeposit, if there is a fee (which is never > 0.0)

                            transaction.AddRow(args.Organization.FinancialAccounts.CostsBankFees, -row.FeeCents, args.CurrentUser);
                            transaction.AddRow(autoDepositAccount, -row.TransactionGrossCents, args.CurrentUser);
                        }
                        else if (amountCents < autoDepositLimit * 100)
                        {
                            // Book against autoDeposit account.

                            transaction.AddRow(autoDepositAccount, -amountCents, args.CurrentUser);
                        }
                    }
                }
                else
                {
                    // Transaction was not imported; assume duplicate

                    result.DuplicateTransactions++;
                }
            }

            // Import complete. Return true if the bookkeeping account matches the bank data.

            Int64 databaseAccountBalanceCents = assetAccount.BalanceTotalCents;

            // Subtract any transactions made after the most recent imported transaction.
            // This is necessary in case of Paypal and others which continuously feed the
            // bookkeeping account with new transactions; it will already have fed transactions
            // beyond the end-of-file.

            Int64 beyondEofCents = assetAccount.GetDeltaCents(result.LatestTransaction.AddSeconds(1), DateTime.Now.AddDays(2)); // Caution: the "AddSeconds(1)" is not foolproof, there may be other new txs on the same second.

            if (databaseAccountBalanceCents - beyondEofCents == import.LatestAccountBalanceCents)
            {
                Payouts.AutomatchAgainstUnbalancedTransactions(args.Organization);
                result.AccountBalanceMatchesBank = true;
                result.BalanceMismatchCents = 0;
            }
            else
            {
                result.AccountBalanceMatchesBank = false;
                result.BalanceMismatchCents = (databaseAccountBalanceCents - beyondEofCents) -
                                              import.LatestAccountBalanceCents;
            }

            result.CurrencyCode = args.Organization.Currency.Code;
            return result;
        }

        /*
        protected void UpdateImportProgressBar (int percentDone)
        {
            RadProgressContext progress = RadProgressContext.Current;
            progress["PrimaryPercent"] = percentDone.ToString();

            System.Threading.Thread.Sleep(100);
        }*/


        public enum ImportResultsCategory
        {
            Unknown = 0,
            Good,
            Questionable,
            Bad
        }



        protected class ImportedPaymentData
        {
            public int PaymentGroupCount;
            public int PaymentCount;
            public int DuplicatePaymentCount;
            public int DuplicatePaymentGroupCount;
            public Int64 PaymentCentsTotal;
            public Currency Currency;
        }




        // These Functions Copied From PWv4 -- may need adaptation and/or modernization


        protected class InMemoryPayment
        {
            public InMemoryPayment()
            {
                this.Information = new List<InMemoryPaymentInformation>();
            }

            public Int64 AmountCents;
            public string Reference;
            public bool HasImage;
            public string FromAccount;
            public string Key;
            public List<InMemoryPaymentInformation> Information;
        }

        protected class InMemoryPaymentInformation
        {
            public InMemoryPaymentInformation(PaymentInformationType type, string data)
            {
                this.Type = type;
                this.Data = data;
            }

            public PaymentInformationType Type;
            public string Data;
        }


        protected ImportedPaymentData ImportBankgiroSE(Stream fileStream)
        {
            string contents = string.Empty;
            using (TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding(1252)))
            {
                contents = reader.ReadToEnd();
            }

            string[] lines = contents.Split('\n');

            DateTime timestamp = DateTime.MinValue;
            int bgMaxVersion = 0;

            ImportedPaymentData result = new ImportedPaymentData();
            Currency currency = null;
            List<InMemoryPayment> curPayments = null;
            InMemoryPayment curPayment = null;
            Int64 curPaymentGroupAmountCents = 0;

            foreach (string line in lines)
            {
                if (line.Length < 2)
                {
                    continue; // CR/LF split causes every other line to be empty
                }

                switch (line.Substring(0, 2))
                {
                    case "01": // BGMAX intro
                        string bgmaxmarker = line.Substring(2, 20).Trim();
                        if (bgmaxmarker != "BGMAX")
                        {
                            throw new Exception("bad format -- not bgmax");
                        }
                        bgMaxVersion = Int32.Parse(line.Substring(22, 2));
                        timestamp = DateTime.ParseExact(line.Substring(24, 20), "yyyyMMddHHmmssffffff",
                                                        CultureInfo.InvariantCulture);
                        break;
                    case "05": // Begin payment group
                        if (bgMaxVersion < 1)
                        {
                            throw new InvalidOperationException("BGMax record must precede first payment group");
                        }
                        curPayments = new List<InMemoryPayment>();
                        currency = Currency.FromCode(line.Substring(22, 3));
                        result.Currency = currency;
                        curPaymentGroupAmountCents = 0;
                        break;
                    case "20": // Begin payment
                        if (curPayments == null)
                        {
                            throw new InvalidOperationException("Payment group start must precede first payment");
                        }

                        // If we have a previous payment in this group, add it to list

                        if (curPayment != null)
                        {
                            curPayments.Add(curPayment);
                        }

                        curPayment = new InMemoryPayment();

                        curPayment.FromAccount = line.Substring(2, 10);
                        curPayment.Reference = line.Substring(12, 25).Trim(); // left space padded in BgMax format
                        curPayment.AmountCents = Int64.Parse(line.Substring(37, 18), CultureInfo.InvariantCulture);
                        curPayment.Key = "SEBGM" + DateTime.Today.Year.ToString() + line.Substring(57, 12);
                        curPayment.HasImage = (line[69] == '1' ? true : false);

                        // TODO: Check if existed already -- must do -- IMPORTANT (same todo as below)

                        curPaymentGroupAmountCents += curPayment.AmountCents;
                        break;
                    case "25": // Payment info: Freeform
                        if (curPayment == null)
                        {
                            throw new InvalidOperationException("Payment start must precede payment information");
                        }
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.Freeform, line.Substring(2, 50).Trim()));
                        break;
                    case "26": // Payment info: Name
                        if (curPayment == null)
                        {
                            throw new InvalidOperationException("Payment start must precede payment information");
                        }
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.Name, line.Substring(2, 35).Trim()));
                        break;
                    case "27": // Payment info: Street, postal code
                        if (curPayment == null)
                        {
                            throw new InvalidOperationException("Payment start must precede payment information");
                        }
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.Street, line.Substring(2, 35).Trim()));
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.PostalCode, line.Substring(37, 9).Replace(" ", ""))); // also removes inspace
                        break;
                    case "28": // Payment info: City, Country
                        if (curPayment == null)
                        {
                            throw new InvalidOperationException("Payment start must precede payment information");
                        }
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.City, line.Substring(2, 35).Trim()));
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.Country, line.Substring(37, 35).Trim()));
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.CountryCode, line.Substring(72, 2).Trim()));
                        break;
                    case "29": // Payment info: Organization or personal ID number
                        if (curPayment == null)
                        {
                            throw new InvalidOperationException("Payment start must precede payment information");
                        }
                        curPayment.Information.Add(new InMemoryPaymentInformation(PaymentInformationType.OrgNumber, line.Substring(2, 12).Trim()));
                        break;
                    case "15": // End payment group
                        if (curPayments == null)
                        {
                            throw new InvalidOperationException("Payment group start must precede payment group end");
                        }

                        // Add currently building payment to group before committing

                        curPayments.Add(curPayment);

                        // This is where we finally get a unique identifier that allows us to dupecheck.

                        string tag = timestamp.Year.ToString() + line.Substring(45, 5);

                        if (timestamp.Year >= 2012)
                        {
                            tag = "SEBGM" + tag; // a flag date where we add a tag for SE Bankgiro Max format, enabling other formats in other namespaces
                        }

                        // Dupe check

                        PaymentGroup dupe = PaymentGroup.FromTag(this.CurrentOrganization, tag);

                        if (dupe == null)
                        {
                            // Commit all recorded payments

                            PaymentGroup newGroup = PaymentGroup.Create(this.CurrentOrganization, timestamp, currency, this.CurrentUser);
                            result.PaymentGroupCount++;

                            Int64 reportedAmountCents = Int64.Parse(line.Substring(50, 18), CultureInfo.InvariantCulture); // may differ because of duplicates
                            newGroup.AmountCents = curPaymentGroupAmountCents;
                            result.PaymentCentsTotal += curPaymentGroupAmountCents;

                            foreach (InMemoryPayment payment in curPayments)
                            {
                                // TODO: DUPECHECK PAYMENT KEY AS WELL (same todo as above)

                                Payment newPayment = newGroup.CreatePayment(payment.AmountCents/100.0, payment.Reference,
                                                                         payment.FromAccount, payment.Key,
                                                                         payment.HasImage);

                                foreach (InMemoryPaymentInformation paymentInfo in payment.Information)
                                {
                                    newPayment.AddInformation(paymentInfo.Type, paymentInfo.Data);
                                }
                                result.PaymentCount++;
                            }

                            newGroup.Tag = tag;
                            newGroup.Open = true; // flags payment group as ready

                            newGroup.MapTransaction();
                        }
                        else
                        {
                            // This was a duplicate

                            result.DuplicatePaymentGroupCount++;
                            result.DuplicatePaymentCount += curPayments.Count;
                        }

                        curPayment = null;
                        curPayments = null;

                        break;
                    case "70": // BGMAX termination
                        break; // don't care
                    default:
                        break; // don't care about other fields
                }
            }

            if (timestamp.Year < 1900)
            {
                // The file contained no instructions at all

                throw new ArgumentException("This was not a BGMAX-SE file");
            }

            return result;
        }



        /*
        protected ImportedBankData ImportSebText(Stream fileStream)
        {
            string contents = string.Empty;
            using (TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding(1252)))
            {
                contents = reader.ReadToEnd();
            }

            string[] lines = contents.Split('\n');

            if (lines[0].Trim() != "Bokföringsdatum\tValutadatum\tVerifikationsnummer\tText/mottagare\tBelopp\tSaldo")
            {
                throw new ArgumentException("Unable to parse");
            }

            List<ImportedBankRow> rows = new List<ImportedBankRow>();
            ImportedBankData result = new ImportedBankData();

            string[] firstLineParts = lines[1].Split('\t');

            string stringBalance = firstLineParts[5].Trim().Replace(",", "");
            result.CurrentBalanceCents = Int64.Parse(stringBalance, CultureInfo.InvariantCulture);
            result.CurrentBalance = result.CurrentBalanceCents/100.0;

            int rowCount = lines.Length;

            for (int lineIndex = 1; lineIndex < rowCount; lineIndex++) // Skip first row on purpose - is header row
            {
                string[] lineParts = lines[lineIndex].Split('\t');

                if (lineParts.Length < 2)
                {
                    // Assume empty line, probably last line; do not process
                    continue;
                }

                string amountString = lineParts[4].Replace(".", "").Replace(",", "");

                ImportedBankRow row = new ImportedBankRow();
                row.DateTime = DateTime.Parse(lineParts[0]);
                row.Comment = lineParts[3].Replace("  ", " ").Trim();
                row.CurrentBalanceCents = Int64.Parse(lineParts[5].Trim().Replace(",", ""), CultureInfo.InvariantCulture);
                row.AmountCentsNet = Int64.Parse(amountString);
                row.HashBase = lineParts[2];

                rows.Add(row);
            }

            result.Rows = rows;
            return result;
        }


        protected ImportedBankData ImportPaypal(Stream fileStream)
        {
            string contents = string.Empty;
            using (TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding(1252)))
            {
                contents = reader.ReadToEnd();
            }

            string[] lines = contents.Split('\n');
            string formatVerifier = lines[0].Trim();
            if (formatVerifier != "Date\t Time\t Time Zone\t Name\t Type\t Status\t Currency\t Gross\t Fee\t Net\t From Email Address\t To Email Address\t Transaction ID\t Counterparty Status\t Address Status\t Item Title\t Item ID\t Shipping and Handling Amount\t Insurance Amount\t Sales Tax\t Option 1 Name\t Option 1 Value\t Option 2 Name\t Option 2 Value\t Auction Site\t Buyer ID\t Item URL\t Closing Date\t Escrow Id\t Invoice Id\t Reference Txn ID\t Invoice Number\t Custom Number\t Receipt ID\t Balance\t Address Line 1\t Address Line 2/District/Neighborhood\t Town/City\t State/Province/Region/County/Territory/Prefecture/Republic\t Zip/Postal Code\t Country\t Contact Phone Number")
            //                     0      1      2           3      4      5        6          7       8     9     10                   11                 12               13                    14               15           16        17                             18                 19          20              21               22              23               24             25         26         27             28          29           30                 31               32              33           34        35               36                                     37          38                                                           39                          40
            {
                throw new ArgumentException("Unable to parse");
            }

            ImportedBankData result = new ImportedBankData();
            List<ImportedBankRow> rows = new List<ImportedBankRow>();

            foreach (string line in lines)
            {
                string[] parts = line.Split('\t');

                if (parts.Length < 30)
                { 
                    continue;
                }

                if (StripQuotes(parts[6]) != "SEK")
                {
                    continue; // HACK: Need to fix currency support at some time
                }

                // Get current balance from the first line in the file

                if (result.CurrentBalanceCents == 0)
                {
                    result.CurrentBalanceCents = Int64.Parse(StripQuotes(parts[34].Replace(".", "").Replace(",", "").Replace(" ", "")), CultureInfo.InvariantCulture);
                }

                ImportedBankRow row = new ImportedBankRow();

                row.SuppliedTransactionId = StripQuotes(parts[12]);
                row.Comment = StripQuotes(parts[4]);
                row.DateTime = DateTime.Parse(StripQuotes(parts[0]) + " " + StripQuotes(parts[1]), CultureInfo.InvariantCulture);
                row.AmountCentsGross = Int64.Parse(StripQuotes(parts[7]).Replace(".", "").Replace(",", "").Replace(" ", ""));
                row.AmountCentsNet = Int64.Parse(StripQuotes(parts[9]).Replace(".", "").Replace(",", "").Replace(" ", ""));
                row.FeeCents = 0;
                if (parts[8] != "\"...\"")  // used as fee field for payments held/cleared
                {
                    row.FeeCents = Int64.Parse(StripQuotes(parts[8]).Replace(".", "").Replace(",", "").Replace(" ", ""), CultureInfo.InvariantCulture);
                }


                // Adjust for timezone -- necessary for Paypal and other int'l services

                string timeZoneString = StripQuotes(parts[2]);

                if (timeZoneString.Length < 3+1+2+1+2 || (!timeZoneString.StartsWith("GMT") && !timeZoneString.StartsWith("UTC")))
                {
                    throw new ArgumentException("Paypal import files should have time zones on format \"GMT+hh:mm\"");
                }

                int timeSign = (timeZoneString[3] == '-' ? -1 : 1);
                int hourDiff = int.Parse(timeZoneString.Substring(4, 2)) * timeSign;
                int minuteDiff = int.Parse(timeZoneString.Substring(7, 2)) * timeSign;

                row.DateTime = row.DateTime.AddHours(-hourDiff).AddMinutes(-minuteDiff).ToLocalTime();

                rows.Add(row);
            }

            result.Rows = rows;
            return result;
        }


        protected ImportedBankData ImportPayson (Stream fileStream)
        {
            string contents = string.Empty;
            using (TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding(1252)))
            {
                contents = reader.ReadToEnd();
            }

            string regexPattern = @"<tr>\s+<td>\s*(?<datetime>[0-9]{4}-[0-9]{2}-[0-9]{2}\s[0-9]{2}:[0-9]{2}:[0-9]{2})\s*</td><td>(?<comment1>[^<]*)</td><td>[^>]*</td><td>(?<txid>[0-9]+)</td>\s*<td>(?<from>[^<]+)</td>\s*<td>(?<to>[^<]+)</td><td class=\""tal\"">(?<gross>[\-0-9,]+)</td><td class=\""tal\"">(?<fee>[\-0-9,]+)</td><td class=\""tal\"">(?<vat>[\-0-9,]+)</td><td class=\""tal\"">(?<net>[\-0-9,]+)</td><td class=\""tal\"">(?<balance>[\-0-9,]+)</td><td>(?<currency>[^<]+)</td><td>(?<reference>[^<]+)</td><td[^>]+?>(?<comment2>[^<]+)</td>";

            Regex regex = new Regex(regexPattern, RegexOptions.Singleline);
            Match match = regex.Match(contents);

            ImportedBankData result = new ImportedBankData();
            List<ImportedBankRow> rows = new List<ImportedBankRow>();

            while (match.Success)
            {
                if (match.Groups["currency"].Value != "SEK")
                {
                    continue; // HACK: Need to fix currency support at some time
                }

                // Get current balance from the first line in the file

                if (result.CurrentBalanceCents == 0)
                {
                    result.CurrentBalanceCents = Int64.Parse(match.Groups["balance"].Value.Replace(",", "")) / 100;
                }

                ImportedBankRow row = new ImportedBankRow();

                string comment = HttpUtility.HtmlDecode(match.Groups["comment2"].Value.Trim());
                if (String.IsNullOrEmpty(comment))
                {
                    comment = match.Groups["comment1"].Value.Trim();
                }

                row.SuppliedTransactionId = "Payson-" + match.Groups["txid"].Value;
                row.Comment = comment;
                row.DateTime = DateTime.Parse(match.Groups["datetime"].Value, CultureInfo.InvariantCulture);
                row.AmountCentsGross = Int64.Parse(match.Groups["gross"].Value.Replace(".", "").Replace(",", "")) / 100;
                row.FeeCents = Int64.Parse(match.Groups["fee"].Value.Replace(".", "").Replace(",", "")) / 100;
                row.AmountCentsNet = Int64.Parse(match.Groups["net"].Value.Replace(".", "").Replace(",", "")) / 100;

                if (row.DateTime < new DateTime(2010,4,1))
                {
                    // This is for historical reasons of how Payson transactions were originally handled.
                    // TODO: Re-handle by not touching closed books instead.

                    match = match.NextMatch();
                    continue;
                }

                rows.Add(row);

                match = match.NextMatch();
            }

            result.Rows = rows;
            return result;
        }



        protected string StripHtml(string input)
        {
            // Doesn't really strip HTML -- what it does is strip a link markup (<a...> and </a>)

            if (input.ToLower().EndsWith("</a>"))
            {
                input = input.Substring(0, input.Length - 4);

                int indexOfLastGT = input.LastIndexOf('>');
                input = input.Substring(indexOfLastGT + 1);
            }

            return input;
        }

        protected string StripQuotes(string input)
        {
            if (input.StartsWith("\""))
            {
                input = input.Substring(1);
            }

            if (input.EndsWith("\""))
            {
                input = input.Substring(0, input.Length - 1);
            }

            return input;
        }*/

    }
}