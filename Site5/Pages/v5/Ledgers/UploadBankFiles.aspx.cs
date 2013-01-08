using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Pirates;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Telerik.Web.UI;
using Telerik.Web.UI.Upload;


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

                this.LabelDownloadInstructions.Text = Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions;
                this.LabelClickImage.Text = Resources.Global.Global_ClickImageToEnlarge;

                this.InfoBoxLiteral = Resources.Pages.Ledgers.UploadBankFiles_Info;

                this.LabelSelectBankAndAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectBankAndAccount;
                this.LabelSelectFileType.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectBankFileType;
                this.LabelSelectAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectAccount;
                this.LabelUploadH2Header.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadBankFile;
                this.LabelUploadH3Header.Text = Resources.Global.Global_UploadFileToActivizr;
                this.Upload.Text = Resources.Global.Global_UploadFile;
                this.LabelProcessing.Text = Resources.Global.Global_ProcessingFile;
                this.LinkUploadAnother.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadAnother;
                this.LabelModalInstructionHeader.Text = Resources.Pages.Ledgers.UploadBankFiles_BankScreenshot;

                // Populate the asset account dropdown, if needed for file type

                PopulateAccountDropDown();

            }

            if (!IsPostBack)
            {
                //Do not display SelectedFilesCount progress indicator.
                this.ProgressIndicator.ProgressIndicators &= ~ProgressIndicators.SelectedFilesCount;
                RadProgressContext progress = RadProgressContext.Current;
                //Prevent the secondary progress from appearing when the file is uploaded (FileCount etc.)
                progress["SecondaryTotal"] = "0";
                progress["SecondaryValue"] = "0";
                progress["SecondaryPercent"] = "0";
            }
        }

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
        }
 

        private void PopulateAccountDropDown()
        {
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(this.CurrentOrganization,
                                                                           FinancialAccountType.Asset);

            this.DropAccounts.Items.Clear();
            this.DropAccounts.Items.Add(Resources.Global.Global_DropInits_SelectFinancialAccount);

            foreach (FinancialAccount account in accounts)
            {
                this.DropAccounts.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
            }

        }

        protected void DropAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LiteralSelectAccountDivStyle.Text = @"style=""opacity:1;display:inline""";

            if (this.DropAccounts.SelectedIndex > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeDownload2",
                                                        "$(\"#DivInstructions\").fadeTo('slow',1.0);", true);
                ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "ShowInstructions",
                                                        "$(\"#DivInstructions\").css('display','inline');", true);
                ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "ReiterateModality",
                                                        "$(function () { $(\"a[rel*=leanModal]\").leanModal();});", true);


                FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(this.DropAccounts.SelectedValue));

                FinancialAccountRows rows = account.GetLastRows(1);
                DateTime lastTransactionDateTime = new DateTime(1801, 1, 1);

                if (rows != null && rows.Count > 0)
                {
                    lastTransactionDateTime = rows[0].RowDateTime;
                }

                this.LiteralLastAccountRecord.Text = String.Format(Resources.Pages.Ledgers.UploadBankFiles_DownloadDataSince, Server.HtmlEncode(account.Name), lastTransactionDateTime, lastTransactionDateTime.AddDays(-5));
            }
        }

        protected void Submit_Click(object sender, EventArgs e)
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

                    ImportedBankData bankData = null;

                    try
                    {
                        // Set initial progress to light up progress box.

                        UpdateImportProgressBar(1);

                        switch(this.HiddenFileType.Value)
                        {
                            case "Seb":
                                bankData = ImportSebText(file.InputStream);
                                break;
                            case "PayPal":
                                bankData = ImportPaypal(file.InputStream);
                                break;
                            case "Payson":
                                bankData = ImportPayson(file.InputStream);
                                break;
                            case "BankgiroSE": // Payment file, not transaction file
                                ImportedPaymentData paymentData = ImportBankgiroSE(file.InputStream);
                                PresentPaymentFileResults(paymentData);
                                return; // Exit function here -- shortcut for when there's payment data and no transaction data
                            default:
                                throw new InvalidOperationException("File type value not set to a valid filter name");
                        }

                        this.LabelProcessing.Text = bankData.Rows.Count.ToString();
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
                    catch (Exception exception)
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
        }


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
        }


        protected class ImportResults
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
        }


        protected ImportResults ProcessImportedData(ImportedBankData import)
        {
            FinancialAccount assetAccount = FinancialAccount.FromIdentity(Int32.Parse(this.DropAccounts.SelectedValue));
            FinancialAccount autoDepositAccount = this.CurrentOrganization.FinancialAccounts.IncomeDonations;
            int autoDepositLimit = 1000; // TODO: this.CurrentOrganization.Parameters.AutoDonationLimit;

            ImportResults result = new ImportResults();
            int count = 0;
            int progressUpdateInterval = import.Rows.Count/40;

            if (progressUpdateInterval > 100)
            {
                progressUpdateInterval = 100;
            }

            foreach (ImportedBankRow row in import.Rows)
            {
                // Update progress.

                count++;
                if (progressUpdateInterval < 2 || count % progressUpdateInterval == 0)
                {
                    int percent = (count*99)/import.Rows.Count;
                    UpdateImportProgressBar(percent);
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


                // Each row is at least a stub, probably more.

                string importKey = row.SuppliedTransactionId;

                // If importKey is empty, construct a hash from the data fields.

                if (string.IsNullOrEmpty(importKey))
                {
                    string commentKey = row.Comment;
                    if (row.DateTime.Year >= 2012)
                    {
                        commentKey = commentKey.ToLowerInvariant();
                    }

                    string hashKey = row.HashBase + commentKey + (row.AmountCentsNet / 100.0).ToString(CultureInfo.InvariantCulture) + (row.CurrentBalanceCents / 100.0).ToString(CultureInfo.InvariantCulture) +
                                     row.DateTime.ToString("yyyy-MM-dd-hh-mm-ss");

                    importKey = SHA1.Hash(hashKey).Replace(" ", "");
                }

                if (importKey.Length > 30)
                {
                    importKey = importKey.Substring(0, 30);
                }

                Int64 amountCents = row.AmountCentsNet;

                if (amountCents == 0)
                {
                    amountCents = row.AmountCentsGross;
                }

                FinancialTransaction transaction = FinancialTransaction.ImportWithStub(this.CurrentOrganization.Identity, row.DateTime,
                                                                                       assetAccount.Identity, amountCents,
                                                                                       row.Comment, importKey,
                                                                                       this.CurrentUser.Identity);

                if (transaction != null)
                {
                    // The transaction was created. Examine if the autobook criteria are true.

                    result.TransactionsImported++;

                    FinancialAccounts accounts = FinancialAccounts.FromBankTransactionTag(row.Comment);

                    if (accounts.Count == 1)
                    {
                        // This is a labelled local donation.

                        Geography geography = accounts[0].AssignedGeography;
                        FinancialAccount localAccount = accounts[0];

                        transaction.AddRow(this.CurrentOrganization.FinancialAccounts.IncomeDonations, -amountCents, this.CurrentUser);
                        transaction.AddRow(this.CurrentOrganization.FinancialAccounts.CostsLocalDonationTransfers,
                                           amountCents, this.CurrentUser);
                        transaction.AddRow(localAccount, -amountCents, this.CurrentUser);

                        PWEvents.CreateEvent(EventSource.PirateWeb, EventType.LocalDonationReceived,
                                                                     this.CurrentUser.Identity, this.CurrentOrganization.Identity,
                                                                     geography.Identity, 0,
                                                                     transaction.Identity, localAccount.Identity.ToString());
                    }
                    else if (row.Comment.ToLowerInvariant().StartsWith(this.CurrentOrganization.IncomingPaymentTag))
                    {
                        // Check for previously imported payment group

                        PaymentGroup group = PaymentGroup.FromTag(this.CurrentOrganization,
                                                                  "SEBGM" + DateTime.Today.Year.ToString() +   // TODO: Get tags from org
                                                                  row.Comment.Substring(this.CurrentOrganization.IncomingPaymentTag.Length).Trim());

                        if (group != null && group.Open)
                        {
                            // There was a previously imported and not yet closed payment group matching this transaction
                            // Close the payment group and match the transaction against accounts receivable

                            transaction.Dependency = group;
                            group.Open = false;
                            transaction.AddRow(this.CurrentOrganization.FinancialAccounts.AssetsOutboundInvoices, -amountCents, this.CurrentUser);
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

                            transaction.AddRow(this.CurrentOrganization.FinancialAccounts.CostsBankFees, -row.FeeCents, this.CurrentUser);
                            transaction.AddRow(autoDepositAccount, -row.AmountCentsGross, this.CurrentUser);
                        }
                        else if (amountCents < autoDepositLimit * 100)
                        {
                            // Book against autoDeposit account.

                            transaction.AddRow(autoDepositAccount, -amountCents, this.CurrentUser);
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

            if (databaseAccountBalanceCents - beyondEofCents == import.CurrentBalanceCents)
            {
                Payouts.AutomatchAgainstUnbalancedTransactions(this.CurrentOrganization);
                result.AccountBalanceMatchesBank = true;
            }
            else
            {
                result.AccountBalanceMatchesBank = false;
            }

            return result;
        }


        protected void UpdateImportProgressBar (int percentDone)
        {
            RadProgressContext progress = RadProgressContext.Current;
            progress["PrimaryPercent"] = percentDone.ToString();

            System.Threading.Thread.Sleep(100);
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
                row.FeeCents = Int64.Parse(StripQuotes(parts[8]).Replace(".", "").Replace(",", "").Replace(" ", ""), CultureInfo.InvariantCulture);
                row.AmountCentsNet = Int64.Parse(StripQuotes(parts[9]).Replace(".", "").Replace(",", "").Replace(" ", ""));

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
        }

    }
}