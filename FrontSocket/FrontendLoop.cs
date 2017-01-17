using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Common.ExtensionMethods;
using Swarmops.Logic.Support;
using WebSocketSharp.Server;

namespace Swarmops.Frontend.Socket
{
    internal class FrontendLoop
    {
        static void Main(string[] args)
        {
            // Are we running yet?

            if (!SystemSettings.DatabaseInitialized)
            {
                // will restart the service every 15s until db initialized on OOBE
                // also, the read of DatabaseInitialized can and will fail if
                // we're not initalized enough to even have a database 

                throw new InvalidOperationException(); 
            }

            // Check if we're Sandbox

            if (PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox))
            {
                _isSandbox = true;
            }

            // Disable SSL cert checking (because Mono doesn't have a cert repo, dammit)
            Swarmops.Logic.Support.SupportFunctions.DisableSslCertificateChecks();

            // Initiate main loop

            UnixSignal[] killSignals = null;

            if (!Debugger.IsAttached)
            {
                killSignals = new UnixSignal[] { new UnixSignal(Signum.SIGINT), new UnixSignal(Signum.SIGTERM) };
            }

            Console.WriteLine(" * Swarmops Frontend Socket Server starting up.");

            OutboundComm.CreateNotification(null, NotificationResource.System_Startup_Frontend);

            _socketServer = new WebSocketServer (12172); // TODO: Read from database
            _socketServer.AddWebSocketService<MasterServices> ("/Master");
            // _socketServer.KeepClean = false; // as per the author's recommendation - this may be bad in the long run
            _socketServer.Start();

            DateTime cycleStartTime = DateTime.UtcNow;
            DateTime cycleEndTime;

            int lastSecond = cycleStartTime.Second;
            int lastMinute = cycleStartTime.Minute;
            int lastHour = cycleStartTime.Hour;

            bool exitFlag = false;

            while (!exitFlag) // exit is handled by signals handling at end of loop
            {
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

                        if (cycleStartTime.Minute%30 == 0)
                        {
                            OnEveryHalfHour();
                        }
                    }

                    if (cycleStartTime.Minute < lastMinute)
                    {
                        OnEveryHour();

                        if (DateTime.Now.Hour == 10 && DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            // OnTuesdayMorning();
                        }
                    }

                    if (cycleStartTime.Hour >= 12 && lastHour < 12)
                    {
                        // OnNoon();
                    }

                    if (cycleStartTime.Hour < lastHour)
                    {
                        // OnMidnight();
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                    // Note each "OnEvery..." catches its own errors and sends Exception mails,
                    // so that failure in one should not stop the others from running. This particular
                    // code should never run.

                    // ExceptionMail.Send (new Exception ("Failed in swarmops-backend main loop", e), true);
                }

                lastSecond = cycleStartTime.Second;
                lastMinute = cycleStartTime.Minute;
                lastHour = cycleStartTime.Hour;

                // Wait for a maximum of ten seconds (the difference between cycleStartTime and cycleEndTime)

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
                        Thread.Sleep(250);
                    }

                    if (signalIndex < 250)
                    {
                        exitFlag = true;
                        Console.WriteLine (" * Swarmops Frontend Socket Server caught signal " + killSignals[signalIndex].Signum + ", exiting");
                    }

                    utcNow = DateTime.UtcNow;
                }
            }

            _socketServer.Stop();
            Thread.Sleep (2000);

            Console.WriteLine(" * Swarmops Frontend Socket Server exiting");
        }

        private static WebSocketServer _socketServer;
        private static bool _isSandbox = false;
        private static int _sandboxDummy1 = 500;
        private static int _sandboxDummy2 = 50000;

        private static void OnEveryTenSeconds()
        {
            SystemSettings.HeartbeatFrontend = DateTime.UtcNow.ToUnix();

            _socketServer.WebSocketServices.Broadcast ("{\"messageType\":\"Heartbeat\"}");

            if (_isSandbox)
            {
                _sandboxDummy1 += new Random().Next(10) - 3;
                _sandboxDummy2 += new Random().Next(1000) - 200;

                JObject data = new JObject();
                data["messageType"] = "SandboxUpdate";
                data["local"] = _sandboxDummy1.ToString(CultureInfo.InvariantCulture);
                data["profit"] = _sandboxDummy2.ToString(CultureInfo.InvariantCulture);

                _socketServer.WebSocketServices.Broadcast(data.ToString());
            }
        }


        private static void OnEveryMinute()
        {
            BroadcastTimestamp();
        }


        private static void OnEveryFiveMinutes()
        {
            CheckPriorityPublish();
            CheckPublish();
            CheckStoryAges();
            RecordStoryMetrics();
        }


        private static void OnEveryHalfHour()
        {
        }



        private static void OnEveryHour()
        {
        }


        private static void CheckPriorityPublish()
        {
            /*
            Stories candidateStories = Stories.GetGlobalPublicationQueue (1);

            if (candidateStories.Count > 0 && candidateStories[0].Priority > 50)
            {
                Console.WriteLine("PRIORITY PUBLICATION!");
                candidateStories[0].Publish();
                BroadcastPublication(candidateStories[0]);
                _socketServer.WebSocketServices.Broadcast(GetQueueInfoJson());
            }*/
        }


        private static void CheckPublish()
        {
            /*
            DateTime lastPublishUtc = Stories.GetLastPublished (1)[0].PublishDateTimeUtc;
            int storyCount = Stories.GetGlobalPublicationQueueLength();
            DateTime nowUtc = DateTime.UtcNow;

            if (nowUtc.Hour > 18)
            {
                if (nowUtc.Hour != 19 || nowUtc.Minute != 0)
                {
                    // For all events past 19:01 CET, do not process
                    return;
                }
            }

            if (nowUtc.Hour < 7)
            {
                // For all events before 07:00 CET, do not process
                return;
            }

            DateTime nextPublish = Stories.GetNextPublication (lastPublishUtc, storyCount, storyCount);

            //Console.WriteLine(" * FWN-Internal: Last pub was {0:HH:mm}, next scheduled for {1:HH:mm}, now {2:HH:mm}", lastPublishUtc, nextPublish, nowUtc);

            if (nowUtc > nextPublish)
            {
                PublishStory();
                _socketServer.WebSocketServices.Broadcast(GetQueueInfoJson());
            }*/
        }

        private static void CheckStoryAges()
        {
            /*
            Stories stories = Stories.GetGlobalEditQueue (null);
            DateTime utcNow = DateTime.UtcNow;

            bool storyTimedout = false;

            foreach (Story story in stories)
            {
                double ageMinutes = (utcNow - story.CreatedDateTimeUtc).TotalMinutes;

                // Console.WriteLine("Story #{0:N0} is {1:N0} minutes old ({4:N1} hours), last notify at {2} minutes ({3} hours)", story.Identity, ageMinutes, story.NotifiedAgeMinutes, story.NotifiedAgeMinutes / 60, ageMinutes / 60);

                if (ageMinutes > 48*60) // 48 hours: kill story
                {
                    story.NotifiedAgeMinutes = 48*60;
                    BroadcastEdit(story.Edit (StoryEditType.TimedOut, null, "Story is 48 hours old. Rejected."));
                    story.ChangeState (StoryState.TimedOut);
                    storyTimedout = true;
                }
                else if (ageMinutes > 36*60 && story.NotifiedAgeMinutes < 36*60)
                {
                    story.NotifiedAgeMinutes = 36 * 60;
                    BroadcastEdit(story.Edit (StoryEditType.Warning, null, "Story is 36 hours old, and is about to time out."));
                }
                else if (ageMinutes > 24 * 60 && story.NotifiedAgeMinutes < 24 * 60)
                {
                    story.NotifiedAgeMinutes = 24 * 60;
                    BroadcastEdit(story.Edit(StoryEditType.Warning, null, "Story is 24 hours old. Please finalize and greenlight."));
                }
                else if (ageMinutes > 12 * 60 && story.NotifiedAgeMinutes < 12 * 60)
                {
                    story.NotifiedAgeMinutes = 12 * 60;
                    BroadcastEdit(story.Edit(StoryEditType.Warning, null, "Story is 12 hours old."));
                }
                else if (ageMinutes > 6 * 60 && story.NotifiedAgeMinutes < 6 * 60)
                {
                    story.NotifiedAgeMinutes = 6 * 60;
                    BroadcastEdit(story.Edit(StoryEditType.Warning, null, "Story is six hours old."));
                }
            }

            if (storyTimedout)
            {
                _socketServer.WebSocketServices.Broadcast (GetQueueInfoJson());
            }*/
        }


        private static void BroadcastTimestamp()
        {
            JObject json = new JObject();
            json["messageType"] = "Timestamp";
            json["Timestamp"] = DateTime.UtcNow.ToUnix();

            _socketServer.WebSocketServices.Broadcast (json.ToString());
        }


        /*
        private static void BroadcastEdit (StoryEdit edit, bool markSystem = true)
        {
            
            JObject json = new JObject();

            json["messageType"] = "AddStoryEdit";
            json["EditTimestamp"] = edit.DateTimeUtc.ToUnix();
            json["StoryEditId"] = edit.Identity;
            json["EditType"] = edit.EditType.ToString();
            json["Comment"] = edit.Comment;
            json["PersonIdString"] = markSystem? "System": string.Empty;
            json["StoryId"] = edit.StoryId;

            _socketServer.WebSocketServices.Broadcast(json.ToString());
        }*/




        public static string GetQueueInfoJson()
        {
            throw new NotImplementedException();
            
            /*
            JObject response = new JObject();
            response["messageType"] = "QueueInfo";
            response["EditCount"] = FwnDb.GetDatabaseForReading().GetStoryGlobalEditQueueCount();
            
            int pubStoryCount = FwnDb.GetDatabaseForReading().GetStoryGlobalPublicationQueueCount();
            response["PubCount"] = pubStoryCount;

            if (pubStoryCount > 0)
            {
                response["PubNext"] =
                    Stories.GetNextPublication (Stories.GetLastPublished (1)[0].PublishDateTimeUtc, pubStoryCount,
                        pubStoryCount).ToString ("HH:mm") + " UTC";
                response["PubExtent"] = CountStoryExtent (pubStoryCount);
            }
            else
            {
                response["PubNext"] = "---";
                response["PubExtent"] = "Empty!";
            }

            return response.ToString();*/
        }

        private static string CountStoryExtent (int storyCount)
        {
            throw new NotImplementedException();

            /*
            if (storyCount == 0)
            {
                return "Empty!";
            }

            int pubStoryTotal = FwnDb.GetDatabaseForReading().GetStoryGlobalPublicationQueueCount();
            DateTime lastPublication = Stories.GetLastPublished (1)[0].PublishDateTimeUtc;

            int pubStoryCount = pubStoryTotal;
            while (pubStoryCount > 0)
            {
                lastPublication = Stories.GetNextPublication (lastPublication, pubStoryCount--, pubStoryTotal);
            }

            if (lastPublication.Date == DateTime.UtcNow.Date)
            {
                return lastPublication.ToString ("HH:mm") + " UTC";
            }
            return lastPublication.ToString ("ddd HH:mm") + " UTC";*/
        }


        private static void PublishStory()
        {/*
            int dupeCheckCount = 2;

            Stories justPublishedStories = Stories.GetLastPublished (dupeCheckCount);

            Console.WriteLine ("Publication sequence. Dupecheck: ");

            Stories candidateStories = Stories.GetGlobalPublicationQueue (50);

            foreach (Story candidateStory in candidateStories)
            {
                Console.Write ("Trying story #{0:N0}... ", candidateStory.Identity);

                if (candidateStory.Priority > 49)
                {
                    // Override everything, just get it out pronto

                    Console.Write (" PRIORITY! Publishing...");
                    candidateStory.Publish();
                    BroadcastPublication (candidateStory);
                    Console.WriteLine(" done.");
                    return;
                }

                int candidateGeographyId = candidateStory.GeographyId;
                int candidateTopicId = candidateStory.TopicId;

                bool geoDupe = false;
                bool topicDupe = false;

                foreach (Story justPublished in justPublishedStories)
                {
                    if (justPublished.GeographyId == candidateGeographyId)
                    {
                        Console.Write ("repeating " + justPublished.GeographyName + ", ");
                        geoDupe = true;
                    }
                    if (justPublished.TopicId == candidateTopicId)
                    {
                        Console.Write ("repeating " + justPublished.Topic.Name + ", ");
                        topicDupe = true;
                    }
                }

                if (!geoDupe & !topicDupe)
                {
                    Console.Write ("good! Publishing...");
                    candidateStory.Publish();
                    BroadcastPublication(candidateStory);
                    Console.WriteLine(" done.");
                    return;
                }
                else
                {
                    Console.WriteLine("rejected.");
                }
            }

            Console.Write ("No good candidates.");

            if (candidateStories.Count > 0)
            {
                Console.Write ("Publishing first story in queue anyway (#{0:N0})...", candidateStories[0].Identity);
                candidateStories[0].Publish();
                BroadcastPublication(candidateStories[0]);
                Console.WriteLine(" done.");
            }
            else
            {
                Console.WriteLine (" Nothing in queue at all, actually.");
            }

            return;*/
        }

        /*
        static void BroadcastPublication (Story story)
        {
            JObject json = new JObject();
            json["messageType"] = "StoryPublished";
            json["StoryId"] = story.Identity;

            _socketServer.WebSocketServices.Broadcast (json.ToString());
        }*/


        static void RecordStoryMetrics()
        {/*
            // Get Facebook data for the most recent 25 stories
            // ------------------------------------------------

            Stories stories = Stories.GetLastPublished (25);
            foreach (Story story in stories)
            {
                try
                {
                    story.UpdateFacebookMetrics();
                }
                catch (InvalidDataException)
                {
                    // Ignore if this story wasn't published to Facebook
                }
            }


            // Get Twitter data (Twitter gives us about 200 stories)
            // -----------------------------------------------------

            ProcessStartInfo processInfo = new ProcessStartInfo("/usr/local/bin/twurl", " /1.1/statuses/user_timeline.json?screen_name=FalconwingNews&exclude_replies=true&count=200&include_rts=false");
            processInfo.EnvironmentVariables["HOME"] = "/root";
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;

            int twitterFollowerCount = 0;

            Process tweeting = Process.Start(processInfo);

            string output = tweeting.StandardOutput.ReadToEnd().Trim();
            if (output.Length == 0)
            {
                output = tweeting.StandardError.ReadToEnd().Trim();

                if (output.Length == 0)
                {
                    throw new InvalidDataException("Twurl returned zero data");
                }
            }

            tweeting.WaitForExit();

            JArray tweetArray = null;
            try
            {
                tweetArray = JArray.Parse(output);
            }
            catch (JsonReaderException)
            {
                Console.WriteLine (output);
                File.WriteAllText ("/tmp/JsonReaderException.txt", output);
                
                throw;
            }

            foreach (JObject tweet in tweetArray.Children<JObject>())
            {
                if (twitterFollowerCount == 0)
                {
                    twitterFollowerCount = Int32.Parse ((string) tweet["user"]["followers_count"]);
                }

                Story story = Story.FromOutputChannelToken(1, (string)tweet["id_str"]);
                StoryChannel storyChannel = StoryChannels.FromStoryAndChannel (story, 1); // the 1 is Twitter

                int shareCount = Int32.Parse ((string) tweet["retweet_count"]);
                int starCount = Int32.Parse ((string) tweet["favorite_count"]);

                storyChannel.StoreMetrics (shareCount, starCount, 0);  // we're not getting view count from Twitter here
                int storyAwards = story.AwardCount;
                shareCount = story.Metrics.Shares; // shares TOTAL, this time: sum of all output channels

                if (storyAwards < 1 && shareCount > 9)
                {
                    BroadcastEdit(story.AddEditNote(StoryEditType.StarAward1, null, "Ten shares achieved"), false);
                }
                if (storyAwards < 2 && shareCount > 30)
                {
                    BroadcastEdit(story.AddEditNote(StoryEditType.StarAward2, null, "Thirty shares achieved"), false);
                }
                if (storyAwards < 3 && shareCount > 99)
                {
                    BroadcastEdit(story.AddEditNote(StoryEditType.StarAward3, null, "Three-digit share count achieved"), false);
                }
                if (storyAwards < 4 && shareCount > 310)
                {
                    BroadcastEdit(story.AddEditNote(StoryEditType.StarAward4, null, "Three and a half digits of share count"), false);
                }
                if (storyAwards < 5 && shareCount > 999)
                {
                    BroadcastEdit(story.AddEditNote(StoryEditType.StarAward5, null, "One thousand shares achieved"), false);
                }
                if (storyAwards < 6 && shareCount > 3100)
                {
                    BroadcastEdit(story.AddEditNote(StoryEditType.StarAward6, null, "Four and a half digits of share count"), false);
                }
            }

            StoryChannel.SetFollowerCount (1, twitterFollowerCount);*/
        }
    }
}
