using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Mail;

using Swarmops.Basic;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using System.Diagnostics;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Support;
using Swarmops.Utility;
using Swarmops.Utility.BotCode;
using Swarmops.Utility.Financial;
using Swarmops.Utility.Mail;
using Swarmops.Utility.Special;
using Swarmops.Utility.Special.Sweden;

using Mono.Unix;
using Mono.Unix.Native;


namespace Swarmops.Backend
{
    internal class Program
    {
        private const string heartbeatFile = "/var/run/swarmops-backend/heartbeat.txt";

        private static void Main (string[] args)
        {
            testMode = false;

            UnixSignal[] killSignals = new UnixSignal[]{
                new UnixSignal (Signum.SIGINT),
                new UnixSignal (Signum.SIGTERM),
            };

            BotLog.Write(0, "MainCycle", string.Empty);
            BotLog.Write(0, "MainCycle", "-----------------------------------------------");
            BotLog.Write(0, "MainCycle", string.Empty);

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "test")
                {/*
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
                    Console.Write("\r\nAll tests run. Waiting for mail queue to flush... ");
                    while (!MailTransmitter.CanExit)
                    {
                        System.Threading.Thread.Sleep(50);
                    }

                    Console.WriteLine("done.");
                    BotLog.Write(0, "MainCycle", "Exiting self-tests");
                    return;
                }

                if (args[0].ToLower() == "console")
                {
                    Console.WriteLine("\r\nRunning Swarmops-Backend in CONSOLE mode.\r\n");

                    // -------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------

                    // -----------------------    INSERT ANY ONE-OFF ACTIONS HERE  -------------------------

                    
                    Console.Write("\r\nWaiting for mail queue to flush... ");

                    while (!MailTransmitter.CanExit)
                    {
                        System.Threading.Thread.Sleep(50);
                    }

                    Console.WriteLine("done.");

                    return;
                }

                if (args[0].ToLower() == "rsm")
                {
                    Console.WriteLine("Testing character encoding: räksmörgås RÄKSMÖRGÅS");
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

            Console.WriteLine(" * Swarmops Backend starting");

            BotLog.Write(0, "MainCycle", "Backend STARTING");

            DateTime cycleStartTime = DateTime.Now;

            int lastSecond = cycleStartTime.Second;
            int lastMinute = cycleStartTime.Minute;
            int lastHour = cycleStartTime.Hour;

            bool exitFlag = false;

            while (!exitFlag)  // exit is handled by signals handling at end of loop
            {
                BotLog.Write(0, "MainCycle", "Cycle Start");

                cycleStartTime = DateTime.Now;

                try
                {
                    OnEveryTenSeconds();

                    if (cycleStartTime.Second < lastSecond)
                    {
                        OnEveryMinute();

                        if (cycleStartTime.Minute % 5 == 0)
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

                // Wait for a maximum of ten seconds

                while (DateTime.Now < cycleStartTime.AddSeconds(10) && !exitFlag)
                {
                    // block until a SIGINT or SIGTERM signal is generated, or one second has passed.
                    int signalIndex = UnixSignal.WaitAny(killSignals, 1000);

                    if (signalIndex < 1000)
                    {
                        exitFlag = true;
                        Console.WriteLine("Caught signal " + killSignals[signalIndex].Signum.ToString() + ", exiting");
                        BotLog.Write(0, "MainCycle", "EXIT SIGNAL (" + killSignals[signalIndex].Signum.ToString() + ", terminating backend");
                    }
                }
            }

            Console.WriteLine(" * Swarmops Backend stopping");
            BotLog.Write(0, "MainCycle", "BACKEND EXITING, sending backend-termination notices");

            /*
            if (HeartBeater.Instance.WasKilled)
            {
                // removed unconditional delete, cron job that restarts bot uses it to know that it is intentionally down.
                ExceptionMail.Send(new Exception("HeartBeater triggered restart of Swarmops Backend. Will commence after 800 seconds."), false);
            }*/

            BotLog.Write(0, "MainCycle", "...done");
            
            /*
            while (!MailTransmitter.CanExit)
            {
                System.Threading.Thread.Sleep(50);
            }*/

            Thread.Sleep(2000);
        }

        private static void OnEveryTenSeconds ()
        {
            try
            {
                BotLog.Write(0, "MainCycle", "Ten-second entry");

                try
                {
                    /*TestTrace("Running EventProcessor.Run()...");
                    EventProcessor.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running MailResolver.Run()...");
                    MailResolver.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running MailProcessor.Run()...");
                    MailProcessor.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                BotLog.Write(0, "MainCycle", "Ten-second exit");

            }
            catch (Exception e)
            {
                ExceptionMail.Send(e, true);
            }
        }

        private static void OnEveryMinute ()
        {
            try
            {
                BotLog.Write(0, "MainCycle", "One-minute process (empty)");
            }
            catch (Exception e)
            {
                TraceAndReport(e);
            }
            // empty
        }


        private static void OnEveryFiveMinutes ()
        {
            try
            {
                BotLog.Write(0, "MainCycle", "Five-minute entry");

                try
                {
                    /*BotLog.Write(1, "FiveMinute", "Starting press release checker");
                    TestTrace("Running PressReleaseChecker.Run()...");
                    PressReleaseChecker.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
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
                    TraceAndReport(e);
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
                    TraceAndReport(e);
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
                    TraceAndReport(e);
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
                    TraceAndReport(e);
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
                    TraceAndReport(e);
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
                    TraceAndReport(e);
                }


                BotLog.Write(0, "MainCycle", "Five-minute exit");
            }
            catch (Exception e)
            {
                ExceptionMail.Send(e, true);
                TestTrace(e.ToString());
            }
        }


        private static void OnEveryHour ()
        {
            DateTime startTime = DateTime.Now;
            try
            {
                BotLog.Write(0, "MainCycle", "One-hour entry");

                try
                {
                    /*TestTrace("Running BlogWatcher.Run()...");
                    BlogWatcher.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running MediaWatcher.Run()...");
                    MediaWatcher.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }
                
                try
                {
                    /*TestTrace("Running PaymentGroupMapper.Run()...");
                    PaymentGroupMapper.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running BlogTop50Scraper.Run()...");
                    BlogTop50Scraper.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running Mappery.CreatePiratpartietOrganizationStrengthCircuitMap()...");
                    Mappery.CreatePiratpartietOrganizationStrengthCircuitMap();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running Mappery.CreatePiratpartietOrganizationStrengthCityMap()...");
                    Mappery.CreatePiratpartietOrganizationStrengthCityMap();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                BotLog.Write(0, "MainCycle", "One-hour exit");
            }
            catch (Exception e)
            {
                TraceAndReport(e);
            }
        }

        private static void OnMidnight ()
        {
            try
            {
                BotLog.Write(0, "MainCycle", "Midnight entry");

                try
                {
                    if (!testMode)
                    {
                        /*TestTrace("Running RosterHousekeeping.ChurnExpiredMembers()...");
                        RosterHousekeeping.ChurnExpiredMembers();
                        TestTrace(" done.\r\n");*/
                    }
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running InternalPollMaintenance.Run()...");
                    InternalPollMaintenance.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running SwedishForumMemberCheck.Run()...");
                    SwedishForumMemberCheck.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running SalaryProcessor.Run()...");
                    SalaryProcessor.Run();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running TurnaroundTracker.Housekeeping()...");
                    TurnaroundTracker.Housekeeping();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running Mappery.CreateUngPiratUptakeMap()...");
                    Mappery.CreateUngPiratUptakeMap();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                try
                {
                    /*TestTrace("Running RosterHousekeeping.TimeoutVolunteers()...");
                    RosterHousekeeping.TimeoutVolunteers();
                    TestTrace(" done.\r\n");*/
                }
                catch (Exception e)
                {
                    TraceAndReport(e);
                }

                BotLog.Write(0, "MainCycle", "Midnight exit");
            }
            catch (Exception e)
            {
                ExceptionMail.Send(e, true);
                TestTrace(e.ToString());
            }
        }

        private static void OnNoon ()
        {
            BotLog.Write(0, "MainCycle", "Noon entry");

            try
            {
                if (!testMode)
                {
                    /*TestTrace("Running RosterHousekeeping.RemindAllExpiries()...");
                    RosterHousekeeping.RemindAllExpiries();
                    TestTrace(" done.\r\n");*/
                }
            }
            catch (Exception e)
            {
                TraceAndReport(e);
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
                TraceAndReport(e);
            }

            try
            {
                /*TestTrace("Running SupportDatabase.CloseDelayWarnings()...");
                SupportDatabase.CloseDelayWarnings();
                TestTrace(" done.\r\n");*/
            }
            catch (Exception e)
            {
                TraceAndReport(e);
            }

            try
            {
                /*TestTrace("Running SupportMailReview.Run()...");
                SupportMailReview.Run();
                TestTrace(" done.\r\n");*/
            }
            catch (Exception e)
            {
                TraceAndReport(e);
            }


            BotLog.Write(0, "MainCycle", "Noon exit");
        }

        private static void OnTuesdayMorning ()
        {
            try
            {
                //TestTrace("Running ActivityMailer.Run()...");
                // PirateWeb.Utility.BotCode.ActivityMailer.Run();
                //TestTrace(" done.\r\n");
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

        private static void TraceAndReport (Exception e)
        {
            TestTrace(" failed.\r\n");
            ExceptionMail.Send(e, true);
            TestTrace(e.ToString());
        }

        private static void TestTrace (string message)
        {
            if (testMode)
            {
                Console.Write(message);
            }
        }


        private static bool testMode;
    }
}