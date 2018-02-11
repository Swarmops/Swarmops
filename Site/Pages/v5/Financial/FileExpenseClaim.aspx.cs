using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using Swarmops.Interface.Support;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.BackendServices;
using Swarmops.Logic.Swarm;
using Swarmops.Site.Pages.Ledgers;
using CsvHelper;


namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class FileExpenseClaim : PageV5Base
    {
        private int[] _tagSetIds;

        protected void Page_Init (object sender, EventArgs e)
        {
            string tagSetIdsString = Request["ctl00$PlaceHolderMain$HiddenTagSetIdentifiers"];

            // Find our tag ids, either from previously hidden var or load from org

            if (String.IsNullOrEmpty (tagSetIdsString))
            {
                this._tagSetIds = FinancialTransactionTagSets.ForOrganization (CurrentOrganization).Identities;
            }
            else
            {
                string[] tagSetIdStrings = tagSetIdsString.Split (',');
                this._tagSetIds = new int[tagSetIdStrings.Length];

                for (int index = 0; index < tagSetIdStrings.Length; index++)
                {
                    this._tagSetIds[index] = Int32.Parse (tagSetIdStrings[index]);
                }
            }

            // Construct data source

            List<TagSetDataSourceItem> dataSourceVisibleTags = new List<TagSetDataSourceItem>();
            List<TagSetDataSourceItem> dataSourceForcedTags = new List<TagSetDataSourceItem>();

            foreach (int tagSetId in this._tagSetIds)
            {
                TagSetDataSourceItem item = new TagSetDataSourceItem
                {
                    TagSetId = tagSetId,
                    TagSetLocalizedName =
                        FinancialTransactionTagSetType.GetLocalizedName (
                            FinancialTransactionTagSet.FromIdentity (tagSetId).
                                FinancialTransactionTagSetTypeId)
                };

                FinancialTransactionTagSet tagSet = FinancialTransactionTagSet.FromIdentity (tagSetId);

                if (tagSet.VisibilityLevel <= 1)
                {
                    dataSourceVisibleTags.Add (item);

                    if (!tagSet.AllowUntagged)
                    {
                        dataSourceForcedTags.Add (item);
                    }
                }
            }

            // Bind data

            // Unused for now

            /*
            this.RepeaterTagLabels.DataSource = dataSourceVisibleTags;
            this.RepeaterTagDrop.DataSource = dataSourceVisibleTags;
            this.RepeaterTagDropScript.DataSource = dataSourceVisibleTags;
            this.RepeaterErrorCheckTags.DataSource = dataSourceForcedTags;

            this.RepeaterTagLabels.DataBind();
            this.RepeaterTagDrop.DataBind();
            this.RepeaterTagDropScript.DataBind();
            this.RepeaterErrorCheckTags.DataBind();*/

            // Write set list back to hidden variable

            List<string> tagSetIdStringList = new List<string>();

            foreach (int tagSetId in this._tagSetIds)
            {
                tagSetIdStringList.Add (tagSetId.ToString (CultureInfo.InvariantCulture));
            }

            this.HiddenTagSetIdentifiers.Value = String.Join (",", tagSetIdStringList.ToArray());
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.BoxTitle.Text = PageTitle = Resources.Pages.Financial.FileExpenseClaim_PageTitle;
            PageIcon = "iconshock-moneyback";
            InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Participant, AccessType.Write);

            if (!Page.IsPostBack)
            {
                // Prime bank details

                this.TextBank.Text = CurrentUser.BankName;
                this.TextClearing.Text = CurrentUser.BankClearing;
                this.TextAccount.Text = CurrentUser.BankAccount;
                this.CurrencyAmount.Cents = 0;
                this.CurrencyVat.Cents = 0;
                this.CurrencyAmount.Focus();

                Localize();
            }
        }


        private void Localize()
        {
            /* Main tab */

            this.LabelAmount.Text = string.Format (Resources.Pages.Financial.FileExpenseClaim_Amount,
                CurrentOrganization.Currency.DisplayCode);
            this.LabelPurpose.Text = Resources.Pages.Financial.FileExpenseClaim_Description;
            this.LabelBudget.Text = Resources.Pages.Financial.FileExpenseClaim_Budget;
            this.LabelHeaderBankDetails.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderBankDetails;
            this.LabelBankName.Text = Resources.Pages.Financial.FileExpenseClaim_BankName;
            this.LabelBankClearing.Text = Resources.Pages.Financial.FileExpenseClaim_BankClearing;
            this.LabelBankAccount.Text = Resources.Pages.Financial.FileExpenseClaim_BankAccount;
            this.LabelHeaderImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderReceiptImages;
            this.LabelImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_UploadRecieptImages;
            this.LabelVat.Text = string.Format(Resources.Pages.Financial.FileExpenseClaim_Vat,
                CurrentOrganization.Currency.DisplayCode);

            this.ButtonRequest.Text = Resources.Pages.Financial.FileExpenseClaim_ButtonRequest;

            /* Expensify tab */

            this.LabelExpensifyUploadHeader.Text = Resources.Pages.Financial.FileExpenseClaim_Title_Expensify;
            this.LabelExpensifyCsv.Text = Resources.Pages.Financial.FileExpenseClaim_Expensify_CsvUploadDescription;
            this.LabelExpensifyInstructions1.Text =
                Resources.Pages.Financial.FileExpenseClaim_Expensify_InstructionsBasic;

            if (CurrentOrganization.VatEnabled)
            {
                this.LabelExpensifyInstructions2.Text =
                    String.Format(Resources.Pages.Financial.FileExpenseClaim_Expensify_InstructionsNeedVat, CurrentOrganization.Name);
            }
            else
            {
                this.LabelExpensifyInstructions2.Text =
                    Resources.Pages.Financial.FileExpenseClaim_Expensify_InstructionsNothingAdvanced;
            }
        }


        [WebMethod(true)]
        public static AjaxCallResult InitializeExpensifyProcessing(string guidFiles, string guidProgress)
        {
            // Start an async thread that does all the work, then return

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            Thread initThread = new Thread(ProcessExpensifyUploadThread);

            ProcessThreadArguments args = new ProcessThreadArguments
            {
                GuidFiles = guidFiles,
                GuidProgress = guidProgress,
                Organization = authData.CurrentOrganization,
                CurrentUser = authData.CurrentUser
            };

            initThread.Start(args);

            return new AjaxCallResult {Success = true};
        }


        private class ProcessThreadArguments
        {
            public string GuidFiles { get; set; }
            public string GuidProgress { get; set; }
            public Organization Organization { get; set; }
            public Person CurrentUser { get; set; }
        }

        public class ReportedImportResults
        {
            public string Category { get; set; }
            public string Html { get; set; }
        }


        private static void ProcessExpensifyUploadThread(object args)
        {
            string guidProgress = ((ProcessThreadArguments) args).GuidProgress;
            string guidFiles = ((ProcessThreadArguments)args).GuidFiles;
            Person currentUser = ((ProcessThreadArguments) args).CurrentUser;
            Organization organization = ((ProcessThreadArguments) args).Organization;

            Documents documents = Documents.RecentFromDescription(guidFiles);
            GuidCache.SetProgress(guidProgress, 1); // to make sure results aren't repeated from last file
            GuidCache.Set(guidProgress + "-UploadResult", UploadBankFiles.ImportResultsCategory.Bad);
            // default - this is what happens if exception

            if (documents.Count != 1)
            {
                return; // abort
            }

            Document uploadedDoc = documents[0];

            // TODO: ATTEMPT TO DETERMINE CURRENCY FROM FILE, USING ORIGINAL CURRENCY + ORIGINAL AMOUNT

            string csvEntire;

            using (StreamReader reader = uploadedDoc.GetReader(1252))
            {
                csvEntire = reader.ReadToEnd();
            }

            string[] csvLines = csvEntire.Split(new char[] {'\r','\n'});
            string[] fieldNames = csvLines[0].Split(',');

            // Map fields to column indexes

            Dictionary<ExpensifyColumns,int> fieldMap = new Dictionary<ExpensifyColumns, int>();

            for (int loop = 0; loop < fieldNames.Length; loop++)
            {
                switch (fieldNames[loop].ToLowerInvariant().Trim('\"'))
                {
                    case "timestamp":
                        fieldMap[ExpensifyColumns.Timestamp] = loop;
                        break;
                    case "amount":
                        fieldMap[ExpensifyColumns.AmountFloat] = loop;
                        break;
                    case "merchant":
                        fieldMap[ExpensifyColumns.Merchant] = loop;
                        break;
                    case "comment":
                        fieldMap[ExpensifyColumns.Comment] = loop;
                        break;
                    case "category":
                        fieldMap[ExpensifyColumns.CategoryCustom] = loop;
                        break;
                    case "mcc":
                        fieldMap[ExpensifyColumns.CategoryStandard] = loop;
                        break;
                    case "vat":
                        fieldMap[ExpensifyColumns.VatFloat] = loop;
                        break;
                    case "original currency":
                        fieldMap[ExpensifyColumns.OriginalCurrency] = loop;
                        break;
                    case "original amount":
                        fieldMap[ExpensifyColumns.OriginalCurrencyAmountFloat] = loop;
                        break;
                    case "receipt":
                        fieldMap[ExpensifyColumns.ReceiptUrl] = loop;
                        break;
                    default:
                        // ignore any unknown fields
                        break;
                }
            }

            // TODO: Much more general-case error conditions if not all fields are filled

            bool vatEnabled = organization.VatEnabled;

            if (vatEnabled && !fieldMap.ContainsKey(ExpensifyColumns.VatFloat))
            {
                // Error: Organization needs a VAT field
                // TODO
                // Set result to bad
                // Set progress to complete
                // Abort

                GuidCache.Set(guidProgress, 100); // terminate progress bar
                throw new BarfException();
            }

            List<ExpensifyRecord> recordList = new List<ExpensifyRecord>();

            CsvHelper.Configuration.Configuration config = new CsvHelper.Configuration.Configuration();
            config.HasHeaderRecord = true;

            using (TextReader textReader = new StringReader(csvEntire))
            {
                CsvReader csvReader = new CsvReader(textReader, config);
                csvReader.Read(); // bypass header record -- why isn't this done automatically?

                while (csvReader.Read())
                {
                    ExpensifyRecord newRecord = new ExpensifyRecord();
                    newRecord.AmountCents =
                        Formatting.ParseDoubleStringAsCents(csvReader.GetField(fieldMap[ExpensifyColumns.AmountFloat]),
                            CultureInfo.InvariantCulture);
                    newRecord.OriginalCurrency =
                        Currency.FromCode(csvReader.GetField(fieldMap[ExpensifyColumns.OriginalCurrency]));
                    newRecord.OriginalAmountCents =
                        Formatting.ParseDoubleStringAsCents(
                            csvReader.GetField(fieldMap[ExpensifyColumns.OriginalCurrencyAmountFloat]),
                            CultureInfo.InvariantCulture);

                    newRecord.Description = csvReader.GetField(fieldMap[ExpensifyColumns.Merchant]);

                    string comment = csvReader.GetField(fieldMap[ExpensifyColumns.Comment]).Trim();
                    if (!string.IsNullOrEmpty(comment))
                    {
                        newRecord.Description += " / " + comment;
                    }
                    newRecord.CategoryCustom = csvReader.GetField(fieldMap[ExpensifyColumns.CategoryCustom]);
                    newRecord.CategoryStandard = csvReader.GetField(fieldMap[ExpensifyColumns.CategoryStandard]);
                    newRecord.ReceiptUrl = csvReader.GetField(fieldMap[ExpensifyColumns.ReceiptUrl]);

                    newRecord.Timestamp = DateTime.Parse(csvReader.GetField(fieldMap[ExpensifyColumns.Timestamp]));

                    if (vatEnabled)
                    {
                        newRecord.VatCents =
                            Formatting.ParseDoubleStringAsCents(csvReader.GetField(fieldMap[ExpensifyColumns.VatFloat]),
                                CultureInfo.InvariantCulture);
                    }

                    recordList.Add(newRecord);
                }
            }

            // We now have a list of records. At this point in time, we need to determine what currency the
            // damn report is in, because that's not specified anywhere in the CSV (who thought this was a
            // good idea anyway?). We do this by iterating through the records and hoping there's at least
            // one record with the exact same amount in the report field as in the "original currency amount"
            // field, and then we guess that's the currency of the report. If we don't find one, or if
            // there are multiple candidates, we need to ask the user what currency the report is in.

            Currency reportCurrency = null;

            foreach (ExpensifyRecord record in recordList)
            {
                if (record.AmountCents == record.OriginalAmountCents)
                {
                    if (reportCurrency == null)
                    {
                        reportCurrency = record.OriginalCurrency;
                    }
                    else if (reportCurrency.Identity != record.OriginalCurrency.Identity)
                    {
                        throw new BarfException();  // TODO: ASK USER
                    }
                }
            }

            if (reportCurrency == null)
            {
                throw new BarfException();  // TODO: ASK USER
            }

            GuidCache.SetProgress(guidProgress, 10);

            // We now need to get all the receipt images. This is a little tricky as we don't have the URL
            // of the receipt directly, we only have the URL of a webpage that contains JavaScript code
            // to fetch the receipt image.

            // Get relative date part

            string relativePath = Document.DailyStorageFolder.Substring(Document.StorageRoot.Length);

            // Get all receipts

            for (int loop = 0; loop < recordList.Count; loop++)
            {
                GuidCache.SetProgress(guidProgress, loop * 90 / recordList.Count + 10);

                using (WebClient client = new WebClient())
                {
                    string receiptResource = client.DownloadString(recordList[loop].ReceiptUrl);

                    // We now have the web page which holds information about where the actual receipt is located.

                    Regex regex = new Regex(@"\s*var transaction\s*=\s*(?<jsonTxInfo>{.*});", RegexOptions.Multiline);
                    Match match = regex.Match(receiptResource);
                    if (match.Success)
                    {
                        string txInfoString = match.Groups["jsonTxInfo"].ToString();
                        JObject txInfo = JObject.Parse(txInfoString);
                        recordList[loop].ExtendedInfo = txInfoString;

                        string expensifyFileName = (string) txInfo["receiptFilename"];
                        string actualReceiptUrl = "https://s3.amazonaws.com/receipts.expensify.com/" + expensifyFileName;
                        string newGuidString = Guid.NewGuid().ToString();

                        string fullyQualifiedFileName = Document.DailyStorageFolder + newGuidString;
                        string relativeFileName = relativePath + newGuidString;

                        client.DownloadFile(actualReceiptUrl, fullyQualifiedFileName);
                        recordList[loop].ReceiptFileNameHere = newGuidString;

                        // If original file name ends in PDF, initiate conversion.

                        if (expensifyFileName.ToLowerInvariant().EndsWith(".pdf"))
                        {
                            // Convert low resolution

                            Documents docs = new PdfProcessor().RasterizeOne(fullyQualifiedFileName, recordList[loop].Description, newGuidString, currentUser, organization);

                            recordList[loop].Documents = docs;

                            // Ask backend for high-res conversion

                            RasterizeDocumentHiresOrder backendOrder =
                                new RasterizeDocumentHiresOrder(docs[0]);
                            backendOrder.Create();
                        }
                        else
                        {
                            Document doc = Document.Create(relativePath + newGuidString, expensifyFileName, 0, newGuidString, null,
                                currentUser);

                            recordList[loop].Documents = Documents.FromSingle(doc);
                        }
                    }
                }
            }



            // We now have the individual expenses and all accompanying receipts.
            // Create the expense claim group, then the individual expense records,
            // and assign the Documents to the records and the records to the Group,
            // so the user can review all of it.



            // TODO: Get all receipts

            // TODO: Present report to user

            // TODO: Let user confirm budgets

            // TODO: Once user has confirmed budgets, save expenses

        }

        public class BarfException: Exception {}


        private enum ExpensifyColumns
        {
            Unknown = 0,
            Timestamp,
            AmountFloat,
            Merchant,
            Comment,
            CategoryCustom,
            CategoryStandard,
            VatFloat,
            OriginalCurrency,
            OriginalCurrencyAmountFloat,
            ReceiptUrl
        }

        private class ExpensifyRecord
        {
            public Int64 AmountCents { get; set; }
            public Int64 VatCents { get; set; }
            public string Description { get; set; }
            public string CategoryCustom { get; set; }
            public string CategoryStandard { get; set; }
            public Currency OriginalCurrency { get; set; }
            public Int64 OriginalAmountCents { get; set; }
            public string ReceiptUrl { get; set; }
            public DateTime Timestamp { get; set; }
            public string ExtendedInfo { get; set; }
            public string ReceiptFileNameHere { get; set; }
            public Documents Documents { get; set; }
        }


        // Timestamp,Merchant,Amount,MCC,Category,Tag,Comment,Reimbursable,"Original Currency","Original Amount",Receipt

        protected void ButtonRequest_Click (object sender, EventArgs e)
        {
            // The data has been validated client-side already. We'll throw unfriendly exceptions if invalid data is passed here.
            // People who choose to disable JavaScript and then submit bad input almost deserve to be hurt.

            Int64 amountCents = this.CurrencyAmount.Cents;
            Int64 vatCents = this.CurrencyVat.Cents;

            string description = this.TextPurpose.Text;

            FinancialAccount budget = FinancialAccount.FromIdentity (Int32.Parse (Request.Form["DropBudgets"]));

            // sanity check

            if (budget.Organization.Identity != CurrentOrganization.Identity)
            {
                throw new InvalidOperationException ("Budget-organization mismatch; won't file expense claim");
            }

            // Store bank details for current user

            CurrentUser.BankName = this.TextBank.Text;
            CurrentUser.BankClearing = this.TextClearing.Text;
            CurrentUser.BankAccount = this.TextAccount.Text;

            // Get documents; check that documents have been uploaded

            Documents documents = Documents.RecentFromDescription (this.FileUpload.GuidString);

            if (documents.Count == 0)
            {
                throw new InvalidOperationException ("No documents uploaded");
            }

            ExpenseClaim claim = ExpenseClaim.Create (CurrentUser, CurrentOrganization, budget, DateTime.UtcNow,
                description, amountCents, vatCents);

            foreach (int tagSetId in this._tagSetIds)
            {
                string selectedTagString =
                    Request.Form["DropTags" + tagSetId.ToString (CultureInfo.InvariantCulture)];

                if (!String.IsNullOrEmpty (selectedTagString))
                {
                    int selectedTagType = Int32.Parse (selectedTagString);
                    if (selectedTagType != 0)
                    {
                        claim.FinancialTransaction.CreateTag (
                            FinancialTransactionTagType.FromIdentity (selectedTagType),
                            CurrentUser);
                    }
                }
            }

            documents.SetForeignObjectForAll (claim);

            string successMessage = string.Format (Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartOne,
                CurrentOrganization.Currency.Code,
                amountCents/100.0,
                budget.Name);

            if (budget.OwnerPersonId != CurrentUser.Identity)
            {
                successMessage += "<br/><br/>" + Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartTwo +
                                  "<br/>";
            }
            else
            {
                successMessage += "<br/><br/>" +
                                  Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartTwoOwnBudget +
                                  "<br/>";
                claim.Attest (CurrentUser);
            }

            DashboardMessage.Set (successMessage);

            // Redirect to dashboard

            Response.Redirect ("/", true);
        }


        protected class TagSetDataSourceItem
        {
            public int TagSetId { get; set; }
            public string TagSetLocalizedName { get; set; }
        }


        // ASPX localizations


        // ReSharper disable InconsistentNaming
        public string Localized_ValidationError_MissingTag
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_MissingTag); }
        }

        public string Localized_ValidationError_BankAccount
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankAccount); }
        }

        public string Localized_ValidationError_BankClearing
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankClearing); }
        }

        public string Localized_ValidationError_BankName
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankName); }
        }

        public string Localized_ValidationError_Purpose
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Purpose); }
        }

        public string Localized_ValidationError_Budget
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_Budget); }
        }

        public string Localized_ValidationError_Amount
        {
            get { return JavascriptEscape(String.Format(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Amount, CurrentOrganization.Currency.DisplayCode)); }
        }

        public string Localized_ValidationError_Documents
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Documents); }
        }

    }
}