using System;
using System.IO;
using System.Text;
using Swarmops.Common;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.BotCode
{
    public class HeartBeater
    {
        private static HeartBeater instance;
        private static readonly object lockObject = new object();

        private bool FlagRestartRequested;
        private DateTime lastBeat = Constants.DateTimeLow;
        private string lastFilename = "";

        public bool WasKilled
        {
            get { return this.FlagRestartRequested; }
        }

        public static HeartBeater Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new HeartBeater();
                    }
                }
                return instance;
            }
        }


        /*

        public void Beat()
        {
            Beat (this.lastFilename);
        }

        public void Beat (string filename)
        {
            lock (lockObject)
            {
                this.lastFilename = filename;
            }

            if ((DateTime.Now.Subtract (this.lastBeat).TotalSeconds > 10)
                && (this.FlagRestartRequested == false))
            {
                try
                {
                    this.lastBeat = DateTime.Now;
                    string nowString = this.lastBeat.ToString();
                    Persistence.Key["PirateBot-L-Heartbeat"] = nowString;

                    if (nowString == Persistence.Key["PirateBot-L-Heartbeat"])
                    {
                        writeFile (filename, DateTime.Now.ToString());
                    }
                    else
                    {
                        SuggestRestart();
                        throw new Exception ("Failed to update HeartBeat");
                    }
                }
                catch (Exception e)
                {
                    SuggestRestart();
                    throw new Exception ("Failed to update HeartBeat", e);
                }
            }
        }
        */

        public void SuggestRestart()
        {
            if (!File.Exists ("./piratebotexit.flag"))
                File.Create ("./piratebotexit.flag");
            this.FlagRestartRequested = true;
        }

        public static void writeFile (string sPath, string content)
        {
            StreamWriter writeStream = new StreamWriter (new FileStream (sPath, FileMode.Create, FileAccess.ReadWrite),
                Encoding.Default);
            writeStream.Write (content);
            writeStream.Close();
        }
    }
}