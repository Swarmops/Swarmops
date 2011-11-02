using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;

public partial class Pages_v4_ImportBankTransactions : PageV4Base
{

    private static readonly int PPOrgId = Organization.PPSEid;


    protected void Page_Load (object sender, EventArgs e)
    {
        // Identify person

        // HACK as to who has access to what data -- improve this later
        if (!IsPostBack)
        {
            if (_currentUser.Identity == 1)
            {
                this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString()));
                this.DropOrganizations.Items.Add(new ListItem("Sandbox", "3"));
                this.DropOrganizations.Items.Add(new ListItem("Rick's Sandbox", "55")); // Debug & Test
            }
            else if (_authority.HasPermission(Permission.CanDoEconomyTransactions, PPOrgId, -1, Authorization.Flag.ExactOrganization))
            {


                this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString() ));
            }
            else
            {
                // Show some dialog saying that the user has no access to the tools on this page
            }
        }

        // Styles

        this.TextData.Style[HtmlTextWriterStyle.Width] = "100%";
    }



    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Organization organization = Organization.FromIdentity(organizationId);

        // TODO: Security parsing
        this.DropAssetAccount.Items.Clear();
        this.DropAutoDeposits.Items.Clear();
        this.DropAutoWithdrawals.Items.Clear();

        if (organizationId == PPOrgId && _authority.HasPermission(Permission.CanDoEconomyTransactions, PPOrgId, -1, Authorization.Flag.ExactOrganization))
        {
            // TODO: Populate from a database table instead

            FinancialAccount assetAccount = organization.FinancialAccounts.AssetsBankAccountMain;
            FinancialAccount incomeAccount = organization.FinancialAccounts.IncomeDonations;
            FinancialAccount costAccount = organization.FinancialAccounts.CostsBankFees;

            this.DropAssetAccount.Items.Add(new ListItem(assetAccount.Name, assetAccount.Identity.ToString()));
            this.DropAssetAccount.Items.Add(new ListItem("Paypal", organization.FinancialAccounts.AssetsPaypal.Identity.ToString()));
            this.DropAutoDeposits.Items.Add(new ListItem(incomeAccount.Name, incomeAccount.Identity.ToString()));
            this.DropAutoWithdrawals.Items.Add(new ListItem(costAccount.Name, costAccount.Identity.ToString()));
            this.TextDepositLimit.Text = "1000";
            this.TextWithdrawalLimit.Text = "0";
        }

        else if (organizationId == 55)
        {
            // Rick's sandbox. Please leave in code for the time being.

            FinancialAccount assetAccount = FinancialAccount.FromIdentity(60);
            FinancialAccount incomeAccount = FinancialAccount.FromIdentity(63);
            FinancialAccount costAccount = FinancialAccount.FromIdentity(65);

            this.DropAssetAccount.Items.Add(new ListItem(assetAccount.Name, assetAccount.Identity.ToString()));
            this.DropAutoDeposits.Items.Add(new ListItem(incomeAccount.Name, incomeAccount.Identity.ToString()));
            this.DropAutoWithdrawals.Items.Add(new ListItem(costAccount.Name, costAccount.Identity.ToString()));
            this.TextDepositLimit.Text = "7500";
            this.TextWithdrawalLimit.Text = "0";
        }
    }

    protected void DropFilters_SelectedIndexChanged (object sender, EventArgs e)
    {
        switch (this.DropFilters.SelectedValue)
        {
            case "SEB.se":
                this.LabelFilterInstructions.Text =
                    "Go to the account overview and select a specific account. You will see 100 transactions per page. Select this entire web page and paste it into the area below, then click Process. You will probably need to repeat this procedure for several pages.";
                break;
            case "Paypal":
                this.LabelFilterInstructions.Text =
                    "Download the account history, all history, tab separated, for the specific time period, plus some overlap with previous history. Paste the entire contents of the downloaded file into the field below.";
                break;
            default:
                this.LabelFilterInstructions.Text =
                    "Select an import filter.";
                break;
        }
    }


    protected void ButtonProcessText_Click (object sender, EventArgs e)
    {
        _currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        // Process the contents of the paste area.
        ImportResult import = null;

        switch (this.DropFilters.SelectedValue)
        {
            case "SEB.se":
                import = ImportSebText(this.TextData.Text);
                break;
            case "Paypal":
                import = ImportPaypal(this.TextData.Text);
                break;
            default:
                throw new InvalidOperationException("Bad translation filter for this import");
                // TODO: Print error message and bail out
        }

        ProcessImportedData(import);
        this.TextData.Text = string.Empty;
    }

    protected void ProcessImportedData (ImportResult import)
    {

        FinancialAccount assetAccount = FinancialAccount.FromIdentity(Int32.Parse(this.DropAssetAccount.SelectedValue));
        FinancialAccount autoDepositAccount =
            FinancialAccount.FromIdentity(Int32.Parse(this.DropAutoDeposits.SelectedValue));
        FinancialAccount autoWithdrawalAccount =
            FinancialAccount.FromIdentity(Int32.Parse(this.DropAutoWithdrawals.SelectedValue));
        int autoDepositLimit = Int32.Parse(this.TextDepositLimit.Text);
        int autoWithdrawalLimit = Int32.Parse(this.TextWithdrawalLimit.Text);
        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Organization organization = Organization.FromIdentity(organizationId);

        int importedTransactionCount = 0;

        foreach (ImportedRow row in import.Rows)
        {
            // Each row is at least a stub, probably more.

            // If too old, ignore.

            if (row.DateTime < new DateTime(2008, 12, 4))
            {
                continue;
            }

            string importKey = row.SuppliedTransactionId;

            // If importKey is empty, construct a hash from the data fields.

            if (string.IsNullOrEmpty(importKey))
            {
                string hashKey = row.HashBase + row.Comment + (row.AmountCentsNet/100.0).ToString(CultureInfo.InvariantCulture) + row.CurrentBalance.ToString(CultureInfo.InvariantCulture) +
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

            FinancialTransaction transaction = FinancialTransaction.ImportWithStub(organizationId, row.DateTime,
                                                                                   assetAccount.Identity, amountCents,
                                                                                   row.Comment, importKey,
                                                                                   _currentUser.Identity);

            if (transaction != null)
            {
                // The transaction was created. Examine if the autobook criteria are true.

                importedTransactionCount++;

                FinancialAccounts accounts = FinancialAccounts.FromBankTransactionTag(row.Comment);

                if (accounts.Count == 1)
                {
                    // This is a labelled local donation.

                    Geography geography = accounts[0].AssignedGeography;
                    FinancialAccount localAccount = accounts[0];

                    transaction.AddRow(organization.FinancialAccounts.IncomeDonations, -amountCents, _currentUser);
                    transaction.AddRow(organization.FinancialAccounts.CostsLocalDonationTransfers,
                                       amountCents, _currentUser);
                    transaction.AddRow(localAccount, -amountCents, _currentUser);

                    Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.LocalDonationReceived,
                                                                 _currentUser.Identity, organizationId,
                                                                 geography.Identity, 0,
                                                                 transaction.Identity, localAccount.Identity.ToString());
                }
                else if (row.Comment.ToLowerInvariant().StartsWith("bg 451-0061 "))   // TODO: Organization.Parameters.FinancialTrackedTransactionPrefix
                {
                    // Check for previously imported payment group

                    PaymentGroup group = PaymentGroup.FromTag(organization,
                                                              "SEBGM" + DateTime.Today.Year.ToString() +   // TODO: Get tagging from org
                                                              row.Comment.Substring(11));

                    if (group != null)
                    {
                        // There was a previously imported and not yet closed payment group matching this transaction
                        // Close the payment group and match the transaction against accounts receivable

                        transaction.Dependency = group;
                        group.Open = false;
                        transaction.AddRow(organization.FinancialAccounts.AssetsOutboundInvoices, -amountCents, _currentUser);
                    }
                }
                else if (amountCents < 0)
                {
                    if ((-amountCents) < autoWithdrawalLimit * 100)
                    {
                        // Book against autoWithdrawal account.
                        transaction.AddRow(autoWithdrawalAccount, -amountCents, _currentUser);
                    }
                }
                else if (amountCents > 0)
                {
                    if (row.Fee < 0)
                    {
                        // This is always an autodeposit, if there is a fee (which is never > 0.0)

                        transaction.AddRow(organization.FinancialAccounts.CostsBankFees, -row.Fee, _currentUser);
                        transaction.AddRow(autoDepositAccount, -row.AmountCentsGross, _currentUser);
                    }
                    else if (amountCents < autoDepositLimit * 100)
                    {
                        // Book against autoDeposit account.

                        transaction.AddRow(autoDepositAccount, -amountCents, _currentUser);
                    }
                }
            }
        }

        // Import complete. Examine if we expect more transactions -- if the imported balance differs from
        // the database balance:

        double databaseAccountBalance = assetAccount.BalanceTotal;

        bool mismatch = false;

        if (databaseAccountBalance != import.CurrentBalance)
        {
            mismatch = true;
        }

        string message = importedTransactionCount.ToString() + " transactions were imported.";

        if (importedTransactionCount == 0)
        {
            message = "No transactions were imported. ";
        }
        else if (importedTransactionCount == 1)
        {
            message = "One transaction was imported. ";
        }

        if (import.CurrentBalance > 0)
        {
            if (mismatch)
            {
                message += " Transactions are missing from the database. Import more transactions.";
            }
            else
            {
                message += " The account balance is up to date. No further import is necessary.";

                ScriptManager.RegisterStartupScript(this, Page.GetType(), "alldone",
                                                    "alert ('The account balance is up to date. No further import is necessary.');",
                                                    true);

                // Auto-match open payouts against new transactions

                Payouts.AutomatchAgainstUnbalancedTransactions(organization);
            }
        }

        this.LabelImportResultText.Text = message;
    }


    protected ImportResult ImportSeb (string contents)
    {
        string patternRows =
            @"<tr class=""[a-z]+?"">\s*<td style=""white-space: nowrap;"">(?<datetime>[0-9\-]+)</td>\s*<td style=""white-space: nowrap;"">[0-9\-]+</td>\s*<td>(?<transactionid>.+?)</td>\s*<td>(?<comment>.*?)</td>\s*<td class=""numeric"">(?<linkdummy><a href="".*?"">)?.*?(?<amount>[0-9\.,\-]+).*?</td>\s*<td class=""numeric"">(?<balance>[0-9\.,\-]+)</td>\s*</tr>";
        string patternBalance =
            @"<tbody>\s*<tr class=""[a-z]+"">\s*<td>[0-9\s]+</td>\s*<td class=""numeric"">(?<balance>[0-9\.\-,]+)</td>\s*<td class=""numeric"">[0-9\.\-,]+</td>\s*<td class=""numeric"">[0-9\.\-,]+</td>\s*</tr>\s*</tbody>";

        List<ImportedRow> rows = new List<ImportedRow>();
        ImportResult result = new ImportResult();

        Regex regexBalance = new Regex(patternBalance, RegexOptions.Singleline);
        Regex regexRows = new Regex(patternRows, RegexOptions.Singleline | RegexOptions.Compiled);

        Match matchBalance = regexBalance.Match(contents);
        if (!matchBalance.Success)
        {
            throw new ArgumentException("Unable to find balance");
        }

        string stringBalance = matchBalance.Groups["balance"].Value.Replace(".", "").Replace(",", ".");
        result.CurrentBalance = Double.Parse(stringBalance, CultureInfo.InvariantCulture);

        Match matchRow = regexRows.Match(contents);
        while (matchRow.Success)
        {
            string amountString = matchRow.Groups["amount"].Value;
            amountString = amountString.Replace(".", "").Replace(",", "");

            ImportedRow row = new ImportedRow();
            row.DateTime = DateTime.Parse(matchRow.Groups["datetime"].Value);
            row.Comment = StripHtml(matchRow.Groups["comment"].Value);
            row.CurrentBalance = Double.Parse(matchRow.Groups["balance"].Value.Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture);
            row.AmountCentsNet = Int64.Parse(amountString);
            row.HashBase = matchRow.Groups["transactionid"].Value;

            rows.Add(row);

            matchRow = matchRow.NextMatch();
        }

        result.Rows = rows;

        if (rows.Count < 20 && rows.Count > 0)
        {
            // A serious error has occurred. Dev assistance is necessary.

            Person.FromIdentity(1).SendNotice("Contents for bank parsing (I see " + rows.Count.ToString() + " rows)", contents, 1);

            Person.FromIdentity(1).SendPhoneMessage("Bank import failed - " + rows.Count.ToString() + " rows parsed - see mail");

            throw new ArgumentException("PirateWeb is unable to parse the page. Developer assistance has been called in.");
        }
        return result;
    }



    protected ImportResult ImportSebText(string contents)
    {
        string patternRows =
            @"^\s(?<datetime>[0-9]{4}\-[0-9]{2}\-[0-9]{2})\s\t[0-9]{4}\-[0-9]{2}\-[0-9]{2}\s\t(?<transactionid>[^\s]+)\s\t(?<comment>[^\t]+)\t(?<amount>[\-,.0-9]+)\s\t(?<balance>[\-,.0-9]+)";
        string patternBalance =
            @"^[0-9\s]+\t[\-0-9\.\,]+\s\t(?<balance>[\-0-9\.\,]+)\s\t[\-0-9\.\,]+";

        List<ImportedRow> rows = new List<ImportedRow>();
        ImportResult result = new ImportResult();

        Regex regexBalance = new Regex(patternBalance, RegexOptions.Multiline | RegexOptions.Compiled);
        Regex regexRows = new Regex(patternRows, RegexOptions.Multiline | RegexOptions.Compiled);

        Match matchBalance = regexBalance.Match(contents);
        if (!matchBalance.Success)
        {
            throw new ArgumentException("Unable to find balance");
        }

        string stringBalance = matchBalance.Groups["balance"].Value.Replace(".", "").Replace(",", ".");
        result.CurrentBalance = Double.Parse(stringBalance, CultureInfo.InvariantCulture);

        Match matchRow = regexRows.Match(contents);
        while (matchRow.Success)
        {
            string amountString = matchRow.Groups["amount"].Value;
            amountString = amountString.Replace(".", "").Replace(",", "");

            ImportedRow row = new ImportedRow();
            row.DateTime = DateTime.Parse(matchRow.Groups["datetime"].Value);
            row.Comment = matchRow.Groups["comment"].Value.Trim();
            row.CurrentBalance = Double.Parse(matchRow.Groups["balance"].Value.Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture);
            row.AmountCentsNet = Int64.Parse(amountString);
            row.HashBase = matchRow.Groups["transactionid"].Value;

            rows.Add(row);

            matchRow = matchRow.NextMatch();
        }

        result.Rows = rows;

        if (rows.Count < 100 && rows.Count > 0)
        {
            // A serious error has occurred. Dev assistance is necessary.

            Person.FromIdentity(1).SendNotice("Contents for bank parsing (I see " + rows.Count.ToString() + " rows)\r\n\r\n", contents, 1);

            Person.FromIdentity(1).SendPhoneMessage("Bank import failed - " + rows.Count.ToString() + " rows parsed - see mail");

            throw new ArgumentException("PirateWeb is unable to parse the page. Developer assistance has been called in.");
        }
        return result;
    }


    protected ImportResult ImportPaypal (string contents)
    {
        string[] lines = contents.Split('\n');
        ImportResult result = new ImportResult();
        List<ImportedRow> rows = new List<ImportedRow>();

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

            if (result.CurrentBalance == 0.0)
            {
                result.CurrentBalance = Double.Parse(StripQuotes(parts[34]), CultureInfo.InvariantCulture);
            }

            ImportedRow row = new ImportedRow();

            row.SuppliedTransactionId = StripQuotes(parts[12]);
            row.Comment = StripQuotes(parts[4]);
            row.DateTime = DateTime.Parse(StripQuotes(parts[0]) + " " + StripQuotes(parts[1]), CultureInfo.InvariantCulture);
            row.AmountCentsGross = Int64.Parse(StripQuotes(parts[7]).Replace(".", ""));
            row.Fee = Double.Parse(StripQuotes(parts[8]), CultureInfo.InvariantCulture);
            row.AmountCentsNet = Int64.Parse(StripQuotes(parts[9]).Replace(".", ""));

            rows.Add(row);
        }

        result.Rows = rows;
        return result;
    }


    protected class ImportResult
    {
        public double CurrentBalance;
        public List<ImportedRow> Rows;
    }

    protected class ImportedRow
    {
        public DateTime DateTime;
        public string HashBase;
        public string SuppliedTransactionId;
        public double CurrentBalance;
        public Int64 AmountCentsNet;
        public Int64 AmountCentsGross;
        public double Fee;
        public string Comment;
    } ;


    protected string StripHtml (string input)
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

    protected string StripQuotes (string input)
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