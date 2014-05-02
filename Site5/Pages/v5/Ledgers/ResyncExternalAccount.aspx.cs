using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class ResyncExternalAccount : PageV5Base
    {
        static ResyncExternalAccount()
        {
            _staticDataLookup = new Dictionary<string, object>();  // This does not work at all for webfarms, which may be a problem down the road
        }

        public static string StorageRoot
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return @"C:\Windows\Temp\\Swarmops-Debug\\"; // Windows debugging environment
                }
                else
                {
                    return "/opt/swarmops/upload/"; // production location on Debian installation  TODO: config file
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-treasure";
            this.PageTitle = Resources.Pages.Ledgers.ResyncExternalAccount_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Ledgers.ResyncExternalAccount_Info;

            if (!Page.IsPostBack)
            {
                this.DropAccounts.Items.Add(new ListItem(Resources.Global.Global_SelectOne, "0"));

                FinancialAccounts accounts = FinancialAccounts.ForOrganization(this.CurrentOrganization,
                                                                               FinancialAccountType.Asset);

                foreach (FinancialAccount account in accounts)
                {
                    // TODO: COnstruct import settings for accounts

                    if (this.CurrentOrganization.Name == "Piratpartiet SE" && account.Identity == 1)
                    {
                        this.DropAccounts.Items.Add(new ListItem(account.Name + " (SE/SEB)", account.Identity.ToString(CultureInfo.InvariantCulture)));
                    }
                }


                Localize();
            }
        }


        private void Localize()
        {
        }



        [WebMethod(EnableSession = true)]
        public static int GetProcessingProgress(string guid)
        {
            // Get data from static variables

            string key = guid + "PercentRead";

            if (!_staticDataLookup.ContainsKey(key))
            {
                return 0;
            }

            lock (_staticDataLookup)
            {
                int percentReady = (int)_staticDataLookup[key];

                if (percentReady >= 100)
                {
                    // copy result from static class-scope variable to session-scope variable
                    HttpContext.Current.Session["LedgersResync" + guid + "MismatchArray"] =
                        _staticDataLookup[guid + "MismatchArray"];

                    HttpContext.Current.Session["LedgersResync" + guid + "Profile"] =
                        _staticDataLookup[guid + "Profile"];

                    _staticDataLookup[guid + "MismatchArray"] = null; // clear the static object, which will otherwise live on
                    _staticDataLookup[guid + "Profile"] = null;
                }

                return percentReady;
            }
        }

        [WebMethod(true)]
        public static ExternalBankDataStatistics GetProcessingStatistics(string guid)
        {
            // Get data from static variables

            string key = guid + "PercentRead";

            if (!_staticDataLookup.ContainsKey(key))
            {
                return null;
            }

            ExternalBankDataStatistics result = new ExternalBankDataStatistics();

            result.FirstTransaction = (string)_staticDataLookup[guid + "FirstTx"];
            result.LastTransaction = (string)_staticDataLookup[guid + "LastTx"];
            result.TransactionCount = (string)_staticDataLookup[guid + "TxCount"];

            return result;
        }





        [WebMethod(true)]
        public static void InitializeProcessing(string guid)
        {
            // Start an async thread that does all the work, then return

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            Thread initThread = new Thread(ProcessUploadThread);

            ProcessThreadArguments args = new ProcessThreadArguments
                                              {Guid = guid, Organization = authData.CurrentOrganization};

            initThread.Start(args);
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
        }


        private static void ProcessUploadThread(object args)
        {
            string guid = ((ProcessThreadArguments) args).Guid;

            Documents documents = Documents.RecentFromDescription(guid);

            if (documents.Count != 1)
            {
                return; // abort
            }

            Document uploadedDoc = documents[0];

            ExternalBankData externalData = new ExternalBankData();
            externalData.Profile = ExternalBankDataProfile.FromIdentity(ExternalBankDataProfile.SESebId); // HACK HACK HACK

            using (StreamReader reader = new StreamReader(StorageRoot + uploadedDoc.ServerFileName, Encoding.GetEncoding(1252)))
            {
                externalData.LoadData(reader, ((ProcessThreadArguments) args).Organization);
            }

            _staticDataLookup[guid + "FirstTx"] = externalData.Records[0].DateTime.ToLongDateString();
            _staticDataLookup[guid + "LastTx"] = 
                externalData.Records[externalData.Records.Length - 1].DateTime.ToLongDateString();
            _staticDataLookup[guid + "TxCount"] = externalData.Records.Length.ToString("N0");

            _staticDataLookup[guid + "Profile"] = externalData.Profile;

            _staticDataLookup[guid + "PercentRead"] = 1;

            int accountId = 1; // TODO: HACK HACK HACK HACK

            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            DateTime timeWalker = externalData.Records[0].DateTime;
            int currentRecordIndex = 0;

            // Walk past the first identical time records, and verify that we have at least one balance record that matches
            // our own for this timestamp. There may be several transactions in the master file, but at least one should have
            // a post-transaction balance that matches our records, or something is much more broken.

            long swarmopsCentsStart = account.GetDeltaCents(DateTime.MinValue, timeWalker.AddSeconds(1));

            // At least one of the transactions for this timestamp should match.

            bool foundMatch = false;

            while (externalData.Records [currentRecordIndex].DateTime == timeWalker)
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
                throw new InvalidOperationException("Unable to locate stable initial conditions for resynchronization");
            }

            // From here on out, every new timestamp should have the exact same delta in Swarmops as it has in the master file.

            List<ExternalBankMismatchingDateTime> mismatchList = new List<ExternalBankMismatchingDateTime>();

            while (currentRecordIndex < externalData.Records.Length)
            {

                DateTime lastTimestamp = timeWalker;

                timeWalker = externalData.Records[currentRecordIndex].DateTime;

                long swarmopsDeltaCents = account.GetDeltaCents(lastTimestamp.AddSeconds(1), timeWalker.AddSeconds(1));
                    // "AddSeconds" because DeltaCents operates on ">= lowbound, < highbound"
                long masterDeltaCents = externalData.Records[currentRecordIndex++].TransactionNetCents;
                int masterTransactionCount = 1;
                int timestampStartIndex = currentRecordIndex;

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

                    for (int innerRecordIndex = timestampStartIndex; innerRecordIndex < currentRecordIndex; innerRecordIndex++)
                    {
                        if (!mismatchConstruction.Master.ContainsKey(externalData.Records[innerRecordIndex].Description))
                        {
                            mismatchConstruction.Master[externalData.Records[innerRecordIndex].Description] =
                                new ExternalBankMismatchingRecordConstruction();
                        }

                        mismatchConstruction.Master[externalData.Records[innerRecordIndex].Description].Cents.Add(externalData.Records[innerRecordIndex].TransactionNetCents);
                        mismatchConstruction.Master[externalData.Records[innerRecordIndex].Description].Dependencies.Add(null); // no dependencies on the master side, only on swarmops side
                    }

                    // Load from Swarmops

                    FinancialAccountRows swarmopsTransactionRows =
                        account.GetRowsFar(lastTimestamp, timeWalker);  // the "select far" is a boundary < x <= boundary selector. Default is boundary <= x < boundary.

                    foreach (FinancialAccountRow swarmopsTransactionRow in swarmopsTransactionRows)
                    {
                        FinancialTransaction swarmopsTransaction = swarmopsTransactionRow.Transaction;

                        if (!mismatchConstruction.Swarmops.ContainsKey(swarmopsTransaction.Description))
                        {
                            mismatchConstruction.Swarmops[swarmopsTransaction.Description] =
                                new ExternalBankMismatchingRecordConstruction();
                        }

                        mismatchConstruction.Swarmops[swarmopsTransaction.Description].Cents.Add(swarmopsTransactionRow.AmountCents);
                        mismatchConstruction.Swarmops[swarmopsTransaction.Description].Dependencies.Add(swarmopsTransaction.Dependency);
                    }

                    // Then, parse the intermediate construction object to the presentation-and-action object.

                    Dictionary <string,ExternalBankMismatchingRecordDescription> mismatchingRecordList = new Dictionary<string, ExternalBankMismatchingRecordDescription>();

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
                        List<ExternalBankMismatchResyncAction> actionsList = new List<ExternalBankMismatchResyncAction>();

                        // STEP 1 - locate all identical matches

                        for (int masterIndex = 0; masterIndex < mismatchConstruction.Master[masterKey].Cents.Count; masterIndex++)
                        {
                            // no "continue" necessary on first run-through; nothing has been checked off yet

                            long findMasterCents = mismatchConstruction.Master[masterKey].Cents[masterIndex];

                            for (int swarmopsIndex = 0; swarmopsIndex < mismatchConstruction.Swarmops[masterKey].Cents.Count; swarmopsIndex++)
                            {
                                if (findMasterCents == mismatchConstruction.Swarmops[masterKey].Cents [swarmopsIndex])
                                {
                                    // There is a match as per case 1. Record both, mark both as used, continue.

                                    masterCentsList.Add(findMasterCents);
                                    swarmopsCentsList.Add(mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex]); // should be equal, we're defensive here
                                    dependenciesList.Add(mismatchConstruction.Swarmops[masterKey].Dependencies[swarmopsIndex]);
                                    actionsList.Add(ExternalBankMismatchResyncAction.NoAction);
                                    
                                    checkMasterIndex[masterIndex] = true;
                                    checkSwarmopsIndex[swarmopsIndex] = true;

                                    break;
                                }
                            }
                        }

                        // STEP 2 - if exactly one record left on both sides, connect and log as mismatching record

                        // TODO: improve logic to handle same number of records left on both sides

                        if (mismatchConstruction.Master[masterKey].Cents.Count - checkMasterIndex.Keys.Count == 1 &&
                            mismatchConstruction.Swarmops [masterKey].Cents.Count - checkSwarmopsIndex.Keys.Count == 1)
                        {
                            for (int masterIndex = 0; masterIndex < mismatchConstruction.Master[masterKey].Cents.Count; masterIndex++)
                            {
                                if (checkMasterIndex.ContainsKey(masterIndex))
                                {
                                    continue; // This will fire for all but one indexes
                                }

                                long findMasterCents = mismatchConstruction.Master[masterKey].Cents[masterIndex];

                                for (int swarmopsIndex = 0; swarmopsIndex < mismatchConstruction.Swarmops[masterKey].Cents.Count; swarmopsIndex++)
                                {
                                    if (checkSwarmopsIndex.ContainsKey(swarmopsIndex))
                                    {
                                        continue;
                                    }

                                    masterCentsList.Add(findMasterCents);
                                    swarmopsCentsList.Add(mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex]);
                                    dependenciesList.Add(mismatchConstruction.Swarmops[masterKey].Dependencies[swarmopsIndex]);
                                    actionsList.Add(ExternalBankMismatchResyncAction.RewriteSwarmops);

                                    checkMasterIndex[masterIndex] = true;
                                    checkSwarmopsIndex[swarmopsIndex] = true;
                                }
                            }
                        }

                        // STEP 3 - log remaining records on both sides as missing counterparts. Only one of these should fire.

                        // STEP 3a - log remaining on Master side

                        if (mismatchConstruction.Master[masterKey].Cents.Count > checkMasterIndex.Keys.Count)
                        {
                            for (int masterIndex = 0; masterIndex < mismatchConstruction.Master[masterKey].Cents.Count; masterIndex++)
                            {
                                if (checkMasterIndex.ContainsKey(masterIndex))
                                {
                                    continue;
                                }

                                masterCentsList.Add(mismatchConstruction.Master[masterKey].Cents[masterIndex]);
                                swarmopsCentsList.Add(0); // null equivalent; invalid value
                                dependenciesList.Add(null);
                                actionsList.Add(ExternalBankMismatchResyncAction.CreateSwarmops);

                                checkMasterIndex[masterIndex] = true;
                            }
                        }

                        // STEP 3b - log remaining on Swarmops side

                        if (mismatchConstruction.Swarmops[masterKey].Cents.Count > checkSwarmopsIndex.Keys.Count)
                        {
                            for (int swarmopsIndex = 0; swarmopsIndex < mismatchConstruction.Swarmops[masterKey].Cents.Count; swarmopsIndex++)
                            {
                                if (checkSwarmopsIndex.ContainsKey(swarmopsIndex))
                                {
                                    continue;
                                }

                                masterCentsList.Add(0); // null equivalent; invalid value
                                swarmopsCentsList.Add(mismatchConstruction.Swarmops[masterKey].Cents[swarmopsIndex]);

                                if (mismatchConstruction.Swarmops[masterKey].Dependencies[swarmopsIndex] != null)
                                {
                                    dependenciesList.Add(
                                        mismatchConstruction.Swarmops[masterKey].Dependencies[swarmopsIndex]);
                                    actionsList.Add(ExternalBankMismatchResyncAction.ManualAction); // can't auto
                                }
                                else
                                {
                                    dependenciesList.Add(null);
                                    actionsList.Add(ExternalBankMismatchResyncAction.DeleteSwarmops);
                                }

                                checkMasterIndex[swarmopsIndex] = true;
                            }
                        }

                        newRecord.MasterCents = masterCentsList.ToArray();
                        newRecord.SwarmopsCents = swarmopsCentsList.ToArray();
                        newRecord.ResyncActions = actionsList.ToArray();
                        newRecord.TransactionDependencies = dependenciesList.ToArray();

                        mismatchingRecordList[masterKey] = newRecord;
                    }

                    foreach (string swarmopsKey in mismatchConstruction.Swarmops.Keys)
                    {
                        if (!mismatchingRecordList.ContainsKey(swarmopsKey))
                        {
                            mismatchingRecordList[swarmopsKey] = new ExternalBankMismatchingRecordDescription();
                            mismatchingRecordList[swarmopsKey].Description = swarmopsKey;
                        }

                        mismatchingRecordList[swarmopsKey].SwarmopsCents =
                            mismatchConstruction.Swarmops[swarmopsKey].Cents.ToArray();
                        mismatchingRecordList[swarmopsKey].TransactionDependencies =
                            mismatchConstruction.Swarmops[swarmopsKey].Dependencies.ToArray();
                    }

                    newMismatch.MismatchingRecords = mismatchingRecordList.Values.ToArray();

                    mismatchList.Add(newMismatch);
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
                    }
                }
            }


        }

        private static Dictionary<string, object> _staticDataLookup;
    }
}