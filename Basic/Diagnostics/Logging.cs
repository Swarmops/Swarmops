using System;
using System.Diagnostics;

namespace Swarmops.Basic.Diagnostics
{
    public class Logging
    {
        public static void LogError (LogSource source, string message)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = source.ToString();
            eventLog.WriteEntry (message, EventLogEntryType.Error);
        }

        public static void LogException (LogSource source, Exception exception)
        {
            LogError (source, exception.ToString());
        }

        public static void LogWarning (LogSource source, string message)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = source.ToString();
            eventLog.WriteEntry (message, EventLogEntryType.Warning);
        }

        public static void LogInformation (LogSource source, string message)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = source.ToString();
            eventLog.WriteEntry (message, EventLogEntryType.Information);
        }
    }

    public enum LogSource
    {
        Unknown = 0,
        PirateDb,
        PirateBot,
        PirateWeb,
        PirateLogic
    }
}