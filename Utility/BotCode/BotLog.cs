using System;
using System.IO;
using System.Text;

namespace Swarmops.Utility.BotCode
{
    public class BotLog
    {
        public static void Write(int generation, string code, string message)
        {
            string line = string.Empty;

            for (int loop = 0; loop < generation; loop++)
            {
                line += "-";
            }

            if (generation > 0)
            {
                line += " ";
            }

            line += string.Format("{0,-16} ", code);

            line += message;

            LogWrite(line);
        }

        private static void LogWrite(string message)
        {
            DateTime now = DateTime.Now;

            string logFileName = "/var/log/swarmops/backend-log-" +
                                 now.ToString("yyyy-MM-dd-HH") + ".log";

            using (StreamWriter writer = new StreamWriter(logFileName, true, Encoding.GetEncoding(1252)))
            {
                writer.WriteLine("[" + now.ToString("HH:mm:ss.fff") + "] " + message);
            }
        }
    }
}