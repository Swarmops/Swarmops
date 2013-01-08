using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Swarmops.Basic.Diagnostics
{
	public class Logging
	{
		static public void LogError(LogSource source, string message)
		{
			EventLog eventLog = new EventLog();
			eventLog.Source = source.ToString();
			eventLog.WriteEntry(message, EventLogEntryType.Error);
		}

		static public void LogException(LogSource source, Exception exception)
		{
			LogError(source, exception.ToString());
		}

		static public void LogWarning(LogSource source, string message)
		{
			EventLog eventLog = new EventLog();
			eventLog.Source = source.ToString();
			eventLog.WriteEntry(message, EventLogEntryType.Warning);
		}

		static public void LogInformation(LogSource source, string message)
		{
			EventLog eventLog = new EventLog();
			eventLog.Source = source.ToString();
			eventLog.WriteEntry(message, EventLogEntryType.Information);
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
