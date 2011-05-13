using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Activizr.Logic.Support;

namespace Activizr.Utility.BotCode
{
    public class HeartBeater
    {
        static private HeartBeater instance = null;
        static private object lockObject = new object();

        private bool FlagRestartRequested = false;
        private DateTime lastBeat = DateTime.MinValue;
        private string lastFilename = "";


        public void Beat ()
        {
            Beat(lastFilename);
        }

        public void Beat (string filename)
        {
            lock (lockObject)
            {
                lastFilename = filename;
            }

            if ((DateTime.Now.Subtract(lastBeat).TotalSeconds > 10)
                            && (this.FlagRestartRequested == false))
            {
                try
                {
                    this.lastBeat = DateTime.Now;
                    string nowString = this.lastBeat.ToString();
                    Persistence.Key["PirateBot-L-Heartbeat"] = nowString;

                    if (nowString == Persistence.Key["PirateBot-L-Heartbeat"])
                    {
                        writeFile(filename, DateTime.Now.ToString());
                    }
                    else
                    {
                        this.SuggestRestart();
                        throw new Exception("Failed to update HeartBeat");
                    }
                }
                catch (Exception e)
                {
                    this.SuggestRestart();
                    throw new Exception("Failed to update HeartBeat", e);
                }
            }
        }


        public void SuggestRestart ()
        {
            if (!File.Exists("./piratebotexit.flag"))
                File.Create("./piratebotexit.flag");
            this.FlagRestartRequested = true;
        }

        public bool WasKilled
        {
            get { return (this.FlagRestartRequested == true); }
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
        static public void writeFile (string sPath, string content)
        {
            StreamWriter writeStream = new StreamWriter(new FileStream(sPath, FileMode.Create, FileAccess.ReadWrite), System.Text.Encoding.Default);
            writeStream.Write(content);
            writeStream.Close();
        }


    }
}
