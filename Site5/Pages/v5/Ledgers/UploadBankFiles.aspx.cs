using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Telerik.Web.UI;
using Telerik.Web.UI.Upload;


// ReSharper disable CheckNamespace
namespace Activizr.Site.Pages.Ledgers
// ReSharper restore CheckNamespace
{
    public partial class UploadBankFiles : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = Resources.Pages.Ledgers.UploadBankFiles_PageTitle;
            this.PageIcon = "iconshock-bank";
            this.PageAccessRequired = new Access(_currentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

            if (!IsPostBack)
            {
                // Localize

                this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
                this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
                this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;
                this.LabelDownloadInstructions.Text = Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions;
                this.LabelClickImage.Text = Resources.Pages.Global.Global_ClickImageToEnlarge;

                this.LabelUploadBankFilesInfo.Text = Resources.Pages.Ledgers.UploadBankFiles_Info;
                this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Todo_Placeholder;

                this.LabelSelectBankAndAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectBankAndAccount;
                this.LabelSelectFileType.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectBankFileType;
                this.LabelSelectAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectAccount;
                this.LabelUploadH2Header.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadBankFile;
                this.LabelUploadH3Header.Text = Resources.Pages.Global.Global_UploadFileToActivizr;
                this.Upload.Text = Resources.Pages.Global.Global_UploadFile;
                this.LabelProcessing.Text = Resources.Pages.Global.Global_ProcessingFile;
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

            this.HiddenFileType.Value = "SebAccount";

            this.ButtonSebAccountFile.CssClass = "FileTypeImage FileTypeImageSelected";

            this.ImageDownloadInstructions.ImageUrl = "~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-small.png";

            this.ImageDownloadInstructionsFull.ImageUrl =
                "~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-full.png";

            this.LiteralDownloadInstructions.Text =
                this.LiteralDownloadInstructionsModal.Text =
                Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions_SebAccountFile;

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


        private void OnSelectedFileType()
        {
            this.ButtonPaypalFile.CssClass = 
            this.ButtonSebAccountFile.CssClass = "FileTypeImage UnselectedType";

            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeType", "$(\".UnselectedType\").fadeTo('fast',0.2);", true);
            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeAccount1",
                                                    "$(\"#DivSelectAccount\").fadeTo('slow', 1.0);", true);
            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeAccount2",
                                                       "$(\"#DivSelectAccount\").css('display','inline');", true);

            this.LiteralLastAccountRecord.Visible = true;
        }
 

        private void PopulateAccountDropDown()
        {
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(_currentOrganization,
                                                                           FinancialAccountType.Asset);

            this.DropAccounts.Items.Clear();
            this.DropAccounts.Items.Add(Resources.Pages.Global.Global_DropInits_SelectFinancialAccount);

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
                            case "SebAccount":
                                bankData = ImportSebText(file.InputStream);
                                break;
                            case "PayPal":
                                bankData = ImportPaypal(file.InputStream);
                                break;
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

                    // Import the data to database


                }

            }

            if (!fileWasUploaded)
            {
                // If no file was uploaded, re-show the instructions div

                this.LabelNoFileUploaded.Text = Resources.Pages.Global.Global_Upload_ErrorSelectFile;
                this.LiteralDivInstructionsStyle.Text = @"style='display:inline'";
            }
            else
            {
                this.LabelNoFileUploaded.Text = string.Empty;
                this.LiteralDivInstructionsStyle.Text = @"style='display:none'";
            }
        }


        protected class ImportResults
        {
            public int TransactionsImported;
            public int DuplicateTransactions;
            public bool AccountBalanceMatchesBank;
        }


        protected ImportResults ProcessImportedData(ImportedBankData import)
        {
            FinancialAccount assetAccount = FinancialAccount.FromIdentity(Int32.Parse(this.DropAccounts.SelectedValue));
            FinancialAccount autoDepositAccount = _currentOrganization.FinancialAccounts.IncomeDonations;
            int autoDepositLimit = 1000; // TODO: _currentOrganization.Parameters.AutoDonationLimit;

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

                FinancialTransaction transaction = FinancialTransaction.ImportWithStub(_currentOrganization.Identity, row.DateTime,
                                                                                       assetAccount.Identity, amountCents,
                                                                                       row.Comment, importKey,
                                                                                       _currentUser.Identity);

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

                        transaction.AddRow(_currentOrganization.FinancialAccounts.IncomeDonations, -amountCents, _currentUser);
                        transaction.AddRow(_currentOrganization.FinancialAccounts.CostsLocalDonationTransfers,
                                           amountCents, _currentUser);
                        transaction.AddRow(localAccount, -amountCents, _currentUser);

                        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.LocalDonationReceived,
                                                                     _currentUser.Identity, _currentOrganization.Identity,
                                                                     geography.Identity, 0,
                                                                     transaction.Identity, localAccount.Identity.ToString());
                    }
                    else if (row.Comment.ToLowerInvariant().StartsWith("bg 451-0061 "))   // TODO: Organization.Parameters.FinancialTrackedTransactionPrefix
                    {
                        // Check for previously imported payment group

                        PaymentGroup group = PaymentGroup.FromTag(_currentOrganization,
                                                                  "SEBGM" + DateTime.Today.Year.ToString() +   // TODO: Get tagging from org
                                                                  row.Comment.Substring(11));

                        if (group != null)
                        {
                            // There was a previously imported and not yet closed payment group matching this transaction
                            // Close the payment group and match the transaction against accounts receivable

                            transaction.Dependency = group;
                            group.Open = false;
                            transaction.AddRow(_currentOrganization.FinancialAccounts.AssetsOutboundInvoices, -amountCents, _currentUser);
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

                            transaction.AddRow(_currentOrganization.FinancialAccounts.CostsBankFees, -row.FeeCents, _currentUser);
                            transaction.AddRow(autoDepositAccount, -row.AmountCentsGross, _currentUser);
                        }
                        else if (amountCents < autoDepositLimit * 100)
                        {
                            // Book against autoDeposit account.

                            transaction.AddRow(autoDepositAccount, -amountCents, _currentUser);
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

            Int64 beyondEofCents = assetAccount.GetDeltaCents(import.LatestTransaction.AddSeconds(1), DateTime.Now.AddDays(2)); // Caution: the "AddSeconds(1)" is not foolproof, there may be other new txs on the same second.

            if (databaseAccountBalanceCents - beyondEofCents == import.CurrentBalanceCents)
            {
                Payouts.AutomatchAgainstUnbalancedTransactions(_currentOrganization);
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




        // These Functions Copied From PWv4 -- may need adaptation and/or modernization


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
            if (lines[0].Trim() != "Date\t Time\t Time Zone\t Name\t Type\t Status\t Currency\t Gross\t Fee\t Net\t From Email Address\t To Email Address\t Transaction ID\t Counterparty Status\t Address Status\t Item Title\t Item ID\t Shipping and Handling Amount\t Insurance Amount\t Sales Tax\t Option 1 Name\t Option 1 Value\t Option 2 Name\t Option 2 Value\t Auction Site\t Buyer ID\t Item URL\t Closing Date\t Escrow Id\t Invoice Id\t Reference Txn ID\t Invoice Number\t Custom Number\t Receipt ID\t Balance\t Address Line 1\t Address Line 2/District/Neighborhood\t Town/City\t State/Province/Region/County/Territory/Prefecture/Republic\t Zip/Postal Code\t Country\t Contact Phone Number")
            //                       0      1      2           3      4      5        6          7       8     9     10                   11                 12               13                    14               15           16        17                             18                 19          20              21               22              23               24             25         26         27             28          29           30                 31               32              33           34        35               36                                     37          38                                                           39                          40
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
                    result.CurrentBalanceCents = Int64.Parse(StripQuotes(parts[34].Replace(".","").Replace(",","")), CultureInfo.InvariantCulture);
                }

                ImportedBankRow row = new ImportedBankRow();

                row.SuppliedTransactionId = StripQuotes(parts[12]);
                row.Comment = StripQuotes(parts[4]);
                row.DateTime = DateTime.Parse(StripQuotes(parts[0]) + " " + StripQuotes(parts[1]), CultureInfo.InvariantCulture);
                row.AmountCentsGross = Int64.Parse(StripQuotes(parts[7]).Replace(".", "").Replace(",", ""));
                row.FeeCents = Int64.Parse(StripQuotes(parts[8]).Replace(".", "").Replace(",", ""), CultureInfo.InvariantCulture);
                row.AmountCentsNet = Int64.Parse(StripQuotes(parts[9]).Replace(".", "").Replace(",", ""));

                // Adjust for timezone -- necessary for Paypal and other int'l services

                string timeZoneString = StripQuotes(parts[2]);

                if (timeZoneString != "PST" && timeZoneString != "PDT")  // this is cheating slightly as DSTs do not coincide
                {
                    throw new ArgumentException("Paypal import files should have time zone PST");
                }

                row.DateTime = row.DateTime.AddHours(8).ToLocalTime();

                rows.Add(row);

                if (row.DateTime < result.EarliestTransaction)
                {
                    result.EarliestTransaction = row.DateTime;
                }

                if (row.DateTime > result.LatestTransaction)
                {
                    result.LatestTransaction = row.DateTime;
                }
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