using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class ResyncExternalAccount : PageV5Base
    {
        private static readonly Dictionary<string, object> _staticDataLookup;

        static ResyncExternalAccount()
        {
            _staticDataLookup = new Dictionary<string, object>();
            // This does not work at all for webfarms, which may be a problem down the road
        }

        public static string StorageRoot
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return @"C:\Windows\Temp\Swarmops-Debug\"; // Windows debugging environment
                }
                return "/var/lib/swarmops/upload/"; // production location on Debian installation  TODO: config file
            }
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageIcon = "iconshock-treasure";
            PageTitle = Resources.Pages.Ledgers.ResyncExternalAccount_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.ResyncExternalAccount_Info;

            if (!Page.IsPostBack)
            {
                this.DropAccounts.Items.Add (new ListItem (Global.Global_SelectOne, "0"));

                FinancialAccounts accounts = FinancialAccounts.ForOrganization (CurrentOrganization,
                    FinancialAccountType.Asset);

                foreach (FinancialAccount account in accounts)
                {
                    // TODO: COnstruct import settings for accounts

                    if (CurrentOrganization.Name == "Piratpartiet SE" && account.Identity == 1)
                    {
                        this.DropAccounts.Items.Add (new ListItem (account.Name + " (SE/SEB)",
                            account.Identity.ToString (CultureInfo.InvariantCulture)));
                    }
                }


                Localize();
            }

            EasyUIControlsUsed = EasyUIControl.DataGrid | EasyUIControl.Tree;
        }


        private void Localize()
        {
        }


        [WebMethod (EnableSession = true)]
        public static int GetProcessingProgress (string guid)
        {
            // Get data from static variables

            string key = guid + "PercentRead";

            if (!_staticDataLookup.ContainsKey (key))
            {
                return 0;
            }

            lock (_staticDataLookup)
            {
                int percentReady = (int) _staticDataLookup[key];

                if (percentReady >= 100)
                {
                    // copy result from static class-scope variable to session-scope variable
                    HttpContext.Current.Session["LedgersResync" + guid + "MismatchArray"] =
                        _staticDataLookup[guid + "MismatchArray"];

                    HttpContext.Current.Session["LedgersResync" + guid + "Profile"] =
                        _staticDataLookup[guid + "Profile"];

                    HttpContext.Current.Session["LedgersResync" + guid + "Account"] =
                        _staticDataLookup[guid + "Account"];

                    _staticDataLookup[guid + "MismatchArray"] = null;
                    // clear the static object, which will otherwise live on
                    _staticDataLookup[guid + "Profile"] = null;

                    // TODO: Clear all _staticDataLookup starting with guid (leaks memory slightly)
                }

                return percentReady;
            }
        }

        [WebMethod (true)]
        public static ExternalBankDataStatistics GetProcessingStatistics (string guid)
        {
            // Get data from static variables

            string key = guid + "PercentRead";

            if (!_staticDataLookup.ContainsKey (key))
            {
                return null;
            }

            ExternalBankDataStatistics result = new ExternalBankDataStatistics();

            result.FirstTransaction = (string) _staticDataLookup[guid + "FirstTx"];
            result.LastTransaction = (string) _staticDataLookup[guid + "LastTx"];
            result.TransactionCount = (string) _staticDataLookup[guid + "TxCount"];

            return result;
        }


        [WebMethod (true)]
        public static void InitializeProcessing (string guid, string accountIdString)
        {
            // Start an async thread that does all the work, then return

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            int accountId = Int32.Parse (accountIdString);
            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            if (account.Organization.Identity != authData.CurrentOrganization.Identity ||
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization, AccessAspect.Bookkeeping,
                    AccessType.Write)))
            {
                throw new UnauthorizedAccessException();
            }

            Thread initThread = new Thread (ProcessUploadThread);

            ProcessThreadArguments args = new ProcessThreadArguments
            {Guid = guid, Organization = authData.CurrentOrganization, Account = account};

            initThread.Start (args);
        }


        [WebMethod (EnableSession = true)]
        public static ResyncResults ExecuteResync (string guid)
        {
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture();

            if (
                !authenticationData.CurrentUser.HasAccess (new Access (authenticationData.CurrentOrganization,
                    AccessAspect.Bookkeeping, AccessType.Write)))
            {
                throw new UnauthorizedAccessException();
            }

            ResyncResults results = new ResyncResults();

            ExternalBankMismatchingDateTime[] mismatchArray =
                (ExternalBankMismatchingDateTime[])
                    HttpContext.Current.Session["LedgersResync" + guid + "MismatchArray"];

            FinancialAccount account =
                (FinancialAccount) HttpContext.Current.Session["LedgersResync" + guid + "Account"];

            long autoDepositDonationCents = 1000*100;
            FinancialAccount autoDonationAccount = account.Organization.FinancialAccounts.IncomeDonations;

            if (authenticationData.CurrentOrganization.Identity != account.OrganizationId)
            {
                throw new InvalidOperationException ("Mismatching org");
            }

            foreach (ExternalBankMismatchingDateTime mismatchDateTime in mismatchArray)
            {
                foreach (
                    ExternalBankMismatchingRecordDescription mismatchingRecord in mismatchDateTime.MismatchingRecords)
                {
                    for (int index = 0; index < mismatchingRecord.MasterCents.Length; index++)
                    {
                        results.RecordsTotal++;
                        long cents = mismatchingRecord.MasterCents[index];

                        bool unhandled = false;
                        bool handlable = true;

                        FinancialTransaction tx = mismatchingRecord.Transactions[index];

                        if (tx != null && tx.Dependency != null)
                        {
                            unhandled = true;

                            IHasIdentity dependency = tx.Dependency;

                            if (dependency is PaymentGroup &&
                                mismatchingRecord.ResyncActions[index] ==
                                ExternalBankMismatchResyncAction.RewriteSwarmops)
                            {
                                if (cents == (dependency as PaymentGroup).SumCents)
                                {
                                    // Amount checks out with dependency; rewrite requested; this is handlable on auto

                                    Dictionary<int, long> newTx = new Dictionary<int, long>();
                                    newTx[account.Identity] = cents;
                                    newTx[account.Organization.FinancialAccounts.AssetsOutboundInvoices.Identity] =
                                        -cents;
                                    tx.RecalculateTransaction (newTx, authenticationData.CurrentUser);
                                    unhandled = false;
                                }
                            }


                            handlable = false; // need to fix this
                        }


                        if (handlable)
                            switch (mismatchingRecord.ResyncActions[index])
                            {
                                case ExternalBankMismatchResyncAction.DeleteSwarmops:
                                    if (tx == null)
                                    {
                                        throw new InvalidOperationException (
                                            "Can't have Delete op on a null transaction");
                                    }

                                    tx.Description = tx.Description + " (killed/zeroed in resync)";
                                    tx.RecalculateTransaction (new Dictionary<int, long>(),
                                        authenticationData.CurrentUser); // zeroes out
                                    break;
                                case ExternalBankMismatchResyncAction.RewriteSwarmops:
                                    if (tx == null)
                                    {
                                        throw new InvalidOperationException (
                                            "Can't have Rewrite op on a null transaction");
                                    }

                                    Dictionary<int, long> newTx = new Dictionary<int, long>();
                                    newTx[account.Identity] = cents;
                                    if (cents > 0 && cents < autoDepositDonationCents)
                                    {
                                        newTx[autoDonationAccount.Identity] = -cents; // negative; P&L account
                                    }
                                    tx.RecalculateTransaction (newTx, authenticationData.CurrentUser);
                                    break;
                                case ExternalBankMismatchResyncAction.CreateSwarmops:
                                    if (tx != null)
                                    {
                                        throw new InvalidOperationException ("Transaction seems to already exist");
                                    }

                                    tx = FinancialTransaction.Create (account.OwnerPersonId, mismatchDateTime.DateTime,
                                        mismatchingRecord.Description);
                                    tx.AddRow (account, cents, authenticationData.CurrentUser);

                                    if (cents > 0 && cents < autoDepositDonationCents)
                                    {
                                        tx.AddRow (autoDonationAccount, -cents, authenticationData.CurrentUser);
                                    }
                                    break;
                                case ExternalBankMismatchResyncAction.NoAction:
                                    // no action
                                    break;
                                default:
                                    // not handled
                                    unhandled = true;
                                    break;
                            }

                        if (unhandled)
                        {
                            results.RecordsFail++;
                        }
                        else
                        {
                            results.RecordsSuccess++;
                        }
                    }
                }
            }

            return results;
        }

        private static void ProcessUploadThread (object args)
        {
            string guid = ((ProcessThreadArguments) args).Guid;

            Documents documents = Documents.RecentFromDescription (guid);

            if (documents.Count != 1)
            {
                return; // abort
            }

            Document uploadedDoc = documents[0];

            FinancialAccount account = ((ProcessThreadArguments) args).Account;

            ExternalBankData externalData = new ExternalBankData();
            externalData.Profile = ExternalBankDataProfile.FromIdentity (ExternalBankDataProfile.SESebId);
            // TODO: HACK HACK HACK HACK LOAD 

            using (
                StreamReader reader = new StreamReader (StorageRoot + uploadedDoc.ServerFileName,
                    Encoding.GetEncoding (1252)))
            {
                externalData.LoadData (reader, ((ProcessThreadArguments) args).Organization);
            }

            _staticDataLookup[guid + "FirstTx"] = externalData.Records[0].DateTime.ToLongDateString();
            _staticDataLookup[guid + "LastTx"] =
                externalData.Records[externalData.Records.Length - 1].DateTime.ToLongDateString();
            _staticDataLookup[guid + "TxCount"] = externalData.Records.Length.ToString ("N0");

            _staticDataLookup[guid + "Profile"] = externalData.Profile;

            _staticDataLookup[guid + "PercentRead"] = 1;

            DateTime timeWalker = externalData.Records[0].DateTime;
            int currentRecordIndex = 0;

            // Walk past the first identical time records, and verify that we have at least one balance record that matches
            // our own for this timestamp. There may be several transactions in the master file, but at least one should have
            // a post-transaction balance that matches our records, or something is much more broken.

            long swarmopsCentsStart = account.GetDeltaCents (DateTime.MinValue, timeWalker.AddSeconds (1));

            // At least one of the transactions for this timestamp should match.

            bool foundMatch = false;

            while (externalData.Records[currentRecordIndex].DateTime == timeWalker)
            {
                if (externalData.Records[currentRecordIndex].AccountBalanceCents == swarmopsCentsStart)
                {
                    foundMatch = true;

                    // continue loop until on first record past initial conditions
                }
                currentRecordIndex++; // no test for array overrun in while
            }

            if (!foundMatch)
            {
                throw new InvalidOperationException ("Unable to locate stable initial conditions for resynchronization");
            }

            // From here on out, every new timestamp should have the exact same delta in Swarmops as it has in the master file.

            List<ExternalBankMismatchingDateTime> mismatchList = new List<ExternalBankMismatchingDateTime>();

            while (currentRecordIndex < externalData.Records.Length)
            {
                DateTime lastTimestamp = timeWalker;

                timeWalker = externalData.Records[currentRecordIndex].DateTime;

                long swarmopsDeltaCents = account.GetDeltaCents (lastTimestamp.AddSeconds (1), timeWalker.AddSeconds (1));
                // "AddSeconds" because DeltaCents operates on ">= lowbound, < highbound"
                int timestampStartIndex = currentRecordIndex;
                long masterDeltaCents = externalData.Records[currentRecordIndex++].TransactionNetCents;
                int masterTransactionCount = 1;

                while (currentRecordIndex < externalData.Records.Length &&
                       externalData.Records[currentRecordIndex].DateTime == timeWalker)
                {
                    masterDeltaCents += externalData.Records[currentRecordIndex++].TransactionNetCents;
                    masterTransactionCount++;
                }

                if (masterDeltaCents != swarmopsDeltaCents)
                {
                    // We have a mismatch. Add it to the list.

                    ExternalBankMismatchingDateTime newMismatch = new ExternalBankMismatchingDateTime();
                    newMismatch.DateTime = timeWalker;
                    newMismatch.MasterDeltaCents = masterDeltaCents;
                    newMismatch.MasterTransactionCount = masterTransactionCount;
                    newMismatch.SwarmopsDeltaCents = swarmopsDeltaCents;
                    newMismatch.SwarmopsTransactionCount = 0; // TODO

                    // Load transactions from both sources. First, create the interim construction object.

                    ExternalBankMismatchConstruction mismatchConstruction = new ExternalBankMismatchConstruction();

                    // Load from Master

                    for (int innerRecordIndex = timestampStartIndex;
                        innerRecordIndex < currentRecordIndex;
                        innerRecordIndex++)
                    {
                        string description = externalData.Records[innerRecordIndex].Description.Replace ("  ", " ");

                        if (!mismatchConstruction.Master.ContainsKey (description))
                        {
                            mismatchConstruction.Master[description] =
                                new ExternalBankMismatchingRecordConstruction();
                        }

                        mismatchConstruction.Master[description].Cents.Add (
                            externalData.Records[innerRecordIndex].TransactionNetCents);
                        mismatchConstruction.Master[description].Transactions.Add (null);
                        // no dependencies on the master side, only on swarmops side
                    }

                    // Load from Swarmops

                    FinancialAccountRows swarmopsTransactionRows =
                        account.GetRowsFar (lastTimestamp, timeWalker);
                    // the "select far" is a boundary < x <= boundary selector. Default is boundary <= x < boundary.

                    Dictionary<int, FinancialTransaction> lookupTransactions =
                        new Dictionary<int, FinancialTransaction>();

                    // note all transaction IDs, then sum up per transaction

                    foreach (FinancialAccountRow swarmopsTransactionRow in swarmopsTransactionRows)
                    {
                        lookupTransactions[swarmopsTransactionRow.FinancialTransactionId] =
                            swarmopsTransactionRow.Transaction;
                    }


                    foreach (FinancialTransaction transaction in lookupTransactions.Values)
                    {
                        string description = transaction.Description.Replace ("  ", " ");
                        // for legacy compatibility with new importer

                        if (!mismatchConstruction.Swarmops.ContainsKey (description))
                        {
                            mismatchConstruction.Swarmops[description] =
                                new ExternalBankMismatchingRecordConstruction();
                        }

                        long cents = transaction[account];

                        if (cents != 0) // only add nonzero records
                        {
                            mismatchConstruction.Swarmops[description].Cents.Add (transaction[account]);
                            mismatchConstruction.Swarmops[description].Transactions.Add (transaction);
                        }
                    }

                    // Then, parse the intermediate construction object to the presentation-and-action object.

                    Dictionary<string, ExternalBankMismatchingRecordDescription> mismatchingRecordList =
                        new Dictionary<string, ExternalBankMismatchingRecordDescription>();

                    foreach (string masterKey in mismatchConstruction.Master.Keys)
                    {
                        Dictionary<int, bool> checkMasterIndex = new Dictionary<int, bool>();
                        Dictionary<int, bool> checkSwarmopsIndex = new Dictionary<int, bool>();

                        // For each key and entry for each key;

                        // 1) locate an exact corresponding amount in swarmops records and log; failing that,
                        // 2) if exactly one record left in master and swarmops records, log; failing that,
                        // 3) log the rest of the master OR rest of swarmops records with no corresponding
                        //    equivalent with counterpart. (May produce bad results if 2 consistent mismatches
                        //    for every description.)

                        ExternalBankMismatchingRecordDescription newRecord =
                            new ExternalBankMismatchingRecordDescription();
                        newRecord.Description = masterKey;

                        List<long> masterCentsList = new List<long>();
                        List<long> swarmopsCentsList = new List<long>();
                        List<object> dependenciesList = new List<object>();
                        List<FinancialTransaction> transactionsList = new List<FinancialTransaction>();
                        List<ExternalBankMismatchResyncAction> actionsList =
                            new List<ExternalBankMismatchResyncAction>();

                        // STEP 1 - locate all identical matches

                        if (mismatchConstruction.Swarmops.ContainsKey (masterKey))
                        {
                            for (int masterIndex = 0;
                                masterIndex < mismatchConstruction.Master[masterKey].Cents.Count;
                                masterIndex++)
                            {
                                // no "continue" necessary on first run-through; nothing has been checked off yet

                                long findMasterCents = mismatchConstruction.Master[masterKey].Cents[masterIndex];

                                for (int swarmopsIndex = 0;
                                    swarmopsIndex < mismatchConstruction.Swarmops[masterKey].Cents.Count;
                                    swarmopsIndex++)
                                {
                                    if (checkSwarmopsIndex.ContainsKey (swarmopsIndex))
                                    {
                                        continue;
                                        // may have been checked off already in the rare case of twin identical amounts
                                    }

                                    if (findMasterCents == mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex])
                                    {
                                        // There is a match as per case 1. Record both, mark both as used, continue.

                                        masterCentsList.Add (findMasterCents);
                                        swarmopsCentsList.Add (
                                            mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex]);
                                        // should be equal, we're defensive here
                                        transactionsList.Add (
                                            mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex]);
                                        dependenciesList.Add (
                                            mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex]
                                                .Dependency);
                                        actionsList.Add (ExternalBankMismatchResyncAction.NoAction);

                                        checkMasterIndex[masterIndex] = true;
                                        checkSwarmopsIndex[swarmopsIndex] = true;

                                        break;
                                    }
                                }
                            }
                        }

                        // STEP 2 - if exactly one record left on both sides, connect and log as mismatching record

                        // TODO: improve logic to handle same number of records left on both sides

                        if (mismatchConstruction.Swarmops.ContainsKey (masterKey) &&
                            mismatchConstruction.Master[masterKey].Cents.Count - checkMasterIndex.Keys.Count == 1 &&
                            mismatchConstruction.Swarmops[masterKey].Cents.Count - checkSwarmopsIndex.Keys.Count == 1)
                        {
                            for (int masterIndex = 0;
                                masterIndex < mismatchConstruction.Master[masterKey].Cents.Count;
                                masterIndex++)
                            {
                                if (checkMasterIndex.ContainsKey (masterIndex))
                                {
                                    continue; // This will fire for all but one indexes
                                }

                                long findMasterCents = mismatchConstruction.Master[masterKey].Cents[masterIndex];

                                for (int swarmopsIndex = 0;
                                    swarmopsIndex < mismatchConstruction.Swarmops[masterKey].Cents.Count;
                                    swarmopsIndex++)
                                {
                                    if (checkSwarmopsIndex.ContainsKey (swarmopsIndex))
                                    {
                                        continue;
                                    }

                                    masterCentsList.Add (findMasterCents);
                                    swarmopsCentsList.Add (mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex]);
                                    dependenciesList.Add (
                                        mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex].Dependency);
                                    transactionsList.Add (
                                        mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex]);
                                    actionsList.Add (ExternalBankMismatchResyncAction.RewriteSwarmops);

                                    checkMasterIndex[masterIndex] = true;
                                    checkSwarmopsIndex[swarmopsIndex] = true;
                                }
                            }
                        }

                        // STEP 3 - log remaining records on both sides as missing counterparts. Only one of these should fire.

                        // STEP 3a - log remaining on Master side

                        if (mismatchConstruction.Master[masterKey].Cents.Count > checkMasterIndex.Keys.Count)
                        {
                            for (int masterIndex = 0;
                                masterIndex < mismatchConstruction.Master[masterKey].Cents.Count;
                                masterIndex++)
                            {
                                if (checkMasterIndex.ContainsKey (masterIndex))
                                {
                                    continue;
                                }

                                masterCentsList.Add (mismatchConstruction.Master[masterKey].Cents[masterIndex]);
                                swarmopsCentsList.Add (0); // null equivalent; invalid value
                                dependenciesList.Add (null);
                                transactionsList.Add (null);
                                actionsList.Add (ExternalBankMismatchResyncAction.CreateSwarmops);

                                checkMasterIndex[masterIndex] = true;
                            }
                        }

                        // STEP 3b - log remaining on Swarmops side

                        if (mismatchConstruction.Swarmops.ContainsKey (masterKey) &&
                            mismatchConstruction.Swarmops[masterKey].Cents.Count > checkSwarmopsIndex.Keys.Count)
                        {
                            for (int swarmopsIndex = 0;
                                swarmopsIndex < mismatchConstruction.Swarmops[masterKey].Cents.Count;
                                swarmopsIndex++)
                            {
                                if (checkSwarmopsIndex.ContainsKey (swarmopsIndex))
                                {
                                    continue;
                                }

                                masterCentsList.Add (0); // null equivalent; invalid value
                                swarmopsCentsList.Add (mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex]);
                                transactionsList.Add (
                                    mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex]);

                                if (mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex].Dependency !=
                                    null)
                                {
                                    dependenciesList.Add (
                                        mismatchConstruction.Swarmops[masterKey].Transactions[swarmopsIndex].Dependency);
                                    actionsList.Add (ExternalBankMismatchResyncAction.ManualAction); // can't auto
                                }
                                else
                                {
                                    dependenciesList.Add (null);
                                    actionsList.Add (ExternalBankMismatchResyncAction.DeleteSwarmops);
                                }

                                checkMasterIndex[swarmopsIndex] = true;
                            }
                        }

                        newRecord.MasterCents = masterCentsList.ToArray();
                        newRecord.SwarmopsCents = swarmopsCentsList.ToArray();
                        newRecord.ResyncActions = actionsList.ToArray();
                        // newRecord.TransactionDependencies = dependenciesList.ToArray();
                        newRecord.Transactions = transactionsList.ToArray();

                        mismatchingRecordList[masterKey] = newRecord;
                    }

                    // Finally, add the transactions that were (described) in Swarmops but not in Master

                    foreach (string swarmopsKey in mismatchConstruction.Swarmops.Keys)
                    {
                        if (!mismatchingRecordList.ContainsKey (swarmopsKey))
                        {
                            mismatchingRecordList[swarmopsKey] = new ExternalBankMismatchingRecordDescription();
                            mismatchingRecordList[swarmopsKey].Description = swarmopsKey;

                            mismatchingRecordList[swarmopsKey].SwarmopsCents =
                                mismatchConstruction.Swarmops[swarmopsKey].Cents.ToArray();
                            mismatchingRecordList[swarmopsKey].Transactions =
                                mismatchConstruction.Swarmops[swarmopsKey].Transactions.ToArray();
                            mismatchingRecordList[swarmopsKey].MasterCents =
                                new long[mismatchConstruction.Swarmops[swarmopsKey].Cents.Count]; // inits to zero

                            mismatchingRecordList[swarmopsKey].ResyncActions =
                                new ExternalBankMismatchResyncAction[
                                    mismatchConstruction.Swarmops[swarmopsKey].Cents.Count];
                            for (int index = 0;
                                index < mismatchingRecordList[swarmopsKey].ResyncActions.Length;
                                index++)
                            {
                                mismatchingRecordList[swarmopsKey].ResyncActions[index] =
                                    ExternalBankMismatchResyncAction.DeleteSwarmops;
                            }
                        }
                    }

                    newMismatch.MismatchingRecords = mismatchingRecordList.Values.ToArray();

                    mismatchList.Add (newMismatch);
                }

                int percentProcessed = (int) (currentRecordIndex*100L/externalData.Records.Length);

                lock (_staticDataLookup)
                {
                    if (percentProcessed > 1)
                    {
                        _staticDataLookup[guid + "PercentRead"] = percentProcessed;
                        // for the progress bar to update async
                    }

                    if (percentProcessed > 99)
                    {
                        // Placed inside loop to have a contiguous lock block, even though it decreases performance.
                        // Should normally be placed just outside.
                        _staticDataLookup[guid + "MismatchArray"] = mismatchList.ToArray();
                        _staticDataLookup[guid + "Account"] = account;
                    }
                }
            }
        }


        public class ExternalBankDataStatistics
        {
            public string FirstTransaction { get; set; }
            public string LastTransaction { get; set; }
            public string TransactionCount { get; set; }
        }


        private class ProcessThreadArguments
        {
            public string Guid { get; set; }
            public Organization Organization { get; set; }
            public FinancialAccount Account { get; set; }
        }

        public class ResyncResults
        {
            public int RecordsTotal { get; set; }
            public int RecordsSuccess { get; set; }
            public int RecordsFail { get; set; }
        }
    }
}