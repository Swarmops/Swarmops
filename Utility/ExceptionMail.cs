using System;
using Swarmops.Basic;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Utility.Mail;

namespace Swarmops.Utility
{
    public class ExceptionMail
    {
        public static void Send(Exception e)
        {
            Send(e, false);
        }

        public static void Send(Exception e, bool logOnly)
        {
            try
            {
                SwarmDb.GetDatabaseForWriting().CreateExceptionLogEntry(DateTime.UtcNow, "ExceptionMail", e);
            }
            catch
            {
            }

            if (!logOnly)
            {
                new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                    "Swarmops EXCEPTION!",
                    e.ToString(), People.FromIdentities(new[] {1}), false).Send();
            }
        }
    }
}