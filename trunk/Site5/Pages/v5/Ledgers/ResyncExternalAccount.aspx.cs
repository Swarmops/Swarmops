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
                    
                    // Load transactions from both sources

                    // Analyze actions


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