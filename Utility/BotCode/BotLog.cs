using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Swarmops.Utility.BotCode
{
    public class BotLog
    {
        public static void Write (int generation, string code, string message)
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

            line += string.Format ("{0,-16} ", code);

            line += message;

            LogWrite (line);
        }

        private static void LogWrite (string message)
        {
            DateTime now = DateTime.UtcNow;

            string logFileName = "/var/log/swarmops/backend-" +
                                 now.ToString ("yyyy-MM-dd-HH") + ".log";

            if (!Debugger.IsAttached)
            {
                using (StreamWriter writer = new StreamWriter (logFileName, true, Encoding.GetEncoding (1252)))
                {
                    writer.WriteLine ("[" + now.ToString ("HH:mm:ss.fff") + "] " + message);
                }
            }
            else
            {
                // If we're in a development environment, write the log entry to the Output window instead.
                Debug.WriteLine("[" + now.ToString("HH:mm:ss.fff") + "] " + message);
            }
        }

        public static void DeleteOld (int daysToKeep)
        {
            // TODO
        }
    }
}