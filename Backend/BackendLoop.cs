using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using NBitcoin.Protocol.Behaviors;
using Newtonsoft.Json.Linq;
using Swarmops.Backend.SocketServices;
using Swarmops.Common.ExtensionMethods;
using Swarmops.Database;
using Swarmops.Logic;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Communications.Resolution;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Utility;
using Swarmops.Utility.BotCode;
using Swarmops.Utility.Communications;
using Swarmops.Utility.Mail;
using WebSocketSharp;
using WebSocketSharp.Server;
using Satoshis=NBitcoin.Money;

namespace Swarmops.Backend
{
    internal class BackendLoop
    {
        private const string heartbeatFile = "/var/run/swarmops/backend/heartbeat.txt";
        private static bool testMode;

        private static void Main (string[] args)
        {
            // Are we running yet?

            if (!SystemSettings.DatabaseInitialized)
            {
                // will restart the service every 15s until db initialized on OOBE
                // also, the read of DatabaseInitialized can and will fail if
                // we're not initalized enough to even have a database 

                throw new InvalidOperationException();
            }

            // Checking for schemata upgrade first of all, after seeing that db exists

            int startupDbVersion = Database.SwarmDb.DbVersion;

            DatabaseMaintenance.UpgradeSchemata();

            if (startupDbVersion < 37)
            {
                // In versions below 37, there was a bug in the PDF rasterization
                // (not in the database, we're just using the dbschema as a versioning
                // tool) that requires re-rasterization of uploaded PDFs. Specifically,
                // transparent-background PDFs were all black (black text on black bg).

                /*
                Console.WriteLine("We need to regenerate all bitmaps from vector PDFs because of a now-fixed bug.");
                Console.WriteLine("This will take a few hours, but only needs to be done once.");

                PdfProcessor.RegenerateAll();*/
            }

            testMode = false;

            SystemSettings.BackendHostname = Dns.GetHostName();


            // Other one-time initializations

            FinancialTransactions.FixAllUnsequenced();
            SupportFunctions.OperatingTopology = OperatingTopology.Backend;

            // Begin main loop

            UnixSignal[] killSignals = null;

            if (!Debugger.IsAttached)
            {
                killSignals = new UnixSignal[] {new UnixSignal (Signum.SIGINT), new UnixSignal (Signum.SIGTERM)};
            }

            BotLog.Write (0, "MainCycle", string.Empty);
            BotLog.Write (0, "MainCycle", "-----------------------------------------------");
            BotLog.Write (0, "MainCycle", string.Empty);

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "test")
                {
/*
                    BotLog.Write(0, "MainCycle", "Running self-tests");
                    HeartBeater.Instance.Beat(heartbeatFile);  // Otherwise Heartbeater.Beat() will fail in various places

                    testMode = true;
                    Console.WriteLine("Testing All Maintenance Processes (except membership-changing ones).");
                    PWLog.Write(PWLogItem.None, 0, PWLogAction.SystemTest, string.Empty, string.Empty);

                    Console.WriteLine("\r\n10-second intervals:");
                    OnEveryTenSeconds();
                    Console.WriteLine("\r\nEvery minute:");
                    OnEveryMinute();
                    Console.WriteLine("\r\nEvery five minutes:");
                    OnEveryFiveMinutes();
                    Console.WriteLine("\r\nEvery hour:");
                    OnEveryHour();
                    Console.WriteLine("\r\nNoon:");
                    OnNoon();
                    Console.WriteLine("\r\nMidnight:");
                    OnMidnight();
                    */

                    Console.WriteLine ("Testing database access...");

                    Console.WriteLine (SwarmDb.GetDatabaseForReading().GetPerson (1).Name);
                    Console.WriteLine (SwarmDb.GetDatabaseForReading().GetPerson (1).PasswordHash);

                    Console.WriteLine ("Creating OutboundComm...");

                    OutboundComm.CreateNotification (null, NotificationResource.System_Startup_Backend);

                    Console.WriteLine ("Transmitting...");

                    OutboundComms comms = OutboundComms.GetOpen();

                    Console.WriteLine ("{0} open items in outbound comms.", comms.Count);

                    foreach (OutboundComm comm in comms)
                    {
                        if (comm.TransmitterClass != "Swarmops.Utility.Communications.CommsTransmitterMail")
                        {
                            throw new NotImplementedException();
                        }

                        ICommsTransmitter transmitter = new CommsTransmitterMail();

                        OutboundCommRecipients recipients = comm.Recipients;
                        PayloadEnvelope envelope = PayloadEnvelope.FromXml (comm.PayloadXml);

                        foreach (OutboundCommRecipient recipient in recipients)
                        {
                            transmitter.Transmit (envelope, recipient.Person);
                        }
                    }


                    Console.Write ("\r\nAll tests run. Waiting for mail queue to flush... ");
                    while (!MailTransmitter.CanExit)
                    {
                        Thread.Sleep (50);
                    }

                    Console.WriteLine ("done.");
                    BotLog.Write (0, "MainCycle", "Exiting self-tests");
                    return;
                }

                if (args[0].ToLower() == "console")
                {
                    Console.WriteLine ("\r\nRunning Swarmops-Backend in CONSOLE mode.\r\n");

                    // -------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------

                    // -----------------------    INSERT ANY ONE-OFF ACTIONS HERE  -------------------------


                    Console.Write ("\r\nWaiting for mail queue to flush... ");

                    while (!MailTransmitter.CanExit)
                    {
                        Thread.Sleep (50);
                    }

                    Console.WriteLine ("done.");

                    return;
                }

                if (args[0].ToLowerInvariant() == "pdfregen")
                {
                    if (args.Length > 1)
                    {
                        int docId = Int32.Parse(args[1]);
                        PdfProcessor.Rerasterize(Document.FromIdentity(docId));
                    }
                    else
                    {
                        Console.WriteLine("Regenerating all bitmaps from PDF uploads.");
                        PdfProcessor.RerasterizeAll();
                        Console.WriteLine("Done.");
                    }

                    return;
                }


                if (args[0].ToLower() == "rsm")
                {
                    Console.WriteLine ("Testing character encoding: räksmörgås RÄKSMÖRGÅS");
                    return;
                }
            }

            /*
            MailMessage message = new MailMessage();
            message.From = new MailAddress(Strings.MailSenderAddress, Strings.MailSenderName);
            message.To.Add (new MailAddress ("rick@piratpartiet.se", "Rick Falkvinge (Piratpartiet)"));
            message.Subject = "Räksmörgåsarnas ékÖNÖMÏåvdëlnïng";
            message.Body = "Hejsan hoppsan Räksmörgåsar.";
            message.BodyEncoding = Encoding.Default;
            message.SubjectEncoding = Encoding.Default;
            
            SmtpClient smtpClient = new SmtpClient ("localhost");
            smtpClient.Credentials = null; // mono bug
            smtpClient.Send (message);*/

            Console.WriteLine (" * Swarmops Backend starting");

            BotLog.Write(0, "MainCycle", "Backend STARTING");

            // Disable certificate checking due to Mono not installing with a certificate repository - this is UTTERLY broken

            SupportFunctions.DisableSslCertificateChecks(); // MONO BUG/MISFEATURE: Mono has no root certificates, so can't verify cert

            // Tell sysop we're starting

            OutboundComm.CreateNotification (null, NotificationResource.System_Startup_Backend);

            // Check for existence of installation ID. If not, create one. Warning: has privacy implications when communicated.

            if (Persistence.Key["SwarmopsInstallationId"] == string.Empty)
            {
                Persistence.Key["SwarmopsInstallationId"] = Guid.NewGuid().ToString();
            }

            // Check for existence of bitcoin hotwallet root

            BitcoinUtility.VerifyBitcoinHotWallet();

            // Initialize backend socket server

            int backendSocketPort = SystemSettings.WebsocketPortBackend;
            _socketServer = new WebSocketServer(backendSocketPort);
            _socketServer.AddWebSocketService<BackendServices>("/Backend");
            _socketServer.Start();

            // Initialize socket client to Blockchain.Info (pending our own services)

            using (
                _blockChainInfoSocket =
                    new WebSocket("wss://ws.blockchain.info/inv?api_code=" + SystemSettings.BlockchainSwarmopsApiKey))
            {

                // Begin maintenance loop

                DateTime cycleStartTime = DateTime.UtcNow;
                DateTime cycleEndTime;

                int lastSecond = cycleStartTime.Second;
                int lastMinute = cycleStartTime.Minute;
                int lastHour = cycleStartTime.Hour;

                bool exitFlag = false;

                _blockChainInfoSocket.OnOpen += new EventHandler(OnBlockchainOpen);
                _blockChainInfoSocket.OnError += new EventHandler<ErrorEventArgs>(OnBlockchainError);
                _blockChainInfoSocket.OnClose += new EventHandler<CloseEventArgs>(OnBlockchainClose);
                _blockChainInfoSocket.OnMessage += new EventHandler<MessageEventArgs>(OnBlockchainMessage);

                _blockChainInfoSocket.Connect();

                while (!exitFlag) // exit is handled by signals handling at end of loop
                {
                    BotLog.Write(0, "MainCycle", "Cycle Start");

                    cycleStartTime = DateTime.UtcNow;
                    cycleEndTime = cycleStartTime.AddSeconds(10);

                    try
                    {
                        OnEveryTenSeconds();

                        if (cycleStartTime.Second < lastSecond)
                        {
                            OnEveryMinute();

                            if (cycleStartTime.Minute%5 == 0)
                            {
                                OnEveryFiveMinutes();
                            }
                        }

                        if (cycleStartTime.Minute < lastMinute)
                        {
                            OnEveryHour();

                            if (DateTime.Now.Hour == 10 && DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                OnTuesdayMorning();
                            }

                            if (DateTime.Now.Hour == 7 && DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                            {
                                OnMondayMorning();
                            }
                        }

                        if (cycleStartTime.Hour >= 12 && lastHour < 12)
                        {
                            OnNoon();
                        }

                        if (cycleStartTime.Hour < lastHour)
                        {
                            OnMidnight();
                        }
                    }

                    catch (Exception e)
                    {
                        // Note each "OnEvery..." catches its own errors and sends Exception mails,
                        // so that failure in one should not stop the others from running. This particular
                        // code should never run.

                        ExceptionMail.Send(new Exception("Failed in swarmops-backend main loop", e), true);
                    }

                    lastSecond = cycleStartTime.Second;
                    lastMinute = cycleStartTime.Minute;
                    lastHour = cycleStartTime.Hour;

                    // Wait for a maximum of ten seconds (the difference between cycleStartTime and cycleEndTime)

                    int iterationCount = 0;
                    DateTime utcNow = DateTime.UtcNow;
                    while (utcNow < cycleEndTime && !exitFlag)
                    {
                        int signalIndex = 250;

                        // Block until a SIGINT or SIGTERM signal is generated, or 1/4 second has passed.
                        // However, we can't do that in a development environment - it won't have the
                        // Mono.Posix assembly, and won't understand UnixSignals. So people running this in
                        // a dev environment will need to stop it manually.

                        if (!Debugger.IsAttached)
                        {
                            signalIndex = UnixSignal.WaitAny(killSignals, 250);
                        }
                        else
                        {
                            TimeSpan timeLeft = (cycleEndTime - utcNow);

                            BotLog.Write(0, "MainCycle Debug",
                                string.Format(CultureInfo.InvariantCulture,
                                    "Waiting for {0:F2} more seconds for cycle end",
                                    timeLeft.TotalMilliseconds/1000.0));
                            Thread.Sleep(250);
                        }

                        if (signalIndex < 250)
                        {
                            exitFlag = true;
                            Console.WriteLine("Caught signal " + killSignals[signalIndex].Signum + ", exiting");
                            BotLog.Write(0, "MainCycle",
                                "EXIT SIGNAL (" + killSignals[signalIndex].Signum + "), terminating backend");
                        }

                        utcNow = DateTime.UtcNow;

                        // Every second, send an internal heartbeat

                        if (iterationCount++%4 == 0)
                        {
                            InternalHeartbeat();
                        }
                    }
                }
            }

            Console.WriteLine (" * Swarmops Backend stopping");
            BotLog.Write (0, "MainCycle", "BACKEND EXITING, sending backend-termination notices");

            /*
            if (HeartBeater.Instance.WasKilled)
            {
                // removed unconditional delete, cron job that restarts bot uses it to know that it is intentionally down.
                ExceptionMail.Send(new Exception("HeartBeater triggered restart of Swarmops Backend. Will commence after 800 seconds."), false);
            }*/

            BotLog.Write (0, "MainCycle", "...done");

            /*
            while (!MailTransmitter.CanExit)
            {
                System.Threading.Thread.Sleep(50);
            }*/

            _socketServer.Stop();

            Thread.Sleep (2000);
        }

        private static void OnEveryTenSeconds()
        {
            try
            {
                BotLog.Write (0, "MainCycle", "Ten-second entry");

                SystemSettings.HeartbeatBackend = (ulong) DateTime.UtcNow.ToUnix();
                CommsTransmitter.Run();

                BroadcastTimestamp();

                try
                {
                    /*TestTrace("Running EventProcessor.Run()...");
                    EventProcessor.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running MailResolver.Run()...");
                    MailResolver.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running MailProcessor.Run()...");
                    MailProcessor.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                BotLog.Write (0, "MainCycle", "Ten-second exit");
            }
            catch (Exception e)
            {
                ExceptionMail.Send (e, true);
            }
        }

        private static void OnEveryMinute()
        {
            try
            {
                BotLog.Write (0, "MainCycle", "One-minute process (empty)");
            }
            catch (Exception e)
            {
                TraceAndReport (e);
            }
            // empty
        }


        private static void OnEveryFiveMinutes()
        {
            try
            {
                BotLog.Write (0, "MainCycle", "Five-minute entry");

                try
                {
                    BotLog.Write(1, "FiveMinute", "Starting automated payout processing");
                    Payouts.PerformAutomated();
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*BotLog.Write(1, "FiveMinute", "Starting newsletter checker");
                    TestTrace("Running NewsletterChecker.Run()...");
                    NewsletterChecker.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*BotLog.Write(1, "FiveMinute", "Starting turnaround tracker");
                    TestTrace("Running TurnaroundTracker.Run()...");
                    TurnaroundTracker.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    // TestTrace(" done.\r\nRunning UpdateStatsCache.Run()...");
                    // UpdateStatsCache.Run();
                    /*TestTrace(" done.\r\nRunning TranslateUrls()...");
                    TranslateUrls();*/

                    // Added during election rush 2010: clean up support database every five minutes instead of once a day
                    // SMS notifications for bounces are turned off, so people don't get SMS notifications in the middle of the night
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*BotLog.Write(1, "FiveMinute", "Closing delay warnings");
                    TestTrace("Running SupportDatabase.CloseDelayWarnings()...");
                    SupportDatabase.CloseDelayWarnings();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running SupportDatabase.NotifyBouncingEmails()...");
                    BotLog.Write(1, "FiveMinute", "Notifying bouncing emails");
                    SupportDatabase.NotifyBouncingEmails();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running SupportMailReview.Run()...");
                    BotLog.Write(1, "FiveMinute", "Running support mail review");
                    SupportMailReview.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }


                BotLog.Write (0, "MainCycle", "Five-minute exit");
            }
            catch (Exception e)
            {
                ExceptionMail.Send (e, true);
                TestTrace (e.ToString());
            }
        }


        private static void OnEveryHour()
        {
            try
            {
                BotLog.Write (0, "MainCycle", "One-hour entry");

                try
                {
                    // This will upgrade the database schema if and only if we failed to do so on entry.

                    if (SwarmDb.DbVersion < SwarmDb.DbVersionExpected)
                    {
                        BotLog.Write (1, "OneHour", "Entering DbUpgrade");
                        DatabaseMaintenance.UpgradeSchemata();
                        BotLog.Write (1, "OneHour", "Exited DbUpgrade");
                    }
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }


                try
                {
                    BotLog.Write(1, "OneHour", "Entering Automatch");
                    Organizations organizations = Organizations.GetAll();
                    foreach (Organization organization in organizations)
                    {
                        if (organization.IsEconomyEnabled)
                        {
                            BotLog.Write(1, "OneHour", "Automatching org #" + organization.Identity.ToString(CultureInfo.InvariantCulture));
                            Payouts.AutomatchAgainstUnbalancedTransactions(organization);
                        }
                    }
                    BotLog.Write(1, "OneHour", "Exited Automatch");

                    /*TestTrace("Running PaymentGroupMapper.Run()...");
                    PaymentGroupMapper.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running BlogTop50Scraper.Run()...");
                    BlogTop50Scraper.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running Mappery.CreatePiratpartietOrganizationStrengthCircuitMap()...");
                    Mappery.CreatePiratpartietOrganizationStrengthCircuitMap();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running Mappery.CreatePiratpartietOrganizationStrengthCityMap()...");
                    Mappery.CreatePiratpartietOrganizationStrengthCityMap();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                BotLog.Write (0, "MainCycle", "One-hour exit");
            }
            catch (Exception e)
            {
                TraceAndReport (e);
            }
        }

        private static void OnMidnight()
        {
            try
            {
                BotLog.Write (0, "MainCycle", "Midnight entry");

                try
                {
                    if (!testMode)
                    {
                        /*TestTrace("Running RosterHousekeeping.ChurnExpiredMembers()...");
                        RosterHousekeeping.ChurnExpiredMembers();
                        TestTrace(" done.\r\n");*/
                    }

                    ExchangeRateSnapshot.Create();
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }
                
                try
                {
                    BotLog.DeleteOld (14); // delete logs older than 14 days
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running SwedishForumMemberCheck.Run()...");
                    SwedishForumMemberCheck.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running SalaryProcessor.Run()...");
                    SalaryProcessor.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running TurnaroundTracker.Housekeeping()...");
                    TurnaroundTracker.Housekeeping();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running Mappery.CreateUngPiratUptakeMap()...");
                    Mappery.CreateUngPiratUptakeMap();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                try
                {
                    /*TestTrace("Running RosterHousekeeping.TimeoutVolunteers()...");
                    RosterHousekeeping.TimeoutVolunteers();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport (e);
                }

                BotLog.Write (0, "MainCycle", "Midnight exit");
            }
            catch (Exception e)
            {
                ExceptionMail.Send (e, true);
                TestTrace (e.ToString());
            }
        }

        private static void OnNoon()
        {
            BotLog.Write (0, "MainCycle", "Noon entry");

            try
            {
                if (!PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE))
                {
                    // PPSE is still running PW4 code for this, so don't run for PPSE

                    Payroll.ProcessMonthly(); // will only actually run on the 1st, but no harm in testing every noon
                }

                // Check all bitcoin accounts for previously-unseen transactions once a day

                Organizations allOrganizations = Organizations.GetAll();
                foreach (Organization organization in allOrganizations)
                {
                    // this actually checks hot storage too, but that's supposed
                    // to be up to date since we're the ones handling it
                    BitcoinUtility.CheckColdStorageForOrganization (organization);
                }

                if (!testMode)
                {
                    /*TestTrace("Running RosterHousekeeping.RemindAllExpiries()...");
                    RosterHousekeeping.RemindAllExpiries();
                    TestTrace(" done.\r\n");*/
                }
            }
            catch (Exception e)
            {
                TraceAndReport (e);
            }

            try
            {
                if (!testMode)
                {
                    /*TestTrace("Running SupportDatabase.NotifyBouncingEmails()...");
                    SupportDatabase.NotifyBouncingEmails();
                    TestTrace(" done.\r\n");*/
                }
            }
            catch (Exception e)
            {
                TraceAndReport (e);
            }

            try
            {
                /*TestTrace("Running SupportDatabase.CloseDelayWarnings()...");
                SupportDatabase.CloseDelayWarnings();
                TestTrace(" done.\r\n");*/
            }
            catch (Exception e)
            {
                TraceAndReport (e);
            }

            try
            {
                /*TestTrace("Running SupportMailReview.Run()...");
                SupportMailReview.Run();
                TestTrace(" done.\r\n");*/
            }
            catch (Exception e)
            {
                TraceAndReport (e);
            }


            BotLog.Write (0, "MainCycle", "Noon exit");
        }

        private static void OnMondayMorning()
        {
            try
            {
                // Detect and log any forex difference exceeding 100 cents.

                FinancialAccounts allAccounts = FinancialAccounts.GetAll(); // across ALL ORGS!

                foreach (FinancialAccount account in allAccounts)
                {
                    // For every account, if it's based on foreign currency, check for forex gains/losses

                    if (account.ForeignCurrency != null)
                    {
                        account.CheckForexProfitLoss();
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionMail.Send(e, true);
                if (testMode)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private static void OnTuesdayMorning()
        {
            try
            {
                //TestTrace("Running ActivityMailer.Run()...");
                // PirateWeb.Utility.BotCode.ActivityMailer.Run();
                //TestTrace(" done.\r\n");
            }
            catch (Exception e)
            {
                ExceptionMail.Send (e, true);
                if (testMode)
                {
                    Console.WriteLine (e.ToString());
                }
            }
        }

        private static void TraceAndReport (Exception e)
        {
            TestTrace (" failed.\r\n");
            ExceptionMail.Send (e, true);
            TestTrace (e.ToString());
        }

        private static void TestTrace (string message)
        {
            if (testMode)
            {
                Console.Write (message);
            }
        }

        private static void InternalHeartbeat()
        {
            JObject json = new JObject();
            json["MessageType"] = "InternalHeartbeat";
            json["Timestamp"] = DateTime.UtcNow.ToUnix();

            _socketServer.WebSocketServices.Broadcast(json.ToString());
        }

        private static void BroadcastTimestamp()
        {
            JObject json = new JObject();
            json["MessageType"] = "Heartbeat";
            json["Timestamp"] = DateTime.UtcNow.ToUnix();

            _socketServer.WebSocketServices.Broadcast(json.ToString());
        }


        public static void ProcessMetapackage(SocketMessage message)
        {
            switch (message.MessageType)
            {
                case "ProfitLossChanged":
                    // Broadcast message to recalculate P&L
                    JObject downstream = new JObject();
                    downstream["MessageType"] = "RecalculateOrganizationProfitLoss";
                    downstream["OrganizationId"] = message.OrganizationId.ToString(CultureInfo.InvariantCulture);
                    Broadcast(downstream);
                    Console.WriteLine(@" - Recalculating P&L for orgId " + message.OrganizationId);
                    break;
                default:
                    // Unhandled. Exception?
                    break;
            }
        }


        private static void Broadcast(JObject json)
        {
            _socketServer.WebSocketServices.Broadcast(json.ToString());
        }

        // ------------------------------- BLOCKCHAIN.INFO WEB SOCKET INTERFACE CODE -------------------------

        public static void OnBlockchainOpen(object sender, EventArgs args)
        {
            Console.WriteLine(" - Socket to Blockchain open and active");
            // TODO: Subscribe to all active addresses
        }

        public static void OnBlockchainError(object sender, ErrorEventArgs args)
        {
            Console.WriteLine(" - ERROR on Blockchain socket: " + args.Message);
        }

        public static void OnBlockchainClose(object sender, CloseEventArgs args)
        {
            Console.WriteLine(" - Socket to Blockchain CLOSED");
        }

        public static void OnBlockchainMessage(object sender, MessageEventArgs args)
        {
            Console.WriteLine(" - Blockchain says: " + args.Data);

            JObject json = JObject.Parse(args.Data);
            string op = (string) json["op"];

            switch (op)
            {
                case "utx":
                    // transaction received on an address we sub to
                    ProcessBitcoinTransaction(json);
                    break;
                default:
                    Console.WriteLine(" -- unknown/unhandled op: '{0}'", op);
                    break;
            }

            // A LOT OF TODO HERE


        }


        internal static void ProcessBitcoinTransaction(JObject blockchainTransaction)
        {
            /* Format:
              
                {
                    "op": "utx",
                    "x": {
                        "lock_time": 0,
                        "ver": 1,
                        "size": 192,
                        "inputs": [
                            {
                                "sequence": 4294967295,
                                "prev_out": {
                                    "spent": true,
                                    "tx_index": 99005468,
                                    "type": 0,
                                    "addr": "1BwGf3z7n2fHk6NoVJNkV32qwyAYsMhkWf",
                                    "value": 65574000,
                                    "n": 0,
                                    "script": "76a91477f4c9ee75e449a74c21a4decfb50519cbc245b388ac"
                                },
                                "script": "483045022100e4ff962c292705f051c2c2fc519fa775a4d8955bce1a3e29884b2785277999ed02200b537ebd22a9f25fbbbcc9113c69c1389400703ef2017d80959ef0f1d685756c012102618e08e0c8fd4c5fe539184a30fe35a2f5fccf7ad62054cad29360d871f8187d"
                            }
                        ],
                        "time": 1440086763,
                        "tx_index": 99006637,
                        "vin_sz": 1,
                        "hash": "0857b9de1884eec314ecf67c040a2657b8e083e1f95e31d0b5ba3d328841fc7f",
                        "vout_sz": 1,
                        "relayed_by": "127.0.0.1",
                        "out": [
                            {
                                "spent": false,
                                "tx_index": 99006637,
                                "type": 0,
                                "addr": "1A828tTnkVFJfSvLCqF42ohZ51ksS3jJgX",
                                "value": 65564000,
                                "n": 0,
                                "script": "76a914640cfdf7b79d94d1c980133e3587bd6053f091f388ac"
                            }
                        ]
                    }
                }
             */

            Console.WriteLine(" - transaction received");

            if (_transactionCache == null)
            {
                _transactionCache = new SerializableDictionary<string, JObject>();
            }

            string txHash = (string) blockchainTransaction["x"]["hash"];
            _transactionCache[txHash] = (JObject)blockchainTransaction["x"];

            foreach (JObject outpoint in blockchainTransaction["x"]["out"])
            {
                Satoshis satoshis = Int64.Parse((string) outpoint["value"]);
                string addressString = (string) outpoint["addr"];

                HotBitcoinAddress hotAddress = null;

                try
                {
                    hotAddress = HotBitcoinAddress.FromAddress(addressString);
                }
                catch (ArgumentException)
                {
                    // Ignore this - it means the addressString isn't ours
                    continue;
                }

                if (hotAddress != null)
                {
                    JObject json = new JObject();
                    json["MessageType"] = "BitcoinReceived";
                    json["Address"] = addressString;
                    json["Hash"] = txHash;

                    Currency currency = hotAddress.Organization.Currency;
                    json["OrganizationId"] = hotAddress.OrganizationId.ToString();
                    json["Currency"] = currency.Code;
                    Swarmops.Logic.Financial.Money organizationCents = new Money(satoshis, Currency.Bitcoin).ToCurrency(currency);
                    json["Satoshis"] = satoshis.ToString();
                    json["Cents"] = organizationCents.Cents.ToString();
                    json["CentsFormatted"] = String.Format("{0:N2}", organizationCents.Cents/100.0);

                    _socketServer.WebSocketServices.Broadcast(json.ToString());

                    // TODO: Examine what address this is, handle accordingly
                }
            }

        }


        public static void AddBitcoinAddress(string address)
        {
            Console.WriteLine(" - request subscribing to address " + address);

            if (_addressLookup == null)
            {
                _addressLookup = new SerializableDictionary<string, bool>();
            }

            if (!_addressLookup.ContainsKey(address))
            {
                Console.WriteLine(" -- subscribed");

                JObject json = new JObject();
                json["op"] = "addr_sub";
                json["addr"] = address;
                _blockChainInfoSocket.Send(json.ToString());

                _addressLookup[address] = true; // dupe prevention
            }

        }


        private static WebSocket _blockChainInfoSocket;
        private static WebSocketServer _socketServer;
        private static SerializableDictionary<string, JObject> _transactionCache; // Need mechanism to clear this over time
        private static SerializableDictionary<string, bool> _addressLookup;
    }
}